using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_itbc_prodestabelecimento
{
    public class ManagerPreEvent : IPlugin
    {
        public Domain.Servicos.ProdutoEstabelecimentoService ProEstabServ { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ProEstabServ = new Domain.Servicos.ProdutoEstabelecimentoService(context.OrganizationName, context.IsExecutingOffline);

            try
            {
                Entity entidade = new Entity();
                Domain.Model.ProdutoEstabelecimento prodEstabAtual = new Domain.Model.ProdutoEstabelecimento(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:

                            #region Popula o objeto prodEstabAtual com o contexto

                            entidade = (Entity)context.InputParameters["Target"];
                            prodEstabAtual = entidade.Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline);

                            #endregion

                            JaExisteProduto(prodEstabAtual);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                            {
                                #region Popula o objeto ProdEstabOriginal com a pre-image

                                entidade = (Entity)context.PreEntityImages["imagem"];
                                Domain.Model.ProdutoEstabelecimento ProdEstabOriginal = entidade.Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto ProdEstabAlterado com o contexto alterado

                                Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                                Domain.Model.ProdutoEstabelecimento ProdEstabAlterado = entidadeAlterada.Parse<Domain.Model.ProdutoEstabelecimento>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto prodEstabAtual mesclando o que foi alterado com os dados originais, para efeito de validação

                                prodEstabAtual.Produto = ProdEstabAlterado.Produto != null ? ProdEstabAlterado.Produto : ProdEstabOriginal.Produto;
                               
                                prodEstabAtual.ID = ProdEstabAlterado.ID != null ? ProdEstabAlterado.ID : ProdEstabOriginal.ID;


                                if (ProdEstabAlterado.Estabelecimento == null)
                                    return;

                                if(prodEstabAtual.Estabelecimento==null)
                                    if (ProdEstabOriginal.Estabelecimento.Id != ProdEstabAlterado.Estabelecimento.Id)
                                      return;

                                #endregion


                                if (prodEstabAtual.Estabelecimento.Id != ProdEstabAlterado.Estabelecimento.Id)
                                    return;

                                JaExisteProduto(prodEstabAtual);
                            }

                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                var msg = Util.Utilitario.TratarErro(ex);
                throw new InvalidPluginExecutionException(msg);
            }
        }

        private bool ValidacoesFormulario(Domain.Model.ProdutoEstabelecimento produtoEstabelecimento)
        {

            return true;

        }

        private void JaExisteProduto(Domain.Model.ProdutoEstabelecimento produtoEstabelecimento)
        {
            if (ProEstabServ.VerificarExistenciaProduto(produtoEstabelecimento))
                throw new ArgumentException("(CRM) O sistema não permite mais de um Produto do Estabelecimento.");
        }
    }
}
