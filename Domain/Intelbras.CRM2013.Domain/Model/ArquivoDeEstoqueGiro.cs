using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_arquivoestoquegiro")]
    public class ArquivoDeEstoqueGiro : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ArquivoDeEstoqueGiro(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ArquivoDeEstoqueGiro(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_arquivoestoquegiroid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_canalid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_datadeenvio")]
        public DateTime? DataDeEnvio { get; set; }

        [LogicalAttribute("itbc_datainicioprocessamento")]
        public DateTime? DataInicioProcessamento { get; set; }

        [LogicalAttribute("itbc_datadeprocessamento")]
        public DateTime? DataDeProcessamento { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_quantidadedelinhascomerro")]
        public int? QuantidadeLinhasErro { get; set; }

        [LogicalAttribute("itbc_quantidadedelinhasprocessadas")]
        public int? QuantidadeLinhasProcessadas { get; set; }

        [LogicalAttribute("itbc_quantidadetotaldelinhas")]
        public int? QuantidadeTotalLinhas { get; set; }

        [LogicalAttribute("itbc_descricao_erros")]
        public String DescricaoErros { get; set; }

        [LogicalAttribute("itbc_origem")]
        public int? Origem { get; set; }
        #endregion
    }
}
