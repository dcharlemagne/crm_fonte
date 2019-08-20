using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0050 : Base, IBase<Intelbras.Message.Helper.MSG0050, Domain.Model.NaturezaOperacao>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0050(string org, bool isOffline)
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

            NaturezaOperacao objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0050>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0030R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).Persistir(objeto);

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
            return CriarMensagemRetorno<Pollux.MSG0050R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public NaturezaOperacao DefinirPropriedades(Intelbras.Message.Helper.MSG0050 xml)
        {
            var crm = new NaturezaOperacao(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            if(!string.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;

            if (!string.IsNullOrEmpty(xml.CodigoNaturezaOperacao))
                crm.Codigo = xml.CodigoNaturezaOperacao;

            if (xml.EmiteDuplicata.HasValue)
                crm.EmiteDuplicata = xml.EmiteDuplicata;

            if(xml.AtualizaEstatistica.HasValue)
                crm.AtualizarEstatisticas = xml.AtualizaEstatistica;

            crm.Tipo = xml.Tipo;

            crm.State = xml.Situacao;

            crm.IntegradoEm = DateTime.Now;

            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;

            crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(NaturezaOperacao objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
