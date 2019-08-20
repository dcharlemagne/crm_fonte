using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0190 : Base, IBase<Pollux.MSG0190, CategoriaB2B>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado();
        #endregion


        #region Construtor
        public MSG0190(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0190>(mensagem));

            objeto = new CategoriaB2BService(this.Organizacao, this.IsOffline).Persistir(objeto);
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
            return CriarMensagemRetorno<Pollux.MSG0190R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public CategoriaB2B DefinirPropriedades(Intelbras.Message.Helper.MSG0190 xml)
        {
            var categoria = new CategoriaB2B(this.Organizacao, this.IsOffline);
            categoria.CodigoCategoriaB2B = xml.CodigoCategoriaB2B;

            if (!String.IsNullOrEmpty(xml.NomeCategoria))
                categoria.Nome = xml.NomeCategoria;

            if (xml.Situacao.HasValue)
                categoria.Status = xml.Situacao;

            return categoria;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(CategoriaB2B objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

