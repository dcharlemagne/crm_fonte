using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0140 : Base, IBase<Intelbras.Message.Helper.MSG0140, Domain.Model.ProdutoEstabelecimento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0140(string org, bool isOffline)
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
            resultadoPersistencia.Mensagem = "Ação de integração não permitida";
            resultadoPersistencia.Sucesso = false;
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0140R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public ProdutoEstabelecimento DefinirPropriedades(Intelbras.Message.Helper.MSG0140 xml)
        {
            var crm = new ProdutoEstabelecimento(this.Organizacao, this.IsOffline);

           

            #region Propriedades Crm->Xml


            #endregion

            return crm;
        }

        public Pollux.MSG0140 DefinirPropriedades(ProdutoEstabelecimento objModel)
        {
            #region Propriedades Crm->Xml
            Product produto = null;
            if (objModel.Produto != null)
            {
                produto = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(objModel.Produto.Id);
            }
            else
            {
                throw new Exception("Produto não encontrado!");
            }
            Pollux.MSG0140 msg0140 = new Pollux.MSG0140(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(produto.Codigo, 40));
            msg0140.CodigoProduto = produto.Codigo;
            if (objModel.Estabelecimento != null)
            {
                Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimento(objModel.Estabelecimento.Id);
                if (estabelecimento.Codigo.HasValue)
                msg0140.CodigoEstabelecimento = estabelecimento.Codigo.Value;
            }
            //Não é preciso passar Status - Msg delete por default
            #endregion

                return msg0140;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ProdutoEstabelecimento objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0140 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0140R1 retorno = CarregarMensagem<Pollux.MSG0140R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return retMsg;
        }

        #endregion
    }
}
