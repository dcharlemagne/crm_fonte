using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IArquivoDeEstoqueGiro<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? Canal, int? RazaoDoStatus, DateTime? DataDeEnvioInicio, DateTime? DataDeEnvioFim);
        Boolean AlterarStatus(Guid ArquivoDeEstoqueGiroId, int status);
        Boolean AlterarSituacao(Guid ArquivoDeEstoqueGiroId, int status);       
    }
}
