using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepCompromissosDoCanal<T> : CrmServiceRepository<T>, ICompromissosDoCanal<T>
    {
        public List<T> ListarPor(Guid canalId, Guid unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.StateCode.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public bool AtualizarStatus(Guid compromissoID, int StateCode, int StatusCode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_compdocanal", compromissoID),
                State = new OptionSetValue(StateCode),
                Status = new OptionSetValue(StatusCode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
        public List<T> ListarPorCanal(Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            //Status
            //query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);

           // Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            //query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public List<T> ListarPor(Guid CompromissosDoCanalId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, CompromissosDoCanalId);
            query.Criteria.Conditions.Add(cond1);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.StateCode.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<Guid> CompromissosProg, Guid Canal, Guid UnidadeNegocios)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, CompromissosProg);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Canal);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, UnidadeNegocios);
            query.Criteria.Conditions.Add(cond3);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid CompromissosDoCanalId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, CompromissosDoCanalId);
            query.Criteria.Conditions.Add(cond1);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorCanalCompromisso(Guid itbc_canalid, Guid itbc_compdoprogid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_compdoprogid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canalid);
            query.Criteria.Conditions.Add(cond2);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public T ObterPor(Guid itbc_compdoprogid, Guid itbc_canalid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_compdoprogid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canalid);
            query.Criteria.Conditions.Add(cond2);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorNome(string itbc_compdoprog, Guid itbc_canalid, Guid unidadeId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_compdoprog);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canalid);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeId);
            query.Criteria.Conditions.Add(cond3);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid itbc_compdoprogid, Guid unidNeg, Guid itbc_canalid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_compdoprogid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidNeg);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canalid);
            query.Criteria.Conditions.Add(cond3);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorContaUnidade(Guid canalId, Guid unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorPlanilha()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_statuscompromissosid", ConditionOperator.NotEqual, new Guid("41725811-75ed-e311-9407-00155d013d38"));//Diferente de nao cumprido
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarLote()
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>");
            strFetchXml.Append(" <entity name='itbc_compdocanal'>");

            strFetchXml.Append("  <link-entity name='itbc_compromissos' from='itbc_compromissosid' to='itbc_compdoprogid' alias='at'>");
            strFetchXml.Append("   <filter type='and'>");
            strFetchXml.Append("    <condition attribute='itbc_tipodemonitoramento' operator='eq' value='993520001' />");
            strFetchXml.Append("    <condition attribute='itbc_compromissosid' operator='ne' uiname='Envio de evidências de Showroom' uitype='itbc_compromissos' value='{380883BC-74ED-E311-9420-00155D013D39}' />");
            strFetchXml.Append("   </filter>");
            strFetchXml.Append("  </link-entity>");
            strFetchXml.Append("  <link-entity name='activitypointer' from='regardingobjectid' to='itbc_compdocanalid' alias='bt' link-type='outer'></link-entity>");
            strFetchXml.Append("   <filter type='and'>");
            strFetchXml.Append("    <condition entityname='bt' attribute='regardingobjectid' operator='null'/>");
            strFetchXml.Append("   </filter>");
            strFetchXml.Append(" </entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public T ObterPor(Guid unidadeNegocioId, Guid contaId, int codigoCompromisso)
        {
            var query = GetQueryExpression<T>(true);

            query.AddLink("itbc_compromissos", "itbc_compdoprogid", "itbc_compromissosid")
                 .LinkCriteria.AddCondition("itbc_codigo", ConditionOperator.Equal, codigoCompromisso);

            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, contaId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.StateCode.Ativo);

            var colecao = RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorCod33EPorMatriz(Guid canalId, Guid UnidadeId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                        ");
            strFetchXml.Append("   <entity name='itbc_compdocanal'>                                                                          ");
            strFetchXml.Append("     <attribute name='itbc_compdocanalid' />                                                                 ");
            strFetchXml.Append("     <attribute name='itbc_name' />                                                                          ");
            strFetchXml.Append("     <attribute name='createdon' />                                                                          ");
            strFetchXml.Append("     <order attribute='itbc_name' descending='false' />                                                      ");
            strFetchXml.Append("     <filter type='and'>                                                                                     ");
            strFetchXml.AppendFormat("       <condition attribute='itbc_businessunitid' operator='eq' uitype='businessunit' value='{0}' />   ", UnidadeId);
            strFetchXml.AppendFormat("       <condition attribute='itbc_canalid' operator='eq' uitype='account' value='{0}' />               ", canalId);
            strFetchXml.Append("     </filter>                                                                                               ");
            strFetchXml.Append("     <link-entity name='itbc_compromissos' from='itbc_compromissosid' to='itbc_compdoprogid' alias='ak'>     ");
            strFetchXml.Append("       <filter type='and'>                                                                                   ");
            strFetchXml.Append("         <condition attribute='itbc_codigo' operator='eq' value='33' />                                      ");
            strFetchXml.Append("       </filter>                                                                                             ");
            strFetchXml.Append("     </link-entity>                                                                                          ");
            strFetchXml.Append("   </entity>                                                                                                 ");
            strFetchXml.Append(" </fetch>                                                                                                    ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento, Domain.Model.StatusCompromissos cumprido, Domain.Model.StatusCompromissos cumpridoForaPrazo)
        {
            var query = GetQueryExpression<T>(true);

            string strFetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='itbc_compdocanal'>
                                <attribute name='itbc_compdocanalid' />
                                <attribute name='itbc_name' />
                                <attribute name='itbc_validade' />
                                <attribute name='itbc_statuscompromissosid' />
                                <attribute name='itbc_compdoprogid' />
                                <attribute name='itbc_canalid' />
                                <attribute name='itbc_businessunitid' />
                                <order attribute='itbc_name' descending='false' />
                                <filter type='and'>
                                  <condition attribute='statuscode' operator='eq' value='1' />
                                  <filter type='or'>
                                    <condition attribute='itbc_validade' operator='on-or-before' value='{3}' />
                                    <condition attribute='itbc_validade' operator='null' />
                                  </filter>
                                  <filter type='or'>
                                    <condition attribute='itbc_statuscompromissosid' operator='null' />
                                    <condition attribute='itbc_statuscompromissosid' operator='in'>
                                      <value uiname='Cumprido' uitype='itbc_statuscompromissos'>{1}</value>
                                      <value uiname='Cumprido Fora do Prazo' uitype='itbc_statuscompromissos'>{2}</value>
                                    </condition>
                                  </filter>
                                </filter>
                                <link-entity name='itbc_compromissos' from='itbc_compromissosid' to='itbc_compdoprogid' alias='aa'>
                                  <filter type='and'>
                                    <condition attribute='itbc_tipodemonitoramento' operator='eq' value='{0}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";

            strFetchXml = string.Format(strFetchXml,
                (int)tipoMonitoramento,
                cumprido.ID.Value,
                cumpridoForaPrazo.ID.Value,
                DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"));

            RetrieveMultipleRequest retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarVencidosManualPorTarefasESolicitacoes(int[] tipoMonitoramento, Guid statusCompromissoId)
        {
            #region datas
            string dia = DateTime.Now.Day.ToString();
            if (int.Parse(dia) < 10)
                dia = "0" + dia;

            string mes = DateTime.Now.Month.ToString();
            if (int.Parse(mes) < 10)
                mes = "0" + mes;

            string ano = DateTime.Now.Year.ToString();
            #endregion

            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();


            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                        ");
            strFetchXml.Append("  <entity name='itbc_compdocanal'>                                                                           ");
            strFetchXml.Append("<attribute name='itbc_compdocanalid' />                                                                      ");
            strFetchXml.Append("<attribute name='itbc_name' />                                                                               ");
            strFetchXml.Append("<attribute name='createdon' />                                                                               ");
            strFetchXml.Append("<attribute name='itbc_validade' />                                                                           ");
            strFetchXml.Append("<attribute name='itbc_businessunitid' />                                                                     ");
            strFetchXml.Append("<attribute name='itbc_statuscompromissosid' />                                                               ");
            strFetchXml.Append("<attribute name='itbc_status' />                                                                             ");
            strFetchXml.Append("<attribute name='statecode' />                                                                               ");
            strFetchXml.Append("<attribute name='overriddencreatedon' />                                                                     ");
            strFetchXml.Append("<attribute name='statuscode' />                                                                              ");
            strFetchXml.Append("<attribute name='ownerid' />                                                                                 ");
            strFetchXml.Append("<attribute name='modifiedonbehalfby' />                                                                      ");
            strFetchXml.Append("<attribute name='modifiedby' />                                                                              ");
            strFetchXml.Append("<attribute name='itbc_just_alt_status_comp' />                                                               ");
            strFetchXml.Append("<attribute name='modifiedon' />                                                                              ");
            strFetchXml.Append("<attribute name='itbc_cumprir_compromisso_em' />                                                             ");
            strFetchXml.Append("<attribute name='createdonbehalfby' />                                                                       ");
            strFetchXml.Append("<attribute name='createdby' />                                                                               ");
            strFetchXml.Append("<attribute name='itbc_compdoprogid' />                                                                       ");
            strFetchXml.Append("<attribute name='itbc_classificacaoid' />                                                                    ");
            strFetchXml.Append("<attribute name='itbc_categoriaid' />                                                                        ");
            strFetchXml.Append("<attribute name='itbc_canalid' />                                                                            ");
            strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                                       ");
            strFetchXml.Append("    <filter type='and'>                                                                                      ");
            strFetchXml.Append("      <condition attribute='statuscode' operator='eq' value='1' />                                           ");
            strFetchXml.Append("      <filter type='or'>                                                                                     ");
            strFetchXml.Append("        <condition attribute='itbc_validade' operator='on-or-before' value='" + ano + "-" + mes + "-" + dia + "' />");
            strFetchXml.Append("        <condition attribute='itbc_validade' operator='null' />                                              ");
            strFetchXml.Append("      </filter>                                                                                              ");
            strFetchXml.AppendFormat("     <condition attribute='itbc_statuscompromissosid' operator='eq' uitype='itbc_statuscompromissos' value='{0}' />", statusCompromissoId);
            strFetchXml.Append("    </filter>                                                                                                ");
            strFetchXml.Append("    <link-entity name='itbc_compromissos' from='itbc_compromissosid' to='itbc_compdoprogid' alias='aa'>      ");
            strFetchXml.Append("      <filter type='and'>                                                                                    ");
            strFetchXml.Append("        <condition attribute='itbc_tipodemonitoramento' operator='in'>                                       ");
            strFetchXml.Append("          <value>" + tipoMonitoramento[0] + "</value>                                                            ");
            strFetchXml.Append("          <value>" + tipoMonitoramento[1] + "</value>                                                            ");
            strFetchXml.Append("          <value>" + tipoMonitoramento[2] + "</value>                                                        ");
            strFetchXml.Append("        </condition>                                                                                         ");
            strFetchXml.Append("      </filter>                                                                                              ");
            strFetchXml.Append("    </link-entity>                                                                                           ");
            strFetchXml.Append("  </entity>                                                                                                  ");
            strFetchXml.Append("</fetch>                                                                                                     ");


            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarAtivosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento, Domain.Model.StatusCompromissos cumprido, Domain.Model.StatusCompromissos cumpridoForaPrazo)
        {
            var query = GetQueryExpression<T>(true);

            var statusCompromisso = new string[] { cumprido.ID.Value.ToString(), cumpridoForaPrazo.ID.Value.ToString() };
            query.Criteria.AddCondition("itbc_statuscompromissosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, statusCompromisso);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.CompromissoCanal.Status.Ativo);

            query.AddLink("itbc_compromissos", "itbc_compdoprogid", "itbc_compromissosid")
                 .LinkCriteria.AddCondition("itbc_tipodemonitoramento", ConditionOperator.Equal, (int)tipoMonitoramento);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorSoliBenDeShowRoom(Guid solId, Guid compromissoId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>                                                              ");
            strFetchXml.Append("  <entity name='itbc_compdocanal'>                                                                                                                ");
            strFetchXml.Append("    <attribute name='itbc_compdocanalid' />                                                                                                       ");
            strFetchXml.Append("    <attribute name='itbc_name' />                                                                                                                ");
            strFetchXml.Append("    <attribute name='createdon' />                                                                                                                ");
            strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                                                                            ");
            strFetchXml.Append("    <filter type='and'>                                                                                                                           ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_compdoprogid' operator='eq' uitype='itbc_compromissos' value='{0}' /> ", compromissoId);
            strFetchXml.Append("    </filter>                                                                                                                                     ");
            strFetchXml.Append("    <link-entity name='account' from='accountid' to='itbc_canalid' alias='bc'>                                                                    ");
            strFetchXml.Append("      <link-entity name='itbc_solicitacaodebeneficio' from='itbc_accountid' to='accountid' alias='bd'>                                            ");
            strFetchXml.Append("        <filter type='and'>                                                                                                                       ");
            strFetchXml.AppendFormat("          <condition attribute='itbc_solicitacaodebeneficioid' operator='eq' uitype='itbc_solicitacaodebeneficio' value='{0}' />                  ", solId);
            strFetchXml.Append("        </filter>                                                                                                                                 ");
            strFetchXml.Append("      </link-entity>                                                                                                                              ");
            strFetchXml.Append("    </link-entity>                                                                                                                                ");
            strFetchXml.Append("  </entity>                                                                                                                                       ");
            strFetchXml.Append("</fetch>                                                                                                                                          ");

            // Build fetch request and obtain results.                                                                                                                        
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }
    }
}
