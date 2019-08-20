using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWSegmento
{
    class Program
    {
        #region Objetos
        OrcamentodaUnidadeporSegmentoService _ServiceOrcamentodaUnidadeporSegmento = null;
        private OrcamentodaUnidadeporSegmentoService ServiceOrcamentodaUnidadeporSegmento
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSegmento == null)
                    _ServiceOrcamentodaUnidadeporSegmento = new OrcamentodaUnidadeporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidadeporSegmento;
            }
        }

        OrcamentodoCanalporSegmentoService _ServiceOrcamentodoCanalporSegmento = null;
        private OrcamentodoCanalporSegmentoService ServiceOrcamentodoCanalporSegmento
        {
            get
            {
                if (_ServiceOrcamentodoCanalporSegmento == null)
                    _ServiceOrcamentodoCanalporSegmento = new OrcamentodoCanalporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodoCanalporSegmento;
            }
        }

        MetadaUnidadeporSegmentoService _ServiceMetadaUnidadeporSegmento = null;
        private MetadaUnidadeporSegmentoService ServiceMetadaUnidadeporSegmento
        {
            get
            {
                if (_ServiceMetadaUnidadeporSegmento == null)
                    _ServiceMetadaUnidadeporSegmento = new MetadaUnidadeporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidadeporSegmento;
            }
        }

        MetadoCanalporSegmentoService _ServiceMetadoCanalporSegmento = null;
        private MetadoCanalporSegmentoService ServiceMetadoCanalporSegmento
        {
            get
            {
                if (_ServiceMetadoCanalporSegmento == null)
                    _ServiceMetadoCanalporSegmento = new MetadoCanalporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadoCanalporSegmento;
            }
        }

        PotencialdoKAporSegmentoService _ServicePotencialdoKAporSegmento = null;
        private PotencialdoKAporSegmentoService ServicePotencialdoKAporSegmento
        {
            get
            {
                if (_ServicePotencialdoKAporSegmento == null)
                    _ServicePotencialdoKAporSegmento = new PotencialdoKAporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoKAporSegmento;
            }
        }

        PotencialdoSupervisorporSegmentoService _ServicePotencialdoSupervisorporSegmento = null;
        private PotencialdoSupervisorporSegmentoService ServicePotencialdoSupervisorporSegmento
        {
            get
            {
                if (_ServicePotencialdoSupervisorporSegmento == null)
                    _ServicePotencialdoSupervisorporSegmento = new PotencialdoSupervisorporSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoSupervisorporSegmento;
            }
        }

        HistoricoCompraSegmentoService _ServiceHistoricoCompraSegmentoService = null;
        private HistoricoCompraSegmentoService ServiceHistoricoCompraSegmentoService
        {
            get
            {
                if (_ServiceHistoricoCompraSegmentoService == null)
                    _ServiceHistoricoCompraSegmentoService = new HistoricoCompraSegmentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoCompraSegmentoService;
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
            Console.WriteLine("{0} - Iniciando Atualização Segmento por Historico Compra", DateTime.Now);
            ServiceHistoricoCompraSegmentoService.AtualizarFaturamentoDoSegmento(ano, trimestre);
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