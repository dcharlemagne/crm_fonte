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
    public class OrigemService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrigemService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrigemService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public Origem BuscaOrigem(String codigoOrigem)
        {
            List<Origem> lstOrigem = RepositoryService.Origem.ListarPor(codigoOrigem);
            if (lstOrigem.Count > 0)
                return lstOrigem.First<Origem>();
            return null;
        }

        public Guid Persistir(Origem origem)
        {
            List<Origem> TmpOrigem = RepositoryService.Origem.ListarPor(origem.Codigo);

            if (TmpOrigem.Count() > 0)
            {
                origem.ID = TmpOrigem.First<Origem>().ID;
                RepositoryService.Origem.Update(origem);
                //Altera Status - Se necessário
                if (!TmpOrigem.First<Origem>().Equals(origem.Status) && origem.Status != null)
                    this.MudarStatus(TmpOrigem.First<Origem>().ID.Value, origem.Status.Value);

                return TmpOrigem.First<Origem>().ID.Value;
            }
            else
                return RepositoryService.Origem.Create(origem);
        }

        public Origem ObterPor(Guid origemId)
        {
            return RepositoryService.Origem.ObterPor(origemId);
        }


        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Origem.AlterarStatus(id, status);
        }
    }
}
