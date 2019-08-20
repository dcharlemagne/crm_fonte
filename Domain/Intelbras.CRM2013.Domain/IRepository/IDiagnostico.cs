using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDiagnostico<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPagamentoPorAstec(int dia);
        List<T> ListarDiagnosticosParaExportacaoDePedidosASTEC();
        List<T> BuscarservicosExecutadosPorFiltros(Ocorrencia ocorrencia);
        List<T> ListarDiagnosticoPortalPor(Ocorrencia ocorrencia);
        List<T> ListarAtualizadosHoje();
        List<T> ListarPor(Ocorrencia ocorrencia);
        List<T> ListarPorFilhoEPai(Guid id);
        List<T> ListarPorFilhoEPai(Guid id, int statuscode, Guid notEqualId);
        List<T> ListarPor(string numeroNotaFiscal, string numeroSerieNotaFiscal, string codigoEstabelecimento);
        Diagnostico BuscarDadosDoServicoPor(Model.Conta postoDeServico, Product produto, DefeitoOcorrenciaCliente defeito, Solucao solucao);
        T ObterDuplicidade(Guid ocorrenciaId, Guid produtoId, Guid servicoId, Guid defeitoId, string notaFiscal, string serieNotaFiscal, Guid diagnosticoid);
        T ObterDiagnosticoSemNotaFical(Ocorrencia ocorencia, int codigoProduto);
        T ObterDiagnosticoPai(Guid id);
        void Cancelar(Guid id);
        void LimparCampoExtratoLogisticaReversa(Guid extratoId);
        decimal PrecoParaGerarPedidoAstec(Diagnostico diagnostico, out List<string> mensagemErro);
        bool DiagnosticoTemNotaFiscal(Guid id);
        int QuantidadeProdutosFaturados(Guid id);
        List<Diagnostico> ObterOsStatusDeDiagnoticoPorOcorrencia(Guid ocorrenciaid);
        Diagnostico CarregarCamposRelacionadosDiagnostico(Diagnostico diagnostico);
    }
}
