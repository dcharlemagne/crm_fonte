using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0048 : Base, IBase<Message.Helper.MSG0048, Domain.Model.Mensagem>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0048(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0048>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0048R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.MensagemService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir Mensagem!";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0048R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);


            return CriarMensagemRetorno<Pollux.MSG0048R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Mensagem DefinirPropriedades(Intelbras.Message.Helper.MSG0048 xml)
        {
            var crm = new Mensagem(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.Codigo = xml.CodigoMensagemPedido;
            if (!String.IsNullOrEmpty(xml.Texto))
            {
                crm.Texto = xml.Texto;
            }
            else
            {
                crm.AddNullProperty("Texto");
            }

            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }

            crm.State = xml.Situacao;
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Mensagem objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
