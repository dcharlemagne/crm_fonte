using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICNAE<T> : IRepository<T>, IRepositoryBase
    {
        T ObterPor(string classe);
    }
}
