using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TreinamentoCertificacaoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TreinamentoCertificacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TreinamentoCertificacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public TreinamentoCertificacaoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Metodos

        public List<TreinamentoCertificacao> ListarPor(Guid treinamentoId)
        {
            return RepositoryService.TreinamentoCertificacao.ListarPor(treinamentoId);
        }

        public TreinamentoCertificacao ObterPor(Guid treinamentoId)
        {
            return RepositoryService.TreinamentoCertificacao.ObterPor(treinamentoId);
        }

        public TreinamentoCertificacao ObterPor(Int32 IdInterno)
        {
            return RepositoryService.TreinamentoCertificacao.ObterPor(IdInterno);
        }

        public TreinamentoCertificacao Persistir(Model.TreinamentoCertificacao objTreinamentoCertificacao)
        {
            TreinamentoCertificacao TmpTreinamentoCertificacao = null;
            if (objTreinamentoCertificacao.IdCurso.HasValue)
            {
                TmpTreinamentoCertificacao = RepositoryService.TreinamentoCertificacao.ObterPor(objTreinamentoCertificacao.IdCurso.Value);

                if (TmpTreinamentoCertificacao != null)
                {
                    objTreinamentoCertificacao.ID = TmpTreinamentoCertificacao.ID;
                    
                    //Altera Status - Se necessário
                    if (!TmpTreinamentoCertificacao.State.Equals(objTreinamentoCertificacao.State) && objTreinamentoCertificacao.State != null)
                        this.MudarStatus(TmpTreinamentoCertificacao.ID.Value, objTreinamentoCertificacao.State.Value);

                    RepositoryService.TreinamentoCertificacao.Update(objTreinamentoCertificacao);

                    return TmpTreinamentoCertificacao;
                }
                else
                {
                    objTreinamentoCertificacao.ID = RepositoryService.TreinamentoCertificacao.Create(objTreinamentoCertificacao);

                    if(objTreinamentoCertificacao.State == (int)Enum.TreinamentoCanal.State.Inativo)
                        this.MudarStatus(objTreinamentoCertificacao.ID.Value, (int)Enum.TreinamentoCanal.State.Inativo);

                    return objTreinamentoCertificacao;
                } 
            }

            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.TreinamentoCertificacao.AlterarStatus(id, status);
        }



        #endregion

    }
}
