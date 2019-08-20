using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Discovery;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace SDKore.Crm
{
    internal sealed class ManagedTokenDiscoveryServiceProxy : DiscoveryServiceProxy
    {
        private AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService> _proxyManager;

        public ManagedTokenDiscoveryServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
            : base(serviceUri, null, userCredentials, null)
        {
            this._proxyManager = new AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService>(this);
        }

        protected override SecurityTokenResponse AuthenticateDeviceCore()
        {
            return this._proxyManager.AuthenticateDevice();
        }

        protected override void AuthenticateCore()
        {
            this._proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            this._proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }
    }

    internal sealed class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
    {
        private AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService> _proxyManager;

        public ManagedTokenOrganizationServiceProxy(Uri serviceUri, ClientCredentials userCredentials)
            : base(serviceUri, null, userCredentials, null)
        {
            this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        protected override SecurityTokenResponse AuthenticateDeviceCore()
        {
            return this._proxyManager.AuthenticateDevice();
        }

        protected override void AuthenticateCore()
        {
            this._proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            this._proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }
    }

    public class CrmServiceEngine
    {
        #region Construtor

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="organization">Organização</param>
        public CrmServiceEngine(string organization)
        {
            this.OrganizationName = organization;
            this.AuthFailureCount = 0;
            this.Tempo = DateTime.Now;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="organization">Organização.</param>
        /// <param name="user">Login do usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <param name="domain">Domínio da aplicação.</param>
        public CrmServiceEngine(string organization, string user, string password, string domain)
        {
            this.OrganizationName = organization;
            this._user = user;
            this._password = password;
            this._domain = domain;
            this.AuthFailureCount = 0;
            this.Tempo = DateTime.Now;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="organization">Organização.</param>
        /// <param name="user">Login do usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        /// <param name="domain">Domínio da aplicação.</param>
        public CrmServiceEngine(string organization, string user, string password, string domain, bool isOffline)
        {
            this.OrganizationName = organization;
            this._user = user;
            this._password = password;
            this._domain = domain;
            this.IsOffline = isOffline;
            this.AuthFailureCount = 0;
            this.Tempo = DateTime.Now;
        }

        #endregion

        #region Variáveis

        public string UserPrincipalName { get; set; }
        public int AuthFailureCount { get; set; }
        public Uri HomeRealmUri { get; set; }

        public string OrganizationName { get; set; }
        public bool IsOffline { get; set; }
        public DateTime Tempo;

        private string _user { get; set; }
        private string _password { get; set; }
        private string _domain { get; set; }
        private static AuthenticationProviderType Authentication { get; set; }
        private static OrganizationDetailCollection Organizations { get; set; }

        #endregion

        #region Propriedades

        private Uri _organizationUri;
        public Uri OrganizationUri
        {
            get
            {
                if (_organizationUri == null)
                    _organizationUri = this.GetOrganizationAddress(this.DiscoveryUri, this.OrganizationName);
                return _organizationUri;
            }
        }

        private Uri _organizationUriOffline;
        public Uri OrganizationUriOffline
        {
            get
            {
                if (_organizationUriOffline == null)
                    _organizationUriOffline = new Uri(string.Format("{0}/XRMServices/2011/Organization.svc", this.GetServerAddress()));
                return _organizationUriOffline;
            }
        }

        private Uri _discoveryUri;
        public Uri DiscoveryUri
        {
            get
            {
                if (_discoveryUri == null)
                    _discoveryUri = new Uri(string.Format(SDKore.Crm.Constants.DiscoveryService, this.GetServerAddress()));
                return _discoveryUri;
            }
        }

        private ClientCredentials _userCredentials;
        public ClientCredentials UserCredentials
        {
            get
            {
                if (_userCredentials == null)
                    _userCredentials = this.GetUserLogonCredentials(this._user, this._password, this._domain);
                return _userCredentials;
            }
        }

        private ClientCredentials _userCredentialsOffline;
        public ClientCredentials UserCredentialsOffline
        {
            get
            {
                if (_userCredentialsOffline == null)
                    _userCredentialsOffline = this.GetUserLogonCredentialsOffline(this._user, this._password, this._domain);
                return _userCredentialsOffline;
            }
        }

        private ClientCredentials _deviceCredentials;
        public ClientCredentials DeviceCredentials
        {
            get
            {
                if (_deviceCredentials == null)
                    _deviceCredentials = this.GetDeviceCredentials();
                return _deviceCredentials;
            }
        }

        private volatile OrganizationServiceProxy _organizationService = null;
        public OrganizationServiceProxy OrganizationService
        {
            get
            {
                lock (this)
                {
                    if (null == this._organizationService)
                    {
                        this.InitializeOrganizationService();
                    }

                    return this._organizationService;
                }
            }
        }

        private DiscoveryServiceProxy _discoveryService = null;
        public DiscoveryServiceProxy DiscoveryService
        {
            get
            {
                if (null == this._discoveryService)
                {
                    lock (this)
                    {
                        if (this._discoveryService == null)
                        {
                            ServicePointManager.ServerCertificateValidationCallback += MyCertificateValidation;

                            this._discoveryService = new ManagedTokenDiscoveryServiceProxy(this.DiscoveryUri, this.UserCredentials);
                        }
                    }
                }

                return this._discoveryService;
            }
        }

        #endregion

        #region Métodos

        protected virtual ClientCredentials GetDeviceCredentials()
        {
            return null;
        }

        /// <summary>
        /// Obter credencial do usuário logado.
        /// </summary>
        /// <returns></returns>
        protected virtual ClientCredentials GetUserLogonCredentialsOffline(string user, string password, string domain)
        {
            ClientCredentials credentials = new ClientCredentials(); ;
            credentials.Windows.ClientCredential = new System.Net.NetworkCredential(user, password, domain);
            return credentials;
        }

        /// <summary>
        /// Obter credencial do usuário logado.
        /// </summary>
        /// <returns></returns>
        protected virtual ClientCredentials GetUserLogonCredentials(string user, string password, string domain)
        {
            ClientCredentials credentials = new ClientCredentials(); ;
            var authenticationType = GetServerType(DiscoveryUri);

            switch (authenticationType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    if (string.IsNullOrEmpty(user) && string.IsNullOrEmpty(password))
                        credentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
                    else
                        credentials.Windows.ClientCredential = new System.Net.NetworkCredential(user, password, domain);
                    break;
                case AuthenticationProviderType.Federation:
                    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(user, password, domain);

                    credentials.UserName.UserName = string.Format("{0}\\{1}", domain, user);
                    credentials.UserName.Password = password;

                    break;
                case AuthenticationProviderType.LiveId:
                    credentials.UserName.UserName = user;
                    credentials.UserName.Password = password;

                    break;
                case AuthenticationProviderType.OnlineFederation:
                    //if (this.AuthFailureCount == 0)
                    //{
                    //    if (String.IsNullOrWhiteSpace(this.UserPrincipalName))
                    //        this.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;
                    //    return null;
                    //}

                    this.UserPrincipalName = String.Empty;
                    credentials.UserName.UserName = user;
                    credentials.UserName.Password = password;
                    break;
                case AuthenticationProviderType.None:
                    credentials = null;
                    break;
            }
            return credentials;
        }

        /// <summary>
        /// Obtém o tipo de autenticação.
        /// </summary>
        /// <param name="uri">URI do DicoveryService do CRM.</param>
        /// <returns>Tipo de autenticação.</returns>
        private static AuthenticationProviderType GetServerType(Uri uri)
        {
            if (Authentication == AuthenticationProviderType.None)
                Authentication = ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(uri).AuthenticationType;

            return Authentication;
        }

        /// <summary>
        /// Executa a consulta do DiscoveryService.
        /// </summary>
        /// <param name="service">DiscoveryService</param>
        /// <returns></returns>
        public OrganizationDetailCollection DiscoverOrganizations(IDiscoveryService service)
        {
            if (Organizations == null)
            {
                RetrieveOrganizationsRequest orgRequest = new RetrieveOrganizationsRequest();
                RetrieveOrganizationsResponse orgResponse = (RetrieveOrganizationsResponse)service.Execute(orgRequest);
                Organizations = orgResponse.Details;
            }

            return Organizations;
        }

        /// <summary>
        /// Retornar o host do Servidor CRM 2011.
        /// </summary>
        /// <returns></returns>
        protected virtual String GetServerAddress()
        {
            if (this.IsOffline)
                return SDKore.Configuration.ConfigurationManager.CrmServiceOffline;

            return SDKore.Configuration.ConfigurationManager.GetCrmService(this.OrganizationName);
        }

        private Uri GetOrganizationAddress(string organization, OrganizationDetailCollection orgs)
        {
            if (orgs == null)
                return null;

            Uri organizationAddress = null;
            for (int index = 0; index < orgs.Count; index++)
            {
                if (orgs[index].UniqueName.ToUpper() != organization.ToUpper())
                    continue;

                organizationAddress = new Uri(orgs[index].Endpoints[EndpointType.OrganizationService]);
                break;
            }

            return organizationAddress;
        }

        /// <summary>
        /// obtém o endereço do serviço do CRM 2011.
        /// </summary>
        /// <param name="discoveryServiceUri">URI do Discovery Service CRM 2011</param>
        /// <param name="organization">Organização.</param>
        /// <returns></returns>
        protected virtual Uri GetOrganizationAddress(Uri discoveryServiceUri, string organization)
        {
            Uri organizationAddress = GetOrganizationAddress(organization, Organizations);
            if (organizationAddress == null)
            {
                
                //using (DiscoveryServiceProxy serviceProxy = GetDiscoveryProxy())
                using (DiscoveryServiceProxy serviceProxy = this.DiscoveryService)
                {
                    if (serviceProxy == null)
                        throw new ArgumentException("Servidor informado no arquivo de configuração é inválido ");

                    OrganizationDetailCollection orgs = DiscoverOrganizations(serviceProxy);
                    if (orgs == null || orgs.Count == 0)
                        throw new ArgumentException("Usuário informado não possui nenhuma organização.");

                    for (int index = 0; index < orgs.Count; index++)
                    {
                        if (orgs[index].UniqueName.ToUpper() != organization.ToUpper())
                            continue;

                        organizationAddress = new Uri(orgs[index].Endpoints[EndpointType.OrganizationService]);
                        break;
                    }
                }

            }
            return organizationAddress;
        }

        /// <summary>
        /// Initialize the CrmService
        /// </summary>
        private void InitializeOrganizationService()
        {
            lock (this)
            {
                this._organizationService = new ManagedTokenOrganizationServiceProxy(OrganizationUri, UserCredentials);
                //this.m_organizationService.EnableProxyTypes();
            }
        }

        private static bool MyCertificateValidation(Object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors Errors)
        {
            return true;
        }

        /// <summary>
        /// Get the discovery service proxy based on existing configuration data.
        /// Added new way of getting discovery proxy.
        /// Also preserving old way of getting discovery proxy to support old scenarios.
        /// </summary>
        /// <returns>An instance of DiscoveryServiceProxy</returns>
        private DiscoveryServiceProxy GetDiscoveryProxy()
        {
            IServiceManagement<IDiscoveryService> serviceManagement = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(DiscoveryUri);
            Authentication = serviceManagement.AuthenticationType;

            // Get the logon credentials.
            _userCredentials = GetUserLogonCredentials(this._user, this._password, this._domain);

            AuthenticationCredentials authCredentials = new AuthenticationCredentials();

            if (!String.IsNullOrWhiteSpace(this.UserPrincipalName))
            {
                // Try to authenticate the Federated Identity organization with UserPrinicipalName.
                authCredentials.UserPrincipalName = this.UserPrincipalName;
                try
                {
                    AuthenticationCredentials tokenCredentials = serviceManagement.Authenticate(authCredentials);
                    DiscoveryServiceProxy discoveryProxy = new DiscoveryServiceProxy(serviceManagement, tokenCredentials.SecurityTokenResponse);
                    // Checking authentication by invoking some SDK methods.
                    OrganizationDetailCollection orgs = DiscoverOrganizations(discoveryProxy);
                    return discoveryProxy;
                }
                catch (System.ServiceModel.Security.SecurityAccessDeniedException ex)
                {
                    // If authentication failed using current UserPrincipalName, 
                    // request UserName and Password to try to authenticate using user credentials.
                    if (ex.Message.Contains("Access is denied."))
                    {
                        this.AuthFailureCount += 1;
                        authCredentials.UserPrincipalName = String.Empty;

                        _userCredentials = GetUserLogonCredentials(this._user, this._password, this._domain);
                    }
                    else
                    { throw ex; }
                }
            }

            // Resetting credentials in the AuthenicationCredentials.  
            if (Authentication != AuthenticationProviderType.ActiveDirectory)
            {
                authCredentials = new AuthenticationCredentials();
                authCredentials.ClientCredentials = UserCredentials;

                if (Authentication == AuthenticationProviderType.LiveId)
                {
                    authCredentials.SupportingCredentials = new AuthenticationCredentials();
                    authCredentials.SupportingCredentials.ClientCredentials = UserCredentials;
                }
                // Try to authenticate with the user credentials.
                AuthenticationCredentials tokenCredentials1 = serviceManagement.Authenticate(authCredentials);
                return new DiscoveryServiceProxy(serviceManagement, tokenCredentials1.SecurityTokenResponse);
            }
            // For an on-premises environment.
            return new DiscoveryServiceProxy(serviceManagement, UserCredentials);
        }

        #endregion

        ///// <summary>
        ///// Obtém todas as organizações configuradas para o MS CRM.
        ///// </summary>
        ///// <returns></returns>
        //public List<OrganizationDetail> GetOrganizations()
        //{
        //    using (DiscoveryServiceProxy serviceProxy = new DiscoveryServiceProxy(this.DiscoveryUri, null, this.UserCredentials, this.DeviceCredentials))
        //    {
        //        if (serviceProxy == null)
        //            throw new Exception("Nome do servidor inválido.");

        //        OrganizationDetailCollection orgs = DiscoverOrganizations(serviceProxy);
        //        if (orgs.Count == 0)
        //            throw new Exception("Servidor sem nenhuma organização.");

        //        List<OrganizationDetail> lista = new List<OrganizationDetail>();
        //        foreach (OrganizationDetail item in orgs)
        //            lista.Add(item);

        //        return lista;
        //    }
        //}



        //public void ConfigureOrganizationServiceProxy()
        //{
        //// Set IServiceManagement for the current organization.
        //IServiceManagement<IOrganizationService> orgServiceManagement =ServiceConfigurationFactory.CreateManagement<IOrganizationService>(this.OrganizationUri);
        //this.OrganizationServiceManagement = orgServiceManagement;

        //// Set SecurityTokenResponse for the current organization.
        //if (Authentication != AuthenticationProviderType.ActiveDirectory)
        //{
        //    // Set the credentials.
        //    AuthenticationCredentials authCredentials = new AuthenticationCredentials();
        //    // If UserPrincipalName exists, use it. Otherwise, set the logon credentials from the configuration.
        //    if (!String.IsNullOrWhiteSpace(this.UserPrincipalName))
        //    {
        //        authCredentials.UserPrincipalName = this.UserPrincipalName;
        //    }
        //    else
        //    {
        //        authCredentials.ClientCredentials = this.UserCredentials;
        //        if (Authentication == AuthenticationProviderType.LiveId)
        //        {
        //            authCredentials.SupportingCredentials = new AuthenticationCredentials();
        //            authCredentials.SupportingCredentials.ClientCredentials = this.DeviceCredentials;
        //        }
        //    }
        //    AuthenticationCredentials tokenCredentials = orgServiceManagement.Authenticate(authCredentials);

        //    if (tokenCredentials != null)
        //    {
        //        if (tokenCredentials.SecurityTokenResponse != null)
        //            this.OrganizationTokenResponse = tokenCredentials.SecurityTokenResponse;
        //    }
        //}
        //this.Tempo = DateTime.Now.AddHours(4);
        //}
    }
}
