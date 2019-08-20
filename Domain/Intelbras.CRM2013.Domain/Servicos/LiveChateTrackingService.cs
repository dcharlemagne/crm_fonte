using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class LiveChatTrackingService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public LiveChatTrackingService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public LiveChatTrackingService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public LiveChatTracking ObterPorOcorrenciaReferenciada(Guid ocorrenciaId)
        {
            return RepositoryService.LiveChatTracking.ObterPorOcorrenciaReferenciada(ocorrenciaId);
        }
        #endregion
    }
}