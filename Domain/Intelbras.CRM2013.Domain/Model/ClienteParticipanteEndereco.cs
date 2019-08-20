using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_cliente_participante_endereco")]
    public class ClienteParticipanteEndereco : DomainBase
    {
        #region Atributos
        private Model.Conta cliente;
        private LocalidadeContrato localidade = null;

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public Model.Conta Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
        [LogicalAttribute("new_data_inicial")]
        public DateTime? DataInicialVigencia { get; set; }
        [LogicalAttribute("new_data_final")]
        public DateTime? DataFinalVigencia { get; set; }
        [LogicalAttribute("new_uf")]
        public String Uf { get; set; }
        [LogicalAttribute("new_rua")]
        public String Rua { get; set; }

        public LocalidadeContrato Localidade
        {
            get { return localidade; }
            set { localidade = value; }
        }
        [LogicalAttribute("new_localidadeid")]
        public Lookup LocalidadeId { get; set; }
        [LogicalAttribute("new_bairro")]
        public String Bairro { get; set; }
        [LogicalAttribute("new_cidade")]
        public String Cidade { get; set; }
        [LogicalAttribute("new_cep")]
        public String Cep { get; set; }
        [LogicalAttribute("new_codigoendereco")]
        public String CodigoEndereco { get; set; }
        [LogicalAttribute("new_enderecoid")]
        public String Endereco { get; set; }
        [LogicalAttribute("new_cliente_participanteid")]
        public Lookup ClienteParticipanteId { get; set; }
        [LogicalAttribute("new_contratoid")]
        public Lookup ContratoId { get; set; }
        [LogicalAttribute("new_clienteid")]
        public Lookup ClienteId { get; set; }
        [LogicalAttribute("New_produtos_endereco")]
        public String ProdutoEndereco { get; set; }
        [LogicalAttribute("itbc_rodovia_pavimentada")]
        public bool? Pavimentada { get; set; }
        [LogicalAttribute("itbc_distancia_capital")]
        public decimal? DistanciaCapital { get; set; }
        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ClienteParticipanteEndereco() { }

        public ClienteParticipanteEndereco(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ClienteParticipanteEndereco(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        public void Clonar(ClienteParticipanteDoContrato clienteParticipanteDestino, Contrato contratoDestino)
        {
            ClienteParticipanteEndereco clienteParticipanteEndereco = Helper.CopyObject<ClienteParticipanteEndereco>(this);
            clienteParticipanteEndereco.Id = Guid.Empty;
            clienteParticipanteEndereco.ClienteParticipanteId = new Lookup(clienteParticipanteDestino.Id, "new_cliente_participante_contrato");
            clienteParticipanteEndereco.ContratoId = new Lookup(contratoDestino.Id, "contract");

            RepositoryService.ClienteParticipanteDoEndereco.Create(clienteParticipanteEndereco);
        }
    }
}