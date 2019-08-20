using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_unidadedokonviva")]
    public class UnidadeKonviva : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UnidadeKonviva(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public UnidadeKonviva(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_unidadedokonvivaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_IdInterno")]
        public Int32? IdInterno { get; set; }

        [LogicalAttribute("itbc_Codigo")]
        public String Codigo { get; set; }

        [LogicalAttribute("statecode")]
        public Int32? State { get; set; }
        
        #endregion
    }
}
