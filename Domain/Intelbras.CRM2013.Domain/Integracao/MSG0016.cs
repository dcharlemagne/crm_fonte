using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0016 : Base, IBase<Pollux.MSG0016, Model.Subclassificacoes>
    {
        #region Propriedades
            //Dictionary que sera enviado como resposta do request
            private Dictionary<string, object> retorno = new Dictionary<string, object>();
            private Domain.Model.Usuario usuarioIntegracao;

            Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
            public MSG0016(string org, bool isOffline)
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

                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Integração não permitida!";

                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0016R1>(numeroMensagem, retorno);
            }
        #endregion

        #region Definir Propriedades
            public Subclassificacoes DefinirPropriedades(Intelbras.Message.Helper.MSG0016 xml)
            {
                throw new NotImplementedException();
            }

            private Intelbras.Message.Helper.MSG0016 DefinirPropriedades(Subclassificacoes crm)
            {
                Intelbras.Message.Helper.MSG0016 xml = new Pollux.MSG0016(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

                xml.CodigoSubClassificacao = crm.ID.ToString();
                xml.Classificacao = crm.Classificacao.Id.ToString();
                xml.Nome = crm.Nome;
                xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
            }
        #endregion

        #region Métodos Auxiliares
            public string Enviar(Subclassificacoes objModel)
            {
                string resposta;
                Intelbras.Message.Helper.MSG0016 mensagem = DefinirPropriedades(objModel);

                Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
                if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
                {
                    Intelbras.Message.Helper.MSG0016R1 retorno = CarregarMensagem<Pollux.MSG0016R1>(resposta);
                    return retorno.Resultado.Mensagem;
                }
                else
                {
                    Intelbras.Message.Helper.ERR0001 retorno = CarregarMensagem<Pollux.ERR0001>(resposta);
                    return retorno.DescricaoErro;
                }

            }
        #endregion        

    }
}
