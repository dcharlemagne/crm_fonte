using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0054 : Base,IBase<Message.Helper.MSG0054,Domain.Model.Rota>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0054(string org, bool isOffline)
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
            Rota objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0054>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0054R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.RotaService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }


            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0054R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Rota DefinirPropriedades(Intelbras.Message.Helper.MSG0054 xml)
        {
            var crm = new Rota(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

                crm.CodigoRota = xml.CodigoRota;

                crm.Nome = xml.Nome;

                if (!string.IsNullOrEmpty(xml.Roteiro))
                    crm.Roteiro = xml.Roteiro;
                else
                    crm.AddNullProperty("Roteiro");

                crm.State = xml.Situacao;

                crm.IntegradoEm = DateTime.Now;

                crm.IntegradoPor = usuarioIntegracao.NomeCompleto;

                crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Rota objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
