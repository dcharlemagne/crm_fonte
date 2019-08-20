using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class CepService
    {

        private string cep = "";

        public CepService(string cep)
        {
            this.cep = cep;
        }

        public Model.Endereco Pesquisar()
        {
            RepositoryService RepositoryService = new RepositoryService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
            return RepositoryService.Endereco.PesquisarEnderecoPor(this.cep);
        }
   }
}
