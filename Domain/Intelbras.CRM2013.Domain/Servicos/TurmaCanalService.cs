using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TurmaCanalService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TurmaCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TurmaCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public TurmaCanalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Metodos

        public List<TurmaCanal> ListarPor(Guid turmaId)
        {
            return RepositoryService.TurmaCanal.ListarPor(turmaId);
        }

        public TurmaCanal ObterPor(Guid turmaId)
        {
            return RepositoryService.TurmaCanal.ObterPor(turmaId);
        }

        public TurmaCanal Persistir(Model.TurmaCanal objTurmaCanal)
        {
            TurmaCanal TmpTurmaCanal = null;
            if (!String.IsNullOrEmpty(objTurmaCanal.IdTurma))
            {
                TmpTurmaCanal = RepositoryService.TurmaCanal.ObterPorIdTurma(objTurmaCanal.IdTurma);

                if (TmpTurmaCanal != null)
                {
                    objTurmaCanal.ID = TmpTurmaCanal.ID;
                    //Altera Status - Se necessário
                    if (!TmpTurmaCanal.State.Equals(objTurmaCanal.State) && objTurmaCanal.State != null)
                        this.MudarStatus(TmpTurmaCanal.ID.Value, objTurmaCanal.State.Value);

                    RepositoryService.TurmaCanal.Update(objTurmaCanal);

                    return TmpTurmaCanal;
                }
                else
                {
                    objTurmaCanal.ID = RepositoryService.TurmaCanal.Create(objTurmaCanal);
                    return objTurmaCanal;
                }
            }
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.TurmaCanal.AlterarStatus(id, status);
        }



        #endregion



    }
}
