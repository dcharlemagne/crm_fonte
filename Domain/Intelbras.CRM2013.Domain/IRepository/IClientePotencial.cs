using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IClientePotencial<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPorEmail(string email);

        T ObterPorNumeroProjeto(string numeroprojeto);
        List<T> ListarPor(Revenda revenda);
        List<T> ListarProjetosPor(string codigoRevenda, string codigoDistribuidor, string codigoExecutivo, string cNPJCliente, int? situacaoProjeto, string codigoSegmento, string CodigoUnidadeNegocio);

        List<T> ListarPor(string CNPJ);

        int ObterUltimoNumeroProjeto(string Ano);

        List<T> ListarProjetosDuplicidade(String CNPJCliente, String UnidadeNegocio);
        void EnviaEmailRegistroProjeto(ClientePotencial clientepotencial, bool duplicado);
    }
}
