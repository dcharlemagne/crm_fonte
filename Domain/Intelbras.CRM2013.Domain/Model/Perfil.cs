using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_perfil")]
    public class Perfil : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Perfil() { }

        public Perfil(String organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Perfil(String organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_perfilid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeDeNegocio { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }

        [LogicalAttribute("itbc_exclusividade")]
        public Boolean Exclusividade { get; set; }

        [LogicalAttribute("itbc_status")]
        public int? status { get; set; }

        #endregion
    }
}
