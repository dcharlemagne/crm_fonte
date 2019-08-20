using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_municipios")]
    public class Municipio : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Municipio() { }

        public Municipio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public Municipio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_municipiosid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_chavedeintegracao")]
        public String ChaveIntegracao { get; set; }

        [LogicalAttribute("itbc_estadoid")]
        public Lookup Estadoid { get; set; }

        private Estado estado;
        public Estado Estado
        {
            get
            {
                if (this.Id != Guid.Empty && this.Estadoid != null)
                    estado = (new Domain.Servicos.RepositoryService()).Estado.Retrieve(Estadoid.Id);
                return estado;
            }
            set { estado = value; }
        }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_capitalouinterior")]
        public int? CapitalOuInterior { get; set; }

        [LogicalAttribute("itbc_codigo_ibge")]
        public int? CodigoIbge { get; set; }

        public Boolean ZonaFranca { get; set; }

        [LogicalAttribute("itbc_regionalid")]
        public Lookup Regionalid { get; set; }

        private Regional regional;
        public Regional Regional
        {
            get
            {
                if (this.Id != Guid.Empty && this.Regionalid != null) regional = (new Domain.Servicos.RepositoryService()).Regional.Retrieve(Regionalid.Id);
                return regional;
            }
            set { regional = value; }
        }




        #endregion
    }
}