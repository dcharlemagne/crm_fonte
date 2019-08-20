using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ValueObjects;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IProduto<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? unidadeNegocioId, Guid? familiaComercialId, Guid? familiaMaterialId, Guid? familiaProdutoId);
        T ObterPor(Guid produtoId);
        T ObterPor(int produtoNumber);
        T ObterPor(string produtoNumber);
        void EnviarValorPSD();
        List<T> ObterPor(string[] produtoNumbers);
        Boolean AlterarStatus(Guid productId, int status);
        List<T> ListarPor(List<string> codigosProduto, bool apenasAtivos = true, params string[] columns);
        List<T> ListarPor(String codigoProduto);
        List<T> ListarPorUnidadeNegocio(Guid unidadenegocioId);
        List<T> ListarParaMeta(Guid unidadenegocioId, Guid[] lstGrupoEstoque);
        List<T> ListarPor(List<Guid> Familias, List<Guid> Produtos);
        List<T> ListarPor(Guid produtoId);
        List<T> ListarTodosProdutos(ref int pagina, int contagem, out bool moreRecords);
        List<T> ListarTodosProdutos();
        List<T> ListarProdutosOrcamentoUnidade(Guid orcamentoId);
        List<T> ListarProdutosOrcamentoUnidade(Guid orcamentoId, int? pagina, int? contagem);
        List<T> ListarProdutosOrcamentoCanal(Guid orcamentoId);
        List<T> ListarProdutosOrcamentoCanal(Guid orcamentoId, int? pagina, int? contagem);
        List<T> ListarProdutosMetasCanal(Guid metaId);
        List<T> ListarProdutosMetasCanal(Guid metaId, int? pagina, int? contagem);
        List<T> ListarProdutosMetasUnidade(Guid metaId);
        List<T> ListarProdutosMetasUnidade(Guid metaId, int? pagina, int? contagem);
        List<T> ListarPotencialKaRepresentante(Guid? unidadeNegocioId, int ano, params string[] columns);
        List<T> ListarProdutosKaRepresentante(Guid metaId, List<Guid> lstGrupoEstoque, int? pagina, int? contagem);
        List<T> ListarProdutosSupervisor(Guid metaId);
        List<T> ListarProdutosSupervisor(Guid metaId, int? pagina, int? contagem);
        List<T> ListarProdutos(List<Guid> lstProductId);
        List<T> ListarProdutosSegmento(List<Guid> lstSegmentoId);
        T ProdutoSubstituto(Guid produtoId);

        //CRM4
        List<T> ListarProdutosPor(FamiliaComercial familiaComercial);
        Product BuscarProdutoPor(string codigo, bool integradoEMS, string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica);
        List<Product> ListarProdutosDeTabelaEspecificaPor(string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco);
        List<T> ListarProdutosPor(Fatura notaFiscal);
        List<T> ListarProdutosPorCampos(string codigo, string familia, string produto, string tabelaDePreco);
        SerieDoProduto PesquisarSerieDoProdutoPor(string numeroDeSerie);
        List<Product> BuscarEstruturaDoProdutoPor(string numeroDeSerie, string codigoDoProduto);
        bool AcessoProdutoParaAssistenciaTecnica(Model.Conta postoDeServico, Product produto);
        bool ProdutoPossuiGarantiaEspecificaDentroDaVigenciaPor(string numeroDeSerie);
        decimal BuscarIndiceFinanceiroDoProdutoPor(int codigoCondicaoDePagamento);
        T ObterPor(Ocorrencia ocorrencia);
        T ObterPor(Diagnostico diagnostico);
        //Product ObterPor(string numeroDeSerie);
        T ObterPorNumero(string numero, params string[] columns);
        //PrecoItem ObterPrecoItem(string codigoProduto, int codigoCliente, string unidadeDeNegocio, int codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica);
        Product NumeroDeSerieExisteNoEMS(string numeroDeSerie, string keyCode);
        PrecoItem ObterPrecoItem(string codigoProduto, int codigoCliente, string unidadeDeNegocio, int codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica);
        List<T> ObterPorClienteExtratoFidelidade(Guid cliente);
        List<T> ObterPorAutorizada(Guid autorizada);
        List<T> ObterPorAutorizada(Conta autorizada, string[] codigoGrupoEstoque);
        List<T> ObterPorVendedorExtratoFidelidade(Guid vendedor);
        //CRM4
    }
}
