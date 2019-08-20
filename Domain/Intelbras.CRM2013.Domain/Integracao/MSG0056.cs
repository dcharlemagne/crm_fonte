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
    public class MSG0056 : Base, IBase<Intelbras.Message.Helper.MSG0056, Domain.Model.AcessoExtranet>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
        public MSG0056(string org, bool isOffline)
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
            return CriarMensagemRetorno<Pollux.MSG0056R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public AcessoExtranet DefinirPropriedades(Intelbras.Message.Helper.MSG0056 xml)
        {
            var crm = new AcessoExtranet(this.Organizacao, this.IsOffline);
            return crm;
        }

        public Pollux.MSG0056 DefinirPropriedades(AcessoExtranet objModel)
        {
            #region Propriedades Crm->Xml

            Pollux.MSG0056 msg0056 = new Pollux.MSG0056(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.Nome, 40));

            msg0056.CodigoPerfil = objModel.ID.ToString();
            msg0056.Nome = objModel.Nome;

            if (objModel.TipoAcesso != null)
                msg0056.TipoAcesso = objModel.TipoAcesso.Id.ToString();

            if (objModel.Status.HasValue)
                msg0056.Situacao = objModel.Status.Value;


            if(objModel.Categoria != null)
            {
               msg0056.Categoria = objModel.Categoria.Id.ToString();
            }

            if (objModel.Classificacao != null)
            {
                msg0056.Classificacao = objModel.Classificacao.Id.ToString();
            }

            #endregion

            return msg0056;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(AcessoExtranet objModel)
        {
            string retMsg = string.Empty;

            Intelbras.Message.Helper.MSG0056 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0056R1 retorno = CarregarMensagem<Pollux.MSG0056R1>(retMsg);
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

        #endregion
    }
}
