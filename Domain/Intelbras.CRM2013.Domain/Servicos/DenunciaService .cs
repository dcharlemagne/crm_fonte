using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DenunciaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DenunciaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public DenunciaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        public TipoDeDenuncia ObterTipoDenuncia(Guid tipoDenuncia)
        {
            return RepositoryService.TipoDeDenuncia.ObterPor(tipoDenuncia);
        }

        public List<TipoDeDenuncia> ListarTipoDenuncia()
        {
            return RepositoryService.TipoDeDenuncia.Listar();
        }


        public Denuncia Persistir(Model.Denuncia objDenuncia)
        {
            Denuncia TmpDenuncia = null;
            if (objDenuncia.ID.HasValue)
            {
                TmpDenuncia = RepositoryService.Denuncia.ObterPor(objDenuncia.ID.Value);

                if (TmpDenuncia != null)
                {
                    RepositoryService.Denuncia.Update(objDenuncia);
                    //Altera Status - Se necessário
                    if (!TmpDenuncia.Status.Equals(objDenuncia.Status) && objDenuncia.Status != null)
                        this.MudarStatus(TmpDenuncia.ID.Value, objDenuncia.Status.Value);
                    return TmpDenuncia;
                }
                else
                    return null;
            }
            else
            {
                objDenuncia.ID = RepositoryService.Denuncia.Create(objDenuncia);
                return objDenuncia;
            }
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Denuncia.AlterarStatus(id, status);
        }

        public List<Denuncia> ListarDenuncias(DateTime dtInicio, DateTime dtFim, List<Guid> lstDenunciantes, List<Guid> lstDenunciados, Guid? representanteId, int? situacaoDenuncia)
        {
            return RepositoryService.Denuncia.ListarDenuncias(dtInicio, dtFim, lstDenunciantes, lstDenunciados, representanteId, situacaoDenuncia);
        }

        public Denuncia ObterDenuncia(Guid denunciaId)
        {
            return RepositoryService.Denuncia.ObterPor(denunciaId);
        }


    }
}
