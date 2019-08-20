using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.Helper.Cache
{
    public interface ICacheManager
    {
        /// <summary>
        /// Adiciona um objeto em cache.
        /// </summary>
        /// <param name="key">Chave do objeto em cache</param>
        /// <param name="value">Objeto que será armazenado em cache</param>
        /// <param name="hours">Tempo em que o objeto ficará em cache.<para>Tempo defino em horas.</para></param>
        void Add(string key, object value, int hours);

        /// <summary>
        /// Adiciona um objeto em cache.
        /// </summary>
        /// <param name="key">Chave do objeto em cache</param>
        /// <param name="value">Objeto que será armazenado em cache</param>
        /// <param name="timeSpan">Tempo em que o objeto ficará em cache.</param>
        void Add(string key, object value, TimeSpan timeSpan);

        /// <summary>
        /// Atualiza um objeto que esteja em cache
        /// </summary>
        /// <param name="key">Chave do objeto em cache</param>
        /// <param name="value">Objeto que será atualizado</param>
        void Update(string key, object value);
        /// <summary>
        /// Apaga o objeto do cache.
        /// </summary>
        /// <param name="key">Chave do objeto em cache</param>
        void Delete(string key);
        /// <summary>
        /// Obtém um objeto do cache
        /// </summary>
        /// <param name="key">Chave do objeto em cache</param>
        /// <returns>Retorna um objeto que foi armazenado no cache.</returns>
        object Get(string key);
    }
}
