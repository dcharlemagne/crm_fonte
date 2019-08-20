using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0130 : Base, IBase<Intelbras.Message.Helper.MSG0130, Domain.Model.UnidadeNegocio>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        private Int32 codigoUnidadeNegocio;
        #endregion

        #region Construtor
        public MSG0130(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0130>(mensagem);
            
            
            //Retorna todos registros
            List<UnidadeNegocio> lstUnidNegocio= new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).ListarTodos();

            List<Pollux.Entities.UnidadeNegocio> unidadesNegociosItens = this.ConverteLista(lstUnidNegocio);

            if (unidadesNegociosItens != null && unidadesNegociosItens.Count > 0)
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("UnidadesNegocioItens", unidadesNegociosItens);

            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "REgistrso não encontrados no Crm.";
            }
            
            
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0130R1>(numeroMensagem, retorno);
        }

        private List<Pollux.Entities.UnidadeNegocio> ConverteLista(List<UnidadeNegocio> LstRegistrosCrm)
        {
            List<Pollux.Entities.UnidadeNegocio> lstPollux = new List<Pollux.Entities.UnidadeNegocio>();

            foreach (UnidadeNegocio item in LstRegistrosCrm)
            {
                Pollux.Entities.UnidadeNegocio Objeto = new Pollux.Entities.UnidadeNegocio();

                if (!String.IsNullOrEmpty(item.ChaveIntegracao))
                    Objeto.CodigoUnidadeNegocio = item.ChaveIntegracao;
                else
                    Objeto.CodigoUnidadeNegocio = "0";

                if (!String.IsNullOrEmpty(item.Nome))
                    Objeto.NomeUnidadeNegocio = item.Nome;

                if (item.NegocioPrimario != null)
                {
                    Objeto.CodigoUnidadeNegocioPrimaria = item.NegocioPrimario.Id.ToString();

                    if (!String.IsNullOrEmpty(item.NegocioPrimario.Name))
                        Objeto.NomeUnidadeNegocioPrimaria = item.NegocioPrimario.Name;
                }


                lstPollux.Add(Objeto);
            }
            return lstPollux;
        }

        #endregion

        #region Definir Propriedades

        public UnidadeNegocio DefinirPropriedades(Intelbras.Message.Helper.MSG0130 xml)
        {
            var crm = new UnidadeNegocio(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(UnidadeNegocio objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
