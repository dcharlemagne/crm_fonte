using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0038 : Base,IBase<Message.Helper.MSG0038,Domain.Model.GrupoEstoque>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor
        public MSG0038(string org, bool isOffline)
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

            GrupoEstoque objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0038>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0038R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.GrupoEstoqueService(this.Organizacao, this.IsOffline).Persistir(objeto);

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
            return CriarMensagemRetorno<Pollux.MSG0038R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public GrupoEstoque DefinirPropriedades(Intelbras.Message.Helper.MSG0038 xml)
        {
            var crm = new GrupoEstoque(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.Codigo = (int)xml.CodigoGrupoEstoque;

            if(!String.IsNullOrEmpty(xml.Nome))
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
    
        public string Enviar(GrupoEstoque objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
