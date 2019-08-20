using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoSupervisorporFamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PotencialdoSupervisorporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoSupervisorporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion


        #region Objetos/propertys
        PotencialdoSupervisorporSubfamiliaService _ServicePotencialdoSupervisorporSubfamilia = null;
        PotencialdoSupervisorporSubfamiliaService ServicePotencialdoSupervisorporSubfamilia
        {
            get
            {
                if (_ServicePotencialdoSupervisorporSubfamilia == null)
                    _ServicePotencialdoSupervisorporSubfamilia = new PotencialdoSupervisorporSubfamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoSupervisorporSubfamilia;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoSupervisorporSegmento mPotencialdoSupervisorporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            PotencialdoSupervisorporFamilia mPotencialdoSupervisorporFamilia;
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             group x by string.Format("{0}/{1}", x.Familia.Id, x.Segmento.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                mPotencialdoSupervisorporFamilia = RepositoryService.PotencialdoSupervisorporFamilia.Obter(OrcaSegFamilia.First().Segmento.Id, OrcaSegFamilia.First().Familia.Id, mPotencialdoSupervisorporSegmento.ID.Value, OrcaSegFamilia.First().Canal.Id, mPotencialdoSupervisorporSegmento.Trimestre.Value);
                if (mPotencialdoSupervisorporFamilia == null)
                {
                    mPotencialdoSupervisorporFamilia = new PotencialdoSupervisorporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoSupervisorporFamilia.ID = Guid.NewGuid();
                    mPotencialdoSupervisorporFamilia.Supervisor = new Lookup(OrcaSegFamilia.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                    mPotencialdoSupervisorporFamilia.UnidadedeNegocio = mPotencialdoSupervisorporSegmento.UnidadedeNegocios;
                    mPotencialdoSupervisorporFamilia.Ano = mPotencialdoSupervisorporSegmento.Ano;
                    mPotencialdoSupervisorporFamilia.Trimestre = mPotencialdoSupervisorporSegmento.Trimestre;
                    mPotencialdoSupervisorporFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mPotencialdoSupervisorporFamilia.FamiliadoProduto = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mPotencialdoSupervisorporFamilia.PotencialdoSupervisorporSegmento = new Lookup(mPotencialdoSupervisorporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporSegmento>());

                    mPotencialdoSupervisorporFamilia.Nome = (mPotencialdoSupervisorporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mPotencialdoSupervisorporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mPotencialdoSupervisorporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.PotencialdoSupervisorporFamilia.Create(mPotencialdoSupervisorporFamilia);
                }
                ServicePotencialdoSupervisorporSubfamilia.Criar(mPotencialdoSupervisorporFamilia, OrcaSegFamilia.ToList());
            }
        }

        public void CalcularMetaSupervisor(Guid metaunidadeId)
        {
            List<PotencialdoSupervisorporFamilia> lstPotencialdoSupervisorporFamilia = RepositoryService.PotencialdoSupervisorporFamilia.ListarFamiliaporMeta(metaunidadeId);
            foreach (PotencialdoSupervisorporFamilia item in lstPotencialdoSupervisorporFamilia)
            {
                item.PotencialPlanejado = 0;

                List<PotencialdoSupervisorporSubfamilia> lstSubFamilia = RepositoryService.PotencialdoSupervisorporSubfamilia.ListarSubFamiliaPor(item.ID.Value);
                foreach (PotencialdoSupervisorporSubfamilia subfamilia in lstSubFamilia)
                {
                    item.PotencialPlanejado += subfamilia.PotencialPlanejado.HasValue ? subfamilia.PotencialPlanejado.Value : 0;
                }

                RepositoryService.PotencialdoSupervisorporFamilia.Update(item);
            }
        }

        public void RetornoDWFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            DataTable dtMetaCanal = RepositoryService.PotencialdoSupervisorporFamilia.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Usuario mUsuario = RepositoryService.Usuario.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());

                if (mUnidadeNegocio != null && mUsuario != null && mSegmento != null && mFamiliaProduto != null)
                {
                    PotencialdoSupervisorporFamilia mPotencialdoSupervisorporFamilia = RepositoryService.PotencialdoSupervisorporFamilia.Obter(mUnidadeNegocio.ID.Value, mUsuario.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value);
                    if (mPotencialdoSupervisorporFamilia != null)
                    {
                        mPotencialdoSupervisorporFamilia.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.PotencialdoSupervisorporFamilia.Update(mPotencialdoSupervisorporFamilia);
                    }
                }
            }
        }
        #endregion
    }
}

