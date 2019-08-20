using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IDenuncia<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(Guid denunciaId);
        Boolean AlterarStatus(Guid denunciaId, int status);
        List<T> ListarDenuncias(DateTime dtInicio, DateTime dtFim, List<Guid> lstDenunciantes, List<Guid> lstDenunciados, Guid? representanteId, int? situacaoDenuncia);
    }
}
