using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_canais_verde")]
    public class CanalVerde : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CanalVerde() { }

        public CanalVerde(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CanalVerde(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_canais_verdeid")]
        public Guid? ID { get; set; }


        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_canalid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_familia_produto_id")]
        public Lookup FamiliaProduto { get; set; }

        [LogicalAttribute("itbc_segmento")]
        public Lookup Segmento { get; set; }

        #endregion
    }
}
