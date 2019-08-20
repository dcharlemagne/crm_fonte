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
    public class MSG0139 : Base, IBase<Intelbras.Message.Helper.MSG0139, Domain.Model.ProdutoEstabelecimento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Int32 codigoEstabelecimento;

        #endregion

        #region Construtor
        public MSG0139(string org, bool isOffline)
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
            resultadoPersistencia.Mensagem = "Intergação não permitida!";
            resultadoPersistencia.Sucesso = false;
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0139R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public ProdutoEstabelecimento DefinirPropriedades(Intelbras.Message.Helper.MSG0139 xml)
        {
            var crm = new ProdutoEstabelecimento(this.Organizacao, this.IsOffline);
            return crm;
        }

        public Pollux.MSG0139 DefinirPropriedades(ProdutoEstabelecimento objModel)
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

            Pollux.MSG0139 msg0139 = new Pollux.MSG0139(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(produto.Codigo, 40));

               msg0139.CodigoProduto = produto.Codigo;
            if (objModel.Estabelecimento != null)
            {
                Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimento(objModel.Estabelecimento.Id);
                if (estabelecimento.Codigo.HasValue)
                    msg0139.CodigoEstabelecimento = estabelecimento.Codigo.Value;
            }
            if (objModel.Status.HasValue)
            {
                if (objModel.Status.Value == 1)
                {
                    msg0139.Situacao = 0;
                }
                else 
                {
                    msg0139.Situacao = 1;
                }
            }


            #endregion

            return msg0139;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ProdutoEstabelecimento objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0139 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0139R1 retorno = CarregarMensagem<Pollux.MSG0139R1>(retMsg);
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
