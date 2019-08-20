using System;
using System.Collections.Generic;
using System.Text;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public class PrecoItem
    {
        public string DescricaoFamiliaComercial { get; set; }
        public string DescricaoItem { get; set; }
        public string CodigoItem { get; set; }
        public string CodigoFamiliaComercial { get; set; }
        public int? QuantidadeMinima { get; set; }
        public decimal? ValorIPI { get; set; }
        public decimal? Preco { get; set; }

        public PrecoItem Obter(string codigoProduto, int codigoCliente, string codigoTabelaDePreco, string codigoEstabelecimento, string unidadeNegocio, int codigoCategoria)
        {
            if (string.IsNullOrEmpty(unidadeNegocio))
                throw new ArgumentNullException("Unidade Negocio esta vazio!");

            if (codigoCategoria < 1)
                throw new ArgumentNullException("Codigo Categoria esta vazio!");

            if (string.IsNullOrEmpty(codigoTabelaDePreco))
                throw new ArgumentNullException("Codigo Tabela de Preco esta vazio!");

            if (codigoCliente < 1)
                throw new ArgumentNullException("Cliente esta vazio!");

            if (string.IsNullOrEmpty(codigoEstabelecimento))
                throw new ArgumentNullException("", "Estabelecimento esta vazio!");

            return (new Domain.Servicos.RepositoryService()).Produto.ObterPrecoItem(codigoProduto, codigoCliente, unidadeNegocio, codigoCategoria, codigoEstabelecimento, codigoTabelaDePreco, 0);
        }
    }
}
