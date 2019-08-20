using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SharepointService
    {
        public static string URLPortal { get; set; }

        //private static SPAuthenticationService.Authentication SPAuthWebServices
        //{
        //    get
        //    {
        //        var authProxy = new SPAuthenticationService.Authentication();
        //        authProxy.Url = string.Format("{0}/_vti_bin/Authentication.asmx", URLPortal);
        //        authProxy.CookieContainer = new CookieContainer();
        //        var loginResult = authProxy.Login(TrideaConfigurationManager.GetSettingValue("SPWebServiceLogin"), TrideaConfigurationManager.GetSettingValue("SPWebServicePass"));

        //        // Caso a autenticação do WebService não seja validado, então gerar a exception abaixo.
        //        if (loginResult.ErrorCode != SPAuthenticationService.LoginErrorCode.NoError)
        //            throw new Exception(string.Format("Erro ao Autenticar o WebService do SharePoint --> ErrorCode: {0}", loginResult.ErrorCode.ToString()));

        //        return authProxy;
        //    }
        //}
        //private static SPPeopleService.People SPPeopleServices
        //{
        //    get
        //    {
        //        var peopleService = new SPPeopleService.People();
        //        peopleService.Url = string.Format("{0}/_vti_bin/People.asmx", URLPortal);
        //        peopleService.CookieContainer = SPAuthWebServices.CookieContainer;

        //        return peopleService;
        //    }
        //}
        //private static SPUserGroupService.UserGroup SPUserGroupServices
        //{
        //    get
        //    {
        //        var serviceUserGroup = new SPUserGroupService.UserGroup();
        //        serviceUserGroup.Url = string.Format("{0}/_vti_bin/UserGroup.asmx", URLPortal);
        //        serviceUserGroup.CookieContainer = SPAuthWebServices.CookieContainer;

        //        return serviceUserGroup;
        //    }
        //}

        //public static bool AdicionarOuRemoverUsuario(string NomeGrupo, string Login, string UrlPortal, bool AdicionarOuRemover)
        //{
        //    try
        //    {
        //        URLPortal = UrlPortal;

        //        var pInfo = SPPeopleServices.ResolvePrincipals(new string[] { Login }, SPPeopleService.SPPrincipalType.User, true);

        //        if (pInfo != null)
        //            if (pInfo.Count() > 0)
        //            {
        //                var user = pInfo[0];
        //                if (user.MoreMatches != null)
        //                    if (user.MoreMatches.Count() > 0)
        //                    {
        //                        var u = user.MoreMatches.Where(_u => _u.AccountName.Contains("provader")).FirstOrDefault();
        //                        if (u != null)
        //                        {
        //                            //var serviceUserGroup = new SPUserGroupService.UserGroup();
        //                            //serviceUserGroup.Url = string.Format("{0}/_vti_bin/UserGroup.asmx", UrlPortal);
        //                            //serviceUserGroup.CookieContainer = authProxy.CookieContainer;

        //                            if (AdicionarOuRemover)
        //                            {
        //                                SPUserGroupServices.AddUserToGroup(NomeGrupo, u.DisplayName, u.AccountName, u.Email, "Adicionado via Plugin");
        //                                //MensagemRetorno = "Usuário adicionado com sucesso.";
        //                                return true;
        //                            }
        //                            else
        //                            {
        //                                SPUserGroupServices.RemoveUserFromGroup(NomeGrupo, u.AccountName);
        //                            }
        //                        }
        //                    }
        //            }

        //        return false;
        //    }
        //    finally
        //    {
        //        SPAuthWebServices.Dispose();
        //        SPPeopleServices.Dispose();
        //        SPUserGroupServices.Dispose();
        //    }
        //}
    }
}
