using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_transportadora")]
    public class Transportadora : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Transportadora() { }

        public Transportadora(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Transportadora(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_transportadoraid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_codigodatransportadora")]
        public Int32? Codigo { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_nomeabreviado")]
        public String NomeAbreviado { get; set; }

        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        [LogicalAttribute("itbc_codigoviatransportadora")]
        public int? CodigoViaTransportadora { get; set; }

        #endregion
    }
}