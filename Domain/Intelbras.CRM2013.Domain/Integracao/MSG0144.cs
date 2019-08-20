using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0144 : Base, IBase<Message.Helper.MSG0144, Domain.Model.CompromissosDoCanal>
    {

        #region Construtor

        public MSG0144(string org, bool isOffline)
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
            Conta contaConsulta = null;
            List<Pollux.Entities.CompromissoCanal> lstRetorno = new List<Pollux.Entities.CompromissoCanal>();

            var xml = this.CarregarMensagem<Pollux.MSG0144>(mensagem);
            //Conta
            if (!String.IsNullOrEmpty(xml.CodigoConta) && xml.CodigoConta.Length == 36)
            {
                contaConsulta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (contaConsulta == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Valor do parâmetro " + xml.CodigoConta + " não existe.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0144R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Parâmetro obrigatório para a consulta não enviado.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0144R1>(numeroMensagem, retorno);
            }
            //Compromissos Canal
            List<CompromissosDoCanal> lstCompromissosCanalCrm = new Servicos.CompromissosDoCanalService(this.Organizacao, this.IsOffline).ListarCompromissoCanalPorConta(contaConsulta.ID.Value);

            if (lstCompromissosCanalCrm != null && lstCompromissosCanalCrm.Count > 0)
            {
                lstRetorno = this.DefinirRetorno(lstCompromissosCanalCrm);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0144R1>(numeroMensagem, retorno);
            }
            if (lstRetorno != null && lstRetorno.Count > 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("CompromissoCanalItens", lstRetorno);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0144R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public CompromissosDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0144 xml)
        {
            CompromissosDoCanal retorno = new CompromissosDoCanal(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.CompromissoCanal> DefinirRetorno(List<Model.CompromissosDoCanal> lstCompCanalCrm)
        {
           
            List<CompromissosDoCanal> lstCompromissosDoCanalCrm = new List<CompromissosDoCanal>();
            List<Pollux.Entities.CompromissoCanal> lstRetorno = new List<Pollux.Entities.CompromissoCanal>();
            #region Propriedades Crm->Xml
            foreach (var itemCrm in lstCompCanalCrm)
            {
                Pollux.Entities.CompromissoCanal compCanalPollux = new Pollux.Entities.CompromissoCanal();
                if (!String.IsNullOrEmpty(itemCrm.Nome))
                    compCanalPollux.NomeCompromissoCanal = itemCrm.Nome;
                else
                    compCanalPollux.NomeCompromissoCanal = "N/A";
                compCanalPollux.CodigoCompromissoCanal = itemCrm.ID.Value.ToString();
                if (itemCrm.UnidadeDeNegocio != null)
                {
                    UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(itemCrm.UnidadeDeNegocio.Id);
                    if (unidadeNeg != null)
                    {
                        if (!String.IsNullOrEmpty(unidadeNeg.ChaveIntegracao))
                            compCanalPollux.CodigoUnidadeNegocio = unidadeNeg.ChaveIntegracao;
                        else 
                            compCanalPollux.CodigoUnidadeNegocio = Guid.Empty.ToString();
                        if (!String.IsNullOrEmpty(unidadeNeg.Nome))
                            compCanalPollux.NomeUnidadeNegocio = unidadeNeg.Nome;
                        else compCanalPollux.NomeUnidadeNegocio = "N/A";
                    }

                    else
                    {
                        compCanalPollux.CodigoUnidadeNegocio = Guid.Empty.ToString();
                        compCanalPollux.NomeUnidadeNegocio = "N/A";
                    }
                }
                else
                {
                    compCanalPollux.CodigoUnidadeNegocio = Guid.Empty.ToString();
                    compCanalPollux.NomeUnidadeNegocio = "N/A";
                }
                if (itemCrm.StatusCompromisso != null)
                {
                    compCanalPollux.CodigoStatusCompromisso = itemCrm.StatusCompromisso.Id.ToString();
                    compCanalPollux.NomeStatusCompromisso = itemCrm.StatusCompromisso.Name;
                }
                else
                {
                    compCanalPollux.CodigoStatusCompromisso = Guid.Empty.ToString();
                    compCanalPollux.NomeStatusCompromisso = "N/A";
                }
                if (itemCrm.CumprirCompromissoEm.HasValue)
                    compCanalPollux.DataCumprimento = itemCrm.CumprirCompromissoEm.Value;

                if (itemCrm.Validade.HasValue)
                    compCanalPollux.DataValidade = itemCrm.Validade.Value;
                lstRetorno.Add(compCanalPollux);
            }
            #endregion
            return lstRetorno;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(CompromissosDoCanal objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
