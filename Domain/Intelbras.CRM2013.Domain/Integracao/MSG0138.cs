using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0138 : Base
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Int32 codigoEstabelecimento;

        #endregion

        #region Construtor
        public MSG0138(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
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
            resultadoPersistencia.Mensagem = "Intergação não permitida!";
            resultadoPersistencia.Sucesso = false;
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0139R1>(numeroMensagem, retorno);
        }

        #endregion

        public Intelbras.Message.Helper.MSG0138 DefinirPropriedades(List<Product> lstProd, List<ProdutosdaSolicitacao> prodSolicLst, List<PrecoProduto> precoProdutoLst, SolicitacaoBeneficio solicBenef)
        {
            Intelbras.Message.Helper.MSG0138 msg0138 = new Intelbras.Message.Helper.MSG0138(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), solicBenef.ID.ToString());
            var estabService = new EstabelecimentoService(this.Organizacao, this.IsOffline);

            var parametroGlobalCondPagamento = new ParametroGlobalService(this.Organizacao, this.IsOffline).ObterPor((int)Enum.TipoParametroGlobal.CondicaoPagamentoTabelaPreco);

            if (parametroGlobalCondPagamento == null)
            {
                throw new ArgumentException("(CRM) Parâmetro global de Condição de Pagamento não localizado [Código: " + (int)Enum.TipoParametroGlobal.CondicaoPagamentoTabelaPreco + "]");
            }

            int codigoCondicaoPagamento;
            if (!int.TryParse(parametroGlobalCondPagamento.Valor, out codigoCondicaoPagamento))
            {
                throw new ArgumentException("(CRM) Parâmetro global de Condição de Pagamento não é um número inteiro [Código: " + (int)Enum.TipoParametroGlobal.CondicaoPagamentoTabelaPreco + "]");
            }

            msg0138.CondicaoPagamento = codigoCondicaoPagamento;
            msg0138.Conta = solicBenef.Canal.Id.ToString();

            msg0138.ProdutosItens = new List<Pollux.Entities.ProdutoValorIcmsItem>();
            foreach (var prodSolicObj in prodSolicLst)
            {
                var estabObj = estabService.BuscaEstabelecimento(prodSolicObj.Estabelecimento.Id);
                var prodObj = lstProd.Find(x => x.ID == prodSolicObj.Produto.Id);
                var precoProdObj = precoProdutoLst.Find(x => x.Produto.ID == prodSolicObj.Produto.Id);

                var tmpPolluxObj = new Pollux.Entities.ProdutoValorIcmsItem();
                tmpPolluxObj.Estabelecimento = estabObj.Codigo;
                tmpPolluxObj.CodigoProduto = prodObj.Codigo;
                tmpPolluxObj.PrecoUnitario = precoProdObj.ValorBase;

                msg0138.ProdutosItens.Add(tmpPolluxObj);
            }

            return msg0138;
        }
        #region Métodos Auxiliares

        public List<ValorProdutoICMSViewModel> Enviar(List<Product> objModel, List<ProdutosdaSolicitacao> prodSolicLst, List<PrecoProduto> precoProdutoLst, SolicitacaoBeneficio solicBenef)
        {
            List<ValorProdutoICMSViewModel> valProdICMSLst = null;
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0138 mensagem = this.DefinirPropriedades(objModel, prodSolicLst, precoProdutoLst, solicBenef);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0138R1 retorno = CarregarMensagem<Pollux.MSG0138R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
                else
                {
                    valProdICMSLst = new List<ValorProdutoICMSViewModel>();
                    foreach (var retornoObj in retorno.ProdutosItens)
                    {
                        var valProd = new ValorProdutoICMSViewModel();
                        valProd.AliquotaICMS = retornoObj.AliquotaICMS;
                        valProd.PrecoLiquido = retornoObj.PrecoLiquido;
                        valProd.CodigoProduto = retornoObj.CodigoProduto;

                        valProdICMSLst.Add(valProd);
                    }
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return valProdICMSLst;
        }

        #endregion
    }
}
