using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using SDKore.DomainModel;
using System.Data;
using Microsoft.Xrm.Sdk;
using System.Data.SqlClient;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OcorrenciaService
    {
        #region propriedades

        Ocorrencia ocorrencia = null;

        public Ocorrencia Ocorrencia
        {
            get { return ocorrencia; }
            set { ocorrencia = value; }
        }

        #endregion

        #region construtor

        private RepositoryService RepositoryService { get; set; }

        public OcorrenciaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OcorrenciaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public OcorrenciaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region métodos

        public void Criar()
        {
            if (!ADataDeEntregaInformadaMaiorQueAtual())
                throw new ArgumentException("Data de Entrega ao Cliente não pode ser maior que a data atual.");

            Areas area = IdentificarAreaDeAtendimento();

            switch (area)
            {
                case Areas.ISOL:
                    AtenderISOL();
                    break;
                case Areas.ASTEC:
                    AtenderAstec();
                    break;
                case Areas.CallCenter:
                    AtenderCallCenter();
                    break;
                default:
                    break;
            }

            this.MontarEstruturaDeAssuntoDaOcorrencia();
        }

        public void Atualizar()
        {
            if (!ADataDeEntregaInformadaMaiorQueAtual())
                throw new ArgumentException("Operação não realizada. Data de Entrega ao Cliente não pode ser maior que a data atual.");

            Areas area = IdentificarAreaDeAtendimento();

            switch (area)
            {
                case Areas.ISOL:
                    AtenderISOL();
                    break;

                case Areas.ASTEC:
                    AtenderAstec();
                    break;

                case Areas.CallCenter:
                    AtenderCallCenter();
                    break;

                default:
                    break;
            }

            this.MontarEstruturaDeAssuntoDaOcorrencia();
        }

        private void AtenderAstec()
        {
            if (this.EhDuplicada())
            {
                throw new ArgumentException("Operação não realizada. Foi identificada ocorrência redundante. Verifique 'Posto de Serviço', 'Produto', 'Cliente/Cliente OS', 'Número de Série', 'Número da Nota Fiscal'.");
            }

            this.CalculaSLADaASTEC();
            this.CriarVinculoPorReincidencia();
        }

        private void AtenderISOL()
        {
            if (!Ocorrencia.DataDeConclusao.HasValue)
            {
                if (Ocorrencia.RazaoStatus != null && Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Fechada)
                    throw new ArgumentException("Preencha o campo Data de Conclusão para alterar o Status para fechado!");
            }
            else if (Ocorrencia.DataOrigem != null && Ocorrencia.DataOrigem.HasValue && Ocorrencia.DataDeConclusao != null && Ocorrencia.DataDeConclusao.Value < Ocorrencia.DataOrigem)
            {
                throw new ArgumentException("A Data de Conclusão não pode ser menor que a Data de Abertura da Ocorrência!");
            }

            if(Ocorrencia.DataDeConclusao != null && Ocorrencia.VeiculoId != null) {
                var veiculo = new CRM2013.Domain.Servicos.RepositoryService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).Veiculo.Retrieve(Ocorrencia.VeiculoId.Id);
                if(veiculo != null && veiculo.DataDeInstalacao == null) {
                    veiculo.DataDeInstalacao = Ocorrencia.DataDeConclusao;
                    var veiculoTemp = new Domain.Servicos.VeiculoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).Persistir(veiculo);
                }
            }

            this.CalculaSLADaISOL();
            this.ValidaVigenciaContrato();
        }

        private void AtenderCallCenter()
        {
        }

        private bool ADataDeEntregaInformadaMaiorQueAtual()
        {
            if (this.Ocorrencia.DataDeEntregaClienteInformada.HasValue)
                return this.Ocorrencia.DataDeEntregaClienteInformada.Value.Date <= DateTime.Now.Date;
            else
                return true;
        }

        private bool EhDuplicada()
        {
            if (this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Analise
                || this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Peça
                || this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Aguardando_Conserto
                || this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Pedido_Solicitado
                || this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Conserto_Realizado
                || this.Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Aberta)
                return RepositoryService.Ocorrencia.ObtemReduntanteASTEC(this.Ocorrencia) != null;
            else
                return false;
        }

        private void CriarVinculoPorReincidencia()
        {
            if (this.Ocorrencia.ProdutosDoCliente != null && this.ocorrencia.TipoDeOcorrencia == 300009)
            {
                var reincidente = RepositoryService.Ocorrencia.ObterOcorrenciaPaiReincidente(
                    this.Ocorrencia.ProdutosDoCliente,
                    this.Ocorrencia.Id);

                if (reincidente != null)
                {
                    if (this.Ocorrencia.OcorrenciaPai != null)
                    { 
                        if (this.Ocorrencia.OcorrenciaPai.Id != reincidente.Id)
                        {
                            this.Ocorrencia.OcorrenciaPaiId = new Lookup(reincidente.Id, "incident");
                        }
                    }
                    else
                    {
                        this.Ocorrencia.OcorrenciaPaiId = null;
                    }

                    if (this.Ocorrencia.OcorrenciaPaiId == null)
                    {
                        this.Ocorrencia.OcorrenciaPaiId = new Lookup(reincidente.Id, "incident");
                    }
                }
            }
        }

        public void CalculaSLADaASTEC()
        {
            switch (this.ocorrencia.RazaoStatus)
            {
                case (int)StatusDaOcorrencia.Aguardando_Analise:
                    if (!this.ocorrencia.DataOrigem.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataOrigem.Value.AddDays(2);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataOrigem.Value.AddDays(3);
                    break;

                case (int)StatusDaOcorrencia.Aguardando_Peça:
                    if (!this.ocorrencia.DataOrigem.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataOrigem.Value.AddDays(6);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataOrigem.Value.AddDays(8);
                    break;

                case (int)StatusDaOcorrencia.Pedido_Solicitado:
                    if (!this.ocorrencia.DataOrigem.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataOrigem.Value.AddDays(9);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataOrigem.Value.AddDays(17);
                    break;

                case (int)StatusDaOcorrencia.Aguardando_Conserto:
                    if (!this.ocorrencia.DataOrigem.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataOrigem.Value.AddDays(15);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataOrigem.Value.AddDays(17);
                    break;

                case (int)StatusDaOcorrencia.Conserto_Realizado:
                    if (!this.ocorrencia.DataOrigem.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataOrigem.Value.AddDays(20);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataOrigem.Value.AddDays(23);
                    break;

                case (int)StatusDaOcorrencia.Auditoria:
                    if (this.ocorrencia.DataAjustePosto.HasValue)
                    {
                        this.ocorrencia.DataEscalacao = this.ocorrencia.DataAjustePosto.Value.AddDays(2);
                        this.ocorrencia.DataSLA = this.ocorrencia.DataAjustePosto.Value.AddDays(3);
                    }
                    else if (this.ocorrencia.DataDeEntregaClienteDigitada != null && this.ocorrencia.DataDeEntregaClienteDigitada != DateTime.MinValue)
                    {
                        this.ocorrencia.DataEscalacao = this.ocorrencia.DataDeEntregaClienteDigitada.Value.AddDays(2);
                        this.ocorrencia.DataSLA = this.ocorrencia.DataDeEntregaClienteDigitada.Value.AddDays(3);
                    }
                    else return;
                    break;

                case (int)StatusDaOcorrencia.Ajuste_Posto_de_Serviço:
                    if (!this.ocorrencia.DataEnvioAjustePosto.HasValue) return;
                    this.ocorrencia.DataEscalacao = this.ocorrencia.DataEnvioAjustePosto.Value.AddDays(20);
                    this.ocorrencia.DataSLA = this.ocorrencia.DataEnvioAjustePosto.Value.AddDays(23);
                    break;
            }

        }

        private void CalculaSLADaISOL()
        {

            #region Validações

            if (!ocorrencia.TipoDeOcorrencia.HasValue)
                return;

            if (!(ocorrencia.TipoDeOcorrencia.Value >= 200090 && ocorrencia.TipoDeOcorrencia.Value <= 200099))
                return;

            if (!ocorrencia.Prioridade.HasValue || ocorrencia.Localidade == null || ocorrencia.Contrato == null)
                return;

            if (RepositoryService.LinhaDoContrato.ListarPor(ocorrencia.Contrato).Count == 0)
                return;

            #endregion

            var sla = RepositoryService.Ocorrencia.ObterSLAPor(ocorrencia.Contrato, ocorrencia.Prioridade.Value, ocorrencia.Localidade.Id, (TipoDeOcorrencia)ocorrencia.TipoDeOcorrencia);

            //Calcula as respectivas datas de SLA e Escalação e popula as propriedades do próprio objeto
            sla = ocorrencia.CalcularDataDeAtendimento(sla);

            if (sla == null)
                return;

            this.ocorrencia.DataSLA = sla.DataSLA.Value;
            this.ocorrencia.DataEscalacao = sla.DataEscalacao.Value;

        }

        private void ValidaVigenciaContrato()
        {
            #region Inicializa variáveis

            if (string.IsNullOrEmpty(ocorrencia.EnderecoId))
                throw new ArgumentException("Selecione o endereço do contrato");

            Contrato contrato = RepositoryService.Contrato.Retrieve(ocorrencia.ContratoId.Id);
           // Model.Conta cliente = new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline) { Id = ocorrencia.Cliente.Id };
            Model.Endereco endereco = new Model.Endereco(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline) { Id = new Guid(ocorrencia.EnderecoId) };

            #endregion

            

            //Model.Endereco endereco = new Model.Endereco() { Id = new Guid(ocorrencia.EnderecoId) };

            if(ocorrencia.DataOrigem == null)
                throw new ArgumentException("Operação não realizada. Data de Origem da Ocorrência inválida");
            if (!ocorrencia.Contrato.EstaVigente(ocorrencia.Cliente, endereco, ocorrencia.DataOrigem.Value))
                throw new ArgumentException("Operação não realizada. Data de Vigência do Contrato inválida");
        }

        public void AtualizarVigenciaContrato()
        {
            #region Validações

            if (this.ocorrencia.ContratoId == null)
                throw new ArgumentNullException("Ocorrencia.Contrato", "Contrato da Ocorrência não pode ser vazio.");

            if (this.ocorrencia.ClienteId == null)
                throw new ArgumentNullException("Ocorrencia.Contrato.Cliente", "Cliente do Contrato não pode ser vazio.");

            if (!ocorrencia.DataDeConclusao.HasValue)
                throw new ArgumentNullException("ocorrencia.DataDeConclusao", "A data de Conclusão não pode ser vazia!");

            #endregion

            if (ocorrencia.Cliente == null)
                ocorrencia.Cliente = RepositoryService.Conta.Retrieve(this.ocorrencia.ClienteId.Id);
            if (ocorrencia.Contrato == null)
                ocorrencia.Contrato = RepositoryService.Contrato.Retrieve(this.ocorrencia.ContratoId.Id);

            Model.Endereco endereco = new Model.Endereco(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline) { Id = new Guid(ocorrencia.EnderecoId) };

            // Define a vigência para Cliente Participante do Contrato caso seja Instalação e esteja Fechada
            if (ocorrencia.TipoDeOcorrencia.HasValue && ocorrencia.TipoDeOcorrencia.Value == (int)TipoDeOcorrencia.Instalacao && ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Fechada)
                ocorrencia.Contrato.DefineVigenciaClienteParticipante(ocorrencia.Cliente, endereco, ocorrencia.DataDeConclusao.Value);
        }

        public Areas IdentificarAreaDeAtendimento()
        {
            if (Ocorrencia.Origem.HasValue)
                if (Ocorrencia.Origem.Value == 200004 || Ocorrencia.Origem.Value == 200006)
                    return Areas.ASTEC;
            if (Ocorrencia.ContratoId != null)
                return Areas.ISOL;

            return Areas.CallCenter;
        }

        private void MontarEstruturaDeAssuntoDaOcorrencia()
        {
            if (this.Ocorrencia.AssuntoId == null)
                return;

            var assunto = new Assunto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline) { Id = this.Ocorrencia.AssuntoId.Id };
            ocorrencia.EstruturaDeAssunto = assunto.EstruturaDoAssunto();
        }

        public void AtualizarValorDoServicoASTEC()
        {
            if (IdentificarAreaDeAtendimento() == Areas.ASTEC)
                if (ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Auditoria || ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Aguardando_Conserto)
                    ProverValorDoServico();
        }

        public void ValidaAberturaOcorrenciaASTEC()
        {
            if (Ocorrencia.Produto == null)
                throw new ArgumentException("Produto não foi encontrado para Número de Série ou Código informado.");

            if (!Ocorrencia.Produto.PermiteOS)
                throw new ArgumentException(string.Format("Produto {0} - {1} não permite a abertura de Ocorrência ", Ocorrencia.Produto.Codigo, Ocorrencia.Produto.Nome));

            bool permiteAcesso = RepositoryService.Produto.AcessoProdutoParaAssistenciaTecnica(Ocorrencia.Autorizada, Ocorrencia.Produto);
            if (!permiteAcesso)
                throw new ArgumentException(String.Format("Posto de serviço não autorizado a abrir ocorrência para o produto {0} - {1} ", Ocorrencia.Produto.Codigo, Ocorrencia.Produto.Nome));

            if (Ocorrencia.NotaFiscalFatura == null || !Ocorrencia.NotaFiscalFatura.DataEmissao.HasValue)
                throw new ArgumentException("Nenhuma ocorrência foi localizada.");

            if (!Ocorrencia.GarantiaPorContratoEstaVigente())
            {
                if (Ocorrencia.Produto.GarantiaEmDias > 0)
                {
                    if (Ocorrencia.NotaFiscalFatura.DataEmissao.Value.AddMonths(Ocorrencia.Produto.GarantiaEmDias) <= Ocorrencia.DataOrigem)
                         throw new ArgumentException("Produto fora de garantia.");
                 }
                else
                {
                    if (Ocorrencia.NotaFiscalFatura.DataEmissao.Value.AddMonths(Convert.ToInt32(SDKore.Configuration.ConfigurationManager.GetSettingValue("GarantiaDeProdutoEmMesesSemConfiguracaoNoERP"))) <= DateTime.Now)
                        throw new ArgumentException("Produto fora de garantia.");
                }
            }
        }

        private void ProverValorDoServico()
        {
            ocorrencia.ValorServico = Ocorrencia.ObterValorDeServico();
        }

        public void AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados()
        {
            List<Diagnostico> statusDiagnosticos = RepositoryService.Diagnostico.ObterOsStatusDeDiagnoticoPorOcorrencia(Ocorrencia.Id);
            ocorrencia = RepositoryService.Ocorrencia.Retrieve(ocorrencia.Id);


            var statusOcorrencia = ObterStatusDaOcorrenciaPorDiagnosticos(statusDiagnosticos, ocorrencia);

            if (statusOcorrencia != 0)
            {
            
                if (this.Ocorrencia.RazaoStatus.Value == statusOcorrencia)
                    return;

                if (this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Aguardando_Analise
                   || this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Aguardando_Peça
                   || this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Pedido_Solicitado
                   || this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Aguardando_Conserto
                   || this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Conserto_Realizado
                   || this.Ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Aberta)
                {
                    this.Ocorrencia.RazaoStatus = statusOcorrencia;
                    RepositoryService.Ocorrencia.Update(this.ocorrencia);
                }
            }
        }

        private int ObterStatusDaOcorrenciaPorDiagnosticos(List<Diagnostico> statusDiagnosticos, Ocorrencia ocorrencia)
        {
            if (statusDiagnosticos.Count == 0) return 200035;

            foreach (var diagnostico in statusDiagnosticos)
            {
                if (diagnostico.pecaEmEStoque == true)
                {
                    if (ocorrencia.DataDeConclusao != null || ocorrencia.DataDeConsertoInformada != null)
                    {
                        return 200038;
                    }
                    else
                    {
                        return 200037;
                    }
                }
            }

            var listaDeOrdensDoProcessoDaOcorrencia = ConverterStatusDoDiagnosticoParaOrdemDoProcessoDeResolucaoDaOcorrencia(statusDiagnosticos);

            switch (MenorOrdemDoProcessoDeResolucao(listaDeOrdensDoProcessoDaOcorrencia))
            {
                case 2: //Aguardando Peça
                    return 200036;
                case 3: //Pedido Solicitado ao EMS
                    return 200045;
                case 4: //Aguardando Conserto
                    return 200037;
                case 5: //Conserto Realizado
                    return 200038;
            }

            return 0;
        }

        private List<int> ConverterStatusDoDiagnosticoParaOrdemDoProcessoDeResolucaoDaOcorrencia(List<Diagnostico> statusDiagnosticos)
        {
            List<int> statusDoProcesso = new List<int>();

            foreach (var status in statusDiagnosticos)
            {
                switch (status.RazaoStatus)
                {
                    case 1:
                    case 6:
                        statusDoProcesso.Add(2);
                        break;
                    case 3:
                        statusDoProcesso.Add(3);
                        break;
                    case 4:
                        statusDoProcesso.Add(4);
                        break;
                    case 5:
                        statusDoProcesso.Add(5);
                        break;
                }
            }

            return statusDoProcesso;
        }

        private int MenorOrdemDoProcessoDeResolucao(List<int> lista)
        {
            var menorStatusDoProcessoDeResolucao = int.MaxValue;
            foreach (var item in lista)
                if (item < menorStatusDoProcessoDeResolucao)
                    menorStatusDoProcessoDeResolucao = item;

            return menorStatusDoProcessoDeResolucao;
        }

        /// <summary>
        /// Verifica se deve Encerrar a ocorrência, aguarda 3 minutos e verifica novamente antes de fechar! Na segunda verificação é feita uma nova busca
        /// </summary>
        public void EncerraOcorrenciaSeNecessario()
        {
            if (!DeveEncerrarOcorrencia())
                return;

            System.Threading.Thread.Sleep(120000);

            ocorrencia = RepositoryService.Ocorrencia.Retrieve(this.Ocorrencia.Id);

            if (DeveEncerrarOcorrencia())
                FecharOcorrencia();
        }

        private void FecharOcorrencia()
        {
            SolucaoOcorrencia solucaoOcorrencia = new SolucaoOcorrencia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
               {
                   DataHoraConclusao = DateTime.Now,
                   OcorrenciaId = ocorrencia.Id
               };

            RepositoryService.Ocorrencia.FecharOcorrencia(ocorrencia, solucaoOcorrencia);
        }

        private bool DeveEncerrarOcorrencia()
        {
            if (this.IdentificarAreaDeAtendimento() != Areas.CallCenter)
                return false;

            if (ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.Resolvido
                || ocorrencia.RazaoStatus == (int)StatusDaOcorrencia.CanceladaSistema)
                return false;

            if (ocorrencia.ManterAberto == true)
                return false;
                                   
            return false;
        }

        public void AtualizaDataSolucaoCliente()
        {
            DataTable dtSolucoesOcorrencia = RepositoryService.Ocorrencia.ListarMenorDataSolucaoOcorrencia();

            foreach (DataRow item in dtSolucoesOcorrencia.Rows)
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = SDKore.Configuration.ConfigurationManager.GetDataBase("crm2015db");
                con.Open();
                var querydel = "UPDATE Incident SET itbc_data_hora_solucao_cliente = '" + Convert.ToDateTime(item["actualend"].ToString()).ToString("yyyy-MM-dd  HH:mm:ss.fff") + "' WHERE IncidentId = '" + item["IncidentId"].ToString() + "'";
                SqlCommand commanddel = new SqlCommand(querydel, con);
                commanddel.ExecuteNonQuery();
                con.Close();
            }
        }
        public Ocorrencia BuscaOcorrencia(Guid ocorrencia)
        {
            return RepositoryService.Ocorrencia.Retrieve(ocorrencia);
        }

        public List<Ocorrencia> ListarOcorrenciasPorDataCriacao(DateTime? DataInicial, DateTime? DataFinal)
        {
            return RepositoryService.Ocorrencia.ListarOcorrenciasPorDataCriacao(DataInicial, DataFinal);
        }
        public List<Ocorrencia> ListarOcorrenciasPorDataModificacao()
        {
            return RepositoryService.Ocorrencia.ListarOcorrenciasPorDataModificacao();
        }
        
        public List<Ocorrencia> ListarOcorrenciasRecalculaSLA(Feriado Feriado)
        {
            return RepositoryService.Ocorrencia.ListarOcorrenciasRecalculaSLA(Feriado);
        }
        /// <summary>
        /// Busca a ocorrencia pelo numero do protocolo do chat
        /// </summary>
        /// <param name="protocolo"></param>
        /// <returns>Retorna a ocorrencia localizada</returns>
        public List<Ocorrencia> BuscarOcorrenciaPorProtocoloChat(string protocolo)
        {
            return RepositoryService.Ocorrencia.BuscarOcorrenciaPorProtocoloChat(protocolo);
        }

        /// <summary>
        /// Inicia a integração com o Barramento
        /// </summary>
        /// <param name="objOcorrencia"></param>
        /// <param name="anexo"></param>
        /// <returns>Retorna o resultado da integração</returns>
        public string IntegracaoBarramento(Ocorrencia objOcorrencia)
        {
            Domain.Integracao.MSG0300 msg0300 = new Domain.Integracao.MSG0300(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msg0300.Enviar(objOcorrencia);
        }
        
        public List<Ocorrencia> ListarOcorrenciasPorNumeroSerie( string numeroSerie)
        {
            return RepositoryService.Ocorrencia.ListarOcorrenciasPorNumeroSerie(numeroSerie);
        }

        public List<Ocorrencia> ListarOcorrenciasPorLinhaDoContrato( Guid linhaDeContratoId)
        {
            return RepositoryService.Ocorrencia.ListarOcorrenciasPorLinhaDoContrato(linhaDeContratoId);
        }
        #endregion

    }
}
