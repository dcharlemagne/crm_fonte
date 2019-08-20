using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_localidade")]
    public class Localidade : DomainBase
    {
        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public Localidade() { }

        public Localidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public Localidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        public Localidade(string logradouro,
                         string bairro,
                         Municipio cidade,
                         Estado uf,
                         string cep,
                         string organization, bool isOffline
                       )
            : base(organization, isOffline)
        {
            this.logradouro = logradouro;
            this.bairro = bairro;
            this.cidade = cidade;
            this.uf = uf;
            this.cep = cep;
        }

        #endregion

        #region Atributos

        private string bairro, cep, logradouro;
        private Estado uf = null;
        private Municipio cidade = null;

        public string Complemento { get; set; }
        public string Numero { get; set; }
        public Municipio Cidade { get; set; }
        public string Logradouro { get; set; }   
        public Pais Pais { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public Estado Uf { get; set; }
        
        #endregion

    }
}
