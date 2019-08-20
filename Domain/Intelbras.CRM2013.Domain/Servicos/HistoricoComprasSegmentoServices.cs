using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System.Data;
using System.Threading;
using System.Diagnostics;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasSegmentoServices
    {
        #region Atributos

        private static bool _isOffline = false;
        public static bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private static string _nomeDaOrganizacao = "";
        public static string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public static object Provider { get; set; }

        #endregion
        
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasSegmentoServices(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public HistoricoComprasSegmentoServices(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos
        //public HistoricoCompra ObterPor(Guid UnidadeNeg,Int32 HistoricoTrimestre,Guid canal)
        //{
        //    return RepositoryService.HistoricoComprasCanal.ObterPor(UnidadeNeg, HistoricoTrimestre, canal).FirstOrDefault();
        //}
        #endregion
    }
}
