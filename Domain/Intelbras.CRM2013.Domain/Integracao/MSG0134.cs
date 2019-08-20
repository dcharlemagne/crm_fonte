using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0134 : Base, IBase<Message.Helper.MSG0134, Domain.Model.Regiao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0134(string org, bool isOffline)
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

            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0134R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Regiao DefinirPropriedades(Intelbras.Message.Helper.MSG0134 xml)
        {
            var crm = new Regiao(this.Organizacao, this.IsOffline);

            return crm;
        }
        private Intelbras.Message.Helper.MSG0134 DefinirPropriedades(Regiao crm)
        {
            Intelbras.Message.Helper.MSG0134 xml = new Pollux.MSG0134(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.ID.Value.ToString(), 40));

            xml.CodigoRegiao = crm.ID.ToString();
            xml.Nome = crm.Nome;
            xml.Descricao = crm.Descricao;
            xml.CodigoGerente = crm.Gerente.Id.ToString();

            return xml;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Regiao objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0134 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0134R1 retorno = CarregarMensagem<Pollux.MSG0134R1>(resposta);
                return retorno.Resultado.Mensagem;
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 retorno = CarregarMensagem<Pollux.ERR0001>(resposta);
                return retorno.DescricaoErro;
            }

        }
 
        #endregion
    }
}
