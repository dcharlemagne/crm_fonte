using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ColaboradorTreinadoCertificadoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ColaboradorTreinadoCertificadoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ColaboradorTreinadoCertificadoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ColaboradorTreinadoCertificadoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Metodos



        public ColaboradorTreinadoCertificado ObterPor(Guid colaboradorId)
        {
            return RepositoryService.ColaboradorTreinadoCertificado.ObterPor(colaboradorId);
        }



        public ColaboradorTreinadoCertificado Persistir(Model.ColaboradorTreinadoCertificado objColaboradorTreinadoCertificado)
        {
            ColaboradorTreinadoCertificado TmpColaboradorTreinadoCertificado = null;
            if (objColaboradorTreinadoCertificado.IdMatricula.HasValue)
            {
                TmpColaboradorTreinadoCertificado = RepositoryService.ColaboradorTreinadoCertificado.ObterPor(objColaboradorTreinadoCertificado.IdMatricula.Value);

                if (TmpColaboradorTreinadoCertificado != null)
                {
                    objColaboradorTreinadoCertificado.ID = TmpColaboradorTreinadoCertificado.ID;
                    
                    //Altera Status - Se necessário
                    if (!TmpColaboradorTreinadoCertificado.Status.Equals(objColaboradorTreinadoCertificado.Status) && objColaboradorTreinadoCertificado.Status != null)
                        this.MudarStatus(TmpColaboradorTreinadoCertificado.ID.Value, objColaboradorTreinadoCertificado.Status.Value);

                    RepositoryService.ColaboradorTreinadoCertificado.Update(objColaboradorTreinadoCertificado);

                    return TmpColaboradorTreinadoCertificado;
                }
                else
                {
                    objColaboradorTreinadoCertificado.ID = RepositoryService.ColaboradorTreinadoCertificado.Create(objColaboradorTreinadoCertificado);
                    return objColaboradorTreinadoCertificado;
                }
            }
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.ColaboradorTreinadoCertificado.AlterarStatus(id, status);
        }



        #endregion

    }
}
