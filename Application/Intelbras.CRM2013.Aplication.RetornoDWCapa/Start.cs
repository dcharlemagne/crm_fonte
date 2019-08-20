using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Aplication.RetornoDWCapa
{
    public class Start
    {
        #region Objetos
        OrcamentodaUnidadeService _ServiceOrcamentodaUnidade = null;
        private OrcamentodaUnidadeService ServiceOrcamentodaUnidade
        {
            get
            {
                if (_ServiceOrcamentodaUnidade == null)
                    _ServiceOrcamentodaUnidade = new OrcamentodaUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodaUnidade;
            }
        }

        OrcamentodoCanalService _ServiceOrcamentodoCanal = null;
        private OrcamentodoCanalService ServiceOrcamentodoCanal
        {
            get
            {
                if (_ServiceOrcamentodoCanal == null)
                    _ServiceOrcamentodoCanal = new OrcamentodoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceOrcamentodoCanal;
            }
        }

        MetadaUnidadeService _ServiceMetadaUnidade = null;
        private MetadaUnidadeService ServiceMetadaUnidade
        {
            get
            {
                if (_ServiceMetadaUnidade == null)
                    _ServiceMetadaUnidade = new MetadaUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadaUnidade;
            }
        }

        MetadoCanalService _ServiceMetadoCanal = null;
        private MetadoCanalService ServiceMetadoCanal
        {
            get
            {
                if (_ServiceMetadoCanal == null)
                    _ServiceMetadoCanal = new MetadoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceMetadoCanal;
            }
        }

        PotencialdoKARepresentanteService _ServicePotencialdoKARepresentante = null;
        private PotencialdoKARepresentanteService ServicePotencialdoKARepresentante
        {
            get
            {
                if (_ServicePotencialdoKARepresentante == null)
                    _ServicePotencialdoKARepresentante = new PotencialdoKARepresentanteService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoKARepresentante;
            }
        }

        PotencialdoSupervisorService _ServicePotencialdoSupervisor = null;
        private PotencialdoSupervisorService ServicePotencialdoSupervisor
        {
            get
            {
                if (_ServicePotencialdoSupervisor == null)
                    _ServicePotencialdoSupervisor = new PotencialdoSupervisorService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServicePotencialdoSupervisor;
            }
        }

        HistoricoCompraUnidadeService _HistoricoComprasUnidade = null;
        private HistoricoCompraUnidadeService HistoricoComprasUnidade
        {
            get
            {
                if (_HistoricoComprasUnidade == null)
                    _HistoricoComprasUnidade = new HistoricoCompraUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _HistoricoComprasUnidade;
            }
        }


        #endregion

        static int Main(string[] args)
        {
            try
            {
                int ano;

                if (args != null && args.Length == 1)
                {
                    ano = int.Parse(args[0]);
                }
                else
                {
                    ano = DateTime.Now.Year;
                }

                new Start().Inicio(ano);
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);

                Console.WriteLine("{0} - ERRO: {1}", DateTime.Now, mensagem);
                return ex.GetHashCode();
            }
        }

        public void Inicio(int ano)
        {
            Console.WriteLine("{0} - Iniciando Atualização Unidade por Histórico de Compra", DateTime.Now);
            HistoricoComprasUnidade.RetornoDWHistoricoCompraUnidade(ano);

            Console.WriteLine("{0} - Iniciando Atualização Unidade por Meta - Representante", DateTime.Now);
            ServicePotencialdoKARepresentante.RetornoDWKaRepresentante(ano);

            Console.WriteLine("{0} - Finalizando processo", DateTime.Now);
        }
    }
}