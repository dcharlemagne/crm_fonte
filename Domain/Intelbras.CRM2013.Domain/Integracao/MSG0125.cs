using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0125 : Base, IBase<Message.Helper.MSG0125, Domain.Model.DocumentoSharePoint>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        List<Pollux.Entities.ArquivoItem> lstArquivoItens = new List<Pollux.Entities.ArquivoItem>();
        Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0125(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0125>(mensagem);

            if (!String.IsNullOrEmpty(xml.CodigoObjeto) && xml.CodigoObjeto.Length == 36)
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
                    return CriarMensagemRetorno<Pollux.MSG0125R1>(numeroMensagem, retorno);
                }

                //Obter a pasta da conta
                List<DocumentoSharePoint> lstDocSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(new Guid(xml.CodigoObjeto));


                foreach (var item in lstDocSharePoint)
                {
                    Pollux.Entities.ArquivoItem arqItem = new Pollux.Entities.ArquivoItem();
                    if (item != null && (!String.IsNullOrEmpty(item.UrlRelativa)) && !String.IsNullOrEmpty(objSharePoint.UrlAbsoluta))
                    {
                        arqItem.Nome = item.Nome;
                        arqItem.URL = objSharePoint.UrlAbsoluta + "/" + xml.TipoObjeto + "/" + item.UrlRelativa;
                        lstArquivoItens.Add(arqItem);
                    }
                    else
                    {
                        resultadoConsulta.Sucesso = true;
                        resultadoConsulta.Mensagem = "Dados do SharePoint no Crm não encontrados.";
                        retorno.Add("Resultado", resultadoConsulta);
                        return CriarMensagemRetorno<Pollux.MSG0125R1>(numeroMensagem, retorno);
                    }
                }
            }
            else 
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Identificador da Entidade (Guid) não enviado/fora do padrão.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0125R1>(numeroMensagem, retorno);
            }

            if (lstArquivoItens.Count == 0)
            {
                resultadoConsulta.Sucesso = true;
                resultadoConsulta.Mensagem = "Documentos não encontrados.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0125R1>(numeroMensagem, retorno);
            
            }

            resultadoConsulta.Sucesso = true;
            resultadoConsulta.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoConsulta);
            retorno.Add("ArquivoItems", lstArquivoItens);
            return CriarMensagemRetorno<Pollux.MSG0125R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades
        public DocumentoSharePoint DefinirPropriedades(Intelbras.Message.Helper.MSG0125 xml)
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
