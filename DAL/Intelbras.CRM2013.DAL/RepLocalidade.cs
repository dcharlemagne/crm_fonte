using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepLocalidade<T> : CrmServiceRepository<T>, ILocalidade<T>
    {
        
        #region Metodos Publicos

        public Localidade PesquisarEnderecoPor(string cep)
        {
            Localidade localidade = null;

            var row = new Buscar_DadosCep_ttRetornoCEPRow[1];
            cep = cep.Replace("-", "");

            //try
            //{
            Domain.Servicos.HelperWS.IntelbrasService.Buscar_DadosCep(cep, out row);
            //}
            //catch (WebException ex)
            //{
            //    string log = String.Format("Erro no acesso do WS Buscar_DadosCep, Message: {0}", ex.Message);
            //    EventLog.WriteEntry("CRM Application", log, EventLogEntryType.Error);
            //}

            if (null != row && row.Length > 0)
                localidade = InstanciarEndereco(row[0], cep);

            return localidade;
        }

        internal Localidade InstanciarEndereco(Buscar_DadosCep_ttRetornoCEPRow row, string cep)
        {
            Localidade localidade = null;

            if (null != row)
            {
                // CEP
                localidade = new Localidade(this.OrganizationName, this.IsOffline);
                localidade.Cep = cep.Trim();

                // Pais
                localidade.Pais = new Pais(this.OrganizationName, this.IsOffline);
                localidade.Pais = (new Domain.Servicos.RepositoryService()).Pais.PesquisarPaisPor("Brasil");

                // Logradouro
                if (!string.IsNullOrEmpty(row.Endereco))
                {
                    localidade.Logradouro = row.Endereco;
                }

                // Bairro
                if (!string.IsNullOrEmpty(row.Bairro))
                {
                    localidade.Bairro = row.Bairro;
                }

                // Cidade
                if (row.ibge.HasValue && row.ibge.Value > 0)
                {
                    Municipio cidade = new Municipio(this.OrganizationName, this.IsOffline);
                    localidade.Cidade = (new Domain.Servicos.RepositoryService()).Municipio.ObterPor(row.ibge.Value);
                    if (localidade.Cidade != null)
                        localidade.Cidade.ZonaFranca = row.CidadeZF.HasValue;
                }

                // UF
                if (!string.IsNullOrEmpty(row.UF))
                {
                    Estado uf = new Estado(this.OrganizationName, this.IsOffline);
                    localidade.Uf = uf.PesquisarPor(row.UF);
                }

            }

            return localidade;
        }
        
        #endregion
    }
}
