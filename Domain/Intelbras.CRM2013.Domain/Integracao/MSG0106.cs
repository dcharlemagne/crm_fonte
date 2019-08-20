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
    public class MSG0106 : Base, IBase<Message.Helper.MSG0106, Domain.Model.ColaboradorTreinadoCertificado>
    {

        #region Construtor

        public MSG0106(string org, bool isOffline)
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
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0106>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0106R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.ColaboradorTreinadoCertificadoService(this.Organizacao, this.IsOffline).Persistir(objeto);

            if (objeto != null)
            {
                retorno.Add("GUIDColaboradorTreinamento", objeto.ID.Value.ToString());
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
            return CriarMensagemRetorno<Pollux.MSG0106R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public ColaboradorTreinadoCertificado DefinirPropriedades(Intelbras.Message.Helper.MSG0106 xml)
        {
            var crm = new ColaboradorTreinadoCertificado(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            
            if (xml.DataConclusao.HasValue)
            {
                crm.DataConclusao = xml.DataConclusao;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "DataConclusao não enviada!";
                return crm;
            }

            if (xml.DataValidade.HasValue)
            {
                crm.DataValidade = xml.DataValidade;
            }
            else
            {
                //Se nao enviar datavalidade adicionamos 24meses a data de conclusao
                crm.DataValidade = crm.DataConclusao.Value.AddMonths(24);
            }

            if (xml.Situacao == 1 || xml.Situacao == 0)
                crm.Status = xml.Situacao;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Situação não enviada!";
                return crm;
            }
            if (xml.StatusAproveitamento.HasValue)
            {
                if (xml.StatusAproveitamento != (int)Enum.ColaboradorTreinamentoCertificado.Status.Aprovado
                    && xml.StatusAproveitamento != (int)Enum.ColaboradorTreinamentoCertificado.Status.Pendente
                    && xml.StatusAproveitamento != (int)Enum.ColaboradorTreinamentoCertificado.Status.Reprovado)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Status Aproveitamento não existente.";
                    return crm;
                }

                crm.RazaoStatus = xml.StatusAproveitamento;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Status Aproveitamento não enviado!";
                return crm;
            }
            if (xml.IdentificadorTreinamento.HasValue)
            {
                TreinamentoCertificacao treinamentoCertif = new Servicos.TreinamentoCertificacaoService(this.Organizacao, this.IsOffline).ObterPor(xml.IdentificadorTreinamento.Value);

                if (treinamentoCertif != null)
                {
                    crm.TreinamentoCertificado = new Lookup(treinamentoCertif.ID.Value, "");
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "IdentificadorTreinamento não cadastrado no Crm!";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "IdentificadorTreinamento não enviado!";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.CodigoContato))
            {
                Guid contato;
                if(!Guid.TryParse(xml.CodigoContato,out contato))
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "CodigoConta inválido!";
                    return crm;
                }

                crm.Contato = new Lookup(contato, "");

                Contato contatoObj = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContato(contato);
                if (contatoObj == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Contato não encontrado!";
                    return crm;
                }
                if (!String.IsNullOrEmpty(contatoObj.NomeCompleto))
                    crm.Nome = contatoObj.NomeCompleto + " em " + xml.IdentificadorTreinamento.ToString();
                else if(!String.IsNullOrEmpty(contatoObj.PrimeiroNome))
                    crm.Nome = contatoObj.PrimeiroNome + " em " + xml.IdentificadorTreinamento.ToString();
                else
                    crm.Nome = xml.IdentificadorTreinamento.ToString();

                Helper.Truncate(crm.Nome, 100);

                if (contatoObj.AssociadoA != null && contatoObj.AssociadoA.Type == SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.Conta>().ToLower())
                {
                    crm.Canal = new Lookup(contatoObj.AssociadoA.Id, "");
                }
                
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "CodigoContato não enviado!";
                return crm;
            }
            
            if (xml.IdentificadorMatricula.HasValue)
            {
                crm.IdMatricula = xml.IdentificadorMatricula;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "IdentificadorMatricula não enviado!";
                return crm;
            }
            
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ColaboradorTreinadoCertificado objModel)
        {
            string resposta = string.Empty;

            return resposta;
        }

        #endregion

    }
}
