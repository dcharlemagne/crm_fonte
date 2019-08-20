// MENSAGEM RELACIONADA AO PROCESSO PEDIDO
// IMPLEMENTADA EM 21/03/14 POR FCJ - PARCIALMENTE

using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0091 : Base, IBase<Intelbras.Message.Helper.MSG0091, Domain.Model.Pedido>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Lookup LookupMoeda = null;

        #region Listas Iniciais para aprimoramento de performance - Em fase de Testes
        private List<Product> listaProdutosCRM = null;
        private List<UnidadeNegocio> listaUnidadesNegocioCRM = null;
        private List<Unidade> listaUnidadesMedidaCRM = null;
        private List<Contato> listaContatosCRM = null;
        private List<NaturezaOperacao> listaNaturezaOperacaoCRM = null;

        #endregion

        #endregion

        #region Construtor
        public MSG0091(string org, bool isOffline)
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

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            usuarioIntegracao = usuario;
            Pollux.MSG0091 polluxMsg0091 = this.CarregarMensagem<Pollux.MSG0091>(mensagem);
            var pedidoService = new PedidoService(this.Organizacao, this.IsOffline);

            var statusPedido = pedidoService.BuscaPedidoEMS(polluxMsg0091.NumeroPedido, "statecode");

            if (statusPedido != null)
            {
                if (statusPedido.Status == (int)Enum.Pedido.StateCode.Cancelada)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Pedido foi ignorado para atualização, o status no CRM é Cancelado e não é feito mais modificações!";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
                }
            }

            #region Carregar Arrays

            String[] aryTempVarParaItensSemDuplicidade = ExtrairArrays(polluxMsg0091.PedidosItens.GroupBy(x => x.Produto));
            listaProdutosCRM = new ProdutoService(Organizacao, IsOffline).BuscaPorCodigo(aryTempVarParaItensSemDuplicidade);

            aryTempVarParaItensSemDuplicidade = ExtrairArrays(polluxMsg0091.PedidosItens.GroupBy(x => x.Representante.ToString()));
            listaContatosCRM = new ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(aryTempVarParaItensSemDuplicidade);

            aryTempVarParaItensSemDuplicidade = ExtrairArrays(polluxMsg0091.PedidosItens.GroupBy(x => x.NaturezaOperacao));
            listaNaturezaOperacaoCRM = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(Organizacao, IsOffline).BuscaNaturezaOperacaoPorCodigo(aryTempVarParaItensSemDuplicidade);

            aryTempVarParaItensSemDuplicidade = ExtrairArrays(polluxMsg0091.PedidosItens.GroupBy(x => x.UnidadeNegocio));
            listaUnidadesNegocioCRM = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(Organizacao, IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(aryTempVarParaItensSemDuplicidade);

            aryTempVarParaItensSemDuplicidade = ExtrairArrays(polluxMsg0091.PedidosItens.GroupBy(x => x.UnidadeMedida));
            listaUnidadesMedidaCRM = new Intelbras.CRM2013.Domain.Servicos.UnidadeService(Organizacao, IsOffline).BuscaUnidadePorNome(aryTempVarParaItensSemDuplicidade);

            #endregion

            Pedido objPedido = this.DefinirPropriedades(polluxMsg0091);
            List<ProdutoPedido> lstProdutoPedido = this.DefinirPropriedadesProdutoPedido(polluxMsg0091, Guid.Empty);

            bool fecharPedido = false;
            bool alteraStatus = false;
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
            }

            if ((int)Enum.Pedido.StateCode.Cumprido == objPedido.Status.Value)
                fecharPedido = true;

            int countErrosIntegracaoItem = lstProdutoPedido.Where(x => x.IntegradoRepresentanteComErro.Value).Count();

            if (countErrosIntegracaoItem > 0)
            {
                objPedido.IntegradoComErros = true;
            }

            //Persistir pedido para obter o id que sera utilizado no ProdutoPedido
            objPedido = pedidoService.Persistir(objPedido, ref alteraStatus, fecharPedido);

            if (objPedido == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir pedido!";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
            }
            else
            {
                lstProdutoPedido = this.DefinirPropriedadesProdutoPedido(polluxMsg0091, objPedido.ID.Value);
            }

            if (!resultadoPersistencia.Sucesso || lstProdutoPedido == null)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
            }

            foreach (var item in lstProdutoPedido)
            {
                item.Pedido = new Lookup(objPedido.ID.Value, "");
            }

            //Prossegue se a lista for preenchida corretamente
            foreach (ProdutoPedido item in lstProdutoPedido)
            {
                ProdutoPedido prodPedidoTemp = new ProdutoPedido(this.Organizacao, this.IsOffline);

                if (item.Acao.ToUpper().Equals("A") || item.Acao.ToUpper().Equals("I"))
                {
                    if (item.SituacaoItem == (int)Enum.ProdutoPedido.Status.Cancelado)
                    {
                        item.CancelarParaSoma();
                    }

                    prodPedidoTemp = new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline)
                        .Persistir(item);
                }
                else if (item.Acao.ToUpper().Equals("C"))
                {
                    item.SituacaoItem = (int)Enum.ProdutoPedido.Status.Cancelado;
                    item.CancelarParaSoma();

                    new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).Persistir(item);
                }
                else if (item.Acao.ToUpper().Equals("E"))
                {
                    new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).Excluir(item);
                }
                else
                {
                    throw new Exception("Ação do item do pedido fora do padrão.");
                }

                if (prodPedidoTemp == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Erro ao persistir ProdutoPedido!";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
                }
            }

            if (fecharPedido)
            {
                try
                {
                    new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).FecharPedido(objPedido.ID.Value);
                }
                catch (Exception ex)
                {
                    SDKore.Helper.Error.Handler(ex);
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Erro ao fechar Pedido!";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
                }
            }
            else if (alteraStatus)
            {
                new Servicos.PedidoService(this.Organizacao, this.IsOffline).MudarStatusPedido(objPedido.ID.Value, objPedido.Status.Value, objPedido.RazaoStatus.Value);
            }
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("CodigoPedido", objPedido.ID.Value.ToString());
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0091R1>(numeroMensagem, retorno);
        }

        #region Definir Propriedades

        private Guid ObterRepresentatePadrao()
        {
            string representanteIdConfig = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Pedido.RepresentantePadrao", true);
            return new Guid(representanteIdConfig);
        }

        private UnidadeNegocio ObterUnidadeNegocioPedido(Message.Helper.MSG0091 xml)
        {
            if (xml.PedidosItens != null && xml.PedidosItens.Count > 0)
            {
                var item = xml.PedidosItens[0];

                if (item.UnidadeNegocio != null)
                {
                    return new Domain.Servicos.RepositoryService(this.Organizacao, this.IsOffline)
                     .UnidadeNegocio.ObterPorChaveIntegracao(item.UnidadeNegocio);
                }
            }

            return null;
        }

        public Pedido DefinirPropriedades(Intelbras.Message.Helper.MSG0091 xml)
        {
            Pedido crm = new Pedido(this.Organizacao, this.IsOffline);

            if (!String.IsNullOrEmpty(xml.UsuarioAprovacao))
                crm.UsuarioAprovacao = xml.UsuarioAprovacao;

            crm.FaturamentoParcial = xml.FaturamentoParcial.Value;

            if (xml.ModalidadeCobranca.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.Modalidade), xml.ModalidadeCobranca))
                    crm.Modalidade = xml.ModalidadeCobranca;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Modalidade Cobranca não encontrada!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Modalidade");
            }

            if (xml.CanalVenda.HasValue)
            {
                CanaldeVenda canalDeVenda = new CanaldeVenda(this.Organizacao, this.IsOffline);
                canalDeVenda = new Intelbras.CRM2013.Domain.Servicos.CanalDeVendaService(this.Organizacao, this.IsOffline).BuscaCanalDeVendaPorCodigoVenda(xml.CanalVenda.Value);
                if (canalDeVenda != null)
                {
                    crm.CanalVendaID = new Lookup(canalDeVenda.ID.Value, "");
                }

                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Canal de Venda não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("CanalVendaID");
            }

            if (!String.IsNullOrEmpty(xml.NumeroPedidoCliente))
                crm.PedidoCliente = xml.NumeroPedidoCliente;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NumeroPedidoCliente não enviado.";
                return crm;
            }

            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.SituacaoAlocacao), xml.SituacaoAlocacao))
                crm.SituacaoAlocacao = xml.SituacaoAlocacao.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situacao Alocacao não encontrado!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.MotivoBloqueioCredito))
                crm.MotivoBloqueioCredito = xml.MotivoBloqueioCredito;
            else
                crm.AddNullProperty("MotivoBloqueioCredito");

            if (xml.TotalSubstituicaoTributaria.HasValue)
                crm.TotalSubstituicaoTributaria = xml.TotalSubstituicaoTributaria;
            else
                crm.AddNullProperty("TotalSubstituicaoTributaria");

            crm.DataImplantacao = xml.DataImplantacao;

            if (!String.IsNullOrEmpty(xml.UsuarioAlteracao))
                crm.UsuarioAlteracao = xml.UsuarioAlteracao;
            else
                crm.AddNullProperty("UsuarioAlteracao");



            //CondicaoPagamento

            if (xml.CondicaoPagamento.HasValue)
            {
                CondicaoPagamento condPgto = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(xml.CondicaoPagamento.Value);
                if (condPgto != null)
                {
                    crm.CondicaoPagamento = new Lookup(condPgto.ID.Value, "");
                }
            }
            else
            {
                crm.AddNullProperty("CondicaoPagamento");
            }


            if (!String.IsNullOrEmpty(xml.CodigoSolicitacaoBeneficio))
            {
                crm.SolicitacaoBeneficio = new Lookup(new Guid(xml.CodigoSolicitacaoBeneficio), "");
            }
            else
            {
                crm.AddNullProperty("SolicitacaoBeneficio");
            }

            if (!String.IsNullOrEmpty(xml.CodigoEntregaTriangular))
                crm.CodigoEntregaTriangular = xml.CodigoEntregaTriangular;
            else
                crm.AddNullProperty("CodigoEntregaTriangular");

            if (!String.IsNullOrEmpty(xml.UsuarioCancelamento))
                crm.UsuarioCancelamento = xml.UsuarioCancelamento;
            else
                crm.AddNullProperty("UsuarioCancelamento");

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Telefone))
                crm.TelefoneEntrega = xml.EnderecoEntrega.Telefone;
            else
                crm.AddNullProperty("TelefoneEntrega");

            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.DestinoMercadoria), xml.DestinoMercadoria))
                crm.DestinoMercadoria = xml.DestinoMercadoria;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Destino Mercadoria não encontrado!";
                return crm;
            }


            if (xml.PercentualDescontoICMS.HasValue)
                crm.PercentualDescontoICMS = xml.PercentualDescontoICMS;
            else
                crm.AddNullProperty("PercentualDescontoICMS");

            if (!String.IsNullOrEmpty(xml.UsuarioReativacao))
                crm.UsuarioReativacao = xml.UsuarioReativacao;
            else
                crm.AddNullProperty("UsuarioReativacao");

            if (!String.IsNullOrEmpty(xml.Observacao))
                crm.Descricao = xml.Observacao;
            else
                crm.AddNullProperty("Descricao");

            if (!String.IsNullOrEmpty(xml.ObservacaoRedespacho))
                crm.CondicoesRedespacho = xml.ObservacaoRedespacho;
            else
                crm.AddNullProperty("CondicoesRedespacho");


            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Estado))
            {
                Model.Estado estado = new Model.Estado(this.Organizacao, this.IsOffline);
                estado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.EnderecoEntrega.Estado);

                if (estado != null && estado.ID.HasValue)
                    crm.EnderecoEntregaEstado = new Lookup(estado.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estado não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Estado não enviado.";
                return crm;
            }

            Estabelecimento estabelecimento = new Estabelecimento(this.Organizacao, this.IsOffline);
            estabelecimento = new Intelbras.CRM2013.Domain.Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimentoPorCodigo(xml.Estabelecimento);
            if (estabelecimento != null)
            {
                crm.Estabelecimento = new Lookup(estabelecimento.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Estabelecimento não encontrado.";
                return crm;
            }


            if (xml.ValorTotalAberto.HasValue)
                crm.ValorTotalAberto = xml.ValorTotalAberto;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor Total Aberto não enviado.";
                return crm;
            }

            //Removido essa validação para que os pedidos de clientes estrangeiros sejam integrados no CRM
            //crm.CPFCNPJ = !String.IsNullOrEmpty(xml.CPF) ? xml.CPF : !String.IsNullOrEmpty(xml.CNPJ) ? xml.CNPJ : String.Empty;
            //if (String.IsNullOrEmpty(crm.CPFCNPJ))
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "CPF/CNPJ não enviado.";
            //    return crm;
            //}

            if (!String.IsNullOrEmpty(xml.NumeroPedidoRepresentante))
                crm.PedidoRepresentante = xml.NumeroPedidoRepresentante;
            else
                crm.AddNullProperty("PedidoRepresentante");

            if (xml.DataCancelamento.HasValue)
                crm.DataCancelamento = xml.DataCancelamento;
            else
                crm.AddNullProperty("DataCancelamento");

            crm.DataEmissao = xml.DataEmissao;

            if (xml.TipoPreco.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.TipoPreco), xml.TipoPreco))
                    crm.TipoPreco = xml.TipoPreco;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tipo Preco não encontrado!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("TipoPreco");
            }

            if (!String.IsNullOrEmpty(xml.MotivoLiberacaoCredito))
                crm.MotivoLiberacaoCredito = xml.MotivoLiberacaoCredito;
            else
                crm.AddNullProperty("MotivoLiberacaoCredito");

            if (xml.CondicaoPagamento.HasValue)
            {
                CondicaoPagamento condicaoPagamento = new CondicaoPagamento(this.Organizacao, this.IsOffline);
                condicaoPagamento = new Intelbras.CRM2013.Domain.Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(xml.CondicaoPagamento.Value);
                if (condicaoPagamento != null)
                    crm.CondicaoPagamento = new Lookup(condicaoPagamento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CondicaoPagamento não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("CondicaoPagamento");
            }

            if (xml.CondicaoFrete.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.CondicoesFrete), xml.CondicaoFrete))
                    crm.CondicoesFrete = xml.CondicaoFrete;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Condições de frete não encontrado!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("CondicoesFrete");
            }

            if (!String.IsNullOrEmpty(xml.TabelaFinanciamento))
            {
                TabelaFinanciamento tabelaFinanciamento = new TabelaFinanciamento(this.Organizacao, this.IsOffline);
                tabelaFinanciamento = new Intelbras.CRM2013.Domain.Servicos.TabelaFinanciamentoService(this.Organizacao, this.IsOffline).ObterTabelaFinanciamento(xml.TabelaFinanciamento);
                if (tabelaFinanciamento != null)
                    crm.TabelaFinanciamento = new Lookup(tabelaFinanciamento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tabela Financiamento não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("TabelaFinanciamento");
            }

            if (!String.IsNullOrEmpty(xml.CidadeCIF))
                crm.CidadeCIF = xml.CidadeCIF;
            else
                crm.AddNullProperty("CidadeCIF");

            if (!String.IsNullOrEmpty(xml.InscricaoEstadual))
                crm.InscricaoEstadual = xml.InscricaoEstadual;
            else
                crm.AddNullProperty("InscricaoEstadual");

            if (xml.IndicacaoAprovacao.HasValue)
                crm.Aprovacao = xml.IndicacaoAprovacao;
            else
                crm.AddNullProperty("Aprovacao");

            if (!String.IsNullOrEmpty(xml.UsuarioAprovacao))
                crm.Aprovador = xml.UsuarioAprovacao;
            else
                crm.AddNullProperty("Aprovador");

            if (!String.IsNullOrEmpty(xml.AprovacaoForcada))
                crm.AprovacaoForcadoPedido = xml.AprovacaoForcada;
            else
                crm.AddNullProperty("AprovacaoForcadoPedido");

            if (xml.ValorMercadoriaAberto.HasValue)
                crm.ValorMercadoriaAberto = xml.ValorMercadoriaAberto;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor Mercadoria Aberto não enviado.";
                return crm;
            }

            if (xml.CondicaoFreteEntrega.HasValue)
                crm.CondicoesFreteEntrega = xml.CondicaoFreteEntrega;
            else
                crm.AddNullProperty("CondicoesFreteEntrega");

            if (xml.DataReativacaoUsuario.HasValue)
                crm.DataReativacaoUsuario = xml.DataReativacaoUsuario;
            else
                crm.AddNullProperty("DataReativacaoUsuario");

            if (xml.DataCancelamentoUsuario.HasValue)
                crm.DataCancelamentoUsuario = xml.DataCancelamentoUsuario;
            else
                crm.AddNullProperty("DataCancelamentoUsuario");

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.CaixaPostal))
                crm.EnderecoEntregaCaixaPostal = xml.EnderecoEntrega.CaixaPostal;
            else
                crm.AddNullProperty("EnderecoEntregaCaixaPostal");

            if (xml.ValorTotalSemFrete.HasValue)
                crm.ValorTotalProdutosSemIPI = xml.ValorTotalSemFrete;
            else
                crm.AddNullProperty("ValorTotalProdutosSemIPI");

            //alterado mapeamento para o campo do barramento "Observacao" - José - 05-08-2014
            //if (!String.IsNullOrEmpty(xml.Descricao))
            //    crm.Descricao = xml.Descricao;

            //Nat1
            if (!String.IsNullOrEmpty(xml.NaturezaOperacao))
            {
                NaturezaOperacao naturezaOperacao = new NaturezaOperacao(this.Organizacao, this.IsOffline);
                naturezaOperacao = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).BuscaNaturezaOperacaoPorCodigo(xml.NaturezaOperacao);

                if (naturezaOperacao != null)
                    crm.NaturezaOperacao = new Lookup(naturezaOperacao.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Natureza Operacao não encontrada.";
                    return crm;
                }
            }

            if (xml.PercentualDesconto2.HasValue)
                crm.PercentualDesconto2 = xml.PercentualDesconto2;
            else
                crm.AddNullProperty("PercentualDesconto2");

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.NomeContato))
                crm.NomeContatoEntrega = xml.EnderecoEntrega.NomeContato;
            else
                crm.AddNullProperty("NomeContatoEntrega");

            if (!String.IsNullOrEmpty(xml.Rota))
            {
                Rota rota = new Rota(this.Organizacao, this.IsOffline);
                rota = new Intelbras.CRM2013.Domain.Servicos.RotaService(this.Organizacao, this.IsOffline).BuscaRotaPorCodigo(xml.Rota);

                if (rota != null)
                    crm.Rota = new Lookup(rota.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Rota não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Rota");
            }

            if (!String.IsNullOrEmpty(xml.UsuarioSuspensao))
                crm.UsuarioSuspensao = xml.UsuarioSuspensao;
            else
                crm.AddNullProperty("UsuarioSuspensao");

            if (xml.ValorFrete.HasValue)
                crm.ValorFrete = xml.ValorFrete;
            else
                crm.AddNullProperty("ValorFrete");

            if (xml.DataAprovacao.HasValue)
                crm.DataAprovacao = xml.DataAprovacao;
            else
                crm.AddNullProperty("DataAprovacao");

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }

            if (xml.RetiraNoLocal.HasValue)
                crm.Remessa = xml.RetiraNoLocal.Value;
            else
                crm.AddNullProperty("Remessa");

            if (!String.IsNullOrEmpty(xml.DescricaoSuspensao))
                crm.DescricaoSuspensao = xml.DescricaoSuspensao;
            else
                crm.AddNullProperty("DescricaoSuspensao");

            crm.DataEntregaSolicitada = xml.DataEntregaSolicitada;

            crm.DataImplantacaoUsuario = xml.DataImplantacaoUsuario;

            if (xml.SituacaoAvaliacao.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.SituacaoAvaliacao), xml.SituacaoAvaliacao))
                    crm.CodigoSituacaoAvaliacao = xml.SituacaoAvaliacao.Value;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Situacao Avaliacao não encontrada!";
                    return crm;
                }

            }
            else
            {
                crm.AddNullProperty("CodigoSituacaoAvaliacao");
            }

            if (!String.IsNullOrEmpty(xml.TipoObjetoCliente) && !String.IsNullOrEmpty(xml.CodigoClienteCRM))
            {
                String tipoObjetoCliente;
                if (xml.TipoObjetoCliente == "account" || xml.TipoObjetoCliente == "contact")
                {
                    tipoObjetoCliente = xml.TipoObjetoCliente;
                    crm.ClienteID = new Lookup(new Guid(xml.CodigoClienteCRM), xml.TipoObjetoCliente);

                    Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(crm.ClienteID.Id);

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "TipoObjetoCliente ou CodigoCliente fora do padrão.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TipoObjetoCliente ou CodigoCliente não enviado.";
                return crm;
            }

            if (xml.Prioridade.HasValue)
                crm.Prioridade = xml.Prioridade;
            else
                crm.AddNullProperty("Prioridade");

            if (xml.Transportadora.HasValue)
            {
                Transportadora transportadora = new Transportadora(this.Organizacao, this.IsOffline);
                transportadora = new Intelbras.CRM2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPorCodigoTransportadora(xml.Transportadora.Value);
                if (transportadora != null)
                    crm.Transportadora = new Lookup(transportadora.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Transportadora não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Transportadora");
            }

            //if (!String.IsNullOrEmpty(xml.Oportunidade))
            //    crm.Oportunidade = new Lookup(new Guid(xml.Oportunidade), "");
            //else
            //    crm.AddNullProperty("Oportunidade");

            //Não obrigatorio
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Oportunidade não Enviada.";
            //    return crm;
            //}

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Bairro))
                crm.BairroEntrega = xml.EnderecoEntrega.Bairro;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Bairro não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CondicaoEspecial))
                crm.CondicoesEspeciais = xml.CondicaoEspecial;
            else
                crm.AddNullProperty("CondicoesEspeciais");

            //Removido Pollux
            //if (xml.TabelaPreco.HasValue)
            //{
            //    TabelaPreco tabelaPreco = new TabelaPreco(this.Organizacao, this.IsOffline);
            //    tabelaPreco = new Intelbras.CRM2013.Domain.Servicos.TabelaPrecoService(this.Organizacao, this.IsOffline).BuscaTabelaPrecoPorCodigo(xml.TabelaPreco.Value);
            //    if (tabelaPreco != null)
            //        crm.TabelaPreco = new Lookup(tabelaPreco.ID.Value, "");
            //}


            if (xml.DataAlteracao.HasValue)
                crm.DataAlteracao = xml.DataAlteracao;
            else
                crm.AddNullProperty("DataAlteracao");

            // Moeda - service
            if (!String.IsNullOrEmpty(xml.Moeda))
            {
                Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);
                moeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);

                if (moeda != null && moeda.ID.HasValue)
                {
                    crm.Moeda = new Lookup(moeda.ID.Value, "");
                    LookupMoeda = crm.Moeda;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda não encontrada!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Moeda não enviada.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.CEP))
                crm.CEPEntrega = xml.EnderecoEntrega.CEP;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CEP não enviado.";
                return crm;
            }

            ///TODO: Aguardar modelagem - Camapanha - rollout 4
            //if (!String.IsNullOrEmpty(xml.CampanhaOrigem))
            // crm.CampanhaID = xml.CampanhaOrigem;

            if (xml.Mensagem.HasValue)
            {
                Mensagem mensagem = new Mensagem(this.Organizacao, this.IsOffline);
                mensagem = new Intelbras.CRM2013.Domain.Servicos.MensagemService(this.Organizacao, this.IsOffline).BuscaMensagemPorCodigo(xml.Mensagem.Value);
                if (mensagem != null)
                    crm.Mensagem = new Lookup(mensagem.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Mensagem do pedido não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Mensagem");
            }

            if (xml.PrecoBloqueado.HasValue)
                crm.PrecoBloqueado = xml.PrecoBloqueado;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Preco Bloqueado não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.NumeroPedido))
            {
                crm.PedidoEMS = xml.NumeroPedido;
                crm.IDPedido = xml.NumeroPedido;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NumeroPedido não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Logradouro))
                crm.EnderecoEntregaRua = xml.EnderecoEntrega.Logradouro;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Logradouro não enviado.";
                return crm;
            }
            if (xml.PedidoCompleto.HasValue)
                crm.Completo = xml.PedidoCompleto;
            else
                crm.AddNullProperty("Completo");

            if (xml.DataLimiteFaturamento.HasValue)
                crm.DataLimiteFaturamento = xml.DataLimiteFaturamento;
            else
                crm.AddNullProperty("DataLimiteFaturamento");

            if (!String.IsNullOrEmpty(xml.CodigoEntrega))
                crm.CodigoEntrega = xml.CodigoEntrega;
            else
                crm.AddNullProperty("CodigoEndereco");

            if (!String.IsNullOrEmpty(xml.NomeAbreviadoCliente))
                crm.NomeAbreviado = xml.NomeAbreviadoCliente;
            else
                crm.AddNullProperty("NomeAbreviado");

            if (!String.IsNullOrEmpty(xml.ClienteTriangular))
                crm.ClienteTriangular = new Lookup(new Guid(xml.ClienteTriangular), "account");
            else
                crm.AddNullProperty("ClienteTriangular");

            if (xml.DiasNegociacao.HasValue)
                crm.DiasNegociacao = xml.DiasNegociacao;
            else
                crm.AddNullProperty("DiasNegociacao");

            if (xml.DataNegociacao.HasValue)
                crm.DataNegociacao = xml.DataNegociacao;
            else
                crm.AddNullProperty("DataNegociacao");

            if (xml.DataCumprimento.HasValue)
                crm.DataCumprimento = xml.DataCumprimento;
            else
                crm.AddNullProperty("DataCumprimento");

            if (!String.IsNullOrEmpty(xml.Classificacao))
                crm.Classificacao = new Lookup(new Guid(xml.Classificacao), "");
            else
                crm.AddNullProperty("Classificacao");

            if (!String.IsNullOrEmpty(xml.DescricaoCancelamento))
                crm.DescricaoCancelamento = xml.DescricaoCancelamento;
            else
                crm.AddNullProperty("DescricaoCancelamento");

            if (xml.OrigemPedido.HasValue)
            {
                if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.OrigemPedido), xml.OrigemPedido.Value))
                    crm.Origem = xml.OrigemPedido.Value;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Origem Pedido não encontrado!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Origem");
            }

            if (xml.DataReativacao.HasValue)
                crm.DataReativacao = xml.DataReativacao;
            else
                crm.AddNullProperty("DataReativacao");

            if (xml.ValorCreditoLiberado.HasValue)
                crm.ValorCreditoLiberado = xml.ValorCreditoLiberado;
            else
                crm.AddNullProperty("ValorCreditoLiberado");

            //Nao preencher tabelaprecoEMS - orientado por Jose.

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Complemento))
                crm.ComplementoEntrega = xml.EnderecoEntrega.Complemento;
            else
                crm.AddNullProperty("ComplementoEntrega");

            if (xml.TotalIPI.HasValue)
                crm.TotalIPI = xml.TotalIPI;
            else
                crm.AddNullProperty("TotalIPI");

            if (xml.DataMinimaFaturamento.HasValue)
                crm.DataMinimaFaturamento = xml.DataMinimaFaturamento;
            else
                crm.AddNullProperty("DataMinimaFaturamento");

            if (xml.DataSuspensao.HasValue)
                crm.DataSuspensao = xml.DataSuspensao;
            else
                crm.AddNullProperty("DataSuspensao");

            if (xml.PercentualDesconto1.HasValue)
                crm.PercentualDesconto1 = xml.PercentualDesconto1;
            else
                crm.AddNullProperty("PercentualDesconto1");


            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Cidade))
            {
                Model.Municipio cidade = new Model.Municipio(this.Organizacao, this.IsOffline);
                cidade = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(xml.EnderecoEntrega.Cidade);

                if (cidade != null && cidade.ID.HasValue)
                    crm.EnderecoEntregaCidade = new Lookup(cidade.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Cidade não encontrada!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Município não enviado.";
                return crm;
            }


            ///TODO: Aguardar modelagem - Cotacao - Rollout 4
            //if (!String.IsNullOrEmpty(xml.Cotacao))
            //    crm.Cotacao = xml.Cotacao;

            if (xml.ValorTotalDesconto.HasValue)
                crm.DescontoGlobalAdicional = xml.ValorTotalDesconto;
            else
                crm.AddNullProperty("DescontoGlobalAdicional");

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Fax))
                crm.FaxEntrega = xml.EnderecoEntrega.Fax;
            else
                crm.AddNullProperty("FaxEntrega");

            if (xml.DataEntrega.HasValue)
                crm.DataEntrega = xml.DataEntrega;
            else
                crm.AddNullProperty("DataEntrega");

            // País
            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Pais))
            {
                Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);
                pais = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.EnderecoEntrega.Pais);

                if (pais != null && pais.ID.HasValue)
                    crm.EnderecoEntregaPais = new Lookup(pais.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "País não encontrado.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "País não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.PedidoOriginal) && (!xml.PedidoOriginal.Equals("0")))
            {
                Pedido pedidoOrig = new Pedido(this.Organizacao, this.IsOffline);
                pedidoOrig = new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(xml.PedidoOriginal);
                if (pedidoOrig != null)
                    crm.PedidoOriginal = new Lookup(pedidoOrig.ID.Value, "");
                else //Vai ignorar caso não encontre???
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Pedido Original não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("PedidoOriginal");
            }

            if (!String.IsNullOrEmpty(xml.EnderecoEntrega.Numero))
                crm.EnderecoEntregaNumero = xml.EnderecoEntrega.Numero;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Numero endereco do pedido não enviado.";
                return crm;
            }

            if (xml.Portador.HasValue)
            {
                Portador portador = new Portador(this.Organizacao, this.IsOffline);
                portador = new Intelbras.CRM2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.Portador.Value);
                if (portador != null)
                    crm.Portador = new Lookup(portador.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Portador do pedido não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Portador");
            }

            crm.DataEntregaOriginal = xml.DataEntregaSolicitada;

            if (!String.IsNullOrEmpty(xml.TipoPedido))
                crm.TipoPedido = xml.TipoPedido;
            else
                crm.AddNullProperty("TipoPedido");

            if (xml.PercentualDesconto.HasValue)
                crm.DescontoGlobalAdicional = xml.PercentualDesconto;
            else
                crm.AddNullProperty("DescontoGlobalAdicional");

            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.StateCode), xml.Situacao))
                crm.Status = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situação não encontrado.";
                return crm;
            }


            if (System.Enum.IsDefined(typeof(Enum.Pedido.RazaoStatus), xml.SituacaoPedido))
                crm.RazaoStatus = xml.SituacaoPedido;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "SituacaoPedido não encontrado.";
                return crm;
            }


            if (!String.IsNullOrEmpty(xml.NumeroPedido))
            {
                Pedido pedido = new Pedido(this.Organizacao, this.IsOffline);
                pedido = new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(xml.NumeroPedido);
                if (pedido != null)
                    crm.ID = pedido.ID;


            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NumeroPedido não enviado.";
                return crm;
            }
            // ListaPreco
            if (!String.IsNullOrEmpty(xml.ListaPreco))
            {
                Model.ListaPreco listaPreco = new Model.ListaPreco(this.Organizacao, this.IsOffline);
                listaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(xml.ListaPreco);

                if (listaPreco != null && listaPreco.ID.HasValue)
                    crm.ListaPreco = new Lookup(listaPreco.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "ListaPreco não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "ListaPreco não enviada.";
                return crm;
            }

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.NomeUsuarioCriacao = xml.NomeUsuarioCriacao;
            crm.TipoUsuarioCriacao = xml.TipoUsuarioCriacao;
            crm.SupervisorOriginal = xml.CodigoSupervisorEMS;
            crm.AssistenteOriginal = xml.CodigoAssistente.ToString();
            crm.RepresentanteOriginal = xml.Representante.ToString();
            crm.IntegradoSupervisorComErro = false;
            crm.IntegradoAssistenteComErro = false;
            crm.IntegradoRepresentanteComErro = false;
            crm.IntegradoComErros = false;

            #region Representante, Assistente e Supervisor

            Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline)
                .BuscaContatoPorCodigoRepresentante(xml.Representante.ToString());

            if (contato != null)
            {
                crm.KeyAccountRepresentante = new Lookup(contato.ID.Value, "");
            }
            else
            {
                crm.KeyAccountRepresentante = new Lookup(ObterRepresentatePadrao(), "");
                crm.IntegradoComErros = true;
                crm.IntegradoRepresentanteComErro = true;
                crm.RepresentanteOriginal = xml.Representante.ToString();
            }

            if (!string.IsNullOrEmpty(xml.CodigoSupervisorEMS))
            {
                Usuario userSuper = new Servicos.UsuarioService(this.Organizacao, this.IsOffline)
                    .BuscaPorCodigoSupervisorEMS(xml.CodigoSupervisorEMS);

                if (userSuper != null)
                {
                    crm.Supervisor = new Lookup(userSuper.ID.Value, "");
                }
                else
                {
                    crm.IntegradoComErros = true;
                    crm.IntegradoSupervisorComErro = true;
                    crm.SupervisorOriginal = xml.CodigoSupervisorEMS;
                }
            }

            Usuario userAssistente = new Servicos.UsuarioService(this.Organizacao, this.IsOffline)
                .BuscaPorCodigoAssistente(xml.CodigoAssistente.Value);

            if (userAssistente != null)
            {
                crm.Assistente = new Lookup(userAssistente.ID.Value, "");
            }
            else
            {
                crm.IntegradoComErros = true;
                crm.IntegradoAssistenteComErro = true;
                crm.AssistenteOriginal = xml.CodigoAssistente.ToString();
            }

            if (crm.IntegradoSupervisorComErro.Value || crm.IntegradoAssistenteComErro.Value)
            {
                var unidadeNegocio = ObterUnidadeNegocioPedido(xml);

                if (unidadeNegocio != null)
                {
                    var portfolioKaRepresentante = new PotencialdoKARepresentanteService(this.Organizacao, this.IsOffline)
                                                   .ObterPor(unidadeNegocio.ID.Value, crm.KeyAccountRepresentante.Id);

                    if (portfolioKaRepresentante != null)
                    {
                        if (crm.IntegradoSupervisorComErro.Value && portfolioKaRepresentante.SupervisordeVendas != null)
                        {
                            crm.Supervisor = portfolioKaRepresentante.SupervisordeVendas;
                        }

                        if (crm.IntegradoAssistenteComErro.Value && portfolioKaRepresentante.AssistentedeAdministracaodeVendas != null)
                        {
                            crm.Assistente = portfolioKaRepresentante.AssistentedeAdministracaodeVendas;
                        }
                    }
                }
            }

            #endregion

            return crm;
        }

        private List<ProdutoPedido> DefinirPropriedadesProdutoPedido(Intelbras.Message.Helper.MSG0091 xml, Guid PedidoID)
        {
            List<ProdutoPedido> lstProdutoPedido = new List<ProdutoPedido>();
            ProdutoPedido crmProdPedido = null;

            foreach (Pollux.Entities.PedidoItem item in xml.PedidosItens)
            {
                crmProdPedido = new ProdutoPedido(this.Organizacao, this.IsOffline);

                // A - E - C
                if (!String.IsNullOrEmpty(item.Acao)
                    &&
                    (item.Acao.ToUpper().Equals("A")
                    || item.Acao.ToUpper().Equals("E")
                    || item.Acao.ToUpper().Equals("C")
                    || item.Acao.ToUpper().Equals("I")))
                    crmProdPedido.Acao = item.Acao;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Ação não enviada para o item do pedido.";
                    lstProdutoPedido = null;
                    break;
                }

                // Recebe o ID do Pedido recem incluso no CRM

                crmProdPedido.IntegradoEm = DateTime.Now;
                crmProdPedido.IntegradoPor = usuarioIntegracao.NomeCompleto;
                crmProdPedido.UsuarioIntegracao = xml.LoginUsuario;

                crmProdPedido.Pedido = new Lookup(PedidoID, "");

                if (!String.IsNullOrEmpty(item.ChaveIntegracao))
                    crmProdPedido.ChaveIntegracao = item.ChaveIntegracao;

                if (!String.IsNullOrEmpty(item.DescricaoItemPedido))
                {
                    crmProdPedido.Descricao = item.DescricaoItemPedido;
                    ///TODO: aguardar inclusão campo DescricaoId - Pollux
                    //crmProdPedido.Descricaoid = item.DescricaoItemPedido;
                }
                else
                {
                    crmProdPedido.AddNullProperty("Descricao");
                }

                if (item.TaxaCambio.HasValue)
                    crmProdPedido.TaxaCambio = item.TaxaCambio;
                else
                    crmProdPedido.AddNullProperty("TaxaCambio");


                crmProdPedido.ValorLiquidoSemIpiSt = item.PrecoNegociado;

                crmProdPedido.Precificacao = item.PermiteSubstituirPreco;
                crmProdPedido.SelecionarProduto = item.ProdutoForaCatalogo;

                if (item.AliquotaIPI.HasValue)
                    crmProdPedido.AliquotaIPI = item.AliquotaIPI.Value;
                else
                    crmProdPedido.AddNullProperty("AliquotaIPI");

                if (item.DataAlteracao.HasValue)
                    crmProdPedido.DataAlteracao = item.DataAlteracao.Value;
                else
                    crmProdPedido.AddNullProperty("DataAlteracao");

                if (item.DataCancelamentoUsuario.HasValue)
                    crmProdPedido.DataCancelamentoUsuario = item.DataCancelamentoUsuario;
                else
                    crmProdPedido.AddNullProperty("DataCancelamentoUsuario");

                if (item.DataCancelamentoSequencia.HasValue)
                    crmProdPedido.DataCancelamentoSequencia = item.DataCancelamentoSequencia;
                else
                    crmProdPedido.AddNullProperty("DataCancelamentoSequencia");

                if (item.DataDevolucao.HasValue)
                    crmProdPedido.DataDevolucao = item.DataDevolucao;
                else
                    crmProdPedido.AddNullProperty("DataDevolucao");

                if (item.DataDevolucaoUsuario.HasValue)
                    crmProdPedido.DataDevolucaoUsuario = item.DataDevolucaoUsuario;
                else
                    crmProdPedido.AddNullProperty("DataDevolucaoUsuario");

                crmProdPedido.DataEntrega = item.DataEntrega;

                crmProdPedido.DataEntregaOriginal = item.DataEntregaSolicitada;

                crmProdPedido.DataImplantacao = item.DataImplantacao;

                crmProdPedido.UsuarioImplantacao = item.UsuarioImplantacao;

                crmProdPedido.CalcularRebate = item.CalcularRebate;

                if (item.DataMaximaFaturamento.HasValue)
                    crmProdPedido.DataMaximaFaturamento = item.DataMaximaFaturamento;
                else
                    crmProdPedido.AddNullProperty("DataMaximaFaturamento");

                if (item.DataMinimaFaturamento.HasValue)
                    crmProdPedido.DataMinimaFaturamento = item.DataMinimaFaturamento;
                else
                    crmProdPedido.AddNullProperty("DataMinimaFaturamento");

                if (item.DataReativacao.HasValue)
                    crmProdPedido.DataReativacao = item.DataReativacao;
                else
                    crmProdPedido.AddNullProperty("DataReativacao");

                if (item.DataReativacaoUsuario.HasValue)
                    crmProdPedido.DataReativacaoUsuario = item.DataReativacaoUsuario;
                else
                    crmProdPedido.AddNullProperty("DataReativacaoUsuario");

                if (item.DataSuspensao.HasValue)
                    crmProdPedido.DataSuspensao = item.DataSuspensao;
                else
                    crmProdPedido.AddNullProperty("DataSuspensao");

                if (item.DataSuspensaoUsuario.HasValue)
                    crmProdPedido.DataSuspensaoUsuario = item.DataSuspensaoUsuario;
                else
                    crmProdPedido.AddNullProperty("DataSuspensaoUsuario");

                if (!String.IsNullOrEmpty(item.DescricaoCancelamento))
                    crmProdPedido.DescricaoCancelamento = item.DescricaoCancelamento;
                else
                    crmProdPedido.AddNullProperty("DescricaoCancelamento");

                if (!String.IsNullOrEmpty(item.DescricaoDevolucao))
                    crmProdPedido.DescricaoDevolucao = item.DescricaoDevolucao;
                else
                    crmProdPedido.AddNullProperty("DescricaoDevolucao");

                if (item.FaturaQuantidadeFamilia.HasValue)
                    crmProdPedido.FaturaQtdeFamilia = item.FaturaQuantidadeFamilia.Value;
                else
                    crmProdPedido.AddNullProperty("FaturaQtdeFamilia");

                if (item.CalcularRebate.HasValue)
                {
                    //TODO: Criar campo de CalcularRebate e gravar valor recebido.
                }

                #region Representante | Validação antiga

                crmProdPedido.RepresentanteOriginal = item.Representante.ToString();

                Contato contato = listaContatosCRM.Find(x => x.CodigoRepresentante == item.Representante.ToString());

                if (contato != null)
                {
                    crmProdPedido.Representante = new Lookup(contato.ID.Value, "");
                    crmProdPedido.IntegradoRepresentanteComErro = false;
                }
                else
                {
                    crmProdPedido.Representante = new Lookup(ObterRepresentatePadrao(), "");
                    crmProdPedido.IntegradoRepresentanteComErro = true;
                }

                #endregion

                #region Natureza Operação | Validação antiga

                //Nat2 - Itempedido
                if (!String.IsNullOrEmpty(item.NaturezaOperacao))
                {
                    NaturezaOperacao naturezaOperacaoItem = new NaturezaOperacao(this.Organizacao, this.IsOffline);
                    naturezaOperacaoItem = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).ExtrairNaturezaOperacaoPorCodigo(item.NaturezaOperacao, listaNaturezaOperacaoCRM);
                    if (naturezaOperacaoItem != null)
                        crmProdPedido.NaturezaOperacao = new Lookup(naturezaOperacaoItem.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "NaturezaOperacao do item do pedido não encontrado.";
                        lstProdutoPedido = null;
                        break;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "NaturezaOperacao do item do pedido não enviado.";
                    lstProdutoPedido = null;
                    break;
                }

                #endregion

                if (!String.IsNullOrEmpty(item.NomeAbreviadoCliente))
                    crmProdPedido.NomeAbreviado = item.NomeAbreviadoCliente;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "NomeAbreviado do item do pedido não enviado.";
                    lstProdutoPedido = null;
                    break;
                }

                if (!String.IsNullOrEmpty(item.Observacao))
                    crmProdPedido.Observacao = item.Observacao;
                else
                    crmProdPedido.AddNullProperty("Observacao");

                if (!String.IsNullOrEmpty(item.NumeroPedidoCliente))
                    crmProdPedido.PedidoCliente = item.NumeroPedidoCliente;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "NumeroPedidoCliente do item do pedido não enviado.";
                    lstProdutoPedido = null;
                    break;
                }

                if (item.PercentualDescontoICMS.HasValue)
                    crmProdPedido.PercentualDescontoICMS = item.PercentualDescontoICMS;
                else
                    crmProdPedido.AddNullProperty("PercentualDescontoICMS");

                if (item.PercentualMinimoFaturamento.HasValue)
                    crmProdPedido.PercentualMinimoFaturamento = item.PercentualMinimoFaturamento;
                else
                    crmProdPedido.AddNullProperty("PercentualMinimoFaturamento");

                crmProdPedido.PrecoMinimo = item.PrecoMinimo;

                crmProdPedido.PrecoNegociado = item.PrecoNegociado;

                if (item.QuantidadeAlocada.HasValue)
                    crmProdPedido.QtdeAlocada = item.QuantidadeAlocada;
                else
                    crmProdPedido.AddNullProperty("QtdeAlocada");

                if (item.QuantidadeAlocadaLogica.HasValue)
                    crmProdPedido.QtdeAlocadaLogica = item.QuantidadeAlocadaLogica;
                else
                    crmProdPedido.AddNullProperty("QtdeAlocadaLogica");

                if (item.QuantidadeDevolvida.HasValue)
                    crmProdPedido.QtdeDevolvida = item.QuantidadeDevolvida;
                else
                    crmProdPedido.AddNullProperty("QtdeDevolvida");

                if (item.RetemICMSFonte.HasValue)
                    crmProdPedido.RetemICMSFonte = item.RetemICMSFonte.Value;
                else
                    crmProdPedido.AddNullProperty("RetemICMSFonte");

                if (item.SituacaoAlocacao.HasValue)
                {
                    if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.SituacaoAlocacao), item.SituacaoAlocacao))
                        crmProdPedido.SituacaoAlocacao = item.SituacaoAlocacao;
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Situacao Alocacao Item Pedido não encontrado!";
                        lstProdutoPedido = null;
                        break;
                    }

                }
                else
                {
                    crmProdPedido.AddNullProperty("SituacaoAlocacao");
                }

                if (item.SituacaoItem.HasValue)
                    crmProdPedido.SituacaoItem = item.SituacaoItem.Value;
                else
                    crmProdPedido.AddNullProperty("SituacaoItem");

                //Removido Pollux
                //if (item.TabelaPreco.HasValue)
                //{
                //    TabelaPreco tabelaPreco = new TabelaPreco(this.Organizacao, this.IsOffline);
                //    tabelaPreco = new Intelbras.CRM2013.Domain.Servicos.TabelaPrecoService(this.Organizacao, this.IsOffline).BuscaTabelaPrecoPorCodigo(item.TabelaPreco.Value);
                //    if (tabelaPreco != null)
                //        crmProdPedido.TabelaPreco = new Lookup(tabelaPreco.ID.Value, "");
                //    else
                //    {
                //        resultadoPersistencia.Sucesso = false;
                //        resultadoPersistencia.Mensagem = "TabelaPreco do item do pedido não encontrado.";
                //        lstProdutoPedido = null;
                //        break;
                //    }
                //}


                if (item.TipoPreco.HasValue)
                {
                    if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.TipoPreco), item.TipoPreco))
                        crmProdPedido.TipoPreco = item.TipoPreco;
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Tipo Preço não encontrado!";
                        lstProdutoPedido = null;
                        break;
                    }
                }
                else
                {
                    crmProdPedido.AddNullProperty("TipoPreco");
                }

                #region Unidade Negócio | Validação Antiga - em melhoria

                if (!String.IsNullOrEmpty(item.UnidadeNegocio))
                {
                    Model.UnidadeNegocio unidadeNegocio = new UnidadeNegocio(this.Organizacao, this.IsOffline);
                    unidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).ExtrairUnidadeNegocioPorChaveIntegracao(item.UnidadeNegocio, listaUnidadesNegocioCRM);

                    if (unidadeNegocio != null)
                        crmProdPedido.UnidadeNegocio = new Lookup(unidadeNegocio.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "UnidadeNegocio do item do pedido não encontrado.";
                        lstProdutoPedido = null;
                        break;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio do item do pedido não encontrado.";
                    lstProdutoPedido = null;
                    break;

                }

                #endregion

                if (!String.IsNullOrEmpty(item.UsuarioAlteracao))
                    crmProdPedido.UsuarioAlteracao = item.UsuarioAlteracao;
                else
                    crmProdPedido.AddNullProperty("UsuarioAlteracao");

                if (!String.IsNullOrEmpty(item.UsuarioCancelamento))
                    crmProdPedido.UsuarioCancelamento = item.UsuarioCancelamento;
                else
                    crmProdPedido.AddNullProperty("UsuarioCancelamento");

                if (!String.IsNullOrEmpty(item.UsuarioDevolucao))
                    crmProdPedido.UsuarioDevolucao = item.UsuarioDevolucao;
                else
                    crmProdPedido.AddNullProperty("UsuarioDevolucao");

                //if (!String.IsNullOrEmpty(item.UsuarioImplantacao))
                crmProdPedido.UsuarioImplantacao = item.UsuarioImplantacao;

                if (!String.IsNullOrEmpty(item.UsuarioReativacao))
                    crmProdPedido.UsuarioReativacao = item.UsuarioReativacao;
                else
                    crmProdPedido.AddNullProperty("UsuarioReativacao");

                if (!String.IsNullOrEmpty(item.UsuarioSuspensao))
                    crmProdPedido.UsuarioSuspensao = item.UsuarioSuspensao;
                else
                    crmProdPedido.AddNullProperty("UsuarioSuspensao");

                crmProdPedido.ValorLiquidoAberto = item.ValorLiquidoAberto;
                crmProdPedido.ValorLiquidoItem = item.ValorLiquido;
                crmProdPedido.ValorMercadoriaAberto = item.ValorMercadoriaAberto;
                crmProdPedido.ValorOriginal = item.ValorOriginal;
                crmProdPedido.ValorTabela = item.ValorTabela;
                crmProdPedido.ValorTotalItem = item.ValorTotal;
                crmProdPedido.ValorSubstTributaria = item.ValorSubstituicaoTributaria;
                crmProdPedido.ValorIPI = item.ValorIPI;
                crmProdPedido.DescontoManual = item.DescontoManual;

                if (!String.IsNullOrEmpty(item.DescricaoProdutoForaCatalogo))
                    crmProdPedido.ProdutoForaCatalogo = item.DescricaoProdutoForaCatalogo;
                else
                    crmProdPedido.AddNullProperty("ProdutoForaCatalogo");

                #region Produto | Validação antiga - em melhoria

                Product produto = new Product(this.Organizacao, this.IsOffline);
                produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ExtrairPorCodigo(item.Produto, listaProdutosCRM);
                if (produto != null)
                    crmProdPedido.Produto = new Lookup(produto.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Produto : " + item.Produto + " não cadastrado no Crm.";
                    lstProdutoPedido = null;
                    break;
                }

                #endregion

                crmProdPedido.Quantidade = item.QuantidadePedida;

                if (item.QuantidadePendente.HasValue)
                    crmProdPedido.QtdePedidoPendente = item.QuantidadePendente;
                else
                    crmProdPedido.AddNullProperty("QtdePedidoPendente");

                if (item.QuantidadeCancelada.HasValue)
                    crmProdPedido.QtdeCancelada = item.QuantidadeCancelada;
                else
                    crmProdPedido.AddNullProperty("QtdeCancelada");

                if (item.QuantidadeEntregue.HasValue)
                    crmProdPedido.QtdeEntregue = item.QuantidadeEntregue;
                else
                    crmProdPedido.AddNullProperty("QtdeEntregue");

                crmProdPedido.DateEntregaSolicitada = item.DataEntregaSolicitada;

                if (item.Sequencia.HasValue)
                    crmProdPedido.NumeroSequencia = item.Sequencia;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.NomeCidade))
                    crmProdPedido.CidadeEntrega = item.EnderecoEntrega.NomeCidade;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.NomeContato))
                    crmProdPedido.NomeContatoEntrega = item.EnderecoEntrega.NomeContato;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.NomePais))
                    crmProdPedido.PaisEntrega = item.EnderecoEntrega.NomePais;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Fax))
                    crmProdPedido.FAXEntrega = item.EnderecoEntrega.Fax;
                else
                    crmProdPedido.AddNullProperty("FAXEntrega");

                if (item.CondicaoFrete.HasValue)
                {
                    if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Pedido.CondicoesFrete), item.CondicaoFrete))
                        crmProdPedido.CondicoesFrete = item.CondicaoFrete;
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Condições de frete do item do pedido não encontrado!";
                        lstProdutoPedido = null;
                        break;
                    }
                }
                else
                {
                    crmProdPedido.AddNullProperty("CondicoesFrete");
                }
                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Logradouro))
                    crmProdPedido.RuaEntrega = item.EnderecoEntrega.Logradouro;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Bairro))
                    crmProdPedido.BairroEntrega = item.EnderecoEntrega.Bairro;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Complemento))
                    crmProdPedido.ComplementoEntrega = item.EnderecoEntrega.Complemento;
                else
                    crmProdPedido.AddNullProperty("ComplementoEntrega");

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.NomeEndereco))
                    crmProdPedido.NomeEntrega = item.EnderecoEntrega.NomeEndereco;
                else
                    crmProdPedido.AddNullProperty("NomeEntrega");

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.CEP))
                    crmProdPedido.CEPEntrega = item.EnderecoEntrega.CEP;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.UF))
                    crmProdPedido.EstadoEntrega = item.EnderecoEntrega.UF;

                if (!String.IsNullOrEmpty(item.EnderecoEntrega.Telefone))
                    crmProdPedido.TelefoneEntrega = item.EnderecoEntrega.Telefone;
                else
                    crmProdPedido.AddNullProperty("TelefoneEntrega");

                crmProdPedido.TotalImposto = item.ValorTotalImposto;

                #region Moeda - Tratamento novo baseado no pedido

                // Moeda - service
                if (!String.IsNullOrEmpty(item.Moeda))
                {
                    Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);

                    if (LookupMoeda != null)
                        crmProdPedido.Moeda = LookupMoeda;
                    else
                    {
                        moeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(item.Moeda);

                        if (moeda != null && moeda.ID.HasValue)
                            crmProdPedido.Moeda = new Lookup(moeda.ID.Value, "");
                        else
                        {
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "Moeda do item do pedido não encontrado.";
                            lstProdutoPedido = null;
                            break;
                        }
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda do item do pedido não enviada.";
                    lstProdutoPedido = null;
                    break;
                }

                #endregion

                #region Unidade Medida | Validação antiga - em melhoria

                if (!String.IsNullOrEmpty(item.UnidadeMedida))
                {
                    Unidade unidade = new Unidade(this.Organizacao, this.IsOffline);
                    unidade = new Intelbras.CRM2013.Domain.Servicos.UnidadeService(this.Organizacao, this.IsOffline).ExtrairUnidadePorNome(item.UnidadeMedida, listaUnidadesMedidaCRM);
                    if (unidade != null)
                        crmProdPedido.Unidade = new Lookup(unidade.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "UnidadeMedida do item do pedido não encontrada.";
                        lstProdutoPedido = null;
                        break;
                    }

                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeMedida do item do pedido não enviada.";
                    lstProdutoPedido = null;
                    break;
                }

                #endregion

                if (item.RetiraNoLocal.HasValue)
                    crmProdPedido.Remessa = item.RetiraNoLocal.Value;
                else
                    crmProdPedido.AddNullProperty("Remessa");

                lstProdutoPedido.Add(crmProdPedido);

            }

            return lstProdutoPedido;
        }

        #endregion

        public string Enviar(Pedido objModel)
        {
            throw new NotImplementedException();
        }

        #region Métodos Privados
        private String[] ExtrairArrays(IEnumerable<IGrouping<String, object>> itensSemDuplicidade)
        {
            return itensSemDuplicidade.Select(x => x.Key).ToArray();
        }
        #endregion
    }
}