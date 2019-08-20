using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NUnit.Framework;
using System;
using System.DirectoryServices.AccountManagement;
using System.ServiceModel.Description;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    [TestFixture]
    public class Integracao : Base
    {
        [TestMethod]
        public void MensagemBarramento()
        {
            string path = @"C:\testeIntegracao.xml";

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(path);

            string tstREsponse;

            new Domain.Servicos.Integracao(OrganizationName, IsOffline).Postar(usuario, senha, xmlDoc.InnerXml, out tstREsponse);
        }

        [Test]
        public void Autenticar()
        {

            //Authenticate using credentials of the logged in user;
            string UserName = "barramento";
            string Password = "vCha6rGxXmWHeu8YKWfeXW2E";
            string Dominio = "INTELBRAS";

            bool autAD = false;

            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Dominio))
            {
                autAD = pc.ValidateCredentials(UserName, Password);
            }

            if (!autAD)
            {

            }

            UserName = Dominio + @"\" + UserName;

            //System.Collections.Generic.List<Domain.Model.ReceitaPadrao> rec = Domain.Servicos.RepositoryService.ReceitaPadrao.ListarPor();
            
            Domain.Model.Usuario usuario = new Domain.Servicos.RepositoryService(this.OrganizationName, this.IsOffline).Usuario.ObterPor(UserName);

            if (usuario == null || usuario.ID == null)
            {
                
            }

            Uri OrganizationUri = new Uri("http://10.1.1.171:5554/XRMServices/2011/Organization.svc");

            //ClientCredentials Credentials = new ClientCredentials();
            //Credentials.UserName.UserName = UserName;
            //Credentials.UserName.Password = Password;

            IServiceManagement<IOrganizationService> orgServiceManagement =
                   ServiceConfigurationFactory.CreateManagement<IOrganizationService>(OrganizationUri);


            #region Testes
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();

            authCredentials.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(UserName, Password, "Intelbras");

            using (OrganizationServiceProxy organizationProxy =
                    GetProxy<IOrganizationService, OrganizationServiceProxy>(orgServiceManagement, authCredentials))
            {
                // This statement is required to enable early-bound type support.
                organizationProxy.EnableProxyTypes();

                IOrganizationService _service = (IOrganizationService)organizationProxy;

                Entity myAccount = new Entity("account");
                myAccount["name"] = "Test Account";
                _service.Create(myAccount);

            //    DiscoveryServiceProxy dsp = new DiscoveryServiceProxy(OrganizationUri, null, Credentials, null);
            //    dsp.Authenticate();
            //    organizationProxy.Authenticate();
                

            //    // Now make an SDK call with the organization service proxy.
            //    // Display information about the logged on user.
            //    //Guid userid = ((WhoAmIResponse)organizationProxy.Execute(new WhoAmIRequest())).UserId;

            //    //SystemUser systemUser = organizationProxy.Retrieve("systemuser", userid, new ColumnSet(new string[] { "firstname", "lastname" })).ToEntity<SystemUser>();

            }

            #endregion

            //Credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            
            //OrganizationServiceProxy serviceProxy;   
            //using (OrganizationServiceProxy serviceProxy = new OrganizationServiceProxy(OrganizationUri, null, Credentials, null))
            //{
            //    serviceProxy.CallerId = new Guid("E53CBD6F-8E9D-E311-888D-00155D013E2E");

            //    IOrganizationService _service = (IOrganizationService)serviceProxy;

            //    Entity myAccount = new Entity("account");
            //    myAccount["name"] = "Test Account";
            //    _service.Create(myAccount);

            //    OrganizationServiceContext orgContext = new OrganizationServiceContext(serviceProxy);

            //}


        }

        private TProxy GetProxy<TService, TProxy>(
            IServiceManagement<TService> serviceManagement,
            AuthenticationCredentials authCredentials)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Type classType = typeof(TProxy);

            if (serviceManagement.AuthenticationType !=
                AuthenticationProviderType.ActiveDirectory)
            {
                AuthenticationCredentials tokenCredentials =
                    serviceManagement.Authenticate(authCredentials);
                // Obtain discovery/organization service proxy for Federated, LiveId and OnlineFederated environments. 
                // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and SecurityTokenResponse.
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, tokenCredentials.SecurityTokenResponse });
            }

            // Obtain discovery/organization service proxy for ActiveDirectory environment.
            // Instantiate a new class of type using the 2 parameter constructor of type IServiceManagement and ClientCredentials.
            return (TProxy)classType
                .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }


        [TestMethod]
        public void MensagemBarramentoNovo()
        {
            //string path = @"C:\Users\lu050727\Documents\Dynamics CRM 2013 Files\XML\MSG0086.xml";
            string path = @"C:\temp\msg0277.txt";

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.Load(path);

            string tstREsponse;

            new Domain.Servicos.Integracao(OrganizationName, IsOffline).Postar(usuario, senha, xmlDoc.InnerXml, out tstREsponse);
        }

        [TestMethod]
        public void IntegraProdutoCondicaoPagamento()
        {
            var prodCondPag = new Intelbras.CRM2013.Domain.Servicos.ProdutoCondicaoPagamentoService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("2FAFB609-DAF8-E811-80D6-0050568DB649"));
            var retorno = new Intelbras.CRM2013.Domain.Servicos.ProdutoCondicaoPagamentoService(this.OrganizationName, this.IsOffline).IntegracaoBarramento(prodCondPag);
        }


        [TestMethod]
        public void CategoriazacaoREvenda()
        {
           // new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline).AtualizaContasReCategorizacao();
            Domain.Servicos.SellinService SellinServices = new Domain.Servicos.SellinService(OrganizationName, IsOffline);
            SellinServices.EnviaRegistroSellinFieloProvedoresSolucoes();
            //SellinServices.EnviaRegistroSellinFieloAstec();

        }

        [TestMethod]
        public void Sellout()
        {
            string resposta;
            try
            {
                var classificacao =  new Domain.Servicos.RepositoryService(this.OrganizationName, this.IsOffline).Classificacao.ObterPor(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Atac_Dist);
                var xml = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline).MontaXmlRevendasFielo();
                resposta = xml.Declaration.ToString() + Environment.NewLine + xml.ToString();
            }
            catch (Exception ex)
            {
                resposta = ex.Message;
            }
        }

    }
}
