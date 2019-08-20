using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_empresas_coligadas")]
    public class EmpresasColigadas:DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EmpresasColigadas() { }

        public EmpresasColigadas(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public EmpresasColigadas(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_empresas_coligadasid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_conta")]
            public Lookup Conta { get; set; }

            [LogicalAttribute("itbc_cpfoucnpj")]
            public String CpfCnpj { get; set; }

            [LogicalAttribute("itbc_nacionalidade")]
            public String Nacionalidade { get; set; }

            [LogicalAttribute("itbc_name")]
            public String RazaoSocial { get; set; }

            [LogicalAttribute("itbc_porcentagem_capital")]
            public Decimal? PorcentagemCapital { get; set; }

            [LogicalAttribute("statecode")]
            public int? Status { get; set; }


        #endregion  
    }
}
