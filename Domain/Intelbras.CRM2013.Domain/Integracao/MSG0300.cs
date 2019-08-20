using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.Net.Mail;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0300 : Base, IBase<MSG0300, Ocorrencia>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0300(string org, bool isOffline) : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar
        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            usuarioIntegracao = usuario;
            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0300R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades
        public Ocorrencia DefinirPropriedades(MSG0300 legado)
        {
            var crm = new Model.Ocorrencia(this.Organizacao, this.IsOffline);
            return crm;
        }

        private Intelbras.Message.Helper.MSG0300 DefinirPropriedades(Ocorrencia crm)
        {
            string strNomeProdutos = string.Empty;

            Intelbras.Message.Helper.MSG0300 xml = new Pollux.MSG0300(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Numero, 40));

            if (crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Atendimento_Avulso)
                xml.TipoAtendimento = "FA";
            else
                if (crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamação_Mau_Atendimento ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamação_Procon ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamação_Atraso_no_Conserto ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.ReclamacaoImprocedente ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamacao_Falha_No_Processo ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamacao_Mau_Funcionamento ||
                crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.Reclamação_Nau_Funcionamento_Com_Solucao)
                xml.TipoAtendimento = "RC";
            else
                if (crm.TipoDeOcorrencia == (int)TipoDeOcorrencia.AnaliseDeDefeito)
                xml.TipoAtendimento = "AD";

            xml.CodigoOcorrencia = crm.ID.Value.ToString();

            if (!string.IsNullOrEmpty(crm.Numero))
            {
                xml.NumeroOcorrencia = crm.Numero;
            }
            if (crm.RazaoStatus.HasValue)
            {
                xml.StatusOcorrencia = crm.RazaoStatus;
            }
            if (crm.PrioridadeValue.HasValue)
            {
                xml.Prioridade = crm.PrioridadeValue;
            }
            if (crm.TipoDeOcorrencia.HasValue)
            {
                xml.TipoOcorrencia = crm.TipoDeOcorrencia;
                xml.DescricaoTipoOcorrencia = DescricaoTipoOcorrencia(crm.TipoDeOcorrencia.Value);
            }
            if (crm.Assunto != null)
            {
                xml.DefeitoAlegado = crm.Assunto.Nome;
            }
            if (!string.IsNullOrEmpty(xml.AtividadeExecutada))
            {
                crm.AtividadeExecutada = xml.AtividadeExecutada;
            }
            if (crm.DataOrigem.HasValue)
            {
                xml.DataHoraAbertura = crm.DataOrigem.Value.ToLocalTime();
            }

            Usuario proprietario = new Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("incident", "incidentid", crm.ID.Value);
            if (proprietario != null)
            {
                xml.NomeProprietario = proprietario.NomeCompleto;
            }

            if (!string.IsNullOrEmpty(crm.ProdutosDoCliente))
            {
                xml.NumeroSerieProduto = crm.ProdutosDoCliente;
            }

            if (crm.Produto != null)
            {
                Product objProduto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(crm.Produto.Id);
                if (objProduto != null)
                {
                    xml.CodigoProduto = objProduto.Codigo;
                    strNomeProdutos = objProduto.Nome;
                }
            }

            #region Montagem do texto padrão do campo observação
            xml.Observacao = crm.Nome + "\n" +
            "DEFEITO ALEGADO: " + (crm.Assunto.Nome != null ? crm.Assunto.Nome : "--");
            //"PRODUTO: " + (xml.CodigoProduto != null ? xml.CodigoProduto + " - " + strNomeProdutos : "--") + "\n" +
            //"NÚMERO DE SÉRIE: " + (xml.NumeroSerieProduto != null ? xml.NumeroSerieProduto : "--") + "\n" +
            //"DATA FABRICAÇÃO: " + (crm.DataFabricacaoProduto != null ? crm.DataFabricacaoProduto.Value.ToLocalTime().ToString("dd/MM/yyyy") : "--") + "\n" +
            //"DATA VENDA: " + (crm.DataCompraIntelbras != null ? crm.DataCompraIntelbras.Value.ToLocalTime().ToString("dd/MM/yyyy") : "--") + "\n" +
            //"PEDIDO VENDA: " + (crm.NumeroPedidoVenda != null ? crm.NumeroPedidoVenda : "-- ") + "     NF VENDA: " + (crm.NumeroNotaFiscal != null ? crm.NumeroNotaFiscal : "--") + "\n\n";
            #endregion

            //if (!string.IsNullOrEmpty(crm.Anexo))
            //{
            //    xml.Observacao += crm.Anexo;
            //}

            xml.NomeUsuario = crm.CriadoPor.Name;

            xml.CNPJAutorizada = crm.Autorizada != null ? crm.Autorizada.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim() : null;
            xml.NomeAutorizada = crm.Autorizada != null ? crm.Autorizada.Nome.Trim() : null;

            #region Dados do Cliente da Ocorrência
            Model.Contato contato = new Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(crm.ClienteId.Id);
            Model.Conta conta = new Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(crm.ClienteId.Id);
            Pollux.Entities.Endereco enderecoContato = new Pollux.Entities.Endereco();
            Pollux.Entities.ClienteOcorrencia clienteOcorrencia = new Pollux.Entities.ClienteOcorrencia();

            clienteOcorrencia.CodigoContato = contato != null ? contato.Id.ToString() : conta.Id.ToString();

            if (conta != null)
            {
                clienteOcorrencia.CodigoCliente = Convert.ToInt32(conta.CodigoMatriz);

                if (!string.IsNullOrEmpty(conta.CpfCnpj))
                {
                    string cnpjCpfObj = conta.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

                    if (!String.IsNullOrEmpty(cnpjCpfObj))
                    {
                        if (cnpjCpfObj.Length <= 11)
                        {
                            clienteOcorrencia.CPF = cnpjCpfObj;
                        }
                        else
                        {
                            clienteOcorrencia.CNPJ = cnpjCpfObj;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(conta.Endereco1CEP))
                    enderecoContato.CEP = conta.Endereco1CEP.Replace("-", "").PadLeft(8, '0');

                if (conta.Endereco1Municipioid != null)
                {
                    Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(conta.Endereco1Municipioid.Id);
                    enderecoContato.Cidade = municipio.ChaveIntegracao;
                    enderecoContato.NomeCidade = municipio.Nome;
                }

                if (conta.Endereco1Estadoid != null)
                {
                    Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(conta.Endereco1Estadoid.Id);
                    if (estado != null)
                    {
                        enderecoContato.Estado = estado.ChaveIntegracao;
                        enderecoContato.UF = estado.SiglaUF;
                    }
                    if (conta.Endereco1Pais == null)
                    {
                        conta.Endereco1Pais = estado.Pais;
                    }
                }

                if (conta.Endereco1Pais != null)
                {
                    Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(conta.Endereco1Pais.Id);
                    enderecoContato.NomePais = pais.Nome;
                    enderecoContato.Pais = pais.ChaveIntegracao; ;
                }
                if (!String.IsNullOrEmpty(conta.Endereco1CaixaPostal))
                    enderecoContato.CaixaPostal = conta.Endereco1CaixaPostal;
            }

            if (contato != null)
            {
                if (contato.AssociadoA != null)
                {
                    conta = new Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(contato.AssociadoA.Id);
                    if (conta != null)
                        clienteOcorrencia.CodigoCliente = Convert.ToInt32(conta.CodigoMatriz);
                }
                if (contato.Aniversario.HasValue)
                    clienteOcorrencia.DataNascimento = contato.Aniversario.Value.ToLocalTime();
                else
                    clienteOcorrencia.DataNascimento = null;

                if (!string.IsNullOrEmpty(contato.CpfCnpj))
                {
                    string cnpjCpfObj = contato.CpfCnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();

                    if (!String.IsNullOrEmpty(cnpjCpfObj))
                    {
                        if (cnpjCpfObj.Length <= 11)
                        {
                            clienteOcorrencia.CPF = cnpjCpfObj;
                        }
                        else
                        {
                            clienteOcorrencia.CNPJ = cnpjCpfObj;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(contato.Endereco1CEP))
                    enderecoContato.CEP = contato.Endereco1CEP.Replace("-", "").PadLeft(8, '0');

                if (contato.Endereco1Municipioid != null)
                {
                    Municipio municipio = new Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(contato.Endereco1Municipioid.Id);
                    enderecoContato.Cidade = municipio.ChaveIntegracao;
                    enderecoContato.NomeCidade = municipio.Nome;
                }

                if (contato.Endereco1Estadoid != null)
                {
                    Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(contato.Endereco1Estadoid.Id);
                    if (estado != null)
                    {
                        enderecoContato.Estado = estado.ChaveIntegracao;
                        enderecoContato.UF = estado.SiglaUF;
                    }
                    if (contato.Endereco1Pais == null)
                    {
                        contato.Endereco1Pais = estado.Pais;
                    }
                }

                if (contato.Endereco1Pais != null)
                {
                    Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(contato.Endereco1Pais.Id);
                    enderecoContato.NomePais = pais.Nome;
                    enderecoContato.Pais = pais.ChaveIntegracao; ;
                }

                if (!String.IsNullOrEmpty(contato.Endereco1CaixaPostal))
                    enderecoContato.CaixaPostal = contato.Endereco1CaixaPostal;
            }
            
            clienteOcorrencia.NomeContato = contato != null ? contato.PrimeiroNome : conta.Nome;
            clienteOcorrencia.SegundoNome = contato != null ? contato.SegundoNome : null;
            clienteOcorrencia.Sobrenome = contato != null ? contato.Sobrenome : null;
            clienteOcorrencia.Email = contato != null ? contato.Email1 : conta.Email;
            clienteOcorrencia.Telefone = contato != null ? contato.TelefoneComercial : conta.Telefone;
            clienteOcorrencia.Ramal = contato != null ? contato.Ramal1 : conta.RamalTelefonePrincipal;
            clienteOcorrencia.Celular = contato != null ? contato.TelefoneCelular : conta.Fax;
            clienteOcorrencia.Fax = contato != null ? contato.Fax : null;
            clienteOcorrencia.RamalFax = contato != null ? contato.RamalFax : null;
            clienteOcorrencia.Sexo = contato != null ? contato.Sexo : null;
            clienteOcorrencia.RG = contato != null ? contato.DocIdentidade : null;
            clienteOcorrencia.OrgaoExpeditor = contato != null ? contato.EmissorDocIdentidade : null;
            #endregion

            #region Endereco
            enderecoContato.Bairro = contato != null ? contato.Endereco1Bairro : conta.Endereco1Bairro;
            enderecoContato.Numero = contato != null ? contato.Endereco1Numero : conta.Endereco1Numero;
            enderecoContato.Complemento = contato != null ? contato.Endereco1Complemento : conta.Endereco1Complemento;
            enderecoContato.Logradouro = contato != null ? contato.Endereco1Rua : conta.Endereco1Rua;
            enderecoContato.NomeEndereco = contato != null ? contato.Endereco1Nome : null;
            enderecoContato.TipoEndereco = contato != null ? contato.Endereco1TipoEndereco : conta.TipoEndereco;

            if (enderecoContato != null)
                clienteOcorrencia.EnderecoPrincipal = enderecoContato;

            #endregion

            #region Lista de anexos da ocorrência
            var urlServico = SDKore.Configuration.ConfigurationManager.GetSettingValue("UrlSiteCRM");

            var strTextosAnotacoes = "";

            if (crm.Anexos != null && crm.Anexos.Count > 0)
            {
                List<Pollux.Entities.AnexoOcorrencia> lstAnexosOcorrencia = new List<Pollux.Entities.AnexoOcorrencia>();

                foreach (Anotacao crmItem in crm.Anexos)
                {
                    if (crmItem.TemArquivo)
                    {
                        Pollux.Entities.AnexoOcorrencia objPollux = new Pollux.Entities.AnexoOcorrencia();
                        objPollux.NomeArquivo = crmItem.NomeArquivos;
                        if (SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente") == "Desenvolvimento")
                            objPollux.URL = urlServico + "/Activities/Attachment/download.aspx?AttachmentType=5&IsNotesTabAttachment=1&AttachmentId=" + crmItem.ID + "&CRMWRPCToken=N%2fF0lhjMEemA3ABQVqpIdBJpbKVyWAZQV4rPHHxT6tlRmtDGA0vqSwwW7tVLsMKh&CRMWRPCTokenTimeStamp=636845305936642215";
                        else
                            objPollux.URL = urlServico + "/Activities/Attachment/download.aspx?AttachmentType=5&IsNotesTabAttachment=1&AttachmentId=" + crmItem.ID;

                        lstAnexosOcorrencia.Add(objPollux);
                    }

                    strTextosAnotacoes += System.Environment.NewLine + "----------------------------------------------------------------------" + System.Environment.NewLine
                            + crmItem.CriadoPor.Name.ToString() + " em " + crmItem.CriadoEm.Value.ToLocalTime() + ". " + (string.IsNullOrEmpty(crmItem.Assunto) ? "ASSUNTO: " + crmItem.Assunto : "") + System.Environment.NewLine
                            + crmItem.Texto;

                }
                xml.ListaAnexosOcorrencia = lstAnexosOcorrencia;
            }
            #endregion

            if (!string.IsNullOrEmpty(strTextosAnotacoes))
            {
                xml.Observacao += strTextosAnotacoes;
            }
            else if (!string.IsNullOrEmpty(crm.Anexo))
            {
                xml.Observacao += System.Environment.NewLine + crm.Anexo;
            }

            xml.ClienteOcorrencia = clienteOcorrencia;

            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(Ocorrencia objOcorrencia)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0300 mensagem = this.DefinirPropriedades(objOcorrencia);

            var ambiente = (SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente") == "Desenvolvimento" ? "- Desenvolvimento" : SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente") == "Homologacao" ? "- Homologação" : "");

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            try
            {
                if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
                {
                    Intelbras.Message.Helper.MSG0300R1 retorno = CarregarMensagem<Pollux.MSG0300R1>(retMsg);
                    if (!retorno.Resultado.Sucesso)
                    {
                        string textoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado(a)s, <br /><br />
                    Não foi possivel integrar a ocorrência " + objOcorrencia.Numero + "." +
                        "<br /><br />A mensagem de erro retornado na integração com o Assist foi: " + retorno.Resultado.Mensagem + "." +
                        "<br /><br />Favor, resolver esta falha de integração, pois, pode existir SLA sobre está ocorrência." +
                        "<br /><br />Verifique no log do barramento para mais detalhes.</FONT>";


                        EnviarEmail("Falha de integração de ocorrências ASTEC " + ambiente, textoEmail, "grupo.crm_ti@intelbras.com.br");
                        throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                    }
                }
                else
                {
                    Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);

                    string textoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado(a)s, <br /><br />
                    Não foi possivel integrar a ocorrência " + objOcorrencia.Numero + "." +
                    "<br /><br />O erro apresentado na tentativa de integração com o Assist foi: " + retMsg + "." +
                    "<br /><br />Favor, resolver esta falha de integração, pois, pode existir SLA sobre está ocorrência." +
                    "<br /><br />Verifique no log do barramento para mais detalhes.</FONT>";
                    EnviarEmail("Falha de integração de ocorrências ASTEC " + ambiente, textoEmail, "grupo.crm_ti@intelbras.com.br");

                    throw new ArgumentException("(CRM) " + string.Concat("Erro de Integração \n", erro001.GenerateMessage(false)));
                }
            }
            catch (Exception ex)
            {
                string textoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado(a)s, <br /><br />
                    Não foi possivel integrar a ocorrência " + objOcorrencia.Numero + "." +
                "<br /><br />A exceção gerada foi: " + ex.Message + "." +
                "<br /><br />Favor, resolver esta falha de integração, pois, pode existir SLA sobre está ocorrência." +
                "<br /><br />Verifique no Event Viewer do CRM para mais detalhes.</FONT>";
                EnviarEmail("Falha de integração de ocorrências ASTEC " + ambiente, textoEmail, "grupo.crm_ti@intelbras.com.br");

                throw new ArgumentException("(CRM) (XSD) " + ex.Message, ex);
            }
            return retMsg;
        }

        private string DescricaoTipoOcorrencia(int tipoDeOcorrencia)
        {
            switch (tipoDeOcorrencia)
            {
                case ((int)TipoDeOcorrencia.Atendimento_Avulso):
                    return "Atendimento Facilitado";
                case ((int)TipoDeOcorrencia.ReclamacaoImprocedente):
                    return "Reclamação Improcedente";
                case ((int)TipoDeOcorrencia.Reclamação_Atraso_no_Conserto):
                    return "Reclamação/ Atraso no Conserto";
                case ((int)TipoDeOcorrencia.Reclamacao_Falha_No_Processo):
                    return "Reclamação/ Falha no Processo";
                case ((int)TipoDeOcorrencia.Reclamação_Mau_Atendimento):
                    return "Reclamação/ Mau Atendimento";
                case ((int)TipoDeOcorrencia.Reclamacao_Mau_Funcionamento):
                    return "Reclamação/ Mau Funcionamento";
                case ((int)TipoDeOcorrencia.Reclamação_Nau_Funcionamento_Com_Solucao):
                    return "Reclamação/ Mau Funcionamento c/ Solução";
                case ((int)TipoDeOcorrencia.Reclamação_Procon):
                    return "Reclamação/ Procon";
                case ((int)TipoDeOcorrencia.AnaliseDeDefeito):
                    return "Analise de Defeito";
            }

            return "Valor inválido";
        }

        /// <summary>
        /// Envia o e-mail caso tenha erro
        /// </summary>
        /// <param name="assunto"></param>
        /// <param name="textoEmail"></param>
        /// <param name="Arquivo"></param>
        private static void EnviarEmail(string assunto, string textoEmail, string para)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "outlook.intelbras.com.br";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("intelbras\\mail.crm2013", "EjPS2hK5VeCH2SBfdxxPjQ6Q");
            client.EnableSsl = false;
            MailMessage mail = new MailMessage();
            mail.Sender = new MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");
            mail.From = new MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");

            //Cria a lista de destinatarios
            foreach (var root in para.Split(new Char[] { ',', ':', ';' }))
            {
                mail.To.Add(new MailAddress(root.ToString().Trim()));
            }

            mail.Subject = assunto;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));

            mail.Body = textoEmail;

            try
            {
                client.Send(mail);
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
            finally
            {
                mail = null;
            }
        }

        #endregion
    }
}
