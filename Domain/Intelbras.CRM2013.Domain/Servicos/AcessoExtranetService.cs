using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AcessoExtranetService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcessoExtranetService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public AcessoExtranetService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public Guid Persistir(AcessoExtranet ObjExtranet)
        {
            AcessoExtranet TmpAcessoExtranet = null;

            if(ObjExtranet.ID.HasValue)
                TmpAcessoExtranet = RepositoryService.AcessoExtranet.ObterPor((Guid)ObjExtranet.ID);

            if (TmpAcessoExtranet !=null)
            {
                ObjExtranet.ID = TmpAcessoExtranet.ID;

                RepositoryService.AcessoExtranet.Update(ObjExtranet);

                return TmpAcessoExtranet.ID.Value;
            }
            else
                return RepositoryService.AcessoExtranet.Create(ObjExtranet);
        }


        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.AcessoExtranetContato.AlterarStatus(id, status);
        }

        public AcessoExtranet buscaAcessoExtranet(Guid perfil)
        {
            AcessoExtranet TmpAcessoExtranet = null;

            TmpAcessoExtranet = RepositoryService.AcessoExtranet.ObterPor(perfil);
             
            if (TmpAcessoExtranet != null)
            {
                return TmpAcessoExtranet;
            }

            return null;
        }

        public List<AcessoExtranet> ListarAcessoExtranet(Guid tipoAcesso)
        {
            List<AcessoExtranet> lstAcessoExtranet = new List<AcessoExtranet>();
            return lstAcessoExtranet = RepositoryService.AcessoExtranet.ListarPor(tipoAcesso);
        }

        public string IntegracaoBarramento(AcessoExtranet objAcessoExtranet)
        {
            Domain.Integracao.MSG0056 msgProdEstab = new Domain.Integracao.MSG0056(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstab.Enviar(objAcessoExtranet);
        }

        public AcessoExtranet ObterPor(Guid acessoExtId)
        {
            return RepositoryService.AcessoExtranet.ObterPor(acessoExtId);
        }
        
        #endregion



    }
}


