using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_cliente_participante_contrato")]
    public class ClienteParticipanteDoContrato : DomainBase
    {
        #region Atributos

        [LogicalAttribute("new_cliente_participante_contratoid")]
        public Guid Id { get; set; }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        [LogicalAttribute("new_descricao")]
        public string Descricao { get; set; }
        [LogicalAttribute("new_nflocacaoid")]
        private Lookup NotaFiscalLocacao { get; set; }
        [LogicalAttribute("new_nfpatrimonioid")]
        public Lookup NotaFiscalPatrimonio { get; set; }
        [LogicalAttribute("new_nfremessaid")]
        public Lookup NotaFiscalRemessa { get; set;}
        [LogicalAttribute("new_notafiscalid")]
        public Lookup NotaFiscalFatura { get; set; }
        [LogicalAttribute("new_data_inicial")]
        public DateTime? InicioVigencia { get; set; }
        [LogicalAttribute("new_data_final")]
        public DateTime? FimVigencia { get; set; }
        [LogicalAttribute("new_codigo_cliente")]
        public string CodigoCliente { get; set; }
        [LogicalAttribute("new_contratoid")]
        public Lookup Contrato { get; set; }
        [LogicalAttribute("new_clienteid")]
        public Lookup Cliente { get; set; }

        #endregion

        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ClienteParticipanteDoContrato() { }

        public ClienteParticipanteDoContrato(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ClienteParticipanteDoContrato(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos

        public void Clonar(Contrato contratoDestino)
        {
            ClienteParticipanteDoContrato novoClienteParticipanteDoContrato = Helper.CopyObject<ClienteParticipanteDoContrato>(this);

            novoClienteParticipanteDoContrato.Contrato = new Lookup(contratoDestino.Id,"contract");
            novoClienteParticipanteDoContrato.Id = Guid.Empty;
            novoClienteParticipanteDoContrato.Id = RepositoryService.ClienteParticipanteDoContrato.Create(novoClienteParticipanteDoContrato);

            var listaParticipanteEndereco = RepositoryService.ClienteParticipanteDoEndereco.ListarPor(this);
            foreach (var item in listaParticipanteEndereco)
                item.Clonar(novoClienteParticipanteDoContrato, contratoDestino);

            var listaParticipanteVeiculos = RepositoryService.Veiculo.ListarPorClienteParticipanteContrato(this.Id);
            foreach (var item in listaParticipanteVeiculos)
                item.Clonar(novoClienteParticipanteDoContrato, contratoDestino);
        }
        

        #endregion
    }
}