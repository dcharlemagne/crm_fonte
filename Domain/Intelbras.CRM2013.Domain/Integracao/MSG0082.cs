using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0082 : Base, IBase<Message.Helper.MSG0082, Domain.Model.ItemListaPreco>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor

        public MSG0082(string org, bool isOffline)
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
            //Trace.Add("ListaPreco " + numeroListaPreco + " XML: {0}", ListaPreco);
            usuarioIntegracao = usuario;
            var objeto = this.DefinirPropriedades(this.CarregarMensagem<Pollux.MSG0082>(mensagem));

            if (!resultadoPersistencia.Sucesso)
            {
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0082R1>(numeroMensagem, retorno);
            }

            //objeto = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).Persistir(objeto);
            if (objeto == null)
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Erro ao persistir ItemListaPreco.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0082R1>(numeroMensagem, retorno);
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                retorno.Add("CodigoItemListaPreco", "00000000-0000-0000-0000-000000000000");
                retorno.Add("Resultado", resultadoPersistencia);
            }
            return CriarMensagemRetorno<Pollux.MSG0082R1>(numeroMensagem, retorno);
        }

        #endregion

        #region Definir Propriedades

        public ItemListaPreco DefinirPropriedades(Intelbras.Message.Helper.MSG0082 xml)
        {
            var crm = new ItemListaPreco(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml
            
            if (!String.IsNullOrEmpty(xml.CodigoItemListaPreco))
            {
                crm.ID = new Guid(xml.CodigoItemListaPreco);
            }

            if (!String.IsNullOrEmpty(xml.Produto))
            {
                Product produto = new Product(this.Organizacao, this.IsOffline);
                produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.Produto);
                if (produto != null)
                    crm.ProdutoID = new Lookup(produto.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Produto não encontrado.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Produto não enviado.";
                return crm;
            }


            if (!String.IsNullOrEmpty(xml.ListaPreco))
            {
                Model.ListaPreco listaPreco = new Model.ListaPreco(this.Organizacao, this.IsOffline);
                listaPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(xml.ListaPreco);

                if (listaPreco != null && listaPreco.ID.HasValue)
                    crm.ListaPrecos = new Lookup(listaPreco.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "ListaPreco não encontrado.";
                    return crm;
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "ListaPreco não enviada.";
                return crm;
            }

            crm.Valor = xml.Valor;

            crm.Porcentual = xml.Porcentagem;

            if (!String.IsNullOrEmpty(xml.ListaDesconto))
            {
                ListaDesconto listaDesconto = new ListaDesconto(this.Organizacao, this.IsOffline);
                listaDesconto = new Intelbras.CRM2013.Domain.Servicos.ListaDescontoService(this.Organizacao, this.IsOffline).BuscaListaDesconto(xml.ListaDesconto);

                if (listaDesconto != null)
                    crm.ListaDesconto = new Lookup(listaDesconto.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Lista Desconto não encontrada.";
                    return crm;
                }
            }
            else
            {
                crm.AddNullProperty("ListaDesconto");
            }

            crm.MetodoPrecificacao = xml.MetodoPrecificacao;

            crm.OpcaoVendaParcial = xml.OpcaoVendaParcial;

            crm.ValorArredondamento = xml.ValorArredondamento;

            crm.OpcaoArredondamento = xml.OpcaoArredondamento;

            crm.PoliticaArredondamento = xml.PoliticaArredondamento;

            if (!String.IsNullOrEmpty(xml.Moeda))
            {
                Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);
                moeda = new Intelbras.CRM2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(xml.Moeda);

                if (moeda != null && moeda.ID.HasValue)
                    crm.Moeda = new Lookup(moeda.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Moeda não encontrada!";
                    return crm;
                }
            }


            if (!String.IsNullOrEmpty(xml.UnidadeMedida))
            {
                Unidade unidade = new Unidade(this.Organizacao, this.IsOffline);
                unidade = new Intelbras.CRM2013.Domain.Servicos.UnidadeService(this.Organizacao, this.IsOffline).BuscaUnidadePorNome(xml.UnidadeMedida);
                if (unidade != null)
                    crm.Unidade = new Lookup(unidade.ID.Value, "");
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "UnidadeMedida não encontrada.";
                    return crm;
                }

            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "UnidadeMedida não enviada.";
                return crm;
            }
            //Entidade não tem ownerid
            //crm.Proprietario = new SDKore.DomainModel.Lookup((Guid)usuarioIntegracao.ID, "systemuser");
            //crm.IntegradoEm = DateTime.Now;
            //crm.IntegradoPor = usuarioIntegracao.NomeCompleto;
            //crm.UsuarioIntegracao = xml.LoginUsuario;


            #endregion

            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(ItemListaPreco objModel)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
