using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0087 : Base, IBase<Pollux.MSG0087, Model.ListaPrecoPSDPPPSCF>
    {
        
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        RepositoryService _Repository = null;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0087(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
            _Repository = new RepositoryService(org, isOffline);
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
            #region linhas seram removidas pois não existe mais um recebimento de acordo com a ESPEC - ITBES0012
            //usuarioIntegracao = usuario;
            //Estado estado = null;
            //Moeda moeda = null;
            //UnidadeNegocio undadeNegocio = null;
            //Guid? produtoId = null;
            //Intelbras.Message.Helper.MSG0087 xml = this.CarregarMensagem<Pollux.MSG0087>(mensagem);

            #region Validações

            ////Estado
            //if (xml.Estados.Count() > 0)
            //{
            //    estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.Estados.First());
            //    if (estado == null)
            //    {
            //    resultadoPersistencia.Sucesso = false;
            //        //resultadoPersistencia.Mensagem = "Estado: " + xml.Estado +" não encontrado no Crm.";
            //        retorno.Add("Resultado", resultadoPersistencia);
            //        return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //    }
            //}
            //else 
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Estados obrigatórios";
            //    retorno.Add("Resultado", resultadoPersistencia);
            //    return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //}

            ////Unidade de Negocio
            //if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            //{
            //    undadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
            //    if (undadeNegocio == null)
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "UnidadeNegocio: " + xml.CodigoUnidadeNegocio + " não encontrado no Crm.";
            //        retorno.Add("Resultado", resultadoPersistencia);
            //        return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //    }
            //}
            //else 
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "CodigoUnidadeNegocio não enviado ou fora do padrão(Guid).";
            //    retorno.Add("Resultado", resultadoPersistencia);
            //    return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //}

            #region Comentarios já existentes antes dessa alteração(Willer)
            //Moeda


            //if (!String.IsNullOrEmpty(xml.Moeda))
            //{
            //    moeda = new Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);
            //    if (moeda == null)
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Moeda: " + xml.Moeda + " não encontrada no Crm.";
            //        retorno.Add("Resultado", resultadoPersistencia);
            //        return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Moeda não enviado ou fora do padrão(Guid).";
            //    retorno.Add("Resultado", resultadoPersistencia);
            //    return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //}

            //Produto - não obrigatório
            //if (!String.IsNullOrEmpty(xml.CodigoProduto))
            //{
            //    Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.CodigoProduto);
            //    if (produto == null)
            //{
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Produto código: " + xml.CodigoProduto + " não encontrado no Crm.";
            //        retorno.Add("Resultado", resultadoPersistencia);
            //        return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            //    }
            //    else
            //        produtoId = produto.ID.Value;
            //}
            #endregion
            #endregion

            //List<Intelbras.Message.Helper.Entities.ProdutoItemPSD> lsTProdutos = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarPSD0087(undadeNegocio.ID.Value, moeda.ID.Value, estado.ID.Value, produtoId);

            //if (lsTProdutos != null && lsTProdutos.Count > 0)
            //{
            //    retorno.Add("ProdutosItens", lsTProdutos);
            //    resultadoPersistencia.Sucesso = true;
            //    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = true;
            //    resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
            //}
            //retorno.Add("Resultado", resultadoPersistencia);

            //return CriarMensagemRetorno<Pollux.MSG0087R1>(numeroMensagem, retorno);
            #endregion

            throw new NotImplementedException();
        }

        //Nao é usado em nenhum lugar mas nao sei se será usado no futuro , deixei aqui na dúvida
        //private List<Intelbras.Message.Helper.Entities.ProdutoR1> ConverteLista(List<Product> lstProduto)
        //{
        //    List<Intelbras.Message.Helper.Entities.ProdutoR1> lstPollux = new List<Intelbras.Message.Helper.Entities.ProdutoR1>();

        //    foreach (Product fnCon in lstProduto)
        //    {
        //        Pollux.Entities.ProdutoR1 polluxObj = new Pollux.Entities.ProdutoR1();
        //        polluxObj.CodigoProduto = fnCon.Codigo;
        //        polluxObj.NomeProduto = fnCon.Nome;
        //        polluxObj.Valor = (decimal)fnCon.CustoAtual;

        //        lstPollux.Add(polluxObj);
        //    }

        //    return lstPollux;
        //}
        #endregion

        #region Definir Propriedades

        public ListaPrecoPSDPPPSCF DefinirPropriedades(Intelbras.Message.Helper.MSG0087 xml)
        {
            throw new NotImplementedException();
        }

        public Intelbras.Message.Helper.MSG0087 DefinirPropriedadesPlugin(ListaPrecoPSDPPPSCF objModel)
        {
            Pollux.MSG0087 xml = new Pollux.MSG0087(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(objModel.Nome, 40));
            var unidadeNegocioModel = _Repository.UnidadeNegocio.Retrieve(objModel.UnidadeNegocio.Id);
            xml.CodigoListaPreco = objModel.ID.Value.ToString();
            xml.CodigoUnidadeNegocio = unidadeNegocioModel.ChaveIntegracao;
            if (objModel.DataFim.HasValue)
                xml.DataFinalVigencia = objModel.DataFim.Value.ToLocalTime();
            else
                xml.DataFinalVigencia = null;
            if (objModel.DataInicio.HasValue)
                xml.DataInicioVigencia = objModel.DataInicio.Value.ToLocalTime();
            else
                xml.DataInicioVigencia = null;
            xml.Estados = ListarEstados(objModel);
            xml.ListaPrecoItens = ListarProdutos(objModel);
            xml.Situacao = (objModel.Status.HasValue ? objModel.Status.Value : 0);

            return xml;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ListaPrecoPSDPPPSCF objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0087 mensagem = this.DefinirPropriedadesPlugin(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0087R1 retorno = CarregarMensagem<Pollux.MSG0087R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException(string.Concat("(CRM) Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }

        private List<string> ListarEstados(ListaPrecoPSDPPPSCF objModel)
        {
            var estadosDaListaCRM = _Repository.Estado.ListarPor(objModel);
            var listaChavesIntegracao = new List<string>();

            foreach (var estadoCRM in estadosDaListaCRM)
            {
                listaChavesIntegracao.Add(estadoCRM.ChaveIntegracao);
            }

            return listaChavesIntegracao;
        }

        private List<Pollux.Entities.ListaPrecoItem> ListarProdutos(ListaPrecoPSDPPPSCF objModel)
        {
            var itensDaListaCRM = _Repository.ProdutoListaPSD.ListarPor(objModel.ID.Value, null);
            List<Pollux.Entities.ListaPrecoItem> listaItensXml = new List<Pollux.Entities.ListaPrecoItem>();

            foreach (var itemCRM in itensDaListaCRM)
            {
                listaItensXml.Add(MontarItemDaLista(itemCRM));
            }

            return listaItensXml;
        }

        private Pollux.Entities.ListaPrecoItem MontarItemDaLista(ProdutoListaPSDPPPSCF produtoDaListaPSDCRM)
        {
            Pollux.Entities.ListaPrecoItem itemLista = new Pollux.Entities.ListaPrecoItem();

            var produtoCRM = _Repository.Produto.Retrieve(produtoDaListaPSDCRM.Produto.Id);

            itemLista.CodigoProduto = produtoCRM.Codigo;
            itemLista.ControlaPSD = produtoDaListaPSDCRM.ControlaPSD;
            itemLista.ValorPP = produtoDaListaPSDCRM.ValorPP.HasValue ? produtoDaListaPSDCRM.ValorPP.Value : 0;
            itemLista.ValorPSCF = produtoDaListaPSDCRM.ValorPSCF.HasValue ? produtoDaListaPSDCRM.ValorPSCF.Value : 0;
            itemLista.ValorPSD = produtoDaListaPSDCRM.ValorPSD.HasValue ? produtoDaListaPSDCRM.ValorPSD.Value : 0;

            return itemLista;
        }
        #endregion

    }
}
