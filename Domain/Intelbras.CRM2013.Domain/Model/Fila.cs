using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("queue")]
    public class Fila : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Fila() { }

        public Fila(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Fila(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("queueid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("name")]
        public String Nome { get; set; }

        #endregion

        public Fila FilaPadraoFidelidade
        {
            get
            {
                var fila = GerenciadorCache.Get("FILAPADRAOFIDELIDADE") as Fila;
                if (fila == null)
                {
                    fila = (new CRM2013.Domain.Servicos.RepositoryService()).Fila.ObterPor(SDKore.Configuration.ConfigurationManager.GetSettingValue("FilaFidelidade"));
                    if (fila == null)
                        throw new ArgumentException("Fila destino não configurada.");

                    GerenciadorCache.Add("FILAPADRAOFIDELIDADE", fila, TimeSpan.FromHours(12));
                }

                return fila;
            }
        }

        public Fila FilaPublicaUsuarioAdministrador
        {
            get
            {
                var fila = GerenciadorCache.Get("FilaPublicaUsuarioAdministrador") as Fila;
                if (fila == null)
                {
                    fila = (new CRM2013.Domain.Servicos.RepositoryService()).Fila.ObterFilaPublicaPor(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_CRMADMIN")));
                    if (fila != null)
                        GerenciadorCache.Add("FilaPublicaUsuarioAdministrador", fila, TimeSpan.FromHours(12));
                }

                return fila;
            }
        }
    }
}
