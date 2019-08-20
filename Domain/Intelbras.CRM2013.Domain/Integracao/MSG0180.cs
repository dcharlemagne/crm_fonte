using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0180 : Base, IBase<Intelbras.Message.Helper.MSG0180, Domain.Model.Conta>
    {
        //A aplicação deverá disponibilizar um botão no "Ribbon" de canais classificados como "Revendas" para que a mensagem "MSG0180 - CONSULTA_DISTRIBS_REVENDA" 
        //seja enviada e listados em um pop up/modal a razão social, nome fantasia, cnpj,  código de emitente e data do último envio de sell out para a revenda. 
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        public MSG0180(string org, bool isOffline) : base(org, isOffline) { }

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public List<Conta> Executar(string revendaIDnoCRM, string numeroMensagem)
        {
            List<Conta> contas = new List<Conta>();
            try
            {
                //Pollux.MSG0180 msg = new Pollux.MSG0180(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.SellOut), revendaIDnoCRM);
                Pollux.MSG0180 msg = new Pollux.MSG0180(Enum.Sistemas.RetornaSistema(Enum.Sistemas.Sistema.CRM), revendaIDnoCRM);
                msg.CodigoConta = revendaIDnoCRM;
                msg.DataInicio = DateTime.Now.AddDays(-9999);
                msg.DataFim = DateTime.Now;
                
                string xml = msg.GenerateMessage();

                Servicos.Integracao integ = new Servicos.Integracao(this.Organizacao, this.IsOffline);
                string retMsg = String.Empty;
                if (integ.EnviarMensagemBarramento(xml, "1", "1", out retMsg))
                {
                    var ret = CarregarMensagem<Pollux.MSG0180R1>(retMsg);
                    if (!ret.Resultado.Sucesso)
                    {
                        throw new ArgumentException("(CRM) " + ret.Resultado.Mensagem);
                    }
                    else
                    {
                        foreach(Pollux.Entities.Distribuidor dist in ret.Distribuidores)
                        {
                            Conta conta = new Servicos.ContaService(Organizacao, IsOffline).BuscaConta(new Guid(dist.CodigoDistribuidor));
                            conta.DataUltimoSelloutRevenda = dist.DataUltimoSellOut;
                            contas.Add(conta);
                        }
                    }
                }
                else
                {
                    var erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                    throw new ArgumentException("(CRM) " + string.Concat(erro001.GenerateMessage(false)));
                }
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Mensagem = Error.Handler(ex);
                resultadoPersistencia.Sucesso = false;
            }

            return contas;
        }

        public Conta DefinirPropriedades(Intelbras.Message.Helper.MSG0180 xml)
        {
            if (string.IsNullOrEmpty(xml.CodigoConta))
            {
                throw new ArgumentException("(CRM) O campo 'CodigoConta' é obrigatório!");
            }
            var conta = new Conta(this.Organizacao, this.IsOffline)
            {
                ID = new Guid(xml.CodigoConta)
            };

            return conta;
        }

        public string Enviar(Conta objModel)
        {
            throw new NotImplementedException();
        }
    }
}