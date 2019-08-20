using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0145 : Base, IBase<Message.Helper.MSG0145, Domain.Model.CompromissosDoCanal>
    {

        #region Construtor

        public MSG0145(string org, bool isOffline)
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
            CompromissosDoCanal compCanalConsulta = null;
            Pollux.Entities.Compromisso  objRetorno = new Pollux.Entities.Compromisso();

            var xml = this.CarregarMensagem<Pollux.MSG0145>(mensagem);
            //Conta
            if (!String.IsNullOrEmpty(xml.CodigoCompromissoCanal) && xml.CodigoCompromissoCanal.Length == 36)
            {
                compCanalConsulta = new Servicos.CompromissosDoCanalService(this.Organizacao, this.IsOffline).BuscarPorGuid(new Guid(xml.CodigoCompromissoCanal));
                if (compCanalConsulta == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Valor do parâmetro " + xml.CodigoCompromissoCanal + " não existe.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0145R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Parâmetro obrigatório para a consulta não enviado.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0145R1>(numeroMensagem, retorno);
            }
            
            objRetorno = this.DefinirRetorno(compCanalConsulta);

            if (objRetorno != null)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("CompromissoCanal", objRetorno);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0145R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public CompromissosDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0145 xml)
        {
            CompromissosDoCanal compCanalPollux = new CompromissosDoCanal(this.Organizacao, this.IsOffline);
            return compCanalPollux;
        }

        public Pollux.Entities.Compromisso DefinirRetorno(Model.CompromissosDoCanal compCanalCrm)
        {
            #region Propriedades Crm->Xml

            Pollux.Entities.Compromisso compCanalPollux = new Pollux.Entities.Compromisso();
            if (!String.IsNullOrEmpty(compCanalCrm.Nome))
                compCanalPollux.NomeCompromissoCanal = compCanalCrm.Nome;
            else
                compCanalPollux.NomeCompromissoCanal = "N/A";
            if (compCanalCrm.UnidadeDeNegocio != null)
            {
                UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(compCanalCrm.UnidadeDeNegocio.Id);
                if (unidadeNeg != null)
                {
                    if (!String.IsNullOrEmpty(unidadeNeg.ChaveIntegracao))
                        compCanalPollux.CodigoUnidadeNegocio = unidadeNeg.ChaveIntegracao;
                    else 
                        compCanalPollux.CodigoUnidadeNegocio = "N/A";
                    if (!String.IsNullOrEmpty(unidadeNeg.Nome))
                        compCanalPollux.NomeUnidadeNegocio = unidadeNeg.Nome;
                    else 
                        compCanalPollux.NomeUnidadeNegocio = "N/A";
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
            if (compCanalCrm.StatusCompromisso != null)
            {
                compCanalPollux.CodigoStatusCompromisso = compCanalCrm.StatusCompromisso.Id.ToString();
                compCanalPollux.NomeStatusCompromisso = compCanalCrm.StatusCompromisso.Name;
            }
            else
            {
                compCanalPollux.CodigoStatusCompromisso = Guid.Empty.ToString();
                compCanalPollux.NomeStatusCompromisso = "N/A";
            }
            if (compCanalCrm.CumprirCompromissoEm.HasValue)
                compCanalPollux.DataCumprimento = compCanalCrm.CumprirCompromissoEm.Value;
            if (compCanalCrm.Validade.HasValue)
                compCanalPollux.DataValidade = compCanalCrm.Validade.Value;


            #endregion
            return compCanalPollux;
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
