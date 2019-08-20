using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_tipo_posto")]
    public class TipoPosto : DomainBase
    {
        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public TipoPosto() { }

        public TipoPosto(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TipoPosto(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        private decimal servico;
        [LogicalAttribute("")]
        public decimal Servico
        {
            get { return servico; }
            set { servico = value; }
        }

        #endregion

        #region Metodos

        public int TipoAcessoRelatorio(string login)
        {
            List<TipoPosto> lista = RepositoryService.TipoPosto.ListarPor(login);

            if (lista.Count == 0) 
                return -1;
            
            foreach(TipoPosto item in lista)
            if (item.Nome.Equals("LAI")) 
                return 2;

            return 1;
        }

        #endregion
    }
}