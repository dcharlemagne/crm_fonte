using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0137 : Base, IBase<Message.Helper.MSG0137, Domain.Model.RelacionamentoCanal>
    {

        #region Construtor

        public MSG0137(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


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
            //resultadoPersistencia.Sucesso = true;
            //resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";


            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0137>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0137R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.RelacionamentoCanalService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("CodigoRelacionamentoCanal", objeto.ID.Value.ToString());
                retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());
                retorno.Add("TipoProprietario", "systemuser");
                retorno.Add("Resultado", resultadoPersistencia);
            }

            return CriarMensagemRetorno<Pollux.MSG0137R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public RelacionamentoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0137 xml)
        {
            var crm = new RelacionamentoCanal(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            crm.IntegrarNoPlugin = true;

            if (!String.IsNullOrEmpty(xml.CodigoRelacionamentoCanal))
                crm.ID = new Guid(xml.CodigoRelacionamentoCanal);

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.CodigoConta) && xml.CodigoConta.Length == 36)
                crm.Canal = new Lookup(new Guid(xml.CodigoConta), "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoConta não enviado ou fora do padrão (Guid).";
                return crm;
            }
            Contato keyAccount = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.ToString());
            if (keyAccount != null)
                crm.KeyAccount = new Lookup(keyAccount.ID.Value, "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "keyAccount não encontrado no Crm.";
                return crm;
            }

            Usuario assistente = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscaPorCodigoAssistente(xml.CodigoAssistente);
            if (assistente != null)
            {
                crm.Assistente = new Lookup(assistente.ID.Value, "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Assistente não encontrado no Crm.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoSupervisor) && xml.CodigoSupervisor.Length == 36)
                crm.Supervisor = new Lookup(new Guid(xml.CodigoSupervisor), "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Supervisor não encontrado no Crm.";
                return crm;
            }
            if (crm.DataInicial.HasValue)
                xml.DataInicial = crm.DataInicial.Value.ToLocalTime();

            if (crm.DataFinal.HasValue)
                xml.DataFinal = crm.DataFinal.Value.ToLocalTime();

            crm.Status = xml.Situacao;

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0137 DefinirPropriedades(RelacionamentoCanal crm)
        {
            Intelbras.Message.Helper.MSG0137 xml = new Pollux.MSG0137(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            Contato contato = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(crm.KeyAccount.Id);
            if (contato != null)
            {
                xml.CodigoRepresentante = Convert.ToInt32(contato.CodigoRepresentante);

            }

            Usuario objUsuarioAssistente = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(crm.Assistente.Id);
            if (objUsuarioAssistente != null)
            {
                if (objUsuarioAssistente.CodigoAssistenteComercial != null)
                {
                    xml.CodigoAssistente = (int)objUsuarioAssistente.CodigoAssistenteComercial.Value;
                    xml.CodigoAssistenteCRM = objUsuarioAssistente.ID.Value.ToString();
                }
            }

            Usuario objUsuarioSuper = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(crm.Supervisor.Id);
            if (objUsuarioSuper != null)
            {
                xml.CodigoSupervisor = objUsuarioSuper.ID.Value.ToString();
                if (!String.IsNullOrEmpty(objUsuarioSuper.CodigoSupervisorEMS))
                {
                    xml.CodigoSupervisorEMS = objUsuarioSuper.CodigoSupervisorEMS;
                }
                else
                    throw new Exception("Codigo Supervisor EMS não preenchido.");
            }
            if (crm.Canal != null)
                xml.CodigoConta = crm.Canal.Id.ToString();
            xml.CodigoRelacionamentoCanal = crm.ID.Value.ToString();
            if (crm.Supervisor != null)
                xml.CodigoSupervisor = crm.Supervisor.Id.ToString();
            xml.DataFinal = crm.DataFinal.Value;
            xml.DataInicial = crm.DataInicial.Value;
            xml.Nome = crm.Nome;
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(RelacionamentoCanal objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0137 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0137R1 retorno = CarregarMensagem<Pollux.MSG0137R1>(resposta);
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
