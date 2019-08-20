using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface ICadastraProdutos<T> : IRepository<T>, IRepositoryBase
    {
        T CadastraProdutos(string distribuidor, string vendedor, string numeroSerie, string keyCode);        
    }
}
