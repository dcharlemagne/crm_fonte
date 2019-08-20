using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.DomainModel
{
    /// <summary>
    /// Classe de metadados dos domínios.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DomainCollection<T>
    {
        /// <summary>
        /// Lista de registros.
        /// </summary>
        public IList<T> List { get; set; }
        /// <summary>
        /// Nome lógico da entidade
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// Obtém ou define se há mais registros disponíveis.
        /// </summary>
        public bool MoreRecords { get; set; }
        /// <summary>
        /// Obtém ou define as informações da paginação atual.
        /// </summary>
        public string PagingCookie { get; set; }
        /// <summary>
        /// Obtém o número total de registros na coleção.
        /// </summary>
        public int TotalRecordCount { get; set; }
        /// <summary>
        /// Obtém ou define se os resultados da consulta excede o registro contagem total.
        /// </summary>
        public bool TotalRecordCountLimitExceeded { get; set; }
    }
}
