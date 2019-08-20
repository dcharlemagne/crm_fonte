using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("contractdetail")]
    public class LinhaDeContrato : DomainBase
    {
        #region Atributos

        [LogicalAttribute("new_tipo_servico")]
        public int? TipoDeOcorrencia { get; set; }

        [LogicalAttribute("activeon")]
        public DateTime InicioContrato { get; set; }

        [LogicalAttribute("expireson")]
        public DateTime FimContrato { get; set; }

        [LogicalAttribute("totalallotments")]
        public int TotalOcorrencias { get; set; }

        [LogicalAttribute("title")]
        public String Nome { get; set; }

        [LogicalAttribute("new_valor_pago")]
        public decimal? PrecoPago { get; set; }

        [LogicalAttribute("price")]
        public decimal PrecoTotal { get; set; }

        [LogicalAttribute("contractid")]
        public Lookup ContratoId { get; set; }

        [LogicalAttribute("itbc_limite_ocorrencias")]
        public Int32? LimiteOcorrencias { get; set; }

        #endregion

        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public LinhaDeContrato() { }

        public LinhaDeContrato(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LinhaDeContrato(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion


    }
}
