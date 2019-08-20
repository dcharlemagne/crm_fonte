using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0187 : Base, IBase<Message.Helper.MSG0187, Domain.Model.ArquivoDeEstoqueGiro>
    {
        #region Construtor

        public MSG0187(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


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
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";

            List<Pollux.Entities.ArquivoEstoqueGiroItem> lstObjeto = this.DefinirRetorno(this.CarregarMensagem<Pollux.MSG0187>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0187R1>(numeroMensagem, retorno);
            }

            Pollux.MSG0187R1 resposta = new Pollux.MSG0187R1(mensagem, numeroMensagem);
            resposta.ArquivoEstoqueGiroItens = new List<Pollux.Entities.ArquivoEstoqueGiroItem>();

            if (lstObjeto.Count > 0)
            {
                resposta.ArquivoEstoqueGiroItens = lstObjeto;
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("ArquivoEstoqueGiroItens", resposta.ArquivoEstoqueGiroItens);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros no Crm.";
                retorno.Add("Resultado", resultadoPersistencia);

            }
            return CriarMensagemRetorno<Pollux.MSG0187R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ArquivoDeEstoqueGiro DefinirPropriedades(Intelbras.Message.Helper.MSG0187 xml)
        {
            ArquivoDeEstoqueGiro retorno = new ArquivoDeEstoqueGiro(this.Organizacao, this.IsOffline);
            return retorno;
        }

        public List<Pollux.Entities.ArquivoEstoqueGiroItem> DefinirRetorno(Intelbras.Message.Helper.MSG0187 xml)
        {
            #region Propriedades Crm->Xml
            List<Pollux.Entities.ArquivoEstoqueGiroItem> lstRetorno = new List<Pollux.Entities.ArquivoEstoqueGiroItem>();

            if (!string.IsNullOrEmpty(xml.CodigoConta))
            {
                List<ArquivoDeEstoqueGiro> lstArquivoDeEstoqueGiro = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeEstoqueGiroServices(this.Organizacao, this.IsOffline).ListarPor(new Guid(xml.CodigoConta), xml.StatusProcessamento, xml.DataEnvioInicio, xml.DataEnvioFim);

                if (lstArquivoDeEstoqueGiro.Count > 0)
                {
                    foreach (ArquivoDeEstoqueGiro registro in lstArquivoDeEstoqueGiro)
                    {
                        Pollux.Entities.ArquivoEstoqueGiroItem tmpArquivoDeEstoqueGiro = new Pollux.Entities.ArquivoEstoqueGiroItem();

                        tmpArquivoDeEstoqueGiro.CodigoArquivoEstoqueGiro = registro.ID.Value.ToString();

                        tmpArquivoDeEstoqueGiro.CodigoConta = registro.Conta.Id.ToString();

                        tmpArquivoDeEstoqueGiro.DataEnvio = registro.DataDeEnvio.Value.ToLocalTime();

                        if (registro.DataDeProcessamento.HasValue)
                            tmpArquivoDeEstoqueGiro.DataProcessamento = registro.DataDeProcessamento.Value.ToLocalTime();

                        tmpArquivoDeEstoqueGiro.LoginUsuario = xml.LoginUsuario;

                        if (String.IsNullOrEmpty(registro.Nome))
                            tmpArquivoDeEstoqueGiro.Nome = (String)this.PreencherAtributoVazio("string");
                        else
                            tmpArquivoDeEstoqueGiro.Nome = registro.Nome;

                        tmpArquivoDeEstoqueGiro.Proprietario = usuarioIntegracao.ID.Value.ToString();

                        if (registro.Status.HasValue)
                            tmpArquivoDeEstoqueGiro.StatusProcessamento = registro.RazaoStatus.Value;
                        else
                            tmpArquivoDeEstoqueGiro.StatusProcessamento = 1;

                        tmpArquivoDeEstoqueGiro.TipoProprietario = "systemuser";
                        tmpArquivoDeEstoqueGiro.QuantidadeLinhasErro = (registro.QuantidadeLinhasErro.HasValue) ? registro.QuantidadeLinhasErro.Value : 0;
                        tmpArquivoDeEstoqueGiro.QuantidadeLinhasProcessadas = (registro.QuantidadeLinhasProcessadas.HasValue) ? registro.QuantidadeLinhasProcessadas.Value : 0;
                        tmpArquivoDeEstoqueGiro.QuantidadeTotalLinhas = (registro.QuantidadeTotalLinhas.HasValue) ? registro.QuantidadeTotalLinhas.Value : 0;
                        tmpArquivoDeEstoqueGiro.OrigemArquivo = registro.Origem;
                                                
                        lstRetorno.Add(tmpArquivoDeEstoqueGiro);
                    }

                    return lstRetorno;
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Nenhum registro encontrado.";
                    return lstRetorno;
                }

            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Valor do parâmetro 'Código Conta' é obrigatório";
                return lstRetorno;
            }
            #endregion
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ArquivoDeEstoqueGiro objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

        ArquivoDeEstoqueGiro IBase<Pollux.MSG0187, ArquivoDeEstoqueGiro>.DefinirPropriedades(Pollux.MSG0187 legado)
        {
            throw new NotImplementedException();
        }
    }
}
