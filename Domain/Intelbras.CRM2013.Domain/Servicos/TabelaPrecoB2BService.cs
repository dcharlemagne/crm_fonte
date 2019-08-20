using System;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TabelaPrecoB2BService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TabelaPrecoB2BService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TabelaPrecoB2BService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }
        
        #endregion

        #region Métodos

        public TabelaPrecoB2B Persistir(TabelaPrecoB2B tabelaPreco, ItemTabelaPrecoB2B item)
        {
            TabelaPrecoB2B tabelaPrecoAtual = RepositoryService.TabelaPrecoB2B.ObterPor(tabelaPreco.CodigoTabelaPrecoEMS);
            if (tabelaPrecoAtual == null)
            {
                tabelaPreco.ID = RepositoryService.TabelaPrecoB2B.Create(tabelaPreco);
                return this.SaveItem(tabelaPreco, item);
            }

            tabelaPreco.ID = tabelaPrecoAtual.ID;

            RepositoryService.TabelaPrecoB2B.Update(tabelaPreco);
            if (tabelaPreco.Status != null && !tabelaPrecoAtual.Status.Equals(tabelaPreco.Status))
            {
                MudarStatus(tabelaPreco.ID.Value, tabelaPreco.Status.Value);
            }

            return this.SaveItem(tabelaPreco, item);
        }

        private TabelaPrecoB2B SaveItem(TabelaPrecoB2B tabelaPreco, ItemTabelaPrecoB2B item)
        {
            // Não foi passado nenhum item... não é necessário persistir
            if (String.IsNullOrEmpty(item.CodigoItemPreco))
            {
                return tabelaPreco;
            }

            // find no item
            ItemTabelaPrecoB2B itemAtual = RepositoryService.ItemTabelaPrecoB2B.ObterPor(tabelaPreco.ID, item.CodigoItemPreco);
            // Novo item
            if (itemAtual == null)
            {
                item.TabelaPreco = new Lookup(tabelaPreco.ID.Value, "");
                RepositoryService.ItemTabelaPrecoB2B.Create(item);

                return tabelaPreco;
            }

            // Atualiza item
            item.ID = itemAtual.ID;
            
            RepositoryService.ItemTabelaPrecoB2B.Update(item);
            if (item.Status != null && !itemAtual.Status.Equals(item.Status))
                MudarStatusItem(item.ID.Value, item.Status.Value);

            return tabelaPreco;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.TabelaPrecoB2B.AlterarStatus(id, status);
        }
        #endregion

        public bool MudarStatusItem(Guid id, int status)
        {
            return RepositoryService.ItemTabelaPrecoB2B.AlterarStatus(id, status);
        }
    }
}
