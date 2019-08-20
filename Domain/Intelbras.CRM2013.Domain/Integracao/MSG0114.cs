using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using SDKore.Configuration;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0114 : Base, IBase<Message.Helper.MSG0114, Domain.Model.DocumentoSharePoint>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor
        public MSG0114(string org, bool isOffline)
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
            //usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0114>(mensagem);
            string url = String.Empty;


            if (!String.IsNullOrEmpty(xml.CodigoEntidade) && xml.CodigoEntidade.Length == 36)
            {

                if (String.IsNullOrEmpty(xml.TipoObjeto))
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "TipoObjeto não enviado.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0114R1>(numeroMensagem, retorno);
                
                }

                //Obter endereço do servidor (urlAbsoluta)
                SharePointSite objSharePoint = new SharePointSite(this.Organizacao, this.IsOffline);
                objSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ObterPorUrlRelativa();

                if (objSharePoint == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "Site Sharepoint não encontrado no Crm.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0114R1>(numeroMensagem, retorno);
                }

                //Obter a pasta da conta
                List<DocumentoSharePoint> lstDocSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(new Guid(xml.CodigoEntidade));


                if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
                {
                    //url = lstDocSharePoint[0].UrlAbsoluta;
                    if(!String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                        url = lstDocSharePoint[0].UrlAbsoluta;
                    else
                        url = ConfigurationManager.GetSettingValue("UrlSiteSharePoint") + "/" + xml.TipoObjeto + "/" + lstDocSharePoint[0].UrlRelativa;
                }
                else
                {
                    resultadoConsulta.Sucesso = true;
                    resultadoConsulta.Mensagem = "Documentos não encontrados.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0114R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Identificador da Entidade (Guid) não enviado/fora do padrão..";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0114R1>(numeroMensagem, retorno);
            }

            resultadoConsulta.Sucesso = true;
            resultadoConsulta.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoConsulta);
            retorno.Add("URL", url);
            return CriarMensagemRetorno<Pollux.MSG0114R1>(numeroMensagem, retorno);
        }
        
        #endregion

        #region Definir Propriedades
        public DocumentoSharePoint DefinirPropriedades(Intelbras.Message.Helper.MSG0114 xml)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(DocumentoSharePoint objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
