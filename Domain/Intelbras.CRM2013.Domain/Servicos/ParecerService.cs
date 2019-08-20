using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ParecerService
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ParecerService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ParecerService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ParecerService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Propriedades

        #endregion

        #region Métodos

        public Parecer Persistir(Parecer ObjParecer)
        {
            Parecer TmpParecer = null;

            if (ObjParecer.ID.HasValue)
            {
                TmpParecer = RepositoryService.Parecer.ObterPor((Guid)ObjParecer.ID);

                if (TmpParecer != null)
                {
                    ObjParecer.ID = TmpParecer.ID;

                    RepositoryService.Parecer.Update(ObjParecer);

                    if (!TmpParecer.Status.Equals(ObjParecer.Status) && ObjParecer.Status != null)
                        this.MudarStatus(TmpParecer.ID.Value, ObjParecer.Status.Value);

                    return TmpParecer;
                }
                else
                    return null;
            }
            else
            {
                ObjParecer.ID = RepositoryService.Parecer.Create(ObjParecer);
                return ObjParecer;
            }
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Parecer.AlterarStatus(id, status);
        }

        public List<Parecer> ListarPor(Guid TarefaId)
        {
            List<Parecer> lstResultado = RepositoryService.Parecer.ListarPor(TarefaId);

            return lstResultado;
        }

        public Parecer ObterPor(Guid itbc_parecerid)
        {
            return RepositoryService.Parecer.ObterPor(itbc_parecerid);

        }
        //public TipoDeAcessoExtranet buscaTipoAcesso(Guid ObjTipoAcesso)
        //{
        //    TipoDeAcessoExtranet TmpTipoAcesso = null;

        //    TmpTipoAcesso = RepositoryService.TipoAcessoExtranet.ObterPor(ObjTipoAcesso);

        //    if (TmpTipoAcesso != null)
        //    {
        //        return TmpTipoAcesso;
        //    }

        //    return null;
        //}
        #endregion

       

    }
}


