using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.IRepository
{
    public interface IPedido<T>:IRepository<T>,IRepositoryBase
    {
        List<T> ListarPor(Guid salesorderid);
        T ObterPor(Guid salesorderid);
        T ObterPor(String pedidoEMS, params string[] columns);
        void AtribuirEquipeParaPedido(Guid Equipe, Guid Pedido);
        Boolean AlterarStatus(Guid id, int stateCode, int statusCode);
        Boolean AlterarStatus(Guid guidId, int status);
        Boolean FecharPedido(Guid pedidoId);

        //CRM4
        bool RemoveItemPedidoB2BnoCRM(string codigoItem, string codigoPedido);
        Pedido SalvarPedidoB2BnoCRM(Pedido pedidoDeVenda);
        string SalvarPedidoB2BnoEMS(Pedido pedidoDeVenda);
        List<Pedido> ListarPedidosPor(Model.Conta cliente);
        T ObterPedidosPor(string campo, string valor);
        void EntradaPedidoASTEC(string pedidoXml, out string xmlRetorno);
        List<Pedido> ListarItensDoPedidoNoERPPor(int CodigoDoPedido, string Aprovador);
        string SalvarItensDoPedidoBloqueadoNoERPPor(List<Pedido> pedidos);
        //CRM4
    }
}
