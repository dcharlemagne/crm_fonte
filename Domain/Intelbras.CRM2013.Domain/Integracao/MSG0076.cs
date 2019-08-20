using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0076 : Base, IBase<Pollux.MSG0076, Model.RegiaoAtuacao>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        #endregion

        #region Construtor

        public MSG0076(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0076>(mensagem);

            var objeto = this.DefinirPropriedades(xml);

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0076R1>(numeroMensagem, retorno);
            }
            if (new Servicos.RegiaoAtuacaoServices(this.Organizacao, this.IsOffline).Acao(objeto, xml.Acao))
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de persistência.";
            }
                
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0076R1>(numeroMensagem, retorno);

        }
        #endregion

        #region Definir Propriedades
        public RegiaoAtuacao DefinirPropriedades(Intelbras.Message.Helper.MSG0076 xml)
        {
            var crm = new RegiaoAtuacao(this.Organizacao, this.IsOffline);

            if (!String.IsNullOrEmpty(xml.CodigoConta))
            {
                Model.Conta conta = new Model.Conta(this.Organizacao, this.IsOffline);
                conta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (conta != null)
                    crm.Canal = conta.ID.Value;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Canal não encontrado!";
                return crm;
            }

            //Municipio
            if (!String.IsNullOrEmpty(xml.ChaveIntegracaoCidade))
            {
                Model.Municipio cidade = new Model.Municipio(this.Organizacao, this.IsOffline);
                cidade = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(xml.ChaveIntegracaoCidade);

                if (cidade != null && cidade.ID.HasValue)
                    crm.MunicipioId = cidade.ID.Value;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Cidade não encontrada!";
                    return crm;
                }
            }

            return crm;

        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(RegiaoAtuacao objModel)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
