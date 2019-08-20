using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PotencialdoKARepresentanteService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKARepresentanteService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public PotencialdoKARepresentanteService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public PotencialdoKARepresentanteService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }
        #endregion

        #region Métodos

        public void Criar(MetadaUnidade mMetadaUnidade, PotencialdoKAporTrimestre mMetadaUnidadeporTrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid temp)
        {
            PotencialdoKARepresentante mMetadoKA;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Canal.Id));

            foreach (var MetaCanal in lstOrcamentoporSegmento)
            {
                mMetadoKA = RepositoryService.PotencialdoKARepresentante.Obter(mMetadaUnidadeporTrimestre.ID.Value, MetaCanal.First().Canal.Id, mMetadaUnidadeporTrimestre.Trimestre.Value);
                if (mMetadoKA == null)
                {
                    mMetadoKA = new PotencialdoKARepresentante(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    mMetadoKA.Ano = mMetadaUnidade.Ano;
                    mMetadoKA.UnidadedeNegocio = mMetadaUnidade.UnidadedeNegocios;
                    mMetadoKA.Trimestre = mMetadaUnidadeporTrimestre.Trimestre;
                    mMetadoKA.Nome = mMetadaUnidadeporTrimestre.Nome;
                    mMetadoKA.KeyAccountRepresentante = new Lookup(MetaCanal.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mMetadoKA.ID = Guid.NewGuid();

                    RepositoryService.PotencialdoKARepresentante.Create(mMetadoKA);
                }

                //ServicePotencialdoKAporTrimestre.Criar(mMetadoKA, MetaCanal.ToList(), mMetadaUnidadeporTrimestre.ID.Value);
            }
        }

        public void CalcularMetaKa(Guid representanteId, Guid UNId, int ano)
        {
            PotencialdoKARepresentante potencialKA = RepositoryService.PotencialdoKARepresentante.ObterPor(representanteId, UNId, ano);

            potencialKA.PotencialPlanejado = 0;

            List<PotencialdoKAporTrimestre> lstTri = RepositoryService.PotencialdoKAporTrimestre.ListarPor(potencialKA.ID.Value, 0);
            foreach (PotencialdoKAporTrimestre trimestre in lstTri)
            {
                potencialKA.PotencialPlanejado += trimestre.PotencialPlanejado.HasValue ? trimestre.PotencialPlanejado.Value : 0;
            }

            RepositoryService.PotencialdoKARepresentante.Update(potencialKA);

        }

        public PortfoliodoKeyAccountRepresentantes ObterPor(Guid unidadeNegocioId, Guid contatoId)
        {
            return RepositoryService.PortfoliodoKeyAccountRepresentantes.ObterPor(unidadeNegocioId, contatoId);
        }

        public void RetornoDWKaRepresentante(int ano)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
            {
                return;
            }

            DataTable dtMetaCanal = RepositoryService.PotencialdoKARepresentante.ListarMetaTrimestreDW(ano, lstMetadaUnidade);

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

                    PotencialdoKARepresentante mPotencialdoKARepresentante = RepositoryService.PotencialdoKARepresentante.Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, "itbc_metakeyaccountid");

                    if (mPotencialdoKARepresentante != null)
                    {
                        mPotencialdoKARepresentante.PotencialRealizado = item.Field<decimal>("vlr");
                        RepositoryService.PotencialdoKARepresentante.Update(mPotencialdoKARepresentante);
                    }

                }

            }
        }

        private void CriarArquivoLog(MetadaUnidade metaUnidade, string mensagem, string pathTemp)
        {
            string file = pathTemp + "Log Error Metas.txt";
            System.IO.File.WriteAllText(file, mensagem);
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                ServiceArquivo.AnexaArquivo(file, "Log de Erros", @"application/plain", new Lookup(metaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(metaUnidade)));
            }
            finally
            {
                ServiceArquivo.ExcluirArquivo(file);
            }
        }

        #endregion

        #region Gerar Meta

        private List<PotencialdoKARepresentante> CriarListarPotencialKa(MetadaUnidade metaUnidade, List<PortfoliodoKeyAccountRepresentantes> listaPortfolioKa)
        {
            var listaPotencialKa = RepositoryService.PotencialdoKARepresentante.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            var listaRepresentantesId = (from x in listaPortfolioKa
                                         where x.KeyAccountRepresentante != null && x.State.Value == (int)Enum.PortfolioKa.State.Ativo
                                         group x by new { id = x.KeyAccountRepresentante.Id, name = x.KeyAccountRepresentante.Name } into g
                                         select new Lookup(g.Key.id, g.Key.name, SDKore.Crm.Util.Utility.GetEntityName<Contato>())).ToList();

            listaPotencialKa.ForEach(x => listaRepresentantesId.RemoveAll(y => y.Id == x.KeyAccountRepresentante.Id));

            foreach (var item in listaRepresentantesId)
            {
                var potencialdoKARepresentante = new PotencialdoKARepresentante(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                potencialdoKARepresentante.Ano = metaUnidade.Ano;
                potencialdoKARepresentante.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                potencialdoKARepresentante.Nome = "Potencial Meta KA - Ano " + metaUnidade.Ano + " Unidade Negocio - " + metaUnidade.UnidadedeNegocio.Nome;
                potencialdoKARepresentante.KeyAccountRepresentante = item;
                potencialdoKARepresentante.PotencialPlanejado = 0m;
                potencialdoKARepresentante.PotencialRealizado = 0m;

                potencialdoKARepresentante.ID = RepositoryService.PotencialdoKARepresentante.Create(potencialdoKARepresentante);
                listaPotencialKa.Add(potencialdoKARepresentante);
            }

            return listaPotencialKa;
        }

        private List<PotencialdoKAporTrimestre> CriarListarPotencialKaTrimestre(MetadaUnidade metaUnidade, List<PotencialdoKARepresentante> listaPortencialKa)
        {
            var listaPotencialKaTrimestre = RepositoryService.PotencialdoKAporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var novos = new List<PotencialdoKARepresentante>();

            foreach (var item in listaPortencialKa)
            {
                if (listaPotencialKaTrimestre.Find(x => x.PotencialdoKARepresentante.Id == item.ID.Value) == null)
                {
                    novos.Add(item);
                }
            }

            foreach (var item in novos)
            {
                var potencialdoKAporTrimestre1 = new PotencialdoKAporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                potencialdoKAporTrimestre1.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                potencialdoKAporTrimestre1.Ano = metaUnidade.Ano;
                potencialdoKAporTrimestre1.PotencialdoKARepresentante = new Lookup(item.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(item));
                potencialdoKAporTrimestre1.KeyAccountRepresentante = item.KeyAccountRepresentante;
                potencialdoKAporTrimestre1.PotencialPlanejado = 0m;
                potencialdoKAporTrimestre1.PotencialRealizado = 0m;
                potencialdoKAporTrimestre1.Nome = "1/" + metaUnidade.Ano + "  - " + metaUnidade.UnidadedeNegocios.Name;
                potencialdoKAporTrimestre1.Trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                potencialdoKAporTrimestre1.ID = RepositoryService.PotencialdoKAporTrimestre.Create(potencialdoKAporTrimestre1);
                listaPotencialKaTrimestre.Add(potencialdoKAporTrimestre1);

                var potencialdoKAporTrimestre2 = new PotencialdoKAporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                potencialdoKAporTrimestre2.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                potencialdoKAporTrimestre2.Ano = metaUnidade.Ano;
                potencialdoKAporTrimestre2.PotencialdoKARepresentante = new Lookup(item.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(item));
                potencialdoKAporTrimestre2.KeyAccountRepresentante = item.KeyAccountRepresentante;
                potencialdoKAporTrimestre2.PotencialPlanejado = 0m;
                potencialdoKAporTrimestre2.PotencialRealizado = 0m;
                potencialdoKAporTrimestre2.Nome = "2/" + metaUnidade.Ano + "  - " + metaUnidade.UnidadedeNegocios.Name;
                potencialdoKAporTrimestre2.Trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                potencialdoKAporTrimestre2.ID = RepositoryService.PotencialdoKAporTrimestre.Create(potencialdoKAporTrimestre2);
                listaPotencialKaTrimestre.Add(potencialdoKAporTrimestre2);


                var potencialdoKAporTrimestre3 = new PotencialdoKAporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                potencialdoKAporTrimestre3.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                potencialdoKAporTrimestre3.Ano = metaUnidade.Ano;
                potencialdoKAporTrimestre3.PotencialdoKARepresentante = new Lookup(item.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(item));
                potencialdoKAporTrimestre3.KeyAccountRepresentante = item.KeyAccountRepresentante;
                potencialdoKAporTrimestre3.PotencialPlanejado = 0m;
                potencialdoKAporTrimestre3.PotencialRealizado = 0m;
                potencialdoKAporTrimestre3.Nome = "3/" + metaUnidade.Ano + "  -  " + metaUnidade.UnidadedeNegocios.Name;
                potencialdoKAporTrimestre3.Trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                potencialdoKAporTrimestre3.ID = RepositoryService.PotencialdoKAporTrimestre.Create(potencialdoKAporTrimestre3);
                listaPotencialKaTrimestre.Add(potencialdoKAporTrimestre3);


                var potencialdoKAporTrimestre4 = new PotencialdoKAporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                potencialdoKAporTrimestre4.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                potencialdoKAporTrimestre4.Ano = metaUnidade.Ano;
                potencialdoKAporTrimestre4.PotencialdoKARepresentante = new Lookup(item.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(item));
                potencialdoKAporTrimestre4.KeyAccountRepresentante = item.KeyAccountRepresentante;
                potencialdoKAporTrimestre4.PotencialPlanejado = 0m;
                potencialdoKAporTrimestre4.PotencialRealizado = 0m;
                potencialdoKAporTrimestre4.Nome = "4/" + metaUnidade.Ano + "  -  " + metaUnidade.UnidadedeNegocios.Name;
                potencialdoKAporTrimestre4.Trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                potencialdoKAporTrimestre4.ID = RepositoryService.PotencialdoKAporTrimestre.Create(potencialdoKAporTrimestre4);
                listaPotencialKaTrimestre.Add(potencialdoKAporTrimestre4);
            }

            return listaPotencialKaTrimestre;
        }

        private List<PotencialdoKAporSegmento> CriarListarPotencialKaSegmento(MetadaUnidade metaUnidade, List<PotencialdoKAporTrimestre> listaPotencialKaTrimestre, List<Product> listaProdutos, List<PortfoliodoKeyAccountRepresentantes> listaPortfolioKa)
        {
            var listaPotencialKASegmento = RepositoryService.PotencialdoKAporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaSegmento = (from x in listaProdutos
                                 group x by new { id = x.Segmento.Id, name = x.Segmento.Name, type = x.Segmento.Type } into g
                                 select new Lookup(g.Key.id, g.Key.name, g.Key.type)).ToList();

            var listaPortfolioKaNovos = new List<PortfoliodoKeyAccountRepresentantes>();
            var novos = new List<PortfoliodoKeyAccountRepresentantes>();

            foreach (var item in listaPortfolioKa)
            {
                if (item.Segmento == null)
                {
                    foreach (var segmento in listaSegmento)
                    {
                        listaPortfolioKaNovos.Add(new PortfoliodoKeyAccountRepresentantes(item.OrganizationName, item.IsOffline)
                        {
                            ID = item.ID,
                            KeyAccountRepresentante = item.KeyAccountRepresentante,
                            Segmento = segmento,
                            SupervisordeVendas = item.SupervisordeVendas,
                            UnidadedeNegocio = item.UnidadedeNegocio
                        });
                    }
                }
                else
                {
                    listaPortfolioKaNovos.Add(new PortfoliodoKeyAccountRepresentantes(item.OrganizationName, item.IsOffline)
                    {
                        ID = item.ID,
                        KeyAccountRepresentante = item.KeyAccountRepresentante,
                        Segmento = item.Segmento,
                        SupervisordeVendas = item.SupervisordeVendas,
                        UnidadedeNegocio = item.UnidadedeNegocio
                    });
                }
            }

            foreach (var item in listaPortfolioKaNovos)
            {
                bool existe = listaPotencialKASegmento.FindAll(x => x.Segmento.Id == item.Segmento.Id && x.KAouRepresentante.Id == item.KeyAccountRepresentante.Id).Count > 0;

                if (!existe)
                {
                    novos.Add(item);
                }
            }

            foreach (var portfolioKa in novos)
            {
                foreach (var potencialTrimestre in listaPotencialKaTrimestre)
                {
                    if (portfolioKa.KeyAccountRepresentante.Id == potencialTrimestre.KeyAccountRepresentante.Id)
                    {
                        var potencialdoKAporSegmento = new PotencialdoKAporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        potencialdoKAporSegmento.Nome = portfolioKa.Segmento.Name;
                        potencialdoKAporSegmento.UnidadedeNegocio = potencialTrimestre.UnidadedeNegocio;
                        potencialdoKAporSegmento.Ano = potencialTrimestre.Ano;
                        potencialdoKAporSegmento.KAouRepresentante = portfolioKa.KeyAccountRepresentante;
                        potencialdoKAporSegmento.Supervisor = portfolioKa.SupervisordeVendas;
                        potencialdoKAporSegmento.PotencialKaTrimestre = new Lookup(potencialTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(potencialTrimestre));
                        potencialdoKAporSegmento.Segmento = portfolioKa.Segmento;
                        potencialdoKAporSegmento.Trimestre = potencialTrimestre.Trimestre;
                        potencialdoKAporSegmento.PotencialPlanejado = 0m;
                        potencialdoKAporSegmento.PotencialRealizado = 0m;

                        potencialdoKAporSegmento.ID = RepositoryService.PotencialdoKAporSegmento.Create(potencialdoKAporSegmento);

                        listaPotencialKASegmento.Add(potencialdoKAporSegmento);
                    }
                }
            }

            return listaPotencialKASegmento;
        }

        private List<PotencialdoKAporFamilia> CriarListarPotencialKaFamilia(MetadaUnidade metaUnidade, List<PotencialdoKAporSegmento> listaPotencialKaSegmento, List<Product> listaProdutos)
        {
            var listaPotencialKAFamiliaComercial = RepositoryService.PotencialdoKAporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaFamiliaComercialExistente = (from x in listaPotencialKAFamiliaComercial group x by x.FamiliadeProduto.Id into g select g.Key).ToList();
            var listaFamiliaComercial = (from x in listaProdutos
                                         group x by new
                                         {
                                             familiaId = x.FamiliaProduto.Id,
                                             familiaName = x.FamiliaProduto.Name,
                                             segmentoId = x.Segmento.Id,
                                             segmentoName = x.Segmento.Name
                                         } into g
                                         select new
                                         {
                                             familia = new Lookup(g.Key.familiaId, g.Key.familiaName, SDKore.Crm.Util.Utility.GetEntityName<FamiliaProduto>()),
                                             segmento = new Lookup(g.Key.segmentoId, g.Key.segmentoName, SDKore.Crm.Util.Utility.GetEntityName<Segmento>())
                                         }).ToList();


            // Excluir da lista já existentes
            //foreach (var item in listaFamiliaComercialExistente)
            //{
            //    listaFamiliaComercial.RemoveAll(x => x.familia.Id == item);
            //}

            // Criar novas metas
            foreach (var familiaComercial in listaFamiliaComercial)
            {
                foreach (var potenciaSegmento in listaPotencialKaSegmento)
                {
                    if (familiaComercial.segmento.Id == potenciaSegmento.Segmento.Id)
                    {
                        bool jaExiste = listaPotencialKAFamiliaComercial.Count(x => x.KAouRepresentante.Id == potenciaSegmento.KAouRepresentante.Id
                                                                                 && x.FamiliadeProduto.Id == familiaComercial.familia.Id
                                                                                 && x.Trimestre.Value == potenciaSegmento.Trimestre.Value) > 0;

                        if (!jaExiste)
                        {
                            var potencialdoKAporFamilia = new PotencialdoKAporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                            potencialdoKAporFamilia.KAouRepresentante = potenciaSegmento.KAouRepresentante;
                            potencialdoKAporFamilia.UnidadedeNegocio = potenciaSegmento.UnidadedeNegocio;
                            potencialdoKAporFamilia.Ano = potenciaSegmento.Ano;
                            potencialdoKAporFamilia.Trimestre = potenciaSegmento.Trimestre;
                            potencialdoKAporFamilia.Segmento = potenciaSegmento.Segmento;
                            potencialdoKAporFamilia.FamiliadeProduto = familiaComercial.familia;
                            potencialdoKAporFamilia.PotencialdoKAporSegmento = new Lookup(potenciaSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(potenciaSegmento));
                            potencialdoKAporFamilia.PotencialRealizado = 0m;
                            potencialdoKAporFamilia.PotencialPlanejado = 0m;
                            potencialdoKAporFamilia.Nome = familiaComercial.familia.Name;

                            potencialdoKAporFamilia.ID = RepositoryService.PotencialdoKAporFamilia.Create(potencialdoKAporFamilia);

                            listaPotencialKAFamiliaComercial.Add(potencialdoKAporFamilia);
                        }
                    }
                }
            }

            return listaPotencialKAFamiliaComercial;
        }

        private List<PotencialdoKAporSubfamilia> CriarListarPotencialKaSubFamilia(MetadaUnidade metaUnidade, List<PotencialdoKAporFamilia> listaPotencialKaFamilia, List<Product> listaProdutos)
        {
            var listaPotencialKASubFamiliaComercial = RepositoryService.PotencialdoKAporSubfamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaSubFamiliaComercialExistente = (from x in listaPotencialKASubFamiliaComercial
                                                     group x.SubfamiliadeProduto by x.SubfamiliadeProduto into g
                                                     select g.Key);

            var listaSubFamiliaComercialNovas = (from x in listaProdutos
                                                 group x by new
                                                 {
                                                     familiaId = x.FamiliaProduto.Id,
                                                     familiaName = x.FamiliaProduto.Name,
                                                     subfamiliaId = x.SubfamiliaProduto.Id,
                                                     subfamiliaName = x.SubfamiliaProduto.Name
                                                 } into g
                                                 select new
                                                 {
                                                     familia = new Lookup(g.Key.familiaId, g.Key.familiaName, SDKore.Crm.Util.Utility.GetEntityName<FamiliaProduto>()),
                                                     subFamilia = new Lookup(g.Key.subfamiliaId, g.Key.subfamiliaName, SDKore.Crm.Util.Utility.GetEntityName<SubfamiliaProduto>())
                                                 }).ToList();

            // Excluir da lista já existentes
            //foreach (var item in listaSubFamiliaComercialExistente)
            //{
            //    listaSubFamiliaComercialNovas.RemoveAll(x => x.subFamilia.Id == item.Id);
            //}

            // Criar novas metas
            foreach (var subfamiliaComercial in listaSubFamiliaComercialNovas)
            {
                foreach (var potenciaFamilia in listaPotencialKaFamilia)
                {
                    if (subfamiliaComercial.familia.Id == potenciaFamilia.FamiliadeProduto.Id)
                    {
                        bool jaExiste = listaPotencialKASubFamiliaComercial.Count(x => x.KAouRepresentante.Id == potenciaFamilia.KAouRepresentante.Id
                                                                                   && x.FamiliadeProduto.Id == subfamiliaComercial.familia.Id
                                                                                   && x.Trimestre.Value == potenciaFamilia.Trimestre.Value) > 0;

                        if (!jaExiste)
                        {
                            var potencialdoKAporSubfamilia = new PotencialdoKAporSubfamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                            potencialdoKAporSubfamilia.UnidadedeNegocio = potenciaFamilia.UnidadedeNegocio;
                            potencialdoKAporSubfamilia.Ano = potenciaFamilia.Ano;
                            potencialdoKAporSubfamilia.Trimestre = potenciaFamilia.Trimestre;
                            potencialdoKAporSubfamilia.Segmento = potenciaFamilia.Segmento;
                            potencialdoKAporSubfamilia.FamiliadeProduto = potenciaFamilia.FamiliadeProduto;
                            potencialdoKAporSubfamilia.SubfamiliadeProduto = subfamiliaComercial.subFamilia;
                            potencialdoKAporSubfamilia.PotencialdoKAporFamilia = new Lookup(potenciaFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(potenciaFamilia));
                            potencialdoKAporSubfamilia.Nome = subfamiliaComercial.subFamilia.Name;
                            potencialdoKAporSubfamilia.KAouRepresentante = potenciaFamilia.KAouRepresentante;
                            potencialdoKAporSubfamilia.PotencialPlanejado = 0m;
                            potencialdoKAporSubfamilia.PotencialRealizado = 0m;

                            potencialdoKAporSubfamilia.ID = RepositoryService.PotencialdoKAporSubfamilia.Create(potencialdoKAporSubfamilia);

                            listaPotencialKASubFamiliaComercial.Add(potencialdoKAporSubfamilia);
                        }
                    }
                }
            }

            return listaPotencialKASubFamiliaComercial;
        }

        private List<PotencialdoKAporProduto> CriarListarPotencialKaProduto(MetadaUnidade metaUnidade, List<PotencialdoKAporSubfamilia> listaPotencialKaSubFamilia, List<Product> listaProdutos)
        {
            var listaPotencialKAProduto = RepositoryService.PotencialdoKAporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaProdutosExistente = (from x in listaPotencialKAProduto
                                          group x by new
                                          {
                                              id = x.Produto.Id,
                                              name = x.Produto.Name,
                                              type = x.Produto.Type
                                          } into g
                                          select new Lookup(g.Key.id, g.Key.name, g.Key.type)).ToList();



            // Excluir da lista já existentes
            //foreach (var item in listaProdutosExistente)
            //{
            //    listaProdutos.RemoveAll(x => x.ID.Value == item.Id);
            //}

            // Criar novas metas
            foreach (var produto in listaProdutos)
            {
                foreach (var potenciaSubFamilia in listaPotencialKaSubFamilia)
                {
                    if (produto.SubfamiliaProduto.Id == potenciaSubFamilia.SubfamiliadeProduto.Id)
                    {
                        bool jaExiste = listaPotencialKAProduto.Count(x => x.KAouRepresentante.Id == potenciaSubFamilia.KAouRepresentante.Id
                                                                        && x.PotencialdoKAporSubfamilia.Id == potenciaSubFamilia.ID
                                                                        && potenciaSubFamilia.SubfamiliadeProduto.Id == produto.SubfamiliaProduto.Id
                                                                        && x.Trimestre.Value == potenciaSubFamilia.Trimestre.Value) > 0;

                        if (!jaExiste)
                        {
                            var potencialdoKAporProduto = new PotencialdoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                            potencialdoKAporProduto.Ano = potenciaSubFamilia.Ano;
                            potencialdoKAporProduto.UnidadeNegocio = potenciaSubFamilia.UnidadedeNegocio;
                            potencialdoKAporProduto.Trimestre = potenciaSubFamilia.Trimestre;
                            potencialdoKAporProduto.Produto = new Lookup(produto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(produto));
                            potencialdoKAporProduto.PotencialdoKAporSubfamilia = new Lookup(potenciaSubFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(potenciaSubFamilia));
                            potencialdoKAporProduto.Nome = produto.Nome;
                            potencialdoKAporProduto.KAouRepresentante = potenciaSubFamilia.KAouRepresentante;
                            potencialdoKAporProduto.PotencialRealizado = 0m;
                            potencialdoKAporProduto.PotencialPlanejado = 0m;
                            potencialdoKAporProduto.QtdePlanejada = 0;
                            potencialdoKAporProduto.QtdeRealizada = 0;

                            potencialdoKAporProduto.ID = RepositoryService.PotencialdoKAporProduto.Create(potencialdoKAporProduto);

                            listaPotencialKAProduto.Add(potencialdoKAporProduto);
                        }
                    }
                }
            }

            return listaPotencialKAProduto;
        }

        private List<MetaDetalhadadoKAporProduto> CriarListarPotencialKaProdutoMes(MetadaUnidade metaUnidade, List<PotencialdoKAporProduto> listaPotencialKaProduto)
        {
            var listaPotencialKAProdutoMes = RepositoryService.MetaDetalhadadoKAporProduto.ListarPorAnoUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaProdutosExistente = (from x in listaPotencialKAProdutoMes group x by x.Produto.Id into g select g.Key).ToList();
            var listaProdutosNovos = (from x in listaPotencialKaProduto
                                      group x by new
                                      {
                                          id = x.Produto.Id,
                                          name = x.Produto.Name,
                                          type = x.Produto.Type
                                      } into g
                                      select new Lookup(g.Key.id, g.Key.name, g.Key.type)).ToList();

            // Excluir da lista já existentes
            foreach (var item in listaProdutosExistente)
            {
                listaProdutosNovos.RemoveAll(x => x.Id == item);
            }

            // Criar novas metas
            foreach (var produto in listaProdutosNovos)
            {
                foreach (var potenciaProduto in listaPotencialKaProduto)
                {
                    if (produto.Id == potenciaProduto.Produto.Id)
                    {
                        var trimestre = (Enum.OrcamentodaUnidade.Trimestres)potenciaProduto.Trimestre.Value;
                        foreach (var mes in Helper.ListarMeses(trimestre))
                        {
                            var novo = CriarProdutoMes(potenciaProduto, mes);
                            listaPotencialKAProdutoMes.Add(novo);
                        }
                    }
                }
            }

            #region Representantes

            var listaRepresentantesExistente = (from x in listaPotencialKAProdutoMes group x by x.KAouRepresentante.Id into g select g.Key).ToList();
            var listaRepresentantesNovos = (from x in listaPotencialKaProduto
                                      group x by new
                                      {
                                          id = x.KAouRepresentante.Id,
                                          name = x.KAouRepresentante.Name,
                                          type = x.KAouRepresentante.Type
                                      } into g
                                      select new Lookup(g.Key.id, g.Key.name, g.Key.type)).ToList();
            
            // Excluir da lista já existentes
            foreach (var item in listaRepresentantesExistente)
            {
                listaRepresentantesNovos.RemoveAll(x => x.Id == item);
            }

            // Criar novas metas
            foreach (var representante in listaRepresentantesNovos)
            {
                foreach (var potenciaProduto in listaPotencialKaProduto)
                {
                    if (representante.Id == potenciaProduto.KAouRepresentante.Id)
                    {
                        var trimestre = (Enum.OrcamentodaUnidade.Trimestres)potenciaProduto.Trimestre.Value;
                        foreach (var mes in Helper.ListarMeses(trimestre))
                        {
                            var novo = CriarProdutoMes(potenciaProduto, mes);
                            listaPotencialKAProdutoMes.Add(novo);
                        }
                    }
                }
            }

            #endregion

            return listaPotencialKAProdutoMes;
        }

        private MetaDetalhadadoKAporProduto CriarProdutoMes(PotencialdoKAporProduto potencialdoKAporProduto, Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes)
        {
            var potencialKAporProduto = new MetaDetalhadadoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            potencialKAporProduto.MetadoKAporProduto = new Lookup(potencialdoKAporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(potencialdoKAporProduto));
            potencialKAporProduto.Ano = potencialdoKAporProduto.Ano;
            potencialKAporProduto.KAouRepresentante = potencialdoKAporProduto.KAouRepresentante;
            potencialKAporProduto.Trimestre = potencialdoKAporProduto.Trimestre;
            potencialKAporProduto.Produto = potencialdoKAporProduto.Produto;
            potencialKAporProduto.Nome = potencialdoKAporProduto.Nome;
            potencialKAporProduto.Mes = (int)mes;
            potencialKAporProduto.QtdePlanejada = 0;
            potencialKAporProduto.QtdeRealizada = 0;
            potencialKAporProduto.MetaRealizada = 0;
            potencialKAporProduto.MetaPlanejada = 0;

            potencialKAporProduto.ID = RepositoryService.MetaDetalhadadoKAporProduto.Create(potencialKAporProduto);

            return potencialKAporProduto;
        }

        public void CriarExtruturaMetaKeyAccount(MetadaUnidade metaUnidade, string pathTemp)
        {
            var trace = new Trace("Meta-KA-GERAR-" + metaUnidade.ID.Value);
            trace.Add("");
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando criação do template de Representante/KA{1}{2}", DateTime.Now, Environment.NewLine + Environment.NewLine + Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.GerandoMetaKARepresentante,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                #region Listando dados

                trace.Add("{0} - Listar produtos para metas", DateTime.Now);
                var listaProdutos = new ProdutoService(RepositoryService).ListarParaMeta(metaUnidade.UnidadedeNegocios.Id);
                trace.Add("{0} - Foram encontrados {1} proodutos", DateTime.Now, listaProdutos.Count);

                trace.Add("{0} - Listar portfólio para metas", DateTime.Now);
                var listaPortfolioKa = RepositoryService.PortfoliodoKeyAccountRepresentantes.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id);
                trace.Add("{0} - Foram encontrados {1} portfólios", DateTime.Now, listaPortfolioKa.Count);

                trace.Add("{0} - CriarListarPotencialKa", DateTime.Now);
                var listaPotencialKa = CriarListarPotencialKa(metaUnidade, listaPortfolioKa);
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKa.Count);

                trace.Add("{0} - CriarListarPotencialKa", DateTime.Now);
                var listaPotencialKaTrimestre = CriarListarPotencialKaTrimestre(metaUnidade, listaPotencialKa);
                listaPotencialKa = null;
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaTrimestre.Count);

                trace.Add("{0} - CriarListarPotencialKaSegmento", DateTime.Now);
                var listaPotencialKaSegmento = CriarListarPotencialKaSegmento(metaUnidade, listaPotencialKaTrimestre, listaProdutos, listaPortfolioKa);
                listaPotencialKaTrimestre = null;
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaSegmento.Count);

                trace.Add("{0} - CriarListarPotencialKaFamilia", DateTime.Now);
                var listaPotencialKaFamilia = CriarListarPotencialKaFamilia(metaUnidade, listaPotencialKaSegmento, listaProdutos);
                listaPotencialKaSegmento = null;
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaFamilia.Count);

                trace.Add("{0} - CriarListarPotencialKaSubFamilia", DateTime.Now);
                var listaPotencialKaSubFamilia = CriarListarPotencialKaSubFamilia(metaUnidade, listaPotencialKaFamilia, listaProdutos);
                listaPotencialKaFamilia = null;
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaSubFamilia.Count);

                trace.Add("{0} - CriarListarPotencialKaProduto", DateTime.Now);
                var listaPotencialKaProduto = CriarListarPotencialKaProduto(metaUnidade, listaPotencialKaSubFamilia, listaProdutos);
                listaPotencialKaSubFamilia = null;
                listaProdutos = null;
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaProduto.Count);

                trace.Add("{0} - CriarListarPotencialKaProdutoMes", DateTime.Now);
                var listaPotencialKaProdutoMes = CriarListarPotencialKaProdutoMes(metaUnidade, listaPotencialKaProduto);
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaPotencialKaProdutoMes.Count);

                var listaTodosRepresentantesMeta = RepositoryService.Contato.ListarKaRepresentantesPotencial(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "contactid", "statecode", "itbc_cpfoucnpj", "itbc_codigodorepresentante");
                var listaTodosProdutosMeta = RepositoryService.Produto.ListarPotencialKaRepresentante(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "productid", "itbc_businessunitid", "itbc_segmentoid", "itbc_familiadeprodid", "itbc_subfamiliadeproduto", "productnumber", "statecode", "name");

                foreach (var item in listaTodosRepresentantesMeta)
                {
                    bool inativo = listaPortfolioKa.Find(x => x.KeyAccountRepresentante.Id == item.ID.Value) == null;

                    if (inativo)
                    {
                        item.Status = (int)Enum.Contato.StateCode.Inativo;
                    }
                }

                #endregion

                #region Montando estrutura da planilha

                trace.Add("{0} - Criando estrutura de objetos para criar arquivo!", DateTime.Now);

                var listaModelo = from potencialProduto in listaPotencialKaProduto
                                  group potencialProduto by new
                                  {
                                      produtoId = potencialProduto.Produto.Id,
                                      produtoName = potencialProduto.Produto.Name,
                                      representanteName = potencialProduto.KAouRepresentante.Name,
                                      representanteId = potencialProduto.KAouRepresentante.Id
                                  } into g
                                  select new ModeloMetaKeyAccountViewModel()
                                  {
                                      Ano = metaUnidade.Ano,
                                      KeyAccount = new Lookup(g.Key.representanteId, g.Key.representanteName, SDKore.Crm.Util.Utility.GetEntityName<Contato>()),
                                      Produto = new Lookup(g.Key.produtoId, g.Key.produtoName, SDKore.Crm.Util.Utility.GetEntityName<Product>()),
                                      Ignorar = false
                                  };

                // Preenche informações do produto
                listaModelo = from modelo in listaModelo
                              join produto in listaTodosProdutosMeta on modelo.Produto.Id equals produto.ID.Value
                              select new ModeloMetaKeyAccountViewModel()
                              {
                                  Ano = modelo.Ano,
                                  KeyAccount = modelo.KeyAccount,
                                  Produto = new Lookup(produto.ID.Value, produto.Nome, SDKore.Crm.Util.Utility.GetEntityName(produto)),
                                  UnidadeNegocio = produto.UnidadeNegocio,
                                  Segmento = produto.Segmento,
                                  Familia = produto.FamiliaProduto,
                                  SubFamilia = produto.SubfamiliaProduto,
                                  CodigoProduto = produto.Codigo,
                                  StatusProduto = produto.Status.Value == (int)Enum.Produto.StateCode.ativo ? "Ativo" : "Inativo",
                                  Ignorar = produto.Status.Value == (int)Enum.Produto.StateCode.ativo ? modelo.Ignorar : true
                              };

                // Preenchendo informaçõs do representante
                var listaModeloFinal = (from modelo in listaModelo
                                        join representante in listaTodosRepresentantesMeta on modelo.KeyAccount.Id equals representante.ID.Value
                                        select new ModeloMetaKeyAccountViewModel()
                                        {
                                            Ano = modelo.Ano,
                                            KeyAccount = modelo.KeyAccount,
                                            Produto = modelo.Produto,
                                            UnidadeNegocio = modelo.UnidadeNegocio,
                                            Segmento = modelo.Segmento,
                                            Familia = modelo.Familia,
                                            SubFamilia = modelo.SubFamilia,
                                            CodigoProduto = modelo.CodigoProduto,
                                            StatusProduto = modelo.StatusProduto,
                                            StatusKeyAccount = representante.Status.Value == (int)Enum.Contato.StateCode.Ativo ? "Ativo" : "Inativo",
                                            Ignorar = representante.Status.Value == (int)Enum.Contato.StateCode.Ativo ? modelo.Ignorar : true,
                                            CnpjKeyAccount = representante.CpfCnpj,
                                            CodigoKeyAccount = representante.CodigoRepresentante
                                        }).ToList();

                for (int i = 0; i < listaModeloFinal.Count(); i++)
                {
                    var listaPotencialDetalhado = listaPotencialKaProdutoMes.FindAll(x => x.KAouRepresentante.Id == listaModeloFinal[i].KeyAccount.Id
                                                                                       && x.Produto.Id == listaModeloFinal[i].Produto.Id);

                    foreach (var potencialDetalhado in listaPotencialDetalhado)
                    {
                        var mes = (Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes)potencialDetalhado.Mes.Value;

                        listaModeloFinal[i].ListaProdutosMes[Helper.ConvertToInt(mes) - 1] = new ModeloMetaProdutoMesViewModel()
                        {
                            Mes = (Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes)potencialDetalhado.Mes.Value,
                            Quantidade = potencialDetalhado.QtdePlanejada,
                            Valor = potencialDetalhado.MetaPlanejada
                        };
                    }
                }

                trace.Add("{0} - finalizando estrutura de objetos para criar arquivo!", DateTime.Now);

                #endregion


                trace.Add("{0} - CriarExcelMetaKeyAccount (Inicio)", DateTime.Now);
                pathFile = ServiceArquivo.CriarExcelMetaKeyAccount(metaUnidade, listaModeloFinal, pathTemp);
                trace.Add("{0} - CriarExcelMetaKeyAccount (Fim)", DateTime.Now);

                ServiceArquivo.AnexaArquivo(metaUnidade, pathFile, "Meta_KaRepresentante_" + metaUnidade.Nome.Replace("/", ""));

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizado a criação do template de Representante/KA{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.MetaKARepresentanteGeradoSucesso,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            catch (Exception ex)
            {
                string mensagem = Error.Handler(ex);

                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroGerarMetaKARepresentante,
                    MensagemdeProcessamento = string.Format("{0} - {1}{2}{3}", DateTime.Now, mensagem, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000)
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        #endregion

        #region Importar Meta

        public void ImportarValores(MetadaUnidade metaUnidade, string pathTemp)
        {
            string pathFile = string.Empty;
            int quantidadePorLote = 1000;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            var trace = new Trace("Meta-KA-IMPORTAR-" + metaUnidade.ID.Value);
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            try
            {
                trace.Add("{0} - Alterando status da meta no CRM!", DateTime.Now);

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando Importação do Representante/KA{1}{2}{3}", DateTime.Now, Environment.NewLine, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ImportandoPlanilhaMetaKARepresentante,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                trace.Add("{0} - Fazendo download do arquivo", DateTime.Now);

                Observacao mObservaocao = RepositoryService.Observacao.Obter("itbc_metas", "objectid", "itbc_metasid", "itbc_metasid", metaUnidade.ID.Value);
                pathFile = ServiceArquivo.DownLoadArquivo(string.Concat(pathTemp, @"\"), mObservaocao.NomeArquivo, mObservaocao.Body, "");

                trace.Add("{0} - Convertando dados do Excel", DateTime.Now);

                List<string> listaErrosPlanilha;
                var itens = ServiceArquivo.ConvertDadosMetaKeyAccount(pathFile, out listaErrosPlanilha);

                if (listaErrosPlanilha.Count > 0)
                {
                    string file = pathTemp + "Log Error Metas.txt";
                    string mensagem = string.Join(Environment.NewLine, listaErrosPlanilha.ToArray());
                    CriarArquivoLog(metaUnidade, mensagem, file);

                    throw new ArgumentException("(CRM) Erro ao converter a planilha importada, consulte o arquivo de log nos anexos.");
                }

                trace.Add("{0} - Atualizando no CRM Potencial por produto mês", DateTime.Now);
                ImportarListarMetaKaDetalhadaProduto(metaUnidade, itens);

                trace.Add("{0} - Atualizando estrutura com valores de planejado", DateTime.Now);
                string mensagemErro = AtualizarValoresTodaEstrutura(metaUnidade, quantidadePorLote);

                if (mensagemErro.Length == 0)
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizando Importação do Representante/KA{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.PlanilhaMetaKARepresentanteImportadaSucesso,
                        RazaodoStatusMetaSupervisor = (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportarPlanilhaMetaSupervisor,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });
                }
                else
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação do Representante/KA{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroImportarMetaKARepresentante,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });

                    trace.Add("{0} - Criando arquivo de log", DateTime.Now);

                    string file = pathTemp + "Log Error Metas.txt";
                    CriarArquivoLog(metaUnidade, mensagemErro.ToString(), file);
                }

                trace.Add("{0} - Finalizando!", DateTime.Now);
            }
            catch (Exception ex)
            {
                string mensagem = Error.Handler(ex);
                trace.Add(ex);

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação: {1}{2}{3}", DateTime.Now, mensagem, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroImportarMetaKARepresentante,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        public string AtualizarValoresTodaEstrutura(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaErros = new Dictionary<Guid, string>();
            var mensagemErro = new StringBuilder();

            // Produto
            listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Produto!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Subfamilia
            listaErros = AtualizarValoresPotencialSubfamilia(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Subfamilia!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Familia
            listaErros = AtualizarValoresPotencialFamilia(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Familia!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Segmento
            listaErros = AtualizarValoresPotencialSegmento(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Segmento!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Trimestre
            listaErros = AtualizarValoresPotencialTrimestre(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Trimeste!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // KA
            listaErros = AtualizarValoresPotencialRepresentante(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização do Potencial por Representante!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            return mensagemErro.ToString();
        }

        private void ImportarListarMetaKaDetalhadaProduto(MetadaUnidade metaUnidade, List<ModeloMetaKeyAccountViewModel> listaModelo)
        {
            var listaPotencialKAProdutoMes = RepositoryService.MetaDetalhadadoKAporProduto.ListarPorAnoUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            foreach (var modeloProduto in listaModelo)
            {
                foreach (var modeloMes in modeloProduto.ListaProdutosMes)
                {
                    var item = listaPotencialKAProdutoMes.Find(x => x.Produto.Id == modeloProduto.Produto.Id
                                                                 && x.KAouRepresentante.Id == modeloProduto.KeyAccount.Id
                                                                 && x.Mes.Value == (int)modeloMes.Mes);

                    if (item == null)
                    {
                        throw new ArgumentException(string.Format("(CRM) Não foi encontrado o produto mês para Mês [{0}] KA [{1}] Produto [{2}]", modeloMes.Mes, modeloProduto.KeyAccount.Id, modeloProduto.Produto.Id));
                    }

                    if (modeloMes.Quantidade.Value != item.QtdePlanejada || item.MetaPlanejada.Value != modeloMes.Valor.Value)
                    {
                        RepositoryService.MetaDetalhadadoKAporProduto.Update(new MetaDetalhadadoKAporProduto(item.OrganizationName, item.IsOffline)
                        {
                            ID = item.ID,
                            MetaPlanejada = modeloMes.Valor.Value,
                            QtdePlanejada = modeloMes.Quantidade.Value,
                            RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporProdutoMes.StatusCode.Ativa
                        });
                    }
                }
            }
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();
            var listaTodosRepresentantesMeta = RepositoryService.Contato.ListarKaRepresentantesPotencial(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "contactid");

            foreach (var representante in listaTodosRepresentantesMeta)
            {
                var lista = RepositoryService.PotencialdoKAporProduto.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, representante.ID.Value);

                lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporProduto.StatusCode.Ativa);

                for (int i = 0; i < lista.Count; i += quantidadePorLote)
                {
                    var retorno = RepositoryService.PotencialdoKAporProduto.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                    foreach (var item in retorno)
                    {
                        listaError.Add(item.Key, item.Value);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialSubfamilia(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            foreach (int trimeste in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var lista = RepositoryService.PotencialdoKAporSubfamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimeste);

                lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporSubfamilia.StatusCode.Ativa);

                for (int i = 0; i < lista.Count; i += quantidadePorLote)
                {
                    var retorno = RepositoryService.PotencialdoKAporSubfamilia.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                    foreach (var item in retorno)
                    {
                        listaError.Add(item.Key, item.Value);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialFamilia(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var lista = RepositoryService.PotencialdoKAporFamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporFamilia.StatusCode.Ativa);

            for (int i = 0; i < lista.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.PotencialdoKAporFamilia.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialSegmento(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var lista = RepositoryService.PotencialdoKAporSegmento.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporSegmento.StatusCode.Ativa);

            for (int i = 0; i < lista.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.PotencialdoKAporSegmento.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialTrimestre(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var lista = RepositoryService.PotencialdoKAporTrimestre.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentanteporTrimestre.StatusCode.Ativa);

            for (int i = 0; i < lista.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.PotencialdoKAporTrimestre.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialRepresentante(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var lista = RepositoryService.PotencialdoKARepresentante.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            lista.ForEach(x => x.RazaoStatus = (int)Domain.Enum.PotencialdoKaRepresentante.StatusCode.Ativa);

            for (int i = 0; i < lista.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.PotencialdoKARepresentante.Update(lista.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        #endregion

        #region AtualizarRealizado

        public void AtualizaValoresRealizado(int ano, Enum.OrcamentodaUnidade.Trimestres _trimestre)
        {
            int trimestre = (int)_trimestre;
            var trace = new Trace("Meta-KA-ATUALIZARREALIZADO-" + ano + "-" + trimestre);
            trace.Add("");
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            try
            {
                List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

                if (lstMetadaUnidade.Count == 0)
                    return;

                trace.Add("{0} - Listar no DW", DateTime.Now);
                DataTable dtMetaCanal = RepositoryService.MetaDetalhadadoKAporProduto.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);


                trace.Add("{0} - Listar no Produtos, Unidades e Representantes no CRM", DateTime.Now);

                List<UnidadeNegocio> unidadeNegocioAll = RepositoryService.UnidadeNegocio.ListarTodosChaveIntegracao();
                Dictionary<string, UnidadeNegocio> unidadeNegocioMap = new Dictionary<string, UnidadeNegocio>();

                foreach (var tmpVar in unidadeNegocioAll)
                {
                    if (tmpVar.ChaveIntegracao != null)
                    {
                        unidadeNegocioMap.Add(tmpVar.ChaveIntegracao, tmpVar);
                    }
                }

                List<Contato> representatesAll = RepositoryService.Contato.ListarKaRepresentantesPotencial(null, ano, "contactid", "statecode", "itbc_cpfoucnpj", "itbc_codigodorepresentante");
                Dictionary<string, Contato> representatesMap = new Dictionary<string, Contato>();

                foreach (var tmpVar in representatesAll)
                {
                    if (tmpVar.CodigoRepresentante != null && representatesMap.ContainsKey(tmpVar.CodigoRepresentante))
                    {
                        representatesMap.Add(tmpVar.CodigoRepresentante, tmpVar);
                    }
                }

                var produtosAll = RepositoryService.Produto.ListarPotencialKaRepresentante(null, ano, "productid", "itbc_businessunitid", "itbc_segmentoid", "itbc_familiadeprodid", "itbc_subfamiliadeproduto", "productnumber", "statecode");
                Dictionary<string, Product> produtosMap = new Dictionary<string, Product>();

                foreach (var tmpVar in produtosAll)
                {
                    if (tmpVar.Codigo != null)
                    {
                        produtosMap.Add(tmpVar.Codigo, tmpVar);
                    }
                }


                trace.Add("{0} - Localizando e atualizando informações", DateTime.Now);

                List<MetaDetalhadadoKAporProduto> lstAtualizarRealizado = new List<MetaDetalhadadoKAporProduto>();
                foreach (DataRow item in dtMetaCanal.Rows)
                {
                    if (item.IsNull("CD_Unidade_Negocio") || item.IsNull("CD_representante") || item.IsNull("CD_Item"))
                    {
                        continue;
                    }

                    var tmpItem = item["CD_Item"].ToString();
                    if (representatesMap.ContainsKey(item["CD_representante"].ToString())
                         && produtosMap.ContainsKey(item["CD_Item"].ToString())
                        )
                    {
                        UnidadeNegocio mUnidadeNegocio = unidadeNegocioMap[item.Field<string>("CD_Unidade_Negocio")];
                        Contato mContato = representatesMap[item["CD_representante"].ToString()];
                        Product mProduto = produtosMap[item["CD_Item"].ToString()];

                        PotencialdoKAporProduto mPotencialdoKAporProduto = RepositoryService.PotencialdoKAporProduto
                            .Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), trimestre, mProduto.ID.Value);

                        if (mPotencialdoKAporProduto != null)
                        {
                            MetaDetalhadadoKAporProduto mMetaDetalhadadoKAporProduto = RepositoryService.MetaDetalhadadoKAporProduto
                                .Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), trimestre, Convert.ToInt32(item["cd_mes"].ToString()), mProduto.ID.Value, mPotencialdoKAporProduto.ID.Value);

                            if (mMetaDetalhadadoKAporProduto != null)
                            {
                                mMetaDetalhadadoKAporProduto.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                                mMetaDetalhadadoKAporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                                lstAtualizarRealizado.Add(mMetaDetalhadadoKAporProduto);
                            }
                        }
                    }
                }

                var listaError = new Dictionary<Guid, string>();
                var mensagemErro = new StringBuilder();

                for (int i = 0; i < lstAtualizarRealizado.Count; i += 1000)
                {
                    var retorno = RepositoryService.MetaDetalhadadoKAporProduto.Update(lstAtualizarRealizado.Skip(i).Take(1000).ToList());

                    foreach (var item in retorno)
                    {
                        listaError.Add(item.Key, item.Value);
                    }
                }

                if (listaError.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Produto/Mês!");
                    foreach (var item in listaError)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                    trace.Add(mensagemErro.ToString());
                }


                foreach (var metaUnidade in lstMetadaUnidade)
                {
                    trace.Add("{0} - Atualizando estrutura da meta com  realizado e planjado. Meta [{1}]", DateTime.Now, metaUnidade.Nome);
                    AtualizarValoresTodaEstrutura(metaUnidade, 1000);
                }

                trace.Add("{0} - Finalizando!", DateTime.Now);
            }
            catch (Exception ex)
            {
                trace.Add(ex);
                SDKore.Helper.Error.Handler(ex);
            }
            finally
            {
                trace.SaveClear();
            }
        }

        #endregion
    }
}