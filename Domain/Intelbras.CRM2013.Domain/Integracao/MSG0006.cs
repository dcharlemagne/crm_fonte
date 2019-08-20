using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0006 : Base, IBase<Intelbras.Message.Helper.MSG0006, Domain.Model.Pais>
    {
        #region Propriedades

        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor
        public MSG0006(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0006>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", this.resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0060R1>(numeroMensagem, retorno);
            }
            objeto= new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "(Integração ocorrida com sucesso";
            }
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0006R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Pais DefinirPropriedades(Intelbras.Message.Helper.MSG0006 xml)
        {
            var crm = new Pais(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Nome não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.ChaveIntegracao))
            crm.ChaveIntegracao = xml.ChaveIntegracao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(ChaveIntegracao não enviado.";
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

        public string Enviar(Pais objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
