using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_estabelecimento")]
    public class Estabelecimento:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Estabelecimento() { }

        public Estabelecimento(string organization, bool isOffline)
            : base(organization, isOffline)
        {        
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Estabelecimento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_estabelecimentoid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_cep")]
            public String CEP { get; set; }

            [LogicalAttribute("itbc_cidade")]
            public String Cidade { get; set; }

            [LogicalAttribute("itbc_cnpj")]
            public String CNPJ { get; set; }

            [LogicalAttribute("itbc_codigo_estabelecimento")]
            public int? Codigo { get; set; }

            [LogicalAttribute("itbc_endereco")]
            public String Endereco { get; set; }

            [LogicalAttribute("itbc_inscricaoestadual")]
            public String InscricaoEstadual { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_razaosocial")]
            public String RazaoSocial { get; set; }

            [LogicalAttribute("itbc_uf")]
            public String UF { get; set; }

            [LogicalAttribute("statecode")]
            public int? State { get; set; }        

        #endregion
    }
}
