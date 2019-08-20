using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0067 : Base, IBase<Intelbras.Message.Helper.MSG0066, Domain.Model.Bens>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado();

        #endregion

        #region Construtor
        public MSG0067(string org, bool isOffline)
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
            resultadoPersistencia.Mensagem = "Ações de deleção não permitidas.";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0067R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Bens DefinirPropriedades(Intelbras.Message.Helper.MSG0066 xml)
        {
            var crm = new Bens(this.Organizacao, this.IsOffline);
            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Bens objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
