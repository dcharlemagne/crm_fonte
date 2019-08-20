using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IArquivoDeSellOut<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid? Canal, int? RazaoDoStatus, DateTime? DataDeEnvioInicio, DateTime? DataDeEnvioFim);
        Boolean AlterarRazaoStatus(Guid ArquivoDeSelloutId, int razaoStatus);
        Boolean AlterarSituacao(Guid ArquivoDeSelloutId, int status);       

    }
}
