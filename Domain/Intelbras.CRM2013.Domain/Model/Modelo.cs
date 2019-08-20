using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_modelo_equipamento")]
    public class Modelo : DomainBase
    {
        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public Modelo() { }

        public Modelo(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Modelo(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_modelo_equipamentoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_marcaequipamentoid")]
        public Lookup MarcaEquipamentoId { get; set; }

        private Marca _Marca = null;
        public Marca Marca
        {
            get
            {
                if (MarcaEquipamentoId != null && _Marca == null && this.Id != Guid.Empty)
                    _Marca = (new CRM2013.Domain.Servicos.RepositoryService()).Marca.Retrieve(this.MarcaEquipamentoId.Id);
                return _Marca;
            }
            set { _Marca = value; }
        }

        #endregion

        #region Metodos

        #endregion
    }
}