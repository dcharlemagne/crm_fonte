using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0022 : Base, IBase<Intelbras.Message.Helper.MSG0022, Transportadora>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0022(string org, bool isOffline)
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

            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0022>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0022R1>(numeroMensagem, retorno);
            }

            //Checa dentro da service se ele tentou mudar o proprietario,se positivo recusa e retorna erro
            bool mudancaProprietario = false;

            objeto = new Intelbras.CRM2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).Persistir(objeto, ref mudancaProprietario);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";
            }
            else
            {
                if (mudancaProprietario == true)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso, não houve alteração do proprietário.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                }
            }

            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0022R1>(numeroMensagem, retorno);
        }
        #endregion

        public Transportadora DefinirPropriedades(Intelbras.Message.Helper.MSG0022 xml)
        {
            if (!xml.CodigoViaTransporte.HasValue)
            {
                throw new ArgumentException("(CRM) O campo CodigoViaTransporte é obrigatório!");
            }

            var crm = new Transportadora(this.Organizacao, this.IsOffline);
            
            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Nome não encontrado!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.NomeAbreviado))
                crm.NomeAbreviado = xml.NomeAbreviado;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do Nome Abreviado não encontrado!";
                return crm;
            }

            crm.Codigo = xml.CodigoTransportadora;
            crm.Status = xml.Situacao;
            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            crm.CodigoViaTransportadora = (int)ConvertCodigoViaTransportadora(xml.CodigoViaTransporte.Value);

            return crm;
        }

        public string Enviar(Transportadora objModel)
        {
            throw new NotImplementedException();
        }

        private Domain.Enum.Transportadora.CodigoViaTransportadora ConvertCodigoViaTransportadora(int codigo)
        {
            switch (codigo)
            {
                case 1: return Enum.Transportadora.CodigoViaTransportadora.Rodoviario;
                case 2: return Enum.Transportadora.CodigoViaTransportadora.Aeroviário;
                case 3: return Enum.Transportadora.CodigoViaTransportadora.Marítimo;
                case 4: return Enum.Transportadora.CodigoViaTransportadora.Ferroviário;
                case 5: return Enum.Transportadora.CodigoViaTransportadora.Rodoferroviário;
                case 6: return Enum.Transportadora.CodigoViaTransportadora.Rodofluvial;
                case 7: return Enum.Transportadora.CodigoViaTransportadora.Rodoaeroviário;
                case 8: return Enum.Transportadora.CodigoViaTransportadora.Outros;
            }

            throw new ArgumentException("(CRM) Codigo Via Transportadora inválido!");
        }
    }
}