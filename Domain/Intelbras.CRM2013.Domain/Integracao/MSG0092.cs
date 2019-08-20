using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0092 : Base, IBase<Intelbras.Message.Helper.MSG0092, Domain.Model.Pedido>
    {
        #region Propriedades
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor
        public MSG0092(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            usuarioIntegracao = usuario;
            Pollux.MSG0092 polluxMsg0092 = this.CarregarMensagem<Pollux.MSG0092>(mensagem);

            if (String.IsNullOrEmpty(polluxMsg0092.NumeroPedido))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Número Pedido é um campo obrigatório";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0092R1>(numeroMensagem, retorno);
            }
            Pedido objPedido = new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(polluxMsg0092.NumeroPedido);

            if (objPedido == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Pedido não encontrado!";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0092R1>(numeroMensagem, retorno);
            }

            if (!String.IsNullOrEmpty(polluxMsg0092.MotivoCancelamento))
                objPedido.DescricaoCancelamento = polluxMsg0092.MotivoCancelamento;

            try
            {
                //Se o status já foi cancelado não precisa mudar
                if (objPedido.Status != (int)Domain.Enum.Pedido.StateCode.Cancelada && objPedido.RazaoStatus != (int)Domain.Enum.Pedido.RazaoStatus.Cancelado)
                {
                    if(!String.IsNullOrEmpty(polluxMsg0092.MotivoCancelamento))
                        new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).Persistir(objPedido);

                    if (false == new Intelbras.CRM2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).MudarStatusPedido(objPedido.ID.Value, (int)Domain.Enum.Pedido.StateCode.Cancelada, (int)Domain.Enum.Pedido.RazaoStatus.Cancelado))
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Não foi possível cancelar o pedido!";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0092R1>(numeroMensagem, retorno);
                    }
                }
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao cancelar o pedido.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0092R1>(numeroMensagem, retorno);
            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0092R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Pedido DefinirPropriedades(Intelbras.Message.Helper.MSG0092 xml)
        {
            Pedido crm = new Pedido(this.Organizacao, this.IsOffline);

            return crm;
        }

        #endregion

        #region Métodos Auxiliares


        public string Enviar(Pedido objModel)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
