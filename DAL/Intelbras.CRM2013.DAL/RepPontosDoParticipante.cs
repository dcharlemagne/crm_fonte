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
//    public class RepPontosDoParticipante<T> : CrmServiceRepository<T>, IPontosDoParticipante<T>
//    {
//        #region Construtores

//        public PontosDoParticipanteRepository()
//        {
//        }

//        internal PontosDoParticipanteRepository(Organizacao organizacao)
//        {
//            base.OrganizacaoCorrente = organizacao;
//        }

//        #endregion

//        public Guid Create(PontosDoParticipante entity)
//        {
//            return base.Provider.Create(PontosDoParticipanteFactory.IntanciaPontosDoParticipante(entity));
//        }

//        public void Delete(Guid entityId)
//        {
//            base.Provider.Delete("new_pontos_participante_fidelidade", entityId);
//        }

//        public PontosDoParticipante Retrieve(Guid entityId)
//        {
//            var be = (new_pontos_participante_fidelidade)base.Provider.Retrieve("new_pontos_participante_fidelidade", entityId, new ColumnSet() { Attributes = PontosDoParticipanteFactory.GetColunas });
//            return PontosDoParticipanteFactory.InstanciarPontosDoParticipante(be, base.OrganizacaoCorrente);
//        }

//        public IList<PontosDoParticipante> RetrieveAll()
//        {
//            throw new NotImplementedException();
//        }

//        public void Update(PontosDoParticipante entity)
//        {
//            base.Provider.Update(PontosDoParticipanteFactory.IntanciaPontosDoParticipante(entity));
//        }

//        public Organizacao Organizacao
//        {
//            set { throw new NotImplementedException(); }
//        }

//        //TODO: Testar através de classe de testes quando houver registros
//        public List<PontosDoParticipante> ListarProdutosCadastradosPorRevendaEDistribuidor(Guid revenda, Guid distribuidor)
//        {
//            List<PontosDoParticipante> pontosParticipante = null;

//            QueryExpression qeh = new QueryExpression("new_pontos_participante_fidelidade");
//            //qeh.ColumnSet.AddColumns(new string[] { "", "", "", "" });
//            qeh.Columns.AddAllColumns();
//            if (revenda != Guid.Empty && revenda != null)
//                qeh.Criteria.Conditions.Add(new ConditionExpression("new_participanteid", ConditionOperator.Equal, revenda);

//            if (distribuidor != Guid.Empty && distribuidor != null)
//                qeh.Criteria.Conditions.Add(new ConditionExpression("new_distribuidorid", ConditionOperator.Equal, distribuidor);

//            var bec = base.Provider.RetrieveMultiple(qeh.Query);

//            if (bec.Entities.Count > 0)
//            {
//                foreach (var item in bec.Entities)
//                {
//                    pontosParticipante.Add(PontosDoParticipanteFactory.InstanciarPontosDoParticipante((new_pontos_participante_fidelidade)item, OrganizacaoCorrente));
//                }
//            }

//            return pontosParticipante;
//        }
//    }
//}
