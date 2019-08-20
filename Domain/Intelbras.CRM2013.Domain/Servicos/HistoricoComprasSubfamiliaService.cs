using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasSubfamiliaService
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

        public HistoricoComprasSubfamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public HistoricoComprasSubfamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos


        public void RetornoDWHistoricoCompraSubfamilia(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraSubfamilia = RepositoryService.HistoricoComprasSubFamilia.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraSubfamilia.Rows)
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
                
                SubfamiliaProduto subFamilia = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_SubFamilia"].ToString());
                if (subFamilia == null)
                    continue;
                
                HistoricoComprasSubfamilia historicoCompraSubfamilia = RepositoryService.HistoricoComprasSubFamilia.ObterPor(mUnidadeNegocio.ID.Value,
                                                                                                                             subFamilia.ID.Value,
                                                                                                                             segmento.ID.Value,
                                                                                                                             familiaProduto.ID.Value,
                                                                                                                             ano,
                                                                                                                             trimestre);

                if (historicoCompraSubfamilia != null)
                {
                    historicoCompraSubfamilia.Valor = decimal.Parse(item["Valor"].ToString());

                    HistoricoComprasFamilia hCompraFamilia = RepositoryService.HistoricoComprasFamilia
                        .ObterPor(mUnidadeNegocio.ID.Value, historicoCompraSubfamilia.Segmento.Id, historicoCompraSubfamilia.Familia.Id, ano, trimestre);

                    if (hCompraFamilia != null)
                    {
                        historicoCompraSubfamilia.FamiliaRelacionamento = new Lookup(hCompraFamilia.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasSubFamilia.Update(historicoCompraSubfamilia);
                }
                else 
                {
                    HistoricoComprasSubfamilia hsCompSubfamilia = new HistoricoComprasSubfamilia(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompSubfamilia.UnidadeNegocio = new Lookup(mUnidadeNegocio.ID.Value, "");
                    hsCompSubfamilia.Segmento = new Lookup(segmento.ID.Value, "");
                    hsCompSubfamilia.Subfamilia = new Lookup(subFamilia.ID.Value, "");
                    hsCompSubfamilia.Familia = new Lookup(familiaProduto.ID.Value, "");
                    hsCompSubfamilia.Valor = decimal.Parse(item["Valor"].ToString());
                    hsCompSubfamilia.Nome = mUnidadeNegocio.Nome + " - " + subFamilia.Nome;
                    hsCompSubfamilia.Ano = ano;
                    hsCompSubfamilia.Trimestre = trimestre;

                    HistoricoComprasFamilia hCompraFamilia =  RepositoryService.HistoricoComprasFamilia
                        .ObterPor(mUnidadeNegocio.ID.Value, hsCompSubfamilia.Segmento.Id, hsCompSubfamilia.Familia.Id, ano, trimestre);

                    if (hCompraFamilia != null)
                    {
                        hsCompSubfamilia.FamiliaRelacionamento = new Lookup(hCompraFamilia.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasSubFamilia.Create(hsCompSubfamilia);
                }
            }
        }

        #endregion
    }
}
