using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Util.CustomException;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0176 : Base
    {
        public MSG0176(string org, bool isOffline)
            : base(org, isOffline) { }

        public List<PosicaoEntregaViewModel> Enviar(Fatura faturaObj, Estabelecimento estabObj, Conta clienteObj)
        {
            string identidadeEmissor = Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM);

            var numeroNotaFiscal = faturaObj.NumeroNF.Remove(0, 1);
            var numeroOperString = numeroNotaFiscal + " - " + faturaObj.Serie;

            var msg0176 = new Pollux.MSG0176(identidadeEmissor, numeroOperString)
            {
                NumeroNotaFiscal = numeroNotaFiscal,
                NumeroSerie = faturaObj.Serie,
                CodigoEstabelecimento = estabObj.Codigo,
                CodigoCliente = Int32.Parse(clienteObj.CodigoMatriz)
            };

            string resposta;
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(msg0176.GenerateMessage(true), "1", "1", out resposta))
            {
                var msg0176r1 = CarregarMensagem<Pollux.MSG0176R1>(resposta);

                if (!msg0176r1.Resultado.Sucesso)
                {
                    throw new BarramentoException(msg0176r1.Resultado.Mensagem, msg0176r1.Resultado.CodigoErro);
                }

                return InstanciarValidarObjeto(msg0176r1);
            }
            else
            {
                var erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
        }

        private List<PosicaoEntregaViewModel> InstanciarValidarObjeto(Pollux.MSG0176R1 message)
        {
            var lista = new List<PosicaoEntregaViewModel>();

            if (message.PosicaoEntregaItens != null)
            {
                foreach (var item in message.PosicaoEntregaItens)
                {
                    lista.Add(new PosicaoEntregaViewModel()
                    {
                        DescricaoMovimentacao = item.DescricaoMovimentacao,
                        DataHoraMovimentacao =  (DateTime) item.DataHoraMovimentacao
                    });
                }
            }

            return lista;
        }
    }
}