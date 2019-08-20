using System;
using System.Net;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SharepointServices
    {
        #region Construtores

        public SharepointServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
        }

        public SharepointServices(string organizacao, bool isOffline, object provider)
        {
            this.OrganizationName = organizacao;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(this.OrganizationName);
            Trace = new SDKore.Helper.Trace(organizacao);
        }

        #endregion

        #region Propriedades

        public string OrganizationName { get; set; }
        public bool IsOffline { get; set; }
        public SDKore.Helper.Trace Trace { get; set; }

        private Domain.IRepository.ISharePointSite<int> _repositorio;
        private Domain.IRepository.ISharePointSite<int> Repositorio
        {
            get
            {
                if (_repositorio == null)
                {
                    _repositorio = SDKore.DomainModel.RepositoryFactory.Instance.Container.Resolve<Domain.IRepository.ISharePointSite<int>>();
                    _repositorio.SetOrganization(this.OrganizationName);
                    _repositorio.SetIsOffline(this.IsOffline);
                }
                return _repositorio;
            }
        }

        #endregion

        /// <summary>
        /// Método responsável por criar o diretório no sharepoint e vincular ao registro do CRM.
        /// </summary>
        public void CriarDiretorio<T>(string nome, Guid id)
        {
            if (!string.IsNullOrEmpty(nome))
            {
                nome = nome.Replace("&", "e");
                nome = nome.Replace("/", "-");

                var nomeEntidade = SDKore.Crm.Util.Utility.GetEntityName<T>();
                var urlSite = Domain.Servicos.Helper.RemoverBarraDoFinal(SDKore.Configuration.ConfigurationManager.GetSettingValue("UrlSiteSharePoint"));
                var usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioSharePoint");
                var senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaSharePoint");
                var dominio = SDKore.Configuration.ConfigurationManager.GetDomain(this.OrganizationName);
                string urlCompleta = string.Format("{0}/{1}/{2}_{3}", urlSite, nomeEntidade, nome, id.ToString().Replace("-", ""));

                Trace.Add("Enviando requição para Sharepoint! UrlComplata {0}", urlCompleta);

                WebRequest req = WebRequest.Create(urlCompleta);
                req.Credentials = new NetworkCredential(SDKore.Helper.Cryptography.Decrypt(usuario), SDKore.Helper.Cryptography.Decrypt(senha), dominio);
                req.Method = "MKCOL";
                WebResponse response = req.GetResponse();
                response.Close();

                Trace.Add("Associando diretório ao registro!");

                Repositorio.AssociarDiretorioAoRegistroCRM(urlCompleta, nomeEntidade, nome, id);
            }
        }

    }
}
