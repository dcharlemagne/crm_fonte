using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0289 : Base, IBase<Message.Helper.MSG0289, Domain.Model.Contato>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0289(string org, bool isOffline) : base(org, isOffline) { }

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
            var xml = this.CarregarMensagem<Pollux.MSG0289>(mensagem);

            if (string.IsNullOrEmpty(xml.CodigoContato))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoContato é obrigatório.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0289R1>(numeroMensagem, retorno);
            }

            Contato contato = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoContato));

            if (contato != null)
            {
                if (contato.Imagem != null)
                {
                    resultadoPersistencia.Sucesso = true;
                    retorno.Add("Foto", Convert.ToBase64String(contato.Imagem));
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0289R1>(numeroMensagem, retorno);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Contato não possui foto para ser enviada.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0289R1>(numeroMensagem, retorno);
                }
            } else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Contato não encontrado no CRM.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0289R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public Contato DefinirPropriedades(Intelbras.Message.Helper.MSG0289 xml)
        {
            var crm = new Model.Contato(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Contato objModel)
        {
            return String.Empty;
        }

        #endregion
    }
}


