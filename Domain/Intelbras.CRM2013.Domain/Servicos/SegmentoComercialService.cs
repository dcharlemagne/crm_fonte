using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SegmentoComercialService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SegmentoComercialService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public SegmentoComercialService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        public SegmentoComercialService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        public SegmentoComercial ObterPor(Guid seguimentoId)
        {
            return RepositoryService.SegmentoComercial.ObterPor(seguimentoId);
        }
        public List<SegmentoComercial> ListarTodos()
        {
            return RepositoryService.SegmentoComercial.ListarTodos();
        }
        public List<SegmentoComercial> ListarSegmentoPorConta(string codigoConta)
        {
            return RepositoryService.SegmentoComercial.ListarSegmentoPorConta(codigoConta);
        }
        public void Desativar(Guid questinarioId)
        {
            //status 1 = desativar. 
            RepositoryService.SegmentoComercial.AlterarStatus(questinarioId, 1);
        }
        public void Ativar(Guid questinarioId)
        {
            //status 0 = ativar. 
            RepositoryService.SegmentoComercial.AlterarStatus(questinarioId, 0);
        }
        public SegmentoComercial ObterPorCodigo(string codigoSeg)
        {
            return RepositoryService.SegmentoComercial.ObterPorCodigo(codigoSeg);
        }
        public void DesassociarSegmentoComercial(List<SegmentoComercial> listaAtual, Guid conta)
        {
            RepositoryService.SegmentoComercial.DesassociarSegmentoComercial(listaAtual, conta);
        }
        public void AssociarSegmentoComercial(List<SegmentoComercial> listaSegmentoComercial, Guid conta)
        {
            RepositoryService.SegmentoComercial.AssociarSegmentoComercial(listaSegmentoComercial, conta);
        }

}
}
