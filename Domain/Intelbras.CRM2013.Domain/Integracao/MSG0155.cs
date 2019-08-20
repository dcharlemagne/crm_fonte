using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0155 : Base, IBase<Message.Helper.MSG0155, Domain.Model.SolicitacaoBeneficio>
    {
        //PriceProtection

        public MSG0155(string org, bool isOffline) : base(org, isOffline) { }

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

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
            usuarioIntegracao = usuario;
                var objPollux = CarregarMensagem<Pollux.MSG0155>(mensagem);
                var objeto = DefinirPropriedades(objPollux);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0155R1>(numeroMensagem, retorno);
            }

                objeto.IntegrarNoPlugin = (objPollux.IdentidadeEmissor != Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.EMS))
                                          ? false
                                          : true;

                List<ProdutosdaSolicitacao> lstProdutoSolicitacao = DefinirPropriedadesItens(objPollux.ProdutoSolicitacaoItens, objPollux.IdentidadeEmissor, objeto.ID);
                objeto.ID = new SolicitacaoBeneficioService(Organizacao, IsOffline).PersistirMensagemIntegracao(objeto, lstProdutoSolicitacao);


                new TarefaService(Organizacao, IsOffline).AtualizarTarefaNaoAjusteManual(objeto);
                //new SolicitacaoBeneficioService(Organizacao, IsOffline).Avanca(objeto);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("CodigoSolicitacaoBeneficio", objeto.ID.ToString());
                retorno.Add("Proprietario", usuario.ID.Value.ToString());
                retorno.Add("TipoProprietario", "systemuser");
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0155R1>(numeroMensagem, retorno);
                    }
            catch (Exception e)
                    {
                                    resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                                    retorno.Add("Resultado", resultadoPersistencia);
                                    return CriarMensagemRetorno<Pollux.MSG0155R1>(numeroMensagem, retorno);
                                }
                            }

        private void AtualizarProdutos(Pollux.MSG0155 objPollux, Guid? solicitacaoID)
                            {
            string origem = "";
            if (!string.IsNullOrEmpty(objPollux.IdentidadeEmissor))
                origem = objPollux.IdentidadeEmissor;

            List<ProdutosdaSolicitacao> lstProdutoSolicitacao = DefinirPropriedadesItens(objPollux.ProdutoSolicitacaoItens, origem, solicitacaoID);

            if (lstProdutoSolicitacao == null)
                {
                new SolicitacaoBeneficioService(Organizacao, IsOffline).Deletar(solicitacaoID.Value);

                throw new ArgumentException("(CRM) Erro de Persistência!");
            }

            new ProdutosdaSolicitacaoService(Organizacao, IsOffline).Persistir(lstProdutoSolicitacao);
        }

        #region Definir Propriedades

        public SolicitacaoBeneficio DefinirPropriedades(Pollux.MSG0155 xml)
        {
            var crm = new SolicitacaoBeneficio(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.IntegrarNoPlugin = true;
            crm.ValorAprovado = xml.ValorAprovado;


            if (!string.IsNullOrEmpty(xml.CodigoSolicitacaoBeneficio))
            {
                if (xml.CodigoSolicitacaoBeneficio.Length == 36)
                    crm.ID = new Guid(xml.CodigoSolicitacaoBeneficio);
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoSolicitacaoBeneficio fora do padrão (Guid)!";
                    return crm;
                }
            }

            string NomeUnidadeNegocio = String.Empty;

            if (!string.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (unidadeNegocio == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio: " + xml.CodigoUnidadeNegocio + " não encontrado no Crm.";
                    return crm;
                }
                else
                {
                    crm.UnidadedeNegocio = new Lookup(unidadeNegocio.ID.Value, "");
                    NomeUnidadeNegocio = unidadeNegocio.Nome;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoUnidadeNegocio não enviado!";
                return crm;
            }
            

            crm.Nome = xml.NomeSolicitacaoBeneficio;
            crm.TipoPriceProtection = xml.TipoPriceProtection;

            if (!String.IsNullOrEmpty(xml.DescricaoSituacaoIrregular))
                crm.SituacaoIrregular = xml.DescricaoSituacaoIrregular;
            else
                crm.AddNullProperty("SituacaoIrregular");

            if (!String.IsNullOrEmpty(xml.CodigoBeneficioCanal) && xml.CodigoBeneficioCanal.Length == 36)
            {
                BeneficioDoCanal beneficioCanal = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoBeneficioCanal));
                if (beneficioCanal != null)
                    crm.BeneficioCanal = new Lookup(beneficioCanal.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoBeneficioCanal : " + xml.CodigoBeneficioCanal + " - não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoBeneficioCanal não Enviado ou fora do padrão(Guid).";
                return crm;
            }

            crm.ValorSolicitado = xml.ValorSolicitado;

            if (!string.IsNullOrEmpty(xml.DescricaoSolicitacao))
                crm.Descricao = xml.DescricaoSolicitacao;
            else
                crm.AddNullProperty("Descricao");

            if (System.Enum.IsDefined(typeof(Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio), xml.SituacaoSolicitacaoBeneficio))
            {
                crm.StatusSolicitacao = xml.SituacaoSolicitacaoBeneficio;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "SituacaoSolicitacaoBeneficio não cadastrado no Crm(PickList).";
                return crm;
            }
            if (xml.Situacao == 0 || xml.Situacao == 1)
                crm.State = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situacao fora do padrão(0 ou 1).";
                return crm;
            }


            if (crm.State.Value.Equals((int)Enum.SolicitacaoBeneficio.State.Ativo))
            {
                if (System.Enum.IsDefined(typeof(Enum.SolicitacaoBeneficio.RazaoStatusAtivo), xml.RazaoStatusSolicitacaoBeneficio))
                {
                    crm.Status = xml.RazaoStatusSolicitacaoBeneficio;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "RazaoStatusSolicitacaoBeneficio não cadastrado para Situação Ativo.";
                    return crm;
                }
            }
            else if (crm.State.Value.Equals((int)Enum.SolicitacaoBeneficio.State.Inativo))
            {
                if (System.Enum.IsDefined(typeof(Enum.SolicitacaoBeneficio.RazaoStatusInativo), xml.RazaoStatusSolicitacaoBeneficio))
                {
                    crm.Status = xml.RazaoStatusSolicitacaoBeneficio;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "RazaoStatusSolicitacaoBeneficio não cadastrado para Situação Inativo.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situacao fora do padrão.";
                return crm;
            }



            if (!String.IsNullOrEmpty(xml.CodigoFormaPagamento) && xml.CodigoFormaPagamento.Length == 36)
            {
                FormaPagamento formaPagamento = new Servicos.FormaPagamentoService(this.Organizacao, this.IsOffline).ObterPorGuid(new Guid(xml.CodigoFormaPagamento));
                if (formaPagamento != null)
                    crm.FormaPagamento = new Lookup(formaPagamento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoFormaPagamento : " + xml.CodigoFormaPagamento + " - não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoFormaPagamento não Enviado ou fora do padrão(Guid).";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoConta) && xml.CodigoConta.Length == 36)
            {
                Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (conta != null)
                    crm.Canal = new Lookup(conta.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoConta : " + xml.CodigoConta + " - não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoConta não Enviado ou fora do padrão(Guid).";
                return crm;
            }



            if (!String.IsNullOrEmpty(xml.CodigoBeneficio) && xml.CodigoBeneficio.Length == 36)
            {
                Beneficio beneficio = new Servicos.BeneficioService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoBeneficio));
                if (beneficio != null)
                    crm.BeneficioPrograma = new Lookup(beneficio.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoBeneficio : " + xml.CodigoBeneficio + " - não cadastrado no Crm.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoBeneficio não Enviado ou fora do padrão(Guid).";
                return crm;
            }

            crm.AlteradaParaStockRotation = xml.AlteradaStockRotation;

            crm.SituacaoIrregularidades = xml.SolicitacaoIrregular;

            if (!String.IsNullOrEmpty(xml.CodigoTipoSolicitacao) && xml.CodigoTipoSolicitacao.Length == 36)
            {
                crm.TipoSolicitacao = new Lookup(new Guid(xml.CodigoTipoSolicitacao), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoTipoSolicitacao não Enviado ou fora do padrão(Guid).";
                return crm;
            }
            //Novos campos - 1409

            crm.AjusteSaldo = xml.SolicitacaoAjuste;

            crm.ValorAbater = xml.ValorAbater;

            Usuario objAssitente = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscaPorCodigoAssistente(xml.CodigoAssistente.Value);
            if (objAssitente != null)
            {
                crm.Assistente = new Lookup(objAssitente.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Assistente não cadastrado no Crm.";
                return crm;
            }

            Usuario supervisorEms = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscaPorCodigoSupervisorEMS(xml.CodigoSupervisorEMS);
            if (supervisorEms != null)
            {
                crm.Supervisor = new Lookup(supervisorEms.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Supervisor não cadastrado no Crm.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoFilial))
            {
                crm.Filial = new Lookup(new Guid(xml.CodigoFilial), "");
            }
            else
            {
                crm.AddNullProperty("Filial");
            }

            if (xml.StatusPagamento.HasValue)
            {
                crm.StatusPagamento = xml.StatusPagamento;
            }
            else
            {
                crm.AddNullProperty("StatusPagamento");
            }

            if (xml.ValorPago.HasValue)
                crm.ValorPago = xml.ValorPago.Value;

            if (xml.ValorCancelado.HasValue)
                crm.ValorCancelado = xml.ValorCancelado.Value;

            if (xml.StatusCalculoPriceProtection.HasValue)
                crm.StatusCalculoPriceProtection = xml.StatusCalculoPriceProtection.Value;

            if (xml.DataValidade.HasValue)
                crm.DataValidade = xml.DataValidade.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataValidade não informado favor verificar integração.";
                return crm;
            }

            if (xml.CodigoCondicaoPagamento.HasValue)
            {
                CondicaoPagamento condicaoPagamento = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(xml.CodigoCondicaoPagamento.Value);
                if (condicaoPagamento != null)
                    crm.CondicaoPagamento = new Lookup(condicaoPagamento.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoCondicaoPagamento informado não registrado no CRM, favor verificar.";
                    return crm;
                }
            }

            crm.DescartarVerba = xml.DescartarVerba;

            if (!string.IsNullOrEmpty(xml.TrimestreCompetencia))
            {
                crm.TrimestreCompetencia = xml.TrimestreCompetencia;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TrimestreCompetencia não informado favor verificar integração.";
                return crm;
            }

            crm.FormaCancelamento = xml.FormaCancelamento;

            if (xml.CodigoSolicitacaoPrincipal != null)
            {
                crm.SolicitacaoBeneficioPrincipal = new Lookup(new Guid(xml.CodigoSolicitacaoPrincipal), "");
            }

            #endregion

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;

            return crm;
        }

        private List<ProdutosdaSolicitacao> DefinirPropriedadesItens(List<Pollux.Entities.ProdutoSolicitacaoItem> listSolicitacaoItens, string origem, Guid? solicitacaoID)
        {
            List<ProdutosdaSolicitacao> lstRetorno = new List<ProdutosdaSolicitacao>();
            #region Lista ProdutoSoliciatacao
            foreach (var itemPollux in listSolicitacaoItens.Where(x => x.Situacao == (int)Enum.ProdutoSolicitacao.Status.Ativo))
            {
                ProdutosdaSolicitacao produtoSolicitacao = new ProdutosdaSolicitacao(this.Organizacao, this.IsOffline);

                if (!String.IsNullOrEmpty(itemPollux.CodigoProduto))
                {
                    Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(itemPollux.CodigoProduto);
                    if (produto != null)
                    {
                        produtoSolicitacao.Produto = new Lookup(produto.ID.Value, "");
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Produto não cadastrado no Crm!";
                        return null;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoProduto não enviado.";
                    return null;

                }

                #region SETA ORIGEM PARA REGRA DE NEGOCIO ESPECIFICA (2)
                if (!string.IsNullOrEmpty(origem))
                    produtoSolicitacao.IntegradoDe = origem;
                else
                    produtoSolicitacao.AddNullProperty("IntegradoDe");
                #endregion

                if (!String.IsNullOrEmpty(itemPollux.CodigoBeneficio) && itemPollux.CodigoBeneficio.Length == 36)
                {
                    Beneficio beneficio = new Servicos.BeneficioService(this.Organizacao, this.IsOffline).ObterPor(new Guid(itemPollux.CodigoBeneficio));
                    if (beneficio != null)
                        produtoSolicitacao.BeneficioPrograma = new Lookup(beneficio.ID.Value, "");
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "CodigoBeneficio do ProdutoSolicitacao : " + itemPollux.CodigoBeneficio + " - não cadastrado no Crm.";
                        return null;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoBeneficio não Enviado ou fora do padrão(Guid).";
                    return null;
                }
                if (solicitacaoID.HasValue)
                {
                    produtoSolicitacao.SolicitacaoBeneficio = new Lookup(solicitacaoID.Value, "");
                }

                produtoSolicitacao.ValorUnitario = itemPollux.ValorUnitario;
                produtoSolicitacao.ValorTotal = itemPollux.ValorTotal;
                if (!String.IsNullOrEmpty(itemPollux.ChaveIntegracaoNotaFiscal))
                {
                    Fatura fatura = new Servicos.FaturaService(this.Organizacao, this.IsOffline).ObterPorChaveIntergacao(itemPollux.ChaveIntegracaoNotaFiscal);
                    if (fatura != null)
                    {
                        produtoSolicitacao.Fatura = new Lookup(fatura.ID.Value, "");
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Fatura referente ChaveIntegracaoNotaFiscal: " + itemPollux.ChaveIntegracaoNotaFiscal + " - não cadastrado no Crm.";
                        return null;
                    }
                }

                if (itemPollux.QuantidadeAprovado.HasValue)
                    produtoSolicitacao.QuantidadeAprovada = itemPollux.QuantidadeAprovado;
                else
                    produtoSolicitacao.AddNullProperty("QuantidadeAprovada");

                if (itemPollux.ValorUnitarioAprovado.HasValue)
                    produtoSolicitacao.ValorUnitarioAprovado = itemPollux.ValorUnitarioAprovado;
                else
                    produtoSolicitacao.AddNullProperty("ValorUnitarioAprovado");

                if (itemPollux.ValorTotalAprovado.HasValue)
                    produtoSolicitacao.ValorTotalAprovado = itemPollux.ValorTotalAprovado;
                else
                    produtoSolicitacao.AddNullProperty("ValorTotalAprovado");

                produtoSolicitacao.QuantidadeSolicitada = itemPollux.Quantidade;
                //Novos campos

                if (!String.IsNullOrEmpty(itemPollux.CodigoProdutoSolicitacao))
                {
                    produtoSolicitacao.ID = new Guid(itemPollux.CodigoProdutoSolicitacao);
                }

                produtoSolicitacao.Acao = itemPollux.Acao;

                Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimentoPorCodigo(itemPollux.CodigoEstabelecimento.Value);

                if (estabelecimento != null)
                {
                    produtoSolicitacao.Estabelecimento = new Lookup(estabelecimento.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estabelecimento codigo : " + itemPollux.CodigoEstabelecimento.ToString() + " - não cadastrado no Crm.";
                    return null;
                }

                if (itemPollux.Situacao.HasValue)
                {
                    produtoSolicitacao.State = itemPollux.Situacao.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Situacao não informado favor verificar integração.";
                    return null;
                }

                if (itemPollux.QuantidadeCancelada.HasValue)
                    produtoSolicitacao.QuantidadeCancelada = itemPollux.QuantidadeCancelada.Value;

                if (itemPollux.ValorPago.HasValue)
                    produtoSolicitacao.ValorPago = itemPollux.ValorPago.Value;

                if (itemPollux.ValorCancelado.HasValue)
                    produtoSolicitacao.ValorCancelado = itemPollux.ValorCancelado.Value;

                if (itemPollux.QuantidadeAjustada.HasValue)
                    produtoSolicitacao.QuantidadeAjustada = itemPollux.QuantidadeAjustada.Value;

                lstRetorno.Add(produtoSolicitacao);
            }
            #endregion

            return lstRetorno;
        }

        private Intelbras.Message.Helper.MSG0155 DefinirPropriedadesPlugin(SolicitacaoBeneficio crm)
        {
            Intelbras.Message.Helper.MSG0155 objPollux = new Pollux.MSG0155(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), crm.Nome.Truncate(40));

            objPollux.CodigoSolicitacaoBeneficio = crm.ID.ToString();
            objPollux.ValorAprovado = crm.ValorAprovado;

            if (!String.IsNullOrEmpty(crm.Nome))
                objPollux.NomeSolicitacaoBeneficio = crm.Nome;
            else
            {
                throw new ArgumentException("(CRM) Nome SolicitacaoBeneficio não preenchido.");
            }

            if (crm.UnidadedeNegocio != null)
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(crm.UnidadedeNegocio.Id);
                if (unidadeNegocio != null)
                {
                    objPollux.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                }
            }
            else
            {
                throw new ArgumentException("(CRM) UnidadedeNegocio não preenchida.");
            }

            if (!String.IsNullOrEmpty(crm.Nome))
                objPollux.DescricaoSituacaoIrregular = crm.SituacaoIrregular;
            

            if (crm.BeneficioCanal != null)
                objPollux.CodigoBeneficioCanal = crm.BeneficioCanal.Id.ToString();
            else
                throw new ArgumentException("(CRM) BeneficioCanal não preenchido.");

            if (crm.ValorSolicitado.HasValue)
                objPollux.ValorSolicitado = crm.ValorSolicitado.Value;


            objPollux.DescricaoSolicitacao = crm.Descricao;

            if (crm.StatusSolicitacao.HasValue)
                objPollux.SituacaoSolicitacaoBeneficio = crm.StatusSolicitacao.Value;

            if (crm.State.HasValue)
                objPollux.Situacao = crm.State.Value;

            if (crm.Status.HasValue)
                objPollux.RazaoStatusSolicitacaoBeneficio = crm.Status.Value;

            if (crm.FormaPagamento != null)
                objPollux.CodigoFormaPagamento = crm.FormaPagamento.Id.ToString();
            else
                throw new ArgumentException("(CRM) FormaPagamento não preenchida.");

            if (crm.Canal != null)
                objPollux.CodigoConta = crm.Canal.Id.ToString();
            else
                throw new ArgumentException("(CRM) Canal não preenchido.");

            if (crm.BeneficioPrograma != null)
            {
                objPollux.CodigoBeneficio = crm.BeneficioPrograma.Id.ToString();

                Beneficio benefProg = new Intelbras.CRM2013.Domain.Servicos.BeneficioService(this.Organizacao, this.IsOffline).ObterPor(crm.BeneficioPrograma.Id);
                if (benefProg != null && benefProg.Codigo.HasValue)
                    objPollux.BeneficioCodigo = benefProg.Codigo.Value;
            }
            else
                throw new ArgumentException("(CRM) BeneficioPrograma não preenchido.");

            objPollux.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
            objPollux.TipoProprietario = "systemuser";


            if (crm.TipoSolicitacao != null)
                objPollux.CodigoTipoSolicitacao = crm.TipoSolicitacao.Id.ToString();
            else
                throw new ArgumentException("(CRM) CodigoTipoSolicitacao não enviado.");

            if (crm.SituacaoIrregularidades.HasValue)
                objPollux.SolicitacaoIrregular = crm.SituacaoIrregularidades.Value;
            objPollux.ProdutoSolicitacaoItens = this.RetornaListaItens(crm.ID.Value);
            if (crm.AlteradaParaStockRotation.HasValue)
                objPollux.AlteradaStockRotation = crm.AlteradaParaStockRotation.Value;
            else
                objPollux.AlteradaStockRotation = false;

            if (crm.TipoPriceProtection.HasValue)
                objPollux.TipoPriceProtection = crm.TipoPriceProtection.Value;
            else
                objPollux.TipoPriceProtection = (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Consumo;
            //Novos Campos

            if (crm.AjusteSaldo.HasValue)
                objPollux.SolicitacaoAjuste = crm.AjusteSaldo.Value;

            if (crm.StatusCalculoPriceProtection.HasValue)
                objPollux.StatusCalculoPriceProtection = crm.StatusCalculoPriceProtection.Value;

            if (crm.ValorAbater.HasValue)
                objPollux.ValorAbater = crm.ValorAbater.Value;
            else
                objPollux.ValorAbater = new decimal(0);

            Usuario assistente = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(crm.Assistente.Id);
            if (assistente != null)
            {
                objPollux.CodigoAssistente = assistente.CodigoAssistenteComercial.Value;
            }
            else
            {
                throw new ApplicationException("(CRM) Assistente não cadastrado / Obrigatório.");
            }

            Usuario supervisor = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(crm.Supervisor.Id);
            if (supervisor != null)
            {
                objPollux.CodigoSupervisorEMS = supervisor.CodigoSupervisorEMS;
            }
            else
            {
                throw new ApplicationException("(CRM) Supervisor não cadastrado / Obrigatório.");
            }

            if (crm.Filial != null)
                objPollux.CodigoFilial = crm.Filial.Id.ToString();
            if (crm.StatusPagamento.HasValue)
                objPollux.StatusPagamento = crm.StatusPagamento;

            if (crm.ValorPago.HasValue)
                objPollux.ValorPago = crm.ValorPago.Value;

            if (crm.ValorCancelado.HasValue)
                objPollux.ValorCancelado = crm.ValorCancelado.Value;

            if (crm.DataCriacao.HasValue)
            {
                objPollux.DataCriacao = crm.DataCriacao.Value.ToLocalTime();
            }
            else
            {
                throw new ArgumentException("(CRM) DataCriacao não cadastrada / Obrigatório.");
            }

            if (crm.DataValidade.HasValue)
            {
                objPollux.DataValidade = crm.DataValidade;
            }
            else
            {
                throw new ArgumentException("(CRM) DataValidade não cadastrada / Obrigatório.");
            }

            if (crm.CondicaoPagamento != null)
            {
                CondicaoPagamento condicaoPagamento = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamento(crm.CondicaoPagamento.Id);
                if (condicaoPagamento != null)
                {
                    if (condicaoPagamento.Codigo.HasValue)
                        objPollux.CodigoCondicaoPagamento = condicaoPagamento.Codigo.Value;
                }
            }

            if (crm.DescartarVerba.HasValue)
                objPollux.DescartarVerba = crm.DescartarVerba.Value;

            if (!string.IsNullOrEmpty(crm.TrimestreCompetencia))
            {
                objPollux.TrimestreCompetencia = crm.TrimestreCompetencia;
            }
            else
            {
                throw new ArgumentException("(CRM) TrimestreCompetencia não cadastrada / Obrigatório.");
            }

            objPollux.FormaCancelamento = crm.FormaCancelamento;

            return objPollux;
        }

        private List<Pollux.Entities.ProdutoSolicitacaoItem> RetornaListaItens(Guid solicitacaoId)
        {
            List<Pollux.Entities.ProdutoSolicitacaoItem> lstRetorno = new List<Pollux.Entities.ProdutoSolicitacaoItem>();

            List<ProdutosdaSolicitacao> lstProdutoSolicitacao = new Servicos.ProdutosdaSolicitacaoService(this.Organizacao, this.IsOffline).ListarPorSolicitacao(solicitacaoId);
            foreach (var itemProdSol in lstProdutoSolicitacao)
            {
                Pollux.Entities.ProdutoSolicitacaoItem objItemPollux = new Pollux.Entities.ProdutoSolicitacaoItem();
                if (itemProdSol.Produto != null)
                {
                    Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(itemProdSol.Produto.Id);
                    if (produto != null)
                        objItemPollux.CodigoProduto = produto.Codigo;
                    else
                    {
                        throw new ArgumentException("(CRM) Produto do Item não preenchido");
                    }
                }
                objItemPollux.Situacao = itemProdSol.State.Value; 

                objItemPollux.Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39";
                objItemPollux.TipoProprietario = "systemuser";

                if (itemProdSol.BeneficioPrograma != null)
                    objItemPollux.CodigoBeneficio = itemProdSol.BeneficioPrograma.Id.ToString();
                else
                    throw new Exception("(CRM) Código do Benefício não preenchido.");

                objItemPollux.CodigoSolicitacaoBeneficio = solicitacaoId.ToString();
                if (!itemProdSol.ValorUnitario.HasValue)
                {
                    objItemPollux.ValorUnitario = 0;
                }
                else
                {
                    objItemPollux.ValorUnitario = itemProdSol.ValorUnitario.Value;
                }

                if (!itemProdSol.ValorTotal.HasValue)
                {
                    objItemPollux.ValorTotal = 0;
                }
                else
                {
                    objItemPollux.ValorTotal = itemProdSol.ValorTotal.Value;
                }
                
                if (itemProdSol.Fatura != null)
                {
                    Fatura fatura = new Servicos.FaturaService(this.Organizacao, this.IsOffline).ObterPor(itemProdSol.Fatura.Id);

                    if (fatura != null)
                        objItemPollux.ChaveIntegracaoNotaFiscal = fatura.ChaveIntegracao;

                }

                if (itemProdSol.QuantidadeAprovada.HasValue)
                {
                    objItemPollux.QuantidadeAprovado = itemProdSol.QuantidadeAprovada.Value;
                }

                if (itemProdSol.ValorUnitarioAprovado.HasValue)
                {
                    objItemPollux.ValorUnitarioAprovado = itemProdSol.ValorUnitarioAprovado.Value;
                }

                if (itemProdSol.ValorTotalAprovado.HasValue)
                {
                    objItemPollux.ValorTotalAprovado = itemProdSol.ValorTotalAprovado.Value;
                }

                objItemPollux.Quantidade = itemProdSol.QuantidadeSolicitada.Value;
                //Campos Novas
                objItemPollux.CodigoProdutoSolicitacao = itemProdSol.ID.Value.ToString();
                Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimento(itemProdSol.Estabelecimento.Id);

                if (estabelecimento != null && estabelecimento.Codigo.HasValue)
                {
                    objItemPollux.CodigoEstabelecimento = estabelecimento.Codigo.Value;
                }
                else
                    throw new ArgumentException("(CRM) Estabelecimento/codigo Estabelecimento não cadastrado no Crm.");

                objItemPollux.Acao = "A";

                if (itemProdSol.QuantidadeCancelada.HasValue)
                    objItemPollux.QuantidadeCancelada = itemProdSol.QuantidadeCancelada.Value;

                if (itemProdSol.ValorPago.HasValue)
                    objItemPollux.ValorPago = itemProdSol.ValorPago.Value;

                if (itemProdSol.ValorCancelado.HasValue)
                    objItemPollux.ValorCancelado = itemProdSol.ValorCancelado.Value;

                lstRetorno.Add(objItemPollux);
            }

            return lstRetorno;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(SolicitacaoBeneficio objModel)
        {
            string resposta;
            Pollux.MSG0155 mensagem = DefinirPropriedadesPlugin(objModel);

            Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Pollux.MSG0155R1 retorno = CarregarMensagem<Pollux.MSG0155R1>(resposta);

                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }
                else
                {
                    if (retorno.SaldoBeneficioCanal != null)
                    {
                        BeneficioDoCanal beneficioCanal = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(new Guid(retorno.SaldoBeneficioCanal.CodigoBeneficioCanal));

                        beneficioCanal.VerbaPeriodoAtual = retorno.SaldoBeneficioCanal.VerbaCalculada.Value;
                        beneficioCanal.VerbaPeriodosAnteriores = retorno.SaldoBeneficioCanal.VerbaPeriodoAnterior.Value;
                        beneficioCanal.VerbaBrutaPeriodoAtual = retorno.SaldoBeneficioCanal.VerbaTotal.Value;
                        beneficioCanal.TotalSolicitacoesAprovadasNaoPagas = retorno.SaldoBeneficioCanal.VerbaEmpenhada.Value;
                        beneficioCanal.VerbaReembolsada = retorno.SaldoBeneficioCanal.VerbaReembolsada.Value;
                        beneficioCanal.VerbaCancelada = retorno.SaldoBeneficioCanal.VerbaCancelada.Value;
                        beneficioCanal.VerbaAjustada = retorno.SaldoBeneficioCanal.VerbaAjustada.Value;
                        beneficioCanal.VerbaDisponivel = retorno.SaldoBeneficioCanal.VerbaDisponivel.Value;

                        new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).AlterarBeneficioCanal(beneficioCanal);

                        if(objModel.TipoPriceProtection.HasValue && objModel.TipoPriceProtection.Value == (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                        {
                            if (objModel.StatusSolicitacao.HasValue && objModel.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                            {
                                objModel.StatusSolicitacao = (int?)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado;
                            }
                        } 
                        
                    }
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + erro001.GenerateMessage(false));
            }
            return resposta;
        }

        #endregion

    }
}
