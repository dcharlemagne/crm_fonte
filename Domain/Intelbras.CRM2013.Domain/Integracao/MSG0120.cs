using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0120 : Base, IBase<Message.Helper.MSG0120, Domain.Model.Conta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0120(string org, bool isOffline)
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
            var objCanais = this.CarregarMensagem<Pollux.MSG0120>(mensagem);

            List<Conta> lstConta = this.GerarListaConta(objCanais.CanaisItem);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0120R1>(numeroMensagem, retorno);
            }
            if (lstConta != null && lstConta.Count > 0)
            {
                foreach (var objConta in lstConta)
                {

                    var objContaRet = new Servicos.ContaService(this.Organizacao, this.IsOffline).Persistir(objConta);

                    if (objContaRet == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Erro de persistência!";
                        return CriarMensagemRetorno<Pollux.MSG0120R1>(numeroMensagem, retorno);
                    }
                }

            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0120R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Conta DefinirPropriedades(Intelbras.Message.Helper.MSG0120 xml)
        {
            var crm = new Conta(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            //crm.Codigo = xml.CodigoCondicaoPagamento;

            //if (!String.IsNullOrEmpty(xml.Nome))
            //    crm.Nome = xml.Nome;

            //if (xml.NumeroParcelas.HasValue)
            //    crm.NumeroParcelas = xml.NumeroParcelas;

            //if (xml.PercentualDesconto.HasValue)
            //    crm.PercDesconto = xml.PercentualDesconto;

            //if (xml.Prazo.HasValue)
            //    crm.Prazos = xml.Prazo;

            //if(xml.Situacao.HasValue)
            //    crm.StateCode = xml.Situacao;

            //    crm.SupplierCard = xml.SupplierCard;

            //crm.Proprietario = new SDKore.DomainModel.Lookup((Guid)usuarioIntegracao.ID, "systemuser");

            //crm.IntegradoEm = DateTime.Now;
            //crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            //crm.UsuarioIntegracao = xml.LoginUsuario;

            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Conta objModel)
        {
            throw new NotImplementedException();
        }

        private List<Conta> GerarListaConta(List<Pollux.Entities.Canal> lstCanais)
        {
            List<Conta> lstContas = new List<Conta>();
            foreach (var item in lstCanais)
            {
                if (!String.IsNullOrEmpty(item.CodigoConta)
                    && item.CodigoConta.Length == 36)
                {
                    Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(item.CodigoConta));

                    if (conta != null)
                    {
                        conta.DiasAtraso = item.NumeroDiasAtraso;
                        lstContas.Add(conta);
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Canal : " + item.CodigoConta + " - não cadastrado no Crm.";
                        return null;
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Identificador do Canal não enviado/fora do padrão (Guid).";
                    return null;
                }
            }
            return lstContas;
        }

        #endregion
    }
}
