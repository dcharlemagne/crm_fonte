using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Xml;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Intelbras.CRM2013.Domain.Enum;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.WebServices.ASTEC
{
    /// <summary>
    /// Summary description for IntegradorASTEC
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class IntegradorASTEC : WebServiceBase
    {
        private string PathDownloadNotaFiscal { get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("path_download_notafiscal_astec"); } }
        

        [WebMethod]
        public XmlDocument ObterOS(string NumeroDaOS, string LoginDoPostoAutorizado, string SenhaDoPostoAutorizado)
        {
            StringBuilder xml = new StringBuilder("<Ocorrencia>"); // "<?xml version=\"1.0\" encoding=\"utf-8\" ?><Intelbras xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.microsoft.com/crm/2009/WebServices\">";

            try
            {
                if (!VerificaAcessoDoPosto(LoginDoPostoAutorizado, SenhaDoPostoAutorizado))
                {
                    Exception AcessoNegado = new Exception("ACESSO NEGADO");
                    throw AcessoNegado;
                }

                
                Ocorrencia ocorrencia = new Ocorrencia(nomeOrganizacao, false);
                ocorrencia = ocorrencia.ObterOcorrenciaPor(NumeroDaOS);

                if (ocorrencia != null)
                {
                    xml.AppendLine("<Numero>" + ocorrencia.Numero + "</Numero>");
                    xml.AppendLine("<StatusOcorrencia>" + ocorrencia.Status + "</StatusOcorrencia>");
                    xml.AppendLine("<DataDeAberturaInformada>" + (ocorrencia.DataOrigem.HasValue ? ocorrencia.DataOrigem.Value.Date.ToShortDateString() : "") + "</DataDeAberturaInformada>");
                    xml.AppendLine("<DataDeAberturaDigitada>" + (ocorrencia.DataDeAberturaDigitada.HasValue ? ocorrencia.DataDeAberturaDigitada.Value.Date.ToShortDateString() : "") + "</DataDeAberturaDigitada>");
                    xml.AppendLine("<DataDeConsertoInformada>" + (ocorrencia.DataDeConsertoInformada.HasValue ? ocorrencia.DataDeConsertoInformada.Value.Date.ToShortDateString() : "") + "</DataDeConsertoInformada>");
                    xml.AppendLine("<DataDeConsertoDigitada>" + (ocorrencia.DataDeConsertoDigitada.HasValue ? ocorrencia.DataDeConsertoDigitada.Value.Date.ToShortDateString() : "") + "</DataDeConsertoDigitada>");
                    xml.AppendLine("<DataDeEntregaClienteInformada>" + (ocorrencia.DataDeEntregaClienteInformada.HasValue ? ocorrencia.DataDeEntregaClienteInformada.Value.Date.ToShortDateString() : "") + "</DataDeEntregaClienteInformada>");
                    xml.AppendLine("<DataDeEntregaClienteDigitada>" + (ocorrencia.DataDeEntregaClienteDigitada.HasValue ? ocorrencia.DataDeEntregaClienteDigitada.Value.Date.ToShortDateString() : "") + "</DataDeEntregaClienteDigitada>");
                    xml.AppendLine("<ValorServico>" + (ocorrencia.ValorServico.HasValue ? ocorrencia.ValorServico.Value.ToString("N") : "") + "</ValorServico>");
                    xml.AppendLine("<RetiradoPorCPF>" + ocorrencia.RetiradoPorCPF + "</RetiradoPorCPF>");
                    xml.AppendLine("<RetiradoPorNome>" + ocorrencia.RetiradoPorNome + "</RetiradoPorNome>");
                    xml.AppendLine("<DefeitoAlegado>" + ocorrencia.DefeitoAlegado + "</DefeitoAlegado>");
                    xml.AppendLine("<AcessoriosOpcionais>" + ocorrencia.AcessoriosOpcionais + "</AcessoriosOpcionais>");
                    xml.AppendLine("<AparenciaDoProduto>" + ocorrencia.AparenciaDoProduto + "</AparenciaDoProduto>");
                    xml.AppendLine("<SolicitantePortal>" + ocorrencia.SolicitantePortal + "</SolicitantePortal>");
                    xml.AppendLine("<EmIntervencaoTecnica>" + ocorrencia.EmIntervencaoTecnica.Value + "</EmIntervencaoTecnica>");
                    xml.AppendLine("<Observacoes>" + ocorrencia.DescricaoDaMensagemDeIntegracao + "</Observacoes>");

                    xml.AppendLine("<ProdutoPricipal>");

                    if (ocorrencia.Produto != null)
                    {
                        xml.AppendLine("<Codigo>" + ocorrencia.Produto.Codigo + "</Codigo>");
                        xml.AppendLine("<Nome>" + ocorrencia.Produto.Nome + "</Nome>");
                        xml.AppendLine("<NumeroDeSerie>" + ocorrencia.ProdutosDoCliente + "</NumeroDeSerie>");
                    }

                    xml.AppendLine("</ProdutoPricipal>");

                    xml.AppendLine("<Cliente>");

                    if (ocorrencia.ClienteId != null && ocorrencia.ClienteId.Type == "account")
                    {
                        xml.AppendLine("<Codigo>" + ocorrencia.Cliente.CodigoMatriz + "</Codigo>");
                        xml.AppendLine("<CPFouCNPJ>" + ocorrencia.Cliente.CpfCnpj + "</CPFouCNPJ>"); //ou vai estar preenchido um ou o outro
                        xml.AppendLine("<Nome>" + ocorrencia.Cliente.Nome + "</Nome>");
                        xml.AppendLine("<Email>" + ocorrencia.Cliente.Email + "</Email>");
                        xml.AppendLine("<TelefonePrincipal>" + ocorrencia.Cliente.Telefone + "</TelefonePrincipal>");
                        xml.AppendLine("<Endereco>");
                        xml.AppendLine("<Logradouro>" + ocorrencia.Cliente.Endereco1Rua + "</Logradouro>");
                        xml.AppendLine("<Numero>" + ocorrencia.Cliente.Endereco1Numero + "</Numero>");
                        xml.AppendLine("<Complemento>" + ocorrencia.Cliente.Endereco1Complemento + "</Complemento>");
                        xml.AppendLine("<Cep>" + ocorrencia.Cliente.Endereco1CEP + "</Cep>");
                        xml.AppendLine("<Bairro>" + ocorrencia.Cliente.Endereco1Bairro + "</Bairro>");
                        xml.AppendLine("<Cidade>" + (ocorrencia.Cliente.Endereco1Municipioid != null ? ocorrencia.Cliente.Endereco1Municipioid.Name : "") + "</Cidade>");
                        xml.AppendLine("<Uf>" + (ocorrencia.Cliente.Endereco1Estadoid != null ? ocorrencia.Cliente.Endereco1Estadoid.Name : "") + "</Uf>");
                        xml.AppendLine("</Endereco>");
                    }
                    else if (ocorrencia.ClienteId != null && ocorrencia.ClienteId.Type == "contact")
                    {
                        xml.AppendLine("<Codigo>" + ocorrencia.ClienteOS.CodigoRemetente + "</Codigo>");
                        xml.AppendLine("<CPFouCNPJ>" + ocorrencia.ClienteOS.CpfCnpj + "</CPFouCNPJ>");
                        xml.AppendLine("<Nome>" + ocorrencia.ClienteOS.Nome + "</Nome>");
                        xml.AppendLine("<Email>" + ocorrencia.ClienteOS.Email1 + "</Email>");
                        xml.AppendLine("<TelefonePrincipal>" + ocorrencia.ClienteOS.TelefoneComercial + "</TelefonePrincipal>");
                        xml.AppendLine("<Endereco>");
                        xml.AppendLine("<Logradouro>" + ocorrencia.ClienteOS.Endereco1Rua + "</Logradouro>");
                        xml.AppendLine("<Numero>" + ocorrencia.ClienteOS.Endereco1Numero + "</Numero>");
                        xml.AppendLine("<Complemento>" + ocorrencia.ClienteOS.Endereco1Complemento + "</Complemento>");
                        xml.AppendLine("<Cep>" + ocorrencia.ClienteOS.Endereco1CEP + "</Cep>");
                        xml.AppendLine("<Bairro>" + ocorrencia.ClienteOS.Endereco1Bairro + "</Bairro>");
                        xml.AppendLine("<Cidade>" + (ocorrencia.ClienteOS.Endereco1Municipioid != null ? ocorrencia.ClienteOS.Endereco1Municipioid.Name : "") + "</Cidade>");
                        xml.AppendLine("<Uf>" + (ocorrencia.ClienteOS.Endereco1Estadoid != null ? ocorrencia.ClienteOS.Endereco1Estadoid.Name : "") + "</Uf>");
                        xml.AppendLine("</Endereco>");
                    }
                    xml.AppendLine("</Cliente>");

                    xml.AppendLine("<NotaFiscalFatura>");
                        xml.AppendLine("<DataDeCompra>" + (ocorrencia.DataConstadoNotaFiscalDeCompra.HasValue ? ocorrencia.DataConstadoNotaFiscalDeCompra.Value.ToShortDateString() : "") + "</DataDeCompra>");
                        xml.AppendLine("<NumeroNF>" + ocorrencia.NumeroNotaFiscalDeCompra + "</NumeroNF>");
                        xml.AppendLine("<NomeClienteNF>" + ocorrencia.NomeConstadoNaNotaFiscalDeCompra + "</NomeClienteNF>");
                        xml.AppendLine("<CPFCNPJClienteNF>" + ocorrencia.CpfCnpjConstadoNaNotaFiscalDeCompra + "</CPFCNPJClienteNF>");
                        xml.AppendLine("<Revenda>");
                        xml.AppendLine("<Cnpj>" + ocorrencia.CnpjDaLojaDoAtendimento + "</Cnpj>");
                        xml.AppendLine("<RazaoSocial>" + ocorrencia.NomeDaLojaDoAtendimento + "</RazaoSocial>");
                        xml.AppendLine("<TelefonePrincipal>" + ocorrencia.TelefoneDaLojaDoAtendimento + "</TelefonePrincipal>");
                        xml.AppendLine("</Revenda>");
                    xml.AppendLine("</NotaFiscalFatura>");

                    xml.AppendLine("<PostoDeServico>");
                    if (ocorrencia.Autorizada != null)
                    {
                        xml.AppendLine("<CodigoPosto>" + ocorrencia.Autorizada.CodigoMatriz + "</CodigoPosto>");
                        xml.AppendLine("<Login>" + ocorrencia.SolicitantePortal + "</Login>");
                    }
                    xml.AppendLine("</PostoDeServico>");

                    if (ocorrencia.Diagnosticos != null && ocorrencia.Diagnosticos.Count > 0)
                    {
                        for (int x = 0; x < ocorrencia.Diagnosticos.Count; x++)
                        {
                            xml.AppendLine("<ServicosExecutados>");
                            xml.AppendLine("<Status>" + (Domain.Enum.StatusDoDiagnostico)ocorrencia.Diagnosticos[x].RazaoStatus + "</Status>");
                            xml.AppendLine("<Defeito>" + (ocorrencia.Diagnosticos[x].DefeitoId != null ? ocorrencia.Diagnosticos[x].DefeitoId.Name : "") + "</Defeito>");
                            xml.AppendLine("<Solucao>" + (ocorrencia.Diagnosticos[x].SolucaoId != null ? ocorrencia.Diagnosticos[x].SolucaoId.Name : "") + "</Solucao>");
                            xml.AppendLine("<QtdeSolicitada>" + (ocorrencia.Diagnosticos[x].QuantidadeSolicitada.HasValue ? ocorrencia.Diagnosticos[x].QuantidadeSolicitada.Value.ToString() : "") + "</QtdeSolicitada>");
                            xml.AppendLine("<DataPedidoEmitidoERP>" + (ocorrencia.Diagnosticos[x].DataPedidoEmitidoERP.HasValue ? ocorrencia.Diagnosticos[x].DataPedidoEmitidoERP.Value.Date.ToShortDateString() : "") + "</DataPedidoEmitidoERP>");
                            xml.AppendLine("<DataFaturamentoERP>" + (ocorrencia.Diagnosticos[x].DataFaturamentoERP.HasValue ? ocorrencia.Diagnosticos[x].DataFaturamentoERP.Value.Date.ToShortDateString() : "") + "</DataFaturamentoERP>");
                            xml.AppendLine("<NumeroNotaFiscal>" + ocorrencia.Diagnosticos[x].NumeroNotaFiscal + "</NumeroNotaFiscal>");
                            xml.AppendLine("<SerieNotaFiscal>" + ocorrencia.Diagnosticos[x].SerieNotaFiscal + "</SerieNotaFiscal>");
                            xml.AppendLine("<Produto>" + ocorrencia.Diagnosticos[x].Produto.Codigo + "</Produto>");
                            xml.AppendLine("<Item>" + ocorrencia.Diagnosticos[x].Produto.Nome + "</Item>");
                            xml.AppendLine("</ServicosExecutados>");
                        }
                    }
                    else
                    {
                        xml.AppendLine("<ServicosExecutados></ServicosExecutados>");
                    }
                }
                else
                {
                    //base.Mensageiro.AdicionarTopico("Achou", false);
                }

            }
            catch (Exception erro)
            {
                //return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(), erro, "CalcularDiferencaEntreDatas");
                xml.AppendLine("<ERRO>Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString() + "</ERRO>");
            }

            xml.AppendLine("</Ocorrencia>");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.ToString());
            return doc;
        }

        [WebMethod]
        public string CriarOS(string XMLDadosDaOS, string LoginDoPostoAutorizado, string SenhaDoPostoAutorizado)
        {
            DateTime dataInicioDoProcesso = DateTime.Now;
            string mensagem = "", logAdicional = "";
            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                try { xmlDocument.LoadXml(XMLDadosDaOS); }
                catch (Exception ex) { throw new ArgumentException("Não foi possível ler o XML!", ex); }

                if (!VerificaAcessoDoPosto(LoginDoPostoAutorizado, SenhaDoPostoAutorizado))
                {
                    throw new ArgumentException(String.Format("Acesso Negado! Login {0} sem acesso ao sistema", LoginDoPostoAutorizado));
                }

                CRM2013.Domain.Model.Conta assistenciaTecnica = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterPorLogin(LoginDoPostoAutorizado);

                if (assistenciaTecnica == null)
                {
                    throw new ArgumentException("Não encontrado a Autorizada para o login: " + LoginDoPostoAutorizado);
                }

                if (!assistenciaTecnica.AcessoPortalASTEC.Value)
                {
                    throw new ArgumentException("Assistência Técnica não tem permissão!");
                }

                mensagem = this.CriarOcorrencia(xmlDocument, assistenciaTecnica, LoginDoPostoAutorizado, ref logAdicional);
            }
            catch (Exception ex)
            {
                //mensagem += ex.Message + ex.StackTrace;
                mensagem += ex.ToString();
            }

            string mensagemLog = String.Format("{0} \nLogin: {1} \nSenha: {2} \nData de inicio - terminio: {3} - {4} \n {6} \nXMLEnviado: {5}",
                                        mensagem,
                                        LoginDoPostoAutorizado,
                                        SenhaDoPostoAutorizado,
                                        dataInicioDoProcesso,
                                        DateTime.Now,
                                        XMLDadosDaOS,
                                        logAdicional);

           // LogHelper.Create(mensagemLog, ClassificacaoLog.WSIntegradorASTEC);

            return mensagem;
        }

        private static bool VerificaAcessoDoPosto(string usuario, string senha)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(SDKore.Configuration.ConfigurationManager.GetSettingValue("connector"));
                //conn = new SqlConnection("Data Source=sqlcrm\\sqlcrm;Initial Catalog=AutenticacaoExterna;User Id=connector;Password=connector");
                conn.Open();

                string sql = @"SELECT [AutenticacaoExterna].[dbo].[aspnet_Users].UserId
                        FROM [AutenticacaoExterna].[dbo].[aspnet_Users], [AutenticacaoExterna].[dbo].[aspnet_Membership]
                        WHERE [AutenticacaoExterna].[dbo].[aspnet_Users].UserId = [AutenticacaoExterna].[dbo].[aspnet_Membership].UserId
                        AND [AutenticacaoExterna].[dbo].[aspnet_Users].UserName = '{0}' 
                        AND [AutenticacaoExterna].[dbo].[aspnet_Membership].Password = '{1}'";
                sql = string.Format(sql, usuario, senha);

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader IDnoCRM = cmd.ExecuteReader();

                return IDnoCRM.HasRows;
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSIntegradorASTEC, "CRM OS Integrada Erro BD");
                return false;
            }
            finally
            {
                if (conn != null)
                    if (conn.State != System.Data.ConnectionState.Closed)
                    {
                        conn.Close();
                        conn.Dispose();
                    }
            }
        }

        public string CriarAnotacaoParaUmaOS(Ocorrencia ocorrencia, string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            string nomeArquivo = path.Substring(path.LastIndexOf("/") + 1, path.Length - path.LastIndexOf("/") - 1);

            var anotacao = new Anotacao(nomeOrganizacao, false)
            {
                NomeArquivos = nomeArquivo,
                Body = Convert.ToBase64String(bytes),
                EntidadeRelacionada = new Lookup(ocorrencia.Id, "incident"),
                Assunto = "Anexo"
            };

            new Domain.Servicos.RepositoryService().Anexo.Create(anotacao);

            return "\n Anexo Criado com sucesso!";
        }

        private Ocorrencia ConverterOcorrencia(XmlDocument xml, Domain.Model.Conta assistenciaTecnica, string LoginDoPostoAutorizado, out string mensagem)
        {
            mensagem = String.Empty;
            string cpfCnpj = string.Empty;

            if (string.IsNullOrEmpty(xml.GetElementsByTagName("CPFouCNPJ")[0].InnerText))
            { 
                mensagem += " \nO número do CNPJ/CPF do cliente deve ser informado";
            }else if (!string.IsNullOrEmpty(xml.GetElementsByTagName("CPFouCNPJ")[0].InnerText))
            {
                cpfCnpj = xml.GetElementsByTagName("CPFouCNPJ")[0].InnerText;
                cpfCnpj = cpfCnpj.Replace("-", "").Replace("/", "").Replace(".", "");

                if (cpfCnpj.Length == 11) cpfCnpj = String.Format(@"{0:000\.000\.000\-00}", Convert.ToInt64(cpfCnpj));
                else cpfCnpj = String.Format(@"{0:00\.000\.000\/0000\-00}", Convert.ToInt64(cpfCnpj));
            }

            if (string.IsNullOrEmpty(xml.GetElementsByTagName("Codigo")[0].InnerText) || string.IsNullOrEmpty(xml.GetElementsByTagName("NumeroDeSerie")[0].InnerText))
                mensagem += " \nO número de série e o código do produto devem ser informados";

            if (string.IsNullOrEmpty(xml.GetElementsByTagName("NumeroNF")[0].InnerText))
                mensagem += " \nO número da nota fiscal deve ser informado";

            Ocorrencia ocorrencia = new Ocorrencia(nomeOrganizacao, false)
            {
                Nome = "Assistência Técnica",
                Rascunho = true,
                Origem = 200006 /*OS Integrada*/,
                TipoDeOcorrencia = (int)TipoDeOcorrencia.Ordem_de_Serviço,
                RazaoStatus = (int)StatusDaOcorrencia.Aguardando_Analise,
                SolicitantePortal = LoginDoPostoAutorizado,
                PrioridadeValue = (int)TipoDePrioridade.Alta,
                DataDeAberturaDigitada = DateTime.Now,
                ProdutosDoCliente = xml.GetElementsByTagName("NumeroDeSerie")[0].InnerText,
                AparenciaDoProduto = xml.GetElementsByTagName("AparenciaDoProduto")[0].InnerText,
                AcessoriosOpcionais = xml.GetElementsByTagName("AcessoriosOpcionais")[0].InnerText,
                RetiradoPorNome = xml.GetElementsByTagName("RetiradoPorNome")[0].InnerText,
                RetiradoPorCPF = xml.GetElementsByTagName("RetiradoPorCPF")[0].InnerText,
                AcaoId = new Lookup(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("guid_acao_portal_astec")), "new_acao"),
                AssuntoId = new Lookup(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("guid_assunto_portal_astec")), "subject"),
                ClienteOS = new Domain.Model.Contato(nomeOrganizacao, false)
                {
                    Nome = xml.GetElementsByTagName("NomeCliente")[0].InnerText,
                    NomeCompleto = xml.GetElementsByTagName("NomeCliente")[0].InnerText,
                    PrimeiroNome = (xml.GetElementsByTagName("NomeCliente")[0].InnerText.Contains(" ") ? xml.GetElementsByTagName("NomeCliente")[0].InnerText.Substring(0, xml.GetElementsByTagName("NomeCliente")[0].InnerText.IndexOf(" ")) : xml.GetElementsByTagName("NomeCliente")[0].InnerText),
                    Sobrenome = (xml.GetElementsByTagName("NomeCliente")[0].InnerText.Contains(" ") ? xml.GetElementsByTagName("NomeCliente")[0].InnerText.Substring(xml.GetElementsByTagName("NomeCliente")[0].InnerText.IndexOf(" ") + 1, xml.GetElementsByTagName("NomeCliente")[0].InnerText.Length - xml.GetElementsByTagName("NomeCliente")[0].InnerText.IndexOf(" ") - 1) : xml.GetElementsByTagName("NomeCliente")[0].InnerText),
                    CpfCnpj = cpfCnpj,
                    Email1 = xml.GetElementsByTagName("EmailCliente")[0].InnerText,
                    Endereco1Rua = xml.GetElementsByTagName("Logradouro")[0].InnerText,
                    Endereco1Numero = xml.GetElementsByTagName("Numero")[0].InnerText,
                    Endereco1Complemento = xml.GetElementsByTagName("Complemento")[0].InnerText,
                    Endereco1Bairro = xml.GetElementsByTagName("Bairro")[0].InnerText,
                    Endereco1Municipio = xml.GetElementsByTagName("Cidade")[0].InnerText,
                    Endereco1CEP = xml.GetElementsByTagName("Cep")[0].InnerText,
                    Endereco1Estado = xml.GetElementsByTagName("Uf")[0].InnerText,
                    TelefoneComercial = xml.GetElementsByTagName("TelefonePrincipalCliente")[0].InnerText
                }
            };
            Municipio cidade = new Domain.Servicos.RepositoryService().Municipio.ObterPor(xml.GetElementsByTagName("Uf")[0].InnerText, xml.GetElementsByTagName("Cidade")[0].InnerText);
            if (cidade != null)
            {
                ocorrencia.ClienteOS.Endereco1Municipioid = new Lookup(cidade.Id, "itbc_municipios");
                if (cidade.Estadoid != null)
                    ocorrencia.ClienteOS.Endereco1Estadoid = cidade.Estadoid;
            }

            #region Autorizada

            ocorrencia.Autorizada = assistenciaTecnica;
            ocorrencia.Autorizada.Nome = LoginDoPostoAutorizado;
            ocorrencia.Autorizada.NomeAbreviado = LoginDoPostoAutorizado;
            ocorrencia.AutorizadaId = new Lookup(assistenciaTecnica.Id, "account");

            #endregion

            if (xml.GetElementsByTagName("Observacoes")[0].InnerText.Length < 4000) ocorrencia.DescricaoDaMensagemDeIntegracao = xml.GetElementsByTagName("Observacoes")[0].InnerText;
            else mensagem += " \nNão foi possível converter Observacoes, o campo deve conter menos de 4000 caracteres";

            if (xml.GetElementsByTagName("DefeitoAlegado")[0].InnerText.Length < 4000) ocorrencia.DefeitoAlegado = xml.GetElementsByTagName("DefeitoAlegado")[0].InnerText;
            else mensagem += " \nNão foi possível converter DefeitoAlegado, o campo deve conter menos de 4000 caracteres";

            if (xml.GetElementsByTagName("OSEmLote")[0].InnerText == "OK") ocorrencia.Tipo = "Lote";

            if (!string.IsNullOrEmpty(xml.GetElementsByTagName("DataDeAberturaInformada")[0].InnerText))
                try { ocorrencia.DataOrigem = Convert.ToDateTime(xml.GetElementsByTagName("DataDeAberturaInformada")[0].InnerText); }
                catch { mensagem += " \nErro ao tentar converter DataDeAberturaInformada para data"; }
            else ocorrencia.DataOrigem = DateTime.Now;

            if (!string.IsNullOrEmpty(xml.GetElementsByTagName("DataDeConsertoInformada")[0].InnerText))
            {
                try { ocorrencia.DataDeConsertoDigitada = Convert.ToDateTime(xml.GetElementsByTagName("DataDeConsertoInformada")[0].InnerText); }
                catch { mensagem += " \nErro ao tentar converter DataDeConsertoInformada para data"; }
                ocorrencia.DataDeConsertoInformada = DateTime.Now;
            }

            if (!string.IsNullOrEmpty(xml.GetElementsByTagName("DataDeEntregaClienteInformada")[0].InnerText))
            {
                try { ocorrencia.DataDeEntregaClienteInformada = Convert.ToDateTime(xml.GetElementsByTagName("DataDeEntregaClienteInformada")[0].InnerText); }
                catch { mensagem += " \nErro ao tentar converter DataDeEntregaClienteInformada para data"; }
                ocorrencia.DataDeEntregaClienteDigitada = DateTime.Now;
            }

            #region Nota Fiscal
            ocorrencia.NotaFiscalFatura = new Domain.Model.Fatura(nomeOrganizacao, false)
            {
                IDFatura = xml.GetElementsByTagName("NumeroNF")[0].InnerText,
                Cliente = new Domain.Model.Conta(nomeOrganizacao, false)
                {
                    Nome = xml.GetElementsByTagName("RazaoSocial")[0].InnerText,
                    CpfCnpj = xml.GetElementsByTagName("Cnpj")[0].InnerText,
                    Telefone = xml.GetElementsByTagName("TelefonePrincipal")[0].InnerText,
                    NomeAbreviado = (xml.GetElementsByTagName("NomeClienteNF")[0].InnerText != "") ? xml.GetElementsByTagName("NomeClienteNF")[0].InnerText : xml.GetElementsByTagName("NomeCliente")[0].InnerText
                    //CpfCnpj = (!String.IsNullOrEmpty(xml.GetElementsByTagName("CPFCNPJClienteNF")[0].InnerText)) ? xml.GetElementsByTagName("CPFCNPJClienteNF")[0].InnerText : xml.GetElementsByTagName("CPFouCNPJ")[0].InnerText
                }
            };
            ocorrencia.NumeroNfConsumido = xml.GetElementsByTagName("NumeroNF")[0].InnerText;
            ocorrencia.NomeDaLojaDoAtendimento = xml.GetElementsByTagName("RazaoSocial")[0].InnerText;
            ocorrencia.NomeConstadoNaNotaFiscalDeCompra = (xml.GetElementsByTagName("NomeClienteNF")[0].InnerText != "") ? xml.GetElementsByTagName("NomeClienteNF")[0].InnerText : xml.GetElementsByTagName("NomeCliente")[0].InnerText;
            ocorrencia.CpfCnpjConstadoNaNotaFiscalDeCompra = (!String.IsNullOrEmpty(xml.GetElementsByTagName("CPFCNPJClienteNF")[0].InnerText)) ? xml.GetElementsByTagName("CPFCNPJClienteNF")[0].InnerText : xml.GetElementsByTagName("CPFouCNPJ")[0].InnerText;
            ocorrencia.CnpjDaLojaDoAtendimento = xml.GetElementsByTagName("Cnpj")[0].InnerText;
            ocorrencia.TelefoneDaLojaDoAtendimento = xml.GetElementsByTagName("TelefonePrincipal")[0].InnerText;

            if (!String.IsNullOrEmpty(xml.GetElementsByTagName("DataDeCompra")[0].InnerText))
                try
                {
                    ocorrencia.NotaFiscalFatura.DataEmissao = Convert.ToDateTime(xml.GetElementsByTagName("DataDeCompra")[0].InnerText);
                    ocorrencia.DataConstadoNotaFiscalDeCompra = Convert.ToDateTime(xml.GetElementsByTagName("DataDeCompra")[0].InnerText);
                }
                catch { mensagem += " \nErro ao tentar converter DataDeCompra para data"; }
            #endregion

            #region Produto Principal

            string numeroSerie = xml.GetElementsByTagName("NumeroDeSerie")[0].InnerText;
            string codigoProduto = xml.GetElementsByTagName("Codigo")[0].InnerText;
            List<Product> produtos = (new Domain.Servicos.RepositoryService()).Produto.BuscarEstruturaDoProdutoPor(numeroSerie, codigoProduto);

            if (produtos.Count > 0)
            {
                ocorrencia.Produto = produtos[0];

                Product produtoCRM = (new Domain.Servicos.RepositoryService()).Produto.ObterPorNumero(ocorrencia.Produto.Codigo);
                if (produtoCRM == null)
                {
                    throw new ArgumentException("Produto não foi encontrado para Número de Série ou Código informado. (CRM)");
                }

                ocorrencia.Produto.Id = produtoCRM.Id;
                ocorrencia.ProdutoId = new Lookup(produtoCRM.Id, "product");

                OcorrenciaService ocorrenciaService = new Domain.Servicos.OcorrenciaService(nomeOrganizacao, false);
                ocorrenciaService.Ocorrencia = ocorrencia;
                ocorrenciaService.ValidaAberturaOcorrenciaASTEC();

                bool UrlImagemNota = string.IsNullOrEmpty(xml.GetElementsByTagName("URLImagemNota")[0].InnerText);

                LinhaComercial linhaComercial = (new Domain.Servicos.RepositoryService()).LinhaComercial.ObterPor(ocorrencia.Produto);

                if (linhaComercial != null)
                {
                    if (UrlImagemNota && linhaComercial.ObrigatorioEnviarNotaFiscal.HasValue && linhaComercial.ObrigatorioEnviarNotaFiscal.Value && !ocorrencia.Tipo.Equals("Lote"))
                    {
                        mensagem += " \nObrigatório o envio do anexo da Nota Fiscal";
                    }
                }

                ocorrencia.DataFabricacaoProduto = ocorrencia.Produto.DataFabricacaoProduto;
            }
            else
            {
                mensagem += " \nProduto não foi encontrado para Numero de Serie ou Codigo informado";
            }

            #endregion

            return ocorrencia;
        }

        private string Download(string path)
        {
            string nomeArquivo = path.Substring(path.LastIndexOf("/") + 1, path.Length - path.LastIndexOf("/") - 1);

            if (string.IsNullOrEmpty(nomeArquivo))
            {
                throw new ArgumentException("Nome do arquivo está vazio.");
            }

            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(new Uri(path), PathDownloadNotaFiscal + nomeArquivo);
                webClient.Dispose();

                byte[] bytes = File.ReadAllBytes(PathDownloadNotaFiscal + nomeArquivo);

                if (bytes.Length > 20000000) //Se o arquivo for maior que 2MB não deixa salvar no CRM
                    throw new ArgumentException("Arquivo com mais de 2 MB não são permitidos (" + path + ")");
            }
            catch (WebException ex)
            {
                throw new ArgumentException("Não foi possível fazer o download da Nota Fiscal", ex);
            }

            return PathDownloadNotaFiscal + nomeArquivo;
        }

        private string CriarOcorrencia(XmlDocument doc, Domain.Model.Conta assistenciaTecnica, string LoginDoPostoAutorizado, ref string logAdicional)
        {
            string mensagem = "";
            Ocorrencia ocorrencia = this.ConverterOcorrencia(doc, assistenciaTecnica, LoginDoPostoAutorizado, out mensagem);

            if (!String.IsNullOrEmpty(mensagem))
                return mensagem;

            string pathXML = doc.GetElementsByTagName("URLImagemNota")[0].InnerText;
            string pathNotaFiscal = string.Empty;

            if (!string.IsNullOrEmpty(pathXML))
            {
                pathNotaFiscal = Download(pathXML);
            }

            //Depois que o anexo foi salvo cria a ocorrencia
            if (string.IsNullOrEmpty(mensagem))
            {
                logAdicional += " - Inicio Salvar OS: " + DateTime.Now.ToString();
                Guid ocorrenciaId = Guid.Empty;
                ocorrenciaId = ocorrencia.SalvarOS(ocorrencia);
                logAdicional += " - Fim Salvar OS: " + DateTime.Now.ToString();

                if (ocorrenciaId == Guid.Empty) return "Não foi possível adicionar Ocorrência! Por gentileza, verifique com a Intelbras.";

                logAdicional += " - Inicio Retrieve OS: " + DateTime.Now.ToString();
                ocorrencia = (new Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(ocorrenciaId);
                mensagem += ocorrencia.Numero;
                logAdicional += " - Fim Retrieve OS: " + DateTime.Now.ToString();
                logAdicional += " - Inicio Anexo CRM: " + DateTime.Now.ToString();

                if (!string.IsNullOrEmpty(pathNotaFiscal))
                {
                    mensagem += CriarAnotacaoParaUmaOS(ocorrencia, pathNotaFiscal);
                }
                logAdicional += " - Fim Anexo CRM: " + DateTime.Now.ToString();
            }

            return mensagem;
        }
    }
}
