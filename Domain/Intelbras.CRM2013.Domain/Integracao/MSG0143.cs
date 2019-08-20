using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0143 : Base, IBase<Intelbras.Message.Helper.MSG0143, Domain.Model.PortfoliodoKeyAccountRepresentantes>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0143(string org, bool isOffline)
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
            resultadoPersistencia.Mensagem = "Intergação não permitida!";
            resultadoPersistencia.Sucesso = false;
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0143R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public PortfoliodoKeyAccountRepresentantes DefinirPropriedades(Intelbras.Message.Helper.MSG0143 xml)
        {
            var crm = new PortfoliodoKeyAccountRepresentantes(this.Organizacao, this.IsOffline);
            return crm;
        }

        public Pollux.MSG0143 DefinirPropriedades(PortfoliodoKeyAccountRepresentantes objModel)
        {
            #region Propriedades Crm->Xml

            Pollux.MSG0143 msg0143 = new Pollux.MSG0143(itb.RetornaSistema(itb.Sistema.CRM), Helper.Truncate(objModel.Nome, 40));
            string nomeUnid = String.Empty;
            string nomeSegmento = String.Empty;

            msg0143.CodigoPortfolioRepresentante = objModel.ID.Value.ToString();
            if (objModel.KeyAccountRepresentante != null)
            {
                Contato representante = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(objModel.KeyAccountRepresentante.Id);
                if (representante != null && representante.CodigoRepresentante.Length > 0)
                    msg0143.CodigoRepresentante = Convert.ToInt32(representante.CodigoRepresentante);
                else
                {
                    throw new Exception("Representante não localizado/sem código representante.");
                }
            }
            if (objModel.UnidadedeNegocio != null)
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(objModel.UnidadedeNegocio.Id);
                if (unidadeNegocio != null)
                {
                    msg0143.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                    nomeUnid = unidadeNegocio.Nome;
                }
                else
                {
                    throw new Exception("Unidade de negocio não localizada ou sem nome.");
                }
            }
            if (objModel.AssistentedeAdministracaodeVendas != null)
            {
                Usuario usuario = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(objModel.AssistentedeAdministracaodeVendas.Id);
                if (usuario != null && usuario.CodigoAssistenteComercial != null)
                {
                    msg0143.CodigoAssistente = usuario.CodigoAssistenteComercial.Value;
                    msg0143.CodigoAssistenteCRM = usuario.ID.Value.ToString();
                }
                
                else
                    throw new Exception("Usuario sem código de assistente comercial.");
            }
            if (objModel.Segmento != null)
            {
                Segmento segmento = new Servicos.SegmentoService(this.Organizacao, this.IsOffline).ObterPor(objModel.Segmento.Id);
                if (segmento != null)
                {
                    msg0143.CodigoSegmento = segmento.CodigoSegmento;
                    nomeSegmento = segmento.Nome;
                }
                else
                    throw new Exception("Segmento não localizado.");
            }
            else
            {
                msg0143.CodigoSegmento = "0000";
                nomeSegmento = "";
            }

            if (objModel.SupervisordeVendas != null)
            {
                Usuario supervisor = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(objModel.SupervisordeVendas.Id);
                if (supervisor != null)
                {
                    msg0143.CodigoSupervisor = supervisor.ID.Value.ToString();
                    if (!String.IsNullOrEmpty(supervisor.CodigoSupervisorEMS))
                    {
                        msg0143.CodigoSupervisorEMS = supervisor.CodigoSupervisorEMS;
                    }
                    else
                        throw new Exception("Codigo Supervisor EMS não preenchido.");
                }
                else
                    throw new Exception("Supervisor não localizado.");
            }

            if (objModel.Status.HasValue)
            {
                if (objModel.Status.Value == 1)
                {
                    msg0143.Situacao = 0;
                }
                else
                {
                    msg0143.Situacao = 1;
                }
            }

            if (!String.IsNullOrEmpty(objModel.Nome))
            {
                string nometmp = objModel.Nome + " - " + nomeUnid + " - " + nomeSegmento;
                if (nometmp.Length > 99)
                    msg0143.Nome = nometmp.Substring(0, 99);
                else
                    msg0143.Nome = nometmp;
            }
            else
                throw new Exception("PortfolioRepresentante sem nome.");

            #endregion

            return msg0143;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(PortfoliodoKeyAccountRepresentantes objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0143 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0143R1 retorno = CarregarMensagem<Pollux.MSG0143R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new Exception(retorno.Resultado.Mensagem);
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new Exception(erro001.GenerateMessage(false));
            }
            return retMsg;
        }

        #endregion
    }
}
