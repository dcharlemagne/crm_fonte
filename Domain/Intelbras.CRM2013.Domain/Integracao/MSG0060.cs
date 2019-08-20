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
    public class MSG0060 : Base, IBase<Message.Helper.MSG0060, Domain.Model.Moeda>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0060(string org, bool isOffline)
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

            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ações de criação não permitidas";
            retorno.Add("Resultado", resultadoPersistencia);


            return CriarMensagemRetorno<Pollux.MSG0060R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Moeda DefinirPropriedades(Intelbras.Message.Helper.MSG0060 xml)
        {
            return new Moeda(this.Organizacao, this.IsOffline);
        }
        private Intelbras.Message.Helper.MSG0060 DefinirPropriedades(Moeda crm)
        {
            Intelbras.Message.Helper.MSG0060 xml = new Pollux.MSG0060(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.NomeMoeda, 40));

            xml.Nome = crm.NomeMoeda;

            xml.NumeroDecimais = crm.Precisão;

            xml.Simbolo = crm.Simbolo;

            xml.TaxaCambio = crm.TaxaCambio;

            xml.CodigoISO = crm.CodigoMoeda;

            return xml;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Moeda objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0060 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0060R1 retorno = CarregarMensagem<Pollux.MSG0060R1>(resposta);
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
