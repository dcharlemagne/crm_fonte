using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class UnidadeService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UnidadeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public UnidadeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public Unidade BuscaUnidadePorNome(String nomeUnidade)
        {
            List<Unidade> lstUnidade = RepositoryService.Unidade.ListarPor(nomeUnidade);
            if (lstUnidade.Count > 0)
                return lstUnidade.First<Unidade>();
            return null;
        }

        public List<Unidade> BuscaUnidadePorNome(String[] nomeUnidade)
        {
            List<Unidade> lstUnidade = RepositoryService.Unidade.ListarPor(nomeUnidade);
            if (lstUnidade.Count > 0)
                return lstUnidade;
            return null;
        }

        public Unidade ExtrairUnidadePorNome(String nomeUnidade, List<Unidade> listaUnidades)
        {
            foreach (var item in listaUnidades)
                if (item.Nome.ToLower() == nomeUnidade.ToLower())
                    return item;
            return null;
        }

        public Unidade BuscaPor(Guid unidadeId)
        {
            return RepositoryService.Unidade.ObterPor(unidadeId);
        }

        public List<Unidade> ListarTodos()
        {
            return RepositoryService.Unidade.ListarTodos();
        }
        public Unidade Persistir(Unidade objUnidade)
        {
            Unidade tmpUnidade = null;
            if (objUnidade.ID.HasValue)
            {
                tmpUnidade = RepositoryService.Unidade.ObterPor(objUnidade.ID.Value);

                if (tmpUnidade != null)
                {
                    objUnidade.ID = tmpUnidade.ID;
                    RepositoryService.Unidade.Update(objUnidade);

                    return tmpUnidade;
                }
                else
                    return null;
            }
            else
            {
                objUnidade.ID = RepositoryService.Unidade.Create(objUnidade);
                return objUnidade;
            }
        }

    }
}
