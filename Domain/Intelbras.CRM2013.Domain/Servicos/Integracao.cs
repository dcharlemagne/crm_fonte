using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Xml.Linq;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.ServiceModel;
using SDKore.Configuration;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class Integracao
    {
        public string Organizacao { get; set; }

        public bool IsOffline { get; set; }

        public Integracao(string org, bool isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        public bool Postar(string usuario, string senha, string mensagem, out string response)
        {
            //mensagem = tst.GenerateMessage(false);
            string codigoMensagem = string.Empty;
            string numeroOperacao = string.Empty;

            try
            {
                Model.Usuario usuarioTransacao = AutenticarUsuario(usuario, senha);

                if (usuarioTransacao == null)
                {
                    numeroOperacao = "Autenticacao";
                    ERR0001 objErro = new ERR0001(itb.RetornaSistema(itb.Sistema.Pollux), numeroOperacao);
                    objErro.DescricaoErro = "Autenticação inválida Erro [44230]" + usuario + " | " + senha;
                    response = objErro.GenerateMessage(false);

                    return false;
                }

                this.ValidarEstruturaXML(mensagem);

                codigoMensagem = this.ObterCodigoDaMensagem(mensagem);
                numeroOperacao = this.ObterNumeroOperacaoMensagem(mensagem);

                //Ajustar config
                var nameSpaceDomain = "Intelbras.CRM2013.Domain";//SDKore.Configuration.ConfigurationManager.GetApplicationDomain(string.Format("{0}Domain", this.Organizacao)).Split(',')[0].Trim();
                dynamic obj = Activator.CreateInstance(Type.GetType(string.Format("{0}.Integracao.{1}", nameSpaceDomain, codigoMensagem)), this.Organizacao, this.IsOffline);
                var objRetorno = obj.Executar(mensagem, numeroOperacao, usuarioTransacao);
                response = objRetorno;

                return true;
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Create(ex, System.Diagnostics.EventLogEntryType.Information);

                string messageError = SDKore.Helper.Error.Handler(ex);
                dynamic retorno = ObterObjetoMensagemRetorno(codigoMensagem, numeroOperacao);

                ERR0001 objErro = new ERR0001(itb.RetornaSistema(itb.Sistema.CRM), numeroOperacao);
                objErro.DescricaoErro = messageError;
                response = objErro.GenerateMessage(false);
                return false;
            }
        }

        private object ObterObjetoMensagemRetorno(string nomeMensagem, string codigoSequencial)
        {
            string namespaceBarramento = ConfigurationManager.GetSettingValue("NamespaceBarramento");
            var ass = System.Reflection.Assembly.Load(namespaceBarramento);
            Type[] types2 = ass.GetTypes();
            Type tipo = types2.FirstOrDefault(obj => obj.FullName == string.Format("{0}.{1}", namespaceBarramento, string.Format("{0}R1", nomeMensagem)));

            return Activator.CreateInstance(tipo, itb.RetornaSistema(itb.Sistema.CRM), codigoSequencial);
        }

        private string ObterNumeroOperacaoMensagem(string mensagem)
        {
            XDocument xml = XDocument.Parse(mensagem);

            var cabecalho = xml.Root.Element("CABECALHO");
            if (cabecalho == null)
                throw new ApplicationException("XML não possui cabeçalho.");

            var numeroOperacao = cabecalho.Element("NumeroOperacao");
            if (numeroOperacao == null)
                throw new ApplicationException("Cabeçalho sem Número de Operação");

            return numeroOperacao.Value;
        }

        private string ObterCodigoDaMensagem(string mensagem)
        {
            XDocument xml = XDocument.Parse(mensagem);
            var cabecalho = xml.Root.Element("CABECALHO");
            if (cabecalho == null)
                throw new ApplicationException("XML não possui cabeçalho.");

            var codMensagem = cabecalho.Element("CodigoMensagem");
            if (codMensagem == null)
                throw new ApplicationException("Cabeçalho sem Codigo da Mensagem");

            return codMensagem.Value;
        }

        private void ValidarEstruturaXML(string mensagem)
        {
            try
            {
                XDocument.Parse(mensagem);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Aparentemente o XML recebido não é válido. Erro: {0}", ex.Message));
            }
        }

        private Domain.Model.Usuario AutenticarUsuario(string UserName, string Password)
        {
            Domain.Model.Usuario usuario = null;
            //string Dominio = "Intelbras";
            string Dominio = ConfigurationManager.GetDomain(this.Organizacao);

            #region Verifica no AD

            //using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Dominio))
            //{
            //    if (!pc.ValidateCredentials(UserName, Password))
            //        return usuario; ;
            //}
            #endregion

            #region Verifica no CRM
            usuario = new Domain.Servicos.RepositoryService(this.Organizacao, this.IsOffline).Usuario.ObterPor(Dominio + @"\barramento");
            #endregion

            return usuario;
        }

        public bool EnviarMensagemBarramento(string msg, string versao, string contrato, out string resposta)
        {
            BasicHttpBinding myBinding = new BasicHttpBinding();
            myBinding.Name = "IntegracaoBarramento";
            myBinding.SendTimeout = SDKore.Configuration.ConfigurationManager.GetSettingValue<TimeSpan>("Intelbras.Barramento.TimeOut");
            myBinding.CloseTimeout = SDKore.Configuration.ConfigurationManager.GetSettingValue<TimeSpan>("Intelbras.Barramento.TimeOut");
            myBinding.OpenTimeout = SDKore.Configuration.ConfigurationManager.GetSettingValue<TimeSpan>("Intelbras.Barramento.TimeOut");
            myBinding.Security.Mode = BasicHttpSecurityMode.None;
            myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;

            EndpointAddress endPointAddress = new EndpointAddress(ConfigurationManager.GetSettingValue("EndpointAddressURL"));

            var pollusws = new BarramentoWebService.MessageReceiverClient(myBinding, endPointAddress);
            return pollusws.PostMessage(msg, versao, contrato, out resposta);
        }

        public string CriarRevendaSellout(string xml)
        {
            return xml;
        }

        public bool Enviar(string usuario, string senha, string mensagem, out string response)
        {

            //mensagem = tst.GenerateMessage(false);

            string codigoMensagem = string.Empty;
            string numeroOperacao = string.Empty;


            try
            {
                Domain.Model.Usuario usuarioTransacao = AutenticarUsuario(usuario, senha);

                if (usuarioTransacao == null)
                {
                    numeroOperacao = "Autenticacao";
                    ERR0001 objErro = new ERR0001(itb.RetornaSistema(itb.Sistema.Pollux), numeroOperacao);
                    objErro.DescricaoErro = "Autenticação inválida Erro [44110]" + usuario + " | " + senha;
                    response = objErro.GenerateMessage(false);
                    return false;
                }




                this.ValidarEstruturaXML(mensagem);

                codigoMensagem = this.ObterCodigoDaMensagem(mensagem);
                numeroOperacao = this.ObterNumeroOperacaoMensagem(mensagem);

                //Ajustar config
                var nameSpaceDomain = "Intelbras.CRM2013.Domain";//SDKore.Configuration.ConfigurationManager.GetApplicationDomain(string.Format("{0}Domain", this.Organizacao)).Split(',')[0].Trim();
                dynamic obj = Activator.CreateInstance(Type.GetType(string.Format("{0}.Integracao.{1}", nameSpaceDomain, codigoMensagem)), this.Organizacao, this.IsOffline);
                var objRetorno = obj.Enviar(mensagem, numeroOperacao, usuarioTransacao);
                response = objRetorno;

                return true;
            }
            catch (Exception ex)
            {
                dynamic retorno = ObterObjetoMensagemRetorno(codigoMensagem, numeroOperacao);

                ERR0001 objErro = new ERR0001(itb.RetornaSistema(itb.Sistema.Pollux), numeroOperacao);
                objErro.DescricaoErro = ex.Message;
                response = objErro.GenerateMessage(false);
                return false;
            }
        }

        public string ObterUrlCrm2013(string guidCrm40, bool tipoConta)
        {
            if (tipoConta)
            {
                Domain.Model.Conta conta = new Domain.Servicos.ContaService(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).BuscarContaIntegracaoCrm4(guidCrm40);

                if (conta != null)
                {
                    string url = "https://crm2013.intelbras.com.br/main.aspx?etn=account&pagetype=entityrecord&id=%" + conta.ID.Value.ToString() + "%7D";
                    return url;
                }
                return String.Empty;
            }
            else
            {
                Domain.Model.Contato contato = new Domain.Servicos.ContatoService(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).BuscarPorIntegracaoCrm4(guidCrm40);

                if (contato != null)
                {
                    string url = "https://crm2013.intelbras.com.br/main.aspx?etn=contact&pagetype=entityrecord&id=%" + contato.ID.Value.ToString() + "%7D";
                    return url;
                }
                return String.Empty;
            }

        }

    }
}