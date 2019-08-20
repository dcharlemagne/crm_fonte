using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_mensagem")]
    public class Mensagem : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Mensagem() { }

        public Mensagem(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Mensagem(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_mensagemid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigo_mensagem")]
        public int? Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_Texto_mensagem")]
        public String Texto { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }
        #endregion
    }
}
