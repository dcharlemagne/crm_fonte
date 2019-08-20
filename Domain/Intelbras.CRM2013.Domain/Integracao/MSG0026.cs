using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0026 : Base, IBase<Message.Helper.MSG0026, Domain.Model.Segmento>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0026(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0026>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0026R1>(numeroMensagem, retorno);
            }
            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).Persistir(objeto, ref mudancaProprietario);

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
            return CriarMensagemRetorno<Pollux.MSG0026R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Segmento DefinirPropriedades(Intelbras.Message.Helper.MSG0026 xml)
        {
            var crm = new Segmento(this.Organizacao, this.IsOffline);

            Segmento TmpSegmento = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(this.Organizacao, this.IsOffline).Segmento.ObterPor(xml.CodigoSegmento);

            if (TmpSegmento != null)
            {
                crm.DescontoVerdeHabilitado = TmpSegmento.DescontoVerdeHabilitado;
            }

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.Nome))
            {
                crm.Nome = xml.Nome;
            }

            if (!String.IsNullOrEmpty(xml.CodigoSegmento))
            {
                crm.CodigoSegmento = xml.CodigoSegmento;
            }

            if (xml.QuantidadeShowRoom.HasValue)
            {
                crm.QtdMaximaShowRoom = xml.QuantidadeShowRoom.Value;
            }
            else
            {
                crm.QtdMaximaShowRoom = 0;
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

            if (!String.IsNullOrEmpty(xml.GerenteResponsavel))
                crm.GerenteResponsavel = new Lookup(new Guid(xml.GerenteResponsavel), "");
            else
                crm.AddNullProperty("GerenteResponsavel");

            crm.Status = xml.Situacao;

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;


            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0026 DefinirPropriedades(Segmento crm)
        {
            Intelbras.Message.Helper.MSG0026 xml = new Pollux.MSG0026(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.Nome = crm.Nome;
            xml.CodigoSegmento = crm.CodigoSegmento;
            xml.GerenteResponsavel = crm.GerenteResponsavel.Name;
            xml.QuantidadeShowRoom = crm.QtdMaximaShowRoom.Value;

            return xml;
        }


        #endregion

        #region Métodos Auxiliares

        public string Enviar(Segmento objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0026 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0026R1 retorno = CarregarMensagem<Pollux.MSG0026R1>(resposta);
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
