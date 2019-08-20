using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0102 : Base, IBase<Intelbras.Message.Helper.MSG0102, Domain.Model.ProdutoEstabelecimento>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;

        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        private Int32 codigoEstabelecimento;
        #endregion

        #region Construtor
        public MSG0102(string org, bool isOffline)
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
            var xml = this.CarregarMensagem<Pollux.MSG0102>(mensagem);
            
            //Na dll está errado, o parametro precisa ser nullable pra poder retornar todos resultados, por enquanto deixei assim para testar
            //if(xml.CodigoEstabelecimento.HasValue())
            if(xml.CodigoEstabelecimento != 0)
            {//Retorna registros com base no estabelecimento

                //Pegamos o estabelecimento com base no codigo do erp dele
                Estabelecimento ObjEstabelecimento = new Intelbras.CRM2013.Domain.Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimentoPorCodigo(xml.CodigoEstabelecimento);
                if (ObjEstabelecimento == null)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Estabelecimento não encontrado.";
                    retorno.Add("Resultado", resultadoPersistencia);
                    return CriarMensagemRetorno<Pollux.MSG0102R1>(numeroMensagem, retorno);
                }

                //CodigoEstabelecimento esta como int na dll,deveria ser Guid, mais um prob na dll, tive que fazer uma gambiarra pra poder testar ele
                List<ProdutoEstabelecimento> LstProdutosEstab = new Intelbras.CRM2013.Domain.Servicos.ProdutoEstabelecimentoService(this.Organizacao, this.IsOffline).ListarPorEstabelecimento((Guid)ObjEstabelecimento.ID);

                if (LstProdutosEstab != null && LstProdutosEstab.Count > 0)
                {
                    //para nao precisar refazer a service
                    codigoEstabelecimento = xml.CodigoEstabelecimento;
                    List<Pollux.Entities.ProdutoItem> ProdutosEst = this.ConverteLista(LstProdutosEstab);
                    resultadoPersistencia.Sucesso = true;
                    retorno.Add("ProdutosItens",ProdutosEst);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                }
            }
            else
            {//Retorna todos registros
                List<ProdutoEstabelecimento> LstProdutosEstab = new Intelbras.CRM2013.Domain.Servicos.ProdutoEstabelecimentoService(this.Organizacao, this.IsOffline).ListarTodos();

                if (LstProdutosEstab != null)
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = ""; resultadoPersistencia.CodigoErro = "";
                    retorno.Add("ProdutosItens", LstProdutosEstab);
                }
                else
                {
                    resultadoPersistencia.Sucesso = true;
                    resultadoPersistencia.Mensagem = "Registros não encontrados.";
                }

            }
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0102R1>(numeroMensagem, retorno);
        }

        private List<Pollux.Entities.ProdutoItem> ConverteLista(List<ProdutoEstabelecimento> LstRegistrosCrm)
        {
            List<Pollux.Entities.ProdutoItem> lstPollux = new List<Pollux.Entities.ProdutoItem>();

            foreach (ProdutoEstabelecimento item in LstRegistrosCrm)
            {
                Pollux.Entities.ProdutoItem Objeto = new Pollux.Entities.ProdutoItem();

                Objeto.CodigoEstabelecimento = codigoEstabelecimento;
                Objeto.NomeEstabelecimento = item.Estabelecimento.Name;

                Product objProduto =  new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).ObterPor(item.Produto.Id);
                
                Objeto.CodigoProduto = objProduto.Codigo;
                Objeto.NomeProduto = objProduto.Nome;

                lstPollux.Add(Objeto);
            }

            return lstPollux;
        }

        #endregion

        #region Definir Propriedades

        public ProdutoEstabelecimento DefinirPropriedades(Intelbras.Message.Helper.MSG0102 xml)
        {
            var crm = new ProdutoEstabelecimento(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

           
            #endregion

            return crm;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(ProdutoEstabelecimento objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
