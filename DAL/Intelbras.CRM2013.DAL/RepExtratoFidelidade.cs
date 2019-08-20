using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using System.Globalization;

namespace Intelbras.CRM2013.DAL
{
    public class RepExtratoFidelidade<T> : CrmServiceRepository<T>, IExtratoFidelidade<T>
    {
        public List<T> ObterExtratoPontosValidos(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrAfter, DateTime.Today.ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.ColumnSet.AddColumns("new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPorNumeroSerie(string numeroSerie)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_numero_serie", ConditionOperator.Equal, numeroSerie));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotEqual, 3));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterPontosCliente(Guid clienteId)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("contact", "new_contatoid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, clienteId));
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosAExpirar(int pagina, int quantidade)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrBefore, DateTime.Today.AddDays(-1).ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.ColumnSet.AddColumns("new_contatoid", "new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = quantidade;
            query.PageInfo.PageNumber = pagina;
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosAExpirarDaRevenda(int qtdDias, Guid revenda)
        {
            var query = GetQueryExpression<T>(true);
            query.LinkEntities[0].AddLink("contact", "new_contatoid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, revenda));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.On, DateTime.Today.AddDays(qtdDias).ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.ColumnSet.AddColumns("new_contatoid", "new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterVendedoresPontuadosPorDistribuidor(Guid revenda, Guid distribuidorId)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("contact", "new_contatoid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, revenda));
            //query.Criteria.Conditions.Add(new ConditionExpression("new_fornecedorid", ConditionOperator.Equal, distribuidorId);
            //query.Columns.AddColumns("new_vendedorid");
            query.Orders.Add(new OrderExpression("new_vendedorid", OrderType.Ascending));
            query.Distinct = true;
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosAExpirar(Guid contatoId, int qtdDias)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrBefore, DateTime.Today.AddDays(qtdDias).ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.ColumnSet.AddColumns("new_contatoid", "new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosDebitados(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrAfter, DateTime.Today.ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, new string[] { "1", "4" }));
            query.ColumnSet.AddColumns("new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosValidosPorDistribuidor(Guid distribuidorId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_fornecedorid", ConditionOperator.Equal, distribuidorId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrAfter, DateTime.Today.ToUniversalTime().ToString("u")));
            query.ColumnSet.AddColumns("new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosDoDistribuidor(Guid distribuidorId, Guid userId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_fornecedorid", ConditionOperator.Equal, distribuidorId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_vendedorid", ConditionOperator.Equal, userId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrAfter, DateTime.Today.ToUniversalTime().ToString("u")));
            query.ColumnSet.AddColumns("new_contatoid");
            query.Distinct = true;
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratoPontosExpirados(Guid contatoId, int qtdDias)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            //query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrBefore, DateTime.Today.AddDays(qtdDias).ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Expirado));
            query.ColumnSet.AddColumns("new_contatoid", "new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public IList<T> ObterExtratoPontosDetalhado(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_expiracao", ConditionOperator.OnOrAfter, DateTime.Today.ToUniversalTime().ToString("u")));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Ativo));
            query.ColumnSet.AddColumns("new_quantidade_pontos", "new_extrato_fidelidadeid", "new_debito_pontos");
            query.Orders.Add(new OrderExpression("new_data_expiracao", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterExtratosContato(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, new string[] { ((int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Ativo).ToString(CultureInfo.InvariantCulture), ((int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Expirado).ToString(CultureInfo.InvariantCulture) }));
            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterNaoValidadosAtivos(DateTime aPartiDe, int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords)
        {
            var query = GetQueryExpression<T>(true);

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = quantidade;
            query.PageInfo.PageNumber = pagina;
            query.PageInfo.PagingCookie = pagingCookie;

            query.Criteria.Conditions.Add(new ConditionExpression("new_numero_serie_valido", ConditionOperator.Equal, false));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotEqual, (int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Inativo));
            query.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.GreaterEqual, aPartiDe));

            var colecao = this.RetrieveMultiple(query);

            moreRecords = colecao.MoreRecords;

            return (List<T>)colecao.List;
        }

        public List<T> ObterInvalidosAtivos(int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords)
        {
            var query = GetQueryExpression<T>(true);

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = quantidade;
            query.PageInfo.PageNumber = pagina;
            query.PageInfo.PagingCookie = pagingCookie;

            query.Criteria.Conditions.Add(new ConditionExpression("new_numero_serie_valido", ConditionOperator.Equal, false));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)Domain.Enum.ExtratoFidelidade.RazaoDoStatus.Ativo));
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.NotNull));

            var colecao = this.RetrieveMultiple(query);

            moreRecords = colecao.MoreRecords;

            return (List<T>)colecao.List;
        }

        public void AlterarStatus(Guid id, int statuscode, bool stateActive)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("new_extrato_fidelidade", id),
                State = new OptionSetValue(stateActive ? 0 : 1),
                Status = new OptionSetValue(statuscode)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public void Associar(Domain.Model.ExtratoFidelidade extrato, Domain.Model.Resgate resgate)
        {
            var request = new AssociateEntitiesRequest();
            request.RelationshipName = "new_extrato_fidelidade_new_resgate_premio";
            request.Moniker1 = new EntityReference("new_extrato_fidelidade", extrato.Id);
            request.Moniker2 = new EntityReference("new_resgate_premio_fidelidade", resgate.Id);
            base.Provider.Execute(request);
        }
    }
}
