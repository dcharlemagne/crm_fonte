using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWSubFamilia
{
    class Program
    {
        #region Objetos
        OrcamentodaUnidadeporSubFamiliaService _ServiceOrcamentodaUnidadeporSubFamilia = null;
        private OrcamentodaUnidadeporSubFamiliaService ServiceOrcamentodaUnidadeporSubFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSubFamilia == null)
                    _ServiceOrcamentodaUnidadeporSubFamilia = new OrcamentodaUnidadeporSubFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidadeporSubFamilia;
            }
        }

        OrcamentodoCanalporSubFamiliaService _ServiceOrcamentodoCanalporSubFamilia = null;
        private OrcamentodoCanalporSubFamiliaService ServiceOrcamentodoCanalporSubFamilia
        {
            get
            {
                if (_ServiceOrcamentodoCanalporSubFamilia == null)
                    _ServiceOrcamentodoCanalporSubFamilia = new OrcamentodoCanalporSubFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodoCanalporSubFamilia;
            }
        }

        MetadaUnidadeporSubfamiliaService _ServiceMetadaUnidadeporSubfamilia = null;
        private MetadaUnidadeporSubfamiliaService ServiceMetadaUnidadeporSubfamilia
        {
            get
            {
                if (_ServiceMetadaUnidadeporSubfamilia == null)
                    _ServiceMetadaUnidadeporSubfamilia = new MetadaUnidadeporSubfamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidadeporSubfamilia;
            }
        }

        MetadoCanalporSubFamiliaService _ServiceMetadoCanalporSubFamilia = null;
        private MetadoCanalporSubFamiliaService ServiceMetadoCanalporSubFamilia
        {
            get
            {
                if (_ServiceMetadoCanalporSubFamilia == null)
                    _ServiceMetadoCanalporSubFamilia = new MetadoCanalporSubFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadoCanalporSubFamilia;
            }
        }

        PotencialdoKAporSubfamiliaService _ServicePotencialdoKAporSubfamilia = null;
        private PotencialdoKAporSubfamiliaService ServicePotencialdoKAporSubfamilia
        {
            get
            {
                if (_ServicePotencialdoKAporSubfamilia == null)
                    _ServicePotencialdoKAporSubfamilia = new PotencialdoKAporSubfamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoKAporSubfamilia;
            }
        }

        PotencialdoSupervisorporSubfamiliaService _ServicePotencialdoSupervisorporSubfamilia = null;
        private PotencialdoSupervisorporSubfamiliaService ServicePotencialdoSupervisorporSubfamilia
        {
            get
            {
                if (_ServicePotencialdoSupervisorporSubfamilia == null)
                    _ServicePotencialdoSupervisorporSubfamilia = new PotencialdoSupervisorporSubfamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoSupervisorporSubfamilia;
            }
        }


        HistoricoComprasSubfamiliaService _ServiceHistoricoComprasSubfamilia = null;
        private HistoricoComprasSubfamiliaService ServiceHistoricoComprasSubfamiliaService
        {
            get
            {
                if (_ServiceHistoricoComprasSubfamilia == null)
                    _ServiceHistoricoComprasSubfamilia = new HistoricoComprasSubfamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasSubfamilia;
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
            Console.WriteLine("{0} - Iniciando Atualização SubFamilia por Historico Compra", DateTime.Now);
            ServiceHistoricoComprasSubfamiliaService.RetornoDWHistoricoCompraSubfamilia(ano, trimestre);
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
