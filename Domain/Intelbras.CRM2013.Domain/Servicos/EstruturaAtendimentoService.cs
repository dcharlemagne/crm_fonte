using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class EstruturaAtendimentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EstruturaAtendimentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public EstruturaAtendimentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public EstruturaAtendimento Persistir(Model.EstruturaAtendimento ObjEstruturaAtendimento)
        {

            EstruturaAtendimento TmpEstruturaAtendimento = null;
            if (ObjEstruturaAtendimento.ID.HasValue)
            {
                TmpEstruturaAtendimento = RepositoryService.EstruturaAtendimento.ObterPor(ObjEstruturaAtendimento.ID.Value);

                if (TmpEstruturaAtendimento != null)
                {
                    ObjEstruturaAtendimento.ID = TmpEstruturaAtendimento.ID;

                    RepositoryService.EstruturaAtendimento.Update(ObjEstruturaAtendimento);

                    if (!TmpEstruturaAtendimento.State.Equals(ObjEstruturaAtendimento.State) && ObjEstruturaAtendimento.State != null)
                        this.MudarStatus(TmpEstruturaAtendimento.ID.Value, ObjEstruturaAtendimento.State.Value);

                    return TmpEstruturaAtendimento;
                }
                else 
                    return null;
            }
            else
            {
                ObjEstruturaAtendimento.ID = RepositoryService.EstruturaAtendimento.Create(ObjEstruturaAtendimento);
                return ObjEstruturaAtendimento;
            }
        }

        public EstruturaAtendimento BuscaEstruturaAtendimento(Guid estruturaAtendimentoID)
        {
            List<EstruturaAtendimento> lstEstruturaAtendimento = RepositoryService.EstruturaAtendimento.ListarPor(estruturaAtendimentoID);
            if (lstEstruturaAtendimento.Count > 0)
                return lstEstruturaAtendimento.First<EstruturaAtendimento>();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.EstruturaAtendimento.AlterarStatus(id, status);
        }

        #endregion

    }
}
