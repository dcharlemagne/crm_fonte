using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using System.Data;
using System.Linq;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0292 : Base, IBase<Message.Helper.MSG0292, Domain.Model.SegmentoComercial>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0292(string org, bool isOffline) : base(org, isOffline)
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

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            var xml = this.CarregarMensagem<Pollux.MSG0292>(mensagem);
            List<Intelbras.Message.Helper.Entities.SegmentoComercialItem> lstSeguimentosComerciais = new List<Pollux.Entities.SegmentoComercialItem>();
            
            List<SegmentoComercial> lstSeguimentosComerciaisItens = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ListarTodos();
            List<SegmentoComercial> lstSeguimentosComerciaisVinculados = new List<SegmentoComercial>();

            if (xml.Conta != null)
                lstSeguimentosComerciaisVinculados = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ListarSegmentoPorConta(xml.Conta);

            #region Lista

            if (lstSeguimentosComerciaisItens != null && lstSeguimentosComerciaisItens.Count > 0)
            {
                foreach (SegmentoComercial crmItem in lstSeguimentosComerciaisItens)
                {
                    Pollux.Entities.SegmentoComercialItem objPollux = new Pollux.Entities.SegmentoComercialItem();

                    objPollux.CodigoSegmentoComercial = crmItem.CodigoSegmento.ToString();
                    if (crmItem.CodigoSegmentoPai != null)
                    {
                        SegmentoComercial segmentoComercial = new Servicos.SegmentoComercialService(this.Organizacao, this.IsOffline).ObterPor(crmItem.CodigoSegmentoPai.Id);
                        objPollux.CodigoSegmentoComercialPai = segmentoComercial.CodigoSegmento.ToString();
                    }                    
                    objPollux.NomeSegmentoComercial = crmItem.Nome;
                    objPollux.Ordem = crmItem.Ordem;

                    var vinculo = lstSeguimentosComerciaisVinculados.Select(campo => campo.Id.ToString().ToUpper()).FirstOrDefault(c => c == crmItem.Id.ToString().ToUpper()); //Verifica se já tem vinculo segmento à conta.
                    objPollux.Vinculado = false;
                    if (vinculo != null)
                        objPollux.Vinculado = true;
                                            
                    lstSeguimentosComerciais.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0292R1>(numeroMensagem, retorno);
            }

            #endregion
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("SegmentosComerciais", lstSeguimentosComerciais);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0292R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public SegmentoComercial DefinirPropriedades(Intelbras.Message.Helper.MSG0292 xml)
        {
            var crm = new Model.SegmentoComercial(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(SegmentoComercial objModel)
        {
            return String.Empty;
        }        
        #endregion
    }
}
