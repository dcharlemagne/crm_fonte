using System;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class RelacionamentoB2BService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RelacionamentoB2BService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public RelacionamentoB2BService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public RelacionamentoB2BService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public RelacionamentoB2B Persistir(RelacionamentoB2B relacionamento)
        {
            RelacionamentoB2B relacionamentoAtual = RepositoryService.RelacionamentoB2B.ObterPor(relacionamento.CodigoRelacionamentoB2B);
            if (relacionamentoAtual == null)
            {
                relacionamento.ID = RepositoryService.RelacionamentoB2B.Create(relacionamento);
                return relacionamento;
            }

            relacionamento.ID = relacionamentoAtual.ID;

            RepositoryService.RelacionamentoB2B.Update(relacionamento);
            if (relacionamento.Status != null && !relacionamentoAtual.Status.Equals(relacionamento.Status))
                MudarStatus(relacionamento.ID.Value, relacionamento.Status.Value);

            return relacionamento;
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            int statuscode = 0;
            switch (stateCode)
            {
                case (int)Enum.StateCode.Ativo:
                    statuscode = (int)Enum.RelacionamentoB2B.Status.Ativo;
                    break;

                case (int)Enum.StateCode.Inativo:
                    statuscode = (int)Enum.RelacionamentoB2B.Status.Inativo;
                    break;
            }

            return RepositoryService.RelacionamentoB2B.AlterarStatus(id, stateCode, statuscode);
        }
    }
}
