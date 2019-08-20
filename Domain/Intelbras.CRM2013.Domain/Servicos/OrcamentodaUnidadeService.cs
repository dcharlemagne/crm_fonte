using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using Excel = ClosedXML.Excel;
using System.IO;
using System.Data;
using System.Diagnostics;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OrcamentodaUnidadeService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeporTrimestreService _ServiceOrcamentodaUnidadeporTrimestre = null;
        private OrcamentodaUnidadeporTrimestreService ServiceOrcamentodaUnidadeporTrimestre
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporTrimestre == null)
                    _ServiceOrcamentodaUnidadeporTrimestre = new OrcamentodaUnidadeporTrimestreService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporTrimestre;
            }
        }

        OrcamentodaUnidadeporSubFamiliaService _ServiceOrcamentodaUnidadeporSubFamilia = null;
        private OrcamentodaUnidadeporSubFamiliaService ServiceOrcamentodaUnidadeporSubFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSubFamilia == null)
                    _ServiceOrcamentodaUnidadeporSubFamilia = new OrcamentodaUnidadeporSubFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporSubFamilia;
            }
        }

        OrcamentodaUnidadeporFamiliaService _ServiceOrcamentodaUnidadeporFamilia = null;
        private OrcamentodaUnidadeporFamiliaService ServiceOrcamentodaUnidadeporFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporFamilia == null)
                    _ServiceOrcamentodaUnidadeporFamilia = new OrcamentodaUnidadeporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporFamilia;
            }
        }

        OrcamentodaUnidadeporSegmentoService _ServiceOrcamentodaUnidadeporSegmento = null;
        private OrcamentodaUnidadeporSegmentoService ServiceOrcamentodaUnidadeporSegmento
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSegmento == null)
                    _ServiceOrcamentodaUnidadeporSegmento = new OrcamentodaUnidadeporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporSegmento;
            }
        }

        OrcamentodaUnidadeporProdutoService _ServiceOrcamentodaUnidadeporProduto = null;
        private OrcamentodaUnidadeporProdutoService ServiceOrcamentodaUnidadeporProduto
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporProduto == null)
                    _ServiceOrcamentodaUnidadeporProduto = new OrcamentodaUnidadeporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporProduto;
            }
        }

        OrcamentodoCanalporProdutoService _ServiceOrcamentodoCanalporProduto = null;
        private OrcamentodoCanalporProdutoService ServiceOrcamentodoCanalporProduto
        {
            get
            {
                if (_ServiceOrcamentodoCanalporProduto == null)
                    _ServiceOrcamentodoCanalporProduto = new OrcamentodoCanalporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporProduto;
            }
        }

        OrcamentodoCanalporFamiliaService _ServiceOrcamentodoCanalporFamilia = null;
        private OrcamentodoCanalporFamiliaService ServiceOrcamentodoCanalporFamilia
        {
            get
            {
                if (_ServiceOrcamentodoCanalporFamilia == null)
                    _ServiceOrcamentodoCanalporFamilia = new OrcamentodoCanalporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporFamilia;
            }
        }

        OrcamentodoCanalporSegmentoService _ServiceOrcamentodoCanalporSegmento = null;
        private OrcamentodoCanalporSegmentoService ServiceOrcamentodoCanalporSegmento
        {
            get
            {
                if (_ServiceOrcamentodoCanalporSegmento == null)
                    _ServiceOrcamentodoCanalporSegmento = new OrcamentodoCanalporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporSegmento;
            }
        }

        OrcamentodoCanalService _ServiceOrcamentodoCanal = null;
        private OrcamentodoCanalService ServiceOrcamentodoCanal
        {
            get
            {
                if (_ServiceOrcamentodoCanal == null)
                    _ServiceOrcamentodoCanal = new OrcamentodoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanal;
            }
        }

        ArquivoService _ServiceArquivo = null;
        private ArquivoService ServiceArquivo
        {
            get
            {
                if (_ServiceArquivo == null)
                    _ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceArquivo;
            }
        }

        OrcamentodoCanalDetalhadoporProdutoService _ServiceOrcamentodoCanalDetalhadoporProduto = null;
        private OrcamentodoCanalDetalhadoporProdutoService ServiceOrcamentodoCanalDetalhadoporProduto
        {
            get
            {
                if (_ServiceOrcamentodoCanalDetalhadoporProduto == null)
                    _ServiceOrcamentodoCanalDetalhadoporProduto = new OrcamentodoCanalDetalhadoporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalDetalhadoporProduto;
            }
        }

        ProdutoService _ServiceProduto = null;
        private ProdutoService ServiceProduto
        {
            get
            {
                if (_ServiceProduto == null)
                    _ServiceProduto = new ProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceProduto;
            }
        }
        #endregion

        #region Métodos chamado pelo Serviço associado a task do Windows
        /// <summary>
        /// Metodo chamado do serviço associado ao task do windows
        /// </summary>
        public void ProcessarPlanilhaOrcamento()
        {
            List<OrcamentodaUnidade> lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportarPlanilhaOrcamento);
            if (lstOrcamentoUnidade != null && lstOrcamentoUnidade.Count > 0)
            {
                OrcamentodaUnidade item = lstOrcamentoUnidade[0];
                try
                {
                    Console.WriteLine(item.Nome);
                    item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportandoPlanilhaOrcamento;
                    RepositoryService.OrcamentodaUnidade.Update(item);

                    this.ProcessarLeituraPlanilha(item.ID.Value);
                }
                catch (Exception erro)
                {
                    item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ErroImportarPlanilhaOrcamento;
                    item.MensagemProcessamento = erro.Message.Substring(0, (erro.Message.Length < 1999 ? erro.Message.Length : 1998));
                    RepositoryService.OrcamentodaUnidade.Update(item);

                    EventLog.WriteEntry("Aplication", erro.Message, EventLogEntryType.Error, 171);
                    throw new ArgumentException(erro.Message);
                }
            }
        }

        /// <summary>
        /// Metodo chamado do serviço associado ao task do windows
        /// </summary>
        public void GerarPlanilhaOrcamento()
        {
            try
            {
                List<OrcamentodaUnidade> lstOrcamentoUnidade = new List<OrcamentodaUnidade>();

                lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerarModelodeOrcamento);
                if (lstOrcamentoUnidade != null && lstOrcamentoUnidade.Count > 0)
                {
                    OrcamentodaUnidade item = lstOrcamentoUnidade[0];

                    try
                    {
                        Console.WriteLine(item.Nome);

                        item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerandoModelodeOrcamento;

                        RepositoryService.OrcamentodaUnidade.Update(item);

                        this.GerarPlanilhaRegistros(item.ID.Value);
                    }
                    catch (Exception erro)
                    {
                        item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ErroGeracaoModeloOrcamento;
                        item.MensagemProcessamento = erro.Message.Substring(0, (erro.Message.Length < 1999 ? erro.Message.Length : 1998));
                        RepositoryService.OrcamentodaUnidade.Update(item);

                        EventLog.WriteEntry("Aplication", erro.Message, EventLogEntryType.Error, 161);
                    }
                }
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }

        }


        /// <summary>
        /// Metodo chamado do serviço associado ao task do windows
        /// </summary>
        public void ProcessarPlanilhaOrcamentoManual()
        {
            List<OrcamentodaUnidade> lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilhaManual((int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportarPlanilhaOrcamentoCanalManual);
            if (lstOrcamentoUnidade != null && lstOrcamentoUnidade.Count > 0)
            {

                OrcamentodaUnidade item = lstOrcamentoUnidade[0];
                try
                {
                    Console.WriteLine(item.Nome);
                    item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportandoPlanilhaOrcamentoCanalManual;
                    RepositoryService.OrcamentodaUnidade.Update(item);
                    this.ProcessarLeituraPlanilhaManual(item.ID.Value);

                    item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.PlanilhaOrcamentoCanalManualImportadaSucesso;
                    item.AddNullProperty("MensagemProcessamento");
                    RepositoryService.OrcamentodaUnidade.Update(item);
                }
                catch (Exception erro)
                {
                    item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ErroImportarOrcamentoCanalManual;
                    item.MensagemProcessamento = erro.Message.Substring(0, (erro.Message.Length < 1999 ? erro.Message.Length : 1998));
                    RepositoryService.OrcamentodaUnidade.Update(item);

                    EventLog.WriteEntry("Aplication", erro.Message, EventLogEntryType.Error, 171);
                    throw new ArgumentException(erro.Message);
                }
            }
        }

        /// <summary>
        /// Metodo chamado do serviço associado ao task do windows
        /// </summary>
        public void GerarPlanilhaOrcamentoManual()
        {
            try
            {
                List<OrcamentodaUnidade> lstOrcamentoUnidade = new List<OrcamentodaUnidade>();

                lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilhaManual((int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerarOrcamentoCanalManual);
                if (lstOrcamentoUnidade != null && lstOrcamentoUnidade.Count > 0)
                {
                    OrcamentodaUnidade item = lstOrcamentoUnidade[0];

                    try
                    {
                        Console.WriteLine(item.Nome);

                        item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerandoOrcamentoCanalManual;

                        RepositoryService.OrcamentodaUnidade.Update(item);

                        this.GerarOrcamentoManual(item.ID.Value);

                    }
                    catch (Exception erro)
                    {

                        item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ErroGerarOrcamentoCanalManual;
                        item.MensagemProcessamento = erro.Message.Substring(0, (erro.Message.Length < 1999 ? erro.Message.Length : 1998));

                        RepositoryService.OrcamentodaUnidade.Update(item);
                        EventLog.WriteEntry("Aplication", erro.Message, EventLogEntryType.Error, 171);
                    }
                }
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
        }

        public void EndTaskOfWindows()
        {
            List<OrcamentodaUnidade> lstOrcamentoUnidade = new List<OrcamentodaUnidade>();
            #region Orcamento
            lstOrcamentoUnidade.AddRange(RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerandoModelodeOrcamento).ToArray());
            lstOrcamentoUnidade.AddRange(RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportandoPlanilhaOrcamento).ToArray());
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemProcessamento = "Serviço foi interrompido.";

                if (item.StateCode == (int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerandoModelodeOrcamento)
                    item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ErroGeracaoModeloOrcamento;
                else if (item.StateCode == (int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportandoPlanilhaOrcamento)
                    item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ErroImportarPlanilhaOrcamento;

                RepositoryService.OrcamentodaUnidade.Update(item);
            }
            #endregion

            lstOrcamentoUnidade.Clear();

            #region Orçamento Manual
            lstOrcamentoUnidade.AddRange(RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerandoOrcamentoCanalManual).ToArray());
            lstOrcamentoUnidade.AddRange(RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportandoPlanilhaOrcamentoCanalManual).ToArray());
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemProcessamento = "Serviço foi interrompido.";

                if (item.RazaoStatusOramentoManual == (int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerandoOrcamentoCanalManual)
                    item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ErroGerarOrcamentoCanalManual;
                else if (item.RazaoStatusOramentoManual == (int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportandoPlanilhaOrcamentoCanalManual)
                    item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ErroImportarOrcamentoCanalManual;

                RepositoryService.OrcamentodaUnidade.Update(item);
            }
            #endregion

            lstOrcamentoUnidade.Clear();
        }
        #endregion

        #region Métodos Plugin
        public void PreCreate(OrcamentodaUnidade mOrcamento)
        {
            List<OrcamentodaUnidade> lstOrcamento = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(mOrcamento.UnidadedeNegocio.Id, mOrcamento.Ano.Value);
            if (lstOrcamento.Count > 0)
                throw new ArgumentException("Já existe Orçamento para Esta Unidade de Negocios.");
        }
        #endregion

        #region Métodos
        public void AjustaStatusStartExe()
        {
            List<OrcamentodaUnidade> lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportandoPlanilhaOrcamento);
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportarPlanilhaOrcamento;
                RepositoryService.OrcamentodaUnidade.Update(item);
            }

            lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilha((int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerandoModelodeOrcamento);
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.GerarModelodeOrcamento;
                RepositoryService.OrcamentodaUnidade.Update(item);
            }

            lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilhaManual((int)Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportandoPlanilhaOrcamentoCanalManual);
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportarPlanilhaOrcamentoCanalManual;
                RepositoryService.OrcamentodaUnidade.Update(item);
            }

            lstOrcamentoUnidade = RepositoryService.OrcamentodaUnidade.ObterOrcamentoParaGerarPlanilhaManual((int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerandoOrcamentoCanalManual);
            foreach (OrcamentodaUnidade item in lstOrcamentoUnidade)
            {
                item.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.GerarOrcamentoCanalManual;
                RepositoryService.OrcamentodaUnidade.Update(item);
            }

        }

        public List<Model.OrcamentoDetalhado> ListarProdutosDetalhadoOrcamentoCanal(OrcamentodaUnidade mOrcamentoUnidade, List<Guid> lstGrupoEstoque)
        {
            #region variaveis/objetos
            Model.OrcamentoDetalhado mOrcamentoDetalhado;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<Model.OrcamentoDetalhado>();
            List<OrcamentodoCanalDetalhadoporProduto> lstOrcamentoCanalProd;

            List<Guid> lstIdCanal = new List<Guid>();
            List<Conta> lstContas = new List<Conta>();
            List<Conta> lstContasPage = new List<Conta>();

            List<Guid> lstIdProdutos = new List<Guid>();
            List<Product> lstProdutos = new List<Product>();
            List<Product> lstProdutosPage = new List<Product>();

            bool paginar = true;
            int page = 1;
            #endregion

            #region pega os produtos e canais do orçamento, ou pela unidade de negocio
            #region produtos
            while (paginar)
            {
                lstProdutosPage = RepositoryService.Produto.ListarProdutosOrcamentoCanal(mOrcamentoUnidade.ID.Value, page, 5000);

                if (lstProdutosPage.Count < 5000)
                    paginar = false;

                lstProdutos.AddRange(lstProdutosPage.ToArray());
                page++;
            }
            #endregion

            page = 1;
            paginar = true;

            #region contas
            while (paginar)
            {
                lstContasPage = RepositoryService.Conta.ListarContasOrcamentoCanal(mOrcamentoUnidade.ID.Value, page, 5000);

                if (lstContasPage.Count < 5000)
                    paginar = false;

                lstContas.AddRange(lstContasPage.ToArray());
                page++;
            }
            #endregion
            #endregion

            #region Retira as duplicidades do produto

            var lstDuplicProd = (from p in lstProdutos
                                 group p by string.Format("{0}", p.ID.Value));

            foreach (var iProd in lstDuplicProd)
                lstIdProdutos.Add(iProd.First().ID.Value);

            #endregion

            #region Retira as duplicidades dos canais

            var lstDuplicCanal = (from c in lstContas
                                  group c by string.Format("{0}", c.ID.Value));

            foreach (var iConta in lstDuplicCanal)
                lstIdCanal.Add(iConta.First().ID.Value);

            #endregion

            #region caso nao tenha orçamento buscar produtos e contas que foram criados pela unidade de negocio,verifica se acumula outro produto
            lstProdutos.AddRange(RepositoryService.Produto.ListarParaMeta(mOrcamentoUnidade.UnidadedeNegocio.Id, lstGrupoEstoque.ToArray()).ToArray());
            ServiceProduto.ProdutoAcumulaOutroProduto(ref lstProdutos);
            #endregion


            lstContas.AddRange(RepositoryService.Conta.ListarContasParticipantes(mOrcamentoUnidade.UnidadedeNegocio.Id, lstIdCanal).ToArray());

            #region relaciona produtos por canal e novos produtos
            var listaProd = (from p in lstProdutos
                             group p by string.Format("{0}/{1}/{2}", p.Segmento.Id, p.FamiliaProduto.Id, p.SubfamiliaProduto.Id));

            foreach (var lstprod in listaProd)
            {
                foreach (var item in lstprod)
                {
                    #region Gera contas/por produtos
                    foreach (Conta conta in lstContas)
                    {
                        if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == item.ID.Value && x.Canal.Id == conta.ID.Value))
                        {
                            #region Gerando Colunas
                            mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
                            mOrcamentoDetalhado.Ano = mOrcamentoUnidade.Ano;
                            mOrcamentoDetalhado.UnidadeNegocio = mOrcamentoUnidade.UnidadedeNegocio;
                            mOrcamentoDetalhado.Segmento = item.Segmento;
                            mOrcamentoDetalhado.Familia = item.FamiliaProduto;
                            mOrcamentoDetalhado.SubFamilia = item.SubfamiliaProduto;
                            mOrcamentoDetalhado.Produto = new Lookup(item.ID.Value, item.Nome, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                            mOrcamentoDetalhado.StatusProduto = item.RazaoStatus.Value == 1 ? "Ativo" : "Descontinuado";
                            mOrcamentoDetalhado.Canal = new Lookup(conta.ID.Value, conta.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                            mOrcamentoDetalhado.StatusParticipacao = conta.ParticipantePrograma.HasValue ? (conta.ParticipantePrograma.Value == 993520001 ? "Sim" : (conta.ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "Não";

                            lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
                            #endregion
                        }
                    }
                    #endregion
                }

                foreach (Conta conta in lstContas)
                {
                    if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == Guid.Empty && x.Canal.Id == conta.ID.Value && x.SubFamilia.Id == lstprod.First().SubfamiliaProduto.Id))
                    {
                        #region Gerando Colunas Novos Produtos/Para Canais
                        mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
                        mOrcamentoDetalhado.Ano = mOrcamentoUnidade.Ano;
                        mOrcamentoDetalhado.UnidadeNegocio = mOrcamentoUnidade.UnidadedeNegocio;
                        mOrcamentoDetalhado.Segmento = new Lookup(lstprod.First().Segmento.Id, lstprod.First().Segmento.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                        mOrcamentoDetalhado.Familia = new Lookup(lstprod.First().FamiliaProduto.Id, lstprod.First().FamiliaProduto.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                        mOrcamentoDetalhado.SubFamilia = new Lookup(lstprod.First().SubfamiliaProduto.Id, lstprod.First().SubfamiliaProduto.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                        mOrcamentoDetalhado.Produto = new Lookup(Guid.Empty, "Novos Produtos", SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcamentoDetalhado.StatusProduto = "Ativo";
                        mOrcamentoDetalhado.Canal = new Lookup(conta.ID.Value, conta.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcamentoDetalhado.StatusParticipacao = conta.ParticipantePrograma.HasValue ? (conta.ParticipantePrograma.Value == 993520001 ? "Sim" : (conta.ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "Não";

                        lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
                        #endregion
                    }
                }
            }
            #endregion

            #region obtem detalhamento dos produtos nos trimestres
            foreach (Model.OrcamentoDetalhado capa in lstOrcamentoDetalhado)
            {
                lstOrcamentoCanalProd = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ListarDetalheProdutosPorCanal(mOrcamentoUnidade.ID.Value, capa.Canal.Id, capa.Produto.Id);
                if (lstOrcamentoCanalProd != null && lstOrcamentoCanalProd.Count > 0)
                {
                    #region
                    capa.Trimestre1 = new Trimestre();
                    capa.Trimestre2 = new Trimestre();
                    capa.Trimestre3 = new Trimestre();
                    capa.Trimestre4 = new Trimestre();

                    foreach (OrcamentodoCanalDetalhadoporProduto item in lstOrcamentoCanalProd)
                    {
                        if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
                            {
                                capa.Trimestre1.Mes1 = item.Mes;
                                capa.Trimestre1.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
                            {
                                capa.Trimestre1.Mes2 = item.Mes;
                                capa.Trimestre1.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
                            {
                                capa.Trimestre1.Mes3 = item.Mes;
                                capa.Trimestre1.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
                            {
                                capa.Trimestre2.Mes1 = item.Mes;
                                capa.Trimestre2.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
                            {
                                capa.Trimestre2.Mes2 = item.Mes;
                                capa.Trimestre2.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
                            {
                                capa.Trimestre2.Mes3 = item.Mes;
                                capa.Trimestre2.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
                            {
                                capa.Trimestre3.Mes1 = item.Mes;
                                capa.Trimestre3.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
                            {
                                capa.Trimestre3.Mes2 = item.Mes;
                                capa.Trimestre3.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
                            {
                                capa.Trimestre3.Mes3 = item.Mes;
                                capa.Trimestre3.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
                            {
                                capa.Trimestre4.Mes1 = item.Mes;
                                capa.Trimestre4.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
                            {
                                capa.Trimestre4.Mes2 = item.Mes;
                                capa.Trimestre4.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
                            {
                                capa.Trimestre4.Mes3 = item.Mes;
                                capa.Trimestre4.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return lstOrcamentoDetalhado;
        }

        public List<Model.OrcamentoDetalhado> ListarProdutosDetalhadoOrcamentoResumido(OrcamentodaUnidade mOrcamentoUnidade, List<Guid> lstGrupoEstoque)
        {
            #region variaveis/objetos
            Model.OrcamentoDetalhado mOrcamentoDetalhado;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<Model.OrcamentoDetalhado>();
            List<OrcamentodaUnidadeDetalhadoporProduto> lstOrcamentoUnidadeProd;
            List<Product> lstProdutos = new List<Product>();

            List<Guid> lstIdProdutos = new List<Guid>();
            List<Product> lstProdutosPage = new List<Product>();
            bool paginar = true;
            int page = 1;
            #endregion

            #region produtos
            while (paginar)
            {
                lstProdutosPage = RepositoryService.Produto.ListarProdutosOrcamentoUnidade(mOrcamentoUnidade.ID.Value, page, 5000);

                if (lstProdutosPage.Count < 5000)
                    paginar = false;

                lstProdutos.AddRange(lstProdutosPage.ToArray());
                page++;
            }
            #endregion

            #region Retira as duplicidades do produto

            var lstDuplicProd = (from p in lstProdutos
                                 group p by string.Format("{0}", p.ID.Value));

            foreach (var iProd in lstDuplicProd)
                lstIdProdutos.Add(iProd.First().ID.Value);

            #endregion

            #region caso nao tenha orçamento buscar produtos e contas que foram criados pela unidade de negocio,verifica se acumula outro produto
            lstProdutos.AddRange(RepositoryService.Produto.ListarParaMeta(mOrcamentoUnidade.UnidadedeNegocio.Id, lstGrupoEstoque.ToArray()));
            ServiceProduto.ProdutoAcumulaOutroProduto(ref lstProdutos);
            #endregion

            #region relaciona produtos por canais
            var listaProd = (from p in lstProdutos
                             group p by string.Format("{0}/{1}/{2}", p.Segmento.Id, p.FamiliaProduto.Id, p.SubfamiliaProduto.Id));

            foreach (var lstprod in listaProd)
            {
                foreach (var item in lstprod)
                {
                    #region Gera por produtos
                    if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == item.ID.Value && x.SubFamilia.Id == item.SubfamiliaProduto.Id))
                    {
                        #region Gerando Colunas
                        mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
                        mOrcamentoDetalhado.Ano = mOrcamentoUnidade.Ano;
                        mOrcamentoDetalhado.UnidadeNegocio = mOrcamentoUnidade.UnidadedeNegocio;
                        mOrcamentoDetalhado.Segmento = item.Segmento;
                        mOrcamentoDetalhado.Familia = item.FamiliaProduto;
                        mOrcamentoDetalhado.SubFamilia = item.SubfamiliaProduto;
                        mOrcamentoDetalhado.Produto = new Lookup(item.ID.Value, item.Nome, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcamentoDetalhado.StatusProduto = item.RazaoStatus.Value == 1 ? "Ativo" : "Descontinuado";
                        mOrcamentoDetalhado.Canal = new Lookup(Guid.Empty, "Não Alocado", SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcamentoDetalhado.StatusParticipacao = "Não";

                        lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
                        #endregion
                    }
                    #endregion
                }

                if (!lstOrcamentoDetalhado.Exists(x => x.Produto.Id == Guid.Empty && x.SubFamilia.Id == lstprod.First().SubfamiliaProduto.Id))
                {
                    #region Gerando Colunas Novos Produtos/Para Canais
                    mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
                    mOrcamentoDetalhado.Ano = mOrcamentoUnidade.Ano;
                    mOrcamentoDetalhado.UnidadeNegocio = mOrcamentoUnidade.UnidadedeNegocio;
                    mOrcamentoDetalhado.Segmento = new Lookup(lstprod.First().Segmento.Id, lstprod.First().Segmento.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentoDetalhado.Familia = new Lookup(lstprod.First().FamiliaProduto.Id, lstprod.First().FamiliaProduto.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentoDetalhado.SubFamilia = new Lookup(lstprod.First().SubfamiliaProduto.Id, lstprod.First().SubfamiliaProduto.Name, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mOrcamentoDetalhado.Produto = new Lookup(Guid.Empty, "Novos Produtos", SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                    mOrcamentoDetalhado.StatusProduto = "Ativo";
                    mOrcamentoDetalhado.Canal = new Lookup(Guid.Empty, "Não Alocado", SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentoDetalhado.StatusParticipacao = "Não";

                    lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
                    #endregion
                }

            }
            #endregion

            #region obtem detalhamento dos produtos nos trimestres
            foreach (Model.OrcamentoDetalhado capa in lstOrcamentoDetalhado)
            {
                lstOrcamentoUnidadeProd = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ObterOrcDetalhadoProdutos(capa.Produto.Id, mOrcamentoUnidade.ID.Value);
                if (lstOrcamentoUnidadeProd != null && lstOrcamentoUnidadeProd.Count > 0)
                {
                    #region
                    capa.Trimestre1 = new Trimestre();
                    capa.Trimestre2 = new Trimestre();
                    capa.Trimestre3 = new Trimestre();
                    capa.Trimestre4 = new Trimestre();

                    foreach (OrcamentodaUnidadeDetalhadoporProduto item in lstOrcamentoUnidadeProd)
                    {
                        if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
                            {
                                capa.Trimestre1.Mes1 = item.Mes;
                                capa.Trimestre1.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
                            {
                                capa.Trimestre1.Mes2 = item.Mes;
                                capa.Trimestre1.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
                            {
                                capa.Trimestre1.Mes3 = item.Mes;
                                capa.Trimestre1.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
                            {
                                capa.Trimestre2.Mes1 = item.Mes;
                                capa.Trimestre2.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
                            {
                                capa.Trimestre2.Mes2 = item.Mes;
                                capa.Trimestre2.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
                            {
                                capa.Trimestre2.Mes3 = item.Mes;
                                capa.Trimestre2.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
                            {
                                capa.Trimestre3.Mes1 = item.Mes;
                                capa.Trimestre3.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
                            {
                                capa.Trimestre3.Mes2 = item.Mes;
                                capa.Trimestre3.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
                            {
                                capa.Trimestre3.Mes3 = item.Mes;
                                capa.Trimestre3.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
                            {
                                capa.Trimestre4.Mes1 = item.Mes;
                                capa.Trimestre4.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
                            {
                                capa.Trimestre4.Mes2 = item.Mes;
                                capa.Trimestre4.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
                            {
                                capa.Trimestre4.Mes3 = item.Mes;
                                capa.Trimestre4.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return lstOrcamentoDetalhado;
        }

        public void GerarPlanilhaRegistros(Guid orcamentounidadeId)
        {
            string sArquivo = string.Empty;
            Guid anexoId = Guid.Empty;
            try
            {
                #region variaveis e objetos
                string[] parametrosgrobais;
                ParametroGlobal mParametroGlobal;
                GrupoEstoque mGrupoEstoque;
                List<Guid> lstGrupoEstoque = new List<Guid>();
                object _missingValue = System.Reflection.Missing.Value;
                OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre1;
                OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre2;
                OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre3;
                OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre4;
                Guid tri1 = Guid.Empty;
                Guid tri2 = Guid.Empty;
                Guid tri3 = Guid.Empty;
                Guid tri4 = Guid.Empty;
                OrcamentodaUnidade mOrcamentodaUnidade;
                Observacao mObservaocao;
                List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado;
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Plan1");
                //Excel.Workbook wb = null;
                //Excel.Worksheet ws;
                //Excel.Range Linha;
                //Excel.Range trimestre;
                #endregion

                try
                {
                    //get orcamento unidade
                    mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(orcamentounidadeId);

                    #region find and Download a file with the NivelOrcamento
                    if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                        mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(41);
                    else
                        mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(44);

                    if (mParametroGlobal == null)
                        throw new ArgumentException("Não foi Encontrado Parametro Global para este nivel de Orçamento.");

                    mObservaocao = RepositoryService.Observacao.ObterPorParametrosGlobais(mParametroGlobal.ID.Value);
                    if (mObservaocao == null)
                        throw new ArgumentException("Não foi Encontrado a Planilha em Parametro Global, para este nivel de Orçamento.");
                    #endregion
                    sArquivo = ServiceArquivo.DownLoadArquivo(SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"), "Orcamento_" + mOrcamentodaUnidade.Nome, mObservaocao.Body, ".xlsx");

                    #region get ParametroGlobal e search all grupoestoque to filter
                    mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(42);
                    if (mParametroGlobal != null)
                    {
                        if (!string.IsNullOrEmpty(mParametroGlobal.Valor))
                        {
                            parametrosgrobais = mParametroGlobal.Valor.Split(';');
                            foreach (string parametro in parametrosgrobais)
                            {
                                mGrupoEstoque = RepositoryService.GrupoEstoque.ObterPor(Convert.ToInt32(parametro));
                                if (mGrupoEstoque != null)
                                    lstGrupoEstoque.Add(mGrupoEstoque.ID.Value);
                            }
                        }
                    }
                    #endregion

                    try
                    {
                        if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                            lstOrcamentoDetalhado = ListarProdutosDetalhadoOrcamentoCanal(mOrcamentodaUnidade, lstGrupoEstoque);
                        else
                            lstOrcamentoDetalhado = ListarProdutosDetalhadoOrcamentoResumido(mOrcamentodaUnidade, lstGrupoEstoque);


                    }
                    catch (Exception erroLista)
                    {
                        throw new ArgumentException("Erro ao Relacionar Produtos e Canais, contate o administrador do sistema. \n" + erroLista.Message);
                    }
                    //open and prepare file
                    /*try
                    {
                        wb = excel.Workbooks.Open(sArquivo);
                        //wb = excel.Workbooks.Open(sArquivo, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue, _missingValue);
                        ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
                        ws.Name = "Plan1";
                    }
                    catch (Exception erroopenexcel)
                    {
                        throw new ArgumentException("Ocorreu um erro ao abrir o \n Excel:" + erroopenexcel.Message);
                    }*/

                    #region Cabeçalho Unidade/ Ano/ Detalhamento
                    ws.Cell("A1").Value = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    //Linha = (ws.Cells[1, 1] as Excel.Range);
                    ws.Cell("E1").Value = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    //Linha = (ws.Cells[1, 5] as Excel.Range);
                    //Linha.Value2 = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    ws.Cell("D2").Value = mOrcamentodaUnidade.Ano;
                    //Linha = (ws.Cells[2, 4] as Excel.Range);
                    //Linha.Value2 = mOrcamentodaUnidade.Ano;
                    
                    //Linha = (ws.Cells[3, 4] as Excel.Range);
                    if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Resumido)
                    {
                        ws.Cell("D3").Value = mOrcamentodaUnidade.Ano;
                        //Linha.Value2 = "Resumido";
                    }
                    else
                    {
                        ws.Cell("D3").Value = "Detalhado";
                        //Linha.Value2 = "Detalhado";
                    }
                    

                    #endregion

                    try
                    {
                        #region Monta os Guids dos Orçamentos Trimetrais
                        /*trimestre = (ws.Cells[4, 18] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre1 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre1 != null ? mOrcamentodaUnidadeporTrimestre1.ID.ToString() : Guid.NewGuid().ToString();
                        tri1 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 24] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre2 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre2 != null ? mOrcamentodaUnidadeporTrimestre2.ID.ToString() : Guid.NewGuid().ToString();
                        tri2 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 30] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre3 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre3 != null ? mOrcamentodaUnidadeporTrimestre3.ID.ToString() : Guid.NewGuid().ToString();
                        tri3 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 36] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre4 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre4 != null ? mOrcamentodaUnidadeporTrimestre4.ID.ToString() : Guid.NewGuid().ToString();
                        tri4 = Guid.Parse(trimestre.Value2);*/
                        #endregion
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Erro ao gerar o Id dos Trimestres, contate o administrador do sistema e verifique a versão da planilha");
                    }

                    try
                    {
                        //ServiceArquivo.GerarPlanilhaOrcamento(mOrcamentodaUnidade, lstOrcamentoDetalhado, ws);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Erro ao gerar a planilha, favor entrar em contato com o administrador.");
                    }


                    //wb.Save();
                    //wb.Close(Type.Missing, Type.Missing, Type.Missing);
                    //excel.Quit();
                    //wb = null;

                    #region Anexa Arquivo ao orçamento
                    Lookup entidade = new Lookup(orcamentounidadeId, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidade>());
                    ServiceArquivo.AnexaArquivo(sArquivo, "Planilha Gerada com Sucesso", "Orcamento_" + mOrcamentodaUnidade.Nome + ".xlsx", entidade, out anexoId);
                    mOrcamentodaUnidade.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.ModelodeOrcamentoGeradocomSucesso;
                    mOrcamentodaUnidade.AddNullProperty("MensagemProcessamento");
                    RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
                    #endregion

                    try
                    {
                        #region Criando os trimestres
                        /*if (mOrcamentodaUnidadeporTrimestre1 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.Atualiza(mOrcamentodaUnidadeporTrimestre1, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.Criar(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 1o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1, tri1, true);


                        if (mOrcamentodaUnidadeporTrimestre2 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.Atualiza(mOrcamentodaUnidadeporTrimestre2, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.Criar(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 2o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2, tri2, false);


                        if (mOrcamentodaUnidadeporTrimestre3 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.Atualiza(mOrcamentodaUnidadeporTrimestre3, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.Criar(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 3o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3, tri3, false);

                        if (mOrcamentodaUnidadeporTrimestre4 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.Atualiza(mOrcamentodaUnidadeporTrimestre4, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.Criar(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 4o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4, tri4, false);*/
                        #endregion
                    }
                    catch (Exception erroCreateTrimestre)
                    {
                        if (anexoId != Guid.Empty)
                        {
                            RepositoryService.Anexo.Delete(anexoId);
                            anexoId = Guid.Empty;
                        }

                        throw new ArgumentException("Ocorreu um erro ao gerar os trimestre deste Orçamento, favor regerar. \n Caso o erro perssista contate o Administrador do Sistema. \n " + erroCreateTrimestre.Message);
                    }
                }
                catch (Exception erro)
                {
                    if (anexoId != Guid.Empty)
                    {
                        RepositoryService.Anexo.Delete(anexoId);
                        anexoId = Guid.Empty;
                    }

                    throw new ArgumentException(erro.Message);
                }
                finally
                {
                    if (File.Exists(sArquivo))
                        File.Delete(sArquivo);
                }

            }
            catch (Exception erros)
            {
                if (anexoId != Guid.Empty)
                {
                    RepositoryService.Anexo.Delete(anexoId);
                    anexoId = Guid.Empty;
                }

                if (File.Exists(sArquivo))
                    File.Delete(sArquivo);

                throw new ArgumentException(erros.Message);
            }
        }

        public void ProcessarLeituraPlanilha(Guid orcamentounidadeId)
        {
            OrcamentodaUnidade mOrcamentodaUnidade;
            mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(orcamentounidadeId);

            try
            {
                this.LerPlanilha(mOrcamentodaUnidade);

                ServiceOrcamentodaUnidadeporFamilia.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);
                ServiceOrcamentodaUnidadeporSegmento.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);
                ServiceOrcamentodaUnidadeporTrimestre.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);

                if (mOrcamentodaUnidade.NiveldoOrcamento == (int)Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                {
                    ServiceOrcamentodoCanalporFamilia.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);
                    ServiceOrcamentodoCanalporSegmento.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);
                    ServiceOrcamentodoCanal.CalcularOrcamento(mOrcamentodaUnidade.ID.Value);
                }

                this.CalcularOrcamento(orcamentounidadeId);
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            OrcamentodaUnidade mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(orcamentounidadeId);
            mOrcamentodaUnidade.OrcamentoNaoAlocado = 0;
            mOrcamentodaUnidade.OrcamentoParaNovosProdutos = 0;
            mOrcamentodaUnidade.OrcamentoPlanejado = 0;

            List<OrcamentodaUnidadeporTrimestre> lstTrimestre = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value);
            foreach (OrcamentodaUnidadeporTrimestre trimestre in lstTrimestre)
            {
                mOrcamentodaUnidade.OrcamentoNaoAlocado += trimestre.OrcamentonaoAlocado.HasValue ? trimestre.OrcamentonaoAlocado.Value : 0;
                mOrcamentodaUnidade.OrcamentoParaNovosProdutos += trimestre.OrcamentoparanovosProdutos.HasValue ? trimestre.OrcamentoparanovosProdutos.Value : 0;
                mOrcamentodaUnidade.OrcamentoPlanejado += trimestre.OrcamentoPlanejado.HasValue ? trimestre.OrcamentoPlanejado.Value : 0;
            }

            mOrcamentodaUnidade.StatusCode = (int)Domain.Enum.OrcamentodaUnidade.StatusCodeOrcamento.PlanilhaOrcamentoImportadaSucesso;
            RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
        }

        /// <summary>
        /// atualiza os registro da orçamento subfamilia,orcamento produto e detalhe orçamento produto
        /// </summary>
        /// <param name="orcamentounidadeId"></param>
        public void LerPlanilha(OrcamentodaUnidade mOrcamentodaUnidade)
        {
            #region Objetos e Variaveis
            OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia1;
            OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia2;
            OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia3;
            OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia4;

            OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia1;
            OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia2;
            OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia3;
            OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia4;
            #endregion

            List<OrcamentoDetalhado> lstOrcamentoDetalhado = this.lerPlanilhaRegistros(mOrcamentodaUnidade.ID.Value);
            try
            {
                #region UnidadeSubFamilia
                var lstOrcamentoporSubFamilia = (from x in lstOrcamentoDetalhado
                                                 orderby x.ProdutoID
                                                 group x by string.Format("{0}/{1}/{2}", x.SegmentoID, x.FamiliaID, x.SubFamiliaID));

                foreach (var OrcaUnidadeSubFamiliaProd in lstOrcamentoporSubFamilia)
                {
                    try
                    {
                        #region OrcamentoUnidadeSubFamilia
                        mOrcamentodaUnidadeporSubFamilia1 = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterPorSubFamiliaTrimestreUnidade(OrcaUnidadeSubFamiliaProd.First().SubFamiliaID.Value, OrcaUnidadeSubFamiliaProd.First().Trimestre1.Id.Value);
                        if (mOrcamentodaUnidadeporSubFamilia1 != null)
                        {
                            mOrcamentodaUnidadeporSubFamilia1.OrcamentoParaNovosProdutos = 0;
                            mOrcamentodaUnidadeporSubFamilia1.OrcamentoPlanejado = 0;
                        }
                        mOrcamentodaUnidadeporSubFamilia2 = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterPorSubFamiliaTrimestreUnidade(OrcaUnidadeSubFamiliaProd.First().SubFamiliaID.Value, OrcaUnidadeSubFamiliaProd.First().Trimestre2.Id.Value);
                        if (mOrcamentodaUnidadeporSubFamilia2 != null)
                        {
                            mOrcamentodaUnidadeporSubFamilia2.OrcamentoParaNovosProdutos = 0;
                            mOrcamentodaUnidadeporSubFamilia2.OrcamentoPlanejado = 0;
                        }
                        mOrcamentodaUnidadeporSubFamilia3 = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterPorSubFamiliaTrimestreUnidade(OrcaUnidadeSubFamiliaProd.First().SubFamiliaID.Value, OrcaUnidadeSubFamiliaProd.First().Trimestre3.Id.Value);
                        if (mOrcamentodaUnidadeporSubFamilia3 != null)
                        {
                            mOrcamentodaUnidadeporSubFamilia3.OrcamentoParaNovosProdutos = 0;
                            mOrcamentodaUnidadeporSubFamilia3.OrcamentoPlanejado = 0;
                        }
                        mOrcamentodaUnidadeporSubFamilia4 = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterPorSubFamiliaTrimestreUnidade(OrcaUnidadeSubFamiliaProd.First().SubFamiliaID.Value, OrcaUnidadeSubFamiliaProd.First().Trimestre4.Id.Value);
                        if (mOrcamentodaUnidadeporSubFamilia4 != null)
                        {
                            mOrcamentodaUnidadeporSubFamilia4.OrcamentoParaNovosProdutos = 0;
                            mOrcamentodaUnidadeporSubFamilia4.OrcamentoPlanejado = 0;
                        }
                        #endregion

                        #region Agrupa efetuando o Sum dos campos
                        var query = (from p in OrcaUnidadeSubFamiliaProd
                                     group p by new { p.ProdutoID }
                                         into s
                                         select new
                                         {
                                             trim1 = s.First().Trimestre1.Id,//0
                                             trim2 = s.First().Trimestre2.Id,//1
                                             trim3 = s.First().Trimestre3.Id,//2
                                             trim4 = s.First().Trimestre4.Id,//3
                                             Atu1 = s.First().AtualizarTrimestre1,//4
                                             Atu2 = s.First().AtualizarTrimestre2,//5
                                             Atu3 = s.First().AtualizarTrimestre3,//6
                                             Atu4 = s.First().AtualizarTrimestre4,//7
                                             id = s.First().ProdutoID,//8
                                             qtde = s.Count(),//9
                                             mes1 = s.Sum(q => q.Trimestre1.Mes1Vlr.Value),//10
                                             Qtde1 = s.Sum(q => q.Trimestre1.Mes1Qtde.Value),//11
                                             mes2 = s.Sum(q => q.Trimestre1.Mes2Vlr.Value),//12
                                             Qtde2 = s.Sum(q => q.Trimestre1.Mes2Qtde.Value),//13
                                             mes3 = s.Sum(q => q.Trimestre1.Mes3Vlr.Value),//14
                                             Qtde3 = s.Sum(q => q.Trimestre1.Mes3Qtde.Value),//15
                                             mes4 = s.Sum(q => q.Trimestre2.Mes1Vlr.Value),//16
                                             Qtde4 = s.Sum(q => q.Trimestre2.Mes1Qtde.Value),//17
                                             mes5 = s.Sum(q => q.Trimestre2.Mes2Vlr.Value),//18
                                             Qtde5 = s.Sum(q => q.Trimestre2.Mes2Qtde.Value),//19
                                             mes6 = s.Sum(q => q.Trimestre2.Mes3Vlr.Value),//20
                                             Qtde6 = s.Sum(q => q.Trimestre2.Mes3Qtde.Value),//21
                                             mes7 = s.Sum(q => q.Trimestre3.Mes1Vlr.Value),//22
                                             Qtde7 = s.Sum(q => q.Trimestre3.Mes1Qtde.Value),//23
                                             mes8 = s.Sum(q => q.Trimestre3.Mes2Vlr.Value),//24
                                             Qtde8 = s.Sum(q => q.Trimestre3.Mes2Qtde.Value),//25
                                             mes9 = s.Sum(q => q.Trimestre3.Mes3Vlr.Value),//26
                                             Qtde9 = s.Sum(q => q.Trimestre3.Mes3Qtde.Value),//27
                                             mes10 = s.Sum(q => q.Trimestre4.Mes1Vlr.Value),//28
                                             Qtde10 = s.Sum(q => q.Trimestre4.Mes1Qtde.Value),//29
                                             mes11 = s.Sum(q => q.Trimestre4.Mes2Vlr.Value),//30
                                             Qtde11 = s.Sum(q => q.Trimestre4.Mes2Qtde.Value),//31
                                             mes12 = s.Sum(q => q.Trimestre4.Mes3Vlr.Value),//32
                                             Qtde12 = s.Sum(q => q.Trimestre4.Mes3Qtde.Value),//33
                                         });
                        #endregion

                        foreach (var itemnew in query)
                        {
                            if (itemnew.id.Value != Guid.Empty)
                            {
                                ServiceOrcamentodaUnidadeporProduto.Atualizar(itemnew);

                                if (itemnew.Atu1)
                                    mOrcamentodaUnidadeporSubFamilia1.OrcamentoPlanejado += (itemnew.mes1 + itemnew.mes2 + itemnew.mes3);
                                if (itemnew.Atu2)
                                    mOrcamentodaUnidadeporSubFamilia2.OrcamentoPlanejado += (itemnew.mes4 + itemnew.mes5 + itemnew.mes6);
                                if (itemnew.Atu3)
                                    mOrcamentodaUnidadeporSubFamilia3.OrcamentoPlanejado += (itemnew.mes7 + itemnew.mes8 + itemnew.mes9);
                                if (itemnew.Atu4)
                                    mOrcamentodaUnidadeporSubFamilia4.OrcamentoPlanejado += (itemnew.mes10 + itemnew.mes11 + itemnew.mes12);
                            }
                            else
                            {
                                if (itemnew.Atu1)
                                    mOrcamentodaUnidadeporSubFamilia1.OrcamentoParaNovosProdutos += (itemnew.mes1 + itemnew.mes2 + itemnew.mes3);
                                if (itemnew.Atu2)
                                    mOrcamentodaUnidadeporSubFamilia2.OrcamentoParaNovosProdutos += (itemnew.mes4 + itemnew.mes5 + itemnew.mes6);
                                if (itemnew.Atu3)
                                    mOrcamentodaUnidadeporSubFamilia3.OrcamentoParaNovosProdutos += (itemnew.mes7 + itemnew.mes8 + itemnew.mes9);
                                if (itemnew.Atu4)
                                    mOrcamentodaUnidadeporSubFamilia4.OrcamentoParaNovosProdutos += (itemnew.mes10 + itemnew.mes11 + itemnew.mes12);
                            }
                        }

                        #region comentado
                        //foreach (var item in OrcaUnidadeSubFamiliaProd)
                        //{
                        //    #region
                        //    if (item.ProdutoID.Value != Guid.Empty)
                        //    {
                        //        ServiceOrcamentodaUnidadeporProduto.Atualizar(item);

                        //        if (item.AtualizarTrimestre1)
                        //            mOrcamentodaUnidadeporSubFamilia1.OrcamentoPlanejado += (item.Trimestre1.Mes1Vlr.HasValue ? item.Trimestre1.Mes1Vlr : 0) + (item.Trimestre1.Mes2Vlr.HasValue ? item.Trimestre1.Mes2Vlr : 0) + (item.Trimestre1.Mes3Vlr.HasValue ? item.Trimestre1.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre2)
                        //            mOrcamentodaUnidadeporSubFamilia2.OrcamentoPlanejado += (item.Trimestre2.Mes1Vlr.HasValue ? item.Trimestre2.Mes1Vlr : 0) + (item.Trimestre2.Mes2Vlr.HasValue ? item.Trimestre2.Mes2Vlr : 0) + (item.Trimestre2.Mes3Vlr.HasValue ? item.Trimestre2.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre3)
                        //            mOrcamentodaUnidadeporSubFamilia3.OrcamentoPlanejado += (item.Trimestre3.Mes1Vlr.HasValue ? item.Trimestre3.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre3.Mes2Vlr : 0) + (item.Trimestre3.Mes3Vlr.HasValue ? item.Trimestre3.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre4)
                        //            mOrcamentodaUnidadeporSubFamilia4.OrcamentoPlanejado += (item.Trimestre4.Mes1Vlr.HasValue ? item.Trimestre4.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre4.Mes2Vlr : 0) + (item.Trimestre4.Mes3Vlr.HasValue ? item.Trimestre4.Mes3Vlr : 0);
                        //    }
                        //    else
                        //    {
                        //        if (item.AtualizarTrimestre1)
                        //            mOrcamentodaUnidadeporSubFamilia1.OrcamentoParaNovosProdutos += (item.Trimestre1.Mes1Vlr.HasValue ? item.Trimestre1.Mes1Vlr : 0) + (item.Trimestre1.Mes2Vlr.HasValue ? item.Trimestre1.Mes2Vlr : 0) + (item.Trimestre1.Mes3Vlr.HasValue ? item.Trimestre1.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre2)
                        //            mOrcamentodaUnidadeporSubFamilia2.OrcamentoParaNovosProdutos += (item.Trimestre2.Mes1Vlr.HasValue ? item.Trimestre2.Mes1Vlr : 0) + (item.Trimestre2.Mes2Vlr.HasValue ? item.Trimestre2.Mes2Vlr : 0) + (item.Trimestre2.Mes3Vlr.HasValue ? item.Trimestre2.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre3)
                        //            mOrcamentodaUnidadeporSubFamilia3.OrcamentoParaNovosProdutos += (item.Trimestre3.Mes1Vlr.HasValue ? item.Trimestre3.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre3.Mes2Vlr : 0) + (item.Trimestre3.Mes3Vlr.HasValue ? item.Trimestre3.Mes3Vlr : 0);
                        //        if (item.AtualizarTrimestre4)
                        //            mOrcamentodaUnidadeporSubFamilia4.OrcamentoParaNovosProdutos += (item.Trimestre4.Mes1Vlr.HasValue ? item.Trimestre4.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre4.Mes2Vlr : 0) + (item.Trimestre4.Mes3Vlr.HasValue ? item.Trimestre4.Mes3Vlr : 0);
                        //    }
                        //    #endregion
                        //}
                        #endregion

                        if (OrcaUnidadeSubFamiliaProd.First().AtualizarTrimestre1)
                            RepositoryService.OrcamentodaUnidadeporSubFamilia.Update(mOrcamentodaUnidadeporSubFamilia1);
                        if (OrcaUnidadeSubFamiliaProd.First().AtualizarTrimestre2)
                            RepositoryService.OrcamentodaUnidadeporSubFamilia.Update(mOrcamentodaUnidadeporSubFamilia2);
                        if (OrcaUnidadeSubFamiliaProd.First().AtualizarTrimestre3)
                            RepositoryService.OrcamentodaUnidadeporSubFamilia.Update(mOrcamentodaUnidadeporSubFamilia3);
                        if (OrcaUnidadeSubFamiliaProd.First().AtualizarTrimestre4)
                            RepositoryService.OrcamentodaUnidadeporSubFamilia.Update(mOrcamentodaUnidadeporSubFamilia4);
                    }
                    catch (Exception erro)
                    {
                        mOrcamentodaUnidade.MensagemProcessamento += erro.Message + "\r\n";
                    }
                }
                #endregion

                if (mOrcamentodaUnidade.NiveldoOrcamento == (int)Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                {
                    #region CanalSubFamilia
                    var lstOrcamentoporSubFamiliaCanal = (from x in lstOrcamentoDetalhado
                                                          group x by string.Format("{0}/{1}/{2}/{3}", x.SegmentoID, x.FamiliaID, x.SubFamiliaID, x.CanalID));

                    foreach (var OrcaCanalSubFamiliaProd in lstOrcamentoporSubFamiliaCanal)
                    {
                        try
                        {
                            #region OrcamentodoCanalSubFamilia
                            mOrcamentodoCanalporSubFamilia1 = RepositoryService.OrcamentodoCanalporSubFamilia.ObterPorSubFamiliaTrimestreCanal(OrcaCanalSubFamiliaProd.First().SubFamiliaID.Value, OrcaCanalSubFamiliaProd.First().CanalID.Value, OrcaCanalSubFamiliaProd.First().Trimestre1.Id.Value);
                            if (mOrcamentodoCanalporSubFamilia1 != null)
                            {
                                mOrcamentodoCanalporSubFamilia1.OrcamentoPlanejado = 0;
                                mOrcamentodoCanalporSubFamilia1.OrcamentoParaNovosProdutos = 0;
                            }
                            mOrcamentodoCanalporSubFamilia2 = RepositoryService.OrcamentodoCanalporSubFamilia.ObterPorSubFamiliaTrimestreCanal(OrcaCanalSubFamiliaProd.First().SubFamiliaID.Value, OrcaCanalSubFamiliaProd.First().CanalID.Value, OrcaCanalSubFamiliaProd.First().Trimestre2.Id.Value);
                            if (mOrcamentodoCanalporSubFamilia2 != null)
                            {
                                mOrcamentodoCanalporSubFamilia2.OrcamentoPlanejado = 0;
                                mOrcamentodoCanalporSubFamilia2.OrcamentoParaNovosProdutos = 0;
                            }
                            mOrcamentodoCanalporSubFamilia3 = RepositoryService.OrcamentodoCanalporSubFamilia.ObterPorSubFamiliaTrimestreCanal(OrcaCanalSubFamiliaProd.First().SubFamiliaID.Value, OrcaCanalSubFamiliaProd.First().CanalID.Value, OrcaCanalSubFamiliaProd.First().Trimestre3.Id.Value);
                            if (mOrcamentodoCanalporSubFamilia3 != null)
                            {
                                mOrcamentodoCanalporSubFamilia3.OrcamentoPlanejado = 0;
                                mOrcamentodoCanalporSubFamilia3.OrcamentoParaNovosProdutos = 0;
                            }
                            mOrcamentodoCanalporSubFamilia4 = RepositoryService.OrcamentodoCanalporSubFamilia.ObterPorSubFamiliaTrimestreCanal(OrcaCanalSubFamiliaProd.First().SubFamiliaID.Value, OrcaCanalSubFamiliaProd.First().CanalID.Value, OrcaCanalSubFamiliaProd.First().Trimestre4.Id.Value);
                            if (mOrcamentodoCanalporSubFamilia4 != null)
                            {
                                mOrcamentodoCanalporSubFamilia4.OrcamentoPlanejado = 0;
                                mOrcamentodoCanalporSubFamilia4.OrcamentoParaNovosProdutos = 0;
                            }
                            #endregion

                            foreach (var item in OrcaCanalSubFamiliaProd)
                            {
                                #region
                                if (item.ProdutoID.Value != Guid.Empty)
                                {
                                    ServiceOrcamentodoCanalporProduto.Atualizar(item);
                                    if (item.AtualizarTrimestre1)
                                        mOrcamentodoCanalporSubFamilia1.OrcamentoPlanejado += (item.Trimestre1.Mes1Vlr.HasValue ? item.Trimestre1.Mes1Vlr : 0) + (item.Trimestre1.Mes2Vlr.HasValue ? item.Trimestre1.Mes2Vlr : 0) + (item.Trimestre1.Mes3Vlr.HasValue ? item.Trimestre1.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre2)
                                        mOrcamentodoCanalporSubFamilia2.OrcamentoPlanejado += (item.Trimestre2.Mes1Vlr.HasValue ? item.Trimestre2.Mes1Vlr : 0) + (item.Trimestre2.Mes2Vlr.HasValue ? item.Trimestre2.Mes2Vlr : 0) + (item.Trimestre2.Mes3Vlr.HasValue ? item.Trimestre2.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre3)
                                        mOrcamentodoCanalporSubFamilia3.OrcamentoPlanejado += (item.Trimestre3.Mes1Vlr.HasValue ? item.Trimestre3.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre3.Mes2Vlr : 0) + (item.Trimestre3.Mes3Vlr.HasValue ? item.Trimestre3.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre4)
                                        mOrcamentodoCanalporSubFamilia4.OrcamentoPlanejado += (item.Trimestre4.Mes1Vlr.HasValue ? item.Trimestre4.Mes1Vlr : 0) + (item.Trimestre4.Mes2Vlr.HasValue ? item.Trimestre4.Mes2Vlr : 0) + (item.Trimestre4.Mes3Vlr.HasValue ? item.Trimestre4.Mes3Vlr : 0);
                                }
                                else
                                {
                                    if (item.AtualizarTrimestre1)
                                        mOrcamentodoCanalporSubFamilia1.OrcamentoParaNovosProdutos += (item.Trimestre1.Mes1Vlr.HasValue ? item.Trimestre1.Mes1Vlr : 0) + (item.Trimestre1.Mes2Vlr.HasValue ? item.Trimestre1.Mes2Vlr : 0) + (item.Trimestre1.Mes3Vlr.HasValue ? item.Trimestre1.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre2)
                                        mOrcamentodoCanalporSubFamilia2.OrcamentoParaNovosProdutos += (item.Trimestre2.Mes1Vlr.HasValue ? item.Trimestre2.Mes1Vlr : 0) + (item.Trimestre2.Mes2Vlr.HasValue ? item.Trimestre2.Mes2Vlr : 0) + (item.Trimestre2.Mes3Vlr.HasValue ? item.Trimestre2.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre3)
                                        mOrcamentodoCanalporSubFamilia3.OrcamentoParaNovosProdutos += (item.Trimestre3.Mes1Vlr.HasValue ? item.Trimestre3.Mes1Vlr : 0) + (item.Trimestre3.Mes2Vlr.HasValue ? item.Trimestre3.Mes2Vlr : 0) + (item.Trimestre3.Mes3Vlr.HasValue ? item.Trimestre3.Mes3Vlr : 0);
                                    if (item.AtualizarTrimestre4)
                                        mOrcamentodoCanalporSubFamilia4.OrcamentoParaNovosProdutos += (item.Trimestre4.Mes1Vlr.HasValue ? item.Trimestre4.Mes1Vlr : 0) + (item.Trimestre4.Mes2Vlr.HasValue ? item.Trimestre4.Mes2Vlr : 0) + (item.Trimestre4.Mes3Vlr.HasValue ? item.Trimestre4.Mes3Vlr : 0);
                                }
                                #endregion
                            }

                            if (OrcaCanalSubFamiliaProd.First().AtualizarTrimestre1)
                                RepositoryService.OrcamentodoCanalporSubFamilia.Update(mOrcamentodoCanalporSubFamilia1);
                            if (OrcaCanalSubFamiliaProd.First().AtualizarTrimestre2)
                                RepositoryService.OrcamentodoCanalporSubFamilia.Update(mOrcamentodoCanalporSubFamilia2);
                            if (OrcaCanalSubFamiliaProd.First().AtualizarTrimestre3)
                                RepositoryService.OrcamentodoCanalporSubFamilia.Update(mOrcamentodoCanalporSubFamilia3);
                            if (OrcaCanalSubFamiliaProd.First().AtualizarTrimestre4)
                                RepositoryService.OrcamentodoCanalporSubFamilia.Update(mOrcamentodoCanalporSubFamilia4);
                        }
                        catch (Exception erro)
                        {
                            mOrcamentodaUnidade.MensagemProcessamento += erro.Message + "\r\n";
                        }
                    }
                    #endregion
                }
            }
            catch (Exception erro)
            {
                //RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
                throw new ArgumentException("Intelbras.CRM2013.Domain.Servicos.OrcamentodaUnidadeService.LerPlanilha", erro);
            }
            finally
            {
                RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
            }
        }

        /// <summary>
        /// Le planilha e adiciona as linhas e colunas em um objeto OrcamentoDetalhado
        /// </summary>
        /// <param name="orcamentounidadeId"></param>
        /// <returns></returns>
        public List<OrcamentoDetalhado> lerPlanilhaRegistros(Guid orcamentounidadeId)
        {
            #region variaveis e objetos
            string diretorio = string.Empty;
            string arquivo = string.Empty;
            string sArquivo = string.Empty;
            List<OrcamentoDetalhado> lstOrcamentoDetalhado;
            #endregion

            try
            {
                Observacao mObservaocao = RepositoryService.Observacao.Obter("itbc_orcamentodaunidade", "objectid", "itbc_orcamentodaunidadeid", "itbc_orcamentodaunidadeid", orcamentounidadeId);
                sArquivo = ServiceArquivo.DownLoadArquivo(SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"), mObservaocao.NomeArquivo, mObservaocao.Body, ".xlsx");

                if (sArquivo == string.Empty)
                    throw new ArgumentException("Não foi localizado Planilha dentro do Orçamento.");

                lstOrcamentoDetalhado = ServiceArquivo.lerPlanilhaOrcamento(mObservaocao.DataCriacao, sArquivo);

                mObservaocao.Text = "Leitura da Planilha Processada com Sucesso.";
                RepositoryService.Observacao.Update(mObservaocao);
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
            finally
            {
                if (File.Exists(sArquivo))
                    File.Delete(sArquivo);
            }

            return lstOrcamentoDetalhado;
        }

        public void GerarOrcamentoManual(Guid orcamentounidadeId)
        {
            string sArquivo = string.Empty;
            Guid anexoId = Guid.Empty;
            #region variaveis e objetos
            object _missingValue = System.Reflection.Missing.Value;
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre1;
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre2;
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre3;
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre4;
            Guid tri1 = Guid.Empty;
            Guid tri2 = Guid.Empty;
            Guid tri3 = Guid.Empty;
            Guid tri4 = Guid.Empty;
            OrcamentodaUnidade mOrcamentodaUnidade;
            ParametroGlobal mParametroGlobal;
            Observacao mObservaocao;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado;
            var excel = new Excel.XLWorkbook();
            var ws = excel.Worksheets.Add("Plan1");
            //Excel.Application excel = new Excel.Application();
            //Excel.Workbook wb = null;
            //Excel.Worksheet ws;
            //Excel.Range Linha;
            //Excel.Range trimestre;
            #endregion

            try
            {
                //get orcamento unidade
                mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(orcamentounidadeId);

                if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Resumido)
                {
                    #region parametro global e arquivos
                    mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(43);
                    if (mParametroGlobal == null)
                        throw new ArgumentException("Não foi Encontrado Parametro Global para este nivel de Orçamento.");

                    mObservaocao = RepositoryService.Observacao.ObterPorParametrosGlobais(mParametroGlobal.ID.Value);
                    if (mObservaocao == null)
                        throw new ArgumentException("Não foi Encontrado a Planilha em Parametro Global, para este nivel de Orçamento.");

                    sArquivo = ServiceArquivo.DownLoadArquivo(SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"), "Orcamento_" + mOrcamentodaUnidade.Nome, mObservaocao.Body, "_Manual.xlsx");
                    #endregion

                    //buscar dados para criar registros e excel
                    try
                    {
                        lstOrcamentoDetalhado = GerarDadosOrcamentoResumidoManual(mOrcamentodaUnidade);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException("Erro ao registros, contate o administrador.");
                    }

                    /*try
                    {
                        wb = excel.Workbooks.Open(sArquivo);
                        ws = (Excel.Worksheet)wb.Worksheets.get_Item(1);
                    }
                    catch (Exception erroopenexcel)
                    {
                        throw new ArgumentException("Ocorreu um erro ao abrir o \n Excel:" + erroopenexcel.Message);
                    }*/
                    
                    #endregion

                    #region Cabeçalho Unidade/ Ano/ Detalhamento
                    ws.Cell("A1").Value = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    //Linha = (ws.Cells[1, 1] as Excel.Range);
                    ws.Cell("E1").Value = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    //Linha = (ws.Cells[1, 5] as Excel.Range);
                    //Linha.Value2 = mOrcamentodaUnidade.UnidadedeNegocio.Name;
                    ws.Cell("D2").Value = mOrcamentodaUnidade.Ano;
                    //Linha = (ws.Cells[2, 4] as Excel.Range);
                    //Linha.Value2 = mOrcamentodaUnidade.Ano;
                    ws.Cell("D3").Value = "Manual";

                    try
                    {
                        #region Monta os Guids dos Orçamentos Trimetrais
                        /*trimestre = (ws.Cells[4, 8] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre1 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre1 != null ? mOrcamentodaUnidadeporTrimestre1.ID.ToString() : Guid.NewGuid().ToString();
                        tri1 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 11] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre2 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre2 != null ? mOrcamentodaUnidadeporTrimestre2.ID.ToString() : Guid.NewGuid().ToString();
                        tri2 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 14] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre3 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre3 != null ? mOrcamentodaUnidadeporTrimestre3.ID.ToString() : Guid.NewGuid().ToString();
                        tri3 = Guid.Parse(trimestre.Value2);

                        trimestre = (ws.Cells[4, 17] as Excel.Range);
                        mOrcamentodaUnidadeporTrimestre4 = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4);
                        trimestre.Value2 = mOrcamentodaUnidadeporTrimestre4 != null ? mOrcamentodaUnidadeporTrimestre4.ID.ToString() : Guid.NewGuid().ToString();
                        tri4 = Guid.Parse(trimestre.Value2);*/
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Erro ao gerar o Id dos Trimestres, contate o administrador do sistema e verifique a versão da planilha", ex);
                    }

                    try
                    {
                        //ServiceArquivo.GerarPlanilhaManualOrcamento(mOrcamentodaUnidade, lstOrcamentoDetalhado, ws);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException("Erro ao gerar a planilha, contate o administrador", ex);
                    }

                    //wb.Save();
                    //wb.Close(Type.Missing, Type.Missing, Type.Missing);
                    //excel.Quit();

                    #region Anexa Arquivo ao orçamento
                    Lookup entidade = new Lookup(orcamentounidadeId, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidade>());
                    ServiceArquivo.AnexaArquivo(sArquivo, "Planilha Gerada com Sucesso", "Orcamento_" + mOrcamentodaUnidade.Nome + "_Manual.xlsx", entidade, out anexoId);
                    mOrcamentodaUnidade.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.OrcamentoCanalManualGeradoSucesso;
                    mOrcamentodaUnidade.AddNullProperty("MensagemProcessamento");
                    RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
                    #endregion

                    try
                    {
                        #region Criando os trimestres
                        /*if (mOrcamentodaUnidadeporTrimestre1 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidadeporTrimestre1, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.CriarManual(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 1o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1, tri1, true);


                        if (mOrcamentodaUnidadeporTrimestre2 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidadeporTrimestre2, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.CriarManual(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 2o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2, tri2, false);

                        if (mOrcamentodaUnidadeporTrimestre3 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidadeporTrimestre3, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.CriarManual(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 3o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3, tri3, false);

                        if (mOrcamentodaUnidadeporTrimestre4 != null)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidadeporTrimestre4, mOrcamentodaUnidade, lstOrcamentoDetalhado);
                        else
                            ServiceOrcamentodaUnidadeporTrimestre.CriarManual(mOrcamentodaUnidade, lstOrcamentoDetalhado, " - 4o Trimestre",
                                (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4, tri4, false);*/
                        #endregion
                    }
                    catch (Exception erroCreateTrimestre)
                    {
                        if (anexoId != Guid.Empty)
                        {
                            RepositoryService.Anexo.Delete(anexoId);
                            anexoId = Guid.Empty;
                        }

                        throw new ApplicationException("Ocorreu um erro ao gerar os trimestre deste Orçamento, favor regerar. \n Caso o erro perssista contate o Administrador do Sistema. \n " + erroCreateTrimestre.Message, erroCreateTrimestre);
                    }
                }
                else
                {
                    mOrcamentodaUnidade.MensagemProcessamento = "Nivel de Orçamento não é valido para gerar Orçamento Manual.";
                    mOrcamentodaUnidade.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ErroGerarOrcamentoCanalManual;
                    RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
                }
            }
            catch (Exception erro)
            {
                if (anexoId != Guid.Empty)
                    RepositoryService.Anexo.Delete(anexoId);

                //wb.Close(Type.Missing, Type.Missing, Type.Missing);
                //excel.Quit();

                string msgErro = "Erro:Intelbras.CRM2013.Domain.Servicos.OrcamentodaUnidadeService.GerarPlanilhaRegistros \n ";
                throw new ArgumentException(msgErro, erro);
            }
            finally
            {
                if (File.Exists(sArquivo))
                    File.Delete(sArquivo);
            }
        }

        public List<Model.OrcamentoDetalhado> GerarDadosOrcamentoResumidoManual(OrcamentodaUnidade mOrcamentoUnidade)
        {
            #region variaveis/objetos
            Model.OrcamentoDetalhado mOrcamentoDetalhado;

            List<Guid> lstIdCanal = new List<Guid>();
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<Model.OrcamentoDetalhado>();
            List<Conta> lstContas = new List<Conta>();
            bool paginar = true;
            int page = 1;
            List<Conta> lstContasPage = new List<Conta>();
            List<OrcamentodoCanalDetalhadoporProduto> LstOrcamentodoCanalDetalhadoporProduto = null;
            #endregion

            #region pega os canais de orçamento do canal pela orçamento/Retira as duplicidades dos canais
            while (paginar)
            {
                lstContasPage = RepositoryService.Conta.ListarContasOrcamentoCanal(mOrcamentoUnidade.ID.Value, page, 5000);

                if (lstContasPage.Count < 5000)
                    paginar = false;

                lstContas.AddRange(lstContasPage.ToArray());
                page++;
            }

            var lstDuplicCanal = (from c in lstContas
                                  group c by string.Format("{0}", c.ID.Value));

            foreach (var iConta in lstDuplicCanal)
                lstIdCanal.Add(iConta.First().ID.Value);

            lstContasPage = RepositoryService.Conta.ListarContasParticipantes(mOrcamentoUnidade.UnidadedeNegocio.Id, lstIdCanal);
            lstContas.AddRange(lstContasPage.ToArray());
            #endregion

            #region lista os canais para inserir e os ja existentes na base
            var lstCanalOrcamento = (from p in lstContas
                                     group p by string.Format("{0}", p.ID));

            foreach (var prodCanal in lstCanalOrcamento)
            {
                if (!lstOrcamentoDetalhado.Exists(x => x.Canal.Id == prodCanal.First().ID.Value))
                {
                    mOrcamentoDetalhado = new Model.OrcamentoDetalhado();
                    mOrcamentoDetalhado.Ano = mOrcamentoUnidade.Ano;
                    mOrcamentoDetalhado.UnidadeNegocio = mOrcamentoUnidade.UnidadedeNegocio;
                    mOrcamentoDetalhado.Canal = new Lookup(prodCanal.First().ID.Value, prodCanal.First().NomeFantasia, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentoDetalhado.StatusParticipacao = prodCanal.First().ParticipantePrograma.HasValue ? (prodCanal.First().ParticipantePrograma.Value == 993520001 ? "Sim" : (prodCanal.First().ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "";

                    lstOrcamentoDetalhado.Add(mOrcamentoDetalhado);
                }
            }
            #endregion

            #region obtem detalhamento dos produtos nos trimestres
            foreach (Model.OrcamentoDetalhado capa in lstOrcamentoDetalhado)
            {
                LstOrcamentodoCanalDetalhadoporProduto = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterDetalhadoProdutosManual(mOrcamentoUnidade.ID.Value, capa.Canal.Id);
                if (LstOrcamentodoCanalDetalhadoporProduto != null && LstOrcamentodoCanalDetalhadoporProduto.Count > 0)
                {
                    #region
                    capa.Trimestre1 = new Trimestre();
                    capa.Trimestre2 = new Trimestre();
                    capa.Trimestre3 = new Trimestre();
                    capa.Trimestre4 = new Trimestre();

                    foreach (OrcamentodoCanalDetalhadoporProduto item in LstOrcamentodoCanalDetalhadoporProduto)
                    {
                        if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
                            {
                                capa.Trimestre1.Mes1 = item.Mes;
                                capa.Trimestre1.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
                            {
                                capa.Trimestre1.Mes2 = item.Mes;
                                capa.Trimestre1.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
                            {
                                capa.Trimestre1.Mes3 = item.Mes;
                                capa.Trimestre1.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre1.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
                            {
                                capa.Trimestre2.Mes1 = item.Mes;
                                capa.Trimestre2.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
                            {
                                capa.Trimestre2.Mes2 = item.Mes;
                                capa.Trimestre2.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
                            {
                                capa.Trimestre2.Mes3 = item.Mes;
                                capa.Trimestre2.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre2.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
                            {
                                capa.Trimestre3.Mes1 = item.Mes;
                                capa.Trimestre3.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
                            {
                                capa.Trimestre3.Mes2 = item.Mes;
                                capa.Trimestre3.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
                            {
                                capa.Trimestre3.Mes3 = item.Mes;
                                capa.Trimestre3.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre3.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                        else if (item.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
                        {
                            #region
                            if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
                            {
                                capa.Trimestre4.Mes1 = item.Mes;
                                capa.Trimestre4.Mes1Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes1Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
                            {
                                capa.Trimestre4.Mes2 = item.Mes;
                                capa.Trimestre4.Mes2Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes2Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            else if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
                            {
                                capa.Trimestre4.Mes3 = item.Mes;
                                capa.Trimestre4.Mes3Qtde = item.QtdePlanejada.HasValue ? (int)item.QtdePlanejada : 0;
                                capa.Trimestre4.Mes3Vlr = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado : 0;
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            }
            #endregion

            return lstOrcamentoDetalhado;
        }

        public void ProcessarLeituraPlanilhaManual(Guid orcamentounidadeId)
        {
            OrcamentodaUnidade mOrcamentodaUnidade;
            mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(orcamentounidadeId);
            if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Resumido)
            {
                try
                {
                    #region variaveis e objetos
                    string diretorio = string.Empty;
                    string arquivo = string.Empty;
                    string sArquivo = string.Empty;
                    List<OrcamentoDetalhado> lstOrcamentoDetalhado;
                    #endregion

                    Observacao mObservaocao = RepositoryService.Observacao.Obter("itbc_orcamentodaunidade", "objectid", "itbc_orcamentodaunidadeid", "itbc_orcamentodaunidadeid", orcamentounidadeId);
                    sArquivo = ServiceArquivo.DownLoadArquivo(SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento"), mObservaocao.NomeArquivo, mObservaocao.Body, ".xlsx");

                    lstOrcamentoDetalhado = ServiceArquivo.lerPlanilhaOrcamentoManual(mObservaocao.DataCriacao, sArquivo);

                    mObservaocao.Text = "Leitura da Planilha Processada com Sucesso Manual.";
                    RepositoryService.Observacao.Update(mObservaocao);

                    foreach (OrcamentoDetalhado item in lstOrcamentoDetalhado)
                    {
                        if (item.AtualizarTrimestre1)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidade, item.Trimestre1, item.CanalID.Value);
                        if (item.AtualizarTrimestre2)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidade, item.Trimestre2, item.CanalID.Value);
                        if (item.AtualizarTrimestre3)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidade, item.Trimestre3, item.CanalID.Value);
                        if (item.AtualizarTrimestre4)
                            ServiceOrcamentodaUnidadeporTrimestre.AtualizaManual(mOrcamentodaUnidade, item.Trimestre4, item.CanalID.Value);
                    }
                }
                catch (Exception erro)
                {
                    mOrcamentodaUnidade.MensagemProcessamento = erro.Message + "\r\n";
                    RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);

                    EventLog.WriteEntry("Aplication", erro.Message, EventLogEntryType.Error, 171);
                }
            }
            else
            {
                mOrcamentodaUnidade.MensagemProcessamento = "Nivel de Orçamento não é valido para Processar o Orçamento Manual.";
                mOrcamentodaUnidade.RazaoStatusOramentoManual = (int)Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.OrcamentoCanalManualGeradoSucesso;
                RepositoryService.OrcamentodaUnidade.Update(mOrcamentodaUnidade);
            }

        }

        public void RetornoDWCapaOrcamento(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamento = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);

            if (lstOrcamento.Count == 0)
            {
                return;
            }

            DataTable dtOrcamentoUpdate = RepositoryService.OrcamentodaUnidade.ObterCapaDW(ano, lstOrcamento);


            foreach (DataRow item in dtOrcamentoUpdate.Rows)
            {
                if (item.IsNull("cd_unidade_negocio"))
                {
                    continue;
                }

                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("cd_unidade_negocio"));
                
                if (mUnidadeNegocio != null)
                {
                    var itemcapa = lstOrcamento.Find(x => x.UnidadedeNegocio.Id == mUnidadeNegocio.ID.Value);

                    if (itemcapa != null)
                    {
                        var orcamentoUnidadeUpdate = new OrcamentodaUnidade(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = itemcapa.ID,
                            OrcamentoRealizado = item.Field<decimal>("vlr")
                        };

                        RepositoryService.OrcamentodaUnidade.Update(orcamentoUnidadeUpdate);
                    }
                }
            }
        }

        #endregion

    }
}

