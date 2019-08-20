using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_docsestoquegiro")]
    public class DocumentoCanalEstoqueGiro : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanalEstoqueGiro(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DocumentoCanalEstoqueGiro(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_docsestoquegiroid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_canalid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_datacriacao")]
        public DateTime? DataCriacao { get; set; }

        [LogicalAttribute("itbc_url")]
        public String Url { get; set; }

        [LogicalAttribute("itbc_somentearquivosnovos")]
        public bool? SomenteArquivosNovos { get; set; }

        [LogicalAttribute("itbc_datainicial")]
        public DateTime? DataInicial { get; set; }

        [LogicalAttribute("itbc_datafinal")]
        public DateTime? DataFinal { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_tipodocumentoid")]
        public Lookup TipoDocumento { get; set; }

        [LogicalAttribute("itbc_tamanho")]
        public String Tamanho { get; set; }

        #endregion
    }
}
