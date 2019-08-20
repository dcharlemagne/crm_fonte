using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace Intelbras.CRM2013.Application.WebServices
{
    /// <summary>
    /// Summary description for CrmWebApoioFormulario
    /// </summary>
    [WebService(Namespace = "urn:crm2013:intelbras.com.br/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CrmWebApoioFormulario : System.Web.Services.WebService
    {
        #region Metodos

        static CrmWebApoioFormulario()
        {
            Util.Utilitario._symmetricAlgorithm = new AesManaged();
            Util.Utilitario._symmetricAlgorithm.GenerateIV();
        }

        private string SDKoreOrganizacaoIntelbras
        {
            get { return ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"); }
        }

        private bool IsOffline
        {
            get { return false; }
        }

        [WebMethod]
        public CepViewModel BuscarCep(string cep)
        {
            try
            {
                return new EnderecoServices(SDKoreOrganizacaoIntelbras, IsOffline).BuscaCep(cep);
            }
            catch (Exception ex)
            {
                return new CepViewModel() { Menssagem = SDKore.Helper.Error.Handler(ex) };
            }
        }

        [WebMethod]
        public string GetListaFamiliaProd(string organizationName, string canalId)
        {
            FamiliaProdutoService familiaProdutoService = new FamiliaProdutoService(organizationName, false);
            SegmentoService segmentoService = new SegmentoService(organizationName, false);
            CanalVerdeService canalVerdeService = new CanalVerdeService(organizationName, false);

            var jsSerializer = new JavaScriptSerializer();
            try
            {
                var listaSeg = segmentoService.ListarCanaisVerdes();
                var listRet = new Dictionary<string, Dictionary<string, string>>();
                var listFam = canalVerdeService.listarPorCanal(new Guid(canalId));

                foreach (var itemSeg in listaSeg)
                {
                    var listaFam = new Dictionary<string, string>();
                    var canalGuid = new Guid(canalId);
                    var listaTmp = familiaProdutoService.ListarPorSegmento((Guid) itemSeg.ID, true, canalGuid, listFam);

                    foreach(var itemTmp in listaTmp)
                    {
                        listaFam.Add(itemTmp.ID.ToString(), itemTmp.Nome);
                    }

                    if(listaFam.Count > 0)
                    {
                        string segNameTmp = itemSeg.Nome + "|" + itemSeg.ID.ToString();
                        listRet.Add(segNameTmp, listaFam);
                    }
                }

                jsSerializer.MaxJsonLength = Int32.MaxValue;
                var json = jsSerializer.Serialize(listRet);

                return json.ToString();
            }
            catch (Exception ex)
            {
                jsSerializer.MaxJsonLength = Int32.MaxValue;

                Dictionary<string, string> retornoMapErr = new Dictionary<string, string>();

                retornoMapErr.Add("return", "FALSE");
                retornoMapErr.Add("message", ex.Message);
                var jsonErr = jsSerializer.Serialize(retornoMapErr);

                return jsonErr.ToString();
            }
        }

        [WebMethod]
        public string CopiarPoliticas(string organizationName, bool copiarProdutos, bool copiarEstados, DateTime dataInicialVigencia, DateTime dataFinalVigencia, string idDosRegistros)
        {
            string[] lstRegitrosId = idDosRegistros.Split(',');
            PoliticaComercialService politicaDomainService = new PoliticaComercialService(organizationName, false); 
            RepositoryService RepositoryService = new RepositoryService(organizationName, false);
            var jsSerializer = new JavaScriptSerializer();

            try
            {
                politicaDomainService.Copiar(organizationName, copiarProdutos, copiarEstados, dataInicialVigencia, dataFinalVigencia, lstRegitrosId);
            }
            catch (Exception e)
            {
                jsSerializer.MaxJsonLength = Int32.MaxValue;

                Dictionary<string, string> retornoMapErr = new Dictionary<string, string>();

                retornoMapErr.Add("return", "FALSE");
                retornoMapErr.Add("message", e.Message);
                var jsonErr = jsSerializer.Serialize(retornoMapErr);

                return jsonErr.ToString();
            }

          
            jsSerializer.MaxJsonLength = Int32.MaxValue;
            Dictionary<string, string> retornoMap = new Dictionary<string, string>();

            retornoMap.Add("return", "TRUE");
            var json = jsSerializer.Serialize(retornoMap);

            return json.ToString();
        }

        [WebMethod]
        public string CriarCanaisVerdesConta(string organizationName, string contaId, string familiasIds)
        {
            string[] lstRegitrosId = familiasIds.Split(';');
            CanalVerdeService canalVerdeService = new CanalVerdeService(organizationName, false);
            RepositoryService RepositoryService = new RepositoryService(organizationName, false);
            DomainExecuteMultiple multipleCreateReturn = null;

            var jsSerializer = new JavaScriptSerializer();

            try
            {
                List<CanalVerde> canalVerdeList = new List<CanalVerde>();
                foreach (var familiaIdTmp in lstRegitrosId)
                {
                    string[] segmentoEFamiliaIdsLst = familiaIdTmp.Split('|');
                    CanalVerde canalVerdeTmp = new CanalVerde(organizationName, false);

                    canalVerdeTmp.Segmento = new Lookup(new Guid(segmentoEFamiliaIdsLst[0]), "");
                    canalVerdeTmp.FamiliaProduto = new Lookup(new Guid(segmentoEFamiliaIdsLst[1]), "");
                    canalVerdeTmp.Canal = new Lookup(new Guid(contaId), "");

                    canalVerdeList.Add(canalVerdeTmp);
                }

                multipleCreateReturn = RepositoryService.CanalVerde.Create(canalVerdeList);
            }
            catch (Exception e)
            {
                jsSerializer.MaxJsonLength = Int32.MaxValue;

                Dictionary<string, string> retornoMapErr = new Dictionary<string, string>();

                retornoMapErr.Add("return", "FALSE");
                retornoMapErr.Add("message", e.Message);
                var jsonErr = jsSerializer.Serialize(retornoMapErr);

                return jsonErr.ToString();
            }


            jsSerializer.MaxJsonLength = Int32.MaxValue;
            Dictionary<string, string> retornoMap = new Dictionary<string, string>();

            if (multipleCreateReturn.IsFaulted)
            {
                retornoMap.Add("return", "FALSE");
            }
            else
            {
                retornoMap.Add("return", "TRUE");
            }

            var json = jsSerializer.Serialize(retornoMap);

            return json.ToString();
        }

        [WebMethod]
        public string ConsultaStatusGKO(string organizationName, String faturaId)
        {
            RepositoryService RepositoryService = new RepositoryService(organizationName, false);
            FaturaService faturaDomainService = new FaturaService(organizationName, false);
            EstabelecimentoService estabelecimentoDomainService = new EstabelecimentoService(organizationName, false);
            ContaService contaDomainService = new ContaService(organizationName, false);

            var jsSerializer = new JavaScriptSerializer();
            List<PosicaoEntregaViewModel> retornoMsg = null;
            try
            {
                var guidFatura = new Guid(faturaId);
                var FaturaObj = faturaDomainService.ObterPor(guidFatura);
                var estabelecimentoObj = estabelecimentoDomainService.BuscaEstabelecimento(FaturaObj.Estabelecimento.Id);
                var contaObj = contaDomainService.BuscaConta(FaturaObj.Cliente.Id);

                Domain.Integracao.MSG0176 msgGKO = new Domain.Integracao.MSG0176(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                retornoMsg = msgGKO.Enviar(FaturaObj, estabelecimentoObj, contaObj);
            }
            catch (Exception e)
            {
                jsSerializer.MaxJsonLength = Int32.MaxValue;

                Dictionary<string, string> retornoMapErr = new Dictionary<string, string>();

                retornoMapErr.Add("return", "FALSE");
                retornoMapErr.Add("message", e.Message);
                var jsonErr = jsSerializer.Serialize(retornoMapErr);

                return jsonErr.ToString();
            }

            jsSerializer.MaxJsonLength = Int32.MaxValue;
            var json = jsSerializer.Serialize(retornoMsg);

            return json.ToString();
        }

        #endregion
    }
}
