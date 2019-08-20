//using System;
//using System.Collections.Generic;
//using Microsoft.Xrm.Sdk;
//using Microsoft.Xrm.Sdk.Query;
//using Microsoft.Crm.Sdk.Messages;
//using SDKore.Crm;
//using Intelbras.CRM2013.Domain.IntelbrasService;
//using Intelbras.CRM2013.Domain.SharepointWebService;
//using Intelbras.CRM2013.Domain.Model;
//using Intelbras.CRM2013.Domain.IRepository;

//namespace Intelbras.CRM2013.DAL
//{
//    public class RepDebitoFidelidade<T> : CrmServiceRepository<T>, IDebitoFidelidadeo<T>
//    {
//        #region Construtores

//        public DebitoFidelidadeRepository() { }
//        public DebitoFidelidadeRepository(Organizacao organizacao) { base.OrganizacaoCorrente = organizacao; }

//        #endregion

//        public Guid Create(DebitoFidelidade debitoFidelidade)
//        {
//            return base.Provider.Create(DebitoFidelidadeFactory.InstanciarDebitoFidelidade(debitoFidelidade));
//        }

//        public void Delete(Guid entityId)
//        {
//            throw new NotImplementedException();
//        }

//        public DebitoFidelidade Retrieve(Guid entityId)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<DebitoFidelidade> RetrieveAll()
//        {
//            throw new NotImplementedException();
//        }

//        public void Update(DebitoFidelidade entity)
//        {
//            throw new NotImplementedException();
//        }

//        public Organizacao Organizacao
//        {
//            set { base.OrganizacaoCorrente = value; }
//        }

//        /// <summary>
//        /// Método que retorna a quantidade de pontos resgatados de um participante
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="situacao">Situação do débito (Picklist)</param>
//        /// <returns>Número inteiro (int)</returns>
//        public int QuantidadeResgatesPor(Guid participanteId, int situacao)
//        {
//            int pontosDebito = 0;

//            QueryExpression queryHelper = new QueryExpression("new_debito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_situacao", ConditionOperator.Equal, situacao);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//            foreach (new_debito_fidelidade item in colecao.Entities)
//                    pontosDebito += item.new_valor.Value;

//            return pontosDebito;

//        }

//    }
//}
