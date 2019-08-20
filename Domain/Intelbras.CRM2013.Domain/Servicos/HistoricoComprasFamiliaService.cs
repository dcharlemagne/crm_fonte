using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasFamiliaService
    {
        #region Atributos

        private static bool _isOffline = false;
        public static bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private static string _nomeDaOrganizacao = "";
        public static string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public static object Provider { get; set; }

        #endregion
        
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public HistoricoComprasFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos


        public void RetornoDWHistoricoCompraFamilia(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraFamilia = RepositoryService.HistoricoComprasFamilia.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                if (!mUnidadeNegocio.ID.HasValue)
                    continue;

                Segmento segmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                if (segmento == null)
                    continue;

                FamiliaProduto familiaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_Familia"].ToString());
                if (familiaProduto == null)
                    continue;


                HistoricoComprasFamilia historicoCompraFamilia = RepositoryService.HistoricoComprasFamilia.ObterPor(mUnidadeNegocio.ID.Value,
                                                                                                                             segmento.ID.Value,
                                                                                                                             familiaProduto.ID.Value, 
                                                                                                                             ano,
                                                                                                                             trimestre);
                if (historicoCompraFamilia != null)
                {
                    historicoCompraFamilia.Valor = decimal.Parse(item["Valor"].ToString());

                    HistoricoComprasSegmento hSegmento = RepositoryService.HistoricoComprasSegmento.ObterPor(historicoCompraFamilia.UnidadeNegocio.Id, historicoCompraFamilia.Segmento.Id, historicoCompraFamilia.Ano.Value, historicoCompraFamilia.Trimestre.ToString());

                    if (hSegmento != null)
                        historicoCompraFamilia.SegmentoRelacionamento = new Lookup(hSegmento.ID.Value, "");


                    RepositoryService.HistoricoComprasFamilia.Update(historicoCompraFamilia);
                }
                else 
                {
                    HistoricoComprasFamilia hsCompFamilia = new HistoricoComprasFamilia(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompFamilia.UnidadeNegocio = new Lookup(mUnidadeNegocio.ID.Value, "");
                    hsCompFamilia.Segmento = new Lookup(segmento.ID.Value, "");
                    hsCompFamilia.FamiliaDoProduto = new Lookup(familiaProduto.ID.Value, "");
                    hsCompFamilia.Valor = decimal.Parse(item["Valor"].ToString());
                    hsCompFamilia.Ano = ano;
                    hsCompFamilia.Trimestre = trimestre;
                    hsCompFamilia.Nome = mUnidadeNegocio.Nome + " - " + familiaProduto.Nome + " - " + hsCompFamilia.Ano.ToString();

                    HistoricoComprasSegmento hSegmento = RepositoryService.HistoricoComprasSegmento.ObterPor(hsCompFamilia.UnidadeNegocio.Id, hsCompFamilia.Segmento.Id, hsCompFamilia.Ano.Value, hsCompFamilia.Trimestre.ToString());
                    if (hSegmento != null)
                        hsCompFamilia.SegmentoRelacionamento = new Lookup(hSegmento.ID.Value,"");

                    RepositoryService.HistoricoComprasFamilia.Create(hsCompFamilia);
                }
            }
        }
        #endregion
    }
}
