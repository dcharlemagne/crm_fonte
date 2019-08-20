using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_estado")]
    public class Estado:IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Estado() { }

        public Estado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Estado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        
        #endregion

        #region Atributos
            [LogicalAttribute("itbc_estadoid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_chave_integracao")]
            public String ChaveIntegracao { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_pais")]
            public Lookup Pais { get; set; }

            private Pais pais;
            public Pais PaisModel
            {
                get
                {
                    if (pais == null && this.Id != Guid.Empty)
                        pais = (new Domain.Servicos.RepositoryService()).Pais.PesquisarPaisPor(this.Id);

                    return pais;
                }
                set { pais = value; }
            }

            [LogicalAttribute("itbc_regiao_geografica")]
            public Lookup RegiaoGeografica { get; set; }

            [LogicalAttribute("itbc_siglauf")]
            public String SiglaUF { get; set; }

            [LogicalAttribute("itbc_uf")]
            public String UF { get; set; }

            [LogicalAttribute("statecode")]
            public new int? Status { get; set; }
        #endregion

        #region Metodos

        public Estado PesquisarPor(string sigla)
        {
            return RepositoryService.Estado.ObterPor(sigla);
        }

        public Estado PesquisarPor(string sigla, Pais pais)
        {
            return RepositoryService.Estado.PesquisarUfPor(sigla, pais);
        }

        public List<Estado> Listar(Guid paisId)
        {
            return RepositoryService.Estado.ListarUf(paisId);
        }

        #endregion

    }
}
