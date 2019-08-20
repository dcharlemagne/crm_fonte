using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetadaUnidadeporTrimestreService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporTrimestreService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadaUnidadeporTrimestreService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Propertys/Objetos
        MetadaUnidadeporSegmentoService _ServiceMetadaUnidadeporSegmento = null;
        MetadaUnidadeporSegmentoService ServiceMetadaUnidadeporSegmento
        {
            get
            {
                if (_ServiceMetadaUnidadeporSegmento == null)
                    _ServiceMetadaUnidadeporSegmento = new MetadaUnidadeporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadaUnidadeporSegmento;
            }
        }

        MetadoCanalService _ServiceMetadoCanal = null;
        MetadoCanalService ServiceMetadoCanal
        {
            get
            {
                if (_ServiceMetadoCanal == null)
                    _ServiceMetadoCanal = new MetadoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadoCanal;
            }
        }

        PotencialdoKARepresentanteService _ServicePotencialdoKARepresentante = null;
        PotencialdoKARepresentanteService ServicePotencialdoKARepresentante
        {
            get
            {
                if (_ServicePotencialdoKARepresentante == null)
                    _ServicePotencialdoKARepresentante = new PotencialdoKARepresentanteService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoKARepresentante;
            }
        }

        PotencialdoSupervisorService _ServicePotencialdoSupervisor = null;
        PotencialdoSupervisorService ServicePotencialdoSupervisor
        {
            get
            {
                if (_ServicePotencialdoSupervisor == null)
                    _ServicePotencialdoSupervisor = new PotencialdoSupervisorService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoSupervisor;
            }
        }
        #endregion

        #region Métodos

        #region Unidade/Canal
        public void Atualiza(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            ServiceMetadaUnidadeporSegmento.Criar(mMetadaUnidadeporTrimestre, lstOrcamentoDetalhado);

            if (mMetadaUnidade.NiveldaMeta.Value == (int)Domain.Enum.MetaUnidade.NivelMeta.Detalhado)
                ServiceMetadoCanal.Criar(mMetadaUnidadeporTrimestre, mMetadaUnidade, lstOrcamentoDetalhado);

        }

        public void Criar(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, string Nome, int trimestre, Guid trimestreId)
        {
            MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre;

            mMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            mMetadaUnidadeporTrimestre.Ano = mMetadaUnidade.Ano;
            mMetadaUnidadeporTrimestre.UnidadedeNegocio = mMetadaUnidade.UnidadedeNegocios;
            mMetadaUnidadeporTrimestre.Trimestre = trimestre;
            mMetadaUnidadeporTrimestre.Nome = mMetadaUnidade.Nome + Nome;
            mMetadaUnidadeporTrimestre.MetadaUnidade = new Lookup(mMetadaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadaUnidade>());
            mMetadaUnidadeporTrimestre.ID = trimestreId;

            RepositoryService.MetadaUnidadeporTrimestre.Create(mMetadaUnidadeporTrimestre);
            ServiceMetadaUnidadeporSegmento.Criar(mMetadaUnidadeporTrimestre, lstOrcamentoDetalhado);

            if (mMetadaUnidade.NiveldaMeta.Value == (int)Domain.Enum.MetaUnidade.NivelMeta.Detalhado)
                ServiceMetadoCanal.Criar(mMetadaUnidadeporTrimestre, mMetadaUnidade, lstOrcamentoDetalhado);

        }
        #endregion

        #region MEta Manual
        public void AtualizaManual(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            //ServiceMetadoCanal.CriarManual(mMetadaUnidadeporTrimestre, mMetadaUnidade, lstOrcamentoDetalhado);
        }

        public void CriarManual(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, string Nome, int trimestre, Guid trimestreId)
        {
            MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre;

            mMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            mMetadaUnidadeporTrimestre.Ano = mMetadaUnidade.Ano;
            mMetadaUnidadeporTrimestre.UnidadedeNegocio = mMetadaUnidade.UnidadedeNegocios;
            mMetadaUnidadeporTrimestre.Trimestre = trimestre;
            mMetadaUnidadeporTrimestre.Nome = mMetadaUnidade.Nome + Nome;
            mMetadaUnidadeporTrimestre.MetadaUnidade = new Lookup(mMetadaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadaUnidade>());
            mMetadaUnidadeporTrimestre.ID = trimestreId;
            RepositoryService.MetadaUnidadeporTrimestre.Create(mMetadaUnidadeporTrimestre);

            //ServiceOrcamentodoCanal.CriarManual(mOrcamentodaUnidadeporTrimestre, mOrcamentodaUnidade, lstOrcamentoDetalhado);
            //ServiceMetadoCanal.CriarManual(mMetadaUnidadeporTrimestre, mMetadaUnidade, lstOrcamentoDetalhado);
        }

        public void AtualizaManualRetorno(Model.MetadaUnidade mMetadaUnidade, Trimestre trimestre, Guid canalId)
        {
            MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre = RepositoryService.MetadaUnidadeporTrimestre.Obterpor(mMetadaUnidade.ID.Value, trimestre.Id.Value);
            ServiceMetadoCanal.AtualizarManual(mMetadaUnidadeporTrimestre, trimestre, canalId);
        }

        public decimal AtualizaValores(Guid metaunidadeId)
        {
            decimal ValorTotal = 0M;
            try
            {
                List<MetadoCanal> lstMetadoCanal = new List<MetadoCanal>();
                decimal vlr = 0M;

                List<MetadaUnidadeporTrimestre> lstMetadaUnidadeporTrimestre = RepositoryService.MetadaUnidadeporTrimestre.Listarpor(metaunidadeId);
                foreach (var item in lstMetadaUnidadeporTrimestre)
                {
                    vlr = 0M;
                    lstMetadoCanal = RepositoryService.MetadoCanal.ListarPor(item.ID.Value);
                    foreach (var metacanal in lstMetadoCanal)
                    {
                        vlr += metacanal.MetaPlanejada.HasValue ? metacanal.MetaPlanejada.Value : 0;
                    }

                    ValorTotal += vlr;
                    item.MetaPlanejada = vlr;
                    RepositoryService.MetadaUnidadeporTrimestre.Update(item);
                }

                return ValorTotal;
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
        }
        #endregion

        #region Ka/Representante

        


        //public void CriarKARepresentante(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, string Nome, int trimestre, Guid trimestreId)
        //{
        //    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre;

        //    mMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
        //    mMetadaUnidadeporTrimestre.Ano = mMetadaUnidade.Ano;
        //    mMetadaUnidadeporTrimestre.UnidadedeNegocio = mMetadaUnidade.UnidadedeNegocios;
        //    mMetadaUnidadeporTrimestre.Trimestre = trimestre;
        //    mMetadaUnidadeporTrimestre.Nome = mMetadaUnidade.Nome + Nome;
        //    mMetadaUnidadeporTrimestre.MetadaUnidade = new Lookup(mMetadaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadaUnidade>());
        //    mMetadaUnidadeporTrimestre.ID = trimestreId;
        //    RepositoryService.MetadaUnidadeporTrimestre.Create(mMetadaUnidadeporTrimestre);

        //    ServicePotencialdoKARepresentante.Criar(mMetadaUnidade, mMetadaUnidadeporTrimestre, lstOrcamentoDetalhado);
        //}

        //public void AtualizaKARepresentante(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        //{
        //    ServicePotencialdoKARepresentante.Criar(mMetadaUnidade, mMetadaUnidadeporTrimestre, lstOrcamentoDetalhado);
        //}
        #endregion
            
        /// <summary>
        /// obtem todos os segmentos e atualiza o trimestre
        /// </summary>
        /// <param name="metaunidadeId"></param>
        public void CalcularMeta(Guid metaunidadeId)
        {
            List<MetadaUnidadeporTrimestre> lstOrcamentodaUnidadeporTri = RepositoryService.MetadaUnidadeporTrimestre.Listarpor(metaunidadeId);
            foreach (MetadaUnidadeporTrimestre item in lstOrcamentodaUnidadeporTri)
            {
                item.MetaPlanejada = 0;

                List<MetadaUnidadeporSegmento> lstSegmento = RepositoryService.MetadaUnidadeporSegmento.Obterpor(item.ID.Value);
                foreach (MetadaUnidadeporSegmento segmento in lstSegmento)
                {
                    item.MetaPlanejada += segmento.MetaPlanejada.HasValue ? segmento.MetaPlanejada.Value : 0;
                }

                RepositoryService.MetadaUnidadeporTrimestre.Update(item);
            }

        }

        public void RetornoDWTrimestre(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetas = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetas.Count == 0)
            {
                Console.WriteLine("{0} - Não foi encontrado nenhuma meta", DateTime.Now);
                return;
            }

            DataTable dtOrcamentoTrimestre = RepositoryService.MetadaUnidadeporTrimestre.ListarMetaTrimestreDW(ano, trimestre, lstMetas);

            Console.WriteLine("{0} - Existem {1} para ser atualizados", DateTime.Now, dtOrcamentoTrimestre.Rows.Count);

            foreach (DataRow item in dtOrcamentoTrimestre.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("CD_Unidade_Negocio"));

                if (mUnidadeNegocio != null)
                {
                    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre = RepositoryService.MetadaUnidadeporTrimestre.Obterpor(mUnidadeNegocio.ID.Value, ano, trimestre);

                    if (mMetadaUnidadeporTrimestre != null)
                    {
                        var metaUnidadeTrimestreUpdate = new Domain.Model.MetadaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = mMetadaUnidadeporTrimestre.ID,
                            MetaRealizada = item.Field<decimal>("vlr")
                        };

                        RepositoryService.MetadaUnidadeporTrimestre.Update(metaUnidadeTrimestreUpdate);
                    }
                }
            }
        }

        #endregion

        public void AtualizaStatusDosTrimestresNoCenarioaManual(Guid metaunidadeId, int _status)
        {
            List<MetadaUnidadeporTrimestre> lstOrcamentodaUnidadeporTri = RepositoryService.MetadaUnidadeporTrimestre.Listarpor(metaunidadeId);
            foreach (MetadaUnidadeporTrimestre trimestre in lstOrcamentodaUnidadeporTri)
            {
                //trimestre.RazaoStatus = _status;
                //RepositoryService.MetadaUnidadeporTrimestre.Update(trimestre);
                RepositoryService.MetadaUnidadeporTrimestre.SetState(trimestre, _status);
                this.AtualizaStatusDosCanaisPor(trimestre.ID.Value, _status);
            }
        }

        public void AtualizaStatusDosTrimestresPor(Guid metaunidadeId, int _status)
        {
            List<MetadaUnidadeporTrimestre> lstOrcamentodaUnidadeporTri = RepositoryService.MetadaUnidadeporTrimestre.Listarpor(metaunidadeId);
            foreach (MetadaUnidadeporTrimestre trimestre in lstOrcamentodaUnidadeporTri)
            {
                trimestre.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporTrimestre.Update(trimestre);
                this.AtualizaStatusDosSegmentosPor(trimestre.ID.Value, _status);
            }
        }

        public void AtualizaStatusDosSegmentosPor(Guid trimestreId, int _status)
        {
            List<MetadaUnidadeporSegmento> lstSegmento = RepositoryService.MetadaUnidadeporSegmento.Obterpor(trimestreId);
            foreach (MetadaUnidadeporSegmento segmento in lstSegmento)
            {
                segmento.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporSegmento.Update(segmento);
                this.AtualizaStatusDasFamiliasPor(segmento.ID.Value, _status);
            }
        }

        public void AtualizaStatusDosCanaisPor(Guid trimestreId, int _status)
        {
            List<MetadoCanal> canais = RepositoryService.MetadoCanal.ListarPor(trimestreId);
            //foreach (MetadoCanal metaCanal in canais)
            //{
            //    metaCanal.RazaoStatus = _status;
            //    RepositoryService.MetadoCanal.Update(metaCanal);
            //     Não se deve consolidar ate produto mes, pois o manual morre no canal
            //    this.AtualizaStatusDosProdutosDoMesPorCanal(metaCanal.ID.Value, _status);
            //}

            RepositoryService.MetadoCanal.SetStateMultiple(canais, _status);
        }

        public void AtualizaStatusDasFamiliasPor(Guid segmentoId, int _status)
        {
            List<MetadaUnidadeporFamilia> lstFamilia = RepositoryService.MetadaUnidadeporFamilia.ObterFamiliaporSegmento(segmentoId);
            foreach (MetadaUnidadeporFamilia familia in lstFamilia)
            {
                familia.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporFamilia.Update(familia);
                this.AtualizaStatusDasSubFamiliasPor(familia.ID.Value, _status);
            }
        }

        public void AtualizaStatusDasSubFamiliasPor(Guid familiaId, int _status)
        {
            List<MetadaUnidadeporSubfamilia> lstFamilia = RepositoryService.MetadaUnidadeporSubfamilia.ObterSubFamiliaPor(familiaId);
            foreach (MetadaUnidadeporSubfamilia subFamilia in lstFamilia)
            {
                subFamilia.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporSubfamilia.Update(subFamilia);
                this.AtualizaStatusDosProdutosPor(subFamilia.ID.Value, _status);
            }
        }

        public void AtualizaStatusDosProdutosPor(Guid subFamiliaId, int _status)
        {
            var lstproduto = RepositoryService.MetadaUnidadeporProduto.ListarPor(subFamiliaId);
            foreach (var produto in lstproduto)
            {
                produto.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporProduto.Update(produto);
                this.AtualizaStatusDosProdutosDoMesPor(produto.ID.Value, _status);
            }
        }

        public void AtualizaStatusDosProdutosDoMesPor(Guid produtoId, int _status)
        {
            List<MetaDetalhadadaUnidadeporProduto> lstprodutoMes = RepositoryService.MetadaUnidadeporProdutoMes.ListarPor(produtoId);
            foreach (var produtoMes in lstprodutoMes)
            {
                produtoMes.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporProdutoMes.Update(produtoMes);
            }
        }

        public void AtualizaStatusDosProdutosDoMesPorCanal(Guid canalId, int _status)
        {
            List<MetaDetalhadadaUnidadeporProduto> lstprodutoMes = RepositoryService.MetadaUnidadeporProdutoMes.ListarPorCanal(canalId);
            foreach (var produtoMes in lstprodutoMes)
            {
                produtoMes.RazaoStatus = _status;
                RepositoryService.MetadaUnidadeporProdutoMes.Update(produtoMes);
            }
        }
    }
}

