using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ValueObjects;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PedidoService
    {
        #region Construtores

        private static FileStream ostrm;
        private static StreamWriter writer;
        private static TextWriter oldOut;

        private RepositoryService RepositoryService { get; set; }

        public PedidoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public PedidoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos Pedido


        public void Persistir(Domain.Model.Pedido objPedido)
        {
            if (objPedido.ID.HasValue)
                RepositoryService.Pedido.Update(objPedido);
        }

        public Domain.Model.Pedido Persistir(Domain.Model.Pedido objPedido, ref bool alteraStatus, bool fecharPedido)
        {
            Domain.Model.Pedido tmpPedido = null;
            if (objPedido.ID.HasValue)
            {
                tmpPedido = RepositoryService.Pedido.ObterPor(objPedido.ID.Value);
            }

            if (tmpPedido == null && !String.IsNullOrEmpty(objPedido.IDPedido))
            {
                tmpPedido = RepositoryService.Pedido.ObterPor(objPedido.IDPedido);
            }
            if (tmpPedido != null)
            {

                //Cancelada = 2,
                //Cumprido = 3,


                int? stateAltera = objPedido.Status;
                int? razaoAltera = objPedido.RazaoStatus;

                //Alterado para permitir reabrir pedidos já cumpridos ou cancelados
                if (tmpPedido.Status == (int?)Enum.Pedido.StateCode.Cumprido
                    || tmpPedido.Status == (int?)Enum.Pedido.StateCode.Cancelada)
                {
                    objPedido.Status = (int)Enum.Pedido.StateCode.Ativa;
                    objPedido.RazaoStatus = (int?)Enum.Pedido.RazaoStatus.Aberto;
                    this.MudarStatusPedido(tmpPedido.ID.Value, (int)Enum.Pedido.StateCode.Ativa, (int)Enum.Pedido.RazaoStatus.Aberto);
                }
                else
                {
                    objPedido.Status = tmpPedido.Status;
                    objPedido.RazaoStatus = tmpPedido.RazaoStatus;
                }

                objPedido.ID = tmpPedido.ID;
                RepositoryService.Pedido.Update(objPedido);
                //Altera Status - Se necessário
                objPedido.Status = stateAltera;
                objPedido.RazaoStatus = razaoAltera;
                if (!tmpPedido.Status.Equals(objPedido.Status)
                    || !tmpPedido.RazaoStatus.Equals(objPedido.RazaoStatus))
                    alteraStatus = true;

                return objPedido;
            }
            else
            {
                if (fecharPedido)
                {
                    objPedido.RazaoStatus = (int?)Enum.Pedido.RazaoStatus.Aberto;
                    objPedido.Status = (int?)Enum.Pedido.StateCode.Ativa;
                    alteraStatus = true;
                    objPedido.ID = RepositoryService.Pedido.Create(objPedido);
                    return objPedido;
                }

                if (objPedido.RazaoStatus == (int?)Enum.Pedido.RazaoStatus.Cancelado)
                {
                    //Altera para ativo para inserir
                    int? stateAltera = objPedido.Status;
                    int? razaoAltera = objPedido.RazaoStatus;
                    objPedido.RazaoStatus = (int?)Enum.Pedido.RazaoStatus.Aberto;
                    objPedido.Status = (int?)Enum.Pedido.StateCode.Ativa;

                    //Insere
                    objPedido.ID = RepositoryService.Pedido.Create(objPedido);
                    //Retorna os valores iniciais
                    objPedido.Status = stateAltera;
                    objPedido.RazaoStatus = razaoAltera;
                    alteraStatus = true;
                    return objPedido;
                }
                objPedido.ID = RepositoryService.Pedido.Create(objPedido);
                return objPedido;
            }
        }

        public Domain.Model.Pedido BuscaPedido(Guid itbc_Pedidoid)
        {
            Domain.Model.Pedido pedido = RepositoryService.Pedido.ObterPor(itbc_Pedidoid);
            if (pedido != null)
                return pedido;
            return null;
        }

        public Domain.Model.Pedido BuscaPedidoEMS(String pedidoEMS, params string[] columns)
        {
            return RepositoryService.Pedido.ObterPor(pedidoEMS, columns);
        }

        public bool MudarStatusPedido(Guid id, int stateCode, int statusCode)
        {
            return RepositoryService.Pedido.AlterarStatus(id, stateCode, statusCode);
        }

        public bool FecharPedido(Guid id)
        {
            return RepositoryService.Pedido.FecharPedido(id);
        }

        private Diagnostico InstanciarDiagnostico(XmlElement xmlItem)
        {
            XmlNode xmlNotaFiscal = xmlItem.ParentNode;
            Diagnostico diagnostico = null;

            try { diagnostico = RepositoryService.Diagnostico.Retrieve(new Guid(xmlItem.SelectSingleNode("guid-os").InnerText)); }
            catch { throw new ArgumentException("Id do diagnostico não contem dados ou o  formato não esta correto!"); }

            if (diagnostico == null)
            {
                throw new ArgumentException("O Diagnóstico (" + xmlItem.SelectSingleNode("guid-os").InnerText + ") não foi encontrado!");
            }
            else if (diagnostico.RazaoStatus.Value == (int)StatusDoDiagnostico.Cancelado)
            {
                throw new ArgumentException("O Status do Diagnostico (" + xmlItem.SelectSingleNode("guid-os").InnerText + ") está cancelado!");
            }
            if (diagnostico.OcorrenciaId == null)
            {
                throw new ArgumentException("O Diagnóstico (" + xmlItem.SelectSingleNode("guid-os").InnerText + ") não possui ocorrência associada!");
            }

            if (xmlItem.SelectSingleNode("it-substituto") != null && !string.IsNullOrEmpty(xmlItem.SelectSingleNode("it-substituto").InnerText))
            {
                diagnostico.ProdutoSubstituto = (new Domain.Servicos.RepositoryService()).Produto.ObterPorNumero(xmlItem.SelectSingleNode("it-codigo").InnerText);
                if (diagnostico.ProdutoSubstituto == null)
                    throw new ArgumentException("Produto substituto não contrado");
                diagnostico.ProdutoSubstitutoId = new Lookup(diagnostico.ProdutoSubstituto.Id, "product");
            }

            #region Estabelecimento
            if (xmlNotaFiscal.Attributes["estabelecimento"] == null || string.IsNullOrEmpty(xmlNotaFiscal.Attributes["estabelecimento"].InnerText))
                throw new ArgumentException("O Estabelecimento não foi informado!");

            Estabelecimento estabelecimento = null;
            try
            {
                estabelecimento = (new Domain.Servicos.RepositoryService()).Estabelecimento.ObterPor(Convert.ToInt32(xmlNotaFiscal.Attributes["estabelecimento"].InnerText));
            }
            catch (Exception ex)
            {
                //LogHelper.Process(ex, ClassificacaoLog.ServicoASTEC);
                throw new ArgumentException("O erro ao localizar o Estabelecimento!");
            }

            if (estabelecimento == null)
                throw new ArgumentException("O Estabelecimento não encontrado");

            diagnostico.EstabelecimentoId = new Lookup(estabelecimento.Id, "itbc_estabelecimento");
            #endregion

            diagnostico.NumeroNotaFiscal = xmlNotaFiscal.Attributes["numero"].InnerText;
            diagnostico.SerieNotaFiscal = xmlNotaFiscal.Attributes["serie"].InnerText;
            diagnostico.DataFaturamentoERP = Convert.ToDateTime(xmlNotaFiscal.Attributes["dtEmissao"].InnerText);
            diagnostico.QuantidadeFaturada = Convert.ToInt32(Convert.ToDecimal(xmlItem.SelectSingleNode("qt-faturada").InnerText.Replace(".", ",")));
            diagnostico.ValorUnitario = Convert.ToDecimal(xmlItem.SelectSingleNode("vl-preuni").InnerText.Replace(".", ","));
            diagnostico.AliquotaIPI = Convert.ToDecimal(xmlItem.SelectSingleNode("aliquota-ipi").InnerText.Replace(".", ","));
            diagnostico.ValorIPI = Convert.ToDecimal(xmlItem.SelectSingleNode("vl-ipi-it").InnerText.Replace(".", ","));
            diagnostico.ValorICMS = Convert.ToDecimal(xmlItem.SelectSingleNode("vl-icms-it").InnerText.Replace(".", ","));
            diagnostico.ValorBaseICMS = Convert.ToDecimal(xmlItem.SelectSingleNode("vl-bicms-it").InnerText.Replace(".", ","));
            diagnostico.NumeroRastreamento = xmlNotaFiscal.Attributes["numeroConhecimento"].InnerText;
            diagnostico.NumeroDoPedido = xmlItem.SelectSingleNode("it-codigo").InnerText;

            return diagnostico;
        }

        public List<string> EfetuaFaturamento(XmlDocument doc)
        {
            List<string> retorno = new List<string>();

            foreach (XmlElement xmlItem in doc.SelectNodes("/nota-fiscal/item"))
                try
                {
                    List<Diagnostico> diagnosticosParaAtualizar = new List<Diagnostico>();
                    Diagnostico diagnosticoFaturado = this.InstanciarDiagnostico(xmlItem);
                    Diagnostico diagnosticoOriginal = null;

                    try
                    {
                        diagnosticoOriginal = (new Domain.Servicos.RepositoryService()).Diagnostico.Retrieve(diagnosticoFaturado.Id);
                    }
                    catch
                    {
                        throw new ArgumentException("O Diagnostico(" + diagnosticoFaturado.Id + ") não foi encontrado!");
                    }

                    if (diagnosticoOriginal == null)
                        throw new ArgumentException("O Diagnostico(" + diagnosticoFaturado.Id + ") não foi encontrado!");

                    if (diagnosticoOriginal.RazaoStatus.Value == (int)StatusDoDiagnostico.Cancelado)
                        throw new ArgumentException("O Status do Diagnostico(" + diagnosticoFaturado.Id + ") está cancelado!");

                    TipoIntegracaoNotaFiscal tipo = this.VerificaCondicaoDoFaturamento(diagnosticoFaturado, diagnosticoOriginal);

                    switch (tipo)
                    {
                        case TipoIntegracaoNotaFiscal.NotaFiscalJaExistente:
                            if (diagnosticoOriginal.RazaoStatus == (int)StatusDoDiagnostico.PedidoSolicitadoAoEms)
                                diagnosticosParaAtualizar.Add(diagnosticoFaturado);
                            break;

                        case TipoIntegracaoNotaFiscal.FaturamentoTotal:
                            diagnosticosParaAtualizar.AddRange(this.AtualizarDiagnosticosParcial(ref diagnosticoFaturado, diagnosticoOriginal));
                            break;

                        case TipoIntegracaoNotaFiscal.FaturamentoParcial:
                            if (!string.IsNullOrEmpty(diagnosticoOriginal.NumeroNotaFiscal) && !string.IsNullOrEmpty(diagnosticoOriginal.SerieNotaFiscal))
                            {
                                this.EfetuaParcialESubstituto(ref diagnosticoFaturado, diagnosticoOriginal, tipo);
                            }
                            diagnosticosParaAtualizar.AddRange(this.AtualizarDiagnosticosParcial(ref diagnosticoFaturado, diagnosticoOriginal));
                            break;

                        case TipoIntegracaoNotaFiscal.ProdutoSubstituto:
                        case TipoIntegracaoNotaFiscal.ProdutoSubstitutoEParcial:
                            if (diagnosticoOriginal.RazaoStatus != (int)StatusDoDiagnostico.Substituido)
                            {
                                this.EfetuaParcialESubstituto(ref diagnosticoFaturado, diagnosticoOriginal, tipo);
                                diagnosticosParaAtualizar.AddRange(this.AtualizarDiagnosticosParcial(ref diagnosticoFaturado, diagnosticoOriginal));
                            }
                            break;
                    }

                    this.EfetuarAtualizacaoDiagnosticoNotaFiscal(diagnosticosParaAtualizar, tipo, doc);

                }
                catch (Exception ex)
                {
                    var mensagem = (string.Format("<strong>{0}</strong><br />{1}", ex.Message, ex.StackTrace));
                    retorno.Add(mensagem);
                }

            return retorno;
        }

        #endregion

        #region ProdutoPedido

        public void Excluir(Model.ProdutoPedido objProdutoPedido)
        {
            Model.ProdutoPedido tmpProdutoPedido = null;
            if (!String.IsNullOrEmpty(objProdutoPedido.ChaveIntegracao))
            {
                tmpProdutoPedido = RepositoryService.ProdutoPedido.ObterPorChaveIntegracao(objProdutoPedido.ChaveIntegracao);

                if (tmpProdutoPedido != null)
                {
                    RepositoryService.ProdutoPedido.Delete(tmpProdutoPedido.ID.Value);
                }
            }
        }

        public Model.ProdutoPedido Persistir(Model.ProdutoPedido objProdutoPedido)
        {
            Model.ProdutoPedido tmpProdutoPedido = null;
            if (!String.IsNullOrEmpty(objProdutoPedido.ChaveIntegracao))
            {
                tmpProdutoPedido = RepositoryService.ProdutoPedido.ObterPorChaveIntegracao(objProdutoPedido.ChaveIntegracao);

                if (tmpProdutoPedido != null)
                {
                    objProdutoPedido.ID = tmpProdutoPedido.ID;
                    RepositoryService.ProdutoPedido.Update(objProdutoPedido);
                    return tmpProdutoPedido;
                }
                else
                    objProdutoPedido.ID = RepositoryService.ProdutoPedido.Create(objProdutoPedido);
                return objProdutoPedido;
            }
            else
            {
                objProdutoPedido.ID = RepositoryService.ProdutoPedido.Create(objProdutoPedido);
                return objProdutoPedido;
            }
        }

        public Model.ProdutoPedido BuscaProdutoPedido(Guid itbc_ProdutoPedidoid)
        {
            Model.ProdutoPedido ProdutoPedido = RepositoryService.ProdutoPedido.ObterPor(itbc_ProdutoPedidoid);
            if (ProdutoPedido != null)
                return ProdutoPedido;
            return null;
        }

        private TipoIntegracaoNotaFiscal VerificaCondicaoDoFaturamento(Diagnostico diagnosticoFaturado, Diagnostico diagnosticoOriginal)
        {
            if (diagnosticoFaturado.NumeroNotaFiscal == diagnosticoOriginal.NumeroNotaFiscal
                && diagnosticoFaturado.SerieNotaFiscal == diagnosticoOriginal.SerieNotaFiscal
                && diagnosticoFaturado.EstabelecimentoId == diagnosticoOriginal.EstabelecimentoId)
                return TipoIntegracaoNotaFiscal.NotaFiscalJaExistente;

            bool faturamentoParcial = (diagnosticoOriginal.QuantidadeSolicitada > diagnosticoFaturado.QuantidadeFaturada);
            bool produtoSubstituto = (diagnosticoFaturado.ProdutoSubstituto != null);


            if (produtoSubstituto)
            {
                if (faturamentoParcial)
                    return TipoIntegracaoNotaFiscal.ProdutoSubstitutoEParcial;

                return TipoIntegracaoNotaFiscal.ProdutoSubstituto;
            }

            if (faturamentoParcial)
            {
                return TipoIntegracaoNotaFiscal.FaturamentoParcial;
            }

            return TipoIntegracaoNotaFiscal.FaturamentoTotal;
        }

        private List<Diagnostico> AtualizarDiagnosticosParcial(ref Diagnostico diagnosticoFaturado, Diagnostico diagnosticoOriginal)
        {
            List<Diagnostico> lista = new List<Diagnostico>();
            int quantidadeFaturada = diagnosticoFaturado.QuantidadeFaturada.Value + (new Domain.Servicos.RepositoryService()).Diagnostico.QuantidadeProdutosFaturados(diagnosticoOriginal.Id);

            //Retriever na ocorrência para carregar os dados e verificar a Data De Conserto Informada
            Ocorrencia ocorrencia = new RepositoryService().Ocorrencia.Retrieve(diagnosticoOriginal.OcorrenciaId.Id);

            // Não atingiu o Faturamento Total
            if (diagnosticoOriginal.QuantidadeSolicitada > quantidadeFaturada)
            {
                diagnosticoFaturado.RazaoStatus = (int)StatusDoDiagnostico.PedidoSolicitadoAoEms;
                lista.Add(diagnosticoFaturado);
                return lista;
            }


            if (diagnosticoOriginal.pecaEmEStoque == true && ocorrencia.DataDeConsertoInformada != null)
            {
                diagnosticoFaturado.RazaoStatus = (int)StatusDoDiagnostico.ConsertoRealizado; //Conserto Realizado                
            }
            else
            {
                diagnosticoFaturado.RazaoStatus = (int)StatusDoDiagnostico.AguardandoConserto;
            }
            diagnosticoFaturado.pecaEmEStoque = diagnosticoOriginal.pecaEmEStoque;
            lista.Add(diagnosticoFaturado);

            // Se for substituto não deve ser alterado o status
            Guid diagnosticoPaiId = Guid.Empty;
            if (diagnosticoFaturado.DiagnosticoPai != null
               && diagnosticoFaturado.DiagnosticoPai.RazaoStatus == (int)StatusDoDiagnostico.Substituido)
                diagnosticoPaiId = diagnosticoFaturado.DiagnosticoPai.Id;


            foreach (Diagnostico item in (new Domain.Servicos.RepositoryService()).Diagnostico.ListarPorFilhoEPai(diagnosticoOriginal.Id, 3, diagnosticoPaiId))
            {
                if (item.pecaEmEStoque == true && ocorrencia.DataDeConsertoInformada != null)
                {
                    item.RazaoStatus = (int)StatusDoDiagnostico.ConsertoRealizado; //Conserto Realizado 
                }
                else
                {
                    item.RazaoStatus = (int)StatusDoDiagnostico.AguardandoConserto; // Aguardando Conserto
                }

                lista.Add(item);
            }

            return lista;
        }

        private void EfetuaParcialESubstituto(ref Diagnostico diagnosticoFaturado, Diagnostico diagnosticoOriginal, TipoIntegracaoNotaFiscal tipo)
        {
            diagnosticoFaturado.DiagnosticoPaiId = new Lookup(diagnosticoOriginal.Id, "new_diagnostico_ocorrencia");

            if (tipo == TipoIntegracaoNotaFiscal.ProdutoSubstituto || tipo == TipoIntegracaoNotaFiscal.ProdutoSubstitutoEParcial)
            {
                diagnosticoFaturado.DiagnosticoPai.RazaoStatus = (int)StatusDoDiagnostico.Substituido;
                diagnosticoFaturado.DiagnosticoPai.QuantidadeSubstituida = diagnosticoOriginal.QuantidadeSolicitada;
                diagnosticoFaturado.DiagnosticoPai.ProdutoSubstitutoId = new Lookup(diagnosticoFaturado.ProdutoSubstituto.Id, "product");
                diagnosticoFaturado.DiagnosticoPai.ProdutoSubstituto = diagnosticoFaturado.ProdutoSubstituto;
                diagnosticoFaturado.ProdutoId = new Lookup(diagnosticoFaturado.ProdutoSubstituto.Id, "product");
                diagnosticoFaturado.Produto = diagnosticoFaturado.ProdutoSubstituto;
                diagnosticoFaturado.ProdutoSubstituto = null;
            }
            else
            {
                diagnosticoFaturado.ProdutoId = new Lookup(diagnosticoOriginal.Produto.Id, "product");
                diagnosticoFaturado.Produto = diagnosticoOriginal.Produto;
            }
            diagnosticoFaturado.DefeitoId = new Lookup(diagnosticoOriginal.Defeito.Id, "new_defeito");
            diagnosticoFaturado.Defeito = diagnosticoOriginal.Defeito;
            diagnosticoFaturado.Posicao = diagnosticoOriginal.Posicao;
            diagnosticoFaturado.SolucaoId = new Lookup(diagnosticoOriginal.Solucao.Id, "new_servico_assistencia_tecnica");
            diagnosticoFaturado.Solucao = diagnosticoOriginal.Solucao;
            diagnosticoFaturado.Valor = diagnosticoOriginal.Valor;
            diagnosticoFaturado.OcorrenciaId = new Lookup(diagnosticoOriginal.OcorrenciaId.Id, "incident");
            diagnosticoFaturado.Ocorrencia = RepositoryService.Ocorrencia.Retrieve(diagnosticoOriginal.OcorrenciaId.Id);

            diagnosticoFaturado.Id = Guid.Empty;
            diagnosticoFaturado.Nome = string.Format("{0} - {1}", diagnosticoFaturado.Produto.Codigo, diagnosticoFaturado.Produto.Nome);
            diagnosticoFaturado.QuantidadeSolicitada = 0;
            diagnosticoFaturado.QuantidadeSubstituida = 0;
            diagnosticoFaturado.DataGeracaoPedido = null;
            diagnosticoFaturado.GeraTroca = false;
            diagnosticoFaturado.NumeroDoPedido = string.Empty;
        }

        private void EfetuarAtualizacaoDiagnosticoNotaFiscal(List<Diagnostico> diagnosticosParaAtualizar, TipoIntegracaoNotaFiscal tipo, XmlDocument doc)
        {
            foreach (Diagnostico item in diagnosticosParaAtualizar)
            {
                this.AtualizarDiagnostico(item, "Integração de Nota Fiscal ASTEC", doc.InnerXml, tipo);
                if (item._diagnosticoPai != null)
                {
                    this.AtualizarDiagnostico(item.DiagnosticoPai, "Integração de Nota Fiscal ASTEC", doc.InnerXml, tipo);
                }
            }
        }

        private bool AtualizarDiagnostico(Diagnostico diagnostico, string nomeRotina, string logDeAlteracao, TipoIntegracaoNotaFiscal tipo)
        {
            bool status = false;
            string messageErro = string.Empty;
            try
            {
                diagnostico.LogPedido = logDeAlteracao.Replace(" ", "");
                if (diagnostico.Id == Guid.Empty)
                {
                    diagnostico.Id = (new Domain.Servicos.RepositoryService()).Diagnostico.Create(diagnostico);
                }
                else
                {
                    (new Domain.Servicos.RepositoryService()).Diagnostico.Update(diagnostico);
                }
                status = true;
            }
            catch (Exception ex)
            {
                //messageErro = LogHelper.Process(ex, ClassificacaoLog.WSIntegradorASTEC);
            }
            return status;
        }

        private void GravarLogIntegracao(Log log)
        {
            try { RepositoryService.Log.Create(log); }
            catch (Exception ex) { /*LogHelper.Process(ex, ClassificacaoLog.WSIntegradorASTEC);*/ }

        }

        public void AtualizarCodigoDeRastreamento(string numeroNotaFiscal, string numeroSerieNotaFiscal, string codigoEstabelecimento, string numeroDeRastreamento)
        {
            if (string.IsNullOrEmpty(numeroNotaFiscal)
                || string.IsNullOrEmpty(numeroSerieNotaFiscal)
                || string.IsNullOrEmpty(codigoEstabelecimento)
                || string.IsNullOrEmpty(numeroDeRastreamento))
                throw new ArgumentException("Erro de atualização rastreamento: Envie todos os dados para alterar o número do rastreamento");

            List<Diagnostico> lista = (new Domain.Servicos.RepositoryService()).Diagnostico.ListarPor(numeroNotaFiscal, numeroSerieNotaFiscal, codigoEstabelecimento);
            string logDeAlteracao = string.Format("Numero da Nota Fiscal: {0} \nSerie da Nota Fiscal: {1} \nEstabelecimento: {2} \nNumero de Rastreamento: {3} \n", numeroNotaFiscal, numeroSerieNotaFiscal, codigoEstabelecimento, numeroDeRastreamento);

            foreach (Diagnostico item in lista)
            {
                item.NumeroRastreamento = numeroDeRastreamento;
                this.AtualizarDiagnostico(item, "Atualizar Codigo de Rastreamento", logDeAlteracao, TipoIntegracaoNotaFiscal.AtualizarCodigoDeRastreamento);
            }
        }

        public void CancelarNotaFiscal(string numeroNotaFiscal, string numeroSerieNotaFiscal, string codigoEstabelecimento)
        {
            if (string.IsNullOrEmpty(numeroNotaFiscal)
                || string.IsNullOrEmpty(numeroSerieNotaFiscal)
                || string.IsNullOrEmpty(codigoEstabelecimento))
                throw new ArgumentException("Erro ao Cancelar Nota Fiscal! Os parametros não foram enviados de forma correta!");

            List<Diagnostico> lista = (new Domain.Servicos.RepositoryService()).Diagnostico.ListarPor(numeroNotaFiscal, numeroSerieNotaFiscal, codigoEstabelecimento);
            string logDeAlteracao = string.Format("Numero da Nota Fiscal: {0} \nSerie da Nota Fiscal: {1} \nEstabelecimento: {2} \n", numeroNotaFiscal, numeroSerieNotaFiscal, codigoEstabelecimento);

            foreach (Diagnostico item in lista)
            {
                List<Diagnostico> listaAux = null;

                if (item.DiagnosticoPai == null)
                {
                    item.RazaoStatus = (int)StatusDoDiagnostico.PedidoSolicitadoAoEms;
                    item.AddNullProperty("NumeroNotaFiscal");
                    item.AddNullProperty("SerieNotaFiscal");
                    item.AddNullProperty("QuantidadeFaturada");
                    item.AddNullProperty("NumeroRastreamento");
                    item.AddNullProperty("DataFaturamentoERP");
                    item.AddNullProperty("EstabelecimentoId");
                    item.LogPedido = logDeAlteracao;

                    (new Domain.Servicos.RepositoryService()).Diagnostico.Update(item);
                    listaAux = (new Domain.Servicos.RepositoryService()).Diagnostico.ListarPorFilhoEPai(item.Id, 4, item.Id);

                    foreach (Diagnostico itemAux in listaAux)
                    {
                        itemAux.RazaoStatus = (int)StatusDoDiagnostico.AguardandoConserto;
                        this.AtualizarDiagnostico(itemAux, "Cancelamento da Nota Fiscal ASTEC", logDeAlteracao, TipoIntegracaoNotaFiscal.Cancelamento);
                    }
                }
                else
                {
                    (new Domain.Servicos.RepositoryService()).Diagnostico.Delete(item.Id);
                    listaAux = (new Domain.Servicos.RepositoryService()).Diagnostico.ListarPorFilhoEPai(item.DiagnosticoPai.Id, int.MinValue, item.DiagnosticoPai.Id);
                    foreach (Diagnostico itemAux in lista)
                    {
                        itemAux.RazaoStatus = (int)StatusDoDiagnostico.AguardandoConserto;
                        this.AtualizarDiagnostico(itemAux, "Cancelamento da Nota Fiscal ASTEC", logDeAlteracao, TipoIntegracaoNotaFiscal.Cancelamento);
                    }
                }
            }
        }

        public void RetornoDoFaturamento(string retorno)
        {
            XmlDocument retornoXml = new XmlDocument();
            retornoXml.LoadXml(retorno);

            if (retornoXml.SelectSingleNode("/faturamento") == null) return;

            #region Xml Factory

            RetornoDoFaturamento retornoDoFaturamento = new RetornoDoFaturamento();
            retornoDoFaturamento.ItensDaNota = new List<ItemDaNota>();

            try { retornoDoFaturamento.CNPJ = retornoXml.SelectSingleNode("/faturamento").Attributes["CNPJ"].Value; }
            catch { }
            try { retornoDoFaturamento.NotaFiscal = retornoXml.SelectSingleNode("/faturamento").Attributes["notaFiscal"].Value; }
            catch { }
            try { retornoDoFaturamento.Estabelecimento = retornoXml.SelectSingleNode("/faturamento").Attributes["estabelecimento"].Value; }
            catch { }
            try { retornoDoFaturamento.DataEmissao = Convert.ToDateTime(retornoXml.SelectSingleNode("/faturamento").Attributes["dataEmissao"].Value); }
            catch { }
            try { retornoDoFaturamento.Serie = retornoXml.SelectSingleNode("/faturamento").Attributes["serie"].Value; }
            catch { }

            foreach (XmlElement item in retornoXml.SelectNodes("/faturamento/item"))
            {
                ItemDaNota itemDaNota = new ItemDaNota();

                try { itemDaNota.Codigo = item.Attributes["codigo"].Value; }
                catch { }
                try { itemDaNota.DiagnosticoId = new Guid(item.Attributes["diagnosticoId"].Value); }
                catch { }
                try { itemDaNota.QuantidadeFaturada = Convert.ToInt32(item.Attributes["quantidadeFaturada"].Value); }
                catch { }
                try { itemDaNota.PrecoUnitario = Convert.ToDecimal(item.Attributes["precoUnitario"].Value); }
                catch { }
                try { itemDaNota.IPIaliquota = Convert.ToDecimal(item.Attributes["IPIaliquota"].Value); }
                catch { }
                try { itemDaNota.IPIvalor = Convert.ToDecimal(item.Attributes["IPIvalor"].Value); }
                catch { }
                try { itemDaNota.ICMSitem = Convert.ToDecimal(item.Attributes["ICMSitem"].Value); }
                catch { }
                try { itemDaNota.ICMSbase = Convert.ToDecimal(item.Attributes["ICMSbase"].Value); }
                catch { }
                try { itemDaNota.ItemSubstituido = item.Attributes["itemSubstituido"].Value; }
                catch { }
                try { itemDaNota.QuantidadeSubstituida = Convert.ToInt32(item.Attributes["quantidadeSubstituida"].Value); }
                catch { }
            }

            #endregion

            foreach (ItemDaNota itemDaNota in retornoDoFaturamento.ItensDaNota)
            {
                bool criar = false;
                Diagnostico diagnosticoAguardandoConserto = new Diagnostico(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                Diagnostico diagnostico = (new RepositoryService()).Diagnostico.Retrieve(itemDaNota.DiagnosticoId);

                //Quando o status do diagnóstico for menor ou igual a "Aguardando conserto" ou quando for "Pedido solicitado ao EMS", seta o status para "Aguardando Conserto"
                if (diagnostico.Ocorrencia.Status.Value <= 200037 || diagnostico.Ocorrencia.Status.Value == 200045)
                {
                    diagnosticoAguardandoConserto.StatusDiagnostico = StatusDoDiagnostico.AguardandoConserto; // Status alterado para Aguardando Conserto                                                                        
                }
                //Quando o conserto da peça já foi feito, seta o status para "Conserto Realizado"
                else if (diagnostico.Ocorrencia.DataDeConsertoInformada != DateTime.MinValue)
                {
                    diagnosticoAguardandoConserto.StatusDiagnostico = StatusDoDiagnostico.ConsertoRealizado; // Status alterado para Conserto Relizado
                }

                diagnosticoAguardandoConserto.RazaoStatus = (int)StatusDoDiagnostico.AguardandoConserto;
                diagnosticoAguardandoConserto.CodigoMatriz = itemDaNota.Codigo;
                diagnosticoAguardandoConserto.QuantidadeFaturada = itemDaNota.QuantidadeFaturada;
                diagnosticoAguardandoConserto.QuantidadeSubstituida = itemDaNota.QuantidadeSubstituida;
                diagnosticoAguardandoConserto.ValorUnitario = itemDaNota.PrecoUnitario;
                diagnosticoAguardandoConserto.ValorIPI = itemDaNota.IPIvalor;
                diagnosticoAguardandoConserto.AliquotaIPI = itemDaNota.IPIaliquota;
                diagnosticoAguardandoConserto.ValorICMS = itemDaNota.ICMSitem;
                diagnosticoAguardandoConserto.ValorBaseICMS = itemDaNota.ICMSbase;
                diagnosticoAguardandoConserto.NumeroNotaFiscal = retornoDoFaturamento.NotaFiscal;

                // VERIFICAR NECESSIDADE DO CNPJ
                diagnosticoAguardandoConserto.SerieNotaFiscal = retornoDoFaturamento.Serie;
                diagnosticoAguardandoConserto.EstabelecimentoId = new Lookup((new Domain.Servicos.RepositoryService()).Estabelecimento.ObterPor(Convert.ToInt32(retornoDoFaturamento.Estabelecimento)).Id, "itbc_estabelecimento");
                diagnosticoAguardandoConserto.DataFaturamentoERP = retornoDoFaturamento.DataEmissao;

                Guid diagnosticoId = itemDaNota.DiagnosticoId;

                if (!String.IsNullOrEmpty(itemDaNota.ItemSubstituido))
                {
                    Diagnostico diagnosticoSubstituido = new Diagnostico(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false)
                    {
                        Id = itemDaNota.DiagnosticoId,
                        Status = (int)StatusDoDiagnostico.Substituido // Status alterado para Produto Substituido
                    };

                    (new Domain.Servicos.RepositoryService()).Diagnostico.Update(diagnosticoSubstituido);
                    diagnosticoAguardandoConserto.Produto = (new Domain.Servicos.RepositoryService()).Produto.ObterPorNumero(itemDaNota.ItemSubstituido);
                    criar = true;
                }

                if (criar)
                {
                    // Valida se já existe a duplicidade que pretende-se criar
                    Diagnostico diagnosticoValidacao = (new Domain.Servicos.RepositoryService()).Diagnostico.Retrieve(itemDaNota.DiagnosticoId);
                    if ((new Domain.Servicos.RepositoryService()).Diagnostico.ObterDuplicidade(
                        diagnosticoValidacao.Ocorrencia.Id,
                        diagnosticoAguardandoConserto.Produto.Id,
                        itemDaNota.DiagnosticoId,
                        diagnosticoValidacao.Defeito.Id,
                        diagnosticoAguardandoConserto.NumeroNotaFiscal,
                        diagnosticoAguardandoConserto.SerieNotaFiscal,
                        itemDaNota.DiagnosticoId) == null)
                        (new Domain.Servicos.RepositoryService()).Diagnostico.Create(diagnosticoAguardandoConserto);
                }
                else
                {
                    diagnosticoAguardandoConserto.Id = itemDaNota.DiagnosticoId;
                    (new Domain.Servicos.RepositoryService()).Diagnostico.Update(diagnosticoAguardandoConserto);
                }
            }
        }

        #endregion

        #region CRM 4
        public void ExportaPedidos()
        {
            int countPedidosProcessados = 0;
            string nomeArquivo;
            string textoEmail = "Olá, <br /><br />";
            textoEmail += "Início da rotina: " + DateTime.Now + " <br /><br />";            
            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoCRM);
            #endregion
            //Cria pasta de armazenamento de log caso não exista.
            if (!Directory.Exists(Settings.Default.Log))
            {
                Directory.CreateDirectory(Settings.Default.Log);
            }

            //Cria o arquivo de log da execução
            if (Settings.Default.HabilitarLog)
            {
                ostrm = new FileStream(Settings.Default.Log + @"\Log_" + DateTime.Now.ToString("yyyy-MM-dd-HH.mm") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
                oldOut = Console.Out;
                Console.SetOut(writer);
            }

            Console.WriteLine("******************************************************************************");
            Console.WriteLine("                           Serviço - Pedidos Astec");
            Console.WriteLine("******************************************************************************");
            Console.WriteLine();

            List<PedidoEnvio> pedidos = new List<PedidoEnvio>();
            List<Diagnostico> diagnosticosParaPedido = new List<Diagnostico>();
            List<Diagnostico> atualizacaoDiagnosticos = new List<Diagnostico>();

            Console.WriteLine("-> Início do processamento (" + DateTime.Now + ")");
            Console.WriteLine();
            Console.WriteLine("-> Buscando pedidos...");
            List<Diagnostico> diagnosticosParaExportacao = RepositoryService.Diagnostico.ListarDiagnosticosParaExportacaoDePedidosASTEC();

            Console.WriteLine("-> Calculando valor do serviço das ocorrências dos diagnósticos...");
            this.CalculaValorServicoDasOcorrenciasDosDiagnosticos(ref diagnosticosParaExportacao);

            Console.WriteLine("-> Total de " + diagnosticosParaExportacao.Count + " pedidos");
            foreach (Diagnostico diagnostico in diagnosticosParaExportacao)
            {
                try
                {
                    string mensagem;
                    switch (diagnostico.DeveFazerPedidoAstec(out mensagem))
                    {
                        case CondicaoPedidoAstec.GerarPedido:
                            diagnosticosParaPedido.Add(diagnostico);
                            countPedidosProcessados++;
                            break;
                        case CondicaoPedidoAstec.AvancarStatus:
                            Diagnostico diagnosticoTemp = new Diagnostico() { Id = diagnostico.Id };
                            diagnosticoTemp.RegraExcessaoPedido = true;
                            diagnosticoTemp.RazaoStatus = 4; // Aguardando Conserto
                            atualizacaoDiagnosticos.Add(diagnosticoTemp);
                            break;
                        default:
                            break;
                    }

                    if (!string.IsNullOrEmpty(mensagem))
                    {
                        Console.WriteLine("Diagnóstico: " + diagnostico.Id.ToString() + " - Mensagem: " + mensagem);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO: Diagnóstico: " + diagnostico.Id.ToString() + " - Mensagem: " + ex.Message);
                }
            }

            pedidos = this.MontaPedidosEnvio(diagnosticosParaPedido);

            foreach (Diagnostico atualizarDiagnostico in atualizacaoDiagnosticos)
            {
                try { RepositoryService.Diagnostico.Update(atualizarDiagnostico); }
                catch (Exception ex)
                {
                    Console.WriteLine("Atualizar Diagnóstico na Exportação de Pedido ASTEC - Mensagem: " + ex.Message);
                }
            }

            foreach (PedidoEnvio pedido in pedidos)
            {
                XmlDocument xml = new XmlDocument();
                try
                {
                    xml = ConvertePedido(pedido);
                    EfetuaEmissaoPedido(xml);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Integrar pedido do cliente - Pedido: " + pedido.Cliente + " - Mensagem: " + ex.Message);
                }
            }
            Console.WriteLine();
            Console.WriteLine("Total de pedidos integrados: " + countPedidosProcessados);
            Console.WriteLine();
            Console.WriteLine("-> Fim do processamento (" + DateTime.Now + ")");
            Console.WriteLine();
            Console.WriteLine("******************************************************************************");
            Console.WriteLine("                          Fim da execução");
            if (Settings.Default.HabilitarLog)
            {
                Console.SetOut(oldOut);
                writer.Close();
                ostrm.Close();
            }
            if (countPedidosProcessados == 0)
            {
                nomeArquivo = ostrm.Name.Replace("//","/");                
                textoEmail += "<b> Serviço de integração de pedidos da Astec executou essa noite, mas não integrou nenhum pedido! </b><br />";
                textoEmail += "Analisar o arquivo <b>" + nomeArquivo + " Servidor (sjo-crm2015-04)</b><br /><br />";
                textoEmail += "Rotina finalizada: " + DateTime.Now;
                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina de integração de pedidos da Astec", "", Convert.ToString(parametroGlobalEmail.Valor));
            }

        }

        private void CalculaValorServicoDasOcorrenciasDosDiagnosticos(ref List<Diagnostico> diagnosticos)
        {
            List<Guid> listaIdsOcorrencias = new List<Guid>();

            for (int x = 0; x < diagnosticos.Count; x++)
            {
                try
                {
                    if (diagnosticos[x].OcorrenciaId != null)
                    {
                        if (!listaIdsOcorrencias.Contains(diagnosticos[x].OcorrenciaId.Id))
                        {
                            listaIdsOcorrencias.Add(diagnosticos[x].OcorrenciaId.Id);
                            diagnosticos[x].Ocorrencia = RepositoryService.Ocorrencia.Retrieve(diagnosticos[x].OcorrenciaId.Id);
                            var valor = diagnosticos[x].Ocorrencia.ObterValorDeServico();

                            if (diagnosticos[x].Ocorrencia.ValorServico.HasValue)
                            {
                                if (diagnosticos[x].Ocorrencia.ValorServico.Value != valor)
                                {
                                    RepositoryService.Ocorrencia.Update(new Ocorrencia() { Id = diagnosticos[x].OcorrenciaId.Id, ValorServico = valor });
                                    diagnosticos[x].Ocorrencia.ValorServico = valor;
                                }
                            }
                            else
                            {
                                RepositoryService.Ocorrencia.Update(new Ocorrencia() { Id = diagnosticos[x].OcorrenciaId.Id, ValorServico = valor });
                                diagnosticos[x].Ocorrencia.ValorServico = valor;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SDKore.Helper.Log.Logar("integrapedidoasteclog.txt", "<<Calculo Valor do Servico>>Diagnostico: " + diagnosticos[x].Id + " ++++ Mensagem: " + ex.Message);
                }
            }
        }

        private List<PedidoEnvio> MontaPedidosEnvio(List<Diagnostico> diagnosticos)
        {
            List<PedidoEnvio> pedidos = new List<PedidoEnvio>();

            foreach (var item in diagnosticos)
            {
                bool incluirPedido = true;
                PedidoEnvio pedido = null;

                foreach (PedidoEnvio pedidoEnvio in pedidos)
                    if (pedidoEnvio.Cliente == item.Ocorrencia.Autorizada.CodigoMatriz
                        && pedidoEnvio.Atendente == item.Ocorrencia.Produto.LinhaComercial.CodigoAtendente.Value.ToString()
                        && pedidoEnvio.Estabelecimento == item.Ocorrencia.Produto.LinhaComercial.Estabelecimento.Codigo.Value)
                    {
                        incluirPedido = false;
                        pedido = pedidoEnvio;
                        break;
                    }

                if (incluirPedido)
                {
                    pedido = new PedidoEnvio();
                    pedido.Diagnosticos = new List<Diagnostico>();

                    try { pedido.Atendente = item.Ocorrencia.Produto.LinhaComercial.CodigoAtendente.Value.ToString(); }
                    catch { }
                    try { pedido.CanalDeVendas = item.Produto.LinhaComercial.CanalDeVenda.CodigoVenda.Value.ToString(); }
                    catch { }
                    try { pedido.Cliente = item.Ocorrencia.Autorizada.CodigoMatriz; }
                    catch { }
                    try { pedido.Estabelecimento = item.Ocorrencia.Produto.LinhaComercial.Estabelecimento.Codigo.Value; }
                    catch { }
                    try { pedido.Transportadora = new RepositoryService().Transportadora.Retrieve(item.Ocorrencia.Autorizada.TransportadoraASTEC.Id); }
                    catch { }

                    pedidos.Add(pedido);
                }

                pedido.Diagnosticos.Add(item);
            }

            return pedidos;
        }

        private XmlDocument ConvertePedido(PedidoEnvio pedido)
        {
            XmlDocument pedidoXml = new XmlDocument();
            XmlDeclaration xmlDeclaration = pedidoXml.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement noPedido = pedidoXml.CreateElement("pedido");
            pedidoXml.InsertBefore(xmlDeclaration, pedidoXml.DocumentElement);
            pedidoXml.AppendChild(noPedido);
            noPedido.SetAttribute("cliente", pedido.Cliente);
            noPedido.SetAttribute("transportadora", pedido.Transportadora.NomeAbreviado);
            noPedido.SetAttribute("estabelecimento", pedido.Estabelecimento.ToString());
            noPedido.SetAttribute("atendente", pedido.Atendente);
            noPedido.SetAttribute("canalDeVendas", pedido.CanalDeVendas);

            foreach (Diagnostico diagnostico in pedido.Diagnosticos)
            {
                XmlElement noDisgnostico = pedidoXml.CreateElement("diagnostico");
                noDisgnostico.SetAttribute("codigoItem", diagnostico.Produto.Codigo);
                noDisgnostico.SetAttribute("quantidade", diagnostico.QuantidadeSolicitada.ToString());
                noDisgnostico.SetAttribute("numeroOs", diagnostico.Ocorrencia.Numero);
                noDisgnostico.SetAttribute("id", diagnostico.Id.ToString());
                noDisgnostico.SetAttribute("codigoProdutoPrincipal", diagnostico.Ocorrencia.Produto.Codigo);
                noDisgnostico.SetAttribute("pecaEstoque", diagnostico.pecaEmEStoque.ToString());
                noDisgnostico.SetAttribute("statusOcorrencia", diagnostico.Ocorrencia.StatusDaOcorrencia.ToString());

                pedidoXml.DocumentElement.PrependChild(noDisgnostico);
            }

            return pedidoXml;
        }

        private void EfetuaEmissaoPedido(XmlDocument pedidoXml)
        {
            string xmlRetorno = string.Empty;
            bool status = true;
            string mensagem = string.Empty;
            string codigoPedido = string.Empty;
            XmlDocument retorno = new XmlDocument();

            RepositoryService.Pedido.EntradaPedidoASTEC(pedidoXml.InnerXml, out xmlRetorno);
            
            //Devido a caracteres especiais retornados do ERP, estamos validando aqui para não gerar mais problemas (podem existir outros caracteres especiais, como o sinal < e >)
            xmlRetorno = xmlRetorno.Replace("&", "E");
            retorno.LoadXml(xmlRetorno);


            if (retorno.SelectSingleNode("/pedido") != null)
            {
                if (retorno.SelectSingleNode("/pedido").Attributes["resultadoDaOperacao"] != null) status = retorno.SelectSingleNode("/pedido").Attributes["resultadoDaOperacao"].Value == "1";
                if (retorno.SelectSingleNode("/pedido").Attributes["mensagem"] != null) mensagem = retorno.SelectSingleNode("/pedido").Attributes["mensagem"].Value;
                if (retorno.SelectSingleNode("/pedido").Attributes["codigoPedido"] != null) codigoPedido = retorno.SelectSingleNode("/pedido").Attributes["codigoPedido"].Value;
            }

            foreach (XmlElement item in pedidoXml.SelectNodes("/pedido/diagnostico"))
            {
                Guid id = item.Attributes["id"] != null && !String.IsNullOrEmpty(item.Attributes["id"].Value) ? new Guid(item.Attributes["id"].Value) : Guid.Empty;
                Diagnostico diagnostico = new Diagnostico(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                diagnostico.Id = id;
                Boolean temPecaEstoque = Convert.ToBoolean(item.Attributes["pecaEstoque"].Value);
                string statusOcorrencia = item.Attributes["statusOcorrencia"].Value;

                string log = "Sucesso Integração ERP: " + status.ToString() + "\nMensagem: " + mensagem + "\n\nXML Pedido: " + pedidoXml.InnerXml + "\n\n" + retorno.InnerXml;

                if (status)
                {
                    diagnostico.RazaoStatus = 3; // Pedido Enviado ao EMS
                    diagnostico.NumeroDoPedido = codigoPedido;
                    diagnostico.DataGeracaoPedido = DateTime.Now;
                    diagnostico.RegraExcessaoPedido = false;
                    diagnostico.LogPedido = log;
                    diagnostico.pecaEmEStoque = temPecaEstoque;
                    try
                    {
                        RepositoryService.Diagnostico.Update(diagnostico);
                        log = "";
                        Console.WriteLine();
                        Console.WriteLine("-> SUCESSO: Diagnóstico: " + id.ToString());
                        Console.WriteLine("    -> Mensagem: " + mensagem);
                        Console.WriteLine("    -> Alterações: " + pedidoXml.InnerText);
                        Console.WriteLine("    -> Retorno TOTVs: " + retorno.InnerXml);
                    }
                    catch (Exception ex)
                    {
                        log += "\n\nErro Atualização CRM: " + ex.Message;
                        Console.WriteLine();
                        Console.WriteLine("-> ERRO: Diagnóstico: " + id.ToString());
                        Console.WriteLine("    -> Mensagem: " + mensagem + ex.Message);
                        Console.WriteLine("    -> Alterações: " + pedidoXml.InnerText);
                        Console.WriteLine("    -> Retorno TOTVs" + retorno.InnerXml);
                        
                    }
                }
                if (!string.IsNullOrEmpty(log))
                {
                    try
                    {
                        diagnostico.LogPedido = log;
                        RepositoryService.Diagnostico.Update(diagnostico);
                    }
                    catch (Exception ex)
                    {
                        //Se houve erro no CRM será logado acima
                    }
                }
                Log logRegistro = new Log();
                logRegistro.Nome = "Integração Pedido ASTEC";
                logRegistro.Acao = "Criação";
                logRegistro.Sucesso = status == false ? 4 : 3;
                logRegistro.Mensagem = mensagem;
                logRegistro.ID = new Lookup(id, "new_diagnostico_ocorrencia");
                logRegistro.Alteracoes = pedidoXml.InnerXml + "\n\n\n" + retorno.InnerXml;

                GravarLogIntegracao(logRegistro);
            }
        }
        #endregion
    }


    internal enum TipoIntegracaoNotaFiscal
    {
        NotaFiscalJaExistente,
        ProdutoSubstituto,
        FaturamentoParcial,
        ProdutoSubstitutoEParcial,
        FaturamentoTotal,
        Cancelamento,
        AtualizarCodigoDeRastreamento
    }
}