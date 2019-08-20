using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Pollux = Intelbras.Message.Helper;
using SDKore.Configuration;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.SharePoint.Client;
using System.Net;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0175 : Base, IBase<Pollux.MSG0175, Domain.Model.DocumentoCanaisExtranet>
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
        public MSG0175(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            //usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0175>(mensagem);
            string urlSite = String.Empty;
            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();

            #region Valida SomenteVigente
            if (!xml.SomenteVigente.HasValue)
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "SomenteVigente não enviado.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0175R1>(numeroMensagem, retorno);
            }
            #endregion

            var listaDocumentosCanaisExtranet = this.BuscarDocumentos(xml);
            urlSite = ObterSiteSharePoint();

            if (string.IsNullOrEmpty(urlSite))
                return CriarMensagemRetorno<Pollux.MSG0175R1>(numeroMensagem, retorno);

            listaDocs = ListarDocumentos(listaDocumentosCanaisExtranet, urlSite);

            if (listaDocs == null)
                return CriarMensagemRetorno<Pollux.MSG0175R1>(numeroMensagem, retorno);

            resultadoConsulta.Sucesso = true;
            resultadoConsulta.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoConsulta);
            retorno.Add("DocumentoItens", listaDocs);
            return CriarMensagemRetorno<Pollux.MSG0175R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades
        public DocumentoCanaisExtranet DefinirPropriedades(Intelbras.Message.Helper.MSG0175 xml)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(DocumentoCanaisExtranet objModel)
        {
            throw new NotImplementedException();
        }

        private List<DocumentoCanaisExtranet> BuscarDocumentos(Intelbras.Message.Helper.MSG0175 xml)
        {
            Guid[] classificacoesId = new Guid[0];
            Guid[] categoriasId = new Guid[xml.CategoriaItens.Count];
            int count = 0;
            Enum.DocumentoCanaisExtranet.RazaoStatus razaoStatus;

            if (!string.IsNullOrEmpty(xml.CodigoClassificacao))
            {
                Array.Resize<Guid>(ref classificacoesId, classificacoesId.Length + 1);
                classificacoesId.SetValue(new Guid(xml.CodigoClassificacao), 0);
            }

            foreach (var item in xml.CategoriaItens)
            {
                categoriasId.SetValue(new Guid(item.CodigoCategoria), count);
                count++;
            }

            if (xml.StatusDocumento.HasValue)
                razaoStatus = (Enum.DocumentoCanaisExtranet.RazaoStatus)xml.StatusDocumento;
            else
                razaoStatus = Enum.DocumentoCanaisExtranet.RazaoStatus.Aprovado;

            var lista = new DocumentoCanaisExtranetService(Organizacao, IsOffline).ListarDocumentosDoCanalDaExtranet(classificacoesId, categoriasId, razaoStatus, xml.SomenteVigente.Value);

            lista = AdicionarDocumentosParaTodosCanais(lista, razaoStatus, xml.SomenteVigente.Value);

            return lista;
        }

        private List<DocumentoCanaisExtranet> AdicionarDocumentosParaTodosCanais(List<DocumentoCanaisExtranet> lista, Enum.DocumentoCanaisExtranet.RazaoStatus razao, bool vigentes)
        {
            var novaLista = new DocumentoCanaisExtranetService(Organizacao, IsOffline).ListarParaTodosCanais(razao, vigentes);

            foreach (var item in novaLista)
            {
                lista.Add(item);
            }

            return lista;
        }

        private List<Pollux.Entities.DocumentoItem> ListarDocumentos(List<DocumentoCanaisExtranet> listaDocumentosCanaisExtranet, string urlSite)
        {
            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();
            string urlFolderDetail = String.Empty;

            if (listaDocumentosCanaisExtranet.Count > 0)
            {
                listaDocumentosCanaisExtranet = listaDocumentosCanaisExtranet.Distinct().ToList();
                foreach (var registroItem in listaDocumentosCanaisExtranet)
                {
                    //Obter a pasta do Regitro "Documento para Canais Extranet"
                    List<DocumentoSharePoint> lstDocSharePoint = new Servicos.SharePointSiteService(this.Organizacao, this.IsOffline).ListarPorIdRegistro(registroItem.ID.Value);

                    if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
                    {
                        if (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                            urlFolderDetail = lstDocSharePoint[0].UrlAbsoluta;
                        else
                        {
                            urlFolderDetail = "itbc_docscanaisextranet" + "/" + lstDocSharePoint[0].UrlRelativa;
                        }

                        listaDocs = BuscarArquivosSharePoint(urlSite, urlFolderDetail, registroItem, listaDocs);
                    }
                    else
                    {
                        resultadoConsulta.Sucesso = true;
                        resultadoConsulta.Mensagem = string.Format("Url dos Documentos do registro {0} não encontrada.", registroItem.Nome);
                        retorno.Add("Resultado", resultadoConsulta);
                        return null;
                    }
                }
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

        private List<Pollux.Entities.DocumentoItem> BuscarArquivosSharePoint(string urlSite, string urlFolderDetail, DocumentoCanaisExtranet registroItem, List<Pollux.Entities.DocumentoItem> listaDocs)
        {
            using (ClientContext spClientContext = new ClientContext(urlSite))
            {
                spClientContext.Credentials = new NetworkCredential(usuarioSharePoint, senhaSharePoint, domain);
                var rootWeb = spClientContext.Web;

                Folder pastaPrincipal = rootWeb.GetFolderByServerRelativeUrl(urlFolderDetail);

                spClientContext.Load(pastaPrincipal, fs => fs.Files, p => p.Folders);
                spClientContext.ExecuteQuery();

                FileCollection fileCollection = pastaPrincipal.Files;

                foreach (var arquivo in fileCollection)
                {
                    listaDocs.Add(MontarDocumento(arquivo, urlSite));
                }

                var rootweb = spClientContext.Web;
                FolderCollection folderCollection = rootweb.GetFolderByServerRelativeUrl(urlFolderDetail).Folders;

                spClientContext.Load(folderCollection, fs => fs.Include(f => f.ListItemAllFields));
                spClientContext.ExecuteQuery();

                foreach (Folder folder in folderCollection)
                {
                    // This property is now populated
                    var item = folder.ListItemAllFields;

                    // This is where the dates you want are stored
                    var created = (DateTime)item["Created"];
                    var nomedapasta = (string)item["Title"];
                    var urlrelativa = (string)item["FileRef"];

                    listaDocs.Add(MontarDocumentoPasta(created, nomedapasta, urlrelativa, urlSite, spClientContext));
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

        private Pollux.Entities.DocumentoItem MontarDocumentoPasta(DateTime datacriacao, string nomedapasta, string urlrelativa, string urlSite, ClientContext spClientContext)
        {
            Pollux.Entities.DocumentoItem docItem = new Pollux.Entities.DocumentoItem();
            List<Pollux.Entities.DocumentoItem> listaDocs = new List<Pollux.Entities.DocumentoItem>();

            // preenche com as informações da pasta
            docItem.DataCriacao = datacriacao;
            docItem.Nome = nomedapasta;
            docItem.Tamanho = string.Empty;
            docItem.TipoDocumento = 993520001;
            docItem.URL = ObterUrlArquivo(urlSite, urlrelativa);

            #region  Obtem os documentos da subpasta
            var rootWeb = spClientContext.Web;
            Folder pastaPrincipal = rootWeb.GetFolderByServerRelativeUrl(urlrelativa);

            spClientContext.Load(pastaPrincipal, fs => fs.Files, p => p.Folders);
            spClientContext.ExecuteQuery();

            FileCollection fileCollection = pastaPrincipal.Files;

            //lista os documentos da pasta
            foreach (var arquivo in fileCollection)
            {
                listaDocs.Add(MontarDocumento(arquivo, urlSite));
            }
            #endregion

            #region Obtem as subpasta que estão em camasas existem na subpasta pai
            var rootweb = spClientContext.Web;
            FolderCollection folderCollection = rootweb.GetFolderByServerRelativeUrl(urlrelativa).Folders;

            spClientContext.Load(folderCollection, fs => fs.Include(f => f.ListItemAllFields));
            spClientContext.ExecuteQuery();
            //lista os arquivos da subpasta
            foreach (Folder subFolder in folderCollection)
            {
                // This property is now populated
                var item = subFolder.ListItemAllFields;

                // This is where the dates you want are stored
                var created = (DateTime)item["Created"];
                var nomedasubpasta = (string)item["Title"];
                var urlrelativaSubpasta = (string)item["FileRef"];

                //Cria recursividade de documentos e subpastas
                listaDocs.Add(MontarDocumentoPasta(created, nomedasubpasta, urlrelativaSubpasta, urlSite, spClientContext));
            }
            #endregion

            docItem.DocumentoItens = listaDocs;

            return docItem;
        }
        #endregion
    }
}
