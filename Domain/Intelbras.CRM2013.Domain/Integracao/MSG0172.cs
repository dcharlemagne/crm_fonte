using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Util.CustomException;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0172 : Base
    {
        public MSG0172(string org, bool isOffline)
            : base(org, isOffline) { }

        /// <summary>
        /// sss
        /// </summary>
        /// <param name="solicitacaoId"></param>
        /// <returns></returns>
        public List<TituloSolicitacaoViewModel> Enviar(Guid solicitacaoId)
        {
            string identidadeEmisso = Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM);

            var msg0172 = new Pollux.MSG0172(identidadeEmisso, solicitacaoId.ToString())
            {
                CodigoSolicitacaoBeneficio = solicitacaoId.ToString()
            };

            string resposta;
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(msg0172.GenerateMessage(true), "1", "1", out resposta))
            {
                var msg0172r1 = CarregarMensagem<Pollux.MSG0172R1>(resposta);

                if (!msg0172r1.Resultado.Sucesso)
                {
                    throw new BarramentoException(msg0172r1.Resultado.Mensagem, msg0172r1.Resultado.CodigoErro);
                }

                return InstanciarValidarObjeto(msg0172r1);
            }
            else
            {
                var erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
        }

        private List<TituloSolicitacaoViewModel> InstanciarValidarObjeto(Pollux.MSG0172R1 message)
        {
            var lista = new List<TituloSolicitacaoViewModel>();

            if (message.TituloSolicitacao != null)
            {
                foreach (var item in message.TituloSolicitacao)
                {
                    lista.Add(new TituloSolicitacaoViewModel()
                    {
                        CNPJEstabelecimento = item.CNPJEstabelecimento,
                        CodigoCliente = item.CodigoCliente,
                        CodigoConta = item.CodigoConta,
                        CodigoEstabelecimento = item.CodigoEstabelecimento,
                        DataVencimento = item.DataVencimento,
                        NomeConta = item.NomeConta,
                        NumeroParcela = item.NumeroParcela,
                        NumeroSerie = item.NumeroSerie,
                        NumeroTitulo = item.NumeroTitulo,
                        SaldoTitulo = item.SaldoTitulo,
                        ValorAbatido = item.ValorAbatido,
                        ValorOriginal = item.ValorOriginal
                    });
                }
            }

            return lista;
        }
    }
}