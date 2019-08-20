using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0287 : Base, IBase<Message.Helper.MSG0287, Domain.Model.ProdutoCondicaoPagamento>
    {
        #region Construtor

        public MSG0287(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
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

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            throw new NotImplementedException();
        }

        #endregion

        public ProdutoCondicaoPagamento DefinirPropriedades(Intelbras.Message.Helper.MSG0287 xml)
        {
            
            throw new NotImplementedException();
        }

        public Pollux.MSG0287 DefinirPropriedades(ProdutoCondicaoPagamento crm)
        {
            var condPagamento = new Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamento(crm.CondicaoPagamento.Id);
            if(condPagamento == null)
            {
                throw new ArgumentException("(CRM) Condição de Pagamento Não Localizada");
            }

            var produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(crm.Produto.Id);
            if (produto == null)
            {
                throw new ArgumentException("(CRM) Produto Não Localizada");
            }

            Pollux.MSG0287 retMsg = new Pollux.MSG0287(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), condPagamento.Codigo.ToString() + "+" + produto.Codigo.ToString());
            retMsg.CodigoCondicaoPagamento = condPagamento.Codigo;
            retMsg.CodigoProduto = produto.Codigo;
            retMsg.Situacao = crm.Status;

            return retMsg;
        }

        public string Enviar(ProdutoCondicaoPagamento objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0287 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0287R1 retorno = CarregarMensagem<Pollux.MSG0287R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }

            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + erro001.GenerateMessage(false));
            }
            return resposta;
        }

    }
}
