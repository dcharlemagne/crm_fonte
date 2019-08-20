using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MunicipioServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MunicipioServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public MunicipioServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Municipio BuscaCidade(String ChaveIntegracao)
        {
            List<Municipio> lstCidade = RepositoryService.Municipio.ListarPor(ChaveIntegracao);
            if (lstCidade.Count > 0)
                return lstCidade.First<Municipio>();
            return null;
        }


        public Municipio ObterCidadePorEstadoIdNomeCidade(Guid estadiId, string nomeCidade)
        {
            if (!string.IsNullOrEmpty(nomeCidade))
            {
                return RepositoryService.Municipio.ObterPor(estadiId, nomeCidade);
            }
            else
            {
                return null;
            }
        }

        public Municipio ObterPor(Guid municipioId)
        {
            return RepositoryService.Municipio.ObterPor(municipioId);
        }

        #endregion

    }
}
