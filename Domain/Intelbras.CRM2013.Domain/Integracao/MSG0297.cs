using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0297 : Base, IBase<Message.Helper.MSG0297, Domain.Model.RegiaoDeAtuacao>
    {
        #region Construtor        
        public MSG0297(string org, bool isOffline) : base(org, isOffline)
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
            resultadoPersistencia.Sucesso = false;
            resultadoPersistencia.Mensagem = "Ação não permitida.";
            retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0297R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public RegiaoDeAtuacao DefinirPropriedades(Intelbras.Message.Helper.MSG0297 xml)
        {
            var crm = new RegiaoDeAtuacao(this.Organizacao, this.IsOffline);
            return crm;
        }

        private Intelbras.Message.Helper.MSG0297 DefinirPropriedades(RegiaoDeAtuacao crm)
        {
            Intelbras.Message.Helper.MSG0297 xml = new Pollux.MSG0297(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            xml.CodigoRegiaoAtuacao = crm.Id.ToString();
            xml.CodigoContato = crm.Contato.Id.ToString();
            xml.Nome = crm.Nome;
            Pais pais = new Servicos.PaisServices(this.Organizacao, this.IsOffline).BuscaPais(crm.Pais.Id);
            xml.NomePais = pais.Nome;

            if (crm.Estado != null)
            {
                Estado estado = new Servicos.EstadoServices(this.Organizacao, this.IsOffline).BuscaEstadoPorId(crm.Estado.Id);
                if (estado != null)
                {
                    xml.SiglaEstado = estado.SiglaUF;
                }
            }
            
            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : 0);

            return xml;
        }
        #endregion

        #region Métodos Auxiliares
        public string Enviar(RegiaoDeAtuacao objModel)
        {
            string retMsg = String.Empty;

            Intelbras.Message.Helper.MSG0297 mensagem = this.DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            if (integracao.EnviarMensagemBarramento(mensagem.GenerateMessage(true), "1", "1", out retMsg))
            {
                Intelbras.Message.Helper.MSG0297R1 retorno = CarregarMensagem<Pollux.MSG0297R1>(retMsg);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + string.Concat(retorno.Resultado.Mensagem));
                }
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 erro001 = CarregarMensagem<Pollux.ERR0001>(retMsg);
                throw new ArgumentException("(CRM) " + string.Concat("Erro de Integração \n", erro001.GenerateMessage(false)));
            }
            return retMsg;
        }
        #endregion
    }
}
