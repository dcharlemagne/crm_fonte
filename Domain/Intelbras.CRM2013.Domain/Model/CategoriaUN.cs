using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{

    [LogicalEntity("new_relacionamento")]
    public class CategoriaUN : DomainBase
    {

        private UnidadeNegocio unidadeDeNegocio;
        private Model.Conta cliente;
        private Categoria categoria;
        private Contato representante;
        private DateTime dataFinal;
        private DateTime dataInicio;

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public DateTime DataInicio
        {
            get { return dataInicio; }
            set { dataInicio = value; }
        }
        public DateTime DataFinal
        {
            get { return dataFinal; }
            set { dataFinal = value; }
        }
        public Contato Representante
        {
            get { return representante; }
            set { representante = value; }
        }
        public UnidadeNegocio UnidadeDeNegocio
        {
            get { return unidadeDeNegocio; }
            set { unidadeDeNegocio = value; }
        }
        public Model.Conta Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
        public Categoria Categoria
        {
            get { return categoria; }
            set { categoria = value; }
        }

        private RepositoryService RepositoryService { get; set; }

        public CategoriaUN() { }

        public CategoriaUN(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CategoriaUN(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
    }
}
