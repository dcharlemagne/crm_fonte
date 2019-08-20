using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ExtratoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ExtratoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ExtratoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public string GeraExtratoASTEC(DateTime data)
        {
            string retorno = "Extrato;OcorrenciaId;AvulsoId;ValorServico;%NL%";

            //Lista as contas que estiverem para gerar extrato na data passada
            var contas = RepositoryService.Conta.ListarContasAstec(data);
            if (contas.Count == 0)
                return "";

            foreach (var conta in contas)
            {
                var ocorrencias = RepositoryService.Ocorrencia.ListarPorAutorizada(conta.Id); //Lista as ocorrências para gerar o extrato
                var listaLancamentoAvulsoDoExtrato = RepositoryService.LancamentoAvulsoDoExtrato.ListarSemExtratoPor(conta); //Lista todos os itens do lançamento avulso

                string numeroExtrato = conta.CodigoMatriz + string.Format("{0:yyyyMMdd}", data);
                Extrato extrato = RepositoryService.Extrato.ObterPor(numeroExtrato);

                if ((ocorrencias.Count > 0 || listaLancamentoAvulsoDoExtrato.Count > 0) && extrato == null)
                {
                    extrato = new Extrato(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        Numero = numeroExtrato,
                        Nome = numeroExtrato,
                        AutorizadaId = new Lookup(conta.Id, "account"),
                        Autorizada = conta
                    };
                    extrato.Id = RepositoryService.Extrato.Create(extrato);
                }

                foreach (Ocorrencia item in ocorrencias)
                {
                    Ocorrencia ocorrenciaTemp = new Ocorrencia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    ocorrenciaTemp.Id = item.Id;
                    ocorrenciaTemp.ReferenciaExtratoPagamentoId = new Lookup(extrato.Id, "new_extrato_pagamento_ocorrencia");
                    if (item.RazaoStatus == (int)StatusDaOcorrencia.Reprovada) ocorrenciaTemp.ValorServico = 0;
                    SolucaoOcorrencia solucaoOcorrencia = new SolucaoOcorrencia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        DataHoraConclusao = DateTime.Now,
                        Nome = "Rotina de Extrato de pagamento",
                        OcorrenciaId = item.Id
                    };

                    try
                    {
                        retorno += extrato.Numero + ";" + item.Id.ToString() + ";-;" + item.ValorServico.ToString() + ";%NL%";
                        RepositoryService.Ocorrencia.Update(ocorrenciaTemp);
                        RepositoryService.Ocorrencia.FecharOcorrencia(item, solucaoOcorrencia);
                    }
                    catch (Exception ex) { }

                }

                foreach (LancamentoAvulsoDoExtrato itemLancamentoAvulsoDoExtrato in listaLancamentoAvulsoDoExtrato)
                {
                    itemLancamentoAvulsoDoExtrato.ExtratoId = new Lookup(extrato.Id, "new_extrato_pagamento_ocorrencia");
                    RepositoryService.LancamentoAvulsoDoExtrato.Update(itemLancamentoAvulsoDoExtrato);
                    retorno += extrato.Numero + ";-;" + itemLancamentoAvulsoDoExtrato.Id.ToString() + ";" + itemLancamentoAvulsoDoExtrato.Valor.ToString() + ";%NL%";
                }

                if (extrato != null)
                {
                    extrato.RazaoStatus = 1; //Aguardando Análise Intelbras
                    extrato.AtualizarValor();
                    RepositoryService.Extrato.Update(extrato);
                    retorno += extrato.Numero + ";ValorTotal;-;" + extrato.ValorTotal.ToString() + ";%NL%";
                }

            }
            return retorno;
        }        
    }
}