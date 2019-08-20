using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0282 : Base, IBase<Pollux.MSG0282, Model.TipoPagamento>
    {
        #region Propriedades
            //Dictionary que sera enviado como resposta do request
            private Dictionary<string, object> retorno = new Dictionary<string, object>();
            Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
            public MSG0282(string org, bool isOffline): base(org, isOffline)
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
                return CriarMensagemRetorno<Pollux.MSG0282R1>(numeroMensagem, retorno);
            }
        #endregion

        #region Definir Propriedades
        public TipoPagamento DefinirPropriedades(Intelbras.Message.Helper.MSG0282 xml)
        {
            throw new NotImplementedException();
        }

        private Intelbras.Message.Helper.MSG0282 DefinirPropriedades(TipoPagamento crm)
        {
            Intelbras.Message.Helper.MSG0282 xml = new Pollux.MSG0282(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.CodigoTipoPagamentoServico = crm.ID.ToString();
            xml.Nome = crm.Nome;
            
            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(TipoPagamento objModel)
        {
            string resposta;

            Intelbras.Message.Helper.MSG0282 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0282R1 retorno = CarregarMensagem<Pollux.MSG0282R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return resposta;
        }
        #endregion        

    }
}
