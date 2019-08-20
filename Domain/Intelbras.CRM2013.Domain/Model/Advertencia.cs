using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using System;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_comunicado_advertencia")]
    public class Advertencia : DomainBase
    {
        private string gravidade = "", resumo = "", descricao = "", justificativa = "", status = "";

        public string StatusDaAdvertencia
        {
            get { return status; }
            set { status = value; }
        }

        private Conta cliente = null;
        public Conta Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        private DateTime dataLeitura = DateTime.MinValue;
        public DateTime DataLeitura
        {
            get { return dataLeitura; }
            set { dataLeitura = value; }
        }

        private DateTime dataJustificativa = DateTime.MinValue;
        public DateTime DataJustificativa
        {
            get { return dataJustificativa; }
            set { dataJustificativa = value; }
        }

        private DateTime dataCriacao = DateTime.MinValue;
        public DateTime DataCriacao
        {
            get { return dataCriacao; }
            set { dataCriacao = value; }
        }

        public string Justificativa
        {
            get { return justificativa; }
            set { justificativa = value; }
        }

        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        public string Resumo
        {
            get { return resumo; }
            set { resumo = value; }
        }

        public string Gravidade
        {
            get { return gravidade; }
            set { gravidade = value; }
        }

        private RepositoryService RepositoryService { get; set; }
        public Advertencia() { }
        public Advertencia(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        public Advertencia(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
    }
}
