using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_turmadocanal")]
    public class TurmaCanal : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public TurmaCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public TurmaCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_turmadocanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_canalid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_datadeinicio")]
        public DateTime? DataInicio { get; set; }

        [LogicalAttribute("itbc_datadetermino")]
        public DateTime? DataTermino { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_idturma")]
        public String IdTurma { get; set; }



        #endregion
    }
}
