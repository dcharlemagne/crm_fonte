using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.ViewModels;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0001 : Base, IBase<Intelbras.Message.Helper.MSG0001, Model.EnderecoCEP>
    {
        #region propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtores
        public MSG0001(string organization, bool isOffline)
            : base(organization, isOffline)
        {
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
            return string.Empty;
            ////Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            //var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0001>(mensagem));

            //if (!resultadoPersistencia.Sucesso)
            //{
            //    retorno.Add("Resultado", resultadoPersistencia);
            //    return CriarMensagemRetorno<Pollux.MSG0001R1>(numeroMensagem, retorno);
            //}
            ////objeto.ID = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).Persistir(objeto, objeto.ChaveIntegracao);
            ////if (!objeto.ID.HasValue)
            ////    throw new Exception("Erro de persistência");

            //resultadoPersistencia.Sucesso = true;
            //resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            //retorno.Add("Resultado", resultadoPersistencia);

            //return CriarMensagemRetorno<Pollux.MSG0001R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public CepViewModel DefinirPropriedades(Pollux.MSG0001R1 xml)
        {
            var cep = new CepViewModel();

            cep.Bairro = xml.EnderecoCEP.Bairro;
            cep.CEP = xml.EnderecoCEP.CEP;
            cep.Cidade = xml.EnderecoCEP.Cidade;
            cep.CidadeZonaFranca = xml.EnderecoCEP.CidadeZonaFranca;
            cep.CodigoIBGE = xml.EnderecoCEP.CodigoIBGE;
            cep.Endereco = xml.EnderecoCEP.Endereco;
            //cep.Estado = xml.EnderecoCEP.Estado;
            cep.NomeCidade = xml.EnderecoCEP.NomeCidade;
            //cep.Pais = xml.EnderecoCEP.Pais;
            cep.UF = xml.EnderecoCEP.UF;

            return cep;
        }

        public Intelbras.Message.Helper.MSG0001 DefinirPropriedades(string cep)
        {
            if (string.IsNullOrEmpty(cep))
            {
                throw new ArgumentException("O CEP é obrigatório para o envio da mensagem!");
            }

            string identidadeEmissor = Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM);
            var xml = new Pollux.MSG0001(identidadeEmissor, cep);

            xml.CEP = cep;

            return xml;
        }

        public EnderecoCEP DefinirPropriedades(Pollux.MSG0001 legado)
        {
            throw new NotImplementedException();
        }
        #endregion

        public CepViewModel Enviar(string cep)
        {
            var objetoBarramento = this.DefinirPropriedades(cep);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            string message = string.Empty;
            string retMsg = String.Empty;

            try
            {
                message = objetoBarramento.GenerateMessage(true);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(XSD) " + ex.Message, ex);
            }

            if (integracao.EnviarMensagemBarramento(message, "1", "1", out retMsg))
            {
                var retorno = CarregarMensagem<Pollux.MSG0001R1>(retMsg);
                if (retorno.Resultado.Sucesso)
                {
                    return DefinirPropriedades(retorno);
                }
                else
                {
                    //Execeção retirada para que ocorra a criação da Revenda
                    //throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);

                    SDKore.Helper.Error.Create(retorno.Resultado.Mensagem, System.Diagnostics.EventLogEntryType.Error);

                    return null;
                }
            }
            else
            {
                var erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                //Execeção retirada para que ocorra a criação da Revenda
                //throw new ArgumentException(string.Concat(erro001.GenerateMessage(false)));

                SDKore.Helper.Error.Create(string.Concat(erro001.GenerateMessage(false)), System.Diagnostics.EventLogEntryType.Error);

                return null;
            }
        }

        public string Enviar(EnderecoCEP objModel)
        {
            throw new NotImplementedException();
        }

    }
}
