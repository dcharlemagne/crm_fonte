using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Configuration;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SDKore.Crm
{
    public abstract class CrmServiceRepository<T>
    {
        #region Propriedades

        private object _linq = null;
        private object _provider = null;
        public bool IsOffline { get; set; }
        public string OrganizationName { get; set; }

        #endregion

        #region Providers

        /// <summary>
        /// Provider de acesso a dados do OrganizationServiceContext
        /// <para>Linq</para>
        /// </summary>
        protected OrganizationServiceContext Linq
        {
            get
            {
                if (this._linq == null)
                    this._linq = new OrganizationServiceContext((IOrganizationService)this.Provider);

                return (OrganizationServiceContext)this._linq;
            }
        }

        /// <summary>
        /// Provedor de acesso a dados do OrganizationService
        /// <para>Não utilizar o Provider. Acesse os Métodos CREATE, UPDATE, DELETE, RETRIEVE, RETRIEVEMULTIPLE e EXECUTE</para>
        /// </summary>
        protected OrganizationServiceProxy Provider
        {
            get
            {
                if (_provider == null)
                {
                    CrmHelper crmHelper = new CrmHelper();
                    _provider = crmHelper.GetOrganizationProxy(OrganizationName, ConfigurationManager.GetUserName(this.OrganizationName), ConfigurationManager.GetPassword(this.OrganizationName), ConfigurationManager.GetDomain(this.OrganizationName), this.IsOffline);
                }

                return (OrganizationServiceProxy)_provider;
            }
            private set
            {
                this._provider = value;
            }
        }

        /// <summary>
        /// Define o Provider a ser utilizado (para execução via plugin)
        /// </summary>
        public void SetProvider(object provider)
        {
            if (provider != null)
            {
                OrganizationServiceProxy tmpService = provider as OrganizationServiceProxy;

                if (tmpService == null)
                {
                    throw new ArgumentException("O Provider deve ser do Tipo OrganizationServiceProxy.");
                }

                Provider = tmpService;
            }
        }
        
        #endregion

        #region Métodos Públicos
        /// <summary>
        /// Define a organização a ser utilizada.
        /// </summary>
        /// <param name="organization">Organização</param>
        public void SetOrganization(string organization)
        {
            this.OrganizationName = organization;
        }

        /// <summary>
        /// Obtém o rótulo de um determino valor do OptionSetValeu (Picklist).
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="propertyName">Nome da priedade do domínio.</param>
        /// <param name="value">Valor do atributo.</param>
        /// <returns>Retorna o rótulo referente ao valor informado.</returns>
        public string RetrieveLabelOptionSetValue(string propertyName, int value)
        {
            var dic = this.RetrieveLabelOptionSetValue(propertyName);
            if (dic.ContainsKey(value))
                return dic[value];

            return string.Empty;
        }

        /// <summary>
        /// Obtém os rótulos de um determino OptionSetValue (Picklist).
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="propertyName">Nome da priedade do domínio.</param>
        /// <returns>Retorna um dicionário com o value sendo "key" e rótulo sendo o value</returns>
        public Dictionary<int, string> RetrieveLabelOptionSetValue(string propertyName)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();

            var request = new RetrieveAttributeRequest()
            {
                EntityLogicalName = Utility.GetEntityName<T>(),
                LogicalName = Utility.GetLogicalAttribute<T>(propertyName).Name
            };

            RetrieveAttributeResponse response = (RetrieveAttributeResponse)this.Provider.Execute(request);

            if (response.Results.Count > 0)
            {
                if (response.Results.Values.ElementAt(0).GetType() != typeof(PicklistAttributeMetadata))
                    throw new Exception("Atributo informado não é do tipo OptionSetValue");

                var picklist = new PicklistAttributeMetadata();
                picklist.OptionSet = ((EnumAttributeMetadata)response.Results.Values.ElementAt(0)).OptionSet;
                foreach (var item in picklist.OptionSet.Options)
                {
                    if (!item.Value.HasValue)
                        continue;

                    dic.Add(item.Value.Value, item.Label.LocalizedLabels.ElementAt(0).Label);
                }
            }

            return dic;
        }
        
        /// <summary>
        /// Define se deve-se utilizar o provider Offline para a execução no CRM.
        /// </summary>
        /// <param name="offline"></param>
        public void SetIsOffline(bool offline)
        {
            this.IsOffline = offline;
        }

        #endregion

        #region Create/Update/Delete/Retrieve/Execute

        public virtual OrganizationResponse Execute(OrganizationRequest request)
        {
            return this.Provider.Execute(request);
        }

        public virtual OrganizationResponse Execute(OrganizationRequest request, Guid userID)
        {
            try
            {
                Provider.CallerId = userID;
                return this.Provider.Execute(request);
            }
            catch
            {
                throw;
            }
            finally
            {
                Provider.CallerId = Guid.Empty;
            }
        }

        /// <summary>
        /// Cria um registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entity">Registro a ser criada</param>
        /// <returns>Id do registro criado.</returns>
        public virtual Guid Create(T entity)
        {
            var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
            return this.Provider.Create(ent);
        }

        /// <summary>
        /// Cria um registro.
        /// </summary>
        /// <param name="entity">Registro a ser criada</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>Id do registro criado.</returns>
        public virtual Guid Create(T entity, Guid userID)
        {
            try
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);

                Provider.CallerId = userID;
                return this.Provider.Create(ent);
            }
            catch
            {
                throw;
            }
            finally
            {
                Provider.CallerId = Guid.Empty;
            }
        }

        public virtual DomainExecuteMultiple Create(List<T> collection)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                request.Requests.Add(new CreateRequest() { Target = ent, RequestId = ent.Id });
            }

            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)this.Provider.Execute(request);

            DomainExecuteMultiple domainExecuteMultiple = new DomainExecuteMultiple();
            domainExecuteMultiple.IsFaulted = responseWithResults.IsFaulted;
            domainExecuteMultiple.List = new DomainExecuteMultipleItem[collection.Count];

            foreach (var item in responseWithResults.Responses)
            {
                domainExecuteMultiple.List[item.RequestIndex] = new DomainExecuteMultipleItem()
                {
                    ID = request.Requests[item.RequestIndex].RequestId.Value,
                    IsFaulted = (item.Fault != null),
                    Message = (item.Fault == null) ? null : item.Fault.Message
                };
            }

            return domainExecuteMultiple;
        }

        /// <summary>
        /// Atualiza um registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entity">Registro a ser atualizado</param>
        public virtual void Update(T entity)
        {
            var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
            this.Provider.Update(ent);
        }

        /// <summary>
        /// Atualiza um registro.
        /// </summary>
        /// <param name="entity">Registro a ser atualizado</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        public virtual void Update(T entity, Guid userId)
        {
            try
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);

                this.Provider.CallerId = userId;
                this.Provider.Update(ent);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Provider.CallerId = Guid.Empty;
            }
        }

        public virtual Dictionary<Guid, string> Update(List<T> collection)
        {
            var listaErros = new Dictionary<Guid, string>();

            ExecuteMultipleRequest request = new ExecuteMultipleRequest()
            {
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = false
                },
                Requests = new OrganizationRequestCollection()
            };

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                request.Requests.Add(new UpdateRequest() { Target = ent, RequestId = ent.Id });
            }

            ExecuteMultipleResponse responseWithResults = (ExecuteMultipleResponse)this.Provider.Execute(request);

            foreach(var item in responseWithResults.Responses)
            {
                if(item.Fault != null)
                {
                    Guid id = request.Requests[item.RequestIndex].RequestId.Value;
                    listaErros.Add(id, item.Fault.Message);
                }
            }

           return listaErros;
        }

        /// <summary>
        /// Atualiza o status de um registro.
        /// </summary>
        /// <param name="entity">Registro a ser atualizado. Obs: Esta entidade precisa ter os campos statecode e statuscode mapeados</param>
        /// <param name="status">Status que irá sobrescrever o status atual</param>
        public virtual void SetState(T entity, int status)
        {
            var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
            this.Provider.Execute(GenerateSetStateRequest(ent, status));
        }

        /// <summary>
        /// Atualiza o status de multiplus registros.
        /// </summary>
        /// <param name="collection">Registro a ser atualizado. Obs: Esta entidade precisa ter os campos statecode e statuscode mapeados</param>
        /// <param name="status">Status que irá sobrescrever o status atual</param>
        public virtual void SetStateMultiple(List<T> collection, int status)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest();
            EntityCollection listEntities = new EntityCollection();
            request.Settings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = false
            };

            request.Requests = new OrganizationRequestCollection();

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                listEntities.Entities.Add(ent);
            }

            foreach (var entity in listEntities.Entities)
            {
                request.Requests.Add(GenerateSetStateRequest(entity, status));
            }

            this.Provider.Execute(request);
        }

        /// <summary>
        /// Executa múltiplas requisições.
        /// </summary>
        /// <param name="collection">Registros a serem atualizados, criados ou deletados.</param>
        /// <param name="requestOperation">Operações válidas são UpdateRequest, CreateRequest e DeleteRequest.</param>
        public virtual KeyValuePair<ExecuteMultipleResponse, ExecuteMultipleRequest> ExecuteMultiple(List<T> collection, OrganizationRequest requestOperation)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest();
            EntityCollection listEntities = new EntityCollection();
            request.Settings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            request.Requests = new OrganizationRequestCollection();

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                listEntities.Entities.Add(ent);
            }

            foreach (var entity in listEntities.Entities)
            {
                if (requestOperation.GetType() == typeof(UpdateRequest))
                {
                    requestOperation = new UpdateRequest() { Target = entity };
                }
                else if(requestOperation.GetType() == typeof(CreateRequest))
                {
                    requestOperation = new CreateRequest() { Target = entity };
                }

                request.Requests.Add(requestOperation);
            }

            return new KeyValuePair<ExecuteMultipleResponse, ExecuteMultipleRequest>((ExecuteMultipleResponse)this.Provider.Execute(request), request);
        }

        /// <summary>
        /// Apaga um registro
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro a ser apagado.</param>
        public virtual void Delete(Guid entityId)
        {

            this.Provider.Delete(Utility.GetEntityName<T>(), entityId);
        }

        public void AssociateManyToMany(EntityReference moniker1, EntityReference moniker2, string strEntityRelationshipName)
        {
            AssociateEntitiesRequest request = new AssociateEntitiesRequest();
            // Set the ID of Moniker1 to the ID of the lead.
            request.Moniker1 = new EntityReference { Id = moniker1.Id, LogicalName = moniker1.Name };
            // Set the ID of Moniker2 to the ID of the contact.
            request.Moniker2 = new EntityReference { Id = moniker2.Id, LogicalName = moniker2.Name };
            // Set the relationship name to associate on.
            request.RelationshipName = strEntityRelationshipName;

            // Execute the request.
            this.Execute(request);
        }

        /// <summary>
        /// Apaga um registro
        /// </summary>
        /// <param name="entityId">Id do registro a ser apagado.</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        public virtual void Delete(Guid entityId, Guid userId)
        {
            try
            {
                this.Provider.CallerId = userId;
                this.Provider.Delete(Utility.GetEntityName<T>(), entityId);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Provider.CallerId = Guid.Empty;
            }
        }

        /// <summary>
        /// Obtém um registro. 
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        //public virtual T Retrieve(Guid entityId)
        //{
        //    Entity entity = this.Provider.Retrieve(Utility.GetEntityName<T>(), entityId, new ColumnSet(GetColumns<T>()));
        //    return (T)entity.Parse<T>(this.OrganizationName, this.IsOffline, this.Provider);
        //}
        public virtual T Retrieve(Guid entityId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition(string.Concat(Utility.GetEntityName<T>(), "id"), ConditionOperator.Equal, entityId);

            var returnValue = RetrieveMultiple(query).List;
            if (returnValue != null && returnValue.Count > 0)
                return returnValue[0];

            return default(T);
        }

        /// <summary>
        /// Obtém um registro. 
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        public virtual T Retrieve(Guid entityId, Guid userId)
        {
            try
            {
                this.Provider.CallerId = userId;
                //Entity entity = this.Provider.Retrieve(Utility.GetEntityName<T>(), entityId, new ColumnSet(GetColumns<T>()));
                //return (T)entity.Parse<T>(this.OrganizationName, this.IsOffline, this.Provider);

                return this.Retrieve(entityId);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Provider.CallerId = Guid.Empty;
            }
        }

        /// <summary>
        /// Obtém um registro. 
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <param name="columns">Atributos do CRM para retorno</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        public virtual T Retrieve(Guid entityId, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            if (columns != null && columns.Length > 0)
                query.ColumnSet.AddColumns(columns);
            query.Criteria.AddCondition(string.Concat(Utility.GetEntityName<T>(), "id"), ConditionOperator.Equal, entityId);
            var returnValue = RetrieveMultiple(query).List;
            if (returnValue != null && returnValue.Count > 0)
                return returnValue[0];
            return default(T);
        }


        /// <summary>
        /// Obtém uma coleção de registros com opção de utilizar o usuário administrador do CRM.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="queryBase">Consulta a ser executada (QueryExpression).</param>
        /// <returns>Coleção de objetos de domínio preenchidos.</returns>
        public virtual DomainCollection<T> RetrieveMultiple(object queryBase)
        {
            this.AddColumnsQueryExpression(ref queryBase);
            this.Provider.Timeout = new TimeSpan(0, 20, 0); //20 minutos
            EntityCollection collection = this.Provider.RetrieveMultiple((QueryBase)queryBase);
            return this.CreateDomainCollection(collection);
        }

        /// <summary>
        /// Obtém uma coleção de registros.
        /// </summary>
        /// <param name="queryBase">Consulta a ser executada (QueryExpression).</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>Coleção de objetos de domínio preenchidos.</returns>
        public virtual DomainCollection<T> RetrieveMultiple(object queryBase, Guid userId)
        {
            try
            {
                this.AddColumnsQueryExpression(ref queryBase);

                this.Provider.CallerId = userId;
                this.Provider.Timeout = new TimeSpan(0, 20, 0); //20 minutos
                EntityCollection collection = this.Provider.RetrieveMultiple((QueryBase)queryBase);
                return this.CreateDomainCollection(collection);
            }
            catch
            {
                throw;
            }
            finally
            {
                this.Provider.CallerId = Guid.Empty;
            }
        }

        /// <summary>
        /// Obtém uma coleção que ultrapasse 5 mil registros.
        /// </summary>
        /// <param name="queryBase">Consulta a ser executada (QueryExpression).</param>
        /// <returns>Coleção de objetos de domínio preenchidos.</returns>
        public virtual DomainCollection<T> RetrieveMultiplePaged(QueryExpression queryBase)
        {
            this.AddColumnsQueryExpression(ref queryBase);

            EntityCollection exit = new EntityCollection();

            int pageNumber = 1;
            RetrieveMultipleRequest multiRequest;
            RetrieveMultipleResponse multiResponse = new RetrieveMultipleResponse();

            do
            {
                queryBase.PageInfo.Count = 5000;
                queryBase.PageInfo.PagingCookie = (pageNumber == 1) ? null : multiResponse.EntityCollection.PagingCookie;
                queryBase.PageInfo.PageNumber = pageNumber++;

                multiRequest = new RetrieveMultipleRequest();
                multiRequest.Query = queryBase;
                this.Provider.Timeout = new TimeSpan(0, 20, 0); //20 minutos
                multiResponse = (RetrieveMultipleResponse)this.Provider.Execute(multiRequest);

                exit.Entities.AddRange(multiResponse.EntityCollection.Entities);
            }
            while (multiResponse.EntityCollection.MoreRecords);

            return this.CreateDomainCollection(exit);
        }
        #endregion

        #region Métodos Privados

        /// <summary>
        /// Adiciona as colunas do negócio a consulta caso não existam colunas especificadas.
        /// </summary>
        /// <param name="queryBase">Query Base</param>
        private void AddColumnsQueryExpression(ref object queryBase)
        {
            if (typeof(QueryExpression) == queryBase.GetType())
            {
                QueryExpression query = (QueryExpression)queryBase;
                AddColumnsQueryExpression(ref query);
            }
        }

        private void AddColumnsQueryExpression(ref QueryExpression queryBase)
        {
            if (queryBase.ColumnSet == null || queryBase.ColumnSet.Columns.Count == 0)
            {
                queryBase.ColumnSet.AddColumns(GetColumns<T>());
                //Coloca condições especiais com a migração do CRM4
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "subject")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "subject")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "annotation")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "annotation")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "systemuser")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "systemuser")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "businessunit")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "businessunit")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "uom")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "uom")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "productpricelevel")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "productpricelevel")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "customeraddress")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "customeraddress")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "salesorderdetail")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "salesorderdetail")
                    queryBase.ColumnSet.Columns.Remove("statecode");
                if (queryBase.ColumnSet.Columns.Contains("statuscode") && queryBase.EntityName.ToLower() == "invoicedetail")
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                if (queryBase.ColumnSet.Columns.Contains("statecode") && queryBase.EntityName.ToLower() == "invoicedetail")
                    queryBase.ColumnSet.Columns.Remove("statecode");

                // Retira as condições de filtro padrão (DomainBase) quando a tabela é "itbc_itbc_politicacomercial_itbc_estado"
                if (queryBase.EntityName.ToLower().StartsWith("itbc_itbc_"))
                {
                    queryBase.ColumnSet.Columns.Remove("statuscode");
                    queryBase.ColumnSet.Columns.Remove("statecode");
                    queryBase.ColumnSet.Columns.Remove("createdon");
                    queryBase.ColumnSet.Columns.Remove("createdby");
                    queryBase.ColumnSet.Columns.Remove("modifiedon");
                    queryBase.ColumnSet.Columns.Remove("modifiedby");
                }

                if (queryBase.ColumnSet.Columns.Contains("id"))
                    queryBase.ColumnSet.Columns.Remove("id");

                if (!queryBase.ColumnSet.Columns.Contains(queryBase.EntityName.ToLower() + "id"))
                {
                    if(queryBase.EntityName.ToLower() == "task" || queryBase.EntityName.ToLower() == "email" || queryBase.EntityName.ToLower() == "codek_livechat_tracking")
                    {
                        queryBase.ColumnSet.Columns.Add("activityid");
                    }
                    else
                    {
                        queryBase.ColumnSet.Columns.Add(queryBase.EntityName.ToLower() + "id");
                    }
                }
            }
        }

        /// <summary>
        /// Obtém os nomes lógicos das propriedades do domínio.
        /// </summary>
        /// <typeparam name="T">Domínio.</typeparam>
        /// <returns></returns>
        private string[] GetColumns<T1>()
        {
            List<string> listReturn = new List<string>();
            var listProperties = typeof(T1).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in listProperties)
            {
                var logicalAttribute = property.GetCustomAttributes(typeof(SDKore.Crm.Util.LogicalAttribute), false);
                if (logicalAttribute == null || logicalAttribute.Length == 0) continue;
                //não usa o id do domain base pois ele deve ser convertido em entidade+id
                if (((SDKore.Crm.Util.LogicalAttribute)logicalAttribute[0]).Name.ToLower() == "id") continue;
                listReturn.Add(((SDKore.Crm.Util.LogicalAttribute)logicalAttribute[0]).Name.ToLower());
            }

            return listReturn.ToArray();
        }

        /// <summary>
        /// Cria lista de registros retornados da pesquisa.
        /// </summary>
        /// <param name="collection">Coleção de entidades do CRM.</param>
        /// <returns></returns>
        private IList<T> CreateIList(EntityCollection collection)
        {
            List<T> list = new List<T>();
            for (int index = 0; index < collection.Entities.Count; index++)
            {
                Entity entity = collection.Entities[index];
                list.Add((T)entity.Parse<T>(this.OrganizationName, this.IsOffline, this.Provider));
            }
            return list;
        }

        /// <summary>
        /// Obtém instância do QueryExpression configurado com o nome da entidade.
        /// </summary>
        /// <typeparam name="T">Classe de domínio.</typeparam>
        /// <param name="nolock">Informe true para usar nolock nas consultas.</param>
        /// <returns></returns>
        public static QueryExpression GetQueryExpression<T1>(bool nolock)
        {
            var query = new QueryExpression(Utility.GetEntityName<T1>());
            query.NoLock = nolock;
            return query;
        }

        /// <summary>
        /// Cria uma coleção de domínio.
        /// </summary>
        /// <param name="collection">Coleção de registros do CRM.</param>
        /// <returns></returns>
        private DomainModel.DomainCollection<T> CreateDomainCollection(EntityCollection collection)
        {
            SDKore.DomainModel.DomainCollection<T> domainCollection = new DomainModel.DomainCollection<T>();
            domainCollection.List = this.CreateIList(collection);
            domainCollection.EntityName = collection.EntityName;
            domainCollection.MoreRecords = collection.MoreRecords;
            domainCollection.PagingCookie = collection.PagingCookie;
            domainCollection.TotalRecordCount = collection.TotalRecordCount;
            domainCollection.TotalRecordCountLimitExceeded = collection.TotalRecordCountLimitExceeded;

            return domainCollection;
        }

        /// <summary>
        /// Cria uma requisição de alteração de Status.
        /// </summary>
        /// <param name="entity">Registro do CRM.</param>
        /// <param name="status">Status a ser atualizado.</param>
        private SetStateRequest GenerateSetStateRequest(Entity entity, int status)
        {
            SetStateRequest state = new SetStateRequest();
            state.State = entity.GetAttributeValue<OptionSetValue>("statecode"); //Ativo
            state.Status = new OptionSetValue(status);
            state.EntityMoniker = new EntityReference(entity.LogicalName, entity.Id);

            return state;
        }

        #endregion
    }
}
