using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0136 : Base, IBase<Message.Helper.MSG0136, Domain.Model.SolicitacaoCadastro>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        //private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        DateTime? dtInicio = null;
        DateTime? dtFim = null;

        #endregion

        #region Construtor
        public MSG0136(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0136>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0136R1>(numeroMensagem, retorno);
            }

            List<Intelbras.Message.Helper.Entities.Solicitacao> lstPolluxSolicitacao = new List<Pollux.Entities.Solicitacao>();
            //SolicitacoesItens
            List<SolicitacaoCadastro> lstSolicitacaoCadastro = new Servicos.SolicitacaoCadastroService(this.Organizacao, this.IsOffline).ListarSolicitacaoCadastro(objeto, dtInicio, dtFim);

            #region Listas

            if (lstSolicitacaoCadastro != null)
            {

                if (lstSolicitacaoCadastro.Count > 0)
                {
                    foreach (SolicitacaoCadastro item in lstSolicitacaoCadastro)
                    {
                        Pollux.Entities.Solicitacao solicitacao = new Pollux.Entities.Solicitacao();

                        solicitacao.DescricaoSolicitacao = item.Descricao;
                        if (!String.IsNullOrEmpty(item.Nome))
                            solicitacao.NomeSolicitacao = item.Nome;
                        else
                            solicitacao.NomeSolicitacao = "N/A";

                        if (item.TipoDeSolicitacao != null)
                        {
                            solicitacao.NomeTipoSolicitacao = item.TipoDeSolicitacao.Name;
                            solicitacao.CodigoTipoSolicitacao = item.TipoDeSolicitacao.Id.ToString();
                        }


                        if (item.Necessidade.HasValue)
                            solicitacao.Necessidade = item.Necessidade.Value;


                        if (item.SupervisorVendas != null)
                        {
                            solicitacao.CodigoSupervisor = item.SupervisorVendas.Id.ToString();
                            solicitacao.NomeSupervisor = item.SupervisorVendas.Name;
                        }

                        if (item.Representante != null)
                        {
                            Contato contatoObj = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(item.Representante.Id);
                            if (contatoObj != null)
                            {
                                solicitacao.CodigoRepresentante = Convert.ToInt32(contatoObj.CodigoRepresentante);
                            }
                            solicitacao.NomeRepresentante = item.Representante.Name;
                        }

                        if (item.Canal != null)
                        {
                            solicitacao.CodigoConta = item.Canal.Id.ToString();
                            solicitacao.NomeConta = item.Canal.Name;
                        }

                        if (item.Status.HasValue)
                            solicitacao.SituacaoSolicitacao = item.Status.Value;
                        else
                            solicitacao.SituacaoSolicitacao = 0;

                        solicitacao.CodigoSolicitacao = item.ID.Value.ToString();


                        lstPolluxSolicitacao.Add(solicitacao);

                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não há solicitações para este representante.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0136R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não há solicitações para este representante.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0136R1>(numeroMensagem, retorno);
            }

            #endregion

            retorno.Add("SolicitacoesItens", lstPolluxSolicitacao);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0136R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public SolicitacaoCadastro DefinirPropriedades(Intelbras.Message.Helper.MSG0136 xml)
        {
            var crm = new Model.SolicitacaoCadastro(this.Organizacao, this.IsOffline);

            Contato contato = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.ToString());
            if (contato != null)
                crm.Representante = new Lookup(contato.ID.Value, "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Contato Representante não encontrado no CRM.";
                return crm;
            }
            //Setar horario para pegar o dia todo na pesquisa
            TimeSpan tsDtIni = new TimeSpan(00, 00, 01);
            TimeSpan tsDtFim = new TimeSpan(23, 59, 59);
            dtInicio = (xml.DataInicio + tsDtIni);
            dtFim = (xml.DataFinal + tsDtFim);

            crm.Status = xml.SituacaoSolicitacao;

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(SolicitacaoCadastro objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
