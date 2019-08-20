using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_parametrosbeneficios")]
    public class ParametroBeneficio : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ParametroBeneficio() { }

        public ParametroBeneficio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public ParametroBeneficio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_parametrosbeneficiosid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_beneficioid")]
        public Lookup Beneficio { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_codigoestabelecimento")]
        public String CodigoEstabelecimento { get; set; }

        [LogicalAttribute("itbc_estabelecimentoid")]
        public Lookup Estabelecimento { get; set; }


        [LogicalAttribute("itbc_tipofluxofinanceiro")]
        public String TipoFluxoFinanceiro { get; set; }

        [LogicalAttribute("itbc_especiedocumento")]
        public String EspecieDocumento { get; set; }

        [LogicalAttribute("itbc_contacontabil")]
        public String ContaContabil { get; set; }

        [LogicalAttribute("itbc_centrocusto")]
        public String CentroCusto { get; set; }

        [LogicalAttribute("itbc_atingimentometaprevisto")]
        public Decimal? AtingimentoMetaPrevisto { get; set; }

        [LogicalAttribute("itbc_custo")]
        public Decimal? Custo { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocio { get; set; }

        private UnidadeNegocio _UnidadeNegocio = null;
        public UnidadeNegocio UnidadeNegocioObj
        {
            get
            {
                if (_UnidadeNegocio == null && UnidadeNegocio.Id != null && UnidadeNegocio.Id != Guid.Empty)
                    _UnidadeNegocio = RepositoryService.UnidadeNegocio.Retrieve(UnidadeNegocio.Id);
                return _UnidadeNegocio;
            }
        }

        #endregion
    }
}
