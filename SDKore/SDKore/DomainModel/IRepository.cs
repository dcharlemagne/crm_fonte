using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;

namespace SDKore.DomainModel
{

    /// <summary>
    /// Interfaces de repositórios
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Define a organização a ser utilizada.
        /// </summary>
        /// <param name="organization">Organização</param>
        void SetOrganization(string organization);

        /// <summary>
        /// Define a forma de acesso. Online ou Offline
        /// </summary>
        /// <param name="isOffline"></param>
        void SetIsOffline(bool isOffline);

        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request);
        Microsoft.Xrm.Sdk.OrganizationResponse Execute(Microsoft.Xrm.Sdk.OrganizationRequest request, Guid userID);

        /// <summary>
        /// Cria um registro.
        /// </summary>
        /// <param name="entity">Registro a ser criada</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>Id do registro criado.</returns>
        Guid Create(T entity, Guid userId);

        /// <summary>
        /// Cria um registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entity">Registro a ser criada</param>
        /// <returns>Id do registro criado.</returns>
        Guid Create(T entity);

        /// <summary>
        /// Cria um registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="collection">Lista de registro a ser criados</param>
        DomainExecuteMultiple Create(List<T> collection);

        /// <summary>
        /// Atualiza um registro.
        /// </summary>
        /// <param name="entity">Registro a ser atualizado</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        void Update(T entity, Guid userId);

        /// <summary>
        /// Atualiza um registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entity">Registro a ser atualizado</param>
        void Update(T entity);

        /// <summary>
        /// Atualiza uma lista de registro.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="collection">Lista de registro a ser atualizados</param>
        Dictionary<Guid, string> Update(List<T> collection);

        /// <summary>
        /// Atualiza o status de um registro.
        /// </summary>
        /// <param name="entity">Registro a ser atualizado. Obs: Esta entidade precisa ter os campos statecode e statuscode mapeados</param>
        /// <param name="status">Status que irá sobrescrever o status atual</param>
        void SetState(T entity, int status);

        /// <summary>
        /// Atualiza o status de multiplus registros.
        /// </summary>
        /// <param name="collection">Registro a ser atualizado. Obs: Esta entidade precisa ter os campos statecode e statuscode mapeados</param>
        /// <param name="status">Status que irá sobrescrever o status atual</param>
        void SetStateMultiple(List<T> collection, int status);


        /// <summary>
        /// Executa múltiplas requisições.
        /// </summary>
        /// <param name="collection">Registros a serem atualizados, criados ou deletados.</param>
        /// <param name="requestOperation">Operações válidas são UpdateRequest, CreateRequest e DeleteRequest.</param>
        KeyValuePair<ExecuteMultipleResponse, ExecuteMultipleRequest> ExecuteMultiple(List<T> collection, OrganizationRequest requestOperation);

        /// <summary>
        /// Apaga um registro
        /// </summary>
        /// <param name="entityId">Id do registro a ser apagado.</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        void Delete(Guid entityId, Guid userId);

        /// <summary>
        /// Apaga um registro
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro a ser apagado.</param>
        void Delete(Guid entityId);

        /// <summary>
        /// Obtém um registro. 
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        T Retrieve(Guid entityId, Guid userId);

        /// <summary>
        /// Obtém um registro. 
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        T Retrieve(Guid entityId);

        /// <summary>
        /// Obtém um registro. 
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="entityId">Id do registro</param>
        /// <param name="columns">Atributos do CRM para retorno</param>
        /// <returns>O objeto de domínio preenchido.</returns>
        T Retrieve(Guid entityId, params string[] columns);

        /// <summary>
        /// Obtém uma coleção de registros.
        /// </summary>
        /// <param name="queryBase">Consulta a ser executada (QueryExpression).</param>
        /// <param name="userId">Usuário que efetuará a ação</param>
        /// <returns>Coleção de objetos de domínio preenchidos.</returns>
        DomainCollection<T> RetrieveMultiple(object queryBase, Guid userId);

        /// <summary>
        /// Obtém uma coleção de registros com opção de utilizar o usuário administrador do CRM.
        /// <para>Ação executado com o usuário Master (CRM ADMIN)</para>
        /// </summary>
        /// <param name="queryBase">Consulta a ser executada (QueryExpression).</param>
        /// <returns>Coleção de objetos de domínio preenchidos.</returns>
        DomainCollection<T> RetrieveMultiple(object queryBase);

        /// <summary>
        /// Define o Provider a ser utilizado
        /// </summary>
        /// <param name="provider">Provider a ser utilizado (ServiceProxy para CRM e Connection para SQL)</param>
        void SetProvider(object provider);
    }
}
