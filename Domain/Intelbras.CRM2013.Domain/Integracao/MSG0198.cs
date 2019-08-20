using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Util.CustomException;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0198 : Base
    {
        public MSG0198(string org, bool isOffline)
            : base(org, isOffline) { }

        /// <summary>
        /// sss
        /// </summary>
        /// <param name="numeroserieproduto"></param>
        /// <returns></returns>
        public SerieDoProduto Enviar(String numeroserieproduto)
        {
            string identidadeEmisso = Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM);

            var msg0198 = new Pollux.MSG0198(identidadeEmisso, numeroserieproduto)
            {
                NumeroSerieProduto = numeroserieproduto
            };

            string resposta;
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(msg0198.GenerateMessage(true), "1", "1", out resposta))
            {
                var msg0198r1 = CarregarMensagem<Pollux.MSG0198R1>(resposta);

                if (!msg0198r1.Resultado.Sucesso)
                {
                    throw new BarramentoException(msg0198r1.Resultado.Mensagem, msg0198r1.Resultado.CodigoErro);
                }

                return InstanciarValidarObjeto(msg0198r1);
            }
            else
            {
                var erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
        }

        private SerieDoProduto InstanciarValidarObjeto(Pollux.MSG0198R1 message)
        {
            if (message.Produto != null)
            {
                if (!string.IsNullOrEmpty(message.Produto.CodigoProduto))
                {
                    SerieDoProduto serieDoProduto = new SerieDoProduto(this.Organizacao, this.IsOffline);
                    serieDoProduto.Produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(message.Produto.CodigoProduto);

                    //serieDoProduto.Produto = new Domain.Model.Product(this.Organizacao, this.IsOffline);

                    serieDoProduto.Produto.Codigo = message.Produto.CodigoProduto;
                    serieDoProduto.Produto.Nome = message.Produto.Nome;
                    serieDoProduto.DataFabricacaoProduto = message.Produto.DataFabricacao;
                    serieDoProduto.NotaFiscal = new Domain.Model.Fatura(this.Organizacao, this.IsOffline);
                    serieDoProduto.NotaFiscal.IDFatura = message.Produto.NumeroNotaFiscal;
                    serieDoProduto.NotaFiscal.DataEmissao = message.Produto.DataEmissao;
                    serieDoProduto.NotaFiscal.Cliente = new Conta();
                    serieDoProduto.NotaFiscal.Cliente.Nome = message.Produto.NomeRazaoSocial;
                    serieDoProduto.NumeroPedido = message.Produto.NumeroPedido;

                    return serieDoProduto;
                }
            }
            return null;
        }
    }
}