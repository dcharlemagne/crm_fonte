using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.Crm.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepDocumentoCanaisExtranet<T> : CrmServiceRepository<T>, IDocumentoCanaisExtranet<T>
    {
        public List<T> ListarPor(Guid[] classificacoesId, Guid[] categoriasId, DocumentoCanaisExtranet.RazaoStatus razaoStatus, bool somenteVigente, bool todosCanais = false)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet = new ColumnSet(true);

            #region Condições

            query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            query.Criteria.AddCondition(new ConditionExpression("itbc_todoscanais", ConditionOperator.Equal, todosCanais));

            if (somenteVigente)
            {
                query.Criteria.AddCondition(new ConditionExpression("itbc_vigenciainicio", ConditionOperator.LessEqual, DateTime.Now.Date));
                query.Criteria.AddCondition(new ConditionExpression("itbc_vigenciafinal", ConditionOperator.GreaterEqual, DateTime.Now.Date));
            }

            query.Criteria.AddCondition(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)razaoStatus));

            #endregion

            #region Links de Relacionamento

            if (!todosCanais)
            {
                if (classificacoesId.Length > 0)
                {
                    query.LinkEntities.Add(new LinkEntity()
                    {
                        LinkFromEntityName = Utility.GetEntityName<T>(),
                        LinkFromAttributeName = "itbc_docscanaisextranetid",
                        LinkToEntityName = "itbc_itbc_docscanaisextranet_itbc_classifica",
                        LinkToAttributeName = "itbc_docscanaisextranetid",
                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = 
                            {
                                new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacoesId)
                            }
                        }
                    });
                }

                if (categoriasId.Length > 0)
                {
                    query.LinkEntities.Add(new LinkEntity()
                    {
                        LinkFromEntityName = Utility.GetEntityName<T>(),
                        LinkFromAttributeName = "itbc_docscanaisextranetid",
                        LinkToEntityName = "itbc_itbc_docscanaisextranet_itbc_categoria",
                        LinkToAttributeName = "itbc_docscanaisextranetid",
                        LinkCriteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions = 
                            {
                                new ConditionExpression("itbc_categoriaid", ConditionOperator.In, categoriasId)
                            }
                        }
                    });
                }
            }
            #endregion

            #region Ordenações
            query.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPor(bool somenteVigente, bool todosCanais = true)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet = new ColumnSet(true);

            #region Condições

            query.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            query.Criteria.AddCondition(new ConditionExpression("itbc_todoscanais", ConditionOperator.Equal, todosCanais));

            if (somenteVigente)
            {
                query.Criteria.AddCondition(new ConditionExpression("itbc_vigenciainicio", ConditionOperator.LessEqual, DateTime.Now));
                query.Criteria.AddCondition(new ConditionExpression("itbc_vigenciafinal", ConditionOperator.GreaterEqual, DateTime.Now));
            }

            #endregion

            #region Ordenações
            query.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
    }
}
