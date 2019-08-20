using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0290 : Base, IBase<Pollux.MSG0290, Conta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0290(string org, bool isOffline) : base(org, isOffline) { }

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
            var xml = this.CarregarMensagem<Pollux.MSG0290>(mensagem);

            if (string.IsNullOrEmpty(xml.IdentificacaoUnidadeNegocio))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "IdentificacaoUnidadeNegocio é obrigatório.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0290R1>(numeroMensagem, retorno);
            }

            if (string.IsNullOrEmpty(xml.CEP))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CEP é obrigatório.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0290R1>(numeroMensagem, retorno);
            }
            //declara a procedure e os parametros que serão usados na Procedure
            string nomeProcedure = "dbo.p_TelefoneAssistencias";
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@P_CEP", xml.CEP));
            list.Add(new SqlParameter("@P_UNIDADE_NEGOCIO", xml.IdentificacaoUnidadeNegocio));

            string pSaida = "@P_RESULTADO";

            string telefones = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterTelefoneAssistencias(nomeProcedure, list, pSaida);

            if (!string.IsNullOrEmpty(telefones))
            {
                //apaga o último caracter que será um ;
                telefones = telefones.Remove(telefones.Length - 1);
                resultadoPersistencia.Sucesso = true;
                retorno.Add("TelefonesAssistencias", telefones);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0290R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados telefones para os parametros informados.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0290R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public Conta DefinirPropriedades(Intelbras.Message.Helper.MSG0290 xml)
        {
            var crm = new Model.Conta(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Conta objModel)
        {
            return String.Empty;
        }

        #endregion
    }
}