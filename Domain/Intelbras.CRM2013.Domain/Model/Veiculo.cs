using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("itbc_veiculo")]
    public class Veiculo : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public Veiculo() { }

        public Veiculo(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public Veiculo(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_veiculoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_placa")]
        public string Placa { get; set; }

        [LogicalAttribute("itbc_descricao")]
        public string Descricao { get; set; }

        [LogicalAttribute("itbc_data_de_instalacao")]
        public DateTime? DataDeInstalacao { get; set; }

        [LogicalAttribute("itbc_cliente_participante_do_contrato")]
        public Lookup ClienteParticipanteDoContrato { get; set; }

        #endregion

        public void Clonar(ClienteParticipanteDoContrato clienteParticipanteDestino, Contrato contratoDestino)
        {
            Veiculo veiculo = Helper.CopyObject<Veiculo>(this);
            veiculo.Id = Guid.Empty;
            veiculo.ID = Guid.Empty;
            veiculo.ClienteParticipanteDoContrato = new Lookup(clienteParticipanteDestino.Id, "new_cliente_participante_contrato");

            RepositoryService.Veiculo.Create(veiculo);
        }
    }
}
