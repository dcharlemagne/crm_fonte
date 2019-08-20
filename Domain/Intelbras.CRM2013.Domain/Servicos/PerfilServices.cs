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
    public class PerfilServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PerfilServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public PerfilServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos Pedido

        public void ConsultaBeneficioCompromisso(Guid Perfil)
        {
            List<BeneficiosCompromissos> pedido = RepositoryService.BeneficioCompromisso.ListarPor(Perfil);

            if (pedido.Count() == 0)
                throw new ArgumentException("(CRM) Não existe nenhum Beneficio x Compromisso para esse Pefil.");
        }

        public Perfil BuscarPerfil(Guid? Classificacao, Guid? UnidadeNeg, Guid? Categoria, Boolean? exclusividade)
        {
            return RepositoryService.Perfil.ListarPorConfigurado(Classificacao, UnidadeNeg, Categoria, exclusividade).FirstOrDefault();
        }

        #endregion
    }
}
