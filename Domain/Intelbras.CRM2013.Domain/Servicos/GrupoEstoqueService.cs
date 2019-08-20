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
    public class GrupoEstoqueService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public GrupoEstoqueService(string organizacao, bool isOffline) : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public GrupoEstoqueService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public GrupoEstoqueService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        public GrupoEstoque Persistir(GrupoEstoque ObjGrupoEstoque)
        {
            GrupoEstoque TmpGrupoEstoque = null;
            if (ObjGrupoEstoque.Codigo.HasValue)
            {
                TmpGrupoEstoque = RepositoryService.GrupoEstoque.ObterPor(ObjGrupoEstoque.Codigo.Value);

                if (TmpGrupoEstoque != null)
                {
                    ObjGrupoEstoque.ID = TmpGrupoEstoque.ID;

                    RepositoryService.GrupoEstoque.Update(ObjGrupoEstoque);

                    if (!TmpGrupoEstoque.Status.Equals(ObjGrupoEstoque.Status) && ObjGrupoEstoque.Status != null)
                        this.MudarStatus(TmpGrupoEstoque.ID.Value, ObjGrupoEstoque.Status.Value);

                    return TmpGrupoEstoque;
                }
                else
                {
                    ObjGrupoEstoque.ID = RepositoryService.GrupoEstoque.Create(ObjGrupoEstoque);
                    return ObjGrupoEstoque;
                }
            }
            else
            {
                return null;
            }
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            int statusCode;
            if (stateCode == 0)
            {
                //Ativar
                statusCode = 1;
            }
            else
            {
                //Inativar
                statusCode = 2;
            }

            return RepositoryService.GrupoEstoque.AlterarStatus(id, stateCode, statusCode);
        }

        public GrupoEstoque ObterPor(Guid grupoEstoqueId)
        {
            return RepositoryService.GrupoEstoque.ObterPor(grupoEstoqueId);
        }

        public GrupoEstoque ObterPor(int codigoEstoque)
        {
            return RepositoryService.GrupoEstoque.ObterPor(codigoEstoque);
        }

        public List<GrupoEstoque> ListarGrupoEstoqueParaMeta()
        {

            var tipoParametroGlobal = Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta;
            var lista = new List<GrupoEstoque>();

            var parametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)tipoParametroGlobal);

            if (parametroGlobal == null || string.IsNullOrEmpty(parametroGlobal.Valor))
            {
                throw new ArgumentException("(CRM) Não foi encontrado Parametro Global [" + (int)tipoParametroGlobal + "].");
            }

            var codigosGrupoEstoque = parametroGlobal.Valor.Split(';');

            return RepositoryService.GrupoEstoque.ListarPor(codigosGrupoEstoque);
        }
    }
}
