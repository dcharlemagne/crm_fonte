using Microsoft.Xrm.Sdk;
using System;

namespace Microsoft.Xrm.Sdk
{
    public static class ITracingServiceExtension
    {
        #region Trace e ErrorHandle

        /// <summary>
        /// Cria uma linha no log de execução do plugin
        /// </summary>
        /// <param name="message">Mensagem a ser exibida</param>
        /// <param name="args">Argumentos passados como {0}, {1}... no parâmetro message</param>
        public static void Trace(this ITracingService tracing, string message, params object[] args)
        {
            tracing.Trace(message, args);
        }

        #endregion

    }
}
