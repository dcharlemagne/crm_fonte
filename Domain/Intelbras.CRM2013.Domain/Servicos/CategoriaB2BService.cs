using System;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class CategoriaB2BService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CategoriaB2BService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CategoriaB2BService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public CategoriaB2B ObterPor(int codigoCategoriaB2B)
        {
            return RepositoryService.CategoriaB2B.ObterPor(codigoCategoriaB2B);
        }

        public CategoriaB2B Persistir(CategoriaB2B categoria)
        {
            CategoriaB2B categoriaAtual = new CategoriaB2B(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            categoriaAtual = RepositoryService.CategoriaB2B.ObterPor((int)categoria.CodigoCategoriaB2B);

            if(categoriaAtual == null)
            {
                categoria.ID = RepositoryService.CategoriaB2B.Create(categoria);
                return categoria;
            }

            categoria.ID = categoriaAtual.ID;

            RepositoryService.CategoriaB2B.Update(categoria);
            if (categoria.Status != null && !categoriaAtual.Status.Equals(categoria.Status))
                MudarStatus(categoria.ID.Value, categoria.Status.Value);

            return categoria;
        }

        public void MudarStatus(Guid guid, int state)
        {
            int statuscode = 0;
            switch (state)
            {
                case (int)Enum.StateCode.Ativo:
                    statuscode = (int)Enum.CategoriaB2B.Status.Ativo;
                    break;

                case (int)Enum.StateCode.Inativo:
                    statuscode = (int)Enum.CategoriaB2B.Status.Inativo;
                    break;
            }
            RepositoryService.CategoriaB2B.AlterarStatus(guid, state, statuscode);
        }

        #endregion
    }
}