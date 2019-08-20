using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_acessoextranet")]
    public class AcessoExtranet : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcessoExtranet() { }

        public AcessoExtranet(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public AcessoExtranet(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }



        #endregion

        #region Atributos

        [LogicalAttribute("itbc_acessoextranetid")]
        public Guid? ID { get; set; }
        


        //String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }
        
        // Lookup
        [LogicalAttribute("itbc_tipodeacesso")]
        public Lookup TipoAcesso { get; set; }

        [LogicalAttribute("itbc_classificacaoid")]
        public Lookup Classificacao { get; set; }

        [LogicalAttribute("itbc_categoriaid")]
        public Lookup Categoria { get; set; }
               
        #endregion
    }
}