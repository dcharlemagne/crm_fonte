using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0129 : Base, IBase<Intelbras.Message.Helper.MSG0129, Domain.Model.Segmento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        private string codigoUnidadeNegocio;
        #endregion

        #region Construtor
        public MSG0129(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0129>(mensagem);
            
            if(!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {//Retorna registros com base no estabelecimento

                //Pegamos a Unidade de negócio com base no codigo do erp dele
                UnidadeNegocio objUnidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);

                if (objUnidadeNegocio == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Unidade de Negócio não encontrada.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0129R1>(numeroMensagem, retorno);
                }

                List<Segmento> lstSegmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).ListarPorUnidadeNegocio((Guid)objUnidadeNegocio.ID);

                if (lstSegmento.Any<Segmento>())
                {
                    //para nao precisar refazer a service
                    codigoUnidadeNegocio = xml.CodigoUnidadeNegocio;
                    List<Pollux.Entities.Segmento> SegmentoItens = this.ConverteLista(lstSegmento);
                    resultadoPersistencia.Sucesso = true;
                    retorno.Add("SegmentosItens", SegmentoItens);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Segmento não encontrado.";
                }
            }
            else
            {//Retorna todos registros
                List<Segmento> lstSegmento = new Intelbras.CRM2013.Domain.Servicos.SegmentoService(this.Organizacao, this.IsOffline).ListarTodos();

                List<Pollux.Entities.Segmento> lstRetorno = this.ConverteLista(lstSegmento);


                if (lstSegmento != null)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "";
                    retorno.Add("SegmentosItens", lstRetorno);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Registros não encontrados.";
                }

            }
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0129R1>(numeroMensagem, retorno);
        }

        private List<Pollux.Entities.Segmento> ConverteLista(List<Segmento> LstRegistrosCrm)
        {
            List<Pollux.Entities.Segmento> lstPollux = new List<Pollux.Entities.Segmento>();

            foreach (Segmento item in LstRegistrosCrm)
            {
                Pollux.Entities.Segmento Objeto = new Pollux.Entities.Segmento();

                if (!String.IsNullOrEmpty(item.CodigoSegmento))
                    Objeto.CodigoSegmento = item.CodigoSegmento;

                if (!String.IsNullOrEmpty(item.Nome))
                Objeto.NomeSegmento = item.Nome;

                if (item.UnidadeNegocios != null)
                {
                    UnidadeNegocio unidadeNeg = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(item.UnidadeNegocios.Id);
                    if (unidadeNeg != null)
                    {
                        Objeto.NomeUnidadeNegocio = unidadeNeg.Nome;
                        Objeto.CodigoUnidadeNegocio = unidadeNeg.ChaveIntegracao;
                    }
                }

                if (item.GerenteResponsavel != null)
                {
                    Objeto.CodigoGerenteResponsavel = item.GerenteResponsavel.Id.ToString() ;

                    Objeto.NomeGerenteResponsavel = item.GerenteResponsavel.Name;

                }
                if(item.QtdMaximaShowRoom.HasValue)
                    Objeto.QuantidadeMaximaShowRoom = item.QtdMaximaShowRoom;

                lstPollux.Add(Objeto);
            }
            return lstPollux;
        }

        #endregion

        #region Definir Propriedades

        public Segmento DefinirPropriedades(Intelbras.Message.Helper.MSG0129 xml)
        {
            var crm = new Segmento(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

           
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Segmento objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
