using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.Crm.Sdk.Messages;

namespace SDKore.Crm
{
    public class CrmHelper
    {
        private string _organization;
        private string _user;
        private string _password;
        private string _domain;

        private CrmServiceEngine _crmEngine;
        private CrmServiceEngine CrmEngine
        {
            get
            {
                if (_crmEngine == null)
                    _crmEngine = new CrmServiceEngine(_organization, _user, _password, _domain);                    

                return _crmEngine;
            }
        }       


        /// <summary>
        /// Cria a instância do serviço do CRM.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <param name="user">Login do usuário</param>
        /// <param name="password">Password do usuário</param>
        /// <param name="domain">Domínio da aplicação</param>
        /// <returns>Objeto OrganizationService configurado.</returns>
        public OrganizationServiceProxy GetOrganizationProxy(string organization, string user, string password, string domain)
        {
            _organization = organization;
            _user = user;
            _password = password;
            _domain = domain;

            return CrmEngine.OrganizationService;
        }

        /// <summary>
        /// Cria a instância do serviço do CRM.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <param name="user">Login do usuário</param>
        /// <param name="password">Password do usuário</param>
        /// <param name="domain">Domínio da aplicação</param>
        /// <returns>Objeto OrganizationService configurado.</returns>
        public OrganizationServiceProxy GetOrganizationProxy(string organization, string user, string password, string domain, bool isOffline)
        {
            if (!isOffline)
                return GetOrganizationProxy(organization, user, password, domain);

            var crmEngine = new CrmServiceEngine(organization, user, password, domain, isOffline);
            var serviceProxy = new OrganizationServiceProxy(crmEngine.OrganizationUriOffline, crmEngine.HomeRealmUri, crmEngine.UserCredentialsOffline, crmEngine.DeviceCredentials);
            return serviceProxy;
        }

    }
}



#region OLD
///// <summary>
///// Obtains the organization service proxy.
///// </summary>
//public OrganizationServiceProxy GetOrganizationProxyWithDefaultCredentials()
//{
//    var credentials = new ClientCredentials();
//    credentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;

//    return new OrganizationServiceProxy(CrmEngine.OrganizationServiceManagement, credentials);
//}


///// <summary>
///// Obtains the organization service proxy.
///// </summary>
///// <param name="serverConfiguration"></param>
///// <returns></returns>
//public OrganizationServiceProxy GetOrganizationProxy()
//{
//    //if (CrmEngine.OrganizationServiceManagement == null)
//    //    throw new ArgumentNullException("Configuração do serviço não foi realizada. OrganizationServiceManagement");

//    //// Obtain the organization service proxy for the Federated, LiveId, and OnlineFederated environments. 
//    //if (CrmEngine.OrganizationServiceManagement != null && CrmEngine.OrganizationTokenResponse != null)
//    //    return new OrganizationServiceProxy(CrmEngine.OrganizationServiceManagement, CrmEngine.OrganizationTokenResponse);

//    //// Obtain the organization service proxy for the ActiveDirectory environment.
//    //return new OrganizationServiceProxy(CrmEngine.OrganizationServiceManagement, CrmEngine.UserCredentials);

//}


#endregion