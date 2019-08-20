using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0046 : Base, IBase<Message.Helper.MSG0046, Domain.Model.Indice>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };


        #endregion

        #region Construtor
        public MSG0046(string org, bool isOffline)
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
            usuarioIntegracao = usuario;

            Indice objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0046>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0046R1>(numeroMensagem, retorno);
            }

            objeto = new Intelbras.CRM2013.Domain.Servicos.IndiceService(this.Organizacao, this.IsOffline).Persistir(objeto);

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
            return CriarMensagemRetorno<Pollux.MSG0046R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public Indice DefinirPropriedades(Intelbras.Message.Helper.MSG0046 xml)
        {
            var crm = new Indice(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            crm.Nome = xml.Nome;

            TabelaFinanciamento TblFinanciamento = new Intelbras.CRM2013.Domain.Servicos.TabelaFinanciamentoService(this.Organizacao, this.IsOffline).
                                                    ObterTabelaFinanciamento(xml.TabelaFinanciamento.ToString());
            if (TblFinanciamento != null)
                crm.TabelaFinanciamento = new Lookup((Guid)TblFinanciamento.ID, "");
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Identificador Tabela Financiamento não encontrado";
                return crm;
            }

            crm.Indiceid = xml.Indice;

            crm.Status = xml.Situacao;
            
            crm.ChaveIntegracao = xml.ChaveIntegracao;
            
            crm.DiaIndice = Convert.ToInt32(xml.NumeroDias);

            crm.IntegradoEm = DateTime.Now;

            crm.IntegradoPor = usuarioIntegracao.NomeCompleto;

            crm.UsuarioIntegracao = xml.LoginUsuario;
            
            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares
     
        public string Enviar(Indice objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
