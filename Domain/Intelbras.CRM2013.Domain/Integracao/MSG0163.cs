using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0163 : Base, IBase<Message.Helper.MSG0163, Domain.Model.ArquivoDeSellOut>
    {
        #region Construtor

        public MSG0163(string org, bool isOffline)
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

            List<Pollux.Entities.ArquivoSelloutItem> lstObjeto = this.DefinirRetorno(this.CarregarMensagem<Pollux.MSG0163>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0163R1>(numeroMensagem, retorno);
            }

            Pollux.MSG0163R1 resposta = new Pollux.MSG0163R1(mensagem, numeroMensagem);
            resposta.ArquivoSelloutItens = new List<Pollux.Entities.ArquivoSelloutItem>();

            if (lstObjeto.Count > 0)
            {
                resposta.ArquivoSelloutItens = lstObjeto;
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("ArquivoSelloutItens", resposta.ArquivoSelloutItens);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros no Crm.";
                retorno.Add("Resultado", resultadoPersistencia);

            }
            return CriarMensagemRetorno<Pollux.MSG0163R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ArquivoDeSellOut DefinirPropriedades(Intelbras.Message.Helper.MSG0163 xml)
        {
            ArquivoDeSellOut retorno = new ArquivoDeSellOut(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.ArquivoSelloutItem> DefinirRetorno(Intelbras.Message.Helper.MSG0163 xml)
        {
            #region Propriedades Crm->Xml
            List<Pollux.Entities.ArquivoSelloutItem> lstRetorno = new List<Pollux.Entities.ArquivoSelloutItem>();

            if (!string.IsNullOrEmpty(xml.CodigoConta))
            {

                List<ArquivoDeSellOut> lstArquivoDeSellOut = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeSellOutServices(this.Organizacao, this.IsOffline).ListarPor(new Guid(xml.CodigoConta), xml.StatusProcessamento, xml.DataEnvioInicio, xml.DataEnvioFim);

                if (lstArquivoDeSellOut.Count > 0)
                {
                    foreach (ArquivoDeSellOut registro in lstArquivoDeSellOut)
                    {
                        Pollux.Entities.ArquivoSelloutItem tmpArquivoDeSellOut = new Pollux.Entities.ArquivoSelloutItem();

                        tmpArquivoDeSellOut.CodigoConta = registro.ID.Value.ToString();
                        
                        tmpArquivoDeSellOut.CodigoArquivoSellout = registro.ID.Value.ToString();
                        
                        tmpArquivoDeSellOut.CodigoConta = registro.Conta.Id.ToString();

                        tmpArquivoDeSellOut.DataEnvio = registro.DataDeEnvio.Value.ToLocalTime();

                        if (registro.DataDeProcessamento.HasValue)
                            tmpArquivoDeSellOut.DataProcessamento = registro.DataDeProcessamento.Value.ToLocalTime();

                        tmpArquivoDeSellOut.LoginUsuario = xml.LoginUsuario;

                        if (String.IsNullOrEmpty(registro.Nome))
                            tmpArquivoDeSellOut.Nome = (String)this.PreencherAtributoVazio("string");
                        else
                            tmpArquivoDeSellOut.Nome = registro.Nome;

                        tmpArquivoDeSellOut.Proprietario = usuarioIntegracao.ID.Value.ToString();

                        if (registro.Status.HasValue)
                            tmpArquivoDeSellOut.StatusProcessamento = registro.RazaoStatus.Value;
                        else
                            tmpArquivoDeSellOut.StatusProcessamento = 1;

                        tmpArquivoDeSellOut.TipoProprietario = "systemuser";
                        tmpArquivoDeSellOut.QuantidadeLinhasErro = (registro.QuantidadeLinhasErro.HasValue) ? registro.QuantidadeLinhasErro.Value : 0;
                        tmpArquivoDeSellOut.QuantidadeLinhasProcessadas = (registro.QuantidadeLinhasProcessadas.HasValue) ? registro.QuantidadeLinhasProcessadas.Value : 0;
                        tmpArquivoDeSellOut.QuantidadeLinhasDuplicadas = (registro.QuantidadeLinhasDuplicadas.HasValue) ? registro.QuantidadeLinhasDuplicadas.Value : 0;
                        tmpArquivoDeSellOut.QuantidadeTotalLinhas = (registro.QuantidadeTotalLinhas.HasValue) ? registro.QuantidadeTotalLinhas.Value : 0;
                        tmpArquivoDeSellOut.OrigemArquivo = registro.Origem;


                        lstRetorno.Add(tmpArquivoDeSellOut);
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

        public string Enviar(ArquivoDeSellOut objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

        ArquivoDeSellOut IBase<Pollux.MSG0163, ArquivoDeSellOut>.DefinirPropriedades(Pollux.MSG0163 legado)
        {
            throw new NotImplementedException();
        }
    }
}
