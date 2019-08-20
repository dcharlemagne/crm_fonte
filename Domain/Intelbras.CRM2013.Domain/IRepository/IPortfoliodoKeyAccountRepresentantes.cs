
using System;
using System.Collections.Generic;
using SDKore.DomainModel;
namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPortfoliodoKeyAccountRepresentantes<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(Guid contatoId, Guid unidadeNegocioId, Guid segmentoId, Guid supervisorId, Guid assintenteId);
        List<T> ListarPorRepresentanteECodigos(Guid usuario);
        T ObterPor(Guid unidadeNegocioId, Guid contatoId);
        List<T> ListarPor(Guid? unidadeNegocioId, int? tipo);
        List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipo);
        T ObterPor(Guid portfolioId);
        List<T> ListarPorUnidadeNegocioEsegmentoVazio(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid unidadeNegocioId);
        List<T> ListarPorUnidadeNegocioEsegmentoNaoVazio(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid unidadeNegocioId);
        List<T> ListarPorUnidadeNegocioEsegmento(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid segmentoId, Guid unidadeNegocioId);
        List<T> ListarPorCodigoRepresentante(Guid unId, string codigodorepresentante);
        List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId);
    }
}

