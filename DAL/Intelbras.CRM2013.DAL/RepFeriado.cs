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
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;

namespace Intelbras.CRM2013.DAL
{
    public class RepFeriado<T> : CrmServiceRepository<T>, IFeriado<T>
    {
        public List<Feriado> ListarNacionais()
        {
            List<Feriado> Feriados = new List<Feriado>();

            var queryHelper = new QueryExpression("new_feriado_municipal_estadual");
            queryHelper.ColumnSet.AddColumns(new string[] { "new_name", "new_uf", "new_cidade", "new_data" });

            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_uf", ConditionOperator.Null));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_cidade", ConditionOperator.Null));

            var EntityCollection = base.Provider.RetrieveMultiple(queryHelper);

            foreach (Entity item in EntityCollection.Entities)
            {
                //TODO: retirado campos do CRM e recalcular com base em um único campo
                //var startDate = dateTime.ResolverTimeZoneDoCrm(calendarioDeFeriados.new_data.Value.ToString());
                //var endDate = dateTime.ResolverTimeZoneDoCrm(calendarioDeFeriados.new_data.Value.ToString()).Subtract(new TimeSpan(0, 1, 0));

                DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(item["new_data"]));
                DateTime endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);

                var dayOfWeek = new GregorianCalendar().GetDayOfWeek(startDate);
                Feriados.Add(new Feriado(this.OrganizationName, this.IsOffline, Convert.ToString(item["new_name"]), dayOfWeek, startDate, endDate));
            }

            return Feriados;
        }

        public List<Feriado> ListarPor(string estado)
        {
            List<Feriado> feriados = new List<Feriado>();

            var queryHelper = new QueryExpression("new_feriado_municipal_estadual");
            queryHelper.ColumnSet.AddColumns(new string[] { "new_name", "new_uf", "new_cidade", "new_data" });

            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_uf", ConditionOperator.Equal, estado));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_cidade", ConditionOperator.Null));

            var EntityCollection = base.Provider.RetrieveMultiple(queryHelper);

            foreach (Entity item in EntityCollection.Entities)
            {
                //TODO: retirado campos do CRM e recalcular com base em um único campo
                //var startDate = dateTime.ResolverTimeZoneDoCrm(calendarioDeFeriados.new_data.Value.ToString());
                //var endDate = dateTime.ResolverTimeZoneDoCrm(calendarioDeFeriados.new_data.Value.ToString()).Subtract(new TimeSpan(0, 1, 0));

                DateTime startDate = Convert.ToDateTime(Convert.ToDateTime(item["new_data"]));
                DateTime endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);


                var dayOfWeek = new GregorianCalendar().GetDayOfWeek(startDate);
                feriados.Add(new Feriado(this.OrganizationName, this.IsOffline, Convert.ToString(item["new_name"]), dayOfWeek, startDate, endDate));
            }

            return feriados;

        }

        public List<Feriado> ListarPor(string estado, string cidade)
        {
            List<Feriado> Feriados = new List<Feriado>();
            var queryHelper = new QueryExpression("new_feriado_municipal_estadual");
            queryHelper.ColumnSet.AddColumns(new string[] { "new_name", "new_uf", "new_cidade", "new_data" });
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_uf", ConditionOperator.Equal, estado));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_cidade", ConditionOperator.Equal, cidade));
            var EntityCollection = base.Provider.RetrieveMultiple(queryHelper);
            foreach (Entity item in EntityCollection.Entities)
            {
                DateTime startDate = Convert.ToDateTime(item["new_data"]).Date;
                DateTime endDate = startDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                var dayOfWeek = new GregorianCalendar().GetDayOfWeek(startDate);
                Feriados.Add(new Feriado(this.OrganizationName, this.IsOffline, Convert.ToString(item["new_name"]), dayOfWeek, startDate, endDate));
            }
            return Feriados;
        }

        public List<Feriado> RetrieveAll()
        {
            List<Feriado> Feriados = new List<Feriado>();

            var queryHelper = new QueryExpression("calendar");
            queryHelper.ColumnSet.AddColumns(new string[] { "isshared", "calendarid", "name" });
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("isshared", ConditionOperator.Equal, true));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, "Business Closure Calendar"));

            var EntityCollection = base.Provider.RetrieveMultiple(queryHelper);

            if (null != EntityCollection)
            {

                DateTime dateTime = new DateTime();
                foreach (Entity item in EntityCollection.Entities)
                {
                    EntityCollection calendarRules = (EntityCollection)item.Attributes["calendarrules"];
                    foreach (Entity calendarRule in calendarRules.Entities)
                    {

                        var startDate = Convert.ToDateTime(calendarRule["effectiveintervalstart"]);
                        var endDate = Convert.ToDateTime(calendarRule["effectiveintervalend"]).Subtract(new TimeSpan(0, 1, 0));



                        var dayOfWeek = new GregorianCalendar().GetDayOfWeek(startDate);
                        Feriados.Add(new Feriado(this.OrganizationName, this.IsOffline, Convert.ToString(calendarRule["name"]), dayOfWeek, startDate, endDate));
                    }
                }
            }

            return Feriados;
        }
        
    }
}
