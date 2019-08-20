using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ListaPrecoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaPrecoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ListaPrecoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public ListaPreco BuscaListaPreco(String nomeListaPreco)
        {
            List<ListaPreco> lstnomeListaPreco = RepositoryService.ListaPreco.ListarPor(nomeListaPreco);
            if (lstnomeListaPreco.Count > 0)
                return lstnomeListaPreco.First<ListaPreco>();
            return null;
        }


        public void VerificaBUItemXListaDePreco(ItemListaPreco Item)
        {
            if (Item.ListaPrecos != null && Item.ListaPrecos.Name != null && Item.ListaPrecos.Name.Equals("Lista Padrão"))
                return;
            
            if(RepositoryService.Produto.Retrieve(Item.ProdutoID.Id).UnidadeNegocio.Id != RepositoryService.ListaPreco.Retrieve(Item.ListaPrecos.Id).UnidadeNegocio.Id)
               throw new ArgumentException("O Produto do item, pertence a uma unidade de negócio diferente da Unidade de Negócio da Lista de Preço");
        }

        public ListaPreco Persistir(ListaPreco listaPreco)
        {
            List<ListaPreco> TmpListaPreco = RepositoryService.ListaPreco.ListarPor(listaPreco.Nome);

            if (TmpListaPreco.Count() > 0)
            {
                listaPreco.ID = TmpListaPreco.First<ListaPreco>().ID;
                RepositoryService.ListaPreco.Update(listaPreco);
                return TmpListaPreco.First<ListaPreco>();
            }
            else
                listaPreco.ID = RepositoryService.ListaPreco.Create(listaPreco);
            return listaPreco;
        }

        public ItemListaPreco Persistir(Product produto)
        {
            ItemListaPreco itemListaPreco = new ItemListaPreco();
            itemListaPreco.ProdutoID = new Lookup(produto.Id, "product");
            List<ListaPreco> TmpListaPreco = RepositoryService.ListaPreco.ListarPor("Lista Padrão");
            itemListaPreco.ListaPrecos = new Lookup(TmpListaPreco.First<ListaPreco>().Id, "pricelevel");
            itemListaPreco.Valor = 0;
            itemListaPreco.Porcentual = 0;
            itemListaPreco.MetodoPrecificacao = 1;
            itemListaPreco.OpcaoVendaParcial = 1;
            itemListaPreco.ValorArredondamento = 0;
            itemListaPreco.OpcaoArredondamento = 2;
            itemListaPreco.Moeda = new Lookup(produto.Moeda.Id, "");
            itemListaPreco.Unidade = new Lookup(produto.UnidadePadrao.Id, "");

            itemListaPreco.ID = RepositoryService.ItemListaPreco.Create(itemListaPreco);
            return itemListaPreco;
        }

        /// <summary>
        /// Retorna true se o registro é duplicado
        /// </summary>
        /// <param name="listaPMA"></param>
        /// <returns></returns>
        public bool ValidarExistencia(Model.ListaPreco listaPMA, List<Guid> lstEstados)
        {
            Guid? listaGuid = Guid.Empty;

            #region Validação de valores
            if (listaPMA.UnidadeNegocio == null)
                throw new ArgumentException("Unidade de Negócio não informada.");
            if (!listaPMA.DataInicio.HasValue)
                throw new ArgumentException("Data início não informada.");
            if (!listaPMA.DataTermino.HasValue)
                throw new ArgumentException("Data fim não informada.");

            //Verificamos se ele enviou o proprio guid pois a funcao eh usada no create/update
            if (listaPMA.ID.HasValue)
            {
                //se enviou o guid é porque nao é create, ai pega o guid do registro e pega os estados relacionados
                listaGuid = listaPMA.ID.Value;
                if (lstEstados.Count <= 0)
                {
                    List<ListaPrecoXEstado> lstPrecoPsdEstado = RepositoryService.ListaPrecoXEstado.ListarPor(null, listaGuid);
                    foreach (var _registro in lstPrecoPsdEstado)
                    {
                        lstEstados.Add(_registro.EstadoId.Value);
                    }
                }
            }
            else
                listaGuid = null;

            #endregion
            var lista = RepositoryService.ListaPreco.ListarPor(listaPMA.UnidadeNegocio.Id, lstEstados, listaGuid, listaPMA.DataInicio.Value, listaPMA.DataTermino.Value);
            if (lista.Count == 0)
                return false;

            return true;
        }

        public ListaPreco ObterPor(Guid PmaId)
        {
            return RepositoryService.ListaPreco.ObterPor(PmaId);
        }
    }
}
