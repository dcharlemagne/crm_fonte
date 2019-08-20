using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0148 : Base, IBase<Message.Helper.MSG0148, Domain.Model.SolicitacaoBeneficio>
    {
        #region Construtor

        public MSG0148(string org, bool isOffline) : base(org, isOffline)
        {
            Organizacao = org;
            IsOffline = isOffline;
        }

        #endregion

        #region Propriedades

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
                SolicitacaoBeneficio solicitacaoBenficioConsulta = null;
                Pollux.Entities.SolicitacaoBeneficioR1 objRetornoPollux = new Pollux.Entities.SolicitacaoBeneficioR1();

                var xml = this.CarregarMensagem<Pollux.MSG0148>(mensagem);
                //Solicitacao Beneficio
                if (!string.IsNullOrEmpty(xml.CodigoSolicitacaoBeneficio) && xml.CodigoSolicitacaoBeneficio.Length == 36)
                {
                    solicitacaoBenficioConsulta = new Servicos.SolicitacaoBeneficioService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoSolicitacaoBeneficio));

                    if (solicitacaoBenficioConsulta == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Valor do parâmetro " + xml.CodigoSolicitacaoBeneficio + " não existe.";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0148R1>(numeroMensagem, retorno);
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Parâmetro obrigatório para a consulta não enviado.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0148R1>(numeroMensagem, retorno);
                }

                objRetornoPollux = DefinirRetorno(solicitacaoBenficioConsulta);

                if (objRetornoPollux == null)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0148R1>(numeroMensagem, retorno);
                }
                else if (!resultadoPersistencia.Sucesso)
                {
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0148R1>(numeroMensagem, retorno);
                }

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("SolicitacaoBeneficio", objRetornoPollux);
                retorno.Add("Resultado", resultadoPersistencia);

                return CriarMensagemRetorno<Pollux.MSG0148R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0173R1>(numeroMensagem, retorno);
            }
        }
       
        #region Definir Propriedades
        public SolicitacaoBeneficio DefinirPropriedades(Intelbras.Message.Helper.MSG0148 xml)
        {
            SolicitacaoBeneficio retorno = new SolicitacaoBeneficio(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public Pollux.Entities.SolicitacaoBeneficioR1 DefinirRetorno(Model.SolicitacaoBeneficio itemCrm)
        {
            Pollux.Entities.SolicitacaoBeneficioR1 solBeneficioPollux = new Pollux.Entities.SolicitacaoBeneficioR1();

            #region Propriedades Crm->Xml
            
            if (!string.IsNullOrEmpty(itemCrm.Nome))
                solBeneficioPollux.NomeSolicitacaoBeneficio = itemCrm.Nome;
            else
                solBeneficioPollux.NomeSolicitacaoBeneficio = "N/A";
            
            if (itemCrm.UnidadedeNegocio != null)
            {
                UnidadeNegocio unidadeNeg = new UnidadeNegocioService(Organizacao, IsOffline).BuscaUnidadeNegocio(itemCrm.UnidadedeNegocio.Id);

                if (unidadeNeg != null)
                {
                    if (!String.IsNullOrEmpty(unidadeNeg.ChaveIntegracao))
                        solBeneficioPollux.CodigoUnidadeNegocio = unidadeNeg.ChaveIntegracao;
                    else solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                    if (!String.IsNullOrEmpty(unidadeNeg.Nome))
                        solBeneficioPollux.NomeUnidadeNegocio = unidadeNeg.Nome;
                    else solBeneficioPollux.NomeUnidadeNegocio = "N/A";
                }
                else
                {
                    solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                    solBeneficioPollux.NomeUnidadeNegocio = "N/A";
                }
            }
            else
            {
                solBeneficioPollux.CodigoUnidadeNegocio = "N/A";
                solBeneficioPollux.NomeUnidadeNegocio = "N/A";
            }
            if (itemCrm.ValorAcao.HasValue)
                solBeneficioPollux.ValorAcao = itemCrm.ValorAcao.Value;
            else
                solBeneficioPollux.ValorAcao = 0;

            if (!string.IsNullOrEmpty(itemCrm.SituacaoIrregular))
                solBeneficioPollux.DescricaoSituacaoIrregular = itemCrm.SituacaoIrregular;
            if (itemCrm.AcaoSubsidiadaVmc != null)
            {
                solBeneficioPollux.CodigoAcaoSubsidiadaVMC = itemCrm.AcaoSubsidiadaVmc.Id.ToString();
                solBeneficioPollux.NomeAcaoSubsidiadaVMC = itemCrm.AcaoSubsidiadaVmc.Name;
            }

            if (itemCrm.ValorSolicitado.HasValue)
                solBeneficioPollux.ValorSolicitado = itemCrm.ValorSolicitado.Value;
            else
                solBeneficioPollux.ValorSolicitado = 0;

            if (itemCrm.StatusSolicitacao.HasValue)
            {
                solBeneficioPollux.SituacaoSolicitacaoBeneficio = itemCrm.StatusSolicitacao.Value;
                solBeneficioPollux.NomeSituacaoSolicitacao = Helper.GetDescription(((Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio)itemCrm.StatusSolicitacao));
            }
            else
            {
                solBeneficioPollux.SituacaoSolicitacaoBeneficio = 0;
                solBeneficioPollux.NomeSituacaoSolicitacao = "N/A";

            }
            if (itemCrm.FormaPagamento != null)
            {
                solBeneficioPollux.CodigoFormaPagamento = itemCrm.FormaPagamento.Id.ToString();
                solBeneficioPollux.NomeFormaPagamento = itemCrm.FormaPagamento.Name;
            }
            else
            {
                solBeneficioPollux.CodigoFormaPagamento = Guid.Empty.ToString();
                solBeneficioPollux.NomeFormaPagamento = "N/A";
            }

            if (itemCrm.ValorAprovado.HasValue)
                solBeneficioPollux.ValorAprovado = itemCrm.ValorAprovado.Value;

            if (itemCrm.DataIniAcao.HasValue)
                solBeneficioPollux.DataInicioAcao = itemCrm.DataIniAcao.Value;

            if (itemCrm.DataFimAcao.HasValue)
                solBeneficioPollux.DataPrevistaRetornoAcao = itemCrm.DataFimAcao.Value;

            if (itemCrm.ValorPago.HasValue)
                solBeneficioPollux.ValorPago = itemCrm.ValorPago.Value;

            if (itemCrm.BeneficioCanal != null)
            {
                BeneficioDoCanal benefCanal = new Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(itemCrm.BeneficioCanal.Id);
                if (benefCanal != null)
                {
                    solBeneficioPollux.CodigoBeneficioCanal = benefCanal.ID.Value.ToString();
                    solBeneficioPollux.NomeBeneficioCanal = benefCanal.Nome;

                    if (benefCanal.StatusBeneficio != null)
                    {
                        solBeneficioPollux.CodigoStatusBeneficio = benefCanal.StatusBeneficio.Id.ToString();
                        solBeneficioPollux.NomeStatusBeneficio = benefCanal.StatusBeneficio.Name;
                    }
                    else
                    {
                        solBeneficioPollux.CodigoStatusBeneficio = Guid.Empty.ToString();
                        solBeneficioPollux.NomeStatusBeneficio = "N/A";
                    }
                }
                else
                {
                    solBeneficioPollux.CodigoBeneficioCanal = Guid.Empty.ToString();
                    solBeneficioPollux.NomeBeneficioCanal = "N/A";
                    solBeneficioPollux.CodigoStatusBeneficio = Guid.Empty.ToString();
                    solBeneficioPollux.NomeStatusBeneficio = "N/A";
                }
            }
            else
            {
                solBeneficioPollux.CodigoBeneficioCanal = Guid.Empty.ToString();
                solBeneficioPollux.NomeBeneficioCanal = "N/A";
                solBeneficioPollux.CodigoStatusBeneficio = Guid.Empty.ToString();
                solBeneficioPollux.NomeStatusBeneficio = "N/A";
            }


            if (itemCrm.BeneficioPrograma != null)
            {
                Beneficio beneficio = new Servicos.BeneficioService(this.Organizacao, this.IsOffline).ObterPor(itemCrm.BeneficioPrograma.Id);
                if (beneficio != null)
                {
                    solBeneficioPollux.CodigoBeneficio = beneficio.ID.Value.ToString();
                    solBeneficioPollux.NomeBeneficio = beneficio.Nome;

                    if (beneficio.Codigo.HasValue)
                        solBeneficioPollux.BeneficioCodigo = beneficio.Codigo.Value;
                    else
                        solBeneficioPollux.BeneficioCodigo = (int)this.PreencherAtributoVazio("int");
                }
                else
                {
                    solBeneficioPollux.CodigoBeneficio = Guid.Empty.ToString();
                    solBeneficioPollux.NomeBeneficio = "N/A";
                    solBeneficioPollux.BeneficioCodigo = (int)this.PreencherAtributoVazio("int");
                }
            }
            else
            {
                solBeneficioPollux.CodigoBeneficioCanal = Guid.Empty.ToString();
                solBeneficioPollux.NomeBeneficioCanal = "N/A";
            }
            if (itemCrm.AlteradaParaStockRotation.HasValue)
                solBeneficioPollux.AlteradaStockRotation = itemCrm.AlteradaParaStockRotation.Value;
            else
                solBeneficioPollux.AlteradaStockRotation = false;
            if (itemCrm.SituacaoIrregularidades.HasValue)
                solBeneficioPollux.SolicitacaoIrregular = itemCrm.SituacaoIrregularidades.Value;
            else
                solBeneficioPollux.SolicitacaoIrregular = false;
            if (itemCrm.Canal != null)
            {
                solBeneficioPollux.CodigoConta = itemCrm.Canal.Id.ToString();
                solBeneficioPollux.NomeConta = itemCrm.Canal.Name;
            }
            else
            {
                solBeneficioPollux.CodigoConta = Guid.Empty.ToString();
                solBeneficioPollux.NomeConta = "N/A";
            }

            if(itemCrm.AjusteSaldo.HasValue)
                solBeneficioPollux.SolicitacaoAjuste = itemCrm.AjusteSaldo.Value;

            if (itemCrm.StatusCalculoPriceProtection.HasValue)
                solBeneficioPollux.StatusCalculoPriceProtection = itemCrm.StatusCalculoPriceProtection.Value;

            if (itemCrm.ResultadoPrevisto.HasValue)
                solBeneficioPollux.ResultadoPrevisto = itemCrm.ResultadoPrevisto.Value;

            if (itemCrm.ResultadoAlcancado.HasValue)
                solBeneficioPollux.ResultadoAlcancado = itemCrm.ResultadoAlcancado.Value;

            if (itemCrm.ValorAbater.HasValue)
                solBeneficioPollux.ValorAbater = itemCrm.ValorAbater.Value;
            else
                solBeneficioPollux.ValorAbater = 0;

            solBeneficioPollux.CodigoSolicitacaoBeneficio = itemCrm.ID.ToString();

            if (itemCrm.TipoSolicitacao != null)
            {
                solBeneficioPollux.CodigoTipoSolicitacao = itemCrm.TipoSolicitacao.Id.ToString();
                solBeneficioPollux.NomeTipoSolicitacao = itemCrm.TipoSolicitacao.Name;
            }
            else
            {
                solBeneficioPollux.CodigoTipoSolicitacao = Guid.Empty.ToString();
                solBeneficioPollux.NomeTipoSolicitacao = "N/A";
            }

            if (itemCrm.TipoPriceProtection.HasValue)
            {
                solBeneficioPollux.TipoPriceProtection = itemCrm.TipoPriceProtection;
                if (itemCrm.TipoSolicitacao != null)
                    solBeneficioPollux.NomeTipoPriceProtection = itemCrm.TipoSolicitacao.Name;
            }


            if (!String.IsNullOrEmpty(itemCrm.Descricao))
            {
                solBeneficioPollux.DescricaoSolicitacao = itemCrm.Descricao;
            }

            solBeneficioPollux.Situacao = itemCrm.State.Value;
            solBeneficioPollux.NomeSituacao = Helper.GetDescription((Domain.Enum.SolicitacaoBeneficio.State)(itemCrm.State.Value));
            solBeneficioPollux.RazaoStatusSolicitacaoBeneficio = itemCrm.Status.Value;
            if (itemCrm.State.Value == (int)(Domain.Enum.SolicitacaoBeneficio.State.Ativo))
            {
                solBeneficioPollux.NomeRazaoStatusSolicitacao = Helper.GetDescription((Domain.Enum.SolicitacaoBeneficio.RazaoStatusAtivo)(itemCrm.Status.Value));
            }
            else
            {
                solBeneficioPollux.NomeRazaoStatusSolicitacao = Helper.GetDescription((Domain.Enum.SolicitacaoBeneficio.RazaoStatusInativo)(itemCrm.Status.Value));
            }

            solBeneficioPollux.DataCriacaoSolicitacao = itemCrm.DataCriacao.Value;
            solBeneficioPollux.Proprietario = usuarioIntegracao.ID.Value.ToString();
            solBeneficioPollux.NomeProprietario = usuarioIntegracao.Nome;
            solBeneficioPollux.TipoProprietario = "systemuser";


            if (itemCrm.Assistente != null)
            {
                Usuario assistente = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(itemCrm.Assistente.Id);
                if (assistente != null && assistente.CodigoAssistenteComercial.HasValue)
                {
                    solBeneficioPollux.CodigoAssistente = assistente.CodigoAssistenteComercial.Value;
                    solBeneficioPollux.NomeAssistente = assistente.Nome;
                }
                else
                {
                    solBeneficioPollux.CodigoAssistente = 0;
                    solBeneficioPollux.NomeAssistente = "N/A";

                }
            }
            else
            {
                solBeneficioPollux.CodigoAssistente = 0;
                solBeneficioPollux.NomeAssistente = "N/A";
            }


            if (itemCrm.Supervisor != null)
            {
                Usuario supervisor = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(itemCrm.Supervisor.Id);
                if (supervisor != null && !String.IsNullOrEmpty(supervisor.CodigoSupervisorEMS))
                {
                    solBeneficioPollux.CodigoSupervisorEMS = supervisor.CodigoSupervisorEMS;
                    solBeneficioPollux.NomeSupervisor = supervisor.Nome;
                }
                else
                {
                    solBeneficioPollux.CodigoSupervisorEMS = "N/A";
                    solBeneficioPollux.NomeSupervisor = "N/A";

                }
            }
            else
            {
                solBeneficioPollux.CodigoSupervisorEMS = "N/A"; ;
                solBeneficioPollux.NomeSupervisor = "N/A";
            }

            if (itemCrm.Filial != null)
            {
                solBeneficioPollux.CodigoFilial = itemCrm.Filial.Id.ToString();
                solBeneficioPollux.NomeFilial = itemCrm.Filial.Name;
            }

            if (itemCrm.StatusPagamento.HasValue)
            {
                solBeneficioPollux.StatusPagamento = itemCrm.StatusPagamento;
                solBeneficioPollux.NomeStatusPagamento = Helper.GetDescription((Domain.Enum.SolicitacaoBeneficio.StatusPagamento)(itemCrm.StatusPagamento.Value));
            }

            if (itemCrm.SolicitacaoBeneficioPrincipal != null)
            {
                solBeneficioPollux.CodigoSolicitacaoPrincipal = itemCrm.SolicitacaoBeneficioPrincipal.Id.ToString();
                solBeneficioPollux.NomeSolicitacaoPrincipal = itemCrm.SolicitacaoBeneficioPrincipal.Name;
            }

            if (itemCrm.ValorCancelado.HasValue)
            {
                solBeneficioPollux.ValorCancelado = itemCrm.ValorCancelado.Value;
            }

            if (itemCrm.DataValidade.HasValue)
            {
                solBeneficioPollux.DataValidade = itemCrm.DataValidade.Value;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataValidade obrigatória, favor verificar registro no CRM.";
                return solBeneficioPollux;
            }

            if (itemCrm.CondicaoPagamento != null)
            {
                CondicaoPagamento condicaoPagamento = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamento(itemCrm.CondicaoPagamento.Id);
                
                if(condicaoPagamento.Codigo.HasValue)
                    solBeneficioPollux.CodigoCondicaoPagamento = condicaoPagamento.Codigo.Value;

                if(!string.IsNullOrEmpty(condicaoPagamento.Nome))
                    solBeneficioPollux.NomeCondicaoPagamento = condicaoPagamento.Nome;
            }

            if (itemCrm.DescartarVerba.HasValue)
            {
                solBeneficioPollux.DescartarVerba = itemCrm.DescartarVerba.Value;
            }

            if (!string.IsNullOrEmpty(itemCrm.TrimestreCompetencia))
            {
                solBeneficioPollux.TrimestreCompetencia = itemCrm.TrimestreCompetencia;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "TrimestreCompetencia obrigatório, favor verificar registro no CRM.";
                return solBeneficioPollux;
            }

            if (itemCrm.FormaCancelamento.HasValue)
                solBeneficioPollux.FormaCancelamento = itemCrm.FormaCancelamento.Value;

            solBeneficioPollux.ObterSolicitacaoItens = this.RetornaSolicitacaoItens(itemCrm);


            //Busca as unidades de negócio relacionadas ao Benefício do canal
            List<SolicitacaoXUnidades> lstUnidadesBenef = new Servicos.SolicitacaoXUnidadesService(this.Organizacao, this.IsOffline).ListarPor(itemCrm.ID.Value);
            solBeneficioPollux.UnidadesRelacionadas = this.ConverteLista(lstUnidadesBenef);


            #endregion

            return solBeneficioPollux;
        }
        private List<Pollux.Entities.UnidadeNegocio> ConverteLista(List<SolicitacaoXUnidades> lstUnidadesBenef)
        {
            List<Pollux.Entities.UnidadeNegocio> lstPollux = new List<Pollux.Entities.UnidadeNegocio>();

            foreach (SolicitacaoXUnidades unidades in lstUnidadesBenef)
            {
                Pollux.Entities.UnidadeNegocio Objeto = new Pollux.Entities.UnidadeNegocio();

                Objeto.CodigoUnidadeNegocio = unidades.UnidadeNegocio.ChaveIntegracao;
                Objeto.NomeUnidadeNegocio = unidades.UnidadeNegocio.Nome;

                lstPollux.Add(Objeto);
            }
            return lstPollux;
        }

        private List<Intelbras.Message.Helper.Entities.ObterSolicitacaoItem> RetornaSolicitacaoItens(Intelbras.CRM2013.Domain.Model.SolicitacaoBeneficio objSolicitacaoBeneficio)
        {
            List<Intelbras.Message.Helper.Entities.ObterSolicitacaoItem> lstSolicitacaoItens = new List<Intelbras.Message.Helper.Entities.ObterSolicitacaoItem>();
            var lstProdSolicitacao = new Servicos.ProdutosdaSolicitacaoService(this.Organizacao, this.IsOffline).ListarPorSolicitacao(objSolicitacaoBeneficio.ID.Value);

            if (lstProdSolicitacao != null && lstProdSolicitacao.Count > 0)
            {
                foreach (var produtoDaSolicitacao in lstProdSolicitacao)
                {

                    Intelbras.Message.Helper.Entities.ObterSolicitacaoItem itemSolicitacao = new Intelbras.Message.Helper.Entities.ObterSolicitacaoItem();

                    itemSolicitacao.CodigoProdutoSolicitacao = produtoDaSolicitacao.ID.Value.ToString();
                    itemSolicitacao.CodigoSolicitacaoBeneficio = objSolicitacaoBeneficio.ID.Value.ToString();
                    Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(produtoDaSolicitacao.Produto.Id);
                    if (produto != null)
                    {
                        itemSolicitacao.CodigoProduto = produto.Codigo;
                        itemSolicitacao.NomeProduto = produto.Nome;
                        itemSolicitacao.CodigoBeneficio = objSolicitacaoBeneficio.BeneficioPrograma.Id.ToString();
                        itemSolicitacao.NomeBeneficio = objSolicitacaoBeneficio.BeneficioPrograma.Name;
                    }
                    else
                    {
                        throw new Exception("Produto ID :" + produtoDaSolicitacao.ID.Value.ToString() + "não encontrado!");
                    }
                    if (produtoDaSolicitacao.ValorUnitario.HasValue)
                        itemSolicitacao.ValorUnitario = produtoDaSolicitacao.ValorUnitario.Value;
                    else
                        itemSolicitacao.ValorUnitario = 0;

                    if (produtoDaSolicitacao.QuantidadeSolicitada.HasValue)
                        itemSolicitacao.Quantidade = Convert.ToInt32(produtoDaSolicitacao.QuantidadeSolicitada.Value);//Mudar Willer
                    else
                        itemSolicitacao.Quantidade = 0;

                    if (produtoDaSolicitacao.ValorTotal.HasValue)
                        itemSolicitacao.ValorTotal = produtoDaSolicitacao.ValorTotal.Value;
                    else
                        itemSolicitacao.ValorTotal = 0;

                    if (produtoDaSolicitacao.ValorUnitarioAprovado.HasValue)
                        itemSolicitacao.ValorUnitarioAprovado = produtoDaSolicitacao.ValorUnitarioAprovado.Value;
                    else
                        itemSolicitacao.ValorUnitarioAprovado = 0;

                    if (produtoDaSolicitacao.ValorTotalAprovado.HasValue)
                        itemSolicitacao.ValorTotalAprovado = produtoDaSolicitacao.ValorTotalAprovado.Value;
                    else
                        itemSolicitacao.ValorTotalAprovado = 0;

                    if (produtoDaSolicitacao.QuantidadeAprovada.HasValue)
                        itemSolicitacao.QuantidadeAprovado = Convert.ToInt32(produtoDaSolicitacao.QuantidadeAprovada.Value);//Mudar Willer
                    else
                        itemSolicitacao.QuantidadeAprovado = 0;

                    if (produtoDaSolicitacao.Fatura != null)
                    {
                        Fatura fatura = new Servicos.FaturaService(this.Organizacao, this.IsOffline).ObterPor(produtoDaSolicitacao.Fatura.Id);
                        if (fatura != null)
                        {
                            itemSolicitacao.ChaveIntegracaoNotaFiscal = fatura.ChaveIntegracao;
                            itemSolicitacao.NomeNotaFiscal = fatura.NomeAbreviado;
                        }
                    }

                    itemSolicitacao.Proprietario = usuarioIntegracao.ID.Value.ToString();
                    itemSolicitacao.NomeProprietario = usuarioIntegracao.Nome;
                    itemSolicitacao.TipoProprietario = "systemuser";

                    if (produtoDaSolicitacao.Estabelecimento != null)
                    {
                        Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimento(produtoDaSolicitacao.Estabelecimento.Id);
                        if (estabelecimento != null && estabelecimento.Codigo.HasValue)
                        {
                            itemSolicitacao.NomeEstabelecimento = estabelecimento.Nome;
                            itemSolicitacao.CodigoEstabelecimento = estabelecimento.Codigo.Value;
                        }
                        else
                        {
                            itemSolicitacao.NomeEstabelecimento = "N/A";
                            itemSolicitacao.CodigoEstabelecimento = 0;
                        }
                    }
                    else
                    {
                        itemSolicitacao.NomeEstabelecimento = "N/A";
                        itemSolicitacao.CodigoEstabelecimento = 0;
                    }

                    itemSolicitacao.Situacao = produtoDaSolicitacao.State.Value;

                    if (produtoDaSolicitacao.State.Value.Equals(0))
                        itemSolicitacao.NomeSituacao = "Ativo";
                    else if (produtoDaSolicitacao.State.Value.Equals(1))
                        itemSolicitacao.NomeSituacao = "Inativo";

                    if (produtoDaSolicitacao.QuantidadeCancelada.HasValue)
                        itemSolicitacao.QuantidadeCancelada = produtoDaSolicitacao.QuantidadeCancelada.Value;

                    if (produtoDaSolicitacao.ValorPago.HasValue)
                        itemSolicitacao.ValorPago = produtoDaSolicitacao.ValorPago.Value;

                    if (produtoDaSolicitacao.QuantidadeAjustada.HasValue)
                        itemSolicitacao.QuantidadeAjustada = produtoDaSolicitacao.QuantidadeAjustada.Value;

                    if (produtoDaSolicitacao.ValorCancelado.HasValue)
                        itemSolicitacao.ValorCancelado = produtoDaSolicitacao.ValorCancelado.Value;

                    lstSolicitacaoItens.Add(itemSolicitacao);
                }
            }

            return lstSolicitacaoItens;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(SolicitacaoBeneficio objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
