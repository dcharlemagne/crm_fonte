using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutosdaSolicitacaoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ProdutosdaSolicitacaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutosdaSolicitacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ProdutosdaSolicitacaoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Método plugin

        public void VerificaStatusDaSolicitacao(ProdutosdaSolicitacao produtosdaSolicitacao, string origem)
        {
            SolicitacaoBeneficio mSolicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(produtosdaSolicitacao.SolicitacaoBeneficio.Id);

            if ((mSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                && mSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada)
                && (origem != "" && origem != Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.EMS)))
            {
                throw new ArgumentException("(CRM) Não é Possivel Incluir/Alterar/Excluir Produtos de uma solicitação quando o Status é diferente de Criada/Em Analise.");
            }
        }
        #endregion

        #region Métodos

        public void Persistir(List<ProdutosdaSolicitacao> lista)
        {
            foreach (var item in lista)
            {
                switch (item.Acao.ToUpper())
                {
                    case "E":
                        if (ObterPor(item.ID.Value, "itbc_produtosdasolicitacaoid") != null)
                        {
                            Deletar(item.ID.Value);
                        }
                        break;

                    case "A":
                    case "I":
                        var retornoItem = Persistir(item);
                        if (retornoItem == null)
                        {
                            throw new ArgumentException("(CRM) Erro de Persistência no Produto da Solicitação!");
                        }
                        break;

                    default:
                        throw new ArgumentException("(CRM) Campo 'Ação'do Produto da Solicitação fora do padrão!");
                }
            }
        }

        public ProdutosdaSolicitacao ObterPor(Guid produtosSolicitacaoID, params string[] columns)
        {
            return RepositoryService.ProdutosdaSolicitacao.Retrieve(produtosSolicitacaoID, columns);
        }

        public ProdutosdaSolicitacao ObterAtivoInativoPor(Guid produtosSolicitacaoID)
        {
            return RepositoryService.ProdutosdaSolicitacao.ObterAtivoInativoPor(produtosSolicitacaoID);
        }

        public ProdutosdaSolicitacao Persistir(ProdutosdaSolicitacao objProdutosdaSolicitacao)
        {
            ProdutosdaSolicitacao TmpProdSolicitacao = null;
            if (objProdutosdaSolicitacao.ID.HasValue)
                TmpProdSolicitacao = RepositoryService.ProdutosdaSolicitacao.ObterPor(objProdutosdaSolicitacao.ID.Value);
            //alterado pesquisa pela chave de integração - Sol. José - 14/08/2014

            if (TmpProdSolicitacao != null)
            {
                //Para poder atualizar state posteriormente
                int? stateUpdate = objProdutosdaSolicitacao.State;
                int? razaoStatusUpdate = objProdutosdaSolicitacao.RazaoStatus;
                objProdutosdaSolicitacao.State = TmpProdSolicitacao.State;
                objProdutosdaSolicitacao.RazaoStatus = TmpProdSolicitacao.RazaoStatus;

                objProdutosdaSolicitacao.ID = TmpProdSolicitacao.ID;
                RepositoryService.ProdutosdaSolicitacao.Update(objProdutosdaSolicitacao);

                //Retorna o state e razao do update
                objProdutosdaSolicitacao.State = stateUpdate;
                objProdutosdaSolicitacao.RazaoStatus = razaoStatusUpdate;

                //Se statusCode for diferente do atual altera
                if (objProdutosdaSolicitacao.RazaoStatus.HasValue
                    && objProdutosdaSolicitacao.State.HasValue
                    && (!TmpProdSolicitacao.State.Equals(objProdutosdaSolicitacao.State)
                        || !TmpProdSolicitacao.RazaoStatus.Equals(objProdutosdaSolicitacao.RazaoStatus)))
                    this.MudarStatus(objProdutosdaSolicitacao.ID.Value, objProdutosdaSolicitacao.State.Value, objProdutosdaSolicitacao.RazaoStatus.Value);
                return TmpProdSolicitacao;
            }
            else
                objProdutosdaSolicitacao.ID = RepositoryService.ProdutosdaSolicitacao.Create(objProdutosdaSolicitacao);
            return objProdutosdaSolicitacao;
        }

        public void Deletar(Guid produtoDaSolicitacaoId)
        {
            RepositoryService.ProdutosdaSolicitacao.Delete(produtoDaSolicitacaoId);
        }

        public bool MudarStatus(Guid id, int stateCode, int statusCode)
        {
            return RepositoryService.ProdutosdaSolicitacao.AlterarStatus(id, stateCode, statusCode);
        }

        public List<ProdutosdaSolicitacao> ListarPorSolicitacao(Guid solicitacaoId)
        {
            return RepositoryService.ProdutosdaSolicitacao.ListarPorSolicitacao(solicitacaoId);
        }

        public List<ProdutosdaSolicitacao> ListarPorSolicitacaoAtivos(Guid solicitacaoId)
        {
            return RepositoryService.ProdutosdaSolicitacao.ListarPorSolicitacaoAtivos(solicitacaoId);
        }

        public void AtualizarValoresSolicitacao(ProdutosdaSolicitacao produtoSolicitacao)
        {
            var solicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.ObterPor(produtoSolicitacao, "itbc_status");

            if (solicitacaoBeneficio.StatusSolicitacao.HasValue)
            {
                if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                    || solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise)
                {
                    solicitacaoBeneficio.IntegrarNoPlugin = produtoSolicitacao.IntegrarNoPlugin;
                    RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficio);
                }
            }
        }

        public void ProdutoSolicitacaoShowRoom(ProdutosdaSolicitacao objProdutoSolicitacao)
        {
            #region Verificações de valores
            if (objProdutoSolicitacao == null)
                throw new ArgumentException("(CRM) Produto Solicitação Vazio");

            if (objProdutoSolicitacao.BeneficioPrograma == null)
                throw new ArgumentException("(CRM) Campo Benefício de Programa obrigatório");

            if (objProdutoSolicitacao.SolicitacaoBeneficio == null)
                throw new ArgumentException("(CRM) Campo Solicitação de Benefício obrigatória");
            #endregion

            Beneficio benefPrograma = new Intelbras.CRM2013.Domain.Servicos.BeneficioService(RepositoryService).ObterPor(objProdutoSolicitacao.BeneficioPrograma.Id);
            #region Verificações de valores
            if (benefPrograma == null)
                throw new ArgumentException("(CRM) Benefício do Programa não encontrado");
            #endregion
            if (benefPrograma.Codigo == (int)Domain.Enum.BeneficiodoPrograma.Codigos.Showroom)
            {
                #region Verificações de valores
                if (objProdutoSolicitacao.Produto == null)
                    throw new ArgumentException("(CRM) Campo Produto obrigatório");
                #endregion

                Product produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(RepositoryService).ObterPor(objProdutoSolicitacao.Produto.Id);

                #region Verificações de valores
                if (produto == null)
                    throw new ArgumentException("(CRM) Produto " + objProdutoSolicitacao.Produto.Name.ToString() + " não encontrado");

                if (produto.Segmento == null)
                    throw new ArgumentException("(CRM) Campo segmento do Produto não preenchido,operação cancelada");
                #endregion

                Segmento objSegmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(RepositoryService).ObterPor(produto.Segmento.Id);

                #region Verificações de valores
                if (objSegmento == null)
                    throw new ArgumentException("(CRM) Segmento do Produto não encontrado");
                #endregion

                ParametroGlobal paramIntervalo = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.PrazoPermitidoNovaCompraShowroom, null, null, null, null, null, null, null);
                #region Verificações de valores
                if (paramIntervalo == null)
                    throw new ArgumentException("(CRM) Parâmetro Global não encontrado para 'Prazo permitido para nova compra de showroom'.Operação Cancelada");
                #endregion

                SolicitacaoBeneficio objSolicBenef = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(RepositoryService).ObterPor(objProdutoSolicitacao.SolicitacaoBeneficio.Id);

                #region Verificação de valores
                if (objSolicBenef == null)
                    throw new ArgumentException("(CRM) Solicitação Benefício não encontrada");

                if (objSolicBenef.BeneficioCanal == null)
                    throw new ArgumentException("(CRM) Benefício do Canal não encontrado");

                #endregion

                List<SolicitacaoBeneficio> solicBenef = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(RepositoryService).ListarPorBeneficioCanalEStatus(objSolicBenef.BeneficioCanal.Id, objSolicBenef.BeneficioPrograma.Id, (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado);

                if (solicBenef.Count > 0)
                {
                    List<SolicitacaoBeneficio> tmpSolicBenef = new List<SolicitacaoBeneficio>();
                    foreach (SolicitacaoBeneficio item in solicBenef)
                    {
                        //DateTime.Compare(item.DataCriacao, objSolicBenef.DataCriacao);
                        int diferencaDias = Math.Abs(item.DataCriacao.Value.Subtract(objSolicBenef.DataCriacao.Value).Days);
                        if ((diferencaDias >= Convert.ToInt32(paramIntervalo.Valor)))
                        {
                            //Lista com os beneficios que serão verificados
                            tmpSolicBenef.Add(item);
                        }
                    }

                    int qtdMaxima;

                    if (objSegmento.QtdMaximaShowRoom.HasValue)
                        qtdMaxima = objSegmento.QtdMaximaShowRoom.Value;
                    else
                    {
                        //Fluxo alternativo 2
                        ParametroGlobal paramGlobal = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.QuantidadeKitsShowroomPorSegmento, null, null, null, null, null, null, null);
                        if (paramGlobal == null)
                            throw new ArgumentException("(CRM) Parâmetro Global não encontrado para Quantidade de Kits para Showroom.Operação Cancelada");

                        qtdMaxima = Convert.ToInt32(paramGlobal.Valor);
                    }

                    #region Agrupa Produtos por Segmento do Produto

                    decimal qtdProdutosSegmento = 0;
                    if (tmpSolicBenef.Count > 0)
                        foreach (var item in tmpSolicBenef)
                        {
                            List<ProdutosdaSolicitacao> produtosSolic = new Intelbras.CRM2013.Domain.Servicos.ProdutosdaSolicitacaoService(RepositoryService).ListarPorSolicitacao(item.ID.Value);

                            if (produtosSolic.Count > 0)
                                foreach (var produtoSolic in produtosSolic)
                                {
                                    if (produtoSolic.Produto == null)
                                        throw new ArgumentException("Produto da Solicitação não cadastrado.");

                                    Product _produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(RepositoryService).ObterPor(produtoSolic.Produto.Id);
                                    if (_produto == null)
                                        throw new ArgumentException("Produto não encontrado");

                                    if (_produto.Segmento == null)
                                        throw new ArgumentException("Segmento do Produto " + _produto.Nome + " não preenchido");

                                    if (_produto.Segmento.Id == objSegmento.ID.Value)
                                        qtdProdutosSegmento += produtoSolic.QuantidadeAprovada.Value;

                                }
                        }
                    #endregion

                    if (qtdProdutosSegmento > qtdMaxima)
                        throw new ArgumentException("(CRM) Limite de compras de showroom atingido para este segmento.Operação bloqueada.");
                }
            }
        }

        public ProdutoEstabelecimento ObterEstabelecimento(ProdutosdaSolicitacao produtoSolicitacao)
        {
            if (produtoSolicitacao.Produto != null)
            {
                return RepositoryService.ProdutoEstabelecimento.ObterPorProduto(produtoSolicitacao.Produto.Id);
            }

            return null;
        }

        public void ValidaCamposObrigatorios(ProdutosdaSolicitacao produtoSolicitacao)
        {
            if (produtoSolicitacao.Produto == null)
            {
                throw new ArgumentException("(CRM) O produto é obrigatório.");
            }

            if (produtoSolicitacao.SolicitacaoBeneficio == null)
            {
                throw new ArgumentException("(CRM) A solicitação de benefício é obrigatória.");
            }

            if (produtoSolicitacao.BeneficioPrograma == null)
            {
                throw new ArgumentException("(CRM) A benefício do programa é obrigatória.");
            }

            if (!produtoSolicitacao.QuantidadeSolicitada.HasValue || produtoSolicitacao.QuantidadeSolicitada.Value <= 0)
            {
                throw new ArgumentException("(CRM) A quantidade solicitada precisa ser maior que zero.");
            }

            if (!produtoSolicitacao.QuantidadeAprovada.HasValue || produtoSolicitacao.QuantidadeAprovada.Value <= 0)
            {
                throw new ArgumentException("(CRM) A quantidade aprovada precisa ser maior que zero.");
            }
        }

        public void ValidaIntegridadeDados(ProdutosdaSolicitacao produtoSolicitacao, out ProdutoPortfolio produtoPortfolio)
        {
            produtoPortfolio = null;
            var solicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(produtoSolicitacao.SolicitacaoBeneficio.Id);
            var beneficio = RepositoryService.Beneficio.ObterPor(produtoSolicitacao.BeneficioPrograma.Id);
            var produto = RepositoryService.Produto.Retrieve(produtoSolicitacao.Produto.Id);
            var canal = RepositoryService.Conta.Retrieve(solicitacaoBeneficio.Canal.Id);

            if (solicitacaoBeneficio.UnidadedeNegocio.Id != produto.UnidadeNegocio.Id && solicitacaoBeneficio.UnidadedeNegocio.Name != "ADMINISTRATIVO" && canal.Classificacao.Name != "Revendas")
            {
                throw new ArgumentException("(CRM) Não é permitido incluir produto em uma solicitação quando a unidade de negócio é diferente! Produto: [" + produto.Codigo + "]");
            }

            if (solicitacaoBeneficio.AjusteSaldo.HasValue && solicitacaoBeneficio.AjusteSaldo.Value)
            {
                if (beneficio.Codigo.Value == (int)Enum.BeneficiodoPrograma.Codigos.PriceProtection)
                {
                    if (!solicitacaoBeneficio.TipoPriceProtection.HasValue 
                        || (solicitacaoBeneficio.TipoPriceProtection.HasValue && solicitacaoBeneficio.TipoPriceProtection.Value != (int)Enum.BeneficiodoPrograma.TipoPriceProtection.Autorizacao))
                        throw new ArgumentException("(CRM) Não é permitido incluir produtos em uma solicitação de ajuste! Produto: [" + produto.Codigo + "]");
                }
                else
                {
                    throw new ArgumentException("(CRM) Não é permitido incluir produtos em uma solicitação de ajuste! Produto: [" + produto.Codigo + "]");
                }
            }

           

            if (solicitacaoBeneficio.FormaPagamento != null)
            {
                if (solicitacaoBeneficio.FormaPagamento.Name != Enum.SolicitacaoBeneficio.FormaPagamento.Produto)
                {
                    throw new ArgumentException("(CRM) Não é permitido incluir produtos em uma solicitação com forma de pagamento diferente de produto! Produto: [" + produto.Codigo + "]");
                }
            }

            switch (beneficio.Codigo.Value)
            {
                case (int)Enum.BeneficiodoPrograma.Codigos.Showroom:

                    if (canal.Classificacao.Name.Contains("Distribuidor"))
                    {
                        if (!produto.Showroom.HasValue || !produto.Showroom.Value)
                        {
                            throw new ArgumentException("(CRM) Produto informado não pode ser adquirido para Show Room. Produto: [" + produto.Codigo + "]");
                        } 
                    }
                    else if (canal.Classificacao.Name == "Revendas")
                    {
                        if (!produto.ShowroomRevenda.HasValue || !produto.ShowroomRevenda.Value)
                        {
                            throw new ArgumentException("(CRM) Produto informado não pode ser adquirido para Show Room. Produto: [" + produto.Codigo + "]");
                        }
                    }
                    break;

                case (int)Enum.BeneficiodoPrograma.Codigos.StockRotation:
                    if (produtoSolicitacao.Fatura == null)
                    {
                        throw new ArgumentException("(CRM) Nota fiscal não informada. Produto: [" + produto.Codigo + "]");
                    }

                    ValidaDadosDaNotaFiscal(produtoSolicitacao, produto);
                    break;

                case (int)Enum.BeneficiodoPrograma.Codigos.Backup:

                    if (canal.Classificacao.Name.Contains("Distribuidor"))
                    {
                        if (!produto.BackupDistribuidor.HasValue || !produto.BackupDistribuidor.Value)
                        {
                            throw new ArgumentException("(CRM) Produto informado não pode ser adquirido para Backup. Produto: [" + produto.Codigo + "]");
                        }
                    }
                    else if (canal.Classificacao.Name == "Revendas")
                    {
                        if (!produto.BackupRevenda.HasValue || !produto.BackupRevenda.Value)
                        {
                            throw new ArgumentException("(CRM) Produto informado não pode ser adquirido para Backup. Produto: [" + produto.Codigo + "]");
                        }
                    }
                    break;
                default:
                    produtoPortfolio = ObterProdutoPortforlio(solicitacaoBeneficio, produto.ID.Value);

                    if (produtoPortfolio == null)
                    {
                        throw new ArgumentException("(CRM) Produto [" + produto.Codigo + "] informado não pertence ao portfólio do cliente.");
                    }

                    if (beneficio.Codigo.Value != (int)Enum.BeneficiodoPrograma.Codigos.PriceProtection
                     && beneficio.Codigo.Value != (int)Enum.BeneficiodoPrograma.Codigos.Backup)
                    {
                        if(canal.Classificacao.Name != "Revendas")
                        ValidaQuantidadeMultipla(produtoSolicitacao, produto);
                    }
                    break;
            }
        }

        public ProdutoPortfolio ObterProdutoPortforlio(SolicitacaoBeneficio solicitacaoBeneficio, Guid produtoId)
        {
            Conta canal = RepositoryService.Conta.Retrieve(solicitacaoBeneficio.Canal.Id);
            var produtoService = new ProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            List<ProdutoPortfolio> lstProdutoPortifolio = produtoService.ProdutosPortfolio(canal, canal.Classificacao.Id, solicitacaoBeneficio.UnidadedeNegocio.Id);

            return lstProdutoPortifolio.Find(x => x.Produto.Id == produtoId);
        }

        public void ValidaQuantidadeMultipla(ProdutosdaSolicitacao produtoSolicitacao, Product produto = null)
        {
            if (produto == null)
            {
                produto = RepositoryService.Produto.Retrieve(produtoSolicitacao.Produto.Id);
            }

            if (produtoSolicitacao.QuantidadeSolicitada.HasValue)
            {
                new ProdutoService(RepositoryService).ValidarQuantidadeMultipla(produto, produtoSolicitacao.QuantidadeSolicitada.Value);
            }

            if (produtoSolicitacao.QuantidadeAprovada.HasValue)
            {
                new ProdutoService(RepositoryService).ValidarQuantidadeMultipla(produto, produtoSolicitacao.QuantidadeAprovada.Value);
            }
        }

        public void ValidaDadosDaNotaFiscal(ProdutosdaSolicitacao produtoSolicitacao, Product produto)
        {
            if (produtoSolicitacao.Fatura != null && produtoSolicitacao.Produto != null)
            {
                var produtoFatura = RepositoryService.ProdutoFatura.ObterObtemPorNotaFiscal(produtoSolicitacao.Produto.Id, produtoSolicitacao.Fatura.Id);
                var fatura = RepositoryService.Fatura.Retrieve(produtoSolicitacao.Fatura.Id, "name");

                if (produtoFatura == null)
                {
                    throw new ArgumentException("(CRM) Não encontrado o produto [" + produto.Codigo + "] na nota fiscal [" + fatura.NumeroNF + "]");
                }

                if (produtoSolicitacao.QuantidadeSolicitada.HasValue)
                {
                    if (produtoSolicitacao.QuantidadeSolicitada.Value > produtoFatura.Quantidade.Value)
                    {
                        throw new ArgumentException("(CRM) A quantidade solicitada do produto [" + produto.Codigo + "] é maior que a quantidade da nota fiscal [" + fatura.NumeroNF + "]");
                    }
                }
            }
        }

        private decimal? ObterPercentualDescontoValorSolicitado(Beneficio beneficio, UnidadeNegocio unidadeNegocio, Classificacao classificacao)
        {
            decimal? fator = null;

            switch (beneficio.Codigo)
            {
                case (int)Enum.BeneficiodoPrograma.Codigos.Showroom:
                    var fatorShowRoom = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.PercentualDescontoShowRoom, unidadeNegocio.ID.Value, classificacao.ID.Value, null, null, null, beneficio.ID, null);

                    if (fatorShowRoom == null)
                    {
                        throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Show Room não localizado para Unidade de Negócio [" + unidadeNegocio.Nome + "]");
                    }

                    fator = Convert.ToDecimal(fatorShowRoom.Valor);

                    break;

                case (int)Enum.BeneficiodoPrograma.Codigos.Backup:
                    var fatorBackup = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.PercentualDescontoBackup, unidadeNegocio.ID.Value, classificacao.ID.Value, null, null, null, beneficio.ID, null);

                    if (fatorBackup == null)
                    {
                        throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Backup não localizado para Unidade de Negócio [" + unidadeNegocio.Nome + "]");
                    }

                    fator = Convert.ToDecimal(fatorBackup.Valor);

                    break;
            }

            return fator;
        }

        public void InativarTodosPorSolicitacao(Guid solicitacaoId)
        {
            var lstProdutos = this.ListarPorSolicitacaoAtivos(solicitacaoId);

            foreach (var item in lstProdutos)
            {
                RepositoryService.ProdutosdaSolicitacao.AlterarStatus(item.ID.Value, (int)Enum.ProdutoSolicitacao.Status.Inativo, (int)Enum.ProdutoSolicitacao.StatusCode.Inativo);
            }
        }

        public bool ExisteDuplicidade(ProdutosdaSolicitacao produtoSolicitacao)
        {
            int resultado = 0;

            if (produtoSolicitacao.SolicitacaoBeneficio != null && produtoSolicitacao.Produto != null)
            {
                var lista = RepositoryService.ProdutosdaSolicitacao.ListarPorSolicitacao(produtoSolicitacao.SolicitacaoBeneficio.Id);

                if (produtoSolicitacao.ID.HasValue)
                {
                    lista.RemoveAll(x => x.ID.Value == produtoSolicitacao.ID.Value);
                }

                if (produtoSolicitacao.Fatura == null)
                {
                    resultado = lista.Count(x => x.Produto.Id == produtoSolicitacao.Produto.Id);
                }
                else
                {
                    resultado = lista.Count(x => x.Produto.Id == produtoSolicitacao.Produto.Id && x.Fatura.Id == produtoSolicitacao.Fatura.Id);
                }
            }

            return resultado == 0 ? false : true;
        }

        #endregion
    }
}
