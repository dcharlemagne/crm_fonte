using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_famildeprod")]
    public class FamiliaProduto : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FamiliaProduto() { }

        public FamiliaProduto(string organization, bool isOffline)
                : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public FamiliaProduto(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_famildeprodid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_familia")]
        public String Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_segmentoid")]
        public Lookup Segmento { get; set; }

        [LogicalAttribute("itbc_desconto_verde_habilitado")]
        public Boolean DescontoVerdeHabilitado { get; set; }

        [LogicalAttribute("itbc_percentual_desconto_verde")]
        public Decimal? PercentualDescontoVerde { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }
        #endregion
    }
}
