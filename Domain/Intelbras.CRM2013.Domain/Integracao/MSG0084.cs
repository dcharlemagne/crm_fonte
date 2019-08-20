using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0084 : Base, IBase<Message.Helper.MSG0084, Domain.Model.Unidade>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0084(string org, bool isOffline)
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
            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Integração não permitida!";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0084R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Unidade DefinirPropriedades(Intelbras.Message.Helper.MSG0084 xml)
        {
            var crm = new Unidade(this.Organizacao, this.IsOffline);
            #region Propriedades Crm->Xml

            if (!string.IsNullOrEmpty(xml.DescricaoUnidadeMedida))
                crm.Nome = xml.DescricaoUnidadeMedida;
            else
                crm.AddNullProperty("Nome");

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0084 DefinirPropriedades(Unidade crm)
        {
            Intelbras.Message.Helper.MSG0084 xml = new Pollux.MSG0084(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.SiglaUnidadeMedida = crm.Nome;
            xml.GrupoUnidadeMedida = crm.GrupoUnidade.Id.ToString();
            
            return xml;

        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Unidade objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0084 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0084R1 retorno = CarregarMensagem<Pollux.MSG0084R1>(resposta);
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

