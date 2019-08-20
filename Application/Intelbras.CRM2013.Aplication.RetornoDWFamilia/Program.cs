using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWFamilia
{
    class Program
    {
        #region Objetos
        OrcamentodaUnidadeporFamiliaService _ServiceOrcamentodaUnidadeporFamilia = null;
        private OrcamentodaUnidadeporFamiliaService ServiceOrcamentodaUnidadeporFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporFamilia == null)
                    _ServiceOrcamentodaUnidadeporFamilia = new OrcamentodaUnidadeporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidadeporFamilia;
            }
        }

        OrcamentodoCanalporFamiliaService _ServiceOrcamentodoCanalporFamilia = null;
        private OrcamentodoCanalporFamiliaService ServiceOrcamentodoCanalporFamilia
        {
            get
            {
                if (_ServiceOrcamentodoCanalporFamilia == null)
                    _ServiceOrcamentodoCanalporFamilia = new OrcamentodoCanalporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodoCanalporFamilia;
            }
        }

        MetadaUnidadeporFamiliaService _ServiceMetadaUnidadeporFamilia = null;
        private MetadaUnidadeporFamiliaService ServiceMetadaUnidadeporFamilia
        {
            get
            {
                if (_ServiceMetadaUnidadeporFamilia == null)
                    _ServiceMetadaUnidadeporFamilia = new MetadaUnidadeporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidadeporFamilia;
            }
        }

        MetadoCanalporFamiliaService _ServiceMetadoCanalporFamilia = null;
        private MetadoCanalporFamiliaService ServiceMetadoCanalporFamilia
        {
            get
            {
                if (_ServiceMetadoCanalporFamilia == null)
                    _ServiceMetadoCanalporFamilia = new MetadoCanalporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadoCanalporFamilia;
            }
        }

        PotencialdoKAporFamiliaService _ServicePotencialdoKAporFamilia = null;
        private PotencialdoKAporFamiliaService ServicePotencialdoKAporFamilia
        {
            get
            {
                if (_ServicePotencialdoKAporFamilia == null)
                    _ServicePotencialdoKAporFamilia = new PotencialdoKAporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoKAporFamilia;
            }
        }

        PotencialdoSupervisorporFamiliaService _ServicePotencialdoSupervisorporFamilia = null;
        private PotencialdoSupervisorporFamiliaService ServicePotencialdoSupervisorporFamilia
        {
            get
            {
                if (_ServicePotencialdoSupervisorporFamilia == null)
                    _ServicePotencialdoSupervisorporFamilia = new PotencialdoSupervisorporFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoSupervisorporFamilia;
            }
        }
        
        HistoricoComprasFamiliaService _ServiceHistoricoComprasFamilia = null;
        private HistoricoComprasFamiliaService ServiceHistoricoComprasFamilia
        {
            get
            {
                if (_ServiceHistoricoComprasFamilia == null)
                    _ServiceHistoricoComprasFamilia = new HistoricoComprasFamiliaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasFamilia;
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
            //Orçamento retirado do sistema
            //Console.WriteLine("{0} - Iniciando Atualização Familia por Orcamento", DateTime.Now);
            //ServiceOrcamentodaUnidadeporFamilia.RetornoDWTrimestreFamilia(ano, trimestre);

            //Console.WriteLine("{0} - Iniciando Atualização Familia (Canal) por Orcamento", DateTime.Now);
            //ServiceOrcamentodoCanalporFamilia.RetornoDWCanalFamilia(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Familia por Meta", DateTime.Now);
            ServiceMetadaUnidadeporFamilia.RetornoDWMetaFamilia(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Familia (Canal) por Meta", DateTime.Now);
            ServiceMetadoCanalporFamilia.RetornoDWMetaCanalFamilia(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Familia (KA) por Meta", DateTime.Now);
            ServicePotencialdoKAporFamilia.RetornoDWKaFamilia(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Familia (Supervisor) por Meta", DateTime.Now);
            ServicePotencialdoSupervisorporFamilia.RetornoDWFamilia(ano, trimestre);

            Console.WriteLine("{0} - Iniciando Atualização Familia por Historico Compra", DateTime.Now);
            ServiceHistoricoComprasFamilia.RetornoDWHistoricoCompraFamilia(ano, trimestre);

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
