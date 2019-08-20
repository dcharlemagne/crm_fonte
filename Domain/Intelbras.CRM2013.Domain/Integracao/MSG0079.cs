using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0079 : Base, IBase<Message.Helper.MSG0079, Domain.Model.LinhaCorteRevenda>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();

        Pollux.Entities.Resultado resultadoConsulta = new Pollux.Entities.Resultado() { Sucesso = true };

        Pollux.Entities.LinhaCorteRevenda retornoLinhaCorteItem = new Pollux.Entities.LinhaCorteRevenda { };

        List<Pollux.Entities.LinhaCorteRevenda> response = new List<Pollux.Entities.LinhaCorteRevenda>();

        #endregion

        #region Construtor
        public MSG0079(string org, bool isOffline)
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

            var xml = this.CarregarMensagem<Pollux.MSG0079>(mensagem);
            Conta objetoConta = null;
            UnidadeNegocio objetoUnidadeNeg = null;
            Categoria objetoCategoria = null;
            List<CategoriasCanal> lstCategCanal = null;

            if (String.IsNullOrEmpty(xml.CodigoConta) && String.IsNullOrEmpty(xml.CodigoUnidadeNegocio) && String.IsNullOrEmpty(xml.CodigoCategoria))
            {
                resultadoConsulta.Sucesso = false;
                resultadoConsulta.Mensagem = "Pelo menos 1 parâmetro deve ser enviado.";
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
            }

            if (!String.IsNullOrEmpty(xml.CodigoConta))
            {
                objetoConta = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.CodigoConta));
                if (objetoConta == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "[Conta] não encontrada.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
                }

            }

            if (!String.IsNullOrEmpty(xml.CodigoUnidadeNegocio))
            {
                objetoUnidadeNeg = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocioPorChaveIntegracao(xml.CodigoUnidadeNegocio);
                if (objetoUnidadeNeg == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "[Unidade Negócio] não encontrada.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0078R1>(numeroMensagem, retorno);
                }
            }

            if (!String.IsNullOrEmpty(xml.CodigoCategoria))
            {
                objetoCategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaService(this.Organizacao, this.IsOffline).ObterPor(new Guid(xml.CodigoCategoria));
                if (objetoCategoria == null)
                {
                    resultadoConsulta.Sucesso = false;
                    resultadoConsulta.Mensagem = "[Categoria] não encontrada.";
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
                }
            }

            List<CategoriasCanal> lstObjetoCategoria = new List<CategoriasCanal>();
            List<Guid> lstUnd = new List<Guid>();
            if (objetoConta != null)
            {
                if (objetoUnidadeNeg != null)
                    lstObjetoCategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.Organizacao, this.IsOffline).ListarPor((Guid)objetoConta.ID, (Guid)objetoUnidadeNeg.ID);
                else
                    lstObjetoCategoria = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.Organizacao, this.IsOffline).ListarPor((Guid)objetoConta.ID, null);

                foreach (CategoriasCanal item in lstObjetoCategoria ?? Enumerable.Empty<CategoriasCanal>())
                {
                    if (!lstUnd.Contains(item.UnidadeNegocios.Id))
                    {
                        lstUnd.Add(item.UnidadeNegocios.Id);
                    }
                }
                if (lstUnd.Count == 0)
                {
                    resultadoConsulta.Sucesso = true;
                    retorno.Add("Resultado", resultadoConsulta);
                    return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
                }
            }
            else if (objetoUnidadeNeg != null)
            {
                lstUnd.Add(objetoUnidadeNeg.ID.Value);
            }
            else if (objetoCategoria != null)
            {
                lstUnd = null;
            }
            List<LinhaCorteRevenda> objLinhaCorteRevenda = new List<LinhaCorteRevenda>();
            if (objetoCategoria != null)
            {
                objLinhaCorteRevenda = new Intelbras.CRM2013.Domain.Servicos.LinhaCorteService(this.Organizacao, this.IsOffline).ListarLinhadeCorteRevenda(lstUnd, objetoCategoria);
            }
            else if (lstObjetoCategoria.Count > 0)
            {
                //Passado Canal, mas nao categoria. As categorias foram obtidas atraves de CategoriasCanal
                foreach (var itemCateg in lstObjetoCategoria)
                {
                    Categoria categ = new Servicos.CategoriaService(this.Organizacao, this.IsOffline).ObterPor(itemCateg.Categoria.Id);
                    List<LinhaCorteRevenda> lstPorItem = new Intelbras.CRM2013.Domain.Servicos.LinhaCorteService(this.Organizacao, this.IsOffline).ListarLinhadeCorteRevenda(lstUnd, categ);
                    if (lstPorItem.Count > 0)
                    {
                        foreach (var linhaCorte in lstPorItem)
                        {
                            objLinhaCorteRevenda.Add(linhaCorte);
                        }
                    }
                }
            }
            else
            {
                //Não foi passsado categoria, apenas CAnal, e nao foi encontrada CategoriasCanal
                objLinhaCorteRevenda = new Intelbras.CRM2013.Domain.Servicos.LinhaCorteService(this.Organizacao, this.IsOffline).ListarLinhadeCorteRevenda(lstUnd, null);
            }


            foreach (var item in objLinhaCorteRevenda)
            {
                if(!response.Exists(x => x.CodigoLinhaCorte == item.ID.Value.ToString()))
                response.Add(this.DefinirPropriedades(item));
            }

            if (!resultadoConsulta.Sucesso)
            {
                retorno.Add("Resultado", resultadoConsulta);
                return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
            }

            resultadoConsulta.Sucesso = true;
            if (response != null && response.Count > 0)
            {
                resultadoConsulta.Mensagem = "Integração ocorrida com sucesso.";
                retorno.Add("LinhasCorteItens", response);
            }
            else
            {
                resultadoConsulta.Mensagem = "Linha de Corte Revenda não encontrado no Crm.";
            }
            retorno.Add("Resultado", resultadoConsulta);


            return CriarMensagemRetorno<Pollux.MSG0079R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public LinhaCorteRevenda DefinirPropriedades(Intelbras.Message.Helper.MSG0079 xml)
        {
            var crm = new LinhaCorteRevenda(this.Organizacao, this.IsOffline);

            return crm;
        }

        public Pollux.Entities.LinhaCorteRevenda DefinirPropriedades(LinhaCorteRevenda objCrm)
        {
            Pollux.Entities.LinhaCorteRevenda xmlRetorno = new Pollux.Entities.LinhaCorteRevenda();

            xmlRetorno.CodigoLinhaCorte = objCrm.ID.Value.ToString();

            if (!String.IsNullOrEmpty(objCrm.Nome))
                xmlRetorno.Nome = objCrm.Nome;

            if (objCrm.Categoria != null)
            {
                xmlRetorno.CodigoCategoria = objCrm.Categoria.Id.ToString();
                xmlRetorno.NomeCategoria = objCrm.Categoria.Name;
            }

            if (objCrm.UnidadeNegocio != null)
            {
                UnidadeNegocio unidadeNegocio = new Intelbras.CRM2013.Domain.Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(objCrm.UnidadeNegocio.Id);
                if (unidadeNegocio != null)
                {
                    xmlRetorno.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                    xmlRetorno.NomeUnidadeNegocio = unidadeNegocio.Nome;
                }
            }

            xmlRetorno.LinhaCorteSemestral = (decimal)objCrm.LinhaCorteSemestral;
            xmlRetorno.LinhaCorteTrimestral = (decimal)objCrm.LinhaCorteTrimestral;
            xmlRetorno.Moeda = "Real";

            Usuario proprietario = new Servicos.UsuarioService(this.Organizacao, this.IsOffline).BuscarProprietario("itbc_linhadecorterevenda", "itbc_linhadecorterevendaid", objCrm.Id);
            if (proprietario != null)
            {
                xmlRetorno.CodigoProprietario = proprietario.Id.ToString();
                xmlRetorno.NomeProprietario = proprietario.NomeCompleto;
            }

            return xmlRetorno;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(LinhaCorteRevenda objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
