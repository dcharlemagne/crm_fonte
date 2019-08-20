using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_registro_log")]
    public class Log : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Log() { }

        public Log(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Log(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos        
        [LogicalAttribute("New_name")]
        public string Nome { get; set; }

        [LogicalAttribute("New_Mensagem")]
        public string Mensagem { get; set; }

        [LogicalAttribute("New_alteracoes")]
        public string Alteracoes { get; set; }

        [LogicalAttribute("New_DiagnosticoId")]
        public Lookup ID { get; set; }

        [LogicalAttribute("New_acao_evento")]
        public string Acao { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Sucesso { get; set; }
        #endregion
    }
}
