using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class LinhaCorteService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public LinhaCorteService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public LinhaCorteService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public List<LinhaCorteEstado> ObterLinhaDeCorteDistribuidorEstadoPorIdDistribuidor(Guid linhaCorteDistribuidorId)
        {
            return RepositoryService.LinhaCorteDistribuidorEstado.ListarPor(linhaCorteDistribuidorId, null);
        }

        public object ObterLinhaCorte(Guid? unidNeg, Guid? estado, Guid? categoria, string tipoLinhaCorte)
        {
            object objRetorno = new object();
            switch (tipoLinhaCorte)
            {
                case "distribuidor":
                    List<Guid> lstUniNeg = new List<Guid>();
                    lstUniNeg.Add(unidNeg.Value);
                    objRetorno = RepositoryService.LinhaCorteDistribuidor.ListarPor(lstUniNeg, estado).FirstOrDefault();
                    break;
                case "revenda":
                    List<Guid> lstUniNegRev = new List<Guid>();
                    lstUniNegRev.Add(unidNeg.Value);
                    objRetorno = RepositoryService.LinhaCorteRevenda.ListarPor(lstUniNegRev, categoria.Value).FirstOrDefault();
                    break;
                default:
                    return null;
            }
            return objRetorno;
        }

        public List<LinhaCorteDistribuidor> ListarLinhadeCorteDistribuidor(List<Guid> UnidadeNegocio, Estado estado, int? capitalOuInterior)
        {
            List<LinhaCorteDistribuidor> lstRetorno = new List<LinhaCorteDistribuidor>();
            LinhaCorteDistribuidor tmpObj = new LinhaCorteDistribuidor(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            if (UnidadeNegocio == null && estado != null)
            {
                List<LinhaCorteEstado> linhaCorteEstado = RepositoryService.LinhaCorteDistribuidorEstado.ListarPor(estado.ID.Value);
                foreach (LinhaCorteEstado item in linhaCorteEstado ?? Enumerable.Empty<LinhaCorteEstado>())
                {
                    tmpObj = RepositoryService.LinhaCorteDistribuidor.ObterPor(item.ID.Value);

                    if (capitalOuInterior.HasValue)
                    {
                        if (tmpObj != null && capitalOuInterior == tmpObj.CapitalOuInterior)
                        {
                            lstRetorno.Add(tmpObj);
                        }
                    }
                    //else
                    //{
                    //    if (tmpObj != null)
                    //    {
                    //        lstRetorno.Add(tmpObj);
                    //    }
                    //}
                }
                return lstRetorno;
            }
            else if (UnidadeNegocio != null && estado == null)
            {
                List<LinhaCorteDistribuidor> lstLinhaCorte = RepositoryService.LinhaCorteDistribuidor.ListarPor(UnidadeNegocio, null);

                foreach (LinhaCorteDistribuidor item in lstLinhaCorte)
                {
                    lstRetorno.Add(item);

                }
                return lstRetorno;
            }
            else if (UnidadeNegocio != null && estado != null)
            {
                List<LinhaCorteEstado> linhaCorteEstado = RepositoryService.LinhaCorteDistribuidorEstado.ListarPor(estado.ID.Value);
                foreach (LinhaCorteEstado item in linhaCorteEstado ?? Enumerable.Empty<LinhaCorteEstado>())
                {
                    tmpObj = RepositoryService.LinhaCorteDistribuidor.ObterPor(item.LinhaCorteDistribuidor.Value);
                    if (tmpObj != null && tmpObj.UnidadeNegocios != null)
                        if (UnidadeNegocio.Contains(tmpObj.UnidadeNegocios.Id))
                        {
                            if (capitalOuInterior.HasValue)
                            {
                                if (tmpObj.CapitalOuInterior == capitalOuInterior)
                                    lstRetorno.Add(tmpObj);
                            }
                            //else
                            //{
                            //    lstRetorno.Add(tmpObj);
                            //}
                        }
                }
                return lstRetorno;
            }
            else
                return null;
        }

        public List<LinhaCorteRevenda> ListarLinhadeCorteRevenda(List<Guid> UnidadeNegocio, Categoria CategoriaLinha)
        {
            List<LinhaCorteRevenda> lstRetorno = new List<LinhaCorteRevenda>();
            if (CategoriaLinha != null)
                lstRetorno = RepositoryService.LinhaCorteRevenda.ListarPor(UnidadeNegocio, CategoriaLinha.ID.Value);
            else
                lstRetorno = RepositoryService.LinhaCorteRevenda.ListarPor(UnidadeNegocio, null);

            return lstRetorno;
        }

        #endregion

    }
}
