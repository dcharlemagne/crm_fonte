using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0276 : Base, IBase<Message.Helper.MSG0276, Domain.Model.Contato>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0276(string org, bool isOffline) : base(org, isOffline) { }

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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0276>(mensagem));


            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0276R1>(numeroMensagem, retorno);
            }

            try
            {
                objeto = new ContatoService(this.Organizacao, this.IsOffline).Persistir(objeto);
                if (objeto == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Não foi possível salvar a alteração. Integração não realizada.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }

            }
            catch (ChaveIntegracaoContatoException ex)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = ex.Message;
            }

            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0276R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Contato DefinirPropriedades(Intelbras.Message.Helper.MSG0276 xml)
        {
            var crm = new Model.Contato(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.IntegrarNoPlugin = true;

            if (!String.IsNullOrEmpty(xml.CodigoContato))
            {
                try
                {
                    Contato contato = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(new Guid(xml.CodigoContato));
                    if (contato != null)
                    {
                        crm.ID = new Guid(xml.CodigoContato);
                        if (!string.IsNullOrEmpty(xml.Foto))
                            crm.Imagem = Convert.FromBase64String(xml.Foto);
                        crm.IntegrarNoPlugin = true;
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Codigo Contato informado não existe para ser atualizado.";
                        return crm;
                    }
                }
                catch (Exception e)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                    return null;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Codigo Contato é obrigatório.";
                return crm;
            }

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0276 DefinirPropriedades(Contato crm)
        {
            Intelbras.Message.Helper.MSG0276 xml = new Pollux.MSG0276(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.ID.ToString(), 40));

            xml.CodigoContato = crm.ID.ToString();
            xml.Foto = Convert.ToBase64String(crm.Imagem);

            return xml;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Contato objModel)
        {
            string resposta;

            Intelbras.Message.Helper.MSG0276 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0276R1 retorno = CarregarMensagem<Pollux.MSG0276R1>(resposta);
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