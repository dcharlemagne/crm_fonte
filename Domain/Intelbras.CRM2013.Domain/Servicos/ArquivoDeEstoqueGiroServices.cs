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
    public class ArquivoDeEstoqueGiroServices
    {
         #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ArquivoDeEstoqueGiroServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ArquivoDeEstoqueGiroServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ArquivoDeEstoqueGiroServices(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public ArquivoDeEstoqueGiro Persistir(Model.ArquivoDeEstoqueGiro ObjArquivoDeEstoqueGiro)
        {

            Model.ArquivoDeEstoqueGiro TmpArquivoDeEstoqueGiro = null; 

            if (ObjArquivoDeEstoqueGiro.ID.HasValue)
                TmpArquivoDeEstoqueGiro = RepositoryService.ArquivoDeEstoqueGiro.Retrieve(ObjArquivoDeEstoqueGiro.ID.Value);
      
            if (TmpArquivoDeEstoqueGiro != null)
            {
                ObjArquivoDeEstoqueGiro.ID = TmpArquivoDeEstoqueGiro.ID;

                RepositoryService.ArquivoDeEstoqueGiro.Update(ObjArquivoDeEstoqueGiro);

                //Altera Status - Se necessário
                if (!TmpArquivoDeEstoqueGiro.Status.Equals(ObjArquivoDeEstoqueGiro.Status) && ObjArquivoDeEstoqueGiro.Status != null)
                    this.MudarSituacaoArquivoDeEstoqueGiro(ObjArquivoDeEstoqueGiro.ID.Value, ObjArquivoDeEstoqueGiro.Status.Value);

                return ObjArquivoDeEstoqueGiro;
            }
            else
                ObjArquivoDeEstoqueGiro.ID = RepositoryService.ArquivoDeEstoqueGiro.Create(ObjArquivoDeEstoqueGiro);
            return ObjArquivoDeEstoqueGiro;
        }

        public bool MudarStatusArquivoDeEstoqueGiro(Guid id, int status)
        {
            return RepositoryService.ArquivoDeEstoqueGiro.AlterarStatus(id, status);
        }

        public bool MudarSituacaoArquivoDeEstoqueGiro(Guid id, int status)
        {
            return RepositoryService.ArquivoDeEstoqueGiro.AlterarSituacao(id, status);
        }

        public List<ArquivoDeEstoqueGiro> ListarPor(Guid? Canal, int? RazaoDoStatus, DateTime? DataDeEnvioInicio, DateTime? DataDeEnvioFim)
        {
            return RepositoryService.ArquivoDeEstoqueGiro.ListarPor(Canal, RazaoDoStatus, DataDeEnvioInicio, DataDeEnvioFim);
        }
    }
}
