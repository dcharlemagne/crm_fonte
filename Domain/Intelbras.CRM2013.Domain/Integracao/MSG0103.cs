using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0103 : Base, IBase<Message.Helper.MSG0103, Domain.Model.TreinamentoCertificacao>
    {

        #region Construtor

        public MSG0103(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0103>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0103R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.TreinamentoCertificacaoService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de Persistência!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0103R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public TreinamentoCertificacao DefinirPropriedades(Intelbras.Message.Helper.MSG0103 xml)
        {
            var crm = new TreinamentoCertificacao(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.NomeTreinamento))
            {
                crm.Nome = xml.NomeTreinamento;
            }
            else 
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "NomeTreinamento não enviado!";
                return crm;
            }
            
            if (xml.Situacao == 1 || xml.Situacao == 0)
                crm.State = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situação não enviada!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.ModalidadeTreinamento))
            {
                crm.ModalidadeCurso = xml.ModalidadeTreinamento;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "ModalidadeTreinamento não enviado!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CategoriaTreinamento))
            {
                crm.CategoriaCurso = xml.CategoriaTreinamento;
            }
            else
                crm.AddNullProperty("CategoriaCurso");

            if (!String.IsNullOrEmpty(xml.CodigoTreinamento))
            {
                crm.CodigoTreinamento = xml.CodigoTreinamento;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoTreinamento não enviado!";
                return crm;
            }

            if (xml.IdentificadorTreinamento.HasValue)
                crm.IdCurso = xml.IdentificadorTreinamento.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador Treinamento não enviado!";
                return crm;
            }

            if (xml.HorasTreinamento.HasValue)
                crm.HorasTreinamento = xml.HorasTreinamento.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Horas Treinamento não enviado!";
                return crm;
            }
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(TreinamentoCertificacao objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
