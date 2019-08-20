using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;
using Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0109 : Base, IBase<Message.Helper.MSG0109, Domain.Model.Denuncia>
    {

        #region Construtor

        public MSG0109(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0109>(mensagem);

            if (!String.IsNullOrEmpty(xml.CodigoDenuncia) && xml.CodigoDenuncia.Length == 36)
            {
                Denuncia denuncia = new Servicos.DenunciaService(this.Organizacao, this.IsOffline).ObterDenuncia(new Guid(xml.CodigoDenuncia));
                if (denuncia != null)
                {
                    var objeto = this.DefinirRetorno(denuncia);
                    if (objeto == null)
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Erro de consulta no Crm.";
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0109R1>(numeroMensagem, retorno);
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = true;
                        resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                        retorno.Add("Denuncia", objeto);
                        retorno.Add("Resultado", resultadoPersistencia);
                        return CriarMensagemRetorno<Pollux.MSG0109R1>(numeroMensagem, retorno);
                    }
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0109R1>(numeroMensagem, retorno);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Código Denúncia não enviado ou fora do padrão(Guid).";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0109R1>(numeroMensagem, retorno);
            }

        }
        #endregion

        #region Definir Propriedades

        public Denuncia DefinirPropriedades(Intelbras.Message.Helper.MSG0109 xml)
        {
            Denuncia retorno = new Denuncia(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public Pollux.Entities.Denuncia DefinirRetorno(Model.Denuncia denunciaCrm)
        {
            Pollux.Entities.Denuncia denunciaPollux = new Pollux.Entities.Denuncia();

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(denunciaCrm.Descricao))
                denunciaPollux.Descricao = denunciaCrm.Descricao;
            else
                denunciaPollux.Descricao = "N/A";

            if (denunciaCrm.TipoDenuncia != null)
            {
                denunciaPollux.NomeDenuncia = "Denúncia de " + denunciaCrm.TipoDenuncia.Name;
                denunciaPollux.CodigoTipoDenuncia = denunciaCrm.TipoDenuncia.Id.ToString();
                denunciaPollux.NomeTipoDenuncia = denunciaCrm.TipoDenuncia.Name;
            }
            else
            {
                denunciaPollux.NomeDenuncia = "N/A";
                denunciaPollux.CodigoTipoDenuncia = Guid.Empty.ToString();
                denunciaPollux.NomeTipoDenuncia = "N/A";
            }
            if (!String.IsNullOrEmpty(denunciaCrm.Justificativa))
                denunciaPollux.Justificativa = denunciaCrm.Justificativa;
            else
                denunciaPollux.Justificativa = "N/A";

            if (denunciaCrm.Denunciado != null)
            {
                denunciaPollux.CodigoCanalDenunciado = denunciaCrm.Denunciado.Id.ToString();
                denunciaPollux.NomeCanalDenunciado = denunciaCrm.Denunciado.Name;
            }
            else
            {
                denunciaPollux.CodigoCanalDenunciado = Guid.Empty.ToString();
                denunciaPollux.NomeCanalDenunciado = "N/A";
            }
            if (denunciaCrm.RazaoStatus.HasValue)
            {
                denunciaPollux.SituacaoDenuncia = denunciaCrm.RazaoStatus.Value;
                switch (denunciaCrm.RazaoStatus.Value)
                {
                    case (int)Enum.Denuncia.StatusDenuncia.EmAnalise:
                        denunciaPollux.NomeSituacaoDenuncia = "Em Análise";
                        break;
                    case (int)Enum.Denuncia.StatusDenuncia.AguardandoJustificativa:
                        denunciaPollux.NomeSituacaoDenuncia = "Aguardando Justificativa";
                        break;
                    case (int)Enum.Denuncia.StatusDenuncia.JustificativaProvida:
                        denunciaPollux.NomeSituacaoDenuncia = "Justificativa Provida";
                        break;
                    case (int)Enum.Denuncia.StatusDenuncia.AnaliseJustificativa:
                        denunciaPollux.NomeSituacaoDenuncia = "Análise de Justificativa";
                        break;
                    case (int)Enum.Denuncia.StatusDenuncia.DenunciaProcedente:
                        denunciaPollux.NomeSituacaoDenuncia = "Denúncia Procedente";
                        break;
                    case (int)Enum.Denuncia.StatusDenuncia.DenunciaImprocedente:
                        denunciaPollux.NomeSituacaoDenuncia = "Denúncia Improcedente";
                        break;
                    default:
                        denunciaPollux.NomeSituacaoDenuncia = "N/A";
                        break;
                }
            }

            else
            {
                denunciaPollux.SituacaoDenuncia = 0;
                denunciaPollux.NomeSituacaoDenuncia = "N/A";
            }
            if (denunciaCrm.Status.HasValue)
                denunciaPollux.Situacao = denunciaCrm.Status.Value;
            if (usuarioIntegracao != null)
            {
                denunciaPollux.Proprietario = usuarioIntegracao.ID.Value.ToString();
                denunciaPollux.NomeProprietario = usuarioIntegracao.Nome;
                denunciaPollux.TipoProprietario = "systemuser";
            }
            else
            {
                denunciaPollux.Proprietario = Guid.Empty.ToString(); 
                denunciaPollux.NomeProprietario = "N/A";
                denunciaPollux.TipoProprietario = "N/A";
            }

            if (denunciaCrm.TipoDenunciante.HasValue)
                denunciaPollux.TipoDenunciante = denunciaCrm.TipoDenunciante.Value;

            if (denunciaCrm.TipoDenunciante.HasValue)
            {
                switch (denunciaCrm.TipoDenunciante.Value)
                {
                    case (int)Enum.Denuncia.TipoDenunciante.Anonimo:
                        denunciaPollux.NomeTipoDenunciante = "Anônimo";
                        break;
                    case (int)Enum.Denuncia.TipoDenunciante.ColaboradordoCanal:
                        denunciaPollux.NomeTipoDenunciante = "Colaborador do Canal";
                        break;
                    case (int)Enum.Denuncia.TipoDenunciante.ColaboradorIntelbras:
                        denunciaPollux.NomeTipoDenunciante = "Colaborador Intelbras";
                        break;
                    case (int)Enum.Denuncia.TipoDenunciante.KeyAccountRepresentante:
                        denunciaPollux.NomeTipoDenunciante = "Key Account/Representante";
                        break;
                    default:
                        denunciaPollux.NomeTipoDenunciante = "N/A";
                        break;
                }

            }
            else
                denunciaPollux.NomeTipoDenunciante = "N/A";

            if (denunciaCrm.Representante != null)
            {
                Contato contatoRep = new Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(denunciaCrm.Representante.Id);

                if (contatoRep != null && !String.IsNullOrEmpty(contatoRep.CodigoRepresentante))
                {
                    int CodRep = 0;
                    if (Int32.TryParse(contatoRep.CodigoRepresentante, out CodRep))
                    {
                        denunciaPollux.CodigoRepresentante = CodRep;
                    }
                    else
                    {
                        denunciaPollux.CodigoRepresentante = 0;
                    }
                    if (!String.IsNullOrEmpty(contatoRep.PrimeiroNome))
                        denunciaPollux.NomeRepresentante = contatoRep.PrimeiroNome;
                    else
                        denunciaPollux.NomeRepresentante = "N/A";
                }
                else
                {
                    denunciaPollux.CodigoRepresentante = 0;
                }
            }

            if (denunciaCrm.ColaboradorCanal != null)
            {
                denunciaPollux.CodigoColaboradorCanal = denunciaCrm.ColaboradorCanal.Id.ToString();
                denunciaPollux.NomeColaboradorCanal = denunciaCrm.ColaboradorCanal.Name;
            }

            if (denunciaCrm.CanalDenunciante != null)
            {
                denunciaPollux.CodigoCanalDenunciante = denunciaCrm.CanalDenunciante.Id.ToString();
                denunciaPollux.NomeCanalDenunciante = denunciaCrm.CanalDenunciante.Name;
            }

            if (denunciaCrm.ColaboradorIntebras != null)
            {
                denunciaPollux.CodigoColaboradorIntelbras = denunciaCrm.ColaboradorIntebras.Id.ToString();
                denunciaPollux.NomeColaboradorIntelbras = denunciaCrm.ColaboradorIntebras.Name;
            }

            if (!String.IsNullOrEmpty(denunciaCrm.Denunciante))
                denunciaPollux.NomeDenunciante = Helper.Truncate(denunciaCrm.Denunciante, 100);
            else
                denunciaPollux.NomeDenunciante = "N/A";


            #endregion

            return denunciaPollux;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Denuncia objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion


        Denuncia IBase<Pollux.MSG0109, Denuncia>.DefinirPropriedades(Pollux.MSG0109 legado)
        {
            throw new NotImplementedException();
        }
    }
}
