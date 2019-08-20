using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0002 : Base, IBase<Message.Helper.MSG0002, Domain.Model.UnidadeNegocio>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor
        public MSG0002(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0002>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0002R1>(numeroMensagem, retorno);
            }

            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).Persistir(objeto);
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
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso, não houve alteração do proprietário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }
            }

            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0002R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades
        public UnidadeNegocio DefinirPropriedades(Intelbras.Message.Helper.MSG0002 xml)
        {
            var crm = new UnidadeNegocio(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;

            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                crm.ChaveIntegracao = xml.CodigoUnidadeNegocio;
            }


            UnidadeNegocio unidadePai = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorNome("Intelbras");

            if (unidadePai != null)
            {
                crm.NegocioPrimario = new Lookup((Guid)unidadePai.ID, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Unidade pai não encontrada.";
            }

            #endregion
            return crm;
        }

        private Intelbras.Message.Helper.MSG0002 DefinirPropriedades(UnidadeNegocio crm)
        {
            Intelbras.Message.Helper.MSG0002 xml = new Pollux.MSG0002(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.Nome = crm.Nome;
            xml.CodigoUnidadeNegocio = crm.ChaveIntegracao;

            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(UnidadeNegocio objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0002 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0002R1 retorno = CarregarMensagem<Pollux.MSG0002R1>(resposta);
                return retorno.Resultado.Mensagem;
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 retorno = CarregarMensagem<Pollux.ERR0001>(resposta);
                return retorno.DescricaoErro;
            }
        }
        #endregion

    }
}