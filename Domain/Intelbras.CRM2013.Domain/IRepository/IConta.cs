using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos.Docs;
using System.Data;
using Microsoft.Xrm.Sdk.Messages;
using System.Collections;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IConta<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(String RazaoSocial);
        List<T> ListarGuidContasParaEnviarRegistroFielo(List<Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout> filtroContas);
        List<T> ListarPorCategoria(string[] codigoCategorias);
        List<T> ListarContasSemMascara();
        T ObterPor(String CPFCNPJ, int tipoConstituicao);
        T ObterPor(String CPFCNPJ);

        T ObterOutraContaPorCpfCnpj(String CPFCNPJ, Guid id, int tipoContituicao);
        T ObterPorIntegracaoCrm4(String guidCrm40conta);
        T ObterPorCodigoEmitente(String accountNumber);
        void AtribuirEquipeParaConta(Guid Equipe, Guid Conta);
        void AdicionarPerfilParaEquipe(Guid Equipe);
        bool AlterarStatus(Guid accountid, Intelbras.CRM2013.Domain.Enum.Conta.StateCode stateCode, Intelbras.CRM2013.Domain.Enum.Conta.StatusCode statusCode);
        void AlterarStatus(string entidade, Guid id, int status, int razaoStatus);
        List<T> ListarTudo(params string[] columns);
        List<T> ListarMetaDoCanalPor(Guid unidadeNegocioId, int ano, params string[] columns);
        List<T> ListarMatrizesParticipantes();
        List<T> ListarMatrizesParticipantesApuracaoCentralizadaNaMatriz();
        List<T> ListarContasParticipantesApuracaoPorFilial();
        List<T> ListarContasParticipantes(Guid unidadenegocioId);
        List<T> ListarContasParticipantes(Guid unidadenegocioId, List<Guid> lstIdCanal);
        List<T> ListarContasOrcamentoCanal(Guid orcamentoId);
        List<T> ListarContasOrcamentoCanal(Guid orcamentoId, int? pagina, int? contagem);
        List<T> ListarContasMetaCanal(Guid metaId);
        List<T> ListarContasMetaCanal(Guid metaId, int? pagina, int? contagem);
        List<T> ListarContasMetaCanal(Guid unidadeNegocioId, int ano);
        List<T> ListarContas(ref int pagina, int contagem, out bool moreRecords);
        List<T> ListarContasFiliaisPorMatriz(Guid contaMatriz);
        List<T> ListarFiliaisMatriz(Guid contaId, params string[] columns);
        List<T> ListarContasParticipantesMAtrizEFilial();
        List<T> ListarParticipantesDoProgramaApenasComApuracaoBeneficio(Guid? unidadeNegocioId, params string[] columns);
        DomainCollection<T> ListarPor(Domain.Model.Classificacao classificacao, Domain.Model.Subclassificacoes subClassificacoes, int count = 100, string pagingCookie = "", int pageNumber = 1);
        T ObterCanal(string codigoemitente);
        List<T> ListarContasAstec(DateTime data);

        //CRM4
        Representante PesquisarRepresentantePor(int codGrupo);
        T PesquisarPor(Documento documento);
        T PesquisarPor(int codigo);
        T PesquisarPor(string nome);
        T ObterPorEmail(string email);
        T ObterPor(Model.Contato contato);
        T ObterPorLogin(string login);
        T ObterPor(Ocorrencia ocorrencia);
        T ObterPor(Extrato extrato);
        T ObterPor(Diagnostico diagnostico);
        T ObterPor(Domain.Model.Fatura fatura);
        T ObterAutorizadaPor(Guid ocorrenciaId);
        T ObterPor(Ocorrencia ocorrencia, string campoOrigem);
        List<T> ListarPor(Contrato contrato);
        List<T> ListarPor(Contrato contrato, int pagina, int quantidadePorPagina, ref bool existemMaisRegistros, ref string cacheDaPagina);
        List<T> PesquisarPor(string valorDoCampo, string nomeDoCampo, string condicao, int NumeroDeRegistros);
        List<Domain.Model.Conta> PesquisarPorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId);
        List<T> PesquisarPorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId, NaturezaDoCliente natureza);
        List<T> PesquisarClientes(string nome);
        List<T> ListarPor(Usuario criadoPor, DateTime dataCriacao, DateTime dataCriacaoFim);
        List<T> ListarDistribuidorPor(Guid contatoId);
        List<T> ListarPorAtributos(Model.Conta cliente);
        Telefone PesquisarTelefonePor(Model.Conta cliente);
        List<Representante> ListarRepresentantesB2BPor(Model.Conta cliente);
        List<Categoria> ListarCategoriasB2BPor(Model.Conta cliente);
        List<CategoriaUN> ListarCategoriaUNB2BPor(Model.Conta cliente, Guid contato);
        decimal BuscarDescontoMangaPor(string codigoRepresentante, string codigoCliente, string codigoUnidadeDeNegocio, string codigoCategoria, string codigoFamiliaComercial);
        List<decimal> BuscarDescontoPor(int codigoCliente, string codigoUnidadeDeNegocio, int codigoCategoria, string codigoFamiliaComercial, int qtdeDeProdutos);
        Boolean CidadeZonaFranca(string cidade, string uf);
        bool ExisteDuplicidade(Model.Conta cliente);
        void CriarAnotacao(Guid Id, string entidade, Anotacao anexo);
        Categoria PesquisarCategoriaCliente(int codigo, UnidadeNegocio UnidadeDeNegocio);
        Categoria ObterCategoriaDoClientePor(Guid id, string codigo);
        Dictionary<Guid, string> ListarDistribuidorInfo(Guid accountId);
        List<T> ObterClientesPorId(string[] clientesId);
        Representante ObterRepresentantePor(Guid id, string codigo);
        List<T> ObterRevendas(Guid distribuidor);
        List<T> ObterDistribuidor(string cnpj);
        List<T> ObterAutorizadas(int modificadasEmDias);
        List<T> ListarPorIntegracaoRevendaSite(Domain.Enum.Cliente.IntegracaoRevendaSite[] listIntegracaoRevendaSite);
        //CRM4
        List<T> ListarContasRecategorizar(DateTime dtTrimestreAtual);
        List<T> ListarContasCategorizar(DateTime dtMesAnterior, DateTime dtMesAtual);
        List<T> ListarContasCategorizacao();
        List<T> ListarContasReCategorizacao();
        //List<T> ListarFaturamentoDw();
        List<T> ListarContasAcessoExtranet();
        List<T> ConsultaAssitenciaFetch(string queryFetch);
        DataTable ObterSellinAstec(string dt_inicio, string dt_fim);
        ExecuteMultipleResponse UpdateMultiplos(List<T> collection);

        String ObterTelefoneAssistencias(string nomeProcedure, ArrayList sqlParameters, string pSaida);

        T BuscaMatrizEconomica(String raizcnpj);

        List<T> ListarContasRecategorizarMensal(DateTime dtMesAnterior);
        List<T> ListarContasRecategorizacaoMensal();
        DataTable ObterSellinProvedoresSolucoes(string dt_inicio, string dt_fim);
        
    }
}
