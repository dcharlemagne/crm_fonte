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
    public class MSG0160 : Base, IBase<Message.Helper.MSG0160, Domain.Model.FormaPagamento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0160(string org, bool isOffline)
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
            List<Intelbras.Message.Helper.Entities.FormaPagamentoItem> lstFormaPagamentoItem = new List<Pollux.Entities.FormaPagamentoItem>();
            //FormaPagamentoItem

            List<FormaPagamento> lstFormaPagamentos = new Servicos.FormaPagamentoService(this.Organizacao, this.IsOffline).Listar();

            #region Lista

            if (lstFormaPagamentos != null && lstFormaPagamentos.Count > 0)
            {
                foreach (FormaPagamento crmItem in lstFormaPagamentos)
                {
                    Pollux.Entities.FormaPagamentoItem objPollux = new Pollux.Entities.FormaPagamentoItem();

                    if (String.IsNullOrEmpty(crmItem.Nome))
                        objPollux.NomeFormaPagamento = (String)this.PreencherAtributoVazio("string");
                    else
                        objPollux.NomeFormaPagamento = crmItem.Nome;

                    objPollux.CodigoFormaPagamento = crmItem.ID.ToString();
                    
                    lstFormaPagamentoItem.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0160R1>(numeroMensagem, retorno);
            }

            #endregion

            retorno.Add("FormaPagamentoItens", lstFormaPagamentoItem);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0160R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public FormaPagamento DefinirPropriedades(Intelbras.Message.Helper.MSG0160 xml)
        {
            var crm = new Model.FormaPagamento(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(FormaPagamento objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
