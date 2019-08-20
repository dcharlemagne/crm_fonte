using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_politicacomercial")]
    public class PoliticaComercial : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PoliticaComercial() { }

        public PoliticaComercial(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public PoliticaComercial(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_politicacomercialid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome {get; set;}

        //Depois que for arrumado esse atributo no produtoService linha 639,retirar da model(o atributo virou n:m)
        //[LogicalAttribute("itbc_accountid")]
        //public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }

        [LogicalAttribute("itbc_estabelecimentoid")]
        public Lookup Estabelecimento { get; set; }

        [LogicalAttribute("itbc_businessunitid")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_fator")]
        public double? Fator { get; set; }
                
        [LogicalAttribute("itbc_tipodepolitica")]
        public int? TipoDePolitica { get; set; }

        [LogicalAttribute("itbc_polticaespecificacanal")]
        public int? PoliticaEspecifica { get; set; }

        [LogicalAttribute("itbc_aplicarpoliticapara")]
        public int? AplicarPoliticaPara { get; set; }

        [LogicalAttribute("itbc_data_inicio")]
        public DateTime? DataInicio { get; set; }

        [LogicalAttribute("itbc_data_fim")]
        public DateTime? DataFim { get; set; }
        #endregion
    }
}