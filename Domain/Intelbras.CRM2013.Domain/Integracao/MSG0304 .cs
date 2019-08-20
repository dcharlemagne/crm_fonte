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
    public class MSG0304 : Base, IBase<MSG0304, QuestionarioResposta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0304(string org, bool isOffline) : base(org, isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0304>(mensagem);
            List<Email> lista = new RepositoryService().Email.ListarPor(xml.Entidade,xml.Campo, xml.Valor);
            
            if (lista != null  && lista.Count > 0)
            {
                List<Intelbras.Message.Helper.Entities.Atividade> lstAtividades = new List<Pollux.Entities.Atividade>();

                foreach (Email email in lista)
                {
                    Intelbras.Message.Helper.Entities.Atividade atividade = new Pollux.Entities.Atividade();
                    atividade.GuidAtividade = email.ID.ToString();
                    atividade.Assunto = email.Assunto;
                    atividade.TipoAtividade = (int)Domain.Enum.TipoDeAtividade.Email;
                    atividade.StatusAtividade = email.StateCode.HasValue ? email.StateCode.Value : (int?) null;
                    atividade.DataUltimaAtualizacao = email.ModificadoEm.HasValue ? email.ModificadoEm.Value : (DateTime ?) null;
                    atividade.DataConclusao = email.TerminoReal.HasValue ? email.TerminoReal.Value : (DateTime?)null; ;
                    atividade.DataEnvio = email.EnviadoEm.HasValue ? email.EnviadoEm.Value : (DateTime?)null; ;
                    atividade.Descricao = email.Mensagem;
                    if (email.De != null && email.De.Count() > 0)
                    {
                        atividade.Solicitante = email.De[0].Name;
                    }
                    else
                    {
                        atividade.Solicitante = "";
                    }

                    lstAtividades.Add(atividade);
                }

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("Atividades", lstAtividades);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0304R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0304R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades
        public Email DefinirPropriedades(Intelbras.Message.Helper.MSG0304 xml)
        {
            var crm = new Model.Email(this.Organizacao, this.IsOffline);            

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(QuestionarioResposta objModel)
        {
            return String.Empty;
        }

        public QuestionarioResposta DefinirPropriedades(MSG0304 legado)
        {

            throw new NotImplementedException();
        }
        #endregion
    }
}
