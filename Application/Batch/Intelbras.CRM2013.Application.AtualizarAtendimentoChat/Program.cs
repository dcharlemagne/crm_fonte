using Intelbras.CRM2013.Application.AtualizarAtendimentoChat.Model;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using RestSharp;
using RestSharp.Authenticators;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace Intelbras.CRM2013.Application.AtualizarAtendimentoChat
{
    class Program
    {
        #region Propriedade privadas
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;
        private static FileStream ostrm;
        private static StreamWriter writer;
        private static TextWriter oldOut;
        #endregion

        static void Main()
        {
            Executar();
            LimparLog();
            if (Settings.Default.HabilitarLog == false)
                Console.ReadKey();
        }

        private static void Executar()
        {
            bool reaberta = false;
            //Cria pasta de armazenamento de log caso não exista.
            if (!Directory.Exists(Settings.Default.Log))
            {
                Directory.CreateDirectory(Settings.Default.Log);
            }

            //Cria o arquivo de log da execução
            if (Settings.Default.HabilitarLog)
            {
                ostrm = new FileStream(Settings.Default.Log + @"\Log_" + DateTime.Now.ToString("yyyy-MM-dd-HH.mm") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                oldOut = Console.Out;
                Console.SetOut(writer);
            }

            Console.WriteLine("******************************************************************************");
            Console.WriteLine("                 Serviço - Integração Chat x CRM");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();
            try
            {
                foreach (var item in ListaAtendimentosFinalizados())
                {
                    List<Ocorrencia> lstOcorrencias = new List<Ocorrencia>();
                    try
                    {
                        lstOcorrencias = new OcorrenciaService(OrganizationName, IsOffline).BuscarOcorrenciaPorProtocoloChat(item.protocolNumber);
                        if(lstOcorrencias.Count == 0)
                        {
                            Console.WriteLine("--Atendimento nº " + item.protocolNumber + " não foi encontrado no CRM!");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("--Erro ao buscar no CRM o nº " + item.protocolNumber + ", " + e.Message);
                    }

                    //Registra os dados da ocorrencia
                    foreach (var ocorrencia in lstOcorrencias)
                    {
                        bool ocorrenciaEstavaFechada = (ocorrencia.Status.Value == 1 || ocorrencia.Status.Value == 2); // Cancelada ou Resolvida
                        if (ocorrenciaEstavaFechada)
                        {
                            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ReabrirOcorrencia(ocorrencia);
                            ocorrencia.RazaoStatus = (int)StatusDaOcorrencia.Aberta;
                            ocorrencia.ManterAberto = true;
                            reaberta = true;
                            ocorrencia.Atualizar();
                        }

                        Console.WriteLine("> Ocorrência nº " + ocorrencia.Numero + " encontrada, atendimento nº " + item.protocolNumber);
                        ocorrencia.OrigemChat = item.origin;
                        ocorrencia.DataEntradaChat = Util.Utilitario.ConverterEmData(item.onSystemDate);
                        ocorrencia.DataInicioAtendimento = Util.Utilitario.ConverterEmData(item.contactStartedDate);
                        ocorrencia.DataFinalAtendimento = Util.Utilitario.ConverterEmData(item.contactFinishedDate);
                        ocorrencia.StatusAtendimentoChat = item.contactState;
                        ocorrencia.TempoNaFilaChat = item.customerInQueueTime;
                        ocorrencia.TempoAtendimentoAtivo = item.agentServingTime;
                        ocorrencia.DuracaoAtendimento = item.contactTotalTime;
                        foreach (var formulario in item.formAnswers)
                            ocorrencia.FormularioAtendimento = "Questão: " + formulario.question + "\n" + "Resposta:" + formulario.answer + "\n";
                        foreach (var nota in item.ratings)
                            ocorrencia.NotaPesquisa = Convert.ToInt32(nota.answer);

                        if (Settings.Default.HabilitarLog == false)
                            Console.WriteLine("** Atualizando Ocorrencia **");


                        if (reaberta == true) // Entrou no if de cancelada e resolvida
                        {
                            ocorrencia.ManterAberto = false;
                            reaberta = false;
                            ocorrencia.Atualizar();
                        }

                        //Antes de resolver a ocorrência, precisa preencher a resolução
                        SolucaoOcorrencia solucaoOcorrencia = new SolucaoOcorrencia(OrganizationName, IsOffline)
                        {
                            DataHoraConclusao = DateTime.Now,
                            Nome = "Rotina Histórico do chat",
                            OcorrenciaId = ocorrencia.Id
                        };

                        //Fecha a ocorrência, caso ela já estivesse fechada.
                        if (ocorrenciaEstavaFechada)
                            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.FecharOcorrencia(ocorrencia, solucaoOcorrencia);


                        if (Settings.Default.HabilitarLog == false)
                            Console.WriteLine("** Gravando Data do Atendimento **");

                        if (Settings.Default.HabilitarLog == false)
                            Console.WriteLine("** Registrando Conversa no CRM **");

                        try
                        {
                            RegistrarConversaCRM(item.protocolNumber, ocorrencia);
                        }
                        catch (System.Exception e)
                        {
                            Console.WriteLine("--Atendimento nº " + item.protocolNumber + " não foi possivel atualizar atendimento, " + e.Message + "!");
                        }
                    }
                    GravaDataAtendimento(Util.Utilitario.ConverterEmData(item.onSystemDate).ToString());
                }
            }
            //Entra no catch caso encontre alguma inconsistência na execução de atualização de informações da ocorrencia
            catch (Exception ex)
            {
                SDKore.Helper.Error.Create("AtualizarAtendimentoChat: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                Console.WriteLine("Erro a executar a aplicação");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("******************************************************************************");
                Console.WriteLine("                          Fim da execução");
                if (Settings.Default.HabilitarLog)
                {
                    Console.SetOut(oldOut);
                    writer.Close();
                    ostrm.Close();
                }
            }
        }

        private static void RegistrarConversaCRM(string protocolo, Ocorrencia ocorrencia)
        {
            var detalheAtendimento = ObterConversa(protocolo);

            if (detalheAtendimento != null)
            {
                var entity = new LiveChatTrackingService(OrganizationName, IsOffline).ObterPorOcorrenciaReferenciada(ocorrencia.Id);

                #region Criar novo registro
                if (entity == null)
                {
                    entity = new LiveChatTracking(OrganizationName, IsOffline);
                    Usuario destinatario = new UsuarioService(OrganizationName, IsOffline).ObterPor(ocorrencia.CriadoPor.Id);

                    entity.Contact = new Lookup(ocorrencia.ClienteId.Id, ocorrencia.ClienteId.Name, "contact");
                    entity.Owner = new Lookup(destinatario.Id, destinatario.Nome, "systemuser");
                    entity.ReferenteA = new Lookup(ocorrencia.Id, ocorrencia.Nome, "incident");

                    entity.Subject = detalheAtendimento.NameVisitor + " :: " + detalheAtendimento.EmailVisitor + " :: " + detalheAtendimento.ID + " :: " + detalheAtendimento.PhoneVisitor;
                    entity.NameVisitor = detalheAtendimento.NameVisitor != null ? detalheAtendimento.NameVisitor : String.Empty;
                    entity.IPVisitor = detalheAtendimento.IPVisitor != null ? detalheAtendimento.IPVisitor : String.Empty;
                    entity.PhoneVisitor = detalheAtendimento.PhoneVisitor != null ? detalheAtendimento.PhoneVisitor : String.Empty;
                    entity.DocumentVisitor = detalheAtendimento.CPF != null ? detalheAtendimento.CPF : String.Empty;
                    entity.IPVisitor = detalheAtendimento.IPVisitor != null ? detalheAtendimento.IPVisitor : String.Empty;
                    entity.EmailVisitor = detalheAtendimento.EmailVisitor != null ? detalheAtendimento.EmailVisitor : String.Empty;
                    //entity.Priority = COnjunto opção Será sempre Normal?

                    Guid id = new RepositoryService().LiveChatTracking.Create(entity);

                    if (detalheAtendimento.Dialogo != null)
                        new RepositoryService().Anexo.Create(
                            new Anotacao()
                            {
                                Texto = detalheAtendimento.Dialogo,
                                EntidadeRelacionada = new Lookup(id, "codek_livechat_tracking"),
                                Assunto = "Gravação de Conversa."
                            }
                        );

                    Console.WriteLine("Conversa criada com sucesso!");
                }
                #endregion

                #region Atualizar registro
                else
                {
                    if (entity.StatusLiveChatTracking != StatusLiveChatTracking.Completed)
                        if (entity.StatusLiveChatTracking != StatusLiveChatTracking.Canceled)
                            if (entity.StatusLiveChatTracking != StatusLiveChatTracking.Waiting)
                            {
                                Usuario destinatario = new UsuarioService(OrganizationName, IsOffline).ObterPor(ocorrencia.CriadoPor.Id);

                                entity.Contact = new Lookup(ocorrencia.ClienteId.Id, ocorrencia.ClienteId.Name, "account");
                                entity.Owner = new Lookup(destinatario.Id, destinatario.Nome, "systemuser");
                                entity.ReferenteA = new Lookup(ocorrencia.Id, ocorrencia.Nome, "incident");

                                entity.Subject = detalheAtendimento.NameVisitor + " :: " + detalheAtendimento.EmailVisitor + " :: " + detalheAtendimento.ID + " :: " + detalheAtendimento.PhoneVisitor;
                                entity.NameVisitor = detalheAtendimento.NameVisitor != null ? detalheAtendimento.NameVisitor : String.Empty;
                                entity.IPVisitor = detalheAtendimento.IPVisitor != null ? detalheAtendimento.IPVisitor : String.Empty;
                                entity.PhoneVisitor = detalheAtendimento.PhoneVisitor != null ? detalheAtendimento.PhoneVisitor : String.Empty;
                                entity.DocumentVisitor = detalheAtendimento.CPF != null ? detalheAtendimento.CPF : String.Empty;
                                entity.IPVisitor = detalheAtendimento.IPVisitor != null ? detalheAtendimento.IPVisitor : String.Empty;
                                entity.EmailVisitor = detalheAtendimento.EmailVisitor != null ? detalheAtendimento.EmailVisitor : String.Empty;
                                //entity.Priority = COnjunto opção Será sempre Normal?

                                new RepositoryService().LiveChatTracking.Update(entity, destinatario.Id);

                                Console.WriteLine("Conversa atualizada com sucesso!");
                            }
                }
                #endregion
            }
        }

        private static DetalheAtendimento ObterConversa(string protocolo)
        {
            var dialogo = ObterDadosDaConversa(protocolo);
            if (dialogo != "")
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(dialogo);

                List<String> Description = new List<String>();
                List<String> Agent = new List<String>();
                List<String> MessageSource = new List<String>();

                XmlNode nodeContact = doc.DocumentElement.SelectSingleNode("/rsp/ArrayOfContact/Contact");

                DetalheAtendimento detalheAtendimento = new DetalheAtendimento();

                #region Contact
                foreach (XmlElement xmlElement in nodeContact)
                {
                    if (xmlElement.Name == "ID")
                        detalheAtendimento.ID = xmlElement.InnerText;
                    if (xmlElement.Name == "CheckSumID")
                        detalheAtendimento.ID += "-" + xmlElement.InnerText;
                    if (xmlElement.Name == "StartDate")
                        detalheAtendimento.StartDate = DateTime.Parse(xmlElement.InnerText);
                    if (xmlElement.Name == "FinalDate")
                        detalheAtendimento.FinalDate = DateTime.Parse(xmlElement.InnerText);
                    //Nó ContactDetail 
                    if (xmlElement.Name == "ContactDetail")
                    {
                        var xml = xmlElement.ChildNodes;

                        foreach (XmlElement xmlContactDetail in xml)
                        {
                            if (xmlContactDetail.Name == "q1:CustomerName")
                                detalheAtendimento.NameVisitor = xmlContactDetail.InnerText;
                            if (xmlContactDetail.Name == "q1:Browser")
                                detalheAtendimento.Browser = xmlContactDetail.InnerText;
                            if (xmlContactDetail.Name == "q1:Version")
                                detalheAtendimento.Version = xmlContactDetail.InnerText;
                            if (xmlContactDetail.Name == "q1:Platform")
                                detalheAtendimento.Platform = xmlContactDetail.InnerText;
                            if (xmlContactDetail.Name == "q1:IP")
                                detalheAtendimento.IPVisitor = xmlContactDetail.InnerText;
                            if (xmlContactDetail.Name == "q1:Dialog")
                            {
                                foreach (XmlElement dialog in xmlContactDetail)
                                {
                                    if (dialog.Name == "q1:Messages")
                                    {
                                        XmlNodeList nodeDialog = dialog.ChildNodes;
                                        foreach (XmlElement messages in nodeDialog)
                                        {
                                            if (messages.Name == "q1:Message")
                                            {
                                                XmlNodeList nodeMessage = messages.ChildNodes;
                                                foreach (XmlElement message in nodeMessage)
                                                {
                                                    if (message.Name == "q1:Description")
                                                    {
                                                        Description.Add(message.InnerText.Replace("<![CDATA[", "").Replace("]]>", "").Replace("<br _tmplitem=\"2\"  />", "").Replace("<br />", ""));
                                                    }
                                                    if (message.Name == "q1:Agent")
                                                    {
                                                        foreach (XmlAttribute a in message.Attributes)
                                                        {
                                                            if (a.Name == "Name")
                                                                Agent.Add(a.InnerText);
                                                        }
                                                    }
                                                    if (message.Name == "q1:MessageSource")
                                                    {
                                                        MessageSource.Add(message.InnerText);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //Camada 1 - Surveys
                    if (xmlElement.Name == "Surveys")
                    {
                        #region Propriedade para controle de fluxo das camadas
                        bool _cpf = false;
                        bool _email = false;
                        bool _telefone = false;
                        #endregion
                        XmlNodeList nodeSurveys = xmlElement.ChildNodes;
                        if (nodeSurveys.Count > 0)
                        {
                            foreach (XmlElement element in nodeSurveys)
                            {
                                foreach (XmlElement x in element)
                                {
                                    //Camada 2 - Questions
                                    if (x.Name == "Questions")
                                    {
                                        XmlNodeList nodeQuestions = x.ChildNodes;
                                        if (nodeQuestions.Count > 0)
                                        {
                                            foreach (XmlElement e in nodeQuestions)
                                            {
                                                foreach (XmlElement y in e)
                                                {
                                                    //Camada 3 - Answer
                                                    if (y.Name == "Name")
                                                    {
                                                        if (y.InnerText == "CPF")
                                                            _cpf = true;

                                                        if (y.InnerText == "E-mail")
                                                            _email = true;

                                                        if (y.InnerText == "Telefone")
                                                            _telefone = true;
                                                    }

                                                    if (y.Name == "Answer" && _cpf)
                                                    {
                                                        XmlNodeList nodeAnswer = y.ChildNodes;
                                                        if (nodeAnswer.Count > 0)
                                                        {
                                                            foreach (XmlElement a in nodeAnswer)
                                                            {
                                                                detalheAtendimento.CPF = a.Name == "Text" ? a.InnerText : null;
                                                            }
                                                        }
                                                        _cpf = false;
                                                    }

                                                    if (y.Name == "Answer" && _email)
                                                    {
                                                        XmlNodeList nodeAnswer = y.ChildNodes;
                                                        if (nodeAnswer.Count > 0)
                                                        {
                                                            foreach (XmlElement a in nodeAnswer)
                                                            {
                                                                detalheAtendimento.EmailVisitor = a.Name == "Text" ? a.InnerText : null;
                                                            }
                                                        }
                                                        _email = false;
                                                    }

                                                    if (y.Name == "Answer" && _telefone)
                                                    {
                                                        XmlNodeList nodeAnswer = y.ChildNodes;
                                                        if (nodeAnswer.Count > 0)
                                                        {
                                                            foreach (XmlElement a in nodeAnswer)
                                                            {
                                                                detalheAtendimento.PhoneVisitor = a.Name == "Text" ? a.InnerText : null;
                                                            }
                                                        }
                                                        _telefone = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion 
                int index = 0;
                String _HistoricoConversa = String.Empty;
                foreach (var messageSource in MessageSource)
                {
                    if (messageSource == "AGENT_TO_USER")
                    {
                        _HistoricoConversa += Agent[index] + ":\n";
                        _HistoricoConversa += Description[index] + "\r\n";
                        _HistoricoConversa += "................................................................................................\r\n";
                    }
                    else
                    {
                        _HistoricoConversa += detalheAtendimento.NameVisitor + ":\n";
                        _HistoricoConversa += Description[index] + "\r\n";
                        _HistoricoConversa += "................................................................................................\r\n";
                    }
                    index++;
                }

                detalheAtendimento.Dialogo = _HistoricoConversa;

                return detalheAtendimento;
            }
            return null;
        }

        private static RestClient CriarSessao()
        {
            RestClient ClienteRest;

            ClienteRest = new RestClient();
            ClienteRest.BaseUrl = new Uri(SDKore.Configuration.ConfigurationManager.GetSettingValue("ChatApiEndpoint"));
            ClienteRest.Authenticator = new HttpBasicAuthenticator(SDKore.Configuration.ConfigurationManager.GetSettingValue("ChatUserName"),
                                                                   SDKore.Configuration.ConfigurationManager.GetSettingValue("ChatPassword"));

            ClienteRest.CookieContainer = new System.Net.CookieContainer();

            return ClienteRest;
        }

        private static List<Atendimento> ListaAtendimentosFinalizados()
        {
            #region Parametros
            DateTime dtFim = DateTime.Now;
            DateTime dtInicio = ObterDataAtendimento(dtFim);
            double startDate = Util.Utilitario.ConverterEmFormatoEpoch(dtInicio);

            //Se data fim maior que início mais 30 dias
            if (dtFim > dtInicio.AddDays(15))
            {
                dtFim = dtInicio.AddDays(15);
            }
            double endDate = Util.Utilitario.ConverterEmFormatoEpoch(dtFim);
            int pageNumber;
            int pageSize = 1000;
            int headersTotalPages = 1;
            bool listaPreenchida = false;
            #endregion

            #region Instâncias
            List<Atendimento> listaAtendimentos = new List<Atendimento>();
            RestRequest atendimento = new RestRequest();
            #endregion

            Console.WriteLine(">------Listando atendimentos finalizados");
            Console.WriteLine();
            Console.WriteLine("-> Data inicio: " + dtInicio);
            Console.WriteLine("-> Data fim: " + dtFim.ToString());
            Console.WriteLine();

            for (pageNumber = 1; pageNumber <= headersTotalPages;)
            {
                atendimento.AddHeader("Accept", "application/json");
                atendimento.Resource = "/1.4/info/contacts/";
                atendimento.AddParameter("startDate", startDate);
                atendimento.AddParameter("endDate", endDate);
                atendimento.AddParameter("pageNumber", pageNumber);
                atendimento.AddParameter("pageSize", pageSize);
                atendimento.Method = Method.GET;
                atendimento.RequestFormat = DataFormat.Json;

                var resul = CriarSessao().Execute<List<Atendimento>>(atendimento);

                #region Obtem dados do cabeçalho da resposta
                if (pageNumber == 1)
                    foreach (var headers in resul.Headers)
                    {
                        if (headers.Name == "X-Pagination-TotalPages")
                        {
                            headersTotalPages = Convert.ToInt32(headers.Value);
                            Console.WriteLine("---Total de paginas: " + headersTotalPages);
                            Console.WriteLine();
                        }
                    }
                #endregion

                #region Validar dados
                if (resul.StatusDescription == "OK")
                {
                    if (resul.ErrorMessage == null)
                    {
                        Console.WriteLine("---Pagina: " + pageNumber);
                        Console.WriteLine("---Total de itens: " + resul.Data.Count);
                        Console.WriteLine();

                        foreach (Atendimento item in resul.Data)
                        {
                            if (item.contactState == "FINALIZADO")
                            {
                                listaAtendimentos.Add(item);
                            }
                        }

                        listaPreenchida = true;
                        if (pageSize == resul.Data.Count)
                        {
                            pageNumber++;
                            atendimento = null;
                            atendimento = new RestRequest();
                        }
                        else if (headersTotalPages == pageNumber)
                            break;
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Erro:");
                        Console.WriteLine("- ErrorMessage: " + resul.ErrorMessage);
                        break;
                    }
                }
                else
                if (resul.StatusDescription == "Unauthorized")
                {
                    Console.WriteLine();
                    Console.WriteLine("Erro:");
                    Console.WriteLine("- Token de autenticação inválido!");
                    foreach (Atendimento item in resul.Data)
                    {
                        Console.WriteLine("Message: " + item.message);
                        break;
                    }
                }
                else
                if (resul.StatusDescription == "Forbidden")
                {
                    Console.WriteLine();
                    Console.WriteLine("Erro:");
                    Console.WriteLine("- Usuário e senha válidos na DirectTalk, mas não para acessar a API!");
                    foreach (Atendimento item in resul.Data)
                    {
                        Console.WriteLine("Message: " + item.message);
                        break;
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Erro: " + resul.StatusCode.ToString());
                    Console.WriteLine("Descrição: " + resul.StatusDescription.ToString());
                    Console.WriteLine("Uri enviada: " + resul.ResponseUri.ToString());
                    if (resul.Content != null)
                        Console.WriteLine(resul.Content.ToString());
                    break;
                }
                #endregion
            }

            if (listaPreenchida)
                Console.WriteLine(">------Total de atendimentos finalizados: " + listaAtendimentos.Count);
            Console.WriteLine();

            return listaAtendimentos;
        }

        private static void GravaDataAtendimento(string dataAtendimento)
        {
            StreamWriter file = new StreamWriter(Settings.Default.App + "\\FileDataAtendimento.txt", false, Encoding.ASCII);
            file.WriteLine(dataAtendimento);
            file.Close();
        }

        private static DateTime ObterDataAtendimento(DateTime dataAtendimento)
        {
            string path = Settings.Default.App + "\\FileDataAtendimento.txt";
            dataAtendimento = DateTime.Now.AddMinutes(-5);

            //Verifica se arquivo existe e cria um novo
            if (!System.IO.File.Exists(path))
            {
                using (System.IO.StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(dataAtendimento);
                    sw.Close();
                }
            }

            //Obtém data armazenada no arquivo
            using (StreamReader file = new StreamReader(path))
            {
                String linha;
                // Lê linha por linha até o final do arquivo
                while ((linha = file.ReadLine()) != null)
                {
                    dataAtendimento = DateTime.Parse(linha.ToString());
                }
                file.Close();
            }

            return dataAtendimento;
        }

        private static string ObterDadosDaConversa(string protocolo)
        {
            protocolo = protocolo.Remove(protocolo.Length - 3);
            string responseFromServer = string.Empty;

            String postData = "Token=" + SDKore.Configuration.ConfigurationManager.GetSettingValue("ChatToken") +
                "&Fields=%3CFields%3E%0D%0A++++%3CAgents%3Etrue%3C%2FAgents%3E%0D%0A++++%3CCustomers%3Etrue%3C%2FCustomers%3E%0D%0A++++%3CClassifications%3Etrue%3C%2FClassifications%3E%0D%0A++++%3CClassification%3E%0D%0A++++++++%3CForms%3Etrue%3C%2FForms%3E%0D%0A++++++++%3CProcesses%3Etrue%3C%2FProcesses%3E%0D%0A++++%3C%2FClassification%3E%0D%0A++++%3CSurvey%3E%0D%0A+++++++%3CInitial%3Etrue%3C%2FInitial%3E%0D%0A++++++++%3CContact%3Etrue%3C%2FContact%3E%0D%0A++++++++%3CFinal%3Etrue%3C%2FFinal%3E%0D%0A++++%3C%2FSurvey%3E%0D%0A++++%3CContactDetail%3E%0D%0A++++++++%3CChat%3Etrue%3C%2FChat%3E%0D%0A++++++++%3CMail%3Efalse%3C%2FMail%3E%0D%0A++++++++%3CPhone%3Efalse%3C%2FPhone%3E%0D%0A++++++++%3CChatDetails%3E%0D%0A+++++++++++%3CDialog%3Etrue%3C%2FDialog%3E%0D%0A++++++++%3C%2FChatDetails%3E%0D%0A++++++++%3CMailDetails%3E%0D%0A++++++++++++%3CBody%3Etrue%3C%2FBody%3E%0D%0A++++++++++++%3CAttachments%3Etrue%3C%2FAttachments%3E%0D%0A++++++++%3C%2FMailDetails%3E%0D%0A++++++++%3CPhoneDetails%3E%0D%0A++++++++%3C%2FPhoneDetails%3E%0D%0A++++%3C%2FContactDetail%3E%0D%0A%3C%2FFields%3E" +
                "&ContactFilter=%3CContactFilter%3E%0D%0A++++%3CStartID%3E" + protocolo + "%3C%2FStartID%3E%0D%0A++++%3CQuantity%3E1%3C%2FQuantity%3E%0D%0A%3C%2FContactFilter%3E";

            WebRequest request = WebRequest.Create(SDKore.Configuration.ConfigurationManager.GetSettingValue("ChatGetContactsEndpoint"));

            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            System.Net.ServicePointManager.Expect100Continue = false;
            Stream dataStream = request.GetRequestStream();

            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription + " - Conversa obtida com sucesso");
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            responseFromServer = reader.ReadToEnd();
            // Display the content.

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        private static void LimparLog()
        {
            // Obtém todos os ficheiros da pasta
            var files = Directory.GetFiles(Settings.Default.Log);
            if (files.Length != 0)
            {
                foreach (var f in files)
                {
                    // Obtém os atributos do ficheiro
                    var attr = File.GetAttributes(f);

                    if (File.GetCreationTime(f) < DateTime.Now.AddDays(-3))
                    {
                        // O ficheiro é 'read-only'?
                        if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            // Sim... Remove o atributo 'read-only' do ficheiro
                            File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
                        }
                        // Apaga o ficheiro
                        File.Delete(f);
                    }
                }
            }
        }
    }
}