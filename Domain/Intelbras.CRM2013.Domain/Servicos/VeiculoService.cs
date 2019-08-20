using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class VeiculoService
    {

        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public VeiculoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public VeiculoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public VeiculoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public List<Veiculo> Persistir(List<Veiculo> lstVeiculo)
        {
            List<Veiculo> veiculosTemp = new List<Veiculo>();
            foreach (var veiculo in lstVeiculo)
            {
                var retornoItem = Persistir(veiculo);
                if (retornoItem == null)
                {
                    throw new ArgumentException("(CRM) Erro de Persistência no Veículo!");
                }
                veiculosTemp.Add(retornoItem);
            }

            return veiculosTemp;
        }
        public Veiculo Persistir(Veiculo objVeiculo)
        {
            Veiculo TmpProdProjeto = null;
            if (objVeiculo.ID.HasValue)
                TmpProdProjeto = ObterPor(objVeiculo.ID.Value);

            if (TmpProdProjeto != null)
            {
                RepositoryService.Veiculo.Update(objVeiculo);
                return TmpProdProjeto;
            }
            else
                objVeiculo.ID = RepositoryService.Veiculo.Create(objVeiculo);
            return objVeiculo;
        }

        public Veiculo ObterPor(Guid veiculoId, params string[] columns)
        {
            return RepositoryService.Veiculo.Retrieve(veiculoId, columns);
        }

        public List<Veiculo> ListarPorClienteParticipanteContrato(Guid clienteParticipanteId)
        {
            return RepositoryService.Veiculo.ListarPorClienteParticipanteContrato(clienteParticipanteId);
        }

        public List<Veiculo> ListarPorClientesParticipantesContrato(List<ClienteParticipante> lstClienteParticipante, string placa)
        {
            return RepositoryService.Veiculo.ListarPorClientesParticipantesContrato(lstClienteParticipante, placa);
        }

        public Veiculo ObterPorPlacaVeiculo(String numeroplaca)
        {
            return RepositoryService.Veiculo.ObterPorPlacaVeiculo(numeroplaca);
        }

        #endregion
    }
}
