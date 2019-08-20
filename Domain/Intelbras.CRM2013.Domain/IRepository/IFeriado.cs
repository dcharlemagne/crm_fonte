using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.Domain.IRepository
{
   public interface IFeriado<T> : IRepository<T>, IRepositoryBase
   {
       List<Feriado> ListarPor(string estado, string cidade);
       List<Feriado> ListarPor(string estado);
       List<Feriado> ListarNacionais();
        List<Feriado> RetrieveAll();
   }
}
