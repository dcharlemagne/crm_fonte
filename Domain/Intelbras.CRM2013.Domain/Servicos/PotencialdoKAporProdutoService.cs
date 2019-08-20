using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoKAporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoKAporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys

        MetaDetalhadadoKAporProdutoService _ServiceMetaDetalhadadoKAporProduto = null;
        MetaDetalhadadoKAporProdutoService ServiceMetaDetalhadadoKAporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadoKAporProduto == null)
                    _ServiceMetaDetalhadadoKAporProduto = new MetaDetalhadadoKAporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetaDetalhadadoKAporProduto;
            }
        }
        #endregion

        #region Método
        public void Criar(PotencialdoKAporSubfamilia mPotencialdoKAporSubfamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid representanteId, int numTrimestre)
        {
            PotencialdoKAporProduto mPotencialdoKAporProduto = null;

            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                
                mPotencialdoKAporSubfamilia = RepositoryService.PotencialdoKAporSubfamilia.Retrieve(mPotencialdoKAporSubfamilia.ID.Value);
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    var tempProduto = RepositoryService.PotencialdoKAporProduto.Obter(representanteId, OrcaProduto.First().Produto.Id, OrcaProduto.First().SubFamilia.Id, mPotencialdoKAporSubfamilia.ID.Value, (int)mPotencialdoKAporSubfamilia.Trimestre);

                    if (tempProduto == null)
                    {
                        mPotencialdoKAporProduto = new PotencialdoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mPotencialdoKAporProduto.ID = Guid.NewGuid();
                        mPotencialdoKAporProduto.Ano = mPotencialdoKAporSubfamilia.Ano;
                        mPotencialdoKAporProduto.Trimestre = numTrimestre;
                        mPotencialdoKAporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialdoKAporProduto.PotencialdoKAporSubfamilia = new Lookup(mPotencialdoKAporSubfamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporSubfamilia>());
                        mPotencialdoKAporProduto.Nome = OrcaProduto.First().Produto.Name;
                        mPotencialdoKAporProduto.KAouRepresentante = new Lookup(representanteId, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());

                        RepositoryService.PotencialdoKAporProduto.Create(mPotencialdoKAporProduto);
                        ServiceMetaDetalhadadoKAporProduto.Criar(mPotencialdoKAporProduto);
                    }
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mMetaDetalhado)
        {
            decimal valor = 0;
            int quantidade = 0;
            PotencialdoKAporProduto mPotencialdoKAporProduto1;
            PotencialdoKAporProduto mPotencialdoKAporProduto2;
            PotencialdoKAporProduto mPotencialdoKAporProduto3;
            PotencialdoKAporProduto mPotencialdoKAporProduto4;

            if (mMetaDetalhado.AtualizarTrimestre1)
            {

                mPotencialdoKAporProduto1 = RepositoryService.PotencialdoKAporProduto.ObterPor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1);
                if (mPotencialdoKAporProduto1 != null)
                {
                    mMetaDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceMetaDetalhadadoKAporProduto.Calcular(mPotencialdoKAporProduto1, mMetaDetalhado.Trimestre1, ref valor, ref quantidade);
                    mPotencialdoKAporProduto1.PotencialPlanejado = valor;
                    mPotencialdoKAporProduto1.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoKAporProduto.Update(mPotencialdoKAporProduto1);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre2)
            {
                mPotencialdoKAporProduto2 = RepositoryService.PotencialdoKAporProduto.ObterPor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2);
                if (mPotencialdoKAporProduto2 != null)
                {
                    mMetaDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    ServiceMetaDetalhadadoKAporProduto.Calcular(mPotencialdoKAporProduto2, mMetaDetalhado.Trimestre2, ref valor, ref quantidade);
                    mPotencialdoKAporProduto2.PotencialPlanejado = valor;
                    mPotencialdoKAporProduto2.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoKAporProduto.Update(mPotencialdoKAporProduto2);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre3)
            {
                mPotencialdoKAporProduto3 = RepositoryService.PotencialdoKAporProduto.ObterPor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3);
                if (mPotencialdoKAporProduto3 != null)
                {
                    mMetaDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    ServiceMetaDetalhadadoKAporProduto.Calcular(mPotencialdoKAporProduto3, mMetaDetalhado.Trimestre3, ref valor, ref quantidade);
                    mPotencialdoKAporProduto3.PotencialPlanejado = valor;
                    mPotencialdoKAporProduto3.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoKAporProduto.Update(mPotencialdoKAporProduto3);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre4)
            {

                mPotencialdoKAporProduto4 = RepositoryService.PotencialdoKAporProduto.ObterPor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4);
                if (mPotencialdoKAporProduto4 != null)
                {
                    mMetaDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    ServiceMetaDetalhadadoKAporProduto.Calcular(mPotencialdoKAporProduto4, mMetaDetalhado.Trimestre4, ref valor, ref quantidade);
                    mPotencialdoKAporProduto4.PotencialPlanejado = valor;
                    mPotencialdoKAporProduto4.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoKAporProduto.Update(mPotencialdoKAporProduto4);
                }
            }
        }

        public void RetornoDWKaProduto(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporProduto.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Contato mContato = RepositoryService.Contato.ObterPor(item["CD_representante"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mContato != null && mProduto != null)
                {
                    PotencialdoKAporProduto mPotencialdoKAporProduto = RepositoryService.PotencialdoKAporProduto.Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), trimestre, mProduto.ID.Value);
                    if (mPotencialdoKAporProduto != null)
                    {
                        mPotencialdoKAporProduto.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        mPotencialdoKAporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                        RepositoryService.PotencialdoKAporProduto.Update(mPotencialdoKAporProduto);
                    }
                }
            }

        }

        #endregion


        //// ***************************************************

        //ProdutoService _ServiceProduto = null;
        //private ProdutoService ServiceProduto
        //{
        //    get
        //    {
        //        if (_ServiceProduto == null)
        //            _ServiceProduto = new ProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

        //        return _ServiceProduto;
        //    }
        //}

        //MetadaUnidadeporTrimestreService _ServiceMetadaUnidadeporTrimestre = null;
        //private MetadaUnidadeporTrimestreService ServiceMetadaUnidadeporTrimestre
        //{
        //    get
        //    {
        //        if (_ServiceMetadaUnidadeporTrimestre == null)
        //            _ServiceMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestreService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

        //        return _ServiceMetadaUnidadeporTrimestre;
        //    }
        //}
        //ArquivoService _ServiceArquivo = null;
        //private ArquivoService ServiceArquivo
        //{
        //    get
        //    {
        //        if (_ServiceArquivo == null)
        //            _ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

        //        return _ServiceArquivo;
        //    }
        //}
        //public void _GerarPlanilha(Guid metaPotencialKAId)
        //{
        //    #region Variaveis

        //    string sArquivo = string.Empty;
        //    Guid anexoId = Guid.Empty;


        //    string labelTipo = string.Empty;
        //    string[] parametrosgrobais;
        //    GrupoEstoque mGrupoEstoque;
        //    List<Guid> lstGrupoEstoque = new List<Guid>();
        //    List<OrcamentoDetalhado> lstOrcamentoDetalhado;
        //    ParametroGlobal mParametroGlobal;
        //    Observacao mObservaocao;
        //    OrcamentodaUnidade mOrcamentodaUnidade;
        //    MetadaUnidade mMetadaUnidade;
        //    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre1;
        //    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre2;
        //    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre3;
        //    MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre4;
        //    Guid tri1 = Guid.Empty;
        //    Guid tri2 = Guid.Empty;
        //    Guid tri3 = Guid.Empty;
        //    Guid tri4 = Guid.Empty;
        //    //para o excel
        //    object _missingValue = System.Reflection.Missing.Value;
        //    Microsoft.Office.Interop.Excel.Application excel = null;
        //    Microsoft.Office.Interop.Excel.Workbook workbook = null;
        //    Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
        //    Microsoft.Office.Interop.Excel.Range Linha;
        //    Microsoft.Office.Interop.Excel.Range trimestre;

        //    #endregion

        //    try
        //    {
        //        excel = new Microsoft.Office.Interop.Excel.Application();

        //        mMetadaUnidade = RepositoryService.MetasUnidade.Retrieve(metaPotencialKAId);

        //        #region get ParametroGlobal e search all grupoestoque to filter
        //        mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta);
        //        if (mParametroGlobal != null)
        //        {
        //            if (!string.IsNullOrEmpty(mParametroGlobal.Valor))
        //            {
        //                parametrosgrobais = mParametroGlobal.Valor.Split(';');
        //                foreach (string parametro in parametrosgrobais)
        //                {
        //                    mGrupoEstoque = RepositoryService.GrupoEstoque.ObterPor(Convert.ToInt32(parametro));
        //                    if (mGrupoEstoque != null)
        //                        lstGrupoEstoque.Add(mGrupoEstoque.ID.Value);
        //                }
        //            }
        //        }
        //        #endregion

        //        #region find and Download a file with the NivelOrcamento
        //        if (mMetadaUnidade.NiveldaMeta.Value == (int)Domain.Enum.MetaUnidade.NivelMeta.Detalhado)
        //        {
        //            labelTipo = "Detalhado";
        //            mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(40);
        //        }
        //        else
        //        {
        //            labelTipo = "Resumido";
        //            mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(45);
        //        }

        //        if (mParametroGlobal == null)
        //            throw new Exception("Não foi Encontrado Parametro Global para este nivel de Orçamento.");

        //        mObservaocao = RepositoryService.Observacao.ObterPorParametrosGlobais(mParametroGlobal.ID.Value);
        //        if (mObservaocao == null)
        //            throw new Exception("Não foi Encontrado a Planilha em Parametro Global, para este nivel de Orçamento.");

        //        sArquivo = ServiceArquivo.DownLoadArquivo(SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"), "Meta_" + mMetadaUnidade.Nome.Replace("/", ""), mObservaocao.Body, ".xlsx");
        //        //sArquivo = ServiceArquivo.DownLoadArquivo(string.Concat(Environment.CurrentDirectory, @"\"), "Meta_" + mMetadaUnidade.Nome, mObservaocao.Body, ".xlsx");
        //        #endregion

        //        try
        //        {
        //            if (mMetadaUnidade.NiveldaMeta.Value == (int)Domain.Enum.MetaUnidade.NivelMeta.Detalhado)
        //                lstOrcamentoDetalhado = _obterProdutosDetalhado(mMetadaUnidade, lstGrupoEstoque);
        //            else
        //                lstOrcamentoDetalhado = _obterProdutosResumidos(mMetadaUnidade, lstGrupoEstoque);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Erro ao listar produtos, entre em contato com o Administrador do sistema.", ex);
        //        }

        //        try
        //        {
        //            workbook = excel.Workbooks.Open(sArquivo, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue);
        //            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);
        //            worksheet.Name = "Plan1";
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Ocorreu um erro ao abrir o \n Excel:" + ex.Message, ex);
        //        }

        //        Linha = (worksheet.Cells[1, 1] as Microsoft.Office.Interop.Excel.Range);

        //        #region Cabeçalho Unidade/ Ano/ Detalhamento
        //        Linha = (worksheet.Cells[1, 5] as Microsoft.Office.Interop.Excel.Range);
        //        Linha.Value2 = mMetadaUnidade.UnidadedeNegocios.Name;

        //        Linha = (worksheet.Cells[2, 4] as Microsoft.Office.Interop.Excel.Range);
        //        Linha.Value2 = mMetadaUnidade.Ano;

        //        Linha = (worksheet.Cells[3, 4] as Microsoft.Office.Interop.Excel.Range);
        //        Linha.Value2 = labelTipo;
        //        #endregion
        //        try
        //        {
        //            #region Monta os Guids dos Orçamentos Trimetrais
        //            trimestre = (worksheet.Cells[4, 18] as Microsoft.Office.Interop.Excel.Range);
        //            mMetadaUnidadeporTrimestre1 = RepositoryService.MetadaUnidadeporTrimestre.Obter(mMetadaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1);
        //            trimestre.Value2 = mMetadaUnidadeporTrimestre1 != null ? mMetadaUnidadeporTrimestre1.ID.ToString() : Guid.NewGuid().ToString();
        //            tri1 = Guid.Parse(trimestre.Value2);

        //            trimestre = (worksheet.Cells[4, 30] as Microsoft.Office.Interop.Excel.Range);
        //            mMetadaUnidadeporTrimestre2 = RepositoryService.MetadaUnidadeporTrimestre.Obter(mMetadaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2);
        //            trimestre.Value2 = mMetadaUnidadeporTrimestre2 != null ? mMetadaUnidadeporTrimestre2.ID.ToString() : Guid.NewGuid().ToString();
        //            tri2 = Guid.Parse(trimestre.Value2);

        //            trimestre = (worksheet.Cells[4, 42] as Microsoft.Office.Interop.Excel.Range);
        //            mMetadaUnidadeporTrimestre3 = RepositoryService.MetadaUnidadeporTrimestre.Obter(mMetadaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3);
        //            trimestre.Value2 = mMetadaUnidadeporTrimestre3 != null ? mMetadaUnidadeporTrimestre3.ID.ToString() : Guid.NewGuid().ToString();
        //            tri3 = Guid.Parse(trimestre.Value2);

        //            trimestre = (worksheet.Cells[4, 54] as Microsoft.Office.Interop.Excel.Range);
        //            mMetadaUnidadeporTrimestre4 = RepositoryService.MetadaUnidadeporTrimestre.Obter(mMetadaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4);
        //            trimestre.Value2 = mMetadaUnidadeporTrimestre4 != null ? mMetadaUnidadeporTrimestre4.ID.ToString() : Guid.NewGuid().ToString();
        //            tri4 = Guid.Parse(trimestre.Value2);
        //            #endregion
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new ApplicationException("Erro ao gerar o Id dos Trimestres, contate o administrador do sistema e verifique a versão da planilha", ex);
        //        }

        //        try
        //        {
        //            ServiceArquivo._GerarPlanilhaMetas(mMetadaUnidade, lstOrcamentoDetalhado, worksheet);
        //        }
        //        catch (Exception errocriarlinhaexcel)
        //        {
        //            throw new Exception("Ocorreu erro ao inserir linhas na Planilha, verifique versão da planilha ou informe o Adiministrador do sistema. \n " + errocriarlinhaexcel.Message);
        //        }

        //        workbook.Save();

        //        // comentado pois ta lockando arquivo
        //        #region Anexa Arquivo ao orçamento
        //        Lookup entidade = new Lookup(mMetadaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadaUnidade>());
        //        //ServiceArquivo.AnexaArquivo(sArquivo, "Template Meta Gerada com Sucesso", "Meta_" + mMetadaUnidade.Nome + ".xlsx", entidade, out anexoId);
        //        mMetadaUnidade.StatusCode = (int)Domain.Enum.MetaUnidade.StatusMetaUnidade.ModelodeMetaGeradocomSucesso;
        //        mMetadaUnidade.AddNullProperty("MensagemdeProcessamento");
        //        RepositoryService.MetasUnidade.Update(mMetadaUnidade);
        //        #endregion

        //        try
        //        {
        //            #region Criando os trimestres
        //            if (mMetadaUnidadeporTrimestre1 != null)
        //                ServiceMetadaUnidadeporTrimestre.Atualiza(mMetadaUnidadeporTrimestre1, mMetadaUnidade, lstOrcamentoDetalhado);
        //            else
        //                ServiceMetadaUnidadeporTrimestre.Criar(mMetadaUnidade, lstOrcamentoDetalhado, " - 1o Trimestre", (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1, tri1);

        //            if (mMetadaUnidadeporTrimestre2 != null)
        //                ServiceMetadaUnidadeporTrimestre.Atualiza(mMetadaUnidadeporTrimestre2, mMetadaUnidade, lstOrcamentoDetalhado);
        //            else
        //                ServiceMetadaUnidadeporTrimestre.Criar(mMetadaUnidade, lstOrcamentoDetalhado, " - 2o Trimestre", (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2, tri2);

        //            if (mMetadaUnidadeporTrimestre3 != null)
        //                ServiceMetadaUnidadeporTrimestre.Atualiza(mMetadaUnidadeporTrimestre3, mMetadaUnidade, lstOrcamentoDetalhado);
        //            else
        //                ServiceMetadaUnidadeporTrimestre.Criar(mMetadaUnidade, lstOrcamentoDetalhado, " - 3o Trimestre", (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3, tri3);

        //            if (mMetadaUnidadeporTrimestre4 != null)
        //                ServiceMetadaUnidadeporTrimestre.Atualiza(mMetadaUnidadeporTrimestre4, mMetadaUnidade, lstOrcamentoDetalhado);
        //            else
        //                ServiceMetadaUnidadeporTrimestre.Criar(mMetadaUnidade, lstOrcamentoDetalhado, " - 4o Trimestre", (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4, tri4);

        //            #endregion
        //        }
        //        catch (Exception erroCreateTrimestre)
        //        {
        //            if (anexoId != Guid.Empty)
        //            {
        //                RepositoryService.Anexo.Delete(anexoId);
        //                anexoId = Guid.Empty;
        //            }

        //            throw new Exception("Ocorreu um erro ao gerar os trimestre deste Orçamento, favor regerar. \n Caso o erro perssista contate o Administrador do Sistema. \n " + erroCreateTrimestre.Message);
        //        }
        //    }
        //    catch (Exception erro)
        //    {
        //        if (anexoId != Guid.Empty)
        //        {
        //            RepositoryService.Anexo.Delete(anexoId);
        //            anexoId = Guid.Empty;
        //        }

        //        throw erro;
        //    }
        //    finally
        //    {
        //        EncerrarExcel(ref excel, ref workbook, ref worksheet);
        //        ExcluirArquivo(sArquivo);
        //    }
        //}

        //public List<OrcamentoDetalhado> _obterProdutosResumidos(MetadaUnidade mMetadaUnidade, List<Guid> lstGrupoEstoque)
        //{
        //    #region variaveis e objetos
        //    List<MetaDetalhadadaUnidadeporProduto> LstMetaDetalhadadaUnidadeporProduto = null;
        //    OrcamentoDetalhado mOrcamentoDetalhado;
        //    List<OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();

        //    List<Product> lstProdutos = new List<Product>();
        //    List<Product> lstProdutosPage = new List<Product>();
        //    bool paginar = true;
        //    int page = 1;
        //    List<Guid> lstIdProdutos = new List<Guid>();
        //    //List<Guid> lstIdCanal = new List<Guid>();
        //    //List<Conta> lstContas = new List<Conta>();
        //    //List<Conta> lstContasPage = new List<Conta>();
        //    #endregion

        //    #region produtos
        //    while (paginar)
        //    {
        //        if (mMetadaUnidade.OrcamentodaUnidade != null)
        //            lstProdutosPage = RepositoryService.Produto.ListarProdutosOrcamentoUnidade(mMetadaUnidade.OrcamentodaUnidade.Id, page, 5000);
        //        else
        //            lstProdutosPage = RepositoryService.Produto.ListarProdutosMetasUnidade(mMetadaUnidade.ID.Value, page, 5000);

        //        if (lstProdutosPage.Count < 5000)
        //            paginar = false;

        //        lstProdutos.AddRange(lstProdutosPage.ToArray());
        //        page++;
        //    }
        //    #endregion

        //    #region Retira as duplicidades do produto

        //    var lstDuplicProd = (from p in lstProdutos
        //                         group p by string.Format("{0}", p.ID.Value));

        //    foreach (var iProd in lstDuplicProd)
        //        lstIdProdutos.Add(iProd.First().ID.Value);

        //    #endregion

        //    #region caso nao tenha orçamento buscar produtos e contas que foram criados pela unidade de negocio
        //    lstProdutos.AddRange(RepositoryService.Produto.ListarPorUnidadeNegocio(mMetadaUnidade.UnidadedeNegocios.Id, lstGrupoEstoque, lstIdProdutos).ToArray());

        //    ServiceProduto.ProdutoAcumulaOutroProduto(ref lstProdutos);
        //    #endregion

        //    #region relaciona produtos por canais
        //    var listaProd = (from p in lstProdutos
        //                     group p by string.Format("{0}/{1}/{2}", p.Segmento.Id, p.FamiliaProduto.Id, p.SubfamiliaProduto.Id));

        //    foreach (var itemProd in listaProd)
        //    {
        //        #region Gera contas/por produtos
        //        foreach (var prod in itemProd)
        //        {
        //            if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == prod.ID.Value && x.Canal.Id == Guid.Empty))
        //            {
        //                #region Gerando Colunas
        //                mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
        //                mOrcamentoDetalhado.Ano = mMetadaUnidade.Ano;
        //                mOrcamentoDetalhado.UnidadeNegocio = mMetadaUnidade.UnidadedeNegocios;
        //                mOrcamentoDetalhado.Segmento = prod.Segmento;
        //                mOrcamentoDetalhado.Familia = prod.FamiliaProduto;
        //                mOrcamentoDetalhado.SubFamilia = prod.SubfamiliaProduto;
        //                mOrcamentoDetalhado.Produto = new Lookup(prod.ID.Value, prod.Nome, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
        //                mOrcamentoDetalhado.StatusProduto = prod.Status.Value == 1 ? "Ativo" : "Descontinuado";
        //                mOrcamentoDetalhado.Canal = new Lookup(Guid.Empty, "Não Alocado", SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
        //                mOrcamentoDetalhado.StatusParticipacao = "Não";

        //                lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
        //                #endregion
        //            }
        //        }
        //        #endregion
        //    }
        //    #endregion

        //    #region obtem detalhamento dos produtos nos trimestres
        //    foreach (Model.OrcamentoDetalhado capa in lstOrcamentoDetalhado)
        //    {
        //        LstMetaDetalhadadaUnidadeporProduto = RepositoryService.MetadaUnidadeDetalhadaProduto.ObterDetalhadoProdutos(capa.Produto.Id, mMetadaUnidade.ID.Value);
        //        if (LstMetaDetalhadadaUnidadeporProduto != null && LstMetaDetalhadadaUnidadeporProduto.Count > 0)
        //        {
        //            #region
        //            capa.Trimestre1 = new Trimestre();
        //            capa.Trimestre2 = new Trimestre();
        //            capa.Trimestre3 = new Trimestre();
        //            capa.Trimestre4 = new Trimestre();

        //            foreach (MetaDetalhadadaUnidadeporProduto item in LstMetaDetalhadadaUnidadeporProduto)
        //            {
        //                if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
        //                    {
        //                        capa.Trimestre1.Mes1 = item.Mes;
        //                        capa.Trimestre1.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
        //                    {
        //                        capa.Trimestre1.Mes2 = item.Mes;
        //                        capa.Trimestre1.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
        //                    {
        //                        capa.Trimestre1.Mes3 = item.Mes;
        //                        capa.Trimestre1.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
        //                    {
        //                        capa.Trimestre2.Mes1 = item.Mes;
        //                        capa.Trimestre2.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
        //                    {
        //                        capa.Trimestre2.Mes2 = item.Mes;
        //                        capa.Trimestre2.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
        //                    {
        //                        capa.Trimestre2.Mes3 = item.Mes;
        //                        capa.Trimestre2.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
        //                    {
        //                        capa.Trimestre3.Mes1 = item.Mes;
        //                        capa.Trimestre3.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
        //                    {
        //                        capa.Trimestre3.Mes2 = item.Mes;
        //                        capa.Trimestre3.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
        //                    {
        //                        capa.Trimestre3.Mes3 = item.Mes;
        //                        capa.Trimestre3.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
        //                    {
        //                        capa.Trimestre4.Mes1 = item.Mes;
        //                        capa.Trimestre4.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
        //                    {
        //                        capa.Trimestre4.Mes2 = item.Mes;
        //                        capa.Trimestre4.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
        //                    {
        //                        capa.Trimestre4.Mes3 = item.Mes;
        //                        capa.Trimestre4.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion

        //    return lstOrcamentoDetalhado;
        //}

        //public List<OrcamentoDetalhado> _obterProdutosDetalhado(MetadaUnidade mMetadaUnidade, List<Guid> lstGrupoEstoque)
        //{
        //    #region variaveis e objetos
        //    //mantem
        //    List<MetaDetalhadadoCanalporProduto> LstMetaDetalhadadoCanalporProduto = null;
        //    OrcamentoDetalhado mOrcamentoDetalhado;
        //    List<OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();

        //    List<Product> lstProdutos = new List<Product>();
        //    List<Product> lstProdutosPage = new List<Product>();
        //    bool paginar = true;
        //    int page = 1;
        //    List<Guid> lstIdProdutos = new List<Guid>();
        //    List<Guid> lstIdCanal = new List<Guid>();
        //    List<Conta> lstContas = new List<Conta>();
        //    List<Conta> lstContasPage = new List<Conta>();
        //    #endregion

        //    #region pega os produtos e canais do orçamento, ou pela unidade de negocio
        //    #region produtos
        //    while (paginar)
        //    {
        //        //mantem ListarProdutosMetasCanal
        //        if (mMetadaUnidade.OrcamentodaUnidade != null)
        //            lstProdutosPage = RepositoryService.Produto.ListarProdutosOrcamentoCanal(mMetadaUnidade.OrcamentodaUnidade.Id, page, 5000);
        //        else
        //            lstProdutosPage = RepositoryService.Produto.ListarProdutosMetasCanal(mMetadaUnidade.ID.Value, page, 5000);

        //        if (lstProdutosPage.Count < 5000)
        //            paginar = false;

        //        lstProdutos.AddRange(lstProdutosPage.ToArray());
        //        page++;
        //    }
        //    #endregion

        //    page = 1;
        //    paginar = true;


        //    #region contas
        //    //verifica se mantem  por canais
        //    while (paginar)
        //    {
        //        if (mMetadaUnidade.OrcamentodaUnidade != null) // nao mantem
        //            lstContasPage = RepositoryService.Conta.ListarContasOrcamentoCanal(mMetadaUnidade.OrcamentodaUnidade.Id, page, 5000);
        //        else // mantem
        //            lstContasPage = RepositoryService.Conta.ListarContasMetaCanal(mMetadaUnidade.ID.Value);

        //        if (lstContasPage.Count < 5000)
        //            paginar = false;

        //        lstContas.AddRange(lstContasPage.ToArray());
        //        page++;
        //    }
        //    #endregion
        //    #endregion

        //    #region Retira as duplicidades do produto
        //    //mantem
        //    var lstDuplicProd = (from p in lstProdutos
        //                         group p by string.Format("{0}", p.ID.Value));

        //    foreach (var iProd in lstDuplicProd)
        //        lstIdProdutos.Add(iProd.First().ID.Value);

        //    #endregion

        //    #region Retira as duplicidades dos canais
        //    //mantem
        //    var lstDuplicCanal = (from c in lstContas
        //                          group c by string.Format("{0}", c.ID.Value));

        //    foreach (var iConta in lstDuplicCanal)
        //        lstIdCanal.Add(iConta.First().ID.Value);

        //    #endregion

        //    #region buscar produtos e contas que foram criados pela unidade de negocio
        //    //mantem
        //    lstProdutos.AddRange(RepositoryService.Produto.ListarPorUnidadeNegocio(mMetadaUnidade.UnidadedeNegocios.Id, lstGrupoEstoque, lstIdProdutos).ToArray());
        //    //mantem
        //    ServiceProduto.ProdutoAcumulaOutroProduto(ref lstProdutos);

        //    // não mantem
        //    lstContas.AddRange(RepositoryService.Conta.ListarContasParticipantes(mMetadaUnidade.UnidadedeNegocios.Id, lstIdCanal).ToArray());
        //    #endregion

        //    #region relaciona produtos por canais
        //    //nao mantem
        //    var listaProd = (from p in lstProdutos
        //                     group p by string.Format("{0}/{1}/{2}", p.Segmento.Id, p.FamiliaProduto.Id, p.SubfamiliaProduto.Id));

        //    //nao mantem
        //    var listaContas = (from c in lstContas
        //                       group c by string.Format("{0}", c.ID.Value));

        //    //nao mantem
        //    foreach (var itemProd in listaProd)
        //    {
        //        foreach (var itemConta in listaContas)
        //        {
        //            #region Gera contas/por produtos
        //            foreach (var prod in itemProd)
        //            {
        //                if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == prod.ID.Value && x.Canal.Id == itemConta.First().ID.Value))
        //                {
        //                    #region Gerando Colunas
        //                    mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
        //                    mOrcamentoDetalhado.Ano = mMetadaUnidade.Ano;
        //                    mOrcamentoDetalhado.UnidadeNegocio = mMetadaUnidade.UnidadedeNegocios;
        //                    mOrcamentoDetalhado.Segmento = prod.Segmento;
        //                    mOrcamentoDetalhado.Familia = prod.FamiliaProduto;
        //                    mOrcamentoDetalhado.SubFamilia = prod.SubfamiliaProduto;
        //                    mOrcamentoDetalhado.Produto = new Lookup(prod.ID.Value, prod.Nome, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
        //                    mOrcamentoDetalhado.StatusProduto = prod.Status.Value == 1 ? "Ativo" : "Descontinuado";
        //                    mOrcamentoDetalhado.Canal = new Lookup(itemConta.First().ID.Value, itemConta.First().RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
        //                    mOrcamentoDetalhado.StatusParticipacao = itemConta.First().ParticipantePrograma.HasValue ? (itemConta.First().ParticipantePrograma.Value == 993520001 ? "Sim" : (itemConta.First().ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "Não";

        //                    lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
        //                    #endregion
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion

        //    #region obtem detalhamento dos produtos nos trimestres
        //    //mantem
        //    foreach (Model.OrcamentoDetalhado capa in lstOrcamentoDetalhado)
        //    {
        //        // vai mudar metodo de obtencao de dados
        //        LstMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.ListarDetalheProdutosPorMeta(mMetadaUnidade.ID.Value, capa.Canal.Id, capa.Produto.Id);
        //        if (LstMetaDetalhadadoCanalporProduto != null && LstMetaDetalhadadoCanalporProduto.Count > 0)
        //        {
        //            #region
        //            capa.Trimestre1 = new Trimestre();
        //            capa.Trimestre2 = new Trimestre();
        //            capa.Trimestre3 = new Trimestre();
        //            capa.Trimestre4 = new Trimestre();

        //            foreach (MetaDetalhadadoCanalporProduto item in LstMetaDetalhadadoCanalporProduto)
        //            {
        //                if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
        //                    {
        //                        capa.Trimestre1.Mes1 = item.Mes;
        //                        capa.Trimestre1.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
        //                    {
        //                        capa.Trimestre1.Mes2 = item.Mes;
        //                        capa.Trimestre1.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
        //                    {
        //                        capa.Trimestre1.Mes3 = item.Mes;
        //                        capa.Trimestre1.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre1.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
        //                    {
        //                        capa.Trimestre2.Mes1 = item.Mes;
        //                        capa.Trimestre2.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
        //                    {
        //                        capa.Trimestre2.Mes2 = item.Mes;
        //                        capa.Trimestre2.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
        //                    {
        //                        capa.Trimestre2.Mes3 = item.Mes;
        //                        capa.Trimestre2.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre2.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
        //                    {
        //                        capa.Trimestre3.Mes1 = item.Mes;
        //                        capa.Trimestre3.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
        //                    {
        //                        capa.Trimestre3.Mes2 = item.Mes;
        //                        capa.Trimestre3.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
        //                    {
        //                        capa.Trimestre3.Mes3 = item.Mes;
        //                        capa.Trimestre3.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre3.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //                else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
        //                {
        //                    #region
        //                    if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
        //                    {
        //                        capa.Trimestre4.Mes1 = item.Mes;
        //                        capa.Trimestre4.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes1Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
        //                    {
        //                        capa.Trimestre4.Mes2 = item.Mes;
        //                        capa.Trimestre4.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes2Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
        //                    {
        //                        capa.Trimestre4.Mes3 = item.Mes;
        //                        capa.Trimestre4.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
        //                        capa.Trimestre4.Mes3Vlr = item.MetaPlanejada.HasValue ? item.MetaPlanejada : 0;
        //                    }
        //                    #endregion
        //                }
        //            }
        //            #endregion
        //        }
        //    }
        //    #endregion

        //    return lstOrcamentoDetalhado;
        //}

        //private bool ExcluirArquivo(string path)
        //{
        //    try
        //    {
        //        if (File.Exists(path))
        //        {
        //            File.Delete(path);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SDKore.Helper.Error.Handler(ex);
        //        return false;
        //    }

        //    return true;
        //}

        //private void EncerrarExcel(ref Microsoft.Office.Interop.Excel.Application app, ref Microsoft.Office.Interop.Excel.Workbook workbook, ref Microsoft.Office.Interop.Excel.Worksheet worksheet)
        //{
        //    if (worksheet != null)
        //    {
        //        if (System.Runtime.InteropServices.Marshal.IsComObject(worksheet))
        //        {
        //            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(worksheet);
        //        }
        //    }

        //    if (workbook != null)
        //    {
        //        try { workbook.Close(false); }
        //        catch { }

        //        if (System.Runtime.InteropServices.Marshal.IsComObject(workbook))
        //        {
        //            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
        //        }
        //    }

        //    if (app != null)
        //    {
        //        app.Quit();

        //        if (System.Runtime.InteropServices.Marshal.IsComObject(app))
        //        {
        //            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(app);
        //        }
        //    }

        //    worksheet = null;
        //    workbook = null;
        //    app = null;
        //}
    }
}

