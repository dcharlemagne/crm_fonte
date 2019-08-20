using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepHistoricoDistribuidor<T> : CrmServiceRepository<T>, IHistoricoDistribuidor<T>
    {
        public List<T> ListarPorRevendaComDataFim(Guid revendaId, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                RetrieveMultipleRequest retrieveMultiple;
                StringBuilder strFetchXml = new StringBuilder();

                strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                                         ");
                strFetchXml.Append("<entity name='itbc_histdistrib'>                                                                                                             ");
                strFetchXml.Append("<attribute name='itbc_histdistribid'/>                                                                                                        ");
                strFetchXml.Append("<filter type='and'>                                                                                                                           ");
                strFetchXml.Append("    <filter type='and'>                                                                                                                       ");
                strFetchXml.Append("           <condition attribute='statuscode' operator='eq' value='1' />                                                                          ");
                strFetchXml.AppendFormat("     <condition attribute='itbc_revendaid' operator='eq' value='{0}' />",                                                          revendaId);
                strFetchXml.Append("       <filter type='or'>                                                                                                                     ");
                strFetchXml.Append("           <filter type='and'>                                                                                                                 ");
                strFetchXml.Append("                  <condition attribute='itbc_datafim' operator='not-null'/>                                                                        ");
                strFetchXml.Append("       <filter type='or'>                                                                                                                     ");
                strFetchXml.Append("             <filter type='and'>                                                                                                              ");
                strFetchXml.Append("                    <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' /> ");
                strFetchXml.Append("                    <condition attribute='itbc_datafim' operator='on-or-after' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' />     ");
                strFetchXml.Append("             </filter>                                                                                                                        ");
                strFetchXml.Append("             <filter type='and'>                                                                                                                  ");
                strFetchXml.Append("                    <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />    ");
                strFetchXml.Append("                    <condition attribute='itbc_datafim' operator='on-or-after' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />        ");
                strFetchXml.Append("             </filter>                                                                                                                            ");
                strFetchXml.Append("             <filter type='and'>                                                                                                                  ");
                strFetchXml.Append("                    <condition attribute='itbc_datainicio' operator='on-or-after' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' />  ");
                strFetchXml.Append("                    <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />    ");
                strFetchXml.Append("             </filter>                                                                                                                            ");
                strFetchXml.Append("             <filter type='and'>                                                                                                                   ");
                strFetchXml.Append("                     <condition attribute='itbc_datafim' operator='on-or-after' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />       ");
                strFetchXml.Append("                     <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />   ");
                strFetchXml.Append("             </filter>                                                                                                                             ");
                strFetchXml.Append("           </filter>                                                                                                                              ");
                strFetchXml.Append("         </filter>                                                                                                                              ");
                strFetchXml.Append("         <filter type='and'>                                                                                                                    ");
                strFetchXml.Append("                     <condition attribute='itbc_datafim' operator='null'/>                                                                    ");
                strFetchXml.Append("            <filter type='or'>                                                                                                                     ");
                strFetchXml.Append("              <filter type='and'>                                                                                                                    ");
                strFetchXml.Append("                     <condition attribute='itbc_datainicio' operator='on-or-after' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' /> ");
                strFetchXml.Append("                     <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataFim.Value.ToString("yyyy-MM-dd") + "' />   ");
                strFetchXml.Append("              </filter>                                                                                                                              ");
                strFetchXml.Append("                     <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' />");
                strFetchXml.Append("           </filter>                                                                                                                               ");
                strFetchXml.Append("         </filter>                                                                                                                                ");
                strFetchXml.Append("       </filter>                                                                                                                                 ");
                strFetchXml.Append("     </filter>                                                                                                                                  ");
                strFetchXml.Append("   </filter>                                                                                                                                    ");
                strFetchXml.Append("</entity>                                                                                                                                     ");
                strFetchXml.Append("</fetch>                                                                                                                                      ");

                retrieveMultiple = new RetrieveMultipleRequest()
                {
                    Query = new FetchExpression(strFetchXml.ToString())
                };
                return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;

            }
            catch (Exception e)
            {
                throw e;

            }            
        }
        public List<T> ListarPorRevendaSemDataFim(Guid revendaId, DateTime? dataInicio = null)
        {
            try
            {
            
            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                                ");
            strFetchXml.Append("   <entity name='itbc_histdistrib'>                                                                                                  ");
            strFetchXml.Append("     <attribute name='itbc_histdistribid' />                                                                                         ");
            strFetchXml.Append("    <filter type='and'>                                                                                                              ");
            strFetchXml.Append("      <filter type='and'>                                                                                                              ");
            strFetchXml.Append("          <condition attribute='statuscode' operator='eq' value='1' />                                                                 ");
            strFetchXml.AppendFormat("    <condition attribute='itbc_revendaid' operator='eq' value='{0}' />", revendaId);
            strFetchXml.Append("          <condition attribute='itbc_datainicio' operator='on-or-before' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' />        ");
            strFetchXml.Append("        <filter type='or'>                                                                                                               ");            
            strFetchXml.Append("           <condition attribute='itbc_datafim' operator='null' />                                                                   ");
            strFetchXml.Append("           <condition attribute='itbc_datafim' operator='on-or-after' value='" + dataInicio.Value.ToString("yyyy-MM-dd") + "' />        ");
            strFetchXml.Append("        </filter>                                                                                                                      ");
            strFetchXml.Append("      </filter>                                                                                                                        ");
            strFetchXml.Append("    </filter>                                                                                                                        ");
            strFetchXml.Append("   </entity>                                                                                                                         ");
            strFetchXml.Append(" </fetch>                                                                                                                            ");


            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;

            }
            catch (Exception e)
            {
                throw e;

            }            
        }
        
        public Boolean AlterarStatus(Guid historicoid, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;

            }
            else if (status == 993520000)
            {
                //fluxo Concluido
                stateCode = 0;
                status = 993520000;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }


            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference(SDKore.Crm.Util.Utility.GetEntityName<HistoricoDistribuidor>(), historicoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }


        public List<T> ListarPorPeriodo(DateTime data)
        {
            var query = GetQueryExpression<T>(true);
            
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.HistoricoDistribuidor.Statecode.Ativo);
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.HistoricoDistribuidor.Statuscode.Ativo);

            FilterExpression filtro = new FilterExpression(LogicalOperator.And);
            filtro.AddCondition("itbc_datainicio", ConditionOperator.LessEqual, data);
            filtro.AddCondition("itbc_datafim", ConditionOperator.GreaterEqual, data);
            query.Criteria.AddFilter(filtro);

            query.TopCount = 300;

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}