using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0161 : Base, IBase<Message.Helper.MSG0161, Domain.Model.Usuario>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0161(string org, bool isOffline)
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
            return String.Empty;
        }
        #endregion

        #region Definir Propriedades

        public Usuario DefinirPropriedades(Intelbras.Message.Helper.MSG0161 xml)
        {
            var crm = new Model.Usuario(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Usuario objModel)
        {
            return String.Empty;
        }

        public bool ValidarRepresentante(Int32 codigoRepresentante, String tipoObjetoCliente, ref String resposta)
        {
            Pollux.MSG0161 msg0161 = new Pollux.MSG0161(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), tipoObjetoCliente);
            msg0161.CodigoRepresentante = codigoRepresentante;

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(msg0161.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0161R1 retorno = CarregarMensagem<Pollux.MSG0161R1>(resposta);
                if (retorno.RepresentanteValido.Equals(1))
                {
                    resposta = retorno.Resultado.Mensagem;
                    return true;
                }
                else
                {
                    resposta = retorno.Resultado.Mensagem;
                    return false;
                }

            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
        }

        #endregion
    }
}
