using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ConfiguracaoBeneficioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ConfiguracaoBeneficioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ConfiguracaoBeneficioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        public ConfiguracaoBeneficio ObterPor(Guid ConfiguracaoBeneficioId)
        {
            return RepositoryService.ConfiguracaoBeneficio.ObterPor(ConfiguracaoBeneficioId);
        }

        public ConfiguracaoBeneficio Persistir(ConfiguracaoBeneficio configuracaoBenef)
        {
            if (configuracaoBenef.Produto != null)
            {
                ConfiguracaoBeneficio TmpConfiguracaoBenef = RepositoryService.ConfiguracaoBeneficio.ObterPor(configuracaoBenef.Produto.Id);

                if (TmpConfiguracaoBenef != null)
                {
                    configuracaoBenef.ID = TmpConfiguracaoBenef.ID;

                    RepositoryService.ConfiguracaoBeneficio.Update(configuracaoBenef);

                   if (!TmpConfiguracaoBenef.Status.Equals(configuracaoBenef.Status) && configuracaoBenef.Status != null)
                        this.MudarStatus(TmpConfiguracaoBenef.ID.Value, configuracaoBenef.Status.Value);

                    return configuracaoBenef;
                }
                else
                {
                    configuracaoBenef.ID = RepositoryService.ConfiguracaoBeneficio.Create(configuracaoBenef);
                }
            }
            else
                return null;

            return configuracaoBenef;
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            return RepositoryService.ConfiguracaoBeneficio.AlterarStatus(id, stateCode);
        }
    }
}
