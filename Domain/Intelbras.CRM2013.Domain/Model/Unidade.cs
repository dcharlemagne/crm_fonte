using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("uom")]
    public class Unidade : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Unidade() { }

        public Unidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public Unidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("uomid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("baseuom")]
        public Lookup UnidadeBase { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        [LogicalAttribute("quantity")]
        public Decimal? Quantidade { get; set; }

        [LogicalAttribute("uomscheduleid")]
        public Lookup GrupoUnidade { get; set; }
        
        #endregion
    }
}
