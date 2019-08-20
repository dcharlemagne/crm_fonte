using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0100 : Base, IBase<Message.Helper.MSG0100, Domain.Model.ProdutoPortfolio>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        Guid? unidadeNegocioId = null;
        Guid? classificacaoId = null;
        private LinhaCorteDistribuidor objRetorno;

        #endregion

        #region Construtor

        public MSG0100(string org, bool isOffline)
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

        #region Executar
        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {

            var xml = this.CarregarMensagem<Pollux.MSG0100>(mensagem);
            List<ProdutoPortfolio> lstProdutoPortifolio = null;
            List<Pollux.Entities.ProdutoPortifolio> lstPolluxPortifolio = new List<Pollux.Entities.ProdutoPortifolio>();

            ProdutoPortfolio objeto = this.DefinirPropriedades(xml);

            if (!String.IsNullOrEmpty(xml.Classificacao))
            {
                Classificacao classif = new Servicos.ClassificacaoService(this.Organizacao, this.IsOffline).BuscaClassificacao(new Guid(xml.Classificacao));
                if (classif != null)
                    classificacaoId = classif.ID.Value;
                else
                { 
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Classificação - " + xml.Classificacao + " - não cadastrada no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                //new 
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "O campo Classificação é obrigatorio.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
            }



            if (!String.IsNullOrEmpty(xml.UnidadeNegocio))
            {
                UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.UnidadeNegocio);
                if (unidadeNeg != null)
                    unidadeNegocioId = unidadeNeg.ID.Value;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio - " + xml.UnidadeNegocio + " - não cadastrada no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                }
            }

             

            if (!String.IsNullOrEmpty(xml.Conta))
            {
                Conta objetoConta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.Conta));
                
                if (objetoConta != null && objetoConta.Status.Value == 0)
                {
                    #region Conta sem categoria de canal ativa
                    //Regra: É preciso recuperar as unidades de negócios com qual o canal possui relacionamento que estão registradas na entidade "Categorias do Canal". 
                    //Se não forem encontrados registros, o calculo do portfólio deve ser interrompido e deve ser retornado ao ESB como resultado "Falha"."
                    var categoriaCanal = new Servicos.CategoriaCanalService(this.Organizacao, this.IsOffline).ListarPor(objetoConta.ID, unidadeNegocioId);
                    if (categoriaCanal == null || categoriaCanal.Count.Equals(0))
                    {
                        resultadoPersistencia.Sucesso = false;                        
                        resultadoPersistencia.Mensagem = "(CRM):Conta não possui relacionamento com unidades de negócio Intelbras";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                    }
                    #endregion

                    #region Nenhum portfólio encontrado
                    if (unidadeNegocioId != null)
                    {
                        var portifolio = new Servicos.PortfolioService(this.Organizacao, this.IsOffline).ListPortifolios(unidadeNegocioId.Value, classificacaoId.Value);
                        if (portifolio.Count == 0)
                        {

                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "(CRM): Não foram encontrados portfólios para a classificação da conta";
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                        }
                    }
                    #endregion

                    #region Nenhum produto de portfólio encontrado

                    lstProdutoPortifolio = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ProdutosPortfolio(objetoConta, classificacaoId, unidadeNegocioId);

                    if (lstProdutoPortifolio != null && lstProdutoPortifolio.Count > 0)
                    {
                        lstPolluxPortifolio = this.CarregarListaProdutoPortifolio(lstProdutoPortifolio);
                    }
                    else
                    {
                        //verifica se existe portfolios sem produto 
                        var portifolio = new Servicos.PortfolioService(this.Organizacao, this.IsOffline).VerificaPortfolio(objetoConta, classificacaoId, unidadeNegocioId);

                        if (portifolio.Count > 0)
                        {
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "(CRM): Foram encontrados portfólios mas nenhum produto relacionado aos mesmos.";
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                        }
                        else
                        {
                            //Erro Lista Portifolio vazia
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "(CRM): Não foram encontrados portfólios para a classificação da conta";
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                        }
                    }
                    #endregion      
                }
                else
                {
                    //Conta inativa ou não existe
                    //Conta nao encontrada
                    resultadoPersistencia.Sucesso = false;                    
                    resultadoPersistencia.Mensagem = "(CRM): Conta não existente e/ ou inativa";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
                }
                    
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Canal não enviado.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
            }

            if (lstPolluxPortifolio.Count > 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("ProdutoItem", lstPolluxPortifolio);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Produtos Itens não encontrados no Crm.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0100R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public ProdutoPortfolio DefinirPropriedades(Intelbras.Message.Helper.MSG0100 xml)
        {
            return new ProdutoPortfolio(this.Organizacao, this.IsOffline);
        }

        #endregion

        #region Métodos Auxiliares


        public List<Pollux.Entities.ProdutoPortifolio> CarregarListaProdutoPortifolio(List<ProdutoPortfolio> lstProdutoPortifolio)
        {
            List<Pollux.Entities.ProdutoPortifolio> lstPolluxPortifolio = new List<Pollux.Entities.ProdutoPortifolio>();

            List<Guid> lstProdutoListaIds = new List<Guid>();
            List<Product> lstProduto = new List<Product>();
            List<Product> lstProdutoCompleto = new List<Product>();

            foreach (var prodItem in lstProdutoPortifolio)
            {
                
                lstProdutoListaIds.Add(prodItem.Produto.Id);
            }

            

            if (lstProdutoListaIds.Count > 0)
                lstProduto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarProduto(lstProdutoListaIds);


            List<ProdutoTreinamento> lstProdTreinamentoCache = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ProdutoTreinamentoListarTodos();
            List<TreinamentoCanal> lstTreinamentoCanalCache = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).TreinamentoCanalListarTodos();

            foreach (Product produto in lstProduto)
            {               
            
                //Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(item.Produto.Id);
                Pollux.Entities.ProdutoPortifolio objPollux = new Pollux.Entities.ProdutoPortifolio();

                objPollux.CodigoProduto = produto.Codigo;
                objPollux.Bloqueado = false;
                objPollux.TemCache = false;
                objPollux.TipoPortfolio = lstProdutoPortifolio.Where(x => x.Produto.Id == produto.ID.Value).First().PortfolioTipo;

                if (produto.ExigeTreinamento.Value)
                {
                    
                    //Para cada um que requere certificação, localiza a lista de Treinamentos/Certificações requeridas na entidade Produtos x Treinamento/Certificação. 
                    List<ProdutoTreinamento> lstProdTreinamento = lstProdTreinamentoCache.Where(x => x.Produto.Id == produto.ID.Value).ToList();
                    //RepositoryService.ProdutoTreinamento.ListarPorProduto(prodPort.Produto.Id);
                    foreach (ProdutoTreinamento prodTreinamento in lstProdTreinamento)
                    {
                        List<TreinamentoCanal> lstTreinamentoCanal = lstTreinamentoCanalCache.Where(y => y.Treinamento.Id == prodTreinamento.Treinamento.Id).
                                                                                              Where(y => y.Canal.Id == lstProdutoPortifolio.First().CanalId).ToList();
                        //RepositoryService.TreinamentoCanal.ListarPor((Guid)prodTreinamento.Treinamento.Id, (Guid)canalId);

                        //Para cada Treinamento encontrado, verifica quais estão com Status igual a “Não cumprido” na entidade Treinamentos do Canal (de mesmo Canal e Treinamento obtidos).
                        if (lstTreinamentoCanal.Count(x => x.StatusCompromisso.Name == Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido) > 0)
                        {
                            //Remove da lista de produtos do portfólio do passo 8, todos os produtos encontrados sem treinamento no passo 9 (remove da lista, não da base do CRM).
                            //E apenas bloqueia o portfolio se tiver o flag marcado como sim
                            //Product prodBloq = RepositoryService.Produto.ObterPor(prodTreinamento.Produto.Id);

                            if (prodTreinamento.BloqueiaPortfolio.Value == (int)Enum.ProdutoTreinamento.BloqueiaPortfolio.Sim)
                                objPollux.Bloqueado = true;
                        }
                    }
                }
           
                lstPolluxPortifolio.Add(objPollux);
            }


            return lstPolluxPortifolio;
        }

        public string Enviar(ProdutoPortfolio objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
