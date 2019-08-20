using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetadoCanalService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public MetadoCanalService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        #region Propertys/Objetos
        MetadoCanalporSegmentoService _ServiceMetadoCanalporSegmento = null;
        private MetadoCanalporSegmentoService ServiceMetadoCanalporSegmento
        {
            get
            {
                if (_ServiceMetadoCanalporSegmento == null)
                    _ServiceMetadoCanalporSegmento = new MetadoCanalporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadoCanalporSegmento;
            }
        }

        MetaDetalhadadoCanalporProdutoService _ServiceMetaDetalhadadoCanalporProduto = null;
        private MetaDetalhadadoCanalporProdutoService ServiceMetaDetalhadadoCanalporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadoCanalporProduto == null)
                    _ServiceMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetaDetalhadadoCanalporProduto;
            }
        }
        #endregion

        #region Métodos
        public void Criar(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanal mMetadoCanal;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Canal.Id));

            foreach (var MetaCanal in lstOrcamentoporSegmento)
            {
                mMetadoCanal = RepositoryService.MetadoCanal.ObterPor(mMetadaUnidadeporTrimestre.ID.Value, MetaCanal.First().Canal.Id, mMetadaUnidadeporTrimestre.Ano.Value, mMetadaUnidadeporTrimestre.Trimestre.Value);
                if (mMetadoCanal == null)
                {
                    mMetadoCanal = new MetadoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    mMetadoCanal.Ano = mMetadaUnidade.Ano;
                    mMetadoCanal.UnidadedeNegocio = mMetadaUnidade.UnidadedeNegocios;
                    //mMetadoCanal.Trimestre = mMetadaUnidadeporTrimestre.Trimestre;
                    mMetadoCanal.Nome = mMetadaUnidadeporTrimestre.Nome;
                    mMetadoCanal.Canal = new Lookup(MetaCanal.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mMetadoCanal.ID = Guid.NewGuid();

                    RepositoryService.MetadoCanal.Create(mMetadoCanal);
                }

                ServiceMetadoCanalporSegmento.Criar(mMetadaUnidadeporTrimestre, mMetadoCanal, MetaCanal.ToList());
            }
        }

        public void CriarManual(MetadaUnidade metaUnidade, List<OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanal mMetadoCanal;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Canal.Id));

            foreach (var MetaCanal in lstOrcamentoporSegmento)
            {
                mMetadoCanal = RepositoryService.MetadoCanal.ObterPor(metaUnidade.UnidadedeNegocios.Id, MetaCanal.First().Canal.Id, metaUnidade.Ano.Value);
                if (mMetadoCanal == null)
                {
                    mMetadoCanal = new MetadoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    mMetadoCanal.Ano = metaUnidade.Ano;
                    mMetadoCanal.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    mMetadoCanal.Nome = metaUnidade.Nome;
                    mMetadoCanal.Canal = new Lookup(MetaCanal.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());

                    RepositoryService.MetadoCanal.Create(mMetadoCanal);
                }

                foreach (int item in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
                {
                    // Criar meta trimeste
                }

                //if (mMetadoCanal.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
                //{
                //    #region
                //    if (MetaCanal.First().Trimestre1 == null)
                //        MetaCanal.First().Trimestre1 = new Trimestre();

                //    if (MetaCanal.First().Trimestre1.trimestre == null)
                //        MetaCanal.First().Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                //    if (MetaCanal.First().Trimestre1.Mes1 == null)
                //        MetaCanal.First().Trimestre1.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Janeiro;

                //    if (MetaCanal.First().Trimestre1.Mes2 == null)
                //        MetaCanal.First().Trimestre1.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Fevereiro;

                //    if (MetaCanal.First().Trimestre1.Mes3 == null)
                //        MetaCanal.First().Trimestre1.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Marco;

                //    ServiceMetaDetalhadadoCanalporProduto.CriarManual(mMetadoCanal, MetaCanal.First().Trimestre1);
                //    #endregion
                //}
                //else if (mMetadoCanal.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
                //{
                //    #region
                //    if (MetaCanal.First().Trimestre2 == null)
                //        MetaCanal.First().Trimestre2 = new Trimestre();

                //    if (MetaCanal.First().Trimestre2.trimestre == null)
                //        MetaCanal.First().Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                //    if (MetaCanal.First().Trimestre2.Mes1 == null)
                //        MetaCanal.First().Trimestre2.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Abril;

                //    if (MetaCanal.First().Trimestre2.Mes2 == null)
                //        MetaCanal.First().Trimestre2.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Maio;

                //    if (MetaCanal.First().Trimestre2.Mes3 == null)
                //        MetaCanal.First().Trimestre2.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Junho;

                //    ServiceMetaDetalhadadoCanalporProduto.CriarManual(mMetadoCanal, MetaCanal.First().Trimestre2);
                //    #endregion
                //}
                //else if (mMetadoCanal.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
                //{
                //    #region
                //    if (MetaCanal.First().Trimestre3 == null)
                //        MetaCanal.First().Trimestre3 = new Trimestre();

                //    if (MetaCanal.First().Trimestre3.trimestre == null)
                //        MetaCanal.First().Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                //    if (MetaCanal.First().Trimestre3.Mes1 == null)
                //        MetaCanal.First().Trimestre3.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Julho;

                //    if (MetaCanal.First().Trimestre3.Mes2 == null)
                //        MetaCanal.First().Trimestre3.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Agosto;

                //    if (MetaCanal.First().Trimestre3.Mes3 == null)
                //        MetaCanal.First().Trimestre3.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Setembro;

                //    ServiceMetaDetalhadadoCanalporProduto.CriarManual(mMetadoCanal, MetaCanal.First().Trimestre3);
                //    #endregion
                //}
                //else if (mMetadoCanal.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
                //{
                //    #region
                //    if (MetaCanal.First().Trimestre4 == null)
                //        MetaCanal.First().Trimestre4 = new Trimestre();

                //    if (MetaCanal.First().Trimestre4.trimestre == null)
                //        MetaCanal.First().Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                //    if (MetaCanal.First().Trimestre4.Mes1 == null)
                //        MetaCanal.First().Trimestre4.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Outubro;

                //    if (MetaCanal.First().Trimestre4.Mes2 == null)
                //        MetaCanal.First().Trimestre4.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Novembro;

                //    if (MetaCanal.First().Trimestre4.Mes3 == null)
                //        MetaCanal.First().Trimestre4.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Dezembro;

                //    ServiceMetaDetalhadadoCanalporProduto.CriarManual(mMetadoCanal, MetaCanal.First().Trimestre4);
                //    #endregion
                //}
            }

        }

        public void AtualizarManual(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, Trimestre trimestre, Guid canalId)
        {
            MetadoCanal mMetadoCanal = RepositoryService.MetadoCanal.ObterPor(mMetadaUnidadeporTrimestre.ID.Value, canalId, mMetadaUnidadeporTrimestre.Ano.Value, trimestre.trimestre.Value);
            //mMetadoCanal.MetaPlanejada = mMetadoCanal.MetaPlanejada.HasValue ? mMetadoCanal.MetaPlanejada.Value : 0;

            if (mMetadoCanal != null)
            {

                mMetadoCanal.MetaPlanejada = trimestre.Mes1Vlr + trimestre.Mes2Vlr + trimestre.Mes3Vlr;

                ServiceMetaDetalhadadoCanalporProduto.CriarManual(mMetadoCanal, trimestre);
                RepositoryService.MetadoCanal.Update(mMetadoCanal);
            }
        }

        public MetadoCanal ObterPor(Guid unidNeg, Int32 trimestre, Guid Canal, int ano)
        {
            return RepositoryService.MetadoCanal.ObterPor(unidNeg, trimestre, Canal, ano);
        }

        public void RetornoDWMetaCanal(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
            {
                return;
            }

            DataTable dtMetaCanal = RepositoryService.MetadoCanal.ListarMetaCanalDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio") || item.IsNull("CD_Emitente"))
                {
                    continue;
                }

                //UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("CD_Unidade_Negocio"));
                //Conta mConta = RepositoryService.Conta.ObterCanal(item.Field<int>("CD_Emitente").ToString());

                var trimestreOrcamento = (Domain.Enum.OrcamentodaUnidade.Trimestres)trimestre;
                var chaveIntegracaoUnidadeNegocio = item.Field<string>("CD_Unidade_Negocio");
                var codigoEmitenteCanal = item["CD_Emitente"].ToString();
                MetadoCanal metaDoCanal = RepositoryService.MetadoCanal.ObterPorCodigo(chaveIntegracaoUnidadeNegocio, trimestreOrcamento, codigoEmitenteCanal, ano, "itbc_metadocanalid");

                //MetadoCanal metaDoCanal = RepositoryService.MetadoCanal.ObterPor(mUnidadeNegocio.ID.Value, trimestre, mConta.ID.Value, ano, "itbc_metadocanalid");

                if (metaDoCanal != null)
                {
                    var metaDoCanalUpdate = new MetadoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        ID = metaDoCanal.ID,
                        MetaRealizada = item.IsNull("vlr") ? 0 : item.Field<decimal>("vlr")
                    };

                    RepositoryService.MetadoCanal.Update(metaDoCanalUpdate);
                }
            }
        }

        #endregion

        #region Gerar Meta

        public void CriarExtruturaMetaDetalhada(MetadaUnidade metaUnidade, string pathTemp)
        {
            var trace = new Trace("Meta-Canal-GERAR-" + metaUnidade.ID.Value);
            trace.Add("");
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            int quantidadePorLote = 1000;
            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando criação do template de Canal{1}{2}", DateTime.Now, Environment.NewLine + Environment.NewLine + Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.GerandoMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                #region Listando dados

                trace.Add("{0} - Listar produtos para metas", DateTime.Now);
                var listaProdutos = new ProdutoService(RepositoryService).ListarParaMeta(metaUnidade.UnidadedeNegocios.Id);
                trace.Add("{0} - Foram encontrados {1} proodutos", DateTime.Now, listaProdutos.Count);

                trace.Add("{0} - Listar canais para metas", DateTime.Now);
                var listaCanais = RepositoryService.Conta.ListarParticipantesDoProgramaApenasComApuracaoBeneficio(metaUnidade.UnidadedeNegocios.Id);
                trace.Add("{0} - Foram encontrados {1} canais", DateTime.Now, listaCanais.Count);

                trace.Add("{0} - CriarListarMetaCanal", DateTime.Now);
                var listaMetaCanal = CriarListarMetaCanal(metaUnidade, listaCanais);
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaMetaCanal.Count);

                trace.Add("{0} - CriarMetaCanalTrimestre", DateTime.Now);
                var listaErros = CriarMetaCanalTrimestre(metaUnidade, listaMetaCanal, quantidadePorLote);
                listaMetaCanal = null;

                trace.Add("{0} - CriarMetaCanalSegmento", DateTime.Now);
                var listaMetaCanalSegmento = CriarMetaCanalSegmento(metaUnidade, listaProdutos, quantidadePorLote);

                trace.Add("{0} - CriarListarPotencialKaFamilia", DateTime.Now);
                listaErros = CriarMetaCanalFamilia(metaUnidade, listaProdutos, quantidadePorLote);

                trace.Add("{0} - CriarMetaCanalSubfamilia", DateTime.Now);
                var listaPotencialKaSubFamilia = CriarMetaCanalSubfamilia(metaUnidade, listaProdutos, quantidadePorLote);

                trace.Add("{0} - CriarMetaCanalProduto", DateTime.Now);
                listaErros = CriarMetaCanalProduto(metaUnidade, listaProdutos, quantidadePorLote);

                trace.Add("{0} - CriarMetaCanalProdutoMes", DateTime.Now);
                listaErros = CriarMetaCanalProdutoMes(metaUnidade, listaProdutos, quantidadePorLote);

                #endregion

                trace.Add("{0} - ListarModeloDados (Inicio)", DateTime.Now);
                var listarModeloDados = ListarModeloDadosDetalhado(metaUnidade);
                trace.Add("{0} - ListarModeloDados (Fim)", DateTime.Now);

                trace.Add("{0} - CriarExcelMetaDetalhadaCanal (Inicio)", DateTime.Now);
                pathFile = ServiceArquivo.CriarExcelMetaDetalhadaCanal(metaUnidade, listarModeloDados, pathTemp);
                trace.Add("{0} - CriarExcelMetaDetalhadaCanal (Fim)", DateTime.Now);

                ServiceArquivo.AnexaArquivo(metaUnidade, pathFile, "Meta_Canal_" + metaUnidade.Nome.Replace("/", ""));

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizado a criação do template de Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.MetaCanalManualGeradoSucesso,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            catch (Exception ex)
            {
                string mensagem = Error.Handler(ex);

                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroGerarMetaCanalManual,
                    MensagemdeProcessamento = string.Format("{0} - {1}{2}{3}", DateTime.Now, mensagem, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000)
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        public void CriarExtruturaMetaResumida(MetadaUnidade metaUnidade, string pathTemp)
        {
            var trace = new Trace("Meta-Canal-GERAR-" + metaUnidade.ID.Value);
            trace.Add("");
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            int quantidadePorLote = 1000;
            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando criação do template de Canal{1}{2}", DateTime.Now, Environment.NewLine + Environment.NewLine + Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.GerandoMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                #region Listando dados

                trace.Add("{0} - Listar canais para metas", DateTime.Now);
                var listaCanais = RepositoryService.Conta.ListarParticipantesDoProgramaApenasComApuracaoBeneficio(metaUnidade.UnidadedeNegocios.Id);
                trace.Add("{0} - Foram encontrados {1} canais", DateTime.Now, listaCanais.Count);

                trace.Add("{0} - CriarListarMetaCanal", DateTime.Now);
                var listaMetaCanal = CriarListarMetaCanal(metaUnidade, listaCanais);
                trace.Add("{0} - Resultado de {1} registros", DateTime.Now, listaMetaCanal.Count);

                trace.Add("{0} - CriarMetaCanalTrimestre", DateTime.Now);
                var listaErros = CriarMetaCanalTrimestre(metaUnidade, listaMetaCanal, quantidadePorLote);
                listaMetaCanal = null;

                trace.Add("{0} - CriarMetaCanalProdutoMes", DateTime.Now);
                listaErros = CriarMetaCanalProdutoMesResumido(metaUnidade, quantidadePorLote);

                #endregion

                trace.Add("{0} - ListarModeloDados (Inicio)", DateTime.Now);
                var listarModeloDados = ListarModeloDadosResumido(metaUnidade);
                trace.Add("{0} - ListarModeloDados (Fim)", DateTime.Now);

                trace.Add("{0} - CriarExcelMetaDetalhadaCanal (Inicio)", DateTime.Now);
                pathFile = ServiceArquivo.CriarExcelMetaResumidoCanal(metaUnidade, listarModeloDados, pathTemp);
                trace.Add("{0} - CriarExcelMetaDetalhadaCanal (Fim)", DateTime.Now);

                ServiceArquivo.AnexaArquivo(metaUnidade, pathFile, "Meta_Canal_" + metaUnidade.Nome.Replace("/", ""));

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizado a criação do template de Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.MetaCanalManualGeradoSucesso,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            catch (Exception ex)
            {
                string mensagem = Error.Handler(ex);

                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroGerarMetaCanalManual,
                    MensagemdeProcessamento = string.Format("{0} - {1}{2}{3}", DateTime.Now, mensagem, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000)
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        private List<ViewModels.ModeloMetaDetalhadaClienteViewModel> ListarModeloDadosDetalhado(MetadaUnidade metaUnidade)
        {
            var lista = RepositoryService.MetadoCanal.ListarModeloMetaDetalhada(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaCategoriasAtivas = RepositoryService.CategoriasCanal.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, Domain.Enum.Conta.ParticipaDoPrograma.Sim, false);

            foreach (var item in listaCategoriasAtivas)
            {
                var updates = lista.FindAll(x => x.Canal.Id == item.ID.Value && x.ClassificacaoCanal.Id == item.Classificacao.Id);
                updates.ForEach(x => x.StatusCanal = Enum.Conta.ParticipaDoPrograma.Descredenciado);
            }

            var listaProdutoMes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            foreach (var item in listaProdutoMes)
            {
                var itemModelo = lista.Find(x => x.Canal.Id == item.Canal.Id && x.Produto.Id == item.Produto.Id);
                var mes = (Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes)item.Mes.Value;

                itemModelo.ListaProdutosMes[Helper.ConvertToInt(mes) - 1] = new ViewModels.ModeloMetaProdutoMesViewModel();
                itemModelo.ListaProdutosMes[Helper.ConvertToInt(mes) - 1].Mes = mes;
                itemModelo.ListaProdutosMes[Helper.ConvertToInt(mes) - 1].Quantidade = item.QtdePlanejada;
                itemModelo.ListaProdutosMes[Helper.ConvertToInt(mes) - 1].Valor = item.MetaPlanejada;
            }

            return lista;
        }

        private List<ViewModels.ModeloMetaResumidaClienteViewModel> ListarModeloDadosResumido(MetadaUnidade metaUnidade)
        {
            var lista = RepositoryService.MetadoCanal.ListarModeloMetaResumida(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaCategoriasAtivas = RepositoryService.CategoriasCanal.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, Domain.Enum.Conta.ParticipaDoPrograma.Sim, false);

            foreach (var item in listaCategoriasAtivas)
            {
                var updates = lista.FindAll(x => x.Canal.Id == item.ID.Value && x.ClassificacaoCanal.Id == item.Classificacao.Id);
                updates.ForEach(x => x.StatusCanal = Enum.Conta.ParticipaDoPrograma.Descredenciado);
            }

            var listaProdutoMes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            foreach (var item in listaProdutoMes)
            {
                var itemModelo = lista.Find(x => x.Canal.Id == item.Canal.Id);
                var mes = (Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes)item.Mes.Value;

                itemModelo.ListaProdutosMes[Helper.ConvertToInt(mes) - 1] = new ViewModels.ModeloMetaProdutoMesViewModel()
                {
                    Mes = mes,
                    Valor = item.MetaPlanejada
                };


            }

            return lista;
        }

        private List<MetadoCanal> CriarListarMetaCanal(MetadaUnidade metaUnidade, List<Conta> listaCanal)
        {
            var listaExistentes = RepositoryService.MetadoCanal.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            listaExistentes.ForEach(x => listaCanal.RemoveAll(y => y.ID.Value == x.Canal.Id));

            foreach (var canal in listaCanal)
            {
                var metaCanal = new MetadoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                metaCanal.Ano = metaUnidade.Ano;
                metaCanal.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                metaCanal.Nome = (canal.NomeAbreviado + " - " + metaUnidade.UnidadedeNegocio.Nome).Truncate(100);
                metaCanal.Canal = new Lookup(canal.ID.Value, canal.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName(canal));
                metaCanal.MetaPlanejada = 0m;
                metaCanal.MetaRealizada = 0m;

                metaCanal.ID = RepositoryService.MetadoCanal.Create(metaCanal);
                listaExistentes.Add(metaCanal);
            }

            return listaExistentes;
        }

        private List<string> CriarMetaCanalTrimestre(MetadaUnidade metaUnidade, List<MetadoCanal> listaMetaCanal, int quantidadePorLote)
        {
            var listaExistentes = RepositoryService.MetadoCanalporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            listaExistentes.ForEach(x => listaMetaCanal.RemoveAll(y => y.Canal.Id == x.Canal.Id));

            var listaNovos = new List<MetadoCanalporTrimestre>();

            foreach (var item in listaMetaCanal)
            {
                foreach (int trimestre in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
                {
                    listaNovos.Add(new MetadoCanalporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                    {
                        UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                        Ano = metaUnidade.Ano,
                        Canal = item.Canal,
                        MetaRealizada = 0,
                        MetaPlanejada = 0,
                        Trimestre = trimestre,
                        MetaCanal = new Lookup(item.ID.Value, item.Nome, SDKore.Crm.Util.Utility.GetEntityName(item)),
                        Nome = metaUnidade.Ano.Value + " - " + metaUnidade.UnidadedeNegocios.Name
                    });
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetadoCanalporTrimestre.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalSegmento(MetadaUnidade metaUnidade, List<Product> listaProdutos, int quantidadePorLote)
        {
            var listaNovos = new List<MetadoCanalporSegmento>();

            foreach (int trimestre in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var listaExistentes = RepositoryService.MetadoCanalporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre);
                var listaMetaCanalTrimestre = RepositoryService.MetadoCanalporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre);
                var listaSegmento = from p in listaProdutos
                                    group p by new { id = p.Segmento.Id, name = p.Segmento.Name, type = p.Segmento.Type } into g
                                    select new Lookup(g.Key.id, g.Key.name, g.Key.type);


                foreach (var metaTrimestre in listaMetaCanalTrimestre)
                {
                    foreach (var segmento in listaSegmento)
                    {
                        bool existe = listaExistentes.Exists(x => x.MetadoTrimestreCanal.Id == metaTrimestre.ID.Value && x.Segmento.Id == segmento.Id);

                        if (!existe)
                            listaNovos.Add(new MetadoCanalporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            {
                                UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                Ano = metaUnidade.Ano,
                                Segmento = segmento,
                                MetaRealizada = 0,
                                MetaPlanejada = 0,
                                Trimestre = trimestre,
                                MetadoTrimestreCanal = new Lookup(metaTrimestre.ID.Value, metaTrimestre.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaTrimestre)),
                                Canal = metaTrimestre.Canal,
                                Nome = segmento.Name
                            });
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetadoCanalporSegmento.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalFamilia(MetadaUnidade metaUnidade, List<Product> listaProdutos, int quantidadePorLote)
        {
            var listaNovos = new List<MetadoCanalporFamilia>();

            foreach (int trimestre in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var listaExistentes = RepositoryService.MetadoCanalporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre);
                var listaMetaCanalSegmento = RepositoryService.MetadoCanalporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre);


                foreach (var metaSegmento in listaMetaCanalSegmento)
                {
                    var listaFamilia = from p in listaProdutos
                                       where p.Segmento.Id == metaSegmento.Segmento.Id
                                       group p by new { id = p.FamiliaProduto.Id, name = p.FamiliaProduto.Name, type = p.FamiliaProduto.Type } into g
                                       select new Lookup(g.Key.id, g.Key.name, g.Key.type);


                    foreach (var familia in listaFamilia)
                    {
                        bool existe = listaExistentes.Exists(x => x.MetadoCanalporSegmento.Id == metaSegmento.ID.Value && x.Familia.Id == familia.Id);

                        if (!existe)
                            listaNovos.Add(new MetadoCanalporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            {
                                UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                Ano = metaUnidade.Ano,
                                Segmento = metaSegmento.Segmento,
                                Familia = familia,
                                MetaRealizada = 0,
                                MetaPlanejada = 0,
                                Trimestre = trimestre,
                                MetadoCanalporSegmento = new Lookup(metaSegmento.ID.Value, metaSegmento.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaSegmento)),
                                Canal = metaSegmento.Canal,
                                Nome = familia.Name
                            });
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetadoCanalporFamilia.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalSubfamilia(MetadaUnidade metaUnidade, List<Product> listaProdutos, int quantidadePorLote)
        {
            var listaNovos = new List<MetadoCanalporSubFamilia>();
            var listaFamilia = from p in listaProdutos
                               group p by new { id = p.FamiliaProduto.Id, name = p.FamiliaProduto.Name, type = p.FamiliaProduto.Type } into g
                               select new Lookup(g.Key.id, g.Key.name, g.Key.type);

            foreach (var familia in listaFamilia)
            {
                foreach (int trimestre in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
                {
                    var listaExistentes = RepositoryService.MetadoCanalporSubFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre, familia.Id);
                    var listaMetaCanalFamilia = RepositoryService.MetadoCanalporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre, familia.Id);

                    foreach (var metaFamilia in listaMetaCanalFamilia)
                    {
                        var listaSubfamilia = from p in listaProdutos
                                              where p.FamiliaProduto.Id == metaFamilia.Familia.Id
                                              group p by new { id = p.SubfamiliaProduto.Id, name = p.SubfamiliaProduto.Name, type = p.SubfamiliaProduto.Type } into g
                                              select new Lookup(g.Key.id, g.Key.name, g.Key.type);


                        foreach (var subfamilia in listaSubfamilia)
                        {
                            bool existe = listaExistentes.Exists(x => x.MetadoCanalporFamilia.Id == metaFamilia.ID.Value && x.Familia.Id == familia.Id);

                            if (!existe)
                                listaNovos.Add(new MetadoCanalporSubFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                {
                                    UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                    Ano = metaUnidade.Ano,
                                    Familia = familia,
                                    Segmento = metaFamilia.Segmento,
                                    SubFamilia = subfamilia,
                                    MetaRealizada = 0,
                                    MetaPlanejada = 0,
                                    Trimestre = trimestre,
                                    MetadoCanalporFamilia = new Lookup(metaFamilia.ID.Value, metaFamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaFamilia)),
                                    Canal = metaFamilia.Canal,
                                    Nome = subfamilia.Name
                                });
                        }
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetadoCanalporSubFamilia.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalProduto(MetadaUnidade metaUnidade, List<Product> listaProdutos, int quantidadePorLote)
        {
            var listaNovos = new List<MetadoCanalporProduto>();
            var listaSubfamilia = from p in listaProdutos
                                  group p by new { id = p.SubfamiliaProduto.Id, name = p.SubfamiliaProduto.Name, type = p.SubfamiliaProduto.Type } into g
                                  select new Lookup(g.Key.id, g.Key.name, g.Key.type);

            foreach (var subfamilia in listaSubfamilia)
            {
                foreach (int trimestre in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
                {
                    var listaExistentes = RepositoryService.MetadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre, subfamilia.Id);
                    var listaMetaCanalSubfamilia = RepositoryService.MetadoCanalporSubFamilia.ListarPorUnidadeNegocioSubfamilia(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, (Enum.OrcamentodaUnidade.Trimestres)trimestre, subfamilia.Id);

                    foreach (var mestaSubfamilia in listaMetaCanalSubfamilia)
                    {
                        var listaProdutosFiltrado = listaProdutos.Where(x => x.SubfamiliaProduto.Id == subfamilia.Id);


                        foreach (var produto in listaProdutosFiltrado)
                        {
                            bool existe = listaExistentes.Exists(x => x.MetadoCanalporSubFamilia.Id == mestaSubfamilia.ID.Value && x.Produto.Id == produto.ID.Value);

                            if (!existe)
                                listaNovos.Add(new MetadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                {
                                    UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                    Ano = metaUnidade.Ano,
                                    Produto = new Lookup(produto.ID.Value, produto.Nome, SDKore.Crm.Util.Utility.GetEntityName(produto)),
                                    MetaRealizada = 0,
                                    MetaPlanejada = 0,
                                    Trimestre = trimestre,
                                    MetadoCanalporSubFamilia = new Lookup(mestaSubfamilia.ID.Value, mestaSubfamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(mestaSubfamilia)),
                                    Canal = mestaSubfamilia.Canal,
                                    Nome = produto.Nome
                                });
                        }
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetadoCanalporProduto.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalProdutoMes(MetadaUnidade metaUnidade, List<Product> listaProdutos, int quantidadePorLote)
        {
            var listaNovos = new List<MetaDetalhadadoCanalporProduto>();

            foreach (var produto in listaProdutos)
            {
                foreach (int t in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
                {
                    var trimestre = (Enum.OrcamentodaUnidade.Trimestres)t;
                    var listaExistentes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimestre, produto.ID.Value);
                    var listaMetaCanalProduto = RepositoryService.MetadoCanalporProduto.ListarPorUnidadeNegocioProduto(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimestre, produto.ID.Value);

                    foreach (var metaCanalProduto in listaMetaCanalProduto)
                    {
                        foreach (var mes in Helper.ListarMeses(trimestre))
                        {
                            bool existe = listaExistentes.Exists(x => x.MetadoCanalporProduto.Id == metaCanalProduto.ID.Value && x.Mes.Value == (int)mes);

                            if (!existe)
                                listaNovos.Add(new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                {
                                    UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                    Ano = metaUnidade.Ano,
                                    Produto = new Lookup(produto.ID.Value, produto.Nome, SDKore.Crm.Util.Utility.GetEntityName(produto)),
                                    MetaRealizada = 0,
                                    MetaPlanejada = 0,
                                    QtdePlanejada = 0,
                                    QtdeRealizada = 0,
                                    Trimestre = t,
                                    Mes = (int)mes,
                                    MetadoCanalporProduto = new Lookup(metaCanalProduto.ID.Value, metaCanalProduto.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaCanalProduto)),
                                    Canal = metaCanalProduto.Canal,
                                    Nome = produto.Nome
                                });
                        }
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetaDetalhadadoCanalporProduto.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private List<string> CriarMetaCanalProdutoMesResumido(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaNovos = new List<MetaDetalhadadoCanalporProduto>();

            foreach (int t in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var trimestre = (Enum.OrcamentodaUnidade.Trimestres)t;
                var listaExistentes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimestre, null);
                var listaMetaCanalTrimestre = RepositoryService.MetadoCanalporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimestre);

                foreach (var metaCanalTrimestre in listaMetaCanalTrimestre)
                {
                    foreach (var mes in Helper.ListarMeses(trimestre))
                    {
                        bool existe = listaExistentes.Exists(x => x.MetadoCanalporTrimestre.Id == metaCanalTrimestre.ID.Value && x.Mes.Value == (int)mes);

                        if (!existe)
                            listaNovos.Add(new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            {
                                UnidadedeNegocio = metaUnidade.UnidadedeNegocios,
                                Ano = metaUnidade.Ano,
                                MetaRealizada = 0,
                                MetaPlanejada = 0,
                                QtdePlanejada = 0,
                                QtdeRealizada = 0,
                                Trimestre = t,
                                Mes = (int)mes,
                                MetadoCanalporTrimestre = new Lookup(metaCanalTrimestre.ID.Value, metaCanalTrimestre.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaCanalTrimestre)),
                                Canal = metaCanalTrimestre.Canal,
                                Nome = mes.GetDescription()
                            });
                    }
                }
            }

            var listaError = new List<string>();
            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                DomainExecuteMultiple retorno = RepositoryService.MetaDetalhadadoCanalporProduto.Create(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add("Create - " + item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        #endregion

        #region Importar Meta

        public void ImportarMetaDetalhada(MetadaUnidade metaUnidade, string pathTemp)
        {
            string pathFile = string.Empty;
            int quantidadePorLote = 1000;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            var trace = new Trace("Meta-Canal-IMPORTAR-" + metaUnidade.ID.Value);
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            try
            {
                trace.Add("{0} - Alterando status da meta no CRM!", DateTime.Now);

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando Importação do Canal{1}{2}{3}", DateTime.Now, Environment.NewLine, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ImportandoPlanilhaMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                trace.Add("{0} - Fazendo download do arquivo", DateTime.Now);

                Observacao mObservaocao = RepositoryService.Observacao.Obter("itbc_metas", "objectid", "itbc_metasid", "itbc_metasid", metaUnidade.ID.Value);
                pathFile = ServiceArquivo.DownLoadArquivo(string.Concat(pathTemp, @"\"), mObservaocao.NomeArquivo, mObservaocao.Body, "");

                trace.Add("{0} - Convertando dados do Excel", DateTime.Now);

                List<string> listaErrosPlanilha;
                var itens = ServiceArquivo.ConvertDadosMetaDetalhadaCanal(pathFile, out listaErrosPlanilha);

                if (listaErrosPlanilha.Count > 0)
                {
                    string file = pathTemp + "Log Error Metas.txt";
                    string mensagem = string.Join(Environment.NewLine, listaErrosPlanilha.ToArray());
                    ServiceArquivo.CriarArquivoLog(metaUnidade, mensagem, file);

                    throw new ArgumentException("(CRM) Erro ao converter a planilha importada, consulte o arquivo de log nos anexos.");
                }

                trace.Add("{0} - Atualizando no CRM meta por produto mês", DateTime.Now);
                ImportarListarMetaDetalhadaProduto(metaUnidade, itens);

                trace.Add("{0} - Atualizando estrutura com valores de planejado", DateTime.Now);
                string mensagemErro = AtualizarValoresTodaEstruturaDetalhada(metaUnidade, quantidadePorLote);

                if (mensagemErro.Length == 0)
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizando Importação do Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.PlanilhaMetaCanalManualImportadaSucesso,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });
                }
                else
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação do Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroImportarMetaCanalManual,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });

                    trace.Add("{0} - Criando arquivo de log", DateTime.Now);

                    string file = pathTemp + "Log Error Metas.txt";
                    ServiceArquivo.CriarArquivoLog(metaUnidade, mensagemErro.ToString(), file);
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
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroImportarMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        public void ImportarMetaResumida(MetadaUnidade metaUnidade, string pathTemp)
        {
            string pathFile = string.Empty;
            int quantidadePorLote = 1000;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            var trace = new Trace("Meta-Canal-IMPORTAR-" + metaUnidade.ID.Value);
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            try
            {
                trace.Add("{0} - Alterando status da meta no CRM!", DateTime.Now);

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando Importação do Canal{1}{2}{3}", DateTime.Now, Environment.NewLine, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ImportandoPlanilhaMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                trace.Add("{0} - Fazendo download do arquivo", DateTime.Now);

                Observacao mObservaocao = RepositoryService.Observacao.Obter("itbc_metas", "objectid", "itbc_metasid", "itbc_metasid", metaUnidade.ID.Value);
                pathFile = ServiceArquivo.DownLoadArquivo(string.Concat(pathTemp, @"\"), mObservaocao.NomeArquivo, mObservaocao.Body, "");

                trace.Add("{0} - Convertando dados do Excel", DateTime.Now);

                List<string> listaErrosPlanilha;
                var itens = ServiceArquivo.ConvertDadosMetaResumidaCanal(pathFile, out listaErrosPlanilha);

                if (listaErrosPlanilha.Count > 0)
                {
                    string file = pathTemp + "Log Error Metas.txt";
                    string mensagem = string.Join(Environment.NewLine, listaErrosPlanilha.ToArray());
                    ServiceArquivo.CriarArquivoLog(metaUnidade, mensagem, file);

                    throw new ArgumentException("(CRM) Erro ao converter a planilha importada, consulte o arquivo de log nos anexos.");
                }

                trace.Add("{0} - Atualizando no CRM meta por produto mês", DateTime.Now);
                ImportarListarMetaResumidaProduto(metaUnidade, itens);

                trace.Add("{0} - Atualizando estrutura com valores de planejado", DateTime.Now);
                string mensagemErro = AtualizarValoresTodaEstruturaResumida(metaUnidade, quantidadePorLote);

                if (mensagemErro.Length == 0)
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizando Importação do Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.PlanilhaMetaCanalManualImportadaSucesso,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });
                }
                else
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação do Canal{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroImportarMetaCanalManual,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });

                    trace.Add("{0} - Criando arquivo de log", DateTime.Now);

                    string file = pathTemp + "Log Error Metas.txt";
                    ServiceArquivo.CriarArquivoLog(metaUnidade, mensagemErro.ToString(), file);
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
                    RazaodoStatusMetaManual = (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ErroImportarMetaCanalManual,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            finally
            {
                trace.SaveClear();
                ServiceArquivo.ExcluirArquivo(pathFile);
            }
        }

        private void ImportarListarMetaDetalhadaProduto(MetadaUnidade metaUnidade, List<ViewModels.ModeloMetaDetalhadaClienteViewModel> listaModelo)
        {
            var listaPotencialKAProdutoMes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            foreach (var modeloProduto in listaModelo)
            {
                foreach (var modeloMes in modeloProduto.ListaProdutosMes)
                {
                    var item = listaPotencialKAProdutoMes.Find(x => x.Produto.Id == modeloProduto.Produto.Id
                                                                 && x.Canal.Id == modeloProduto.Canal.Id
                                                                 && x.Mes.Value == (int)modeloMes.Mes);

                    if (item == null)
                    {
                        throw new ArgumentException(string.Format("(CRM) Não foi encontrado o produto mês para Mês [{0}] KA [{1}] Produto [{2}]", modeloMes.Mes, modeloProduto.Canal.Id, modeloProduto.Produto.Id));
                    }

                    if (modeloMes.Quantidade.Value != item.QtdePlanejada || item.MetaPlanejada.Value != modeloMes.Valor.Value)
                    {
                        RepositoryService.MetaDetalhadadoCanalporProduto.Update(new MetaDetalhadadoCanalporProduto(item.OrganizationName, item.IsOffline)
                        {
                            ID = item.ID,
                            MetaPlanejada = modeloMes.Valor.Value,
                            QtdePlanejada = modeloMes.Quantidade.Value,
                            RazaoStatus = (int)Domain.Enum.MetadoCanalporProdutoMes.StatusCode.Ativa
                        });
                    }
                }
            }
        }

        private void ImportarListarMetaResumidaProduto(MetadaUnidade metaUnidade, List<ViewModels.ModeloMetaResumidaClienteViewModel> listaModelo)
        {
            var listaExistente = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            foreach (var modeloProduto in listaModelo)
            {
                foreach (var modeloMes in modeloProduto.ListaProdutosMes)
                {
                    var item = listaExistente.Find(x => x.Canal.Id == modeloProduto.Canal.Id
                                                     && x.Mes.Value == (int)modeloMes.Mes);

                    if (item == null)
                    {
                        throw new ArgumentException(string.Format("(CRM) Não foi encontrado o produto mês para Mês [{0}] KA [{1}]", modeloMes.Mes, modeloProduto.Canal.Id));
                    }

                    if ((modeloMes.Quantidade.HasValue && modeloMes.Quantidade.Value != item.QtdePlanejada) || item.MetaPlanejada.Value != modeloMes.Valor.Value)
                    {
                        RepositoryService.MetaDetalhadadoCanalporProduto.Update(new MetaDetalhadadoCanalporProduto(item.OrganizationName, item.IsOffline)
                        {
                            ID = item.ID,
                            MetaPlanejada = modeloMes.Valor,
                            QtdePlanejada = modeloMes.Quantidade,
                            RazaoStatus = (int)Domain.Enum.MetadoCanalporProdutoMes.StatusCode.Ativa
                        });
                    }
                }
            }
        }

        public string AtualizarValoresTodaEstruturaDetalhada(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaErros = new Dictionary<Guid, string>();
            var mensagemErro = new StringBuilder();

            // Produto
            listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Produto!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Subfamilia
            listaErros = AtualizarValoresPotencialSubfamilia(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Subfamilia!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Familia
            listaErros = AtualizarValoresPotencialFamilia(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Familia!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Segmento
            listaErros = AtualizarValoresPotencialSegmento(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Segmento!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Trimestre
            listaErros = AtualizarValoresPotencialTrimestre(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Trimeste!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Canal
            listaErros = AtualizarValoresPotencialCanal(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por canal!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            return mensagemErro.ToString();
        }

        public string AtualizarValoresTodaEstruturaResumida(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaErros = new Dictionary<Guid, string>();
            var mensagemErro = new StringBuilder();

            // Trimestre
            listaErros = AtualizarValoresPotencialTrimestreResumido(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por Trimeste!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            // Canal
            listaErros = AtualizarValoresPotencialCanal(metaUnidade, quantidadePorLote);

            if (listaErros.Count > 0)
            {
                mensagemErro.AppendLine("Atualização da meta por canal!");
                foreach (var item in listaErros)
                {
                    mensagemErro.AppendLine(item.Key + " - " + item.Value);
                }
            }

            return mensagemErro.ToString();
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();
            var listaNovos = new List<MetadoCanalporProduto>();
            var listaTodosCanalMeta = RepositoryService.Conta.ListarMetaDoCanalPor(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "accountid");

            foreach (var canal in listaTodosCanalMeta)
            {
                listaNovos.AddRange(RepositoryService.MetadoCanalporProduto.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, canal.ID.Value));
            }

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporProduto.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporProduto.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialSubfamilia(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();
            var listaNovos = new List<MetadoCanalporSubFamilia>();
            var listaTodosCanalMeta = RepositoryService.Conta.ListarMetaDoCanalPor(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "accountid");

            foreach (var canal in listaTodosCanalMeta)
            {
                listaNovos.AddRange(RepositoryService.MetadoCanalporSubFamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, canal.ID.Value));
            }

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporSubfamilia.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporSubFamilia.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialFamilia(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();
            var listaNovos = new List<MetadoCanalporFamilia>();
            var listaTodosCanalMeta = RepositoryService.Conta.ListarMetaDoCanalPor(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, "accountid");

            foreach (var canal in listaTodosCanalMeta)
            {
                listaNovos.AddRange(RepositoryService.MetadoCanalporFamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, canal.ID.Value));
            }

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporFamilia.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporFamilia.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

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
            var listaNovos = new List<MetadoCanalporSegmento>();

            foreach (int t in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var trimeste = (Enum.OrcamentodaUnidade.Trimestres)t;
                listaNovos.AddRange(RepositoryService.MetadoCanalporSegmento.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste));
            }

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporSegmento.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporSegmento.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

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

            var listaNovos = RepositoryService.MetadoCanalporTrimestre.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporTrimestre.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporTrimestre.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialTrimestreResumido(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var listaNovos = RepositoryService.MetadoCanalporTrimestre.ListarValoresPorUnidadeNegocioResunmida(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanalporTrimestre.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanalporTrimestre.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialCanal(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            var listaNovos = RepositoryService.MetadoCanal.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            listaNovos.ForEach(x => x.RazaoStatus = (int)Domain.Enum.MetadoCanal.StatusCode.Ativa);

            for (int i = 0; i < listaNovos.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadoCanal.Update(listaNovos.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            return listaError;
        }

        #endregion

        #region AtualizarRealizado

        public void AtualizarRealizado(int ano, Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            foreach (var meta in lstMetadaUnidade)
            {
                string chaveIntegracaoUnidadeNegocio = RepositoryService.UnidadeNegocio.Retrieve(meta.UnidadedeNegocios.Id, "itbc_chave_integracao").ChaveIntegracao;

                DataTable dtMetaCanalProdDetalhado = RepositoryService.MetaDetalhadadoCanalporProduto.ListarDadosDWPor(ano, (int)trimestre, chaveIntegracaoUnidadeNegocio);
                var listaClientes = RepositoryService.Conta.ListarContasMetaCanal(meta.UnidadedeNegocios.Id, ano);
                var listaExistentes = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorUnidadeNegocio(meta.UnidadedeNegocios.Id, meta.Ano.Value, trimestre, null);

                foreach (DataRow item in dtMetaCanalProdDetalhado.Rows)
                {
                    string codigoCanal = Convert.ToString(item.Field<Int32>("CD_Emitente"));
                    var mes = item.Field<int>("cd_mes");
                    var canal = listaClientes.Find(x => x.CodigoMatriz == codigoCanal);

                    if (canal == null) continue;

                    var metaProduto = listaExistentes.Find(x => x.Canal.Id == canal.ID.Value && x.Mes.Value == mes);

                    if (metaProduto != null)
                    {
                        RepositoryService.MetaDetalhadadoCanalporProduto.Update(new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = metaProduto.ID,
                            MetaRealizada = item.Field<decimal>("vlr"),
                            QtdeRealizada = item.Field<decimal>("qtde")
                        });
                    }
                }

                AtualizarValoresTodaEstruturaResumida(meta, 1000);
            }
        }

        #endregion
    }
}