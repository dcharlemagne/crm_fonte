using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Pollux = Intelbras.Message.Helper;
using SDKore.Configuration;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.SharePoint.Client;
using SP = Microsoft.SharePoint.Client;
using System.Net;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0193 : Base
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };
        string usuarioSharePoint = SDKore.Helper.Cryptography.Decrypt(ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        string senhaSharePoint = SDKore.Helper.Cryptography.Decrypt(ConfigurationManager.GetSettingValue("SenhaSharePoint"));
        string domain = ConfigurationManager.GetDomain(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        #endregion

        #region Construtor
        public MSG0193(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            //usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0193>(mensagem);
            string urlSite = String.Empty;
            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();

            #region Valida Codigo
            if (string.IsNullOrEmpty(xml.CodigoConta))
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Códido da conta não informado.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0193R1>(numeroMensagem, retorno);
            }
            #endregion

            urlSite = ObterSiteSharePoint();

            if (string.IsNullOrEmpty(urlSite))
                return CriarMensagemRetorno<Pollux.MSG0193R1>(numeroMensagem, retorno);

            listaDocs = ListarDocumentos((new Guid(xml.CodigoConta)),
                                         xml.URL,
                                         xml.DataCriacao,
                                         xml.DataInicial,
                                         xml.DataFinal,
                                         urlSite);

            if (listaDocs == null)
                return CriarMensagemRetorno<Pollux.MSG0193R1>(numeroMensagem, retorno);

            resultadoConsulta.Sucesso = true;
            resultadoConsulta.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoConsulta);
            retorno.Add("DocumentoItens", listaDocs);
            return CriarMensagemRetorno<Pollux.MSG0193R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Métodos Auxiliares

        private List<Pollux.Entities.DocumentoItem> ListarDocumentos(Guid CodigoConta,
                                                                    string URL,
                                                                    DateTime? DataCriacao,
                                                                    DateTime? DataInicial,
                                                                    DateTime? DataFinal,
                                                                    string urlSite)
        {

            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();
            string urlFolderDetail = String.Empty;

            //Obter a pasta do Regitro "Documento para Canais Extranet"
            List<DocumentoSharePoint> lstDocSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(CodigoConta);

            if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
            {
                if (!string.IsNullOrEmpty(URL))
                {
                    urlFolderDetail = URL;
                }
                else
                {
                    if (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                        urlFolderDetail = lstDocSharePoint[0].UrlAbsoluta;
                    else
                    {
                        urlFolderDetail = string.Format("{0}{1}/{2}", urlSite, "account", lstDocSharePoint[0].UrlRelativa);
                    }
                }

                listaDocs = BuscarArquivosSharePoint(urlSite, urlFolderDetail, DataCriacao, DataInicial, DataFinal);
            }
            else
            {
                resultadoConsulta.Sucesso = true;
                resultadoConsulta.Mensagem = string.Format("Url dos Documentos do registro {0} não encontrada.", CodigoConta);
                retorno.Add("Resultado", resultadoConsulta);
                return null;
            }

            return listaDocs;
        }

        private string ObterSiteSharePoint()
        {
            //Obter endereço do servidor (urlAbsoluta)
            SharePointSite objSharePoint = new SharePointSite(this.Organizacao, this.IsOffline);
            objSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ObterPorUrlRelativa();

            if (objSharePoint == null)
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Site Sharepoint não encontrado no Crm.";
                retorno.Add("Resultado", resultadoConsulta);
                return string.Empty;
            }

            if (!String.IsNullOrEmpty(objSharePoint.UrlAbsoluta))
                return objSharePoint.UrlAbsoluta;
            else
                return ConfigurationManager.GetSettingValue("UrlSiteSharePoint");
        }

        private string ObterUrlArquivo(string urlSite, string urlServerRelative)
        {
            var siteSplit = urlSite.Split('/');
            urlSite = "";

            foreach (var itemSplited in siteSplit)
            {
                if (!string.IsNullOrEmpty(itemSplited))
                {
                    if (!urlServerRelative.Contains(itemSplited))
                    {
                        if (siteSplit.First() == itemSplited)
                            urlSite += itemSplited + "/";
                        else
                            urlSite += "/" + itemSplited;
                    }
                }
            }

            return urlSite + urlServerRelative;
        }

        private List<Pollux.Entities.DocumentoItem> BuscarArquivosSharePoint(string urlSite, string urlFolderDetail, DateTime? DataCriacao, DateTime? DataInicial, DateTime? DataFinal)
        {

            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();

            using (ClientContext spClientContext = new ClientContext(urlSite))
            {
                spClientContext.Credentials = new NetworkCredential(usuarioSharePoint, senhaSharePoint, domain);
                var rootWeb = spClientContext.Web;

                Folder pastaPrincipal = rootWeb.GetFolderByServerRelativeUrl(urlFolderDetail);

                spClientContext.Load(pastaPrincipal, fs => fs.Files, p => p.Folders);
                spClientContext.ExecuteQuery();
                FolderCollection folderCollection = pastaPrincipal.Folders;
                FileCollection fileCollection = pastaPrincipal.Files;


                foreach (var arquivo in fileCollection)
                {
                    if (DataCriacao.HasValue)
                    {
                        if (arquivo.TimeCreated.Date == DataCriacao)
                        {
                            listaDocs.Add(MontarDocumento(arquivo, urlSite));
                        }
                    }
                    else if (DataInicial.HasValue && DataFinal.HasValue)
                    {
                        if ((arquivo.TimeCreated.Date >= DataInicial) && (arquivo.TimeCreated.Date <= DataFinal))
                        {
                            listaDocs.Add(MontarDocumento(arquivo, urlSite));
                        }
                    }
                    else
                    {
                        listaDocs.Add(MontarDocumento(arquivo, urlSite));
                    }
                }
            }

            using (ClientContext spClientContext = new ClientContext(urlSite))
            {

                spClientContext.Credentials = new NetworkCredential(usuarioSharePoint, senhaSharePoint, domain);
                var rootweb = spClientContext.Web;

                FolderCollection folderCollection =
                    rootweb.GetFolderByServerRelativeUrl(urlFolderDetail).Folders;

                spClientContext.Load(folderCollection, fs => fs.Include(f => f.ListItemAllFields));
                spClientContext.ExecuteQuery();

                foreach (Folder folder in folderCollection)
                {
                    var item = folder.ListItemAllFields;

                    var datacriacao = (DateTime)item["Created"];
                    var nomedapasta = (string)item["Title"];
                    var urlrelativa = (string)item["FileRef"];

                    if (DataCriacao.HasValue)
                    {
                        if (datacriacao.Date == DataCriacao)
                        {
                            listaDocs.Add(MontarDocumentoPasta(datacriacao, nomedapasta, urlrelativa, urlSite));
                        }
                    }
                    else if (DataInicial.HasValue && DataFinal.HasValue)
                    {
                        if ((datacriacao.Date >= DataInicial) && (datacriacao <= DataFinal))
                        {
                            listaDocs.Add(MontarDocumentoPasta(datacriacao, nomedapasta, urlrelativa, urlSite));
                        }
                    }
                    else
                    {
                        listaDocs.Add(MontarDocumentoPasta(datacriacao, nomedapasta, urlrelativa, urlSite));
                    }
                }
            }

            return listaDocs;
        }

        private Pollux.Entities.DocumentoItem MontarDocumento(File arquivo, string urlSite)
        {
            Pollux.Entities.DocumentoItem docItem = new Pollux.Entities.DocumentoItem();

            docItem.DataCriacao = arquivo.TimeCreated;
            docItem.Nome = arquivo.Name;
            docItem.Tamanho = arquivo.Length.ToString();
            docItem.TipoDocumento = 993520000;
            docItem.URL = ObterUrlArquivo(urlSite, arquivo.ServerRelativeUrl);

            return docItem;
        }

        private Pollux.Entities.DocumentoItem MontarDocumentoPasta(DateTime datacriacao, string nomedapasta, string urlrelativa, string urlSite)
        {
            Pollux.Entities.DocumentoItem docItem = new Pollux.Entities.DocumentoItem();

            docItem.DataCriacao = datacriacao;
            docItem.Nome = nomedapasta;
            docItem.Tamanho = string.Empty;
            docItem.TipoDocumento = 993520001;
            docItem.URL = ObterUrlArquivo(urlSite, urlrelativa);

            return docItem;
        }

        #endregion
    }
}
