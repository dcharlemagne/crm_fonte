using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Intelbras.CRM2013.Aplication.DWCapaTrimestre
{
    class Program
    {
        #region Objetos
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

        OrcamentodaUnidadeService _ServiceoOrcamentodaUnidade = null;
        private OrcamentodaUnidadeService ServiceoOrcamentodaUnidade
        {
            get
            {
                if (_ServiceoOrcamentodaUnidade == null)
                    _ServiceoOrcamentodaUnidade = new OrcamentodaUnidadeService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceoOrcamentodaUnidade;
            }
        }
        
        MetadoCanalService _ServiceoMetadoCanal = null;
        private MetadoCanalService ServiceoMetadoCanal
        {
            get
            {
                if (_ServiceoMetadoCanal == null)
                    _ServiceoMetadoCanal = new MetadoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceoMetadoCanal;
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



        OrcamentodaUnidadeporTrimestreService _ServiceoOrcamentodaUnidadeporTrimestre = null;
        private OrcamentodaUnidadeporTrimestreService ServiceoOrcamentodaUnidadeporTrimestre
        {
            get
            {
                if (_ServiceoOrcamentodaUnidadeporTrimestre == null)
                    _ServiceoOrcamentodaUnidadeporTrimestre = new OrcamentodaUnidadeporTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceoOrcamentodaUnidadeporTrimestre;
            }
        }

        MetadaUnidadeporTrimestreService _ServiceoMetadaUnidadeporTrimestre = null;
        private MetadaUnidadeporTrimestreService ServiceoMetadaUnidadeporTrimestre
        {
            get
            {
                if (_ServiceoMetadaUnidadeporTrimestre == null)
                    _ServiceoMetadaUnidadeporTrimestre = new MetadaUnidadeporTrimestreService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceoMetadaUnidadeporTrimestre;
            }
        }

        #endregion

        static void Main(string[] args)
        {
        }
    }
}
