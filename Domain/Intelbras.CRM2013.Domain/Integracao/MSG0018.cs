using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0018 : Base, IBase<Pollux.MSG0018, Categoria>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado();

        #endregion


        #region Construtor
        public MSG0018(string org, bool isOffline)
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
            //Trace.Add("Mensagem " + numeroMensagem + " XML: {0}", mensagem);
            
            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "(Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0018R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Categoria DefinirPropriedades(Intelbras.Message.Helper.MSG0018 xml)
        {
            var crm = new Categoria(this.Organizacao, this.IsOffline);

            return crm;
        }

        private Intelbras.Message.Helper.MSG0018 DefinirPropriedades(Categoria crm)
        {
            Intelbras.Message.Helper.MSG0018 xml = new Pollux.MSG0018(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));
            xml.CodigoCategoria = crm.ID.ToString();
            xml.Nome = crm.Nome;

            int codCateg = 0;
            if (!String.IsNullOrEmpty(crm.CodigoCategoria) 
                && Int32.TryParse(crm.CodigoCategoria, out codCateg))
                xml.Codigo = crm.CodigoCategoria;
            else
            {
                throw new Exception("(CRM) CodigoCategoria : " + crm.CodigoCategoria + " - inválido, deve ser obrigatóriamente numérico!");
            }
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(Categoria objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0018 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0018R1 retorno = CarregarMensagem<Pollux.MSG0018R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return resposta;
        }
        #endregion
    }
}

