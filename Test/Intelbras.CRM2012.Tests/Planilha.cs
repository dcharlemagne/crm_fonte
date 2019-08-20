using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Crm.Sdk.Query;
using System.IO;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class Planilha : Base
    {
        [Test]
        public void Solicitacao()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='itbc_produtosdasolicitacao'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            List<ProdutosdaSolicitacao> lst = (List<ProdutosdaSolicitacao>)sRepos.ProdutosdaSolicitacao.RetrieveMultiple(retrieveMultiple.Query).List;
            //ProdutosdaSolicitacao mProdutosdaSolicitacao = sRepos.ProdutosdaSolicitacao.Retrieve(Guid.Parse(""));
            ProdutosdaSolicitacao mProdutosdaSolicitacao = lst[0];

            ProdutosdaSolicitacaoService ServiceProdutosdaSolicitacao = new ProdutosdaSolicitacaoService(this.OrganizationName, this.IsOffline);
        }

        [Test]
        public void TesteFetch()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            //<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
            strFetchXml.Append("<entity name='itbc_metadocanalporsubfamilia'>");
            strFetchXml.Append("<attribute name='itbc_unidadedenegociosid' />");
            strFetchXml.Append("<attribute name='itbc_trimestre' />");
            strFetchXml.Append("<attribute name='itbc_subfamiliaid' />");
            strFetchXml.Append("<attribute name='itbc_segmentoid' />");
            strFetchXml.Append("<attribute name='itbc_metarealizada' />");
            strFetchXml.Append("<attribute name='itbc_metaplanejada' />");
            strFetchXml.Append("<attribute name='itbc_familiaid' />");
            strFetchXml.Append("<attribute name='itbc_ano' />");
            strFetchXml.Append("<attribute name='itbc_metadocanalporsubfamiliaid' />");
            strFetchXml.Append("<attribute name='itbc_canalid' />");
            strFetchXml.Append("<order attribute='itbc_ano' descending='false' />");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.Append("<condition attribute='itbc_subfamiliaid' operator='eq' uiname='Partes e pecas - ICON' uitype='itbc_subfamiliadeproduto' value='{3F37D33C-43E9-E311-9420-00155D013D39}' />");
            strFetchXml.Append("<condition attribute='itbc_canalid' operator='eq' uiname='ATOL DISTRIBUIDORA LTDA' uitype='account' value='{CA638629-2809-E411-9420-00155D013D39}' />");
            strFetchXml.Append("</filter>");
            strFetchXml.Append("<link-entity name='itbc_metadocanalporfamilia' from='itbc_metadocanalporfamiliaid' to='itbc_metadocanalporfamiliaid' alias='bh'>");
            strFetchXml.Append("<link-entity name='itbc_metadocanalporsegmento' from='itbc_metadocanalporsegmentoid' to='itbc_metadocanalporsegmentoid' alias='bi'>");
            strFetchXml.Append("<link-entity name='itbc_metadocanal' from='itbc_metadocanalid' to='itbc_metadocanalid' alias='bj'>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.Append("<condition attribute='itbc_metadotrimestreid' operator='in'>");
            strFetchXml.Append("<value uiname='ICON - 2017 - 4o Trimestre' uitype='itbc_metaportrimestre'>{A85FFB05-9432-4F00-9DAF-6D33C30CB33E}</value>");
            strFetchXml.Append("<value uiname='ICON - 2017 - 1o Trimestre' uitype='itbc_metaportrimestre'>{FE0B0D1C-A21B-4338-9905-4EA1CB08A254}</value>");
            strFetchXml.Append("</condition>");
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            List<MetadoCanalporSubFamilia> lst = (List<MetadoCanalporSubFamilia>)sRepos.MetadoCanalporSubFamilia.RetrieveMultiple(retrieveMultiple.Query).List;
            string strerro = string.Empty;

            foreach (var item in lst)
            {
                try
                {
                    //var teste = new PortfoliodoKeyAccountRepresentantes(OrganizationName, IsOffline);
                    //teste.KeyAccountRepresentante = item.KeyAccountRepresentante;
                    //teste.UnidadedeNegocio = item.UnidadedeNegocio;
                    //if (item.Segmento != null)
                    //    teste.Segmento = item.Segmento;
                    //teste.SupervisordeVendas = item.SupervisordeVendas;
                    //teste.AssistentedeAdministracaodeVendas = item.AssistentedeAdministracaodeVendas;

                    ////sRepos.PortfoliodoKeyAccountRepresentantes.Create(teste);
                    //item.UltAtualizacaoIntegracao = DateTime.Now;
                    //sRepos.PortfoliodoKeyAccountRepresentantes.Update(item);
                }
                catch (Exception erro)
                {
                    strerro = erro.Message;
                }
            }
        }

        [Test]
        public void AlteraStatus()
        {
            Guid orcamentounidadeId = Guid.Parse("1DE4C90D-6A49-E411-A6A3-00155D013D51");
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            OrcamentodaUnidade morcamento = sRepos.OrcamentodaUnidade.Retrieve(orcamentounidadeId);

            morcamento.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ErroGeracaoModeloOrcamento;
            morcamento.AddNullProperty("MensagemProcessamento");
            //sRepos.OrcamentodaUnidade.Update(morcamento);
        }

    }
}
