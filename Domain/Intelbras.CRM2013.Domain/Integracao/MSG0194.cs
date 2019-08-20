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
    public class MSG0194 : Base, IBase<Message.Helper.MSG0194, Domain.Model.RelacionamentoB2B>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request

        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        #endregion

        #region Construtor

        public MSG0194(string org, bool isOffline)
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
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0194>(mensagem));
            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0194R1>(numeroMensagem, retorno);
            }

            objeto = new Servicos.RelacionamentoB2BService(this.Organizacao, this.IsOffline).Persistir(objeto);
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
            return CriarMensagemRetorno<Pollux.MSG0194R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades

        public RelacionamentoB2B DefinirPropriedades(Intelbras.Message.Helper.MSG0194 xml)
        {
            var crm = new RelacionamentoB2B(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            crm.CodigoRelacionamentoB2B = xml.CodigoRelacionamentoB2B;

            if (!String.IsNullOrEmpty(xml.NomeRelacionamento))
                crm.NomeRelacionamento = xml.NomeRelacionamento;

            if (xml.CodigoGrupoCliente.HasValue)
                crm.CodigoGrupoCliente = xml.CodigoGrupoCliente;
            
            // Categoria B2B
            CategoriaB2B categoria = new CategoriaB2BService(this.Organizacao, this.IsOffline).ObterPor((int)xml.CodigoCategoriaB2B);
            if (categoria == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(CRM) Categoria B2B não encontrada.";
                return crm;
            }
            crm.CategoriaB2B = new Lookup(new Guid(categoria.ID.ToString()), "");

            // Conta
            Conta conta = new ContaService(this.Organizacao, this.IsOffline).BuscarPorCodigoEmitente(xml.CodigoCliente.ToString());
            if (conta == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "(CRM) Conta não encontrada para o Código Cliente informado.";
                return crm;
            }
            crm.Canal = new Lookup(new Guid(conta.ID.ToString()), "account");

            crm.CodigoRepresentante = xml.CodigoRepresentante;

            // Unidade Negócio / Unidade Comercial
            if (!String.IsNullOrEmpty(xml.NomeUnidadeComercial))
            {                
                UnidadeNegocio unidade = new UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorNome(xml.NomeUnidadeComercial);
                if (unidade != null)
                {
                    crm.NomeUnidadeNegocio = xml.NomeUnidadeComercial;
                    crm.UnidadeNegocio = new Lookup(unidade.ID.Value, "");
                }                
            }

            if(xml.Sequencia.HasValue)
                crm.Sequencia = xml.Sequencia;

            if(xml.DataInicial.HasValue)
                crm.DataInicial = xml.DataInicial;

            if (xml.DataFinal.HasValue)
                crm.DataFinal = xml.DataFinal;

            if(! String.IsNullOrEmpty(xml.Mensagem))
                crm.Mensagem = xml.Mensagem;

            crm.Status = xml.Situacao;
            
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(RelacionamentoB2B objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
