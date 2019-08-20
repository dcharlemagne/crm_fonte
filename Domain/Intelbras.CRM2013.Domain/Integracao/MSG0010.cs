using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0010 : Base, IBase<Intelbras.Message.Helper.MSG0010, Domain.Model.Estado>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor
        public MSG0010(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0010>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0010R1>(numeroMensagem, retorno);
            }
            objeto = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).Persistir(objeto);

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

            return CriarMensagemRetorno<Pollux.MSG0010R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Estado DefinirPropriedades(Intelbras.Message.Helper.MSG0010 xml)
        {
            var crm = new Estado(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
                crm.UF = xml.Nome;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Nome não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Sigla))
                crm.SiglaUF = xml.Sigla;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Sigla UF não enviado.";
                return crm;
            }

            //Não obrigatório
            if (!String.IsNullOrEmpty(xml.RegiaoGeografica))
            {
                Model.Itbc_regiaogeo regiao = new Model.Itbc_regiaogeo(this.Organizacao, this.IsOffline);

                var regiaoGeoId = new Guid(xml.RegiaoGeografica);

                regiao = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaRegiaoGeo(regiaoGeoId);

                if (regiao != null && regiao.ID.HasValue)
                    crm.RegiaoGeografica = new Lookup(regiao.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "(Região não encontrada!";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("RegiaoGeografica");
            }

            if (!String.IsNullOrEmpty(xml.ChaveIntegracao))
                crm.ChaveIntegracao = xml.ChaveIntegracao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(ChaveIntegracao não enviada.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Pais))
            {
                Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);

                pais = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(xml.Pais);

                if (pais != null && pais.ID.HasValue)
                    crm.Pais = new Lookup(pais.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "(Pais não encontrado!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(País não enviada.";
                return crm;
            }
            crm.Status = xml.Situacao;
           
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
           

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Estado objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
