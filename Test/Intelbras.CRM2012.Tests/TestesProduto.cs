using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.Message.Helper.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesProduto : Base
    {
        private RepositoryService RepositoryService = null;

        public TestesProduto()
            : base()
        {
            RepositoryService = new RepositoryService(OrganizationName, IsOffline);
        }

        [TestMethod]
        public void Teste_Esb_Msg0088_Postar()
        {
            var request = new Intelbras.Message.Helper.MSG0088(Guid.NewGuid().ToString(), "numero operacao")
            {
                CodigoProduto = "1884034",
                Nome = "Pasta Térmica Tubo BR VIP S3020",
                Descricao = "Pasta Térmica Tubo BR VIP S3020",
                PesoEstoque = (decimal?)3243243.00,
                Situacao = 0,
                TipoProduto = 12,
                NaturezaProduto = 993520000,
                GrupoEstoque = 10,
                UnidadeNegocio = "SEC",
                Segmento = "3400",
                Familia = "34000",
                SubFamilia = "3400000",
                Origem = "34000000",
                UnidadeMedida = "UN",
                GrupoUnidadeMedida = "Unidade Padrão",
                FamiliaMaterial = "40006200",
                FamiliaComercial = "34000000",
                ListaPreco = "Lista Padrão",
                Moeda = "real",
                QuantidadeDecimal = 1,
                CustoAtual = (decimal?)0.0,
                PrecoLista = (decimal?)0.0,
                Fabricante = "",
                NumeroPecaFabricante = "",
                VolumeEstoque = (decimal?)0.0,
                ComplementoProduto = "complemento",
                URL = "",
                QuantidadeDisponivel = (decimal?)4343.00,
                ExigeTreinamento = true,
                Fornecedor = "",
                CustoPadrao = (decimal?)0.0,
                RebateAtivado = false,
                ConsiderarOrcamentoMeta = true,
                FaturamentoOutroProduto = false,
                QuantidadeMultipla = (decimal)1.00,
                DataAlteracaoPrecoVenda = DateTime.Now,
                ShowRoom = true,
                AliquotaIPI = (decimal)0.0,
                NCM = "1234567890",
                EAN = "1234567890123456",
                BloquearComercializacao = true,
                PossuiSubstituto = false,
                CodigoProdutoSubstituto = "",
                EKit = false,
                ComercializadoForaKit = false,
                PoliticaPosVendas = 993520000,
                TempoGarantia = 10,
                ProdutosFilhos = new List<ProdutoFilho>
                {

                }
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String retorno = String.Empty;

            bool sucesso = integ.Postar(usuario, senha, request.GenerateMessage(), out retorno);

            Assert.IsTrue(sucesso, retorno);
        }

        [TestMethod]
        public void Teste_Esb_Msg0088_Enviar()
        {
            var codigoProduto = "4000021";
            var produto = RepositoryService.Produto.ObterPor(codigoProduto);

            var msg0088 = new Domain.Integracao.MSG0088(OrganizationName, IsOffline);
            msg0088.Enviar(produto);
        }

        [TestMethod]
        public void Teste_AtualizarCamposProduto()
        {
            int pagina = 1;
            bool moreRecords = false;

            do
            {
                var lista = RepositoryService.Produto.ListarTodosProdutos(ref pagina, 500, out moreRecords);

                foreach (var item in lista)
                {
                    var produto = new Domain.Model.Product(OrganizationName, IsOffline)
                    {
                        ID = item.ID,
                        EKit = false,
                        ComercializadoForaKit = false,
                        PossuiSubstituto = false,
                        IntegrarNoPlugin = true
                    };
                    
                    RepositoryService.Produto.Update(produto);
                }
            }
            while (moreRecords);
        }
    }
}
