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
    public class CategoriaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CategoriaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CategoriaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public string IntegracaoBarramento(Categoria categoria)
        {
            Domain.Integracao.MSG0018 msgCategoria = new Domain.Integracao.MSG0018(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgCategoria.Enviar(categoria);
        }

        public Categoria ObterPor(Guid categoriaId)
        {
            return RepositoryService.Categoria.ObterPor(categoriaId);
        }
        public Categoria BuscaCategoria(Guid itbc_Categoriaid)
        {
            Categoria categoria = RepositoryService.Categoria.ObterPor(itbc_Categoriaid);
            if (categoria != null)
                return categoria;
            return null;
        }
        
        #endregion
    }
}