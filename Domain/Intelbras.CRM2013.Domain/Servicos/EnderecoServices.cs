using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class EnderecoServices
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EnderecoServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public EnderecoServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Estado
        /// <summary>
        /// Busca Estado
        /// </summary>
        /// <param name="ChaveIntegracao">NomePais,SiglaEstado</param>
        /// <returns></returns>
        public Estado BuscaEstado(String ChaveIntegracao)
        {
            List<Estado> lstEstado = RepositoryService.Estado.ListarPor(ChaveIntegracao);
            if (lstEstado.Count > 0)
                return lstEstado.First<Estado>();
            return null;
        }

        /// <summary>
        /// Busca Estado
        /// </summary>
        /// <param name="ChaveIntegracao">NomePais,SiglaEstado</param>
        /// <returns></returns>
        public Estado BuscaEstadoPorGuid(Guid itbc_estadoid)
        {
            Estado objEstado = RepositoryService.Estado.ObterPor(itbc_estadoid);
            if (objEstado != null)
                return objEstado;
            return null;
        }
        /// <summary>
        /// Persistir Estado
        /// </summary>
        /// <param name="objEstado">Objeto Estado</param>
        /// <param name="ChaveIntegracao">NomePais , SiglaEstado</param>
        /// <returns>Guid</returns>
        public Estado Persistir(Model.Estado objEstado)
        {
            List<Model.Estado> TmpEstado = RepositoryService.Estado.ListarPor(objEstado.ChaveIntegracao);

            if (TmpEstado.Count() > 0)
            {
                objEstado.ID = TmpEstado.First<Estado>().ID;
                RepositoryService.Estado.Update(objEstado);
                //Altera Status - Se necessário
                if (!TmpEstado.First<Estado>().Status.Equals(objEstado.Status) && objEstado.Status != null)
                    this.MudarStatusEstado(TmpEstado.First<Estado>().ID.Value, objEstado.Status.Value);

                return TmpEstado.First<Estado>();
            }
            else
                objEstado.ID = RepositoryService.Estado.Create(objEstado);
            return objEstado;
        }

        public bool MudarStatusEstado(Guid id, int status)
        {
            return RepositoryService.Estado.AlterarStatus(id, status);
        }

        #endregion

        #region RegiaoGeo

        public Itbc_regiaogeo BuscaRegiaoGeo(Guid regiao)
        {
            List<Itbc_regiaogeo> lstRegiao = RepositoryService.RegiaoGeografica.ListarPor(regiao);
            if (lstRegiao.Count > 0)
                return lstRegiao.First<Itbc_regiaogeo>();
            return null;
        }

        /// <summary>
        /// Persistir Regiao Geografica
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Itbc_regiaogeo Persistir(Model.Itbc_regiaogeo objRegiaoGeo)
        {

            List<Model.Itbc_regiaogeo> TmpRegiaoGeo = RepositoryService.RegiaoGeografica.ListarPor(objRegiaoGeo.Nome);

            if (TmpRegiaoGeo.Count() > 0)
            {
                objRegiaoGeo.ID = TmpRegiaoGeo.First<Itbc_regiaogeo>().ID;

                RepositoryService.RegiaoGeografica.Update(objRegiaoGeo);

                //Altera Status - Se necessário
                if (!TmpRegiaoGeo.First<Itbc_regiaogeo>().Status.Equals(objRegiaoGeo.Status) && objRegiaoGeo.Status != null)
                    this.MudarStatusPais(TmpRegiaoGeo.First<Itbc_regiaogeo>().ID.Value, objRegiaoGeo.Status.Value);

                return TmpRegiaoGeo.First<Itbc_regiaogeo>();
            }
            else
                objRegiaoGeo.ID = RepositoryService.RegiaoGeografica.Create(objRegiaoGeo);
            return objRegiaoGeo;
        }

        public bool MudarStatusRegiaoGeo(Guid id, int status)
        {
            return RepositoryService.RegiaoGeografica.AlterarStatus(id, status);
        }

        public string IntegracaoBarramento(Itbc_regiaogeo regiaoGeografica)
        {
            Domain.Integracao.MSG0008 msgregiaoGeo = new Domain.Integracao.MSG0008(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgregiaoGeo.Enviar(regiaoGeografica);
        }

        #endregion

        #region País
        /// <summary>
        /// Busca Pais
        /// </summary>
        /// <param name="ChaveIntegracao">NomePais</param>
        /// <returns></returns>
        public Pais BuscaPais(String ChaveIntegracao)
        {
            List<Pais> lstPais = RepositoryService.Pais.ListarPor(ChaveIntegracao);
            if (lstPais.Count > 0)
                return lstPais.First<Pais>();
            return null;
        }

        public Pais Persistir(Model.Pais ObjPais)
        {

            List<Model.Pais> TmpPais = RepositoryService.Pais.ListarPor(ObjPais.ChaveIntegracao);

            if (TmpPais.Count() > 0)
            {
                ObjPais.ID = TmpPais.First<Pais>().ID;
                RepositoryService.Pais.Update(ObjPais);
                //Altera Status - Se necessário
                if (!TmpPais.First<Pais>().State.Equals(ObjPais.State) && ObjPais.State != null)
                    this.MudarStatusPais(TmpPais.First<Pais>().ID.Value, ObjPais.State.Value);

                return TmpPais.First<Pais>();
            }
            else
                ObjPais.ID = RepositoryService.Pais.Create(ObjPais);
            return ObjPais;
        }


        public bool MudarStatusPais(Guid id, int status)
        {
            return RepositoryService.Pais.AlterarStatus(id, status);
        }
        #endregion

        #region Municipio
        /// <summary>
        /// Busca Municipio por chave integração
        /// </summary>
        /// <param name="ChaveIntegracao">NomePais</param>
        /// <returns></returns>
        /// 
        public Municipio BuscaMunicipio(String ChaveIntegracao)
        {
            List<Municipio> lstMunicipio = RepositoryService.Municipio.ListarPor(ChaveIntegracao);
            if (lstMunicipio.Count > 0)
                return lstMunicipio.First<Municipio>();
            return null;
        }

        public Municipio ObterMunicipio(Guid municipioID)
        {
            Municipio municipio = RepositoryService.Municipio.ObterPor(municipioID);
            if (municipio != null)
                return municipio;
            return null;
        }

        public Municipio Persistir(Model.Municipio objMunicipio)
        {
            var temp = RepositoryService.Municipio.ObterPor(objMunicipio.ChaveIntegracao, "itbc_municipiosid");

            if (temp != null)
            {
                objMunicipio.ID = temp.ID;
                RepositoryService.Municipio.Update(objMunicipio);

                if (!temp.State.Equals(objMunicipio.State) && objMunicipio.State != null)
                {
                    this.MudarStatusMunicipio(temp.ID.Value, objMunicipio.State.Value);
                }

                return temp;
            }
            else
            {
                objMunicipio.ID = RepositoryService.Municipio.Create(objMunicipio);
            }
            
            return objMunicipio;
        }

        public bool MudarStatusMunicipio(Guid id, int status)
        {
            return RepositoryService.Municipio.AlterarStatus(id, status);
        }

        #endregion

        #region CEP
        public CepViewModel BuscaCep(string cep)
        {
            var mensagemCep = new Domain.Integracao.MSG0001(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            CepViewModel enderecoCep = mensagemCep.Enviar(cep.GetOnlyNumbers());

            if (enderecoCep != null)
                {
                    if (enderecoCep.CodigoIBGE.HasValue)
                    {
                        IbgeViewModel ibgeViewModel = RepositoryService.Municipio.ObterIbgeViewModelPor(enderecoCep.CodigoIBGE.Value);

                        if (ibgeViewModel != null)
                        {
                            enderecoCep.Municipio = new SDKore.DomainModel.Lookup()
                            {
                                Id = ibgeViewModel.CidadeId,
                                Name = ibgeViewModel.CidadeNome,
                                Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                            };

                            enderecoCep.Estado = new SDKore.DomainModel.Lookup()
                            {
                                Id = ibgeViewModel.EstadoId,
                                Name = ibgeViewModel.EstadoNome,
                                Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Estado>()
                            };

                            enderecoCep.Pais = new SDKore.DomainModel.Lookup()
                            {
                                Id = ibgeViewModel.PaisId,
                                Name = ibgeViewModel.PaisNome,
                                Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Pais>()
                            };
                        }
                    }
                }
                else
                {
                    enderecoCep = null;
                }

            return enderecoCep;
        }

        #endregion
    }
}
