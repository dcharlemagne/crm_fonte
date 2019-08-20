using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using System.Data;
using System.Linq;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0299 : Base, IBase<MSG0299, Acao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0299(string org, bool isOffline) : base(org, isOffline)
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
        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            usuarioIntegracao = usuario;
            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0299R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades
        public Acao DefinirPropriedades(MSG0299 legado)
        {
            var crm = new Model.Acao(this.Organizacao, this.IsOffline);
            return crm;
        }

        private Intelbras.Message.Helper.MSG0299 DefinirPropriedades(Acao crm)
        {
            Intelbras.Message.Helper.MSG0299 xml = new Pollux.MSG0299(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.DescricaoAcao, 40));

            xml.GUIDAcaoFinal = crm.Id.ToString();
            xml.CodigoAcao = crm.Codigo;
            xml.DescricaoAcao = crm.DescricaoAcao;
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(Acao objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0299 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0299R1 retorno = CarregarMensagem<Pollux.MSG0299R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException("(CRM) " + string.Concat("Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }
        #endregion
    }
}