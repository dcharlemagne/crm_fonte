using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_histricodecategoriasdaconta")]
    public class HistoricoCategoria : IntegracaoBase
    {
        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoCategoria() { }

        public HistoricoCategoria(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoCategoria(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_histricodecategoriasdacontaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_contaid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_categoriaanterior")]
        public Lookup CategoriaAnterior { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup CategoriaAtual { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime DataCriacao { get; set; }

        #endregion

    }
}
