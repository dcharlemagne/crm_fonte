using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasTrimestreService
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

        public HistoricoComprasTrimestreService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public HistoricoComprasTrimestreService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        public void RetornoDWHistoricoCompraTrimestre(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraTrimestre = RepositoryService.HistoricoComprasTrimestre.ListarPor(ano.ToString(), trimestre.ToString());

            Console.WriteLine("{0} - Existem {1} para ser atualizados", DateTime.Now, dtHistoricoCompraTrimestre.Rows.Count);

            foreach (DataRow item in dtHistoricoCompraTrimestre.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio"))
                {
                    continue;
                }

                UnidadeNegocio metaUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("CD_Unidade_Negocio"));

                if (metaUnidadeNegocio == null)
                {
                    continue;
                }

                decimal valor = item.IsNull("Valor") ? 0 : item.Field<decimal>("Valor");
                HistoricoCompra hsCompra = RepositoryService.HistoricoCompra.ObterPor(metaUnidadeNegocio.ID.Value, ano);
                HistoricoComprasTrimestre historicoCompraTrimestre = RepositoryService.HistoricoComprasTrimestre.ObterPor(metaUnidadeNegocio.ID.Value, ano, trimestre);

                if (historicoCompraTrimestre != null)
                {
                    var historicoCompraTrimestreUpdate = new HistoricoComprasTrimestre(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline)
                    {
                        ID = historicoCompraTrimestre.ID,
                        Valor = valor
                    };

                    if (hsCompra != null)
                    {
                        historicoCompraTrimestreUpdate.HistoricoCompraUnidade = new Lookup(hsCompra.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasTrimestre.Update(historicoCompraTrimestreUpdate);
                }
                else
                {

                    HistoricoComprasTrimestre hsCompTri = new HistoricoComprasTrimestre(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompTri.UnidadeNegocio = new Lookup(metaUnidadeNegocio.ID.Value, "");
                    hsCompTri.Ano = ano;
                    hsCompTri.Trimestre = trimestre;
                    hsCompTri.Valor = valor;
                    hsCompTri.Nome = string.Format("{0} - {1} o trimestre - {2}", metaUnidadeNegocio.Nome, trimestre, ano).Truncate(100);

                    if (hsCompra != null)
                    {
                        hsCompTri.HistoricoCompraUnidade = new Lookup(hsCompra.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasTrimestre.Create(hsCompTri);
                }
            }
        }
    }
}