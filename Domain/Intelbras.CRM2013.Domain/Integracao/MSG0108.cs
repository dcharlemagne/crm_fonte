using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0108 : Base, IBase<Message.Helper.MSG0108, Domain.Model.Denuncia>
    {

        #region Construtor

        public MSG0108(string org, bool isOffline)
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


            List<Pollux.Entities.DenunciaItem> lstObjeto = this.DefinirRetorno(this.CarregarMensagem<Pollux.MSG0108>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0108R1>(numeroMensagem, retorno);
            }

            if (lstObjeto != null && lstObjeto.Count > 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("DenunciasItens", lstObjeto);
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0108R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Denuncia DefinirPropriedades(Intelbras.Message.Helper.MSG0108 xml)
        {
            Denuncia retorno = new Denuncia(this.Organizacao, this.IsOffline);
            return retorno;
        }
        public List<Pollux.Entities.DenunciaItem> DefinirRetorno(Intelbras.Message.Helper.MSG0108 xml)
        {
            #region Propriedades Crm->Xml
            List<Denuncia> lstDenunciaCrm = new List<Denuncia>();
            List<Pollux.Entities.DenunciaItem> lstRetorno = new List<Pollux.Entities.DenunciaItem>();

            if (xml.SituacaoDenuncia!= null)
            {
                if (!System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Denuncia.StatusDenuncia), xml.SituacaoDenuncia))
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "SituaçãoDenuncia não cadastrada no Crm.";
                    return null;
                }
            }
            List<Guid> lstCanalDenunciantes = new List<Guid>();
            if (xml.CanaisDenunciantes != null && xml.CanaisDenunciantes.Count > 0)
            {
                foreach (var itemDenunciante in xml.CanaisDenunciantes)
                {
                    if (itemDenunciante.Length == 36)
                    {
                        var canalItemDenunciante = new Guid(itemDenunciante);
                        lstCanalDenunciantes.Add(canalItemDenunciante);
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Canal Denunciante fora do padrão(Guid)";
                        return null;
                    }
                }
            }

            List<Guid> lstCanalDenunciado = new List<Guid>();
            if (xml.CanaisDenunciados != null && xml.CanaisDenunciados.Count > 0)
            {
                foreach (var itemDenunciado in xml.CanaisDenunciados)
                {
                    if (itemDenunciado.Length == 36)
                    {
                        var canalItem = new Guid(itemDenunciado);
                        lstCanalDenunciado.Add(canalItem);
                    }
                    else
                    {
                        resultadoPersistencia.Sucesso = false;
                        resultadoPersistencia.Mensagem = "Canal Denunciado fora do padrão(Guid)";
                        return null;
                    }
                }
            }
            Guid? representanteKeyAccountId = null;

            if (xml.CodigoRepresentante.HasValue)
            {
                Contato contato = new Contato(this.Organizacao, this.IsOffline);
                contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.Value.ToString());

                if (contato != null)
                {
                    representanteKeyAccountId = contato.ID.Value;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Representante não encontrado.";
                    return null;
                }
            }


            lstDenunciaCrm = new Servicos.DenunciaService(this.Organizacao, this.IsOffline).ListarDenuncias(xml.DataInicio, xml.DataFinal, lstCanalDenunciantes, lstCanalDenunciado, representanteKeyAccountId, xml.SituacaoDenuncia);
            //De -Para - Crm - pollux
            if (lstDenunciaCrm != null && lstDenunciaCrm.Count > 0)
            {

                foreach (var itemCrm in lstDenunciaCrm)
                {
                    Pollux.Entities.DenunciaItem denunciaItem = new Pollux.Entities.DenunciaItem();
                    if (itemCrm.Descricao != null && !String.IsNullOrEmpty(itemCrm.Descricao))
                        denunciaItem.Descricao = itemCrm.Descricao;
                    else
                        denunciaItem.Descricao = "N/A";
                    if (itemCrm.Nome != null && !String.IsNullOrEmpty(itemCrm.Nome))
                        denunciaItem.NomeDenuncia = itemCrm.Nome;
                    else
                        denunciaItem.NomeDenuncia = "N/A";

                    if (itemCrm.RazaoStatus.HasValue)
                        denunciaItem.SituacaoDenuncia = itemCrm.RazaoStatus.Value;
                    else
                        denunciaItem.SituacaoDenuncia = 0;

                    switch (itemCrm.RazaoStatus.Value)
                    {
                        case (int)Enum.Denuncia.StatusDenuncia.EmAnalise:
                            denunciaItem.NomeSituacaoDenuncia = "Em Análise";
                            break;
                        case (int)Enum.Denuncia.StatusDenuncia.AguardandoJustificativa:
                            denunciaItem.NomeSituacaoDenuncia = "Aguardando Justificativa";
                            break;
                        case (int)Enum.Denuncia.StatusDenuncia.JustificativaProvida:
                            denunciaItem.NomeSituacaoDenuncia = "Justificativa Provida";
                            break;
                        case (int)Enum.Denuncia.StatusDenuncia.AnaliseJustificativa:
                            denunciaItem.NomeSituacaoDenuncia = "Análise de Justificativa";
                            break;
                        case (int)Enum.Denuncia.StatusDenuncia.DenunciaProcedente:
                            denunciaItem.NomeSituacaoDenuncia = "Denúncia Procedente";
                            break;
                        case (int)Enum.Denuncia.StatusDenuncia.DenunciaImprocedente:
                            denunciaItem.NomeSituacaoDenuncia = "Denúncia Improcedente";
                            break;
                        default:
                            denunciaItem.NomeSituacaoDenuncia = "N/A";
                            break;
                    }

                    if (itemCrm.TipoDenuncia != null)
                    {
                        denunciaItem.NomeTipoDenuncia = itemCrm.TipoDenuncia.Name;
                        denunciaItem.CodigoTipoDenuncia = itemCrm.TipoDenuncia.Id.ToString();
                    }
                    else
                    {
                        denunciaItem.NomeTipoDenuncia = "N/A";
                        denunciaItem.CodigoTipoDenuncia = Guid.Empty.ToString();
                    }
                    if (!String.IsNullOrEmpty(itemCrm.Justificativa))
                        denunciaItem.Justificativa = itemCrm.Justificativa;

                    if (itemCrm.DataCriacao.HasValue)
                        denunciaItem.DataCriacao = itemCrm.DataCriacao.Value;
                    else
                        denunciaItem.DataCriacao = DateTime.MinValue;
                    denunciaItem.CodigoDenuncia = itemCrm.ID.Value.ToString();

                    lstRetorno.Add(denunciaItem);
                }
            }

            return lstRetorno;
            #endregion
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Denuncia objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion


        Denuncia IBase<Pollux.MSG0108, Denuncia>.DefinirPropriedades(Pollux.MSG0108 legado)
        {
            throw new NotImplementedException();
        }
    }
}
