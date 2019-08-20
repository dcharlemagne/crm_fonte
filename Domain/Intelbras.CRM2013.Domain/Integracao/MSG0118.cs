using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0118 : Base, IBase<Intelbras.Message.Helper.MSG0118, Domain.Model.TipoSolicitacao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0118(string org, bool isOffline)
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

            List<TipoSolicitacao> lstTipos = new Intelbras.CRM2013.Domain.Servicos.TipoSolicitacaoService(this.Organizacao, this.IsOffline).Listar();

            List<Pollux.Entities.TipoSolicitacaoItem> lstRetorno = new List<Pollux.Entities.TipoSolicitacaoItem>();

            if (lstTipos.Count > 0)
            {
                lstRetorno = ConverteLista(lstTipos);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";

                retorno.Add("TiposSolicitacaoItens", lstRetorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Tipos de Solicitação não encontrados.";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0118R1>(numeroMensagem, retorno);
        }

        private List<Pollux.Entities.TipoSolicitacaoItem> ConverteLista(List<TipoSolicitacao> lstTipos)
        {
            List<Pollux.Entities.TipoSolicitacaoItem> lstPollux = new List<Pollux.Entities.TipoSolicitacaoItem>();

            foreach (TipoSolicitacao fnCon in lstTipos)
            {
                Pollux.Entities.TipoSolicitacaoItem polluxObj = new Pollux.Entities.TipoSolicitacaoItem();
                polluxObj.CodigoTipoSolicitacao = fnCon.ID.ToString();
                if (fnCon.Nome.Length > 100)
                    polluxObj.Nome = fnCon.Nome.Substring(0, 99);
                else
                    polluxObj.Nome = fnCon.Nome;

                lstPollux.Add(polluxObj);
            }

            return lstPollux;
        }
        #endregion

        #region Definir Propriedades

        public TipoSolicitacao DefinirPropriedades(Intelbras.Message.Helper.MSG0118 xml)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(TipoSolicitacao objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
