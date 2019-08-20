using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasCanalService
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

        public HistoricoComprasCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public HistoricoComprasCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Repositórios De Domínio


        #endregion
        #region Métodos

        public void RetornoDWHistoricoCompraCanal(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraCanal = RepositoryService.HistoricoComprasCanal.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraCanal.Rows)
            {
                Conta canal = RepositoryService.Conta.Retrieve(new Guid(item["CD_guid"].ToString()));

                if (canal == null)
                    continue;

                UnidadeNegocio unidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());

                if (unidadeNegocio == null)
                    continue;

                HistoricoCompraCanal historicoCompraCanal = RepositoryService.HistoricoCompraCanal.ObterPor(unidadeNegocio.ID.Value, trimestre, ano, canal.ID.Value);

                if (historicoCompraCanal != null)
                {
                    historicoCompraCanal.Valor = decimal.Parse(item["Valor"].ToString());

                    HistoricoComprasTrimestre hsTrimestre = RepositoryService.HistoricoComprasTrimestre
                        .ObterPor(unidadeNegocio.ID.Value, historicoCompraCanal.Ano.Value, historicoCompraCanal.Trimestre.Value);

                    if (hsTrimestre != null)
                    {
                        historicoCompraCanal.TrimestreRelacionamento = new Lookup(hsTrimestre.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasCanal.Update(historicoCompraCanal);
                }
                else
                {
                    HistoricoCompraCanal hsHistoricoCompraCanal = new HistoricoCompraCanal(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);

                    hsHistoricoCompraCanal.Canal = new Lookup(canal.ID.Value, "");
                    hsHistoricoCompraCanal.UnidadeNegocio = new Lookup(unidadeNegocio.ID.Value, "");
                    hsHistoricoCompraCanal.Valor = decimal.Parse(item["Valor"].ToString());
                    hsHistoricoCompraCanal.Ano = ano;
                    hsHistoricoCompraCanal.Trimestre = trimestre;
                    hsHistoricoCompraCanal.Nome = canal.NomeFantasia + " - " + unidadeNegocio.Nome + " - " + ano;

                    HistoricoComprasTrimestre hsTrimestre = RepositoryService.HistoricoComprasTrimestre
                        .ObterPor(unidadeNegocio.ID.Value, hsHistoricoCompraCanal.Ano.Value, hsHistoricoCompraCanal.Trimestre.Value);

                    if (hsTrimestre != null)
                    {
                        hsHistoricoCompraCanal.TrimestreRelacionamento = new Lookup(hsTrimestre.ID.Value, "");
                    }

                    RepositoryService.HistoricoCompraCanal.Create(hsHistoricoCompraCanal);
                }
            }
        }

        #endregion

    }
}