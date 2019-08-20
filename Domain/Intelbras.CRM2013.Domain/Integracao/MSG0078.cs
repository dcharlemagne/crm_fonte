using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0078 : Base, IBase<Message.Helper.MSG0078, Domain.Model.LinhaCorteDistribuidor>
    {

        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };

        Pollux.Entities.LinhaCorte retornoLinhaCorteItem = new Pollux.Entities.LinhaCorte { };

        Pollux.Entities.EstadoItem listaEstados = new Pollux.Entities.EstadoItem() { };

        List<Pollux.Entities.LinhaCorte> response = new List<Pollux.Entities.LinhaCorte>();

        #endregion

        #region Construtor
        public MSG0078(string org, bool isOffline)
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

            var xml = this.CarregarMensagem<Pollux.MSG0078>(mensagem);
            Conta objetoConta = null;
            UnidadeNegocio objetoUnidade = null;
            Classificacao objetoclassificacao = null;
            Categoria objetocategoria = null;

            Estado objetoEstado = null;
            if (string.IsNullOrEmpty(xml.CodigoConta) && string.IsNullOrEmpty(xml.CodigoUnidadeNegocio) && string.IsNullOrEmpty(xml.ChaveIntegracaoEstado))
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Paramêtros não enviados.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
            }

            if(!String.IsNullOrEmpty(xml.CodigoConta))
            {
                objetoConta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (objetoConta == null)
                {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "[Conta] não encontrada.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }

            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                objetoUnidade = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (objetoUnidade == null)
                {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "[Unidade Negócio] não encontrada.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }
            
            //new

            if (!String.IsNullOrEmpty(xml.Classificacao))
            {
                objetoclassificacao = new Intelbras.CRM2013.Domain.Servicos.ClassificacaoService(this.Organizacao, this.IsOffline).BuscaClassificacao(new Guid(xml.Classificacao));
                if (objetoclassificacao == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "[Classificao] não encontrada.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }

            if (!String.IsNullOrEmpty(xml.Categoria))
            {
                objetocategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaService(this.Organizacao, this.IsOffline).BuscaCategoria(new Guid(xml.
                     Categoria));
                if (objetocategoria == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "[Categoria] não encontrada.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }

            //

            if (!String.IsNullOrEmpty(xml.ChaveIntegracaoEstado))
            {
                objetoEstado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(xml.ChaveIntegracaoEstado);
                if (objetoEstado == null)
                {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "[Estado] não encontrado.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }

            List<CategoriasCanal> lstObjetoCategoria = new List<CategoriasCanal>();
            List<Guid> lstUnd = new List<Guid>();
            if (objetoConta != null)
            {
                if(objetoUnidade != null)
                    lstObjetoCategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.Organizacao, this.IsOffline).ListarPor((Guid)objetoConta.ID, (Guid)objetoUnidade.ID);
                else
                    lstObjetoCategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.Organizacao, this.IsOffline).ListarPor((Guid)objetoConta.ID, null);

                foreach (CategoriasCanal item in lstObjetoCategoria ?? Enumerable.Empty<CategoriasCanal>())
                {
                    if (!lstUnd.Contains(item.UnidadeNegocios.Id))
                    {
                        lstUnd.Add(item.UnidadeNegocios.Id);
                    }
                }
                if (lstUnd != null && lstUnd.Count == 0)
                {
                    resultadoConsulta.Sucesso = true;
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }
            else if (objetoUnidade != null)
            {
                lstUnd.Add(objetoUnidade.ID.Value);
            }

            var objetoMunicipio = new Intelbras.CRM2013.Domain.Servicos.MunicipioServices(this.Organizacao, this.IsOffline).ObterPor(objetoConta.Endereco1Municipioid.Id);
            List<LinhaCorteDistribuidor> objLinhaCorteItem = new Intelbras.CRM2013.Domain.Servicos.LinhaCorteService(this.Organizacao, this.IsOffline).ListarLinhadeCorteDistribuidor(lstUnd,objetoEstado, objetoMunicipio.CapitalOuInterior);
            
            foreach (var item in objLinhaCorteItem)
            {
                // service que pega o objLinhaCorte com base na unidade de negócio e o estadoId da conta
                List<LinhaCorteEstado> LinhaCorteEstado = new Intelbras.CRM2013.Domain.Servicos.LinhaCorteService(this.Organizacao,this.IsOffline).ObterLinhaDeCorteDistribuidorEstadoPorIdDistribuidor(item.ID.Value);

                retornoLinhaCorteItem.CodigoLinhaCorte = item.ID.ToString();
                
                if(!String.IsNullOrEmpty(item.Nome))
                    retornoLinhaCorteItem.Nome = item.Nome;

                if (item.UnidadeNegocios != null)
                {
                    retornoLinhaCorteItem.CodigoUnidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(item.UnidadeNegocios.Id).ChaveIntegracao;
                    retornoLinhaCorteItem.NomeUnidadeNegocio = item.UnidadeNegocios.Name;
                }
                retornoLinhaCorteItem.LinhaCorteSemestral = (decimal) item.LinhaCorteSemestral;

                retornoLinhaCorteItem.LinhaCorteTrimestral = (decimal)item.LinhaCorteTrimestral;

                if (item.Moeda != null)
                {
                    retornoLinhaCorteItem.Moeda = item.Moeda.Name;
                }

                if (usuario != null)
                {
                    retornoLinhaCorteItem.CodigoProprietario = usuario.ID.Value.ToString();

                    retornoLinhaCorteItem.NomeProprietario = usuario.Nome;
                }
                //if (LinhaCorteEstado.Count <= 0)
                //{
                //    retornoLinhaCorteItem.EstadosItens.Add(new Pollux.Entities.EstadoItem(){});
                //}
                foreach (var estado in LinhaCorteEstado)
	            {
                    Estado objEstado = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.Organizacao,this.IsOffline).BuscaEstadoPorGuid(estado.Estado.Value);
		            if(objEstado != null && item.ID == estado.LinhaCorteDistribuidor.Value)
                    {
                        if(!String.IsNullOrEmpty(objEstado.Nome))
                        listaEstados.NomeEstado = objEstado.Nome;
                        if (!String.IsNullOrEmpty(objEstado.ChaveIntegracao))
                        listaEstados.ChaveIntegracaoEstado = objEstado.ChaveIntegracao;
                        retornoLinhaCorteItem.EstadosItens.Add(listaEstados);
                        listaEstados = new Pollux.Entities.EstadoItem { };
                    }
	            }
                
                //Depois de pegar as infos adiciona o dictionary em uma lista de dictionaries 
                response.Add(retornoLinhaCorteItem);

                retornoLinhaCorteItem = new Pollux.Entities.LinhaCorte { };

            }

            retorno.Add("LinhasCorteItens", response);

            retorno.Add("Resultado", resultadoConsulta);

            return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
            
        }
        #endregion

        #region Definir Propriedades
        public LinhaCorteDistribuidor DefinirPropriedades(Intelbras.Message.Helper.MSG0078 xml)
        {
            return new LinhaCorteDistribuidor(this.Organizacao, this.IsOffline);
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(LinhaCorteDistribuidor objModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
