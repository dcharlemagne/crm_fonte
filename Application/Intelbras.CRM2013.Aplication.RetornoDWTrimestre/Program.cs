using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWTrimestre
{
    class Program
    {
        #region Objetos
        OrcamentodaUnidadeporTrimestreService _ServiceOrcamentodaUnidadeporTrimestre = null;
        private OrcamentodaUnidadeporTrimestreService ServiceOrcamentodaUnidadeporTrimestre
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporTrimestre == null)
                    _ServiceOrcamentodaUnidadeporTrimestre = new OrcamentodaUnidadeporTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidadeporTrimestre;
            }
        }

        MetadaUnidadeporTrimestreService _ServiceMetadaUnidadeporTrimestre = null;
        private MetadaUnidadeporTrimestreService ServiceMetadaUnidadeporTrimestre
        {
            get
            {
                if (_ServiceMetadaUnidadeporTrimestre == null)
                    _ServiceMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidadeporTrimestre;
            }
        }

        HistoricoComprasTrimestreService _ServiceHistoricoComprasTrimestre = null;
        private HistoricoComprasTrimestreService ServiceHistoricoComprasTrimestre
        {
            get
            {
                if (_ServiceHistoricoComprasTrimestre == null)
                    _ServiceHistoricoComprasTrimestre = new HistoricoComprasTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceHistoricoComprasTrimestre;
            }
        }


        PotencialdoKAporTrimestreService _ServicePotencialdoKAporTrimestre = null;
        private PotencialdoKAporTrimestreService ServicePotencialdoKAporTrimestre
        {
            get
            {
                if (_ServicePotencialdoKAporTrimestre == null)
                    _ServicePotencialdoKAporTrimestre = new PotencialdoKAporTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoKAporTrimestre;
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
            Console.WriteLine("{0} - Iniciando Atualização Trimestre por Historico Compra", DateTime.Now);

            ServiceHistoricoComprasTrimestre.RetornoDWHistoricoCompraTrimestre(ano, trimestre);

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