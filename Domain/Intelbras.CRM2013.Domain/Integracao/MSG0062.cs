using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0062 : Base, IBase<Message.Helper.MSG0062, Domain.Model.EstruturaAtendimento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0062(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0062>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0062R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.EstruturaAtendimentoService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir objeto.";
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoEstruturaMinima", objeto.ID.Value.ToString());
            }
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0062R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public EstruturaAtendimento DefinirPropriedades(Intelbras.Message.Helper.MSG0062 xml)
        {
            var crm = new EstruturaAtendimento(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoEstruturaMinima))
            {
                crm.ID = new Guid(xml.CodigoEstruturaMinima);
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

            crm.TipoEstruturaAtendimento = xml.Tipo;

            crm.PossueEstrutura = xml.PossuiEstruturaMinima;

            if (!String.IsNullOrEmpty(xml.Conta))
            {
                crm.Canal = new Lookup(new Guid(xml.Conta), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Canal não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.UnidadeNegocio))
            {
                Model.UnidadeNegocio unidadeNegocio = new UnidadeNegocio(this.Organizacao, this.IsOffline);
                unidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.UnidadeNegocio);
                if (unidadeNegocio != null)
                    crm.UnidadeNegocios = new Lookup(unidadeNegocio.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Unidade de Negocio não encontrada!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Unidade de Negocio não enviado.";
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

        public string Enviar(EstruturaAtendimento objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
