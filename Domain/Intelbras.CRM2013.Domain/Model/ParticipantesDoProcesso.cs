using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_participantedoprocesso")]
    public class ParticipantesDoProcesso : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ParticipantesDoProcesso() { }

        public ParticipantesDoProcesso(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ParticipantesDoProcesso(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_participantedoprocessoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_processoid")]
        public Lookup Processo { get; set; }

        [LogicalAttribute("itbc_userid")]
        public Lookup Usuario { get; set; }

        [LogicalAttribute("itbc_equipeid")]
        public Lookup Equipe { get; set; }

        [LogicalAttribute("itbc_contactid")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("itbc_ordem")]
        public int Ordem { get; set; }

        [LogicalAttribute("itbc_papelnocanal")]
        public int? PapelNoCanal { get; set; }

        [LogicalAttribute("itbc_papelid")]
        public Lookup Papel { get; set; }

        [LogicalAttribute("itbc_departamento")]
        public int? Departamento { get; set; }

        [LogicalAttribute("itbc_tipodoparecer")]
        public int? TipoDoParecer { get; set; }

        #endregion
    }
}
