using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Net;
using System.Xml;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ArquivoDeSellOutServices
    {
         #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ArquivoDeSellOutServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ArquivoDeSellOutServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ArquivoDeSellOutServices(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public ArquivoDeSellOut Persistir(Model.ArquivoDeSellOut ObjArquivoDeSellOut)
        {

            Model.ArquivoDeSellOut TmpArquivoDeSellOut = null; 

            if (ObjArquivoDeSellOut.ID.HasValue)
                TmpArquivoDeSellOut = RepositoryService.ArquivoDeSellOut.Retrieve(ObjArquivoDeSellOut.ID.Value);
      
            if (TmpArquivoDeSellOut != null)
            {
                ObjArquivoDeSellOut.ID = TmpArquivoDeSellOut.ID;

                RepositoryService.ArquivoDeSellOut.Update(ObjArquivoDeSellOut);

                //Altera Status - Se necessário
                if (!TmpArquivoDeSellOut.RazaoStatus.Equals(ObjArquivoDeSellOut.RazaoStatus) && ObjArquivoDeSellOut.RazaoStatus != null)
                    this.MudarRazaoStatusArquivoDeSellOut(ObjArquivoDeSellOut.ID.Value, ObjArquivoDeSellOut.RazaoStatus.Value);

                return ObjArquivoDeSellOut;
            }
            else
                ObjArquivoDeSellOut.ID = RepositoryService.ArquivoDeSellOut.Create(ObjArquivoDeSellOut);
            return ObjArquivoDeSellOut;
        }

        public bool MudarRazaoStatusArquivoDeSellOut(Guid id, int razaoStatus)
        {
            return RepositoryService.ArquivoDeSellOut.AlterarRazaoStatus(id, razaoStatus);
        }

        public bool MudarSituacaoArquivoDeSellOut(Guid id, int status)
        {
            return RepositoryService.ArquivoDeSellOut.AlterarSituacao(id, status);
        }

        public List<ArquivoDeSellOut> ListarPor(Guid? Canal, int? RazaoDoStatus, DateTime? DataDeEnvioInicio, DateTime? DataDeEnvioFim)
        {
            return RepositoryService.ArquivoDeSellOut.ListarPor(Canal, RazaoDoStatus, DataDeEnvioInicio, DataDeEnvioFim);
        }
    }
}
