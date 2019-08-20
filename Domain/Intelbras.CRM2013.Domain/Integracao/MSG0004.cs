// MENSAGEM RELACIONADA AO PROCESSO REGISTRA CONDICAO DE PGTO
// IMPLEMENTADA EM 18/03/14 POR FCJ

using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0004 : Base, IBase<Message.Helper.MSG0004, Domain.Model.CondicaoPagamento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        RepositoryService _Repository = null;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0004(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;

            _Repository = new RepositoryService(org, isOffline);
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
            usuarioIntegracao = usuario;

            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0004>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0004R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).Persistir((Guid)usuario.ID, objeto);

            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Registro não encontrado!";

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            }

            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0004R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public CondicaoPagamento DefinirPropriedades(Intelbras.Message.Helper.MSG0004 xml)
        {
            var crm = new CondicaoPagamento(this.Organizacao, this.IsOffline);

            crm.Codigo = xml.CodigoCondicaoPagamento;

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;

            if (xml.NumeroParcelas.HasValue)
                crm.NumeroParcelas = xml.NumeroParcelas;

            if (xml.ChaveIntegracaoIndice != null && xml.ChaveIntegracaoIndice != String.Empty)
            {
                var IntegracaoIndice = _Repository.Indice.ObterPor(xml.ChaveIntegracaoIndice);
                if(IntegracaoIndice != null)
                {
                    crm.Indice = new SDKore.DomainModel.Lookup(IntegracaoIndice.ID.Value, IntegracaoIndice.Nome, SDKore.Crm.Util.Utility.GetEntityName(IntegracaoIndice));
                }
            }

            if (xml.NumeroTabelaFinanciamento != null && xml.NumeroTabelaFinanciamento != String.Empty)
            {
                var IntegracaoTabelaFinanciamento = _Repository.TabelaFinanciamento.ObterPor(xml.NumeroTabelaFinanciamento);
                if (IntegracaoTabelaFinanciamento != null)
                    crm.TabelaFinanciamento = new SDKore.DomainModel.Lookup(IntegracaoTabelaFinanciamento.ID.Value, IntegracaoTabelaFinanciamento.Nome, SDKore.Crm.Util.Utility.GetEntityName(IntegracaoTabelaFinanciamento));
            }

            if (xml.PercentualDesconto.HasValue)
                crm.PercDesconto = xml.PercentualDesconto;

            if (xml.Prazo.HasValue)
                crm.Prazos = xml.Prazo;

            if (xml.Situacao.HasValue)
                crm.Status = xml.Situacao;

            crm.SupplierCard = xml.SupplierCard;

            crm.IntegradoEm = DateTime.Now;
            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            crm.UsuarioIntegracao = xml.LoginUsuario;
            crm.UtilizadoParaB2B = xml.UtilizadoB2B;
            crm.UtilizadoParaCanais = xml.UtilizadoCanais;
            crm.UtilizadoParaFornecedores = xml.UtilizadoFornecedores;
            crm.UtilizadoParaSdcv = xml.UtilizadoSDCV;
            crm.UtilizadoRevenda = xml.UtilizadoRevenda;

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(CondicaoPagamento objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}