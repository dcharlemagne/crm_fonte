using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoKAporTrimestreService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PotencialdoKAporTrimestreService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoKAporTrimestreService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoKAporSegmentoService _ServicePotencialdoKAporSegmento = null;
        PotencialdoKAporSegmentoService ServicePotencialdoKAporSegmento
        {
            get
            {
                if (_ServicePotencialdoKAporSegmento == null)
                    _ServicePotencialdoKAporSegmento = new PotencialdoKAporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoKAporSegmento;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoKARepresentante mPotencialdoKARepresentante, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            int numTrimestreFormatado= 0;
            PotencialdoKAporTrimestre mPotencialdoKAporTrimestre;

            for (int cont = 1; cont <= 4; cont++)
            {                
                #region monta trimeste
                switch (cont)
                {
                    case 1:
                        numTrimestreFormatado = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                        break;
                    case 2:
                        numTrimestreFormatado =(int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                        break;
                    case 3:
                        numTrimestreFormatado = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                        break;
                    case 4:
                        numTrimestreFormatado = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                        break;

                }
                #endregion

                Console.WriteLine("Trimestre " + (int)numTrimestreFormatado + " em  " + cont + "/ " + cont + " as " + DateTime.Now.ToString());


                mPotencialdoKAporTrimestre = RepositoryService.PotencialdoKAporTrimestre.ObterPor(mPotencialdoKARepresentante.ID.Value, mPotencialdoKARepresentante.KeyAccountRepresentante.Id, numTrimestreFormatado);
                if (mPotencialdoKAporTrimestre == null)
                {
                    mPotencialdoKAporTrimestre = new PotencialdoKAporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoKAporTrimestre.ID = Guid.NewGuid();
                    mPotencialdoKAporTrimestre.Nome = "Trimestre " + numTrimestreFormatado + " / " + mPotencialdoKARepresentante.Ano + "  -  Unidade de Negocio " + mPotencialdoKARepresentante.UnidadedeNegocio.Name;
                    mPotencialdoKAporTrimestre.UnidadedeNegocio = mPotencialdoKARepresentante.UnidadedeNegocio;
                    mPotencialdoKAporTrimestre.Ano = mPotencialdoKARepresentante.Ano;
                    mPotencialdoKAporTrimestre.Trimestre = numTrimestreFormatado;
                    mPotencialdoKAporTrimestre.PotencialdoKARepresentante = new Lookup(mPotencialdoKARepresentante.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKARepresentante>());

                    RepositoryService.PotencialdoKAporTrimestre.Create(mPotencialdoKAporTrimestre);
                }

                ServicePotencialdoKAporSegmento.Criar(mPotencialdoKAporTrimestre, lstOrcamentoDetalhado, mPotencialdoKARepresentante.KeyAccountRepresentante.Id, numTrimestreFormatado);
            }
        }

        public void AtualizarFaturamentoDoSegmentoRepresentante(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporSegmento.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Contato mContato = RepositoryService.Contato.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                if (mUnidadeNegocio != null && mContato != null && mSegmento != null)
                {
                    PotencialdoKAporSegmento mPotencialdoKAporSegmento = RepositoryService.PotencialdoKAporSegmento.Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, trimestre, mSegmento.ID.Value);
                    if (mPotencialdoKAporSegmento != null)
                    {
                        mPotencialdoKAporSegmento.PotencialRealizado = item.Field<decimal>("vlr");
                        RepositoryService.PotencialdoKAporSegmento.Update(mPotencialdoKAporSegmento);
                    }
                }
            }

        }

        public void AtualizarRealizadoDoRepresentantePorTrimestre(int ano, int trimestre)
        {
            List<PotencialdoKAporTrimestre> lstPotencialTrimestre = RepositoryService.PotencialdoKAporTrimestre.ListarPorTrimestre(ano, trimestre);
            if (lstPotencialTrimestre.Count == 0)
                return;

            List<UnidadeNegocio> filterUnidadeNeg = new List<UnidadeNegocio>();
            foreach (var lstTmpUnidade in lstPotencialTrimestre)
            {
                UnidadeNegocio filterUnTmp = RepositoryService.UnidadeNegocio.ObterPor(lstTmpUnidade.UnidadedeNegocio.Id);
                filterUnidadeNeg.Add(filterUnTmp);
            }

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporTrimestre.ListarMetaTrimestreDW(ano, trimestre, filterUnidadeNeg);

            List<UnidadeNegocio> unidadeNegocioAll = RepositoryService.UnidadeNegocio.ListarTodosChaveIntegracao();
            Dictionary<string, UnidadeNegocio> unidadeNegocioMap = new Dictionary<string, UnidadeNegocio>();

            foreach (var tmpVar in unidadeNegocioAll)
            {
                if (tmpVar.ChaveIntegracao != null)
                {
                    unidadeNegocioMap.Add(tmpVar.ChaveIntegracao, tmpVar);
                }
            }

            List<Contato> representatesAll = RepositoryService.Contato.ListarKaRepresentantes(null);
            Dictionary<string, Contato> representatesMap = new Dictionary<string, Contato>();

            foreach (var tmpVar in representatesAll)
            {
                if (tmpVar.CodigoRepresentante != null)
                {
                    representatesMap.Add(tmpVar.CodigoRepresentante, tmpVar);
                }
            }

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio") || item.IsNull("CD_representante"))
                {
                    continue;
                }

                if (unidadeNegocioMap.ContainsKey(item.Field<string>("CD_Unidade_Negocio")) && representatesMap.ContainsKey(item["CD_representante"].ToString()))
                {
                    UnidadeNegocio mUnidadeNegocio = unidadeNegocioMap[item.Field<string>("CD_Unidade_Negocio")];
                    Contato mContato = representatesMap[item["CD_representante"].ToString()];

                    PotencialdoKAporTrimestre mPotencialdoKAporTrimestre = RepositoryService.PotencialdoKAporTrimestre.Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, trimestre);
                    if (mPotencialdoKAporTrimestre != null)
                    {
                        mPotencialdoKAporTrimestre.PotencialRealizado = item.Field<decimal>("vlr");
                        RepositoryService.PotencialdoKAporTrimestre.Update(mPotencialdoKAporTrimestre);
                    }
                }
            }

        }
        #endregion

        public void CalcularMeta(Guid potencialKA)
        {
            List<PotencialdoKAporTrimestre> lstPotencialporTri = RepositoryService.PotencialdoKAporTrimestre.Obter(potencialKA);
            foreach (PotencialdoKAporTrimestre item in lstPotencialporTri)
            {
                item.PotencialPlanejado = 0;

                List<PotencialdoKAporSegmento> lstSegmento = RepositoryService.PotencialdoKAporSegmento.ListarPorTrimestreId(item.ID.Value);
                foreach (PotencialdoKAporSegmento segmento in lstSegmento)
                {
                    item.PotencialPlanejado += segmento.PotencialPlanejado.HasValue ? segmento.PotencialPlanejado.Value : 0;
                }

                RepositoryService.PotencialdoKAporTrimestre.Update(item);
            }

        }
    }
}
