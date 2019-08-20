using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_relacionamentodob2b")]
    public class RelacionamentoB2B : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RelacionamentoB2B(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public RelacionamentoB2B(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_relacionamentodob2bid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigocategoriab2b")]
        public Lookup CategoriaB2B { get; set; }

        [LogicalAttribute("itbc_codigocliente")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_codigogrupocliente")]
        public int? CodigoGrupoCliente { get; set; }

        [LogicalAttribute("itbc_codigorelacionamentob2b")]
        public String CodigoRelacionamentoB2B { get; set; }

        [LogicalAttribute("itbc_codigo_representante")]
        public int? CodigoRepresentante { get; set; }

        [LogicalAttribute("itbc_datafinal")]
        public DateTime? DataFinal { get; set; }

        [LogicalAttribute("itbc_datainicial")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_name")]
        public String NomeRelacionamento { get; set; }

        [LogicalAttribute("itbc_nomeunidadecomercial")]
        public String NomeUnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_codigounidadecomercial")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_mensagem")]
        public String Mensagem { get; set; }

        [LogicalAttribute("itbc_sequencia")]
        public int? Sequencia { get; set; }

        #endregion
    }
}
