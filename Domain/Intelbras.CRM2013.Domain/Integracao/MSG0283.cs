using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0283 : Base, IBase<Message.Helper.MSG0283, Domain.Model.PagamentoServico>
    {
        #region Construtor

        public MSG0283(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


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

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0283>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0283R1>(numeroMensagem, retorno);
            }

            objeto = new Domain.Servicos.PagamentoServicoService(this.Organizacao, this.IsOffline).Persistir(objeto); ;

            if (objeto != null)
            {
                resultadoPersistencia.Sucesso = true;
                retorno.Add("CodigoPagamento", objeto.ID.Value.ToString());
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Ocorreu problema na atualização de Pagamento Serviço.";
            }

            return CriarMensagemRetorno<Pollux.MSG0283R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public PagamentoServico DefinirPropriedades(Intelbras.Message.Helper.MSG0283 xml)
        {
            var crm = new Model.PagamentoServico(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            crm.IntegrarNoPlugin = true;
            if (!String.IsNullOrEmpty(xml.CodigoPagamento))
            {
                PagamentoServico pagamentoServico = new Servicos.PagamentoServicoService(this.Organizacao, this.IsOffline).BuscaPagamentoServico(new Guid(xml.CodigoPagamento));
                if (pagamentoServico != null)
                {
                    crm.Id = new Guid(xml.CodigoPagamento);
                    crm.ID = new Guid(xml.CodigoPagamento);
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoPagamento informado não existe para ser atualizado.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.CodigoOcorrencia))
            {
                Ocorrencia ocorrencia = new Intelbras.CRM2013.Domain.Servicos.OcorrenciaService(this.Organizacao, this.IsOffline).BuscaOcorrencia(new Guid(xml.CodigoOcorrencia));
                if (ocorrencia != null)
                {
                    crm.OcorrenciaId = new Lookup(ocorrencia.ID.Value, "incident");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoOcorrencia não encontrado no Crm.";
                    return crm;
                }
            }

            if (!string.IsNullOrEmpty(xml.CodigoTipoPagamentoServico))
            {
                TipoPagamento tipoPagamento = new Intelbras.CRM2013.Domain.Servicos.TipoPagamentoService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoTipoPagamentoServico));
                if (tipoPagamento != null)
                {
                    crm.TipoPagamentoId = new Lookup(tipoPagamento.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoTipoPagamentoServico não encontrado no Crm.";
                    return crm;
                }
            }

            if (xml.Quantidade.HasValue)
            {
                crm.Quantidade = xml.Quantidade;
            }

            if (xml.ValorTotal.HasValue)
            {
                crm.Valor = xml.ValorTotal;
            }

            if (!string.IsNullOrEmpty(xml.NumeroNotaFiscal))
            {
                crm.NumeroNotaFiscal = xml.NumeroNotaFiscal;
            }

            if (xml.DataPagamento.HasValue)
            {
                crm.DataPagamento = xml.DataPagamento;
            }

            if (!string.IsNullOrEmpty(xml.Observacao))
            {
                crm.Observacao = xml.Observacao;
            }

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0283 DefinirPropriedades(PagamentoServico crm)
        {
            Intelbras.Message.Helper.MSG0283 xml = new Pollux.MSG0283(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.ID.ToString(), 40));

            xml.CodigoPagamento = crm.ID.ToString();
            xml.CodigoTipoPagamentoServico = crm.TipoPagamentoId.Id.ToString();
            xml.CodigoOcorrencia = crm.OcorrenciaId.Id.ToString();
            if (crm.Quantidade.HasValue)
            {
                xml.Quantidade = crm.Quantidade;
            }
            if (crm.Valor.HasValue)
            {
                xml.ValorTotal = crm.Valor;
            }
            if (!string.IsNullOrEmpty(crm.NumeroNotaFiscal))
            {
                xml.NumeroNotaFiscal = crm.NumeroNotaFiscal;
            }

            if (crm.DataPagamento.HasValue)
            {
                xml.DataPagamento = crm.DataPagamento;
            }

            if (!string.IsNullOrEmpty(crm.Observacao))
            {
                xml.Observacao = crm.Observacao;
            }

            return xml;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(PagamentoServico objModel)
        {
            string resposta;

            Intelbras.Message.Helper.MSG0283 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0283R1 retorno = CarregarMensagem<Pollux.MSG0283R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return resposta;
        }

        #endregion
    }
}
