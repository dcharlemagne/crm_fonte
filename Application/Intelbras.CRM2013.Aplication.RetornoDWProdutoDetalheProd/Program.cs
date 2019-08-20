using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWProdutoDetalheProd
{
    class Program
    {
        #region Objetos

        MetadaUnidadeporProdutoService _ServiceMetadaUnidadeporProduto = null;
        private MetadaUnidadeporProdutoService ServiceMetadaUnidadeporProduto
        {
            get
            {
                if (_ServiceMetadaUnidadeporProduto == null)
                    _ServiceMetadaUnidadeporProduto = new MetadaUnidadeporProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidadeporProduto;
            }
        }

        MetadoCanalporProdutoService _ServiceMetadoCanalporProduto = null;
        private MetadoCanalporProdutoService ServiceMetadoCanalporProduto
        {
            get
            {
                if (_ServiceMetadoCanalporProduto == null)
                    _ServiceMetadoCanalporProduto = new MetadoCanalporProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadoCanalporProduto;
            }
        }

        MetaDetalhadadaUnidadeporProdutoService _ServiceMetaDetalhadadaUnidadeporProduto = null;
        private MetaDetalhadadaUnidadeporProdutoService ServiceMetaDetalhadadaUnidadeporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadaUnidadeporProduto == null)
                    _ServiceMetaDetalhadadaUnidadeporProduto = new MetaDetalhadadaUnidadeporProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetaDetalhadadaUnidadeporProduto;
            }
        }

        MetaDetalhadadoCanalporProdutoService _ServiceMetaDetalhadadoCanalporProduto = null;
        private MetaDetalhadadoCanalporProdutoService ServiceMetaDetalhadadoCanalporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadoCanalporProduto == null)
                    _ServiceMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetaDetalhadadoCanalporProduto;
            }
        }
        
        HistoricoComprasProdutoService _ServiceHistoricoComprasProduto = null;
        private HistoricoComprasProdutoService ServiceHistoricoComprasProduto
        {
            get
            {
                if (_ServiceHistoricoComprasProduto == null)
                    _ServiceHistoricoComprasProduto = new HistoricoComprasProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasProduto;
            }
        }

        HistoricoComprasProdutoMesService _ServiceHistoricoComprasProdutoMes = null;
        private HistoricoComprasProdutoMesService ServiceHistoricoComprasProdutoMes
        {
            get
            {
                if (_ServiceHistoricoComprasProdutoMes == null)
                    _ServiceHistoricoComprasProdutoMes = new HistoricoComprasProdutoMesService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasProdutoMes;
            }
        }

        HistoricoComprasCanalProdutoMesService _ServiceHistoricoComprasCanalProdutoMes = null;
        private HistoricoComprasCanalProdutoMesService ServiceHistoricoComprasCanalProdutoMes
        {
            get
            {
                if (_ServiceHistoricoComprasCanalProdutoMes == null)
                    _ServiceHistoricoComprasCanalProdutoMes = new HistoricoComprasCanalProdutoMesService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasCanalProdutoMes;
            }
        }

        HistoricoComprasCanalService _ServiceHistoricoComprasCanal = null;
        private HistoricoComprasCanalService ServiceHistoricoComprasCanal
        {
            get
            {
                if (_ServiceHistoricoComprasCanal == null)
                    _ServiceHistoricoComprasCanal = new HistoricoComprasCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasCanal;
            }
        }


        #endregion

        static int Main(string[] args)
        {
            try
            {
                int ano, trimestre;

                if (args != null && args.Length > 1)
                {
                    ano = int.Parse(args[0]);
                    int trimestreParce = int.Parse(args[1]);
                    trimestre = (int)ConvertTrimestre(trimestreParce);
                }
                else
                {
                    ano = DateTime.Now.Year;
                    trimestre = Helper.TrimestreAtual()[1];
                }

                new Program().Inicio(ano, trimestre);
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);

                Console.WriteLine("{0} - ERRO: {1}", DateTime.Now, mensagem);
                return ex.GetHashCode();
            }
        }

        public void Inicio(int ano, int trimestre)
        {
        //    Console.WriteLine("{0} - Iniciando Atualização Produto por Meta", DateTime.Now);
        //    ServiceMetadaUnidadeporProduto.RetornoDWMetaProduto(ano, trimestre);

        //    Console.WriteLine("{0} - Iniciando Atualização Produto Mês por Meta", DateTime.Now);
        //    ServiceMetaDetalhadadaUnidadeporProduto.RetornoDWMetaProdutoDetalhado(ano, trimestre);

            //Console.WriteLine("{0} - Iniciando Atualização Produto (Canal) por Meta", DateTime.Now);
            //ServiceMetadoCanalporProduto.RetornoDWMetaCanalProduto(ano, trimestre);

            //Console.WriteLine("{0} - Iniciando Atualização Produto Mês (Canal) por Meta", DateTime.Now);
            //ServiceMetaDetalhadadoCanalporProduto.RetornoDWMetaCanalProdutoDetalhado(ano, trimestre);

            //Console.WriteLine("{0} - Iniciando Atualização Produto (Meta Manual do Canal) por Meta", DateTime.Now);
            //ServiceMetaDetalhadadoCanalporProduto.RetornoDWMetaCanalManualProdutoDetalhado(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Produto por Histotico Compra", DateTime.Now);
            ServiceHistoricoComprasProduto.RetornoDWHistoricoCompraProduto(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Produto Mês por Histotico Compra", DateTime.Now);
            ServiceHistoricoComprasProdutoMes.RetornoDWHistoricoCompraProdutoMes(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Produto Mês (Canal) por Histotico Compra", DateTime.Now);
            ServiceHistoricoComprasCanalProdutoMes.RetornoDWHistoricoCompraCanalProdutoMes(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Produto (Canal) por Histotico Compra", DateTime.Now);
            ServiceHistoricoComprasCanal.RetornoDWHistoricoCompraCanal(ano, trimestre);

            Console.WriteLine("{0} - Finalizando processo", DateTime.Now);
        }

        private static Domain.Enum.OrcamentodaUnidade.Trimestres ConvertTrimestre(int value)
        {
            switch (value)
            {
                case 1:
                    return Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                case 2:
                    return Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                case 3:
                    return Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                case 4:
                    return Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                default:
                    throw new ArgumentException("(CRM) O trimestre aceita apenas o seguintes valores: 1, 2, 3 e 4");
            }
        }
    }
}
