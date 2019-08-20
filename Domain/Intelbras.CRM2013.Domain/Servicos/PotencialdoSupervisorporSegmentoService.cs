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

    public class PotencialdoSupervisorporSegmentoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoSupervisorporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoSupervisorporFamiliaService _ServicePotencialdoSupervisorporFamilia = null;
        PotencialdoSupervisorporFamiliaService ServicePotencialdoSupervisorporFamilia
        {
            get
            {
                if (_ServicePotencialdoSupervisorporFamilia == null)
                    _ServicePotencialdoSupervisorporFamilia = new PotencialdoSupervisorporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoSupervisorporFamilia;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoSupervisor mPotencialdoSupervisor, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            //PotencialdoSupervisorporSegmento mPotencialdoSupervisorporSegmento;
            //var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
            //                               group x by string.Format("{0}", x.Segmento.Id));

            //foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            //{
            //    mPotencialdoSupervisorporSegmento = RepositoryService.PotencialdoSupervisorporSegmento.Obter(mPotencialdoSupervisor.ID.Value, OrcaSegmento.First().Canal.Id, mPotencialdoSupervisor.Trimestre.Value, OrcaSegmento.First().Segmento.Id);
            //    if (mPotencialdoSupervisorporSegmento == null)
            //    {
            //        mPotencialdoSupervisorporSegmento = new PotencialdoSupervisorporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            //        mPotencialdoSupervisorporSegmento.ID = Guid.NewGuid();
            //        mPotencialdoSupervisorporSegmento.Nome = mPotencialdoSupervisor.Nome + " - " + OrcaSegmento.First().Segmento.Name;
            //        mPotencialdoSupervisorporSegmento.UnidadedeNegocios = mPotencialdoSupervisor.UnidadedeNegocio;
            //        mPotencialdoSupervisorporSegmento.Ano = mPotencialdoSupervisor.Ano;
            //        mPotencialdoSupervisorporSegmento.Supervisor = new Lookup(OrcaSegmento.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
            //        //mPotencialdoSupervisorporSegmento.Trimestre = mPotencialdoSupervisor.Trimestre;
            //        mPotencialdoSupervisorporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
            //        mPotencialdoSupervisorporSegmento.PotencialdoTrimestreSupervisor = new Lookup(mPotencialdoSupervisor.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisor>());

            //        RepositoryService.PotencialdoSupervisorporSegmento.Create(mPotencialdoSupervisorporSegmento);
            //    }
            //    ServicePotencialdoSupervisorporFamilia.Criar(mPotencialdoSupervisorporSegmento, OrcaSegmento.ToList());
            //}
        }

        public void CalcularMetaSupervisor(Guid metaunidadeId)
        {
            List<PotencialdoSupervisorporSegmento> lstPotencialdoSupervisorporSegmento = RepositoryService.PotencialdoSupervisorporSegmento.ListarPor(metaunidadeId);
            foreach (PotencialdoSupervisorporSegmento item in lstPotencialdoSupervisorporSegmento)
            {
                item.PotencialPlanejado = 0;

                List<PotencialdoSupervisorporFamilia> lstFamilia = RepositoryService.PotencialdoSupervisorporFamilia.ListarFamiliaporSegmento(item.ID.Value);
                foreach (PotencialdoSupervisorporFamilia familia in lstFamilia)
                {
                    item.PotencialPlanejado += familia.PotencialPlanejado.HasValue ? familia.PotencialPlanejado.Value : 0;
                }

                RepositoryService.PotencialdoSupervisorporSegmento.Update(item);
            }

        }

        public void AtualizarFaturamentoDoSegmentoSupervisor(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoSupervisorporSegmento.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Usuario mUsuario = RepositoryService.Usuario.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                if (mUnidadeNegocio != null && mUsuario != null && mSegmento != null)
                {
                    PotencialdoSupervisorporSegmento mPotencialdoSupervisorporSegmento = RepositoryService.PotencialdoSupervisorporSegmento.Obter(mUnidadeNegocio.ID.Value, mUsuario.ID.Value, ano, trimestre, mSegmento.ID.Value);
                    if (mPotencialdoSupervisorporSegmento != null)
                    {
                        mPotencialdoSupervisorporSegmento.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.PotencialdoSupervisorporSegmento.Update(mPotencialdoSupervisorporSegmento);
                    }
                }
            }
        }
        #endregion
    }
}

