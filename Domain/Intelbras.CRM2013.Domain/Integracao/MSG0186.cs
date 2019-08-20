using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0186 : Base, IBase<Message.Helper.MSG0186, Domain.Model.ArquivoDeEstoqueGiro>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        #region Construtor
        public MSG0186(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0186>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", this.resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0186R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeEstoqueGiroServices(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persisrtir Arquivo de EstoqueGiro.";
                return CriarMensagemRetorno<Pollux.MSG0186R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";


            //retorno.Add("Sucesso", true);

            //retorno.Add("Mensagem", "Integração ocorrida com sucesso.");

            if (objeto.ID.HasValue)
                retorno.Add("CodigoArquivoEstoqueGiro", objeto.ID.Value.ToString());

            retorno.Add("TipoProprietario", "systemuser");

            if (usuarioIntegracao != null)
                retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());

            retorno.Add("Resultado", resultadoPersistencia);


            return CriarMensagemRetorno<Pollux.MSG0186R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ArquivoDeEstoqueGiro DefinirPropriedades(Intelbras.Message.Helper.MSG0186 xml)
        {
            var crm = new ArquivoDeEstoqueGiro(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoArquivoEstoqueGiro))
                crm.ID = new Guid(xml.CodigoArquivoEstoqueGiro);

            if (xml.DataEnvio.HasValue)
                crm.DataDeEnvio = xml.DataEnvio.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Data de Envio não Informada.";
                return crm;
            }

            if (xml.StatusProcessamento.HasValue)
            {
                if (xml.StatusProcessamento.Value != 2)
                    crm.RazaoStatus = xml.StatusProcessamento.Value;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Status de Processamento não Informada.";
                return crm;
            }

            if (xml.Situacao.HasValue)
            {
                crm.Status = xml.Situacao.Value;
            }
         

            crm.Nome = xml.Nome;

            if (xml.DataProcessamento.HasValue)
                crm.DataDeProcessamento = xml.DataProcessamento.Value;



            if (!String.IsNullOrEmpty(xml.CodigoConta))
                crm.Conta = new Lookup(new Guid(xml.CodigoConta.ToString()), "account");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Canal relacionado ao arquivo de Estoque Giro não Informada.";
                return crm;
            }

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            crm.Origem = xml.OrigemArquivo;

            #endregion

            return crm;
        }

        public Pollux.MSG0186 DefinirPropriedades(ArquivoDeEstoqueGiro objModel)
        {
            #region Propriedades Crm->Xml

            Pollux.MSG0186 msg0186 = new Pollux.MSG0186(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.Nome, 40));
            //msg0162.CodigoArquivoSellout = objModel.

            #endregion

            return msg0186;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ArquivoDeEstoqueGiro objModel, ref string nomeAbrevRet, ref string codigoClienteRet)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0186 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0186R1 retorno = CarregarMensagem<Pollux.MSG0186R1>(retMsg);
                if (retorno.Resultado.Sucesso)
                {
                    //if (!String.IsNullOrEmpty(retorno.NomeAbreviado))
                    //    nomeAbrevRet = retorno.NomeAbreviado;
                    //if (retorno.CodigoCliente.HasValue)
                    //    codigoClienteRet = retorno.CodigoCliente.Value.ToString();
                }
                else
                {
                    throw new Exception(string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new Exception(string.Concat(erro001.GenerateMessage(false)));
            }
            return retMsg;
        }

        public string Enviar(ArquivoDeEstoqueGiro ArquivoDeEstoqueGiroObj)
        {
            return String.Empty;
        }

        #endregion
    }
}
