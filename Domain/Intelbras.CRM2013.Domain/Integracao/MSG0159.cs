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
    public class MSG0159 : Base, IBase<Message.Helper.MSG0159, Domain.Model.BeneficioDoCanal>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private List<string> listaErros = new List<string>();

        #endregion

        #region Construtor
        public MSG0159(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0159>(mensagem);


            try
            {

                foreach (var itemBeneficioCanal in xml.BeneficioCanalItens)
                {
                    try
                    {
                        if (String.IsNullOrEmpty(itemBeneficioCanal.CodigoBeneficioCanal))
                        {
                            listaErros.Add(itemBeneficioCanal.CodigoBeneficioCanal + " - Parâmetro obrigatório[CódigoBenefícioCanal] para a consulta não enviado");
                            continue;
                        }

                Guid benefCanalGuid;
                        if (!Guid.TryParse(itemBeneficioCanal.CodigoBeneficioCanal, out benefCanalGuid))
                {
                            listaErros.Add(itemBeneficioCanal.CodigoBeneficioCanal + " - Guid Benefício Canal em formato inválido.");
                            continue;
                }

                //Pegamos o benefCanal com base no Guid
                BeneficioDoCanal benefCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).ObterPor(benefCanalGuid);

                if (benefCanal == null)
                {
                            listaErros.Add(itemBeneficioCanal.CodigoBeneficioCanal + " - Valor do parâmetro [BeneficioCanal] não existe. Não encontrado na base CRM.");
                            continue;
                }

                        benefCanal.VerbaPeriodoAtual = decimal.Round(itemBeneficioCanal.VerbaCalculada.Value, 4);
                        benefCanal.VerbaPeriodosAnteriores = decimal.Round(itemBeneficioCanal.VerbaPeriodoAnterior.Value, 4);
                        benefCanal.VerbaBrutaPeriodoAtual = decimal.Round(itemBeneficioCanal.VerbaTotal.Value, 4);
                        benefCanal.TotalSolicitacoesAprovadasNaoPagas = decimal.Round(itemBeneficioCanal.VerbaEmpenhada.Value, 4);
                        benefCanal.VerbaReembolsada = decimal.Round(itemBeneficioCanal.VerbaReembolsada.Value, 4);
                        benefCanal.VerbaCancelada = decimal.Round(itemBeneficioCanal.VerbaCancelada.Value, 4);
                        benefCanal.VerbaAjustada = decimal.Round(itemBeneficioCanal.VerbaAjustada.Value, 4);
                        benefCanal.VerbaDisponivel = decimal.Round(itemBeneficioCanal.VerbaDisponivel.Value, 4);

                new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.Organizacao, this.IsOffline).AlterarBeneficioCanal(benefCanal);

                    }
                    catch (Exception e)
                    {
                        listaErros.Add(String.IsNullOrEmpty(itemBeneficioCanal.CodigoBeneficioCanal) ?
                            itemBeneficioCanal.CodigoBeneficioCanal + " - " : "Erro sem Guid informado - " + SDKore.Helper.Error.Handler(e));
                    }
                }

                if (listaErros.Count() == 0)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Erros apresentados nesta requisição: ");

                    foreach (var mensagemErro in listaErros)
                    {
                        sb.AppendLine(mensagemErro + ";");
                    }
                    resultadoPersistencia.Mensagem = sb.ToString();
                }

                retorno.Add("Resultado", resultadoPersistencia);

                return CriarMensagemRetorno<Pollux.MSG0159R1>(numeroMensagem, retorno);
            }
            catch (Exception e)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(e);
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0159R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public BeneficioDoCanal DefinirPropriedades(Intelbras.Message.Helper.MSG0159 xml)
        {
            var crm = new Model.BeneficioDoCanal(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(BeneficioDoCanal objModel)
        {
            return String.Empty;
        }
        #endregion

    }
}
