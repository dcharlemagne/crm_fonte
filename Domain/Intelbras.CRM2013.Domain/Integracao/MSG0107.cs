using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0107 : Base, IBase<Message.Helper.MSG0107, Domain.Model.Denuncia>
    {

        #region Construtor

        public MSG0107(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0107>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0107R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.DenunciaService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("CodigoDenuncia", objeto.ID.Value.ToString());
                if (usuarioIntegracao != null)
                {
                    retorno.Add("Proprietario", usuarioIntegracao.ID.Value.ToString());
                    retorno.Add("TipoProprietario", "systemuser");
                }
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro de Persistência!";
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0107R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public Denuncia DefinirPropriedades(Intelbras.Message.Helper.MSG0107 xml)
        {
            var crm = new Denuncia(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml


            if (!String.IsNullOrEmpty(xml.CodigoDenuncia))
            {
                if (xml.CodigoDenuncia.Length == 36)
                    crm.ID = new Guid(xml.CodigoDenuncia);
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoDenuncia enviado está fora do padrão(Guid).";
                    return crm;
                }
            }
            if (!String.IsNullOrEmpty(xml.Descricao))
                crm.Descricao = xml.Descricao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Descrição não enviada.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoTipoDenuncia) && xml.CodigoTipoDenuncia.Length == 36)
            {
                TipoDeDenuncia tipoDenuncia = new Servicos.DenunciaService(this.Organizacao, this.IsOffline).ObterTipoDenuncia(new Guid(xml.CodigoTipoDenuncia));
                if (tipoDenuncia != null)
                {
                    crm.Nome = "Denúncia de : " + tipoDenuncia.Nome;
                    crm.TipoDenuncia = new Lookup(tipoDenuncia.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tipo de Denuncia não cadastrado no Crm.";
                    return crm;
                }

            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoTipoDenuncia fora do padrão(Guid) ou não enviado.";
                return crm;
            }
            if (!String.IsNullOrEmpty(xml.Justificativa))
            {
                crm.Justificativa = xml.Justificativa;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Justificativa não enviada.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoCanalDenunciado) && xml.CodigoCanalDenunciado.Length == 36)
            {
                crm.Denunciado = new Lookup(new Guid(xml.CodigoCanalDenunciado), "");
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Codigo Canal Denunciado não enviado ou fora do padrão(Guid).";
                return crm;
            }
            //Status 
            if (xml.Situacao == 1 || xml.Situacao == 0)
                crm.Status = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Status não encontrado!";
                return crm;
            }

            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Denuncia.StatusDenuncia), xml.SituacaoDenuncia))
                crm.RazaoStatus = xml.SituacaoDenuncia;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "SituacaoDenuncia não encontrada!";
                return crm;
            }
            if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Domain.Enum.Denuncia.TipoDenunciante), xml.TipoDenunciante))
                crm.TipoDenunciante = xml.TipoDenunciante;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "SituacaoDenuncia não encontrada!";
                return crm;
            }

            if (xml.CodigoRepresentante.HasValue)
            {

                Contato contato = new Contato(this.Organizacao, this.IsOffline);
                contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(xml.CodigoRepresentante.Value.ToString());

                if (contato != null)
                    crm.Representante = new Lookup(contato.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Representante não encontrado.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("Representante");
            }

            if (!String.IsNullOrEmpty(xml.CodigoColaboradorIntelbras))
            {
                if (xml.CodigoColaboradorIntelbras.Length == 36)
                    crm.ColaboradorIntebras = new Lookup(new Guid(xml.CodigoColaboradorIntelbras), "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoColaboradorIntelbras fora do padrão(Guid).";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ColaboradorIntebras");
            }

            if (!String.IsNullOrEmpty(xml.CodigoCanalDenunciante))
            {
                if (xml.CodigoCanalDenunciante.Length == 36)
                    crm.CanalDenunciante = new Lookup(new Guid(xml.CodigoCanalDenunciante), "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoCanalDenunciante fora do padrão(Guid).";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("CanalDenunciante");
            }

            if (!String.IsNullOrEmpty(xml.CodigoColaboradorCanal))
            {
                if (xml.CodigoColaboradorCanal.Length == 36)
                    crm.ColaboradorCanal = new Lookup(new Guid(xml.CodigoColaboradorCanal), "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoColaboradorCanal fora do padrão(Guid).";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ColaboradorCanal");
            }

            if (!String.IsNullOrEmpty(xml.NomeDenunciante))
            {
                crm.Denunciante = xml.NomeDenunciante;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome do Denunciante não Enviado.";
                return crm;
            }




            string tipoProprietario;

            if (xml.TipoProprietario == "team" || xml.TipoProprietario == "systemuser")
                tipoProprietario = xml.TipoProprietario;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Tipo Proprietário não Enviado.";
                return crm;
            }

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Denuncia objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
