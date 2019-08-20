using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0068 : Base, IBase<Message.Helper.MSG0068, Domain.Model.FornecedorCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0068(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0068>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0068R1>(numeroMensagem, retorno);
            }
            
            objeto = new Intelbras.CRM2013.Domain.Servicos.FornecedorCanalService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoFornecedor", objeto.ID.Value.ToString());
            }
            retorno.Add("Resultado", resultadoPersistencia);
            

            return CriarMensagemRetorno<Pollux.MSG0068R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public FornecedorCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0068 xml)
        {
            var crm = new FornecedorCanal(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
            }
            if (!String.IsNullOrEmpty(xml.NomeContato))
            {
                crm.Contato = xml.NomeContato;
            }

            if (!String.IsNullOrEmpty(xml.CodigoFornecedor))
            {
                crm.ID = new Guid(xml.CodigoFornecedor);
            }

            if (!String.IsNullOrEmpty(xml.Telefone))
            {
                crm.Telefone = xml.Telefone;
            }

            if (!String.IsNullOrEmpty(xml.Conta))
            {
                crm.Canal = new Lookup(new Guid(xml.Conta), "");
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

        public string Enviar(FornecedorCanal objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
