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
    public class CanalVerdeService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CanalVerdeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public CanalVerdeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public CanalVerde Persistir(CanalVerde objCanalVerde)
        {
            if (objCanalVerde.ID.HasValue)
            {
                CanalVerde tmpCanalVerde = RepositoryService.CanalVerde.ObterPor(objCanalVerde.ID.Value);

                if (tmpCanalVerde != null)
                {
                    tmpCanalVerde.ID = tmpCanalVerde.ID;

                    RepositoryService.CanalVerde.Update(tmpCanalVerde);

                    //Altera Status - Se necessário
                    if (tmpCanalVerde.Status.HasValue && !tmpCanalVerde.Status.Equals(objCanalVerde.Status))
                        this.MudarStatus(tmpCanalVerde.ID.Value, objCanalVerde.Status.Value);

                    return tmpCanalVerde;
                }
                else
                {
                    objCanalVerde.ID = RepositoryService.CanalVerde.Create(tmpCanalVerde);
                    return objCanalVerde;
                }
            }
            else
            {
                objCanalVerde.ID = RepositoryService.CanalVerde.Create(objCanalVerde);
                return objCanalVerde;
            }
        }

        public string[] listarPorCanal(Guid canalGuid)
        {
            var list = RepositoryService.CanalVerde.ListarPorCanal(canalGuid);

            return list.Select(x => x.FamiliaProduto.Id.ToString()).ToArray();
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.CanalVerde.AlterarStatus(id, status);
        }

        public List<CanalVerde> listarPorSegmento(Guid segmentoGuid)
        {
            return RepositoryService.CanalVerde.ListarPorSegmento(segmentoGuid);
        }

        public List<CanalVerde> listarPorConta(Guid contaGuid)
        {
            return RepositoryService.CanalVerde.ListarPorCanal(contaGuid);
        }

        public List<CanalVerde> listarPorContaTodos(Guid contaGuid)
        {
            return RepositoryService.CanalVerde.ListarPorCanalTodos(contaGuid);
        }

        #endregion
    }
}
