using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MoedaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MoedaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public MoedaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public Moeda BuscaMoedaPorNome(String nomeMoeda)
        {
            List<Moeda> lstMoeda = RepositoryService.Moeda.ListarPor(nomeMoeda);
            if (lstMoeda.Count > 0)
                return lstMoeda.First<Moeda>();
            return null;
        }

        public Guid Persistir(Moeda moeda, String codigoMoeda)
        {
            List<Moeda> TmpMoeda = RepositoryService.Moeda.ListarPor(codigoMoeda);

            if (TmpMoeda.Count() > 0)
            {
                moeda.ID = TmpMoeda.First<Moeda>().ID;
                RepositoryService.Moeda.Update(moeda);
                return TmpMoeda.First<Moeda>().ID.Value;
            }
            else
                return RepositoryService.Moeda.Create(moeda);
        }

        public Moeda BuscaMoedaPorCodigo(String isocurrencycode)
        {
            Moeda ObjMoeda = RepositoryService.Moeda.ObterPor(isocurrencycode);
            if (ObjMoeda != null)
                return ObjMoeda;
            return null;
        }
    }
}
