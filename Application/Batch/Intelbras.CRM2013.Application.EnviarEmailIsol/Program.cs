using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            List<Ocorrencia> lstOcorrencias = new Intelbras.CRM2013.Domain.Servicos.OcorrenciaService(organizationName, false).ListarOcorrenciasPorDataModificacao();

            foreach (Ocorrencia ocorrencia in lstOcorrencias)
            {
                try
                {
                    Boolean enviarEmail = false;
                    //Listar os e-mails da ocorrência corrente
                    Email obterEmail = new EmailService(organizationName, false).ObterDataUltimoEmailEnviado(ocorrencia.Id);

                    if (obterEmail != null)
                    {
                        //Só enviar novamente e-mail, se a data do último enviado for maior que 5 dias;
                        if (obterEmail.ModificadoEm < DateTime.Now.AddDays(-5))
                            enviarEmail = true;
                    }

                    if (obterEmail == null || (obterEmail != null & enviarEmail == true))
                    {
                        var email = new Email(organizationName, false);
                        Guid fromUserId;

                        if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail", true), out fromUserId))
                        {
                            throw new ArgumentException("(CRM) O parâmetro 'Intelbras.Usuario.EnvioEmail' não foi possível converter em GUID.");
                        }
                        //buscar o e-mail do proprietário da ocorrência
                        Usuario proprietario = new RepositoryService().Usuario.BuscarProprietario("incident", "incidentid", ocorrencia.Id);
                        if (proprietario != null)
                        {
                            email.ParaEnderecos = proprietario.EmailPrimario;
                        }

                        email.Assunto = "ALERTA ISOL – " + ocorrencia.Numero + " , " + ocorrencia.Nome;
                        email.De = new Lookup[1];
                        email.De[0] = new Lookup(fromUserId, "systemuser");
                        email.ReferenteA = new Lookup(ocorrencia.Id, ocorrencia.Nome, "incident");
                        string linkOcorrencia = string.Format("<a href=\"{0}\">Clique Aqui</a>", SDKore.Configuration.ConfigurationManager.GetSettingValue("LINK_QRCODE_OS") + "etn=incident&id=" + ocorrencia.Id + "&pagetype=entityrecord");

                        var NomeCliente = "";
                        if (ocorrencia.Cliente == null)
                            NomeCliente = ocorrencia.ClienteId.Name;
                        else
                            NomeCliente = ocorrencia.Cliente.Nome;

                        email.Mensagem = string.Format("Prezado(a), <br /><br />" +
                                                         " Favor verificar a Ordem de Serviço <b>" + ocorrencia.Numero + "</b> para atualização do andamento ou encerramento da mesma. <br /><br />" +
                                                         " <b>Cliente:</b> " + NomeCliente + "<br />" +
                                                         " <b>Tipo de Ocorrência:</b> " + ocorrencia.Tipo + " <br />" +
                                                         " <b>Prioridade:</b> " + ocorrencia.Prioridade + " <br />" +
                                                         " <b>Status:</b> " + ocorrencia.StatusDaOcorrencia + " <br />" +
                                                         " <b>Tempo SLA:</b> " + ocorrencia.DataSLA + " <br />" +
                                                         " <b>Última atualização:</b> " + ocorrencia.ModificadoEm + " <br /><br />" +
                                                         " Para maiores detalhes, " + linkOcorrencia + " para acessar a Ordem de Serviço. "
                                                       );

                        new RepositoryService().Email.EnviaEmailParaDestinatariosNaoCRM(email);//Envia o e-mail
                    }
                }
                catch (Exception ex) //Entra no catch caso encontre alguma inconsistência no update ou no envio de e-mail
                {
                    string Log = "(CRM) Erro ao enviar E-mail -" + ex.Message;
                }
            }
        }
    }
}
