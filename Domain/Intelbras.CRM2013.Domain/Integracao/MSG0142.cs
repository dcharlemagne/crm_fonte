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
    public class MSG0142 : Base, IBase<Message.Helper.MSG0142, Domain.Model.ParametroBeneficio>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0142(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0142>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0142R1>(numeroMensagem, retorno);
            }

            List<Intelbras.Message.Helper.Entities.ParametroBeneficio> lstPolluxParametroBeneficio = new List<Pollux.Entities.ParametroBeneficio>();
            //SolicitacoesItens
            List<ParametroBeneficio> lstParametroBeneficio = new Servicos.ParametroBeneficioService(this.Organizacao, this.IsOffline).ListarPorBeneficio(objeto.Beneficio.Id);

            #region Lista

            if (lstParametroBeneficio != null && lstParametroBeneficio.Count > 0)
            {

                foreach (ParametroBeneficio crmParametroBeneficio in lstParametroBeneficio)
                {
                    Pollux.Entities.ParametroBeneficio parametroBeneficioPollux = new Pollux.Entities.ParametroBeneficio();


                    if (crmParametroBeneficio.UnidadeNegocioObj != null)
                        parametroBeneficioPollux.CodigoUnidadeNegocio = crmParametroBeneficio.UnidadeNegocioObj.ChaveIntegracao;
                    else
                        parametroBeneficioPollux.CodigoUnidadeNegocio = Guid.Empty.ToString();


                    if (crmParametroBeneficio.Estabelecimento != null)
                    {
                        Estabelecimento estabelecimento = new Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimento(crmParametroBeneficio.Estabelecimento.Id);
                        if (estabelecimento != null && estabelecimento.Codigo.HasValue)
                            parametroBeneficioPollux.CodigoEstabelecimento = estabelecimento.Codigo.Value;
                        else
                            parametroBeneficioPollux.CodigoEstabelecimento = 0;
                    }

                    if (!String.IsNullOrEmpty(crmParametroBeneficio.TipoFluxoFinanceiro))
                        parametroBeneficioPollux.TipoFluxoFinanceiro = Helper.Truncate(crmParametroBeneficio.TipoFluxoFinanceiro,12);
                    else
                        parametroBeneficioPollux.TipoFluxoFinanceiro = "N/A";

                    if (!String.IsNullOrEmpty(crmParametroBeneficio.EspecieDocumento))
                        parametroBeneficioPollux.EspecieDocumento = Helper.Truncate(crmParametroBeneficio.EspecieDocumento, 3);
                    else
                        parametroBeneficioPollux.EspecieDocumento = "N/A";

                    if (!String.IsNullOrEmpty(crmParametroBeneficio.ContaContabil))
                        parametroBeneficioPollux.ContaContabil = Helper.Truncate(crmParametroBeneficio.ContaContabil, 20);
                    else
                        parametroBeneficioPollux.ContaContabil = "N/A";

                    if (!String.IsNullOrEmpty(crmParametroBeneficio.CentroCusto))
                        parametroBeneficioPollux.CentroCusto = Helper.Truncate(crmParametroBeneficio.CentroCusto, 11);
                    else
                        parametroBeneficioPollux.CentroCusto = "N/A";

                    if (crmParametroBeneficio.AtingimentoMetaPrevisto.HasValue)
                        parametroBeneficioPollux.PercentualAtingimentoMeta = crmParametroBeneficio.AtingimentoMetaPrevisto.Value;
                    else
                        parametroBeneficioPollux.PercentualAtingimentoMeta = 0;

                    if (crmParametroBeneficio.Custo.HasValue)
                        parametroBeneficioPollux.PercentualCusto = crmParametroBeneficio.Custo.Value;
                    else
                        parametroBeneficioPollux.PercentualCusto = 0;

                    lstPolluxParametroBeneficio.Add(parametroBeneficioPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0142R1>(numeroMensagem, retorno);
            }

            #endregion

            retorno.Add("ParametroBeneficioItens", lstPolluxParametroBeneficio);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0142R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ParametroBeneficio DefinirPropriedades(Intelbras.Message.Helper.MSG0142 xml)
        {
            var crm = new Model.ParametroBeneficio(this.Organizacao, this.IsOffline);

            if (!String.IsNullOrEmpty(xml.CodigoBeneficio) && xml.CodigoBeneficio.Length == 36)
            {
                Beneficio beneficio = new Servicos.RepositoryService(this.Organizacao, this.IsOffline).Beneficio.Retrieve(new Guid(xml.CodigoBeneficio));
                if (beneficio != null)
                {
                    crm.Beneficio = new Lookup(beneficio.ID.Value, "");
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Codigo Beneficio não enviado ou fora do padrão(Guid).";
                return crm;
            }
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ParametroBeneficio objModel)
        {
            return String.Empty;
        }
        #endregion
    }
}
