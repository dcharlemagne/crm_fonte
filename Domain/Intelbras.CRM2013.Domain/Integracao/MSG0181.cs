using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0181 : Base, IBase<Intelbras.Message.Helper.MSG0181, Domain.Model.Conta>
    {
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        public MSG0181(string org, bool isOffline) : base(org, isOffline) { }

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                retorno.Add("Resultado", this.resultadoPersistencia);

                var xml = this.CarregarMensagem<Pollux.MSG0181>(mensagem);
                var objeto = this.DefinirPropriedades(xml);

                new Servicos.ContaService(Organizacao, IsOffline).Persistir(objeto);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(ex);
                resultadoPersistencia.Sucesso = false;
            }

            return CriarMensagemRetorno<Pollux.MSG0181R1>(numeroMensagem, retorno);
        }

        public Model.Conta DefinirPropriedades(Intelbras.Message.Helper.MSG0181 xml)
        {
            if (string.IsNullOrEmpty(xml.CodigoConta))
            {
                throw new ArgumentException("(CRM) O campo 'CodigoConta' é obrigatório!");
            }


            var conta = new Conta(this.Organizacao, this.IsOffline)
            {
                ID = new Guid(xml.CodigoConta)
            };

            if (xml.DataUltimoSellOut.HasValue)
            {
                conta.DataUltimoSelloutRevenda = xml.DataUltimoSellOut;
            }
            else
            {
                conta.AddNullProperty("DataUltimoSelloutRevenda");
            }
            //Adicionado par não enviar a mensagem de conta.
            conta.IntegrarNoPlugin = true;

            return conta;
        }

        public string Enviar(Conta objModel)
        {
            throw new NotImplementedException();
        }
    }
}