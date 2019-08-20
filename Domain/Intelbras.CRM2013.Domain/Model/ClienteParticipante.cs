using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_cliente_participante_contrato")] //DEVE MANTER UM LogicalEntity para acesso ao Domain mesmo que não seja a entidade correta
    public class ClienteParticipante : DomainBase
    {
        #region Atributos
        public Guid? ID { get; set; }
        private DateTime? inicioVigencia = null;
        private DateTime? fimVigencia = null;
        private Guid? enderecoId = null;
        private Guid? codigoDaLocalidade = null;
        private string localidade = null;
        private string cidade;
        private string cep;
        private string descricao = null;
        private string uf;
        private string bairro;
        private string logradouro;
        private string pais;
        private Guid? clienteParticipanteId = null;
        private Guid? clienteId = null;
        private Guid? contratoId = null;
        private string codigoCliente;

        //[LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public string CodigoCliente
        {
            get { return codigoCliente; }
            set { codigoCliente = value; }
        }

        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }


        public Guid? ContratoId
        {
            get { return contratoId; }
            set { contratoId = value; }
        }
        public Guid? ClienteId
        {
            get { return clienteId; }
            set { clienteId = value; }
        }
        public Guid? ClienteParticipanteId
        {
            get { return clienteParticipanteId; }
            set { clienteParticipanteId = value; }
        }
        public string Pais
        {
            get { return pais; }
            set { pais = value; }
        }
        public string Cidade
        {
            get { return cidade; }
            set { cidade = value; }
        }
        public string Cep
        {
            get { return cep; }
            set { cep = value; }
        }
        public string Uf
        {
            get { return uf; }
            set { uf = value; }
        }
        public string Bairro
        {
            get { return bairro; }
            set { bairro = value; }
        }
        public string Logradouro
        {
            get { return logradouro; }
            set { logradouro = value; }
        }
        public Guid? CodigoDaLocalidade
        {
            get { return codigoDaLocalidade; }
            set { codigoDaLocalidade = value; }
        }
        public string Localidade
        {
            get { return localidade; }
            set { localidade = value; }
        }
        public DateTime? InicioVigencia
        {
            get { return inicioVigencia; }
            set
            {
                if (value != DateTime.MinValue)
                    inicioVigencia = value;
            }
        }
        public DateTime? FimVigencia
        {
            get { return fimVigencia; }
            set
            {
                if (value != DateTime.MaxValue)
                    fimVigencia = value;
            }
        }
        public Guid? EnderecoId
        {
            get { return enderecoId; }
            set { enderecoId = value; }
        }
        #endregion

        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ClienteParticipante() { }

        public ClienteParticipante(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ClienteParticipante(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Metodos
        public bool EstaVigente(ClienteParticipante clienteParticipante)
        {
            DateTime agora = DateTime.Now;

            if (clienteParticipante == null)
                return false;

            if (clienteParticipante.InicioVigencia.HasValue)
            {
                if (clienteParticipante.inicioVigencia.Value >= agora)
                    return false;
            }

            if (clienteParticipante.FimVigencia.HasValue)
            {
                if (clienteParticipante.FimVigencia <= agora)
                    return false;
            }

            return true;
        }

        public void SalvarDataDeVigencia(ClienteParticipante clienteParticipante, TipoDeVigencia vigencia)
        {
            if (vigencia == TipoDeVigencia.Cliente)

               RepositoryService.ClienteParticipanteDoContrato.Update(RepositoryService.ClienteParticipante.InstanciarClienteParticipanteDoContrato(clienteParticipante));
            else if (vigencia == TipoDeVigencia.Endereco)
                RepositoryService.ClienteParticipanteDoEndereco.Update(RepositoryService.ClienteParticipante.InstanciarClienteParticipanteEndereco(clienteParticipante));
        }

        public void AtualizarEnderecoParticipante(ClienteParticipante item)
        {
            RepositoryService.ClienteParticipanteDoEndereco.Update(RepositoryService.ClienteParticipante.InstanciarClienteParticipanteEndereco(item));
        }

        public List<ClienteParticipante> ObterTodosOsEnderecosClienteParticipantePor(Model.Conta cliente)
        {
            return RepositoryService.ClienteParticipante.ListarPor(cliente);
        }
        public List<ClienteParticipante> ObterTodosOsEnderecosClienteParticipantePor(Guid clienteId, Guid enderecoId)
        {
            return RepositoryService.ClienteParticipante.ListarPor(clienteId, enderecoId);
        }

        public ClienteParticipante RetornaGuidClienteParticipantePorContrato(Guid clienteId, Guid contratoId)
        {
            return RepositoryService.ClienteParticipante.ObterPor(clienteId, contratoId);
        }

        public void Delete(Guid clienteParticipanteId)
        {
            RepositoryService.ClienteParticipante.Delete(clienteParticipanteId);
        }

        public void DeleteAll(Contrato contrato)
        {
            List<ClienteParticipante> ListaClienteParticipante = RepositoryService.ClienteParticipante.ListarPor(contrato);
            foreach (var item in ListaClienteParticipante)
                this.Delete(item.ID.Value);
        }

        #endregion
    }
}
