using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoCompraUnidadeService
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

        public HistoricoCompraUnidadeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public HistoricoCompraUnidadeService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos
     
        public void RetornoDWHistoricoCompraUnidade(int ano)
        {            
            DataTable dtHistoricoCompra = RepositoryService.HistoricoCompra.ListarPor(ano.ToString());

            foreach (DataRow item in dtHistoricoCompra.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio"))
                {
                    continue;
                }

                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("CD_Unidade_Negocio"));

                if (mUnidadeNegocio == null)
                {
                    continue;
                }

                HistoricoCompra historicoCompra = RepositoryService.HistoricoCompra.ObterPor(mUnidadeNegocio.ID.Value, ano, "itbc_historicocomprasdaunidadeid");
                
                decimal valorDW = item.Field<decimal>("Valor");

                if (valorDW < 0)
                {
                    valorDW = 0;
                }
                
                if (historicoCompra != null)
                {
                    historicoCompra.Valor = valorDW;
                    RepositoryService.HistoricoCompra.Update(historicoCompra);
                }
                else
                {
                    HistoricoCompra hsComp = new HistoricoCompra(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    hsComp.UnidadeNegocio = new Lookup(mUnidadeNegocio.ID.Value, "");
                    hsComp.Ano = ano;
                    hsComp.Nome = mUnidadeNegocio.Nome + " - " + ano;
                    hsComp.Valor = valorDW;
                    RepositoryService.HistoricoCompra.Create(hsComp);
                }

            }
        }
      
        #endregion
    }
}