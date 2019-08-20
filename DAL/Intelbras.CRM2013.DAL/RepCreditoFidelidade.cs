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
//    public class RepCreditoFidelidade<T> : CrmServiceRepository<T>, ICreditoFidelidade<T>
//    {
//        #region Construtores

//        public CreditoFidelidadeRepository() { }
//        public CreditoFidelidadeRepository(Organizacao organizacao) { base.OrganizacaoCorrente = organizacao; }

//        #endregion

//        public Guid Create(CreditoFidelidade entity)
//        {
//            return base.Provider.Create(CreditoFidelidadeFactory.InstanciarCreditoFidelidade(entity));
//        }

//        public void Delete(Guid entityId)
//        {
//            throw new NotImplementedException();
//        }

//        public CreditoFidelidade Retrieve(Guid entityId)
//        {
//            throw new NotImplementedException();
//        }

//        public IList<CreditoFidelidade> RetrieveAll()
//        {
//            throw new NotImplementedException();
//        }

//        public void Update(CreditoFidelidade creditoFidelidade)
//        {
//            base.Provider.Update(CreditoFidelidadeFactory.InstanciarCreditoFidelidade(creditoFidelidade));
//        }

//        public Organizacao Organizacao
//        {
//            set { base.OrganizacaoCorrente = value; }
//        }

//        /// <summary>
//        /// Metodo que verifica se o participante possui creditos disponiveis para um resgate
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="pontosResgate">Quantidade de pontos a serem resgatado</param>
//        /// <param name="data">Data para verificar se o credito está dentro do prazo</param>
//        /// <returns>Verdade/Falso</returns>
//        public bool PossuiPor(Guid participanteId, int pontosResgate , DateTime data)
//        { 
//            bool possuiCredito = false;
//            int pontosCredito = 0;

//            QueryExpression queryHelper = new QueryExpression("new_credito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.GreaterEqual, data.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_valor_disponivel",ConditionOperator.GreaterThan,0);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//           foreach (new_credito_fidelidade item in colecao.Entities)
//                    pontosCredito += item.new_valor_disponivel.Value;

//            if (pontosCredito >= pontosResgate) possuiCredito = true;

//            return possuiCredito;
//        }

//        /// <summary>
//        /// Metodo que retorna uma lista de Creditos disponiveis de um participante
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="data">Data para verificar se o credito está dentro do prazo</param>
//        /// <returns>Lista de CreditoFidelidade</returns>
//        public List<CreditoFidelidade> ListarDisponiveisPor(Guid participanteId, DateTime data)
//        {
//            List<CreditoFidelidade> listaCreditos = new List<CreditoFidelidade>();

//            QueryExpression queryHelper = new QueryExpression("new_credito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.GreaterEqual, data.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_valor_disponivel", ConditionOperator.GreaterThan, 0);
//            queryHelper.Orders.Add("new_vencimento", OrderType.Ascending);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//            foreach (new_credito_fidelidade item in colecao.Entities)
//                    listaCreditos.Add(CreditoFidelidadeFactory.InstanciarCreditoFidelidade(item, base.OrganizacaoCorrente));    

//            return listaCreditos;
//        }

//        /// <summary>
//        /// Método que retorna a quantidade de créditos disponiveis de um participante
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="data">Data para verificar se o credito está dentro do prazo</param>
//        /// <returns>Número inteiro (int)</returns>
//        public int QuantidadeCreditosDisponiveisPor(Guid participanteId, DateTime data)
//        {
//            int pontosCredito = 0;

//            QueryExpression queryHelper = new QueryExpression("new_credito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.GreaterEqual, data.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_valor_disponivel", ConditionOperator.GreaterThan, 0);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//            foreach (new_credito_fidelidade item in colecao.Entities)
//                    pontosCredito += item.new_valor_disponivel.Value;

//            return pontosCredito;
//        }

//        /// <summary>
//        /// Método que retorna a quantidade de créditos disponiveis de um participante que estão a 1 mês de vencer a validade
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="data">>Data para verificar se o credito está dentro do prazo</param>
//        /// <returns>Número inteiro (int)</returns>
//        public int QuantidadeCreditosAVencerPor(Guid participanteId, DateTime data)
//        {
//            int pontosCredito = 0;

//            DateTime dataExpiracao = new DateTime();
//            dataExpiracao = data.AddMonths(1);

//            QueryExpression queryHelper = new QueryExpression("new_credito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.GreaterEqual, data.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.LessEqual, dataExpiracao.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_valor_disponivel", ConditionOperator.GreaterThan, 0);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//            foreach (new_credito_fidelidade item in colecao.Entities)
//                    pontosCredito += item.new_valor_disponivel.Value;

//            return pontosCredito;
//        }

//        /// <summary>
//        /// Método que retorna a quantidade de créditos vencidos de um participante
//        /// </summary>
//        /// <param name="participanteId">Id do Participante (Contato)</param>
//        /// <param name="data">Data para verificar se o credito está fora do prazo</param>
//        /// <returns>Número inteiro (int)</returns>
//        public int QuantidadeCreditosVencidosPor(Guid participanteId, DateTime data)
//        {
//            int pontosCredito = 0;

//            QueryExpression queryHelper = new QueryExpression("new_credito_fidelidade");
//            queryHelper.Columns.AddAllColumns();
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_guid_participante", ConditionOperator.Equal, participanteId.ToString());
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_vencimento", ConditionOperator.LessThan, data.Date.ToString("yyyy-MM-dd"));
//            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_valor_disponivel", ConditionOperator.GreaterThan, 0);

//            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

//            foreach (new_credito_fidelidade item in colecao.Entities)
//                    pontosCredito += item.new_valor_disponivel.Value;

//            return pontosCredito;

//        }

//    }
//}
