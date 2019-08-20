using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("post")]
    public class Postagem : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public Postagem() { }

        public Postagem(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Postagem(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public Postagem(RepositoryService repositoryService) : base(repositoryService.NomeDaOrganizacao, repositoryService.IsOffline)
        {
            this.RepositoryService = repositoryService;
        }

        #endregion


        #region Atributos
        [LogicalAttribute("postid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("text")]
        public String Texto { get; set; }

        [LogicalAttribute("regardingobjectid")]
        public Lookup ReferenteA { get; set; }

        //[LogicalAttribute("createdon")]
        //public DateTime? CriadoEm { get; set; }

        [LogicalAttribute("createdby")]
        public Lookup UsuarioAtividade { get; set; }

        [LogicalAttribute("source")]
        public Int32? Source { get; set; } 

        #endregion
    }
}
