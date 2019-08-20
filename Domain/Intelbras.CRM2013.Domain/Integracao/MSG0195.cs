using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0195 : Base, IBase<Message.Helper.MSG0195, Domain.Model.TabelaPrecoB2B>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor

        public MSG0195(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var tabela = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0195>(mensagem));
            var item = this.DefinirPropriedadesItem(this.CarregarMensagem<Pollux.MSG0195>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0195R1>(numeroMensagem, retorno);
            }

            var resultado = new Servicos.TabelaPrecoB2BService(this.Organizacao, this.IsOffline).Persistir(tabela, item);
            if (resultado == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0195R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public TabelaPrecoB2B DefinirPropriedades(Pollux.MSG0195 xml)
        {
            var crm = new TabelaPrecoB2B(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            // Propriedades da tabela de preço
            crm.CodigoTabelaPrecoEMS = xml.TabelaPrecoEMS;
            crm.Nome = xml.NomeTabela;
            crm.DataInicial = xml.DataInicial;
            crm.DataFinal = xml.DataFinal;
            crm.CodigoMoeda = xml.CodigoMoeda;
            crm.NomeMoeda = xml.NomeMoeda;
            crm.Status = xml.SituacaoTabela;          
            
            #endregion

            return crm;
        }

        public ItemTabelaPrecoB2B DefinirPropriedadesItem(Pollux.MSG0195 xml)
        {
            // Propriedade do item da tabela
            var crm = new ItemTabelaPrecoB2B(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml            
            if (String.IsNullOrEmpty(xml.CodigoProduto))
                return crm;

            if (! String.IsNullOrEmpty(xml.CodigoProduto))
            {
                Product produto = new ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.CodigoProduto);
                if (produto == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "(CRM) Não foi possível recuperar o produto informado.";
                    return crm;
                }
                crm.Produto = new Lookup(produto.ID.Value, "");
            }
            
            if(!String.IsNullOrEmpty(xml.CodigoItemPreco))
                crm.CodigoItemPreco = xml.CodigoItemPreco;
            else
                crm.AddNullProperty("CodigoItemPreco");

            if (xml.PMA.HasValue)
                crm.ValorPMA = xml.PMA;

            if (xml.PMD.HasValue)
                crm.ValorPMD = xml.PMD;
            
            if (xml.PrecoFOB.HasValue)
                crm.PrecoFOB = xml.PrecoFOB;

            if (xml.PrecoMinimoCIF.HasValue)
                crm.PrecoMinimoCIF = xml.PrecoMinimoCIF;

            if (xml.PrecoMinimoFOB.HasValue)
                crm.PrecoMinimoFOB = xml.PrecoMinimoFOB;

            if (xml.PrecoUnico.HasValue)
                crm.PrecoUnico = xml.PrecoUnico;

            if (xml.PrecoVenda.HasValue)
                crm.PrecoVenda= xml.PrecoVenda;

            if (xml.QuantidadeMinima.HasValue)
                crm.QuantidadeMinima = xml.QuantidadeMinima;

            if (xml.SituacaoItem.HasValue)
                crm.Status = xml.SituacaoItem;
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(TabelaPrecoB2B objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
