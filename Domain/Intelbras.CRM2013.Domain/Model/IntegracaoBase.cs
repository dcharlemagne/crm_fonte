using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKore.Helper.Cache;
using SDKore.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    public class IntegracaoBase : DomainBase
    {

        private RepositoryService RepositoryService { get; set; }
        
        public IntegracaoBase()
        {
        }

        public IntegracaoBase(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public IntegracaoBase(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        
        
        [LogicalAttribute("itbc_ult_atualizacao_integracao")]
        public DateTime? IntegradoEm { get; set; }

        [LogicalAttribute("itbc_usuariointegracao")]
        public string UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_integradopor")]
        public string IntegradoPor { get; set; }

        public string DescricaoDaMensagemDeIntegracao { get; set; }

    }
}
