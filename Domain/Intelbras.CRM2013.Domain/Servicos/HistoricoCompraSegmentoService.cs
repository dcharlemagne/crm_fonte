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
    public class HistoricoCompraSegmentoService
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

        public HistoricoCompraSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public HistoricoCompraSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public void AtualizarFaturamentoDoSegmento(int ano, int trimestre)
        {            
            DataTable dtHistoricoCompraSegmento = RepositoryService.HistoricoComprasSegmento.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraSegmento.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                if (!mUnidadeNegocio.ID.HasValue)
                    continue;
                
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                if (mSegmento == null)
                    continue;

                HistoricoComprasSegmento historicoSegmento = RepositoryService.HistoricoComprasSegmento.ObterPor(mUnidadeNegocio.ID.Value,mSegmento.ID.Value, ano, trimestre.ToString());

                if (historicoSegmento != null)
                {
                        historicoSegmento.Valor = decimal.Parse(item["Valor"].ToString());
                        //Temos que converter pra decimal primeiro por conta do dado do DW
                        historicoSegmento.Quantidade = Convert.ToInt32(decimal.Parse(item["qtd"].ToString()));

                        HistoricoComprasTrimestre hTrimestre = RepositoryService.HistoricoComprasTrimestre.ObterPor(historicoSegmento.UnidadeNegocio.Id, ano, trimestre);
                        if (hTrimestre != null)
                            historicoSegmento.TrimestreRelacionamento = new Lookup(hTrimestre.ID.Value, "");

                        RepositoryService.HistoricoComprasSegmento.Update(historicoSegmento);
                }
                else
                {
                    HistoricoComprasSegmento hsComp = new HistoricoComprasSegmento(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    
                    hsComp.UnidadeNegocio = new Lookup(mUnidadeNegocio.ID.Value, "");
                    hsComp.Ano = int.Parse(item["cd_ano"].ToString());

                    if(String.IsNullOrEmpty(mSegmento.Nome))
                        hsComp.Nome = mUnidadeNegocio.Nome + " - " + item["cd_ano"].ToString();
                    else
                        hsComp.Nome = mUnidadeNegocio.Nome + " - " + mSegmento.Nome + " - " + item["cd_ano"].ToString() ;

                    hsComp.Quantidade = Convert.ToInt32(decimal.Parse(item["qtd"].ToString()));
                    hsComp.Valor = decimal.Parse(item["Valor"].ToString());
                    if(mSegmento != null)
                        hsComp.Segmento = new Lookup(mSegmento.ID.Value, "");
                    hsComp.Trimestre = trimestre;

                    HistoricoComprasTrimestre hTrimestre = RepositoryService.HistoricoComprasTrimestre.ObterPor(hsComp.UnidadeNegocio.Id, hsComp.Ano, trimestre);

                    if (hTrimestre != null)
                    {
                        hsComp.TrimestreRelacionamento = new Lookup(hTrimestre.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasSegmento.Create(hsComp);
                }
            }
        }
        #endregion
    }
}
