using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IExtratoFidelidade<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ObterExtratoPontosValidos(Guid contatoId);
        List<T> ObterExtratoPontosValidosPorDistribuidor(Guid distribuidorId);
        List<T> ObterExtratoPontosDoDistribuidor(Guid distribuidorId, Guid userId);
        List<T> ObterExtratoPontosAExpirar(int pagina, int quantidade);
        List<T> ObterExtratoPontosAExpirar(Guid contatoId, int qtdDias);
        List<T> ObterExtratoPontosAExpirarDaRevenda(int qtdDias, Guid revenda);
        List<T> ObterExtratoPontosExpirados(Guid contatoId, int qtdDias);
        List<T> ObterExtratosContato(Guid contatoId);
        List<T> ObterVendedoresPontuadosPorDistribuidor(Guid revenda, Guid distribuidorId);
        List<T> ObterPontosCliente(Guid clienteId);
        List<T> ObterExtratoPorNumeroSerie(string numeroSerie);
        List<T> ObterNaoValidadosAtivos(DateTime aPartiDe, int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords);
        List<T> ObterInvalidosAtivos(int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords);
        void AlterarStatus(Guid id, int statuscode, bool stateActive);
        void Associar(ExtratoFidelidade extrato, Domain.Model.Resgate resgate);
    }
}
