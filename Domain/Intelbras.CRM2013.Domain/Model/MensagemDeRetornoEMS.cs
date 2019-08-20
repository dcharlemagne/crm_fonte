using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    public class MensagemDeRetornoEMS : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public MensagemDeRetornoEMS() { }

        public MensagemDeRetornoEMS(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public MensagemDeRetornoEMS(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        private string descricao = "";
        private string CodigoMatriz;
        private string codigoEndereco = "";

        public string CodigoEndereco
        {
            get { return codigoEndereco; }
            set { codigoEndereco = value; }
        }
        
        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

    }
}