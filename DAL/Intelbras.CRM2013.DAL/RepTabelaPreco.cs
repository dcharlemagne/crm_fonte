using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepTabelaPreco<T> : CrmServiceRepository<T>, ITabelaPreco<T>
    {
        public List<T> ListarPor(Guid itbc_tabeladeprecoid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition(new ConditionExpression("itbc_tabeladeprecoid", ConditionOperator.Equal, itbc_tabeladeprecoid));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<TabelaDePreco> ListarPor(Domain.Model.Conta cliente)
        {
            List<TabelaDePreco> tabelaDePreco = new List<TabelaDePreco>();
            BuscaTabelas_ttTbPrecoRow[] row = null;
            decimal limiteDeCreditoIntelbras = 0,
                    limiteDeCreditoDisponivelIntelbras = 0,
                    limiteDeCreditoSupplierCard = 0,
                    limiteDeCreditoDisponivelSupplierCard = 0;

            Domain.Servicos.HelperWS.IntelbrasService.BuscaTabelas(int.Parse(cliente.CodigoMatriz), out row, out limiteDeCreditoSupplierCard, out limiteDeCreditoDisponivelSupplierCard, out limiteDeCreditoIntelbras, out limiteDeCreditoDisponivelIntelbras);

            if (row != null)
            {
                cliente.LimiteDeCreditoIntelbras = limiteDeCreditoIntelbras;
                cliente.LimiteDeCreditoDisponivelIntelbras = limiteDeCreditoDisponivelIntelbras;
                cliente.LimiteDeCreditoSupplierCard = limiteDeCreditoSupplierCard;
                cliente.LimiteDeCreditoDisponivelSupplierCard = limiteDeCreditoDisponivelSupplierCard;

                foreach (var item in row)
                {
                    TabelaDePreco _tabelaPreco = new TabelaDePreco(OrganizationName, IsOffline);
                    _tabelaPreco.CodigoDaTabelaDePreco = item.nrtabpre;
                    _tabelaPreco.Nome = item.dsdescricao;
                    _tabelaPreco.TabelaEspecifica = item.lgtbespecifica.Value;
                    _tabelaPreco.Cliente = cliente;
                    _tabelaPreco.UnidadeDeNegocio = new UnidadeNegocio(OrganizationName, IsOffline);
                    _tabelaPreco.UnidadeDeNegocio.CodigoEms = item.cdunidnegoc;
                    _tabelaPreco.Categoria = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterCategoriaDoClientePor(Guid.Empty, item.cdcategoria.ToString());
                    _tabelaPreco.Representante = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterRepresentantePor(Guid.Empty, item.codrep.Value.ToString());
                    _tabelaPreco.CondicaoDePagamento = new CondicaoPagamento(OrganizationName, IsOffline);
                    _tabelaPreco.CondicaoDePagamento.Codigo = item.codcondpag.Value;
                    tabelaDePreco.Add(_tabelaPreco);
                }
            }

            return tabelaDePreco;
        }

        public T ObterPor(Guid itbc_tabeladeprecoid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_tabeladeprecoid", ConditionOperator.Equal, itbc_tabeladeprecoid));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(int itbc_codigo_tabela)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_tabela", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_codigo_tabela);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_tabeladeprecoid, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_tabeladepreco", itbc_tabeladeprecoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        //CRM4
        public List<TabelaDePreco> ListarTabelaDePreco(Domain.Model.Conta cliente)
        {
            List<TabelaDePreco> tabelaDePreco = new List<TabelaDePreco>();
            BuscaTabelas_ttTbPrecoRow[] row = null;
            decimal limiteDeCreditoIntelbras = 0,
                    limiteDeCreditoDisponivelIntelbras = 0,
                    limiteDeCreditoSupplierCard = 0,
                    limiteDeCreditoDisponivelSupplierCard = 0;

            Domain.Servicos.HelperWS.IntelbrasService.BuscaTabelas(int.Parse(cliente.CodigoMatriz), out row, out limiteDeCreditoSupplierCard, out limiteDeCreditoDisponivelSupplierCard, out limiteDeCreditoIntelbras, out limiteDeCreditoDisponivelIntelbras);

            if (row != null)
            {
                cliente.LimiteDeCreditoIntelbras = limiteDeCreditoIntelbras;
                cliente.LimiteDeCreditoDisponivelIntelbras = limiteDeCreditoDisponivelIntelbras;
                cliente.LimiteDeCreditoSupplierCard = limiteDeCreditoSupplierCard;
                cliente.LimiteDeCreditoDisponivelSupplierCard = limiteDeCreditoDisponivelSupplierCard;

                foreach (var item in row)
                {
                    TabelaDePreco tabela = new TabelaDePreco(this.OrganizationName, this.IsOffline);
                    tabela.UnidadeDeNegocio = (new Domain.Servicos.RepositoryService()).UnidadeNegocio.ObterPorChaveIntegracao(item.cdunidnegoc);
                    tabela.Categoria = (new Domain.Servicos.RepositoryService()).Conta.ObterCategoriaDoClientePor(Guid.Empty, item.cdcategoria.ToString());
                    tabela.Representante = (new Domain.Servicos.RepositoryService()).Conta.ObterRepresentantePor(Guid.Empty, item.codrep.Value.ToString());
                    tabela.CondicaoDePagamento = (new Domain.Servicos.RepositoryService()).CondicaoPagamento.ObterPor(item.codcondpag.Value);
                    tabela.CodigoDaTabelaDePreco = item.nrtabpre;
                    tabela.Nome = item.dsdescricao;
                    tabela.TabelaEspecifica = item.lgtbespecifica.Value;
                }
            }

            return tabelaDePreco;
        }


        public List<TabelaDePreco> ListarTabelaDePrecoPor(int codigoCliente)
        {
            List<TabelaDePreco> tabelaDePreco = new List<TabelaDePreco>();
            BuscaTabelas_ttTbPrecoRow[] row = null;
            decimal limiteDeCreditoIntelbras = 0, limiteDeCreditoDisponivelIntelbras = 0, limiteDeCreditoSupplierCard = 0, limiteDeCreditoDisponivelSupplierCard = 0;
            Domain.Servicos.HelperWS.IntelbrasService.BuscaTabelas(codigoCliente, out row, out limiteDeCreditoSupplierCard, out limiteDeCreditoDisponivelSupplierCard, out limiteDeCreditoIntelbras, out limiteDeCreditoDisponivelIntelbras);
            if (row != null)
            {
                Domain.Model.Conta cliente = (new Domain.Servicos.RepositoryService()).Conta.PesquisarPor(codigoCliente);
                cliente.LimiteDeCreditoIntelbras = limiteDeCreditoIntelbras;
                cliente.LimiteDeCreditoDisponivelIntelbras = limiteDeCreditoSupplierCard;
                cliente.LimiteDeCreditoSupplierCard = limiteDeCreditoIntelbras;
                cliente.LimiteDeCreditoDisponivelSupplierCard = limiteDeCreditoSupplierCard;
                if (row.Length > 0)
                {
                    foreach (var item in row)
                    {
                        TabelaDePreco tabela = new TabelaDePreco(this.OrganizationName, this.IsOffline);
                        tabela.UnidadeDeNegocio = (new Domain.Servicos.RepositoryService()).UnidadeNegocio.ObterPorChaveIntegracao(item.cdunidnegoc);
                        tabela.Categoria = (new Domain.Servicos.RepositoryService()).Conta.ObterCategoriaDoClientePor(Guid.Empty, item.cdcategoria.ToString());
                        tabela.Representante = (new Domain.Servicos.RepositoryService()).Conta.ObterRepresentantePor(Guid.Empty, item.codrep.Value.ToString());
                        tabela.CondicaoDePagamento = (new Domain.Servicos.RepositoryService()).CondicaoPagamento.ObterPor(item.codcondpag.Value);
                        tabela.CodigoDaTabelaDePreco = item.nrtabpre;
                        tabela.Nome = item.dsdescricao;
                        tabela.TabelaEspecifica = item.lgtbespecifica.Value;
                        //tabelaDePreco.Add(new TabelaDePreco(item.nrtabpre, item.dsdescricao, item.lgtbespecifica.Value, cliente, new UnidadeDeNegocio(item.cdunidnegoc, "", Guid.Empty), ObterCategoriaDoClientePor(Guid.Empty, item.cdcategoria.ToString()), ObterRepresentantePor(Guid.Empty, item.codrep.Value.ToString()), new CondicaoPagamento(item.codcondpag.Value, "", Guid.Empty), base.OrganizacaoCorrente));
                    }
                }
            }

            return tabelaDePreco;
        }

    }
}
