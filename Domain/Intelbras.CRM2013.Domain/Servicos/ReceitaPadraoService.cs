using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ReceitaPadraoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ReceitaPadraoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public ReceitaPadraoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public ReceitaPadrao Persistir(Guid usuarioId, Model.ReceitaPadrao receitaPadrao)
        {
            ReceitaPadrao receitaPadraoAtual = new ReceitaPadrao(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

            receitaPadraoAtual = RepositoryService.ReceitaPadrao.ObterPor((int)receitaPadrao.CodReceitaPadrao);

            if (receitaPadraoAtual != null)
            {
                receitaPadrao.ID = receitaPadraoAtual.ID;

                RepositoryService.ReceitaPadrao.Update(receitaPadrao, usuarioId);

                if (receitaPadrao.State != null && !receitaPadraoAtual.State.Equals(receitaPadrao.State))
                    MudarStatus(receitaPadrao.ID.Value, receitaPadrao.State.Value, usuarioId);

                return receitaPadrao;
            }
            else
            {
                receitaPadrao.ID = RepositoryService.ReceitaPadrao.Create(receitaPadrao, usuarioId);
                return receitaPadrao;
            }
        }

        public ReferenciasCanal BuscaReferenciasCanal(Guid referenciasCanalID)
        {
            List<ReferenciasCanal> lstReferenciasCanal = RepositoryService.ReferenciasCanal.ListarPor(referenciasCanalID);
            if (lstReferenciasCanal.Count > 0)
                return lstReferenciasCanal.First<ReferenciasCanal>();
            return null;
        }

        public Model.ReceitaPadrao BuscaPorCodigo(int codigo)
        {
            return RepositoryService.ReceitaPadrao.ObterPor(codigo);            
        }


        public Model.ReceitaPadrao BuscaPorCodigo(Guid receitaId)
        {
            return RepositoryService.ReceitaPadrao.ObterPor(receitaId);
        }

        public void MudarStatus(Guid receitaId, int state, Guid usuarioId)
        {
            int statuscode = 0;

            switch (state)
            {
                case (int)Enum.StateCode.Ativo :
                    statuscode = (int)Enum.ReceitaPadrao.Status.Ativo;
                    break;

                case (int)Enum.StateCode.Inativo :
                    statuscode = (int)Enum.ReceitaPadrao.Status.Inativo;
                    break;
            }

            RepositoryService.ReceitaPadrao.AlterarStatus(receitaId, state, statuscode, usuarioId);
        }

        #endregion

    }
}
