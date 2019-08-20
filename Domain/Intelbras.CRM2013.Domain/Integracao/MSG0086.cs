using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0086 : Base, IBase<Intelbras.Message.Helper.MSG0086, Domain.Model.Product>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0086(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            usuarioIntegracao = usuario;
            Estado estado = null;
            Moeda moeda = null;
            UnidadeNegocio undadeNegocio = null;
            Guid? produtoId = null;
            Intelbras.Message.Helper.MSG0086 xml = this.CarregarMensagem<Pollux.MSG0086>(mensagem);
            #region Validações
            //Estado
            if (!String.IsNullOrEmpty(xml.Estado))
            {
                estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.Estado);
                if (estado == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estado: " + xml.Estado + " não encontrado no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Estado obrigatório";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
            }
            //Unidade de Negocio
            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                undadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (undadeNegocio == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeNegocio: " + xml.CodigoUnidadeNegocio + " não encontrado no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoUnidadeNegocio não enviado ou fora do padrão(Guid).";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
            }

            //Moeda


            if (!String.IsNullOrEmpty(xml.Moeda))
            {
                moeda = new Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);
                if (moeda == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda: " + xml.Moeda + " não encontrada no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Moeda não enviado ou fora do padrão(Guid).";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
            }

            //Produto - não obrigatório
            if (!String.IsNullOrEmpty(xml.CodigoProduto))
            {
                Product produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.CodigoProduto);
                if (produto == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Produto código: " + xml.CodigoProduto + " não encontrado no Crm.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
                }
                else
                    produtoId = produto.ID.Value;
            }
            #endregion

            List<Intelbras.Message.Helper.Entities.ProdutoR1> lsTProdutos = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ListarPSDPPPSCF(undadeNegocio.ID.Value, moeda.ID.Value, estado.ID.Value, produtoId);

            if (lsTProdutos != null && lsTProdutos.Count > 0)
            {
                retorno.Add("ProdutosItens", lsTProdutos);
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
            }
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0086R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Product DefinirPropriedades(Intelbras.Message.Helper.MSG0086 xml)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Product objModel)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}