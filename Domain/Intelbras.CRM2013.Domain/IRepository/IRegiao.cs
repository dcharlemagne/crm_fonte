﻿using System;
using System.Collections.Generic;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IRegiao<T> : IRepository<T>, IRepositoryBase
    {
        List<T> ListarPor(string nome);
        T ObterPor(string nome);
    }
}
