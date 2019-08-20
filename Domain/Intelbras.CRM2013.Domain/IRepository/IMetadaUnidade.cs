using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IMetadaUnidade<T> : IRepository<T>, IRepositoryBase
    {
        MetadaUnidade ObterValoresPor(Guid unidadeNegocioId, int ano);
        List<T> ObterLerGerarPlanilha(int status);
        List<T> ListarPor(Enum.MetaUnidade.RazaodoStatusMetaManual status);
        List<T> ListarPor(Enum.MetaUnidade.RazaodoStatusMetaKARepresentante status);
        List<T> ObterLerGerarPlanilhaSupervisor(int status);
        List<T> ListarMetas(int ano);
        List<T> ListarMetas(Guid unidadenegocioId, int ano);
        DataTable ObterCapaDW(int ano, List<MetadaUnidade> lstMetasUnidade);
    }
}