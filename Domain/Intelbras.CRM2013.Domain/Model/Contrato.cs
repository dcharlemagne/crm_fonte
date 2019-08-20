using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("contract")]
    public class Contrato : DomainBase
    {
        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public Contrato() { }

        public Contrato(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Contrato(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("new_nfremessaid")]
        public Lookup NotaFiscalRemessaId { get; set; }
        private Fatura notaFiscalRemessa;
        public Fatura NotaFiscalRemessa
        {
            get
            {
                if (this.notaFiscalRemessa == null && NotaFiscalRemessaId != null && this.Id != Guid.Empty)
                    this.notaFiscalRemessa = new RepositoryService().Fatura.Retrieve(NotaFiscalRemessaId.Id);
                 return this.notaFiscalRemessa;
            }

            set { notaFiscalRemessa = value; }
        }
        [LogicalAttribute("new_nota_fiscalid")]
        public Lookup NotaFiscalFaturaId { get; set; }
        private Fatura notaFiscalFatura;
        public Fatura NotaFiscalFatura
        {
            get
            {
                if (this.notaFiscalFatura == null && NotaFiscalFaturaId != null && this.Id != Guid.Empty)
                  this.notaFiscalFatura = new RepositoryService().Fatura.Retrieve(NotaFiscalFaturaId.Id);
                return this.notaFiscalFatura;
            }
            set { this.notaFiscalFatura = value; }
        }
        [LogicalAttribute("contractlanguage")]
        public string ObservacaoOs { get; set; }

        private bool ativo;
        public bool Ativo
        {
            get { return ativo; }
            set { ativo = value; }
        }
        [LogicalAttribute("contractservicelevelcode")]
        public int? TipoVigenciaValue { get; set; }
        private TipoDeVigencia tipovigencia = TipoDeVigencia.Vazio;
        public TipoDeVigencia TipoVigencia
        {
            get
            {
                if (TipoVigenciaValue.HasValue)
                    tipovigencia = (TipoDeVigencia)TipoVigenciaValue.Value;
                return tipovigencia;
            }
            set { tipovigencia = value; }
        }
        [LogicalAttribute("new_tipo_ocorrencia")]
        public int? TipodeOcorrencia { get; set; }
        [LogicalAttribute("activeon")]
        public DateTime? InicioVigencia { get; set; }
        [LogicalAttribute("expireson")]
        public DateTime? FimVigencia { get; set; }
        [LogicalAttribute("new_data_termino_real")]
        public DateTime? FimRealVigencia { get; set; }
        [LogicalAttribute("duration")]
        public int? DuracaoDias { get; set; }
        [LogicalAttribute("title")]
        public string NomeContrato { get; set; }
        [LogicalAttribute("contractnumber")]
        public string NumeroContrato { get; set; }

        private CalendarioDeTrabalho calendario = null;
        public CalendarioDeTrabalho Calendario
        {
            get
            {
                if (null == this.calendario)
                {
                    if (this.Id != Guid.Empty)
                        this.calendario = new RepositoryService().Contrato.ObterCalendario(this.Id);
                }

                return calendario;

            }
        }
        [LogicalAttribute("itbc_acrescer_hora_kms")]
        public int? AcrescerHoraKm { get; set; }
        [LogicalAttribute("itbc_adicional_minutos_nao_pavimentada")]
        public int? AdicionalNaoPavimentada { get; set; }
        [LogicalAttribute("itbc_adicional_minutos_pavimentada")]
        public int? AdicionalPavimentada { get; set; }
        
        #endregion

        #region Metodos

        public List<ClienteParticipante> ObterEnderecoClientesParticipantesPor(Contrato contrato, Model.Conta cliente)
        {
            return new RepositoryService().Contrato.ObterEnderecoClientesParticipantesPor(contrato, cliente);
        }

        public List<Endereco> ObterParticipantesPor(Contrato contrato, Model.Conta cliente)
        {

            var enderecos = new List<Endereco>();
            Endereco end = null;
            var participantes = this.ObterEnderecoClientesParticipantesPor(contrato, cliente);
            foreach (var item in participantes)
            {
                if (item.EnderecoId.HasValue)
                {
                    end = new RepositoryService().Endereco.Retrieve(item.EnderecoId.Value);
                    if (end != null)
                    {
                        if (item.CodigoDaLocalidade.HasValue)
                        {
                            end.CodigoDaLocalidade = item.CodigoDaLocalidade.Value;
                            end.Localidade = item.Localidade;
                        }
                        enderecos.Add(end);
                    }
                }
            }

            return enderecos;

        }

        public List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato)
        {
            return new RepositoryService().Contrato.ObterClientesParticipantesPor(contrato);
        }

        public ClienteParticipante ObterParticipante(Guid participanteId)
        {
            return new RepositoryService().Contrato.PesquisarEnderecoParticipantePor(participanteId);
        }

        public ClienteParticipante ObterParticipante(Contrato contrato, Model.Conta cliente, Endereco endereco, TipoDeVigencia vigencia)
        {
            if (vigencia == TipoDeVigencia.Cliente) return new RepositoryService().Contrato.PesquisarParticipantePor(contrato, cliente);
            else return new RepositoryService().Contrato.PesquisarParticipantePor(contrato, cliente, endereco);
        }

        public bool EstaVigente(Model.Conta cliente, Endereco endereco, DateTime dataInicioVigencia)
        {

            switch (this.TipoVigencia)
            {
                case TipoDeVigencia.Contrato:
                    if (!this.InicioVigencia.HasValue || !this.FimRealVigencia.HasValue) return false;
                    if (this.InicioVigencia.Value.Date > dataInicioVigencia.Date) return false;
                    if (dataInicioVigencia.Date > this.FimRealVigencia.Value.Date) return false;
                    break;


                case TipoDeVigencia.Cliente:
                case TipoDeVigencia.Endereco:
                    ClienteParticipante participante = this.ObterParticipante(this, cliente, endereco, this.TipoVigencia);
                    if (participante != null && this.TemData(participante))
                    {
                        if (participante.InicioVigencia.Value.Date > dataInicioVigencia.Date) return false;
                        if (participante.FimVigencia.Value.Date <= dataInicioVigencia.Date) return false;
                    }

                    break;
                
                case TipoDeVigencia.Por_Veiculo_Instalacao:
                case TipoDeVigencia.Por_veiculo_Contrato:
                    break;

                case TipoDeVigencia.Vazio:
                    throw new ArgumentException("Tipo de Vigencia do contrato não pode ser vazio.", "TipoDeVigencia");

                default:
                    throw new ArgumentException("Tipo de Vigencia do contrato não cadastrada", "TipoDeVigencia");
            }
            return true;
        }

        public void DefineVigenciaClienteParticipante(Model.Conta cliente, Endereco endereco, DateTime dataInicioVigencia)
        {
            if (this.TipoVigencia == TipoDeVigencia.Cliente || this.TipoVigencia == TipoDeVigencia.Endereco)

            {
                ClienteParticipante participante = this.ObterParticipante(this, cliente, endereco, this.TipoVigencia);

                if (participante != null)
                {
                    if (!participante.FimVigencia.HasValue)
                    {
                        int duracao = (this.FimRealVigencia.Value - this.InicioVigencia.Value).Days;

                        participante.InicioVigencia = dataInicioVigencia;
                        participante.FimVigencia = dataInicioVigencia.AddDays(duracao);
                        participante.SalvarDataDeVigencia(participante, this.TipoVigencia);
                    }
                }
            }
        }

        public void AdicionarParticipante(Guid clienteParticipanteId, Model.Conta cliente, List<Endereco> enderecosSelecinados)
        {
            if (null == cliente) throw new ArgumentNullException("Participante inválido.");

            foreach (var endereco in enderecosSelecinados)
                if (!new RepositoryService().Contrato.ExisteEndereco(this, cliente.Id, endereco.Id))
                    new RepositoryService().Contrato.SalvarEnderecosParticipantes(clienteParticipanteId, this, cliente, endereco);
        }

        public void AdicionarClienteParticipante(Contrato contrato, Model.Conta cliente)
        {
            var clienteParticipante = new ClienteParticipante();

            clienteParticipante.ClienteId = cliente.Id;
            clienteParticipante.ContratoId = contrato.Id;
            clienteParticipante.Nome = cliente.Nome;

            new RepositoryService().Contrato.SalvarClienteParticipante(clienteParticipante);
        }

        public void AtualizarContrato(Contrato contrato)
        {
            new RepositoryService().Contrato.Update(contrato);
        }

        public void CriarLinhaDeContrato(Contrato contrato)
        {
            LinhaDeContrato linhaDeContrato = null;

            if (contrato != null)
            {
                linhaDeContrato = new LinhaDeContrato();
                linhaDeContrato.ContratoId = new Lookup(contrato.Id,"");
                linhaDeContrato.Nome = "Linha Padrão";
                linhaDeContrato.InicioContrato = contrato.InicioVigencia.Value;

                new RepositoryService().LinhaDoContrato.Create(linhaDeContrato);
            }
        }

        //public void RenovarClientesParticipantesPor(Guid contratoOrigemId)
        public void RenovarClientesParticipantesPor(Guid contratoOrigemId, Contrato contratoDestino)
        {
            Contrato contratoOrigem = new RepositoryService().Contrato.Retrieve(contratoOrigemId);
            List<ClienteParticipanteDoContrato> clientesParticipantesDoContrato = new RepositoryService().ClienteParticipanteDoContrato.ListarPor(contratoOrigem);

            foreach (ClienteParticipanteDoContrato clienteParticipanteDoContrato in clientesParticipantesDoContrato)
            {
                // Renova data de Vigencia
                if (contratoOrigem.TipoVigencia == TipoDeVigencia.Cliente
                    && clienteParticipanteDoContrato.FimVigencia != null
                    && clienteParticipanteDoContrato.InicioVigencia != null)
                    clienteParticipanteDoContrato.FimVigencia = clienteParticipanteDoContrato.FimVigencia.Value.AddDays((contratoOrigem.DuracaoDias.HasValue ? contratoOrigem.DuracaoDias.Value : 0));

                //clienteParticipanteDoContrato.Clonar(this);
                clienteParticipanteDoContrato.Clonar(contratoDestino);
            }
        }

        public void EfetuaCopiaClientesParticipantesPor(Guid contratoOrigemId, Guid contratoDestinoId)
        {
            var contratoDestino = new Contrato() { Id = contratoDestinoId };

            new RepositoryService().ClienteParticipanteDoContrato.DeleteAll(contratoDestino);
            new RepositoryService().ClienteParticipanteDoEndereco.DeleteAll(contratoDestino);

            Anotacao anotacao = new Anotacao();
            anotacao.Assunto = "Operação do Sistema, Copia dos Clientes Participantes do Contrato.";
            anotacao.EntidadeRelacionada = new Lookup(contratoDestinoId, "contract");

            try
            {
                anotacao.Texto = "Foi inicializada a cópia dos clientes participantes do contrato de origem ";
                new RepositoryService().Anexo.Create(anotacao);
                RenovarClientesParticipantesPor(contratoOrigemId, contratoDestino);
                anotacao.Texto = "A operação foi concluida com sucesso.";
                new RepositoryService().Anexo.Create(anotacao);
            }
            catch (Exception ex)
            {
                anotacao.Texto = ex.Message;
                new RepositoryService().Anexo.Create(anotacao);
            }
        }

        private void DefineDataRealDeVigencia(Contrato contrato, DateTime dataFinalDeVigencia)
        {
            int _statusAntigo = contrato.Status.Value;
            bool statusFoiAlterado = false;

            if (contrato.FimRealVigencia == DateTime.MinValue)
            {
                if (contrato.FimRealVigencia < dataFinalDeVigencia)
                {
                    if (contrato.Status.Value != (int)StatusDoContrato.Rascunho)
                    {
                        this.AlterarStatusDoContrato(contrato, StatusDoContrato.Rascunho);
                        statusFoiAlterado = true;
                    }

                    contrato.FimRealVigencia = dataFinalDeVigencia;
                    new RepositoryService().Contrato.Update(contrato);

                    if (statusFoiAlterado)
                        this.AlterarStatusDoContrato(contrato, (StatusDoContrato)_statusAntigo);
                }
            }
            else
            {
                if (contrato.Status.Value != (int)StatusDoContrato.Rascunho)
                {
                    this.AlterarStatusDoContrato(contrato, StatusDoContrato.Rascunho);
                    statusFoiAlterado = true;
                }

                contrato.FimRealVigencia = dataFinalDeVigencia;
                new RepositoryService().Contrato.Update(contrato);

                if (statusFoiAlterado)
                    this.AlterarStatusDoContrato(contrato, (StatusDoContrato)_statusAntigo);
            }


        }

        private void AlterarStatusDoContrato(Contrato contrato, StatusDoContrato novoStatus)
        {
            new RepositoryService().Contrato.AlterarStatusDoContrato(contrato.Id, -1);
        }

        private bool TemData(ClienteParticipante clienteParticipante)
        {
            return (clienteParticipante.InicioVigencia.HasValue && clienteParticipante.FimVigencia.HasValue);
        }

        private ClienteParticipante DefineDataDeVigencia(ClienteParticipante clienteParticipante, TipoDeVigencia vigencia, int duracao, DateTime dataInicioVigencia)
        {
            //DateTime agora = DateTime.Now;
            DateTime agora = dataInicioVigencia;
            if (!clienteParticipante.InicioVigencia.HasValue)
            {
                clienteParticipante.InicioVigencia = agora;
                //clienteParticipante.FimVigencia = agora.AddDays(ObterDuracao(this.Id));
                clienteParticipante.FimVigencia = agora.AddDays(duracao);
            }
            else
            {
                Contrato contrato = new RepositoryService().Contrato.Retrieve(this.Id);
                clienteParticipante.FimVigencia = clienteParticipante.InicioVigencia.Value.AddDays((contrato.DuracaoDias.HasValue ? contrato.DuracaoDias.Value : 0));
            }

            clienteParticipante.SalvarDataDeVigencia(clienteParticipante, vigencia);
            return clienteParticipante;
        }

        #endregion
    }
}