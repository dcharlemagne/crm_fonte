using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_acessosextranetcontatos")]
    public class AcessoExtranetContato : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcessoExtranetContato() { }

        public AcessoExtranetContato(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public AcessoExtranetContato(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("itbc_acessosextranetcontatosid")]
        public Guid? ID { get; set; }

        
        // String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }
        
        // Lookup
        [LogicalAttribute("itbc_tipodeacesso")]
        public Lookup TipoAcesso { get; set; }

        [LogicalAttribute("itbc_contactid")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("itbc_acessoextranetid")]
        public Lookup AcessoExtranetid { get; set; }

        [LogicalAttribute("itbc_canal")]
        public Lookup Canal { get; set; }
      
        // Datetime
        [LogicalAttribute("itbc_validade")]
        public DateTime? Validade { get; set; }

        [LogicalAttribute("itbc_dataaceitetermodeuso")]
        public DateTime? DataAceiteTermo { get; set; }
        
        // Boolean
        [LogicalAttribute("itbc_acaocrm")]
        public bool? IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_aceitoutermodeuso")]
        public bool? UsuarioAceitouTermoUso { get; set; }

        #endregion
    }
}
