using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0179 : Base, IBase<Intelbras.Message.Helper.MSG0179, Domain.Model.HistoricoDistribuidor>
    {
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        public MSG0179(string org, bool isOffline) : base(org, isOffline) { }

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                retorno.Add("Resultado", this.resultadoPersistencia);

                var xml = this.CarregarMensagem<Pollux.MSG0179>(mensagem);
                var objeto = this.DefinirPropriedades(xml);

                Guid historicoDistribuidorId = new Servicos.HistoricoDistribuidorService(Organizacao, IsOffline).Persistir(objeto);

                retorno.Add("CodigoHistoricoDistribuidor", historicoDistribuidorId.ToString());

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(ex);
                resultadoPersistencia.Sucesso = false;
            }

            return CriarMensagemRetorno<Pollux.MSG0179R1>(numeroMensagem, retorno);
        }

        public Model.HistoricoDistribuidor DefinirPropriedades(Intelbras.Message.Helper.MSG0179 xml)
        {

            if (string.IsNullOrEmpty(xml.CodigoDistribuidor))
            {
                throw new ArgumentException("(CRM) O campo 'CodigoDistribuidor' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.CodigoConta))
            {
                throw new ArgumentException("(CRM) O campo 'CodigoConta' é obrigatório!");
            }

            if (!xml.DataInicio.HasValue)
            {
                throw new ArgumentException("(CRM) O campo 'DataInicio' é obrigatório!");
            }

            if (string.IsNullOrEmpty(xml.Nome))
            {
                throw new ArgumentException("(CRM) O campo 'Nome' é obrigatório!");
            }

            var historicoDistribuidor = new HistoricoDistribuidor(this.Organizacao, this.IsOffline)
            {
                Nome = xml.Nome,
                Distribuidor = new SDKore.DomainModel.Lookup(new Guid(xml.CodigoDistribuidor), SDKore.Crm.Util.Utility.GetEntityName<Conta>()),
                Revenda = new SDKore.DomainModel.Lookup(new Guid(xml.CodigoConta), SDKore.Crm.Util.Utility.GetEntityName<Conta>()),
                DataInicio = xml.DataInicio,
                DataFim = xml.DataFim,
                MotivoTroca = xml.MotivoTroca,
                Status = xml.Situacao
            };

            //verifica se tem distribuidor pai
            if (!string.IsNullOrEmpty(xml.CodigoHistoricoDistribuidorAnterior))
            {
                historicoDistribuidor.DistribuidorPai = new SDKore.DomainModel.Lookup(new Guid(xml.CodigoHistoricoDistribuidorAnterior), SDKore.Crm.Util.Utility.GetEntityName<Conta>());
            }

            //Controle Looping Msg
            historicoDistribuidor.IntegrarNoPlugin = true;

            Guid id;

            if (Guid.TryParse(xml.CodigoHistoricoDistribuidor, out id))
            {
                historicoDistribuidor.ID = id;
            }

            return historicoDistribuidor;
        }

        public Intelbras.Message.Helper.MSG0179 DefinirPropriedades(HistoricoDistribuidor historicoDistribuidor)
        {
            var xml = new Intelbras.Message.Helper.MSG0179(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), historicoDistribuidor.Nome.Truncate(40));

            xml.CodigoHistoricoDistribuidor = historicoDistribuidor.ID.Value.ToString();
            xml.Nome = historicoDistribuidor.Nome;
            xml.MotivoTroca = historicoDistribuidor.MotivoTroca;
            if (historicoDistribuidor.DataInicio.HasValue)
                xml.DataInicio = historicoDistribuidor.DataInicio.Value.ToLocalTime();
            else
                xml.DataInicio = null;
            if (historicoDistribuidor.DataFim.HasValue)
                xml.DataFim = historicoDistribuidor.DataFim.Value.ToLocalTime();
            else
                xml.DataFim = null;
            xml.Situacao = (historicoDistribuidor.Status.HasValue ? historicoDistribuidor.Status.Value : (int)Enum.StateCode.Ativo);

            if (historicoDistribuidor.Revenda != null)
            {
                xml.CodigoConta = historicoDistribuidor.Revenda.Id.ToString();
            }

            if (historicoDistribuidor.Distribuidor != null)
            {
                xml.CodigoDistribuidor = historicoDistribuidor.Distribuidor.Id.ToString();
            }

            if (historicoDistribuidor.DistribuidorPai != null)
            {
                xml.CodigoHistoricoDistribuidorAnterior = historicoDistribuidor.DistribuidorPai.Id.ToString();
            }

            return xml;
        }

        public string Enviar(HistoricoDistribuidor objModel)
        {
            var mensagem = DefinirPropriedades(objModel);
            var integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            string retMsg = string.Empty;

            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                var retorno = CarregarMensagem<Pollux.MSG0179R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException(string.Concat("(CRM) Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }
    }
}