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
    public class MSG0008 : Base, IBase<Message.Helper.MSG0008, Domain.Model.Itbc_regiaogeo>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0008(string org, bool isOffline)
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
            resultadoPersistencia.Mensagem = "(Integração não permitida!";

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0008R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public Itbc_regiaogeo DefinirPropriedades(Intelbras.Message.Helper.MSG0008 xml)
        {
            var crm = new Itbc_regiaogeo(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoRegiaoGeografica))
                crm.ID = new Guid(xml.CodigoRegiaoGeografica);
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(CodigoRegiaoGeografica não enviado.";
                return crm;
            }


            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(Nome não enviado.";
                return crm;
            }

            crm.Status = xml.Situacao;
           

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0008 DefinirPropriedades(Itbc_regiaogeo crm)
        {
            Intelbras.Message.Helper.MSG0008 xml = new Pollux.MSG0008(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.CodigoRegiaoGeografica = crm.ID.ToString();
            xml.Nome = crm.Nome;
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
        }

        #endregion

        #region Métodos Auxiliares

        public string EnviarRegiaoGeo(Itbc_regiaogeo objModel)
        {
            
            string resposta;
            Intelbras.Message.Helper.MSG0008 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0008R1 retorno = CarregarMensagem<Pollux.MSG0008R1>(resposta);
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


        public string Enviar(Itbc_regiaogeo objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0008 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0008R1 retorno = CarregarMensagem<Pollux.MSG0008R1>(resposta);
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
