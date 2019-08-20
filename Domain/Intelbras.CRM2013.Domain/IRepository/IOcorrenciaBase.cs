using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IOcorrenciaBase<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(string numeroDaOcorrencia);
        List<T> ListarPorStatus(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorStatusDiagnostico(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, string[] statusDiagnostico, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNumeroSerie(string numeroSerie, Model.Conta assistenciaTecnica, string[] status, string[] origem);
        List<T> ListarPorCpfCnpjCliente(string cpfCnpj, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNomeCliente(string nomeCliente, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarPorNotaFiscalCompra(string notaFical, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Model.Conta assistenciaTecnica, string[] origem);
        List<T> ListarOcorrenciaAbertaComIntervencaoTecnica(Model.Conta assistenciaTecnica);
        List<T> ListarPor(Extrato extrato, Model.Conta assistenciaTecnica = null);
    }
}