using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("connection")]
    public class Conexao : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Conexao() { }

        public Conexao(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Conexao(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("connectionid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("name")]
            public String Nome { get; set; }

            [LogicalAttribute("record2roleid")]
            public Lookup Funcao_Ate { get; set; }

            [LogicalAttribute("record2id")]
            public Lookup Conectado_a { get; set; }

            [LogicalAttribute("record1roleid")]
            public Lookup Funcao_De { get; set; }

            [LogicalAttribute("record1id")]
            public Lookup Conectado_de { get; set; }

            [LogicalAttribute("description")]
            public String Descricao { get; set; }

            [LogicalAttribute("effectivestart")]
            public DateTime? Inicio { get; set; }

            [LogicalAttribute("effectiveend")]
            public DateTime? Termino { get; set; }
        #endregion
    }
}