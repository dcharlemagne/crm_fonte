using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using System.Data;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOcorrencia<T> : IRepository<T>, IRepositoryBase
    {
        Servicos.GestaoSLA.SLA ObterSLAPor(Contrato contator, TipoDePrioridade prioridade, Guid localidadeId, TipoDeOcorrencia tipoOcorrencia);
        T ObterPor(string numeroDaOcorrencia);
        T ObterPorNumeroOcorrencia(string numeroDaOcorrencia, Model.Conta assistenciaTecnica);
        string ObterNumeroOcorrenciaPor(Guid id);
        T ObterPor(HistoricoDePostagem historico);
        T ObterPor(Diagnostico servicoExecutado);
        T ObtemReduntanteASTEC(Ocorrencia ocorrencia);
        T ObterOcorrenciaPaiReincidente(string produtoSerial, Guid ignorarOcorrenciaId);
        T ObterImpressaoOcorrenciaIsol(Guid id);
        List<T> ListarPor(Model.Conta cliente);
        List<T> ListarPor(string notaFiscal, string numeroDeSerieDoProduto, string numeroDaOcorrencia, string postoDeServicoId, int status, int tipoDeOcorrencia, string nomeDoCliente, DateTime dataInicial, DateTime dataFinal);
        List<T> ListarPor(UnidadeNegocio unidadeDeNegocio);
        List<T> ListarPor(Extrato extrato, Model.Conta assistenciaTecnica = null);
        List<T> ListarPor(Contrato contrato, Model.Conta cliente);
        List<T> ListarPor(Contrato contrato);
        List<T> ListarPor(StatusDaOcorrencia status);
        List<T> ListarPor(List<StatusDaOcorrencia> ListaStatus);
        List<T> ListarPorAutorizada(Guid autorizadaId);
        List<T> ListarPor(string[] diagnosticoIDs);
        List<T> ListarPorDataCriacao(DateTime dataCriacaoInicio, DateTime dataCriacaoFim, OrigemDaOcorrencia[] arrayOrigem);
        List<T> ListarPreOSPor(string CPFouCNPJ);
        List<T> ListarOcorrenciasParaGeracaoDeArquivoXmlParaOsCorreios();
        List<T> PesquisarOcorrenciaPor(DateTime dataInicial, DateTime dataFinal, UnidadeNegocio unidadeDeNegocio);
        List<T> ListarOcorrenciasDashBoard();
        List<T> ListarOcorrenciasParaRastreioNosCorreios();
        List<T> ListarOcorrenciasPor(Guid solicitante);
        List<T> ListarOcorrenciasEspeciaisPor(Domain.Model.Fatura notaFiscal);
        List<T> ListarPorDiasDeReicidencia(Ocorrencia ocorrencia);
        List<T> ListarPorPerfilUsuarioServico(Model.Contato contato, bool emAndamento);
        List<T> ListarOcorrenciasSemSLADosUltimos(int dias);
        List<Ocorrencia> ListarOcorrenciasComDiagnosticoDivergenteDosUltimos(int dias);
        List<Ocorrencia> ListarOcorrenciasComNumeroDeSerieDosUltimos(int dias);
        List<T> ListarOcorrenciaAbertaComIntervencaoTecnica(Model.Conta assistenciaTecnica);
        List<T> ListarPorStatus(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorStatusDiagnostico(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, string[] statusDiagnostico, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNumeroSerie(string numeroSerie, Model.Conta assistenciaTecnica, string[] status, string[] origem);
        List<T> ListarPorCpfCnpjCliente(string cpfCnpj, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNomeCliente(string nomeCliente, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNotaFiscalCompra(string notaFical, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<ContratosAssociados> PesquisarClientesAssociadosPor(Guid contatoId);
        List<SolucaoOcorrencia> ListarSolucoesOcorrencia(Ocorrencia ocorrencia);
        void AtualizaValoresDosLancamentosAvulsosNo(Guid extratoid);
        void ReabrirOcorrencia(Ocorrencia ocorrencia);
        void FecharOcorrencia(Ocorrencia ocorrencia, SolucaoOcorrencia solucao);
        List<ProdutoBase> GerarLogisticaReversaDo(Model.Conta postoDeServico, Estabelecimento estabelecimento, bool naoUsar = false);
        List<Product> GerarLogisticaReversaDo(Model.Conta postoDeServico, Estabelecimento estabelecimento);
        List<ProdutoBase> ObterLogisticaReversaPor(Guid extratoId);
        Guid SalvarLogisticaReversa(List<Diagnostico> logisticaReversa, Domain.Model.Fatura notaFiscal);
        void AtualizarStuatuDa(Guid ocorrenciaId);

        void AtualizarStatusDosDiagnóstico(Ocorrencia ocorrencia, int novoStatusDoDiagnostico);
        void ExcluirServicosRealizadosPor(Ocorrencia ocorrenciar);
        Guid ObterIdEmailCorporativo(string identificador);
        List<T> ListarParaIsol(Guid contratoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento);
        List<T> ListarParaIsol(Guid contratoId, Guid contatoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie, string numeroOcorrencia, bool exibirTodas, int tipoOcorrencia);
        void AlterarStatus(Guid ocorrenciaid, int status);
        void Cancelar(Guid ocorrenciaid);
        DateTime ObterDataDeCriacaoDoReincidentePorDiagnostico(Guid diagnosticoid);
        List<T> ListarOcorrenciasTransportadoraPor(Domain.Model.Fatura notaFiscal);
        DataTable ListarMenorDataSolucaoOcorrencia();
        List<T> ListarOcorrenciasPorDataCriacao(DateTime? dataCriacaoInicio, DateTime? dataCriacaoFim);
        List<T> ListarOcorrenciasPorDataModificacao();
        List<T> ListarOcorrenciasRecalculaSLA(Feriado feriado);
        T ObterPorProtocoloTelefonico(string protocoloTelefonico);
        List<T> BuscarOcorrenciaPorProtocoloChat(string protocolo);
        List<T> ListarOcorrenciasPorNumeroSerie(string numeroSerie);
        List<T> ListarOcorrenciasPorLinhaDoContrato(Guid linhaDeContratoId);
    }
}