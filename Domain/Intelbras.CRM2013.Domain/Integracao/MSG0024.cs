using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0024 : Base, IBase<Intelbras.Message.Helper.MSG0024, Domain.Model.Portador>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
        public MSG0024(string org, bool isOffline)
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

            Pollux.MSG0024 xml = this.CarregarMensagem<Pollux.MSG0024>(mensagem);

            var objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0024R1>(numeroMensagem, retorno);
            }

            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                if (mudancaProprietario == true)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "(Integração ocorrida com sucesso, não houve alteração do proprietário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "(Integração ocorrida com sucesso";
                }
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0024R1>(numeroMensagem, retorno);
        }
        #endregion

       
        #region Definir Propriedades
        public Portador DefinirPropriedades(Intelbras.Message.Helper.MSG0024 xml)
        {
            var crm = new Portador(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            crm.CodigoPortador = xml.CodigoPortador;

            crm.Nome = xml.Nome;

            crm.Status = xml.Situacao;

            crm.IntegradoEm = DateTime.Now;

            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;

            crm.UsuarioIntegracao = xml.LoginUsuario;
            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares
       
        public string Enviar(Portador objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
