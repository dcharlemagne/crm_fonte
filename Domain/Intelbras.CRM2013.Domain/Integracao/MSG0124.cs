using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0124 : Base, IBase<Message.Helper.MSG0124, Domain.Model.RelacionamentoCanal>
    {
        #region Construtor

        public MSG0124(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0124>(mensagem));
            List<Pollux.Entities.RelacionamentoCanal> lstPolluxRelCanal = new List<Pollux.Entities.RelacionamentoCanal>();
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0124R1>(numeroMensagem, retorno);
            }


            List<RelacionamentoCanal> lstRelacionamentoCanal = new Servicos.RelacionamentoCanalService(this.Organizacao, this.IsOffline).ListarAtivosPorCanal(objeto.Canal.Id);

            if (lstRelacionamentoCanal != null && lstRelacionamentoCanal.Count > 0)
            {
                foreach (RelacionamentoCanal item in lstRelacionamentoCanal)
                {
                    Pollux.Entities.RelacionamentoCanal relCanal = new Pollux.Entities.RelacionamentoCanal();
                    if (item.Assistente != null)
                    {
                        Usuario objUsuario = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(item.Assistente.Id);
                        if (objUsuario != null)
                        {
                            relCanal.CodigoAssistente = (int)objUsuario.CodigoAssistenteComercial;
                            relCanal.NomeAssistente = item.Assistente.Name;
                            relCanal.CodigoAssistenteCRM = objUsuario.ID.Value.ToString();
                        }

                        if (item.KeyAccount != null)
                        {
                            Contato contato = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(item.KeyAccount.Id);
                            if (contato != null &&
                                contato.CodigoRepresentante != null
                                && (!String.IsNullOrEmpty(contato.CodigoRepresentante)
                                && (!contato.CodigoRepresentante.Equals("0"))
                                && contato.PrimeiroNome != null
                                && (!String.IsNullOrEmpty(contato.PrimeiroNome))))
                            {
                                relCanal.CodigoRepresentante = Convert.ToInt32(contato.CodigoRepresentante);
                                relCanal.NomeRepresentante = contato.PrimeiroNome;
                            }
                            else
                            {

                                resultadoPersistencia.Sucesso = false;
                                resultadoPersistencia.Mensagem = "Erro de inconsistência de dados no Crm. Elemento 'NomeRepresentante / CodigoRepresentante' não preenchido na base Crm para o RelacionamentoCanal - " + item.ID.Value.ToString();
                                retorno.Add("Resultado", resultadoPersistencia);
                                return CriarMensagemRetorno<Pollux.MSG0124R1>(numeroMensagem, retorno);
                            }

                        }
                        if (item.Supervisor != null)
                        {
                            Usuario usuarioSuper = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).ObterPor(item.Supervisor.Id);
                            if (usuarioSuper != null)
                            {
                                relCanal.CodigoSupervisor = item.Supervisor.Id.ToString();
                                relCanal.NomeSupervisor = usuarioSuper.Nome;

                                if (!String.IsNullOrEmpty(usuarioSuper.CodigoSupervisorEMS))
                                {
                                    relCanal.CodigoSupervisorEMS = usuarioSuper.CodigoSupervisorEMS;
                                }
                                else
                                    throw new Exception("Codigo Supervisor EMS não preenchido.");
                            }
                        }
                        if(item.DataInicial.HasValue)
                        relCanal.DataInicial = item.DataInicial.Value;
                        if (item.DataFinal.HasValue)
                            relCanal.DataFinal = item.DataFinal.Value;
                        
                        //Nome Obrigatorio
                        if (!String.IsNullOrEmpty(item.Nome))
                        {
                            relCanal.Nome = item.Nome;
                        }
                        else 
                        {
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "Erro de inconsistência de dados no Crm. Elemento 'Nome' não preenchido na base Crm para o RelacionamentoCanal - " + item.ID.Value.ToString();
                            retorno.Add("Resultado", resultadoPersistencia);
                            return CriarMensagemRetorno<Pollux.MSG0124R1>(numeroMensagem, retorno);
                        }
                        

                    }
                    relCanal.CodigoRelacionamentoCanal = item.ID.ToString();
                    lstPolluxRelCanal.Add(relCanal);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não encontrado RelacionamentoCanal para este Canal no CRM.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0124R1>(numeroMensagem, retorno);

            }

            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
            retorno.Add("RelacionamentosCanalItens", lstPolluxRelCanal);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0124R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public RelacionamentoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0124 xml)
        {
            var crm = new RelacionamentoCanal(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml


            if (!String.IsNullOrEmpty(xml.Canal))
            {
                Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.Canal));
                if (conta != null)
                    crm.Canal = new Lookup(conta.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Canal não encontrado no CRM.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador do canal não enviado.";
                return crm;
            }
            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(RelacionamentoCanal objModel)
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
