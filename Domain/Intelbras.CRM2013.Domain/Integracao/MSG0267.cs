using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;


namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0267 : Base, IBase<Message.Helper.MSG0267, Domain.Model.ClientePotencial>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0267(string org, bool isOffline) : base(org, isOffline)
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
          var xml = this.CarregarMensagem<Pollux.MSG0267>(mensagem);

            List<Intelbras.Message.Helper.Entities.ProjetoItem> lstProjetoItem = new List<Pollux.Entities.ProjetoItem>();

            if (string.IsNullOrEmpty(xml.CodigoRevenda) && string.IsNullOrEmpty(xml.CodigoDistribuidor) && string.IsNullOrEmpty(xml.CodigoExecutivo) && string.IsNullOrEmpty(xml.CNPJCliente) && (!xml.SituacaoProjeto.HasValue))
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "É necessário informar ao menos 1 critério de busca para a consulta.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0267R1>(numeroMensagem, retorno);
            }

            string cnpjCliente = "";
            if (!String.IsNullOrEmpty(xml.CNPJCliente))
               cnpjCliente = Servicos.Helper.FormatarCnpj(xml.CNPJCliente);
            else
               cnpjCliente = "";

            List<ClientePotencial> lstProjetos = new Servicos.LeadService(this.Organizacao, this.IsOffline).ListarProjetosPor(xml.CodigoRevenda, xml.CodigoDistribuidor, xml.CodigoExecutivo, cnpjCliente, xml.SituacaoProjeto, xml.CodigoSegmento, xml.CodigoUnidadeNegocio);

            #region Lista

            if (lstProjetos != null && lstProjetos.Count > 0)
            {
                foreach (ClientePotencial crmItem in lstProjetos)
                {
                    Pollux.Entities.ProjetoItem objPollux = new Pollux.Entities.ProjetoItem();

                    Oportunidade Oportunidade = new Servicos.RepositoryService().Oportunidade.BuscarPor(crmItem);

                    objPollux.NumeroProjeto = crmItem.NumeroProjeto;

                    if (Oportunidade != null)
                    {
                        objPollux.ClassificacaoProjeto = 993520001;
                    } else
                    {
                        objPollux.ClassificacaoProjeto = 993520000;
                    }

                    objPollux.CodigoClientePotencial = crmItem.ID.Value.ToString();

                    if (Oportunidade != null)
                    {
                        objPollux.CodigoOportunidade = Oportunidade.ID.Value.ToString();
                        objPollux.SituacaoProjeto = ObterRazaoStatusOportunidade(Oportunidade);
                    }
                    else
                    {
                        objPollux.SituacaoProjeto = ObterRazaoStatusClientePotencial(crmItem);
                    }

                    if (crmItem.RevendaIntegrador != null)
                    {
                        objPollux.CodigoRevenda = crmItem.RevendaIntegrador.Id.ToString();
                        objPollux.NomeRevenda = crmItem.RevendaIntegrador.Name;
                    }
                    else
                    {
                        objPollux.CodigoRevenda = string.Empty;
                        objPollux.NomeRevenda = string.Empty;
                    }

                    if (crmItem.Distribuidor != null)
                    {
                        objPollux.CodigoDistribuidor = crmItem.Distribuidor.Id.ToString();
                        objPollux.NomeDistribuidor = crmItem.Distribuidor.Name;
                    }

                    if (crmItem.Executivo != null)
                    {
                        objPollux.CodigoExecutivo = crmItem.Executivo.Id.ToString();
                        objPollux.NomeExecutivo = crmItem.Executivo.Name;
                    }

                    objPollux.CNPJCliente = crmItem.Cnpj.Replace("-", "").Replace(".", "").Replace("/", "").Trim();
                    objPollux.RazaoSocial = crmItem.NomeDaEmpresa;
                    if (crmItem.ValorEstimado.HasValue)
                    {
                        objPollux.ReceitaEstimada = crmItem.ValorEstimado;
                    }
                    else {
                        objPollux.ReceitaEstimada = 0;
                    }
                    if (crmItem.DataEstimada != null)
                        objPollux.DataPrevisaoFechamento = crmItem.DataEstimada.ToLocalTime();
                    else
                        objPollux.DataPrevisaoFechamento = null;

                    if (crmItem.UnidadeNegocio != null)
                    {
                        UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(crmItem.UnidadeNegocio.Id);
                        if (unidadeNegocio != null)
                        {
                            objPollux.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                            objPollux.NomeUnidadeNegocio = unidadeNegocio.Nome;
                        }
                    }
                    if (crmItem.DataCriacao != null)
                        objPollux.DataCadastro = crmItem.DataCriacao.ToLocalTime();
                    else
                        objPollux.DataCadastro = null;

                    lstProjetoItem.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0267R1>(numeroMensagem, retorno);
            }

            #endregion

            retorno.Add("ProjetosItens", lstProjetoItem);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0267R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ClientePotencial DefinirPropriedades(Intelbras.Message.Helper.MSG0267 xml)
        {
            var crm = new Model.ClientePotencial(this.Organizacao, this.IsOffline);
            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ClientePotencial objModel)
        {
            return String.Empty;
        }

        public int? ObterRazaoStatusClientePotencial(ClientePotencial clientePotencial)
        {
            int? razaostatus = 993520003;
            if (clientePotencial.RazaoStatus == 1)
            {
                razaostatus = 993520003;
            }
            else if (clientePotencial.RazaoStatus == 2)
            {
                if (clientePotencial.StageId != null)
                {
                    if ((clientePotencial.StageId.Value.ToString().ToUpper() == "00D0B952-45A4-3C15-E2A3-04E57485816E") || (clientePotencial.StageId.Value.ToString().ToUpper() == "B25D7A8D-C61D-1103-492C-AE77C795C9EE"))
                    {
                        razaostatus = 993520005;
                    }
                    if ((clientePotencial.StageId.Value.ToString().ToUpper() == "8AA82057-E761-70CE-4A4C-0184CA48D938") || (clientePotencial.StageId.Value.ToString().ToUpper() == "5106C715-92B3-0C12-3503-D3810765A259"))
                    {
                        razaostatus = 993520006;
                    }
                }
            }
            else if (clientePotencial.RazaoStatus == 3)
            {
                razaostatus = 993520007;
            }
            else if ((clientePotencial.RazaoStatus == 4) || (clientePotencial.RazaoStatus == 5))
            {
                razaostatus = 993520000;
            }else if ((clientePotencial.RazaoStatus == 6) || (clientePotencial.RazaoStatus == 7))
            {
                razaostatus = 993520002;
            }
            else
            {
                razaostatus = clientePotencial.RazaoStatus;
            }
            return razaostatus;
        }

        public int? ObterRazaoStatusOportunidade(Oportunidade oportunidade)
        {
            int? razaostatus = 993520003;
            if ((oportunidade.RazaoStatus == 1) || (oportunidade.RazaoStatus == 200011))
            {
                if (oportunidade.StageId != null)
                {
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "A5C2D354-2233-D85E-CE43-366685757134")
                    {
                        razaostatus = 993520001;
                    }
                    if ((oportunidade.StageId.Value.ToString().ToUpper() == "1DB22B78-19E7-5CA1-CCB1-BD84AE230087") || (oportunidade.StageId.Value.ToString().ToUpper() == "E2B91BCD-488A-5410-9049-1E7C8ADBE72C"))
                    {
                        razaostatus = 993520007;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "CDA64E48-BABC-4EBB-CE63-8D7DE2C9729D")
                    {
                        razaostatus = 993520008;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "08B6E87E-5D19-3952-64BC-D6C0EDE6DA37")
                    {
                        razaostatus = 993520009;
                    }
                    if (oportunidade.StageId.Value.ToString().ToUpper() == "6E7E126A-A9A0-6CDE-74AC-1509ABA87087")
                    {
                        razaostatus = 993520010;
                    }
                }
                else { razaostatus = 993520007; }
            }
            else if (oportunidade.RazaoStatus == 3)
            {
                razaostatus = 993520001;
            }
            else if (oportunidade.RazaoStatus == 4) 
            {
                razaostatus = 993520002;
            }
            else if (oportunidade.RazaoStatus == 5)
            {
                razaostatus = 993520000;
            }
            else
            {
                razaostatus = oportunidade.RazaoStatus;
            }
            return razaostatus;
        }
        #endregion
    }
}
