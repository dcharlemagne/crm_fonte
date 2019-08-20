using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0101 : Base, IBase<Message.Helper.MSG0101, Domain.Model.PrecoProduto>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private RepositoryService RepositoryService { get; set; }
        #endregion

        #region Construtor

        public MSG0101(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            RepositoryService = new RepositoryService(org, isOffline);
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0101>(mensagem);

            var lstPrecoProduto = this.DefinirPropriedadesLista(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0101R1>(numeroMensagem, retorno);
            }

            List<PrecoProduto> lstPrecoProdutoRetorno = new Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarPor(lstPrecoProduto);

            if (lstPrecoProdutoRetorno == null || lstPrecoProdutoRetorno.Count == 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Preço(s) não encontrado(s).";
                return CriarMensagemRetorno<Pollux.MSG0101R1>(numeroMensagem, retorno);
            }


            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração Ocorrida com sucesso.";
            retorno.Add("ProdutosItens", this.GerarListaProdutoValorItem(lstPrecoProdutoRetorno));
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0101R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public List<PrecoProduto> DefinirPropriedadesLista(Intelbras.Message.Helper.MSG0101 xml)
        {
            List<PrecoProduto> lstPrecoProduto = new List<PrecoProduto>();

            List<String> lstCodProd = new List<String>();
            foreach (var item in xml.ProdutosItens)
            {
                lstCodProd.Add(item.CodigoProduto);
            }

            List<Product> lstProduto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarProdutosPorCodigo(lstCodProd);

            Guid contaProduto = new Guid();
            if (!String.IsNullOrEmpty(xml.Conta) && xml.Conta.Length == 36)
                contaProduto = new Guid(xml.Conta);
            else if (!String.IsNullOrEmpty(xml.Conta) && xml.Conta.Length < 36)
            {
                Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaContaPorCpfCnpj(xml.Conta);

                if (conta != null)
                {
                    contaProduto = conta.ID.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Canal não encontrado.";
                    lstPrecoProduto = null;
                    return lstPrecoProduto;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Canal não enviado.";
                lstPrecoProduto = null;
                return lstPrecoProduto;
            }

            foreach (var item in xml.ProdutosItens)
            {
                PrecoProduto precoProduto = new PrecoProduto(this.Organizacao, this.IsOffline);

                if (!String.IsNullOrEmpty(item.CodigoProduto))
                {
                    precoProduto.CodigoProduto = item.CodigoProduto;

                    Product produto = new Product(this.Organizacao, this.IsOffline);
                    produto = lstProduto.Where(x => x.Codigo == item.CodigoProduto).FirstOrDefault<Product>();
                    if (produto != null)
                    {
                        precoProduto.ProdutoId = produto.ID.Value;
                        precoProduto.Produto = produto;

                        if (item.TipoPortfolio == (int)Domain.Enum.Portfolio.Tipo.CrossSelling)
                            precoProduto.TipoCrossSelling = true;
                        else
                            precoProduto.TipoCrossSelling = false;

                        precoProduto.codEstabelecimento = item.CodigoEstabelecimento.Value;
                        precoProduto.codUnidade = item.CodigoUnidadeNegocio;
                        precoProduto.codFamiliaComl = item.CodigoFamiliaComercial;
                        precoProduto.tipoPortofolio = item.TipoPortfolio.Value;

                    }
                    else
                    {
                        precoProduto.Produto = null;
                        precoProduto.ProdutoId = Guid.Empty;
                        precoProduto.ValorProduto = 0;
                        precoProduto.MensagemErro = "Produto não cadastrado no Crm.";

                    }
                }
                else
                {
                    precoProduto.Produto = null;
                    precoProduto.ProdutoId = Guid.Empty;
                    precoProduto.ValorProduto = 0;
                    precoProduto.MensagemErro = "Identificador do Produto não enviado.";

                }

                if (!String.IsNullOrEmpty(item.Moeda))
                {
                    precoProduto.Moeda = item.Moeda;
                }
                else
                {
                    precoProduto.Produto = null;
                    precoProduto.ProdutoId = Guid.Empty;
                    precoProduto.ValorProduto = 0;
                    precoProduto.MensagemErro = "Moeda não enviada.";

                }

                precoProduto.ContaId = contaProduto;
                decimal qtd = 0;

                if (Decimal.TryParse(item.Quantidade.ToString(), out qtd))
                {
                    try
                    {
                        precoProduto.Quantidade = (int)qtd;
                    }
                    catch
                    {
                        precoProduto.Produto = null;
                        precoProduto.ProdutoId = Guid.Empty;
                        precoProduto.ValorProduto = 0;
                        precoProduto.MensagemErro = "Produto com 'Quantidade' fora do padrão.";
                    }

                }
                else
                {
                    precoProduto.Produto = null;
                    precoProduto.ProdutoId = Guid.Empty;
                    precoProduto.ValorProduto = 0;
                    precoProduto.MensagemErro = "Produto com 'Quantidade' fora do padrão ou não enviado.";

                }
                precoProduto.ValorProduto = 0;
                lstPrecoProduto.Add(precoProduto);
            }
            return lstPrecoProduto;
        }

        public PrecoProduto DefinirPropriedades(Intelbras.Message.Helper.MSG0101 xml)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Métodos Auxiliares


        private List<Pollux.Entities.ProdutoValorItem> GerarListaProdutoValorItem(List<PrecoProduto> lstPrecoProduto)
        {
            List<Pollux.Entities.ProdutoValorItem> lstPrecoValorItem = new List<Pollux.Entities.ProdutoValorItem>();

            foreach (var item in lstPrecoProduto)
            {
                Pollux.Entities.ProdutoValorItem prodValorItem = new Pollux.Entities.ProdutoValorItem();

                prodValorItem.CodigoProduto = item.CodigoProduto;
                prodValorItem.ValorProduto = item.ValorProduto;
                if (item.NomePoliticaComercial != null && !String.IsNullOrEmpty(item.NomePoliticaComercial))
                {
                    prodValorItem.NomePoliticaComercial = item.NomePoliticaComercial;
                }
                else if (item.TipoCrossSelling)
                {
                    prodValorItem.NomePoliticaComercial = "Cross Selling.";
                }
                else
                {
                    prodValorItem.NomePoliticaComercial = "Política Comercial não aplicada.";
                }

                if (item.CustoProduto.HasValue)//(item.Produto != null && item.Produto.CustoAtual != null && item.Produto.CustoAtual.HasValue)
                    prodValorItem.PrecoBase = item.CustoProduto;
                else
                    prodValorItem.PrecoBase = 0;

                prodValorItem.TemCache = false;

                if (item.QuantidadeFinal.HasValue)
                    prodValorItem.QuantidadeMaxima = item.QuantidadeFinal;
                else
                    prodValorItem.QuantidadeMaxima = 999999999;

                var itemConfiguracaoBenef = RepositoryService.ConfiguracaoBeneficio.ObterPorProduto((Guid)item.Produto.ID);

                if (itemConfiguracaoBenef != null)
                {
                    prodValorItem.RebateAntecipado = itemConfiguracaoBenef.AnteciparRebate;

                    if (itemConfiguracaoBenef.PercRebateAntecipado != null)
                    {
                        prodValorItem.PercentualRebateAntecipado = itemConfiguracaoBenef.PercRebateAntecipado / 100;
                    }
                    else
                    {
                        prodValorItem.PercentualRebateAntecipado = 0;
                    }

                    if (itemConfiguracaoBenef.CalcularRebate.HasValue)
                    {
                        prodValorItem.CalcularRebate = itemConfiguracaoBenef.CalcularRebate;
                    }
                    else
                    {
                        prodValorItem.CalcularRebate = true;
                    }
                }
                else
                {
                    prodValorItem.RebateAntecipado = false;
                    prodValorItem.PercentualRebateAntecipado = 0;
                    prodValorItem.CalcularRebate = true;
                }


                if (item.ValorComDesconto == 0)
                {
                    prodValorItem.ValorComDesconto = item.ValorProduto;
                }
                else
                {
                    prodValorItem.ValorComDesconto = item.ValorComDesconto;
                }

                prodValorItem.PossivelDescontoMaisVerde = item.PercentualPossivelDesconto;
                prodValorItem.PercentualDescontoVerde = item.PercentualDesconto;
                prodValorItem.DataValidade = item.DataValidade;

                if (item.PrecoAlterado.HasValue)
                {
                    prodValorItem.PrecoAlterado = item.PrecoAlterado;
                }
                else
                {
                    prodValorItem.PrecoAlterado = false;
                }

                lstPrecoValorItem.Add(prodValorItem);
            }

            return lstPrecoValorItem;
        }

        #endregion

        public string Enviar(PrecoProduto objModel)
        {
            throw new NotImplementedException();
        }


    }
}
