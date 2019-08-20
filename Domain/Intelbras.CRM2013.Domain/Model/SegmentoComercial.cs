using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_segmentocomercial")]
    public class SegmentoComercial : IntegracaoBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public SegmentoComercial() { }

        public SegmentoComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SegmentoComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_segmentocomercialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_site")]
        public int? CodigoSegmento { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_segmento_comercial_pai_id")]        
        public Lookup CodigoSegmentoPai { get; set; }

        [LogicalAttribute("itbc_tiposegmentosite")]
        public String TipoSegmento { get; set; }

        [LogicalAttribute("itbc_ordem")]
        public int? Ordem { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }
        #endregion
    }
}
