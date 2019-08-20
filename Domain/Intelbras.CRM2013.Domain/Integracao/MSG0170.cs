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
    public class MSG0170 : Base, IBase<Message.Helper.MSG0170, Domain.Model.PrioridadeLigacaoCallCenter>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor

        public MSG0170(string org, bool isOffline)
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

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario = null)
        {
            retorno.Add("Resultado", resultadoPersistencia);

            try
            {
                var objeto = this.CarregarMensagem<Intelbras.Message.Helper.MSG0170>(mensagem);

                if (string.IsNullOrEmpty(objeto.CpfCnpj))
                {
                    throw new ArgumentException("CpfCnpj é obrigatório");
                }

                if (string.IsNullOrEmpty(objeto.NomeFila))
                {
                    throw new ArgumentException("NomeFila é obrigatório");
                }

                var servicePrioridade = new PrioridadeLigacaoCallCenterService(base.Organizacao, base.IsOffline);
                int? prioridade = servicePrioridade.ObterPrioridade(objeto.CpfCnpj, objeto.NomeFila);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";

                retorno.Add("RetornoPrioridade", Convert(prioridade));
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0170R1>(numeroMensagem, retorno);
            }
            return CriarMensagemRetorno<Pollux.MSG0170R1>(numeroMensagem, retorno);
        }

        private Intelbras.Message.Helper.Entities.RetornoPrioridade Convert(int? prioridade)
        {
            var result = new Intelbras.Message.Helper.Entities.RetornoPrioridade();

            result.UrlOcorrencia = "";
            result.NumeroOcorrencia = "";
            result.Prioridade = prioridade.HasValue ? prioridade : 0;

            return result;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(PrioridadeLigacaoCallCenter objModel)
        {
            return String.Empty;
        }

        public PrioridadeLigacaoCallCenter DefinirPropriedades(Intelbras.Message.Helper.MSG0170 xml)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
