using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.Plugin.itbc_produtopoliticacomercial
{
    public class ManagerPreEvent : IPlugin
    {
        public Domain.Servicos.PoliticaComercialService PoliticaComercialServ { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            PoliticaComercialServ = new Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline);

            try
            {
                Entity entidade = new Entity();
                Domain.Model.ProdutoPoliticaComercial produtoPCAtual = new Domain.Model.ProdutoPoliticaComercial(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:

                            #region Popula o objeto politicaComercialAtual com o contexto

                            entidade = (Entity)context.InputParameters["Target"];
                            produtoPCAtual = entidade.Parse<Domain.Model.ProdutoPoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                            #endregion

                            ValidacoesFormulario(produtoPCAtual);
                            JaExisteProdutoIntervalo(produtoPCAtual);
                            

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                            {
                                #region Popula o objeto politicaComercial com a pre-image

                                entidade = (Entity)context.PreEntityImages["imagem"];
                                Domain.Model.ProdutoPoliticaComercial ProdutoPCOriginal = entidade.Parse<Domain.Model.ProdutoPoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto politicaComercial com o contexto alterado

                                Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                                Domain.Model.ProdutoPoliticaComercial produtoPCAlterada = entidadeAlterada.Parse<Domain.Model.ProdutoPoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto politicaComercial mesclando o que foi alterado com os dados originais, para efeito de validação

                                produtoPCAtual.PoliticaComercial = produtoPCAlterada.PoliticaComercial != null ? produtoPCAlterada.PoliticaComercial : ProdutoPCOriginal.PoliticaComercial;
                                produtoPCAtual.Produto = produtoPCAlterada.Produto != null ? produtoPCAlterada.Produto : ProdutoPCOriginal.Produto;
                                produtoPCAtual.QtdInicial = produtoPCAlterada.QtdInicial != null ? produtoPCAlterada.QtdInicial : ProdutoPCOriginal.QtdInicial;
                                produtoPCAtual.QtdFinal = produtoPCAlterada.QtdFinal != null ? produtoPCAlterada.QtdFinal : ProdutoPCOriginal.QtdFinal;
                                produtoPCAtual.ID = produtoPCAlterada.ID.HasValue ? produtoPCAlterada.ID : ProdutoPCOriginal.ID;
                                produtoPCAtual.DataInicioVigencia = produtoPCAlterada.DataInicioVigencia != null ? produtoPCAlterada.DataInicioVigencia : ProdutoPCOriginal.DataInicioVigencia;
                                produtoPCAtual.DataFimVigencia = produtoPCAlterada.DataFimVigencia != null ? produtoPCAlterada.DataFimVigencia : ProdutoPCOriginal.DataFimVigencia;
                                

                                #endregion
                                ValidacoesFormulario(produtoPCAtual);
                                JaExisteProdutoIntervalo(produtoPCAtual);
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

        private bool ValidacoesFormulario(Domain.Model.ProdutoPoliticaComercial produtoPoliticaComercial)
        {
                       
            if(produtoPoliticaComercial.DataInicioVigencia.HasValue == false || produtoPoliticaComercial.DataFimVigencia.HasValue == false)
                 throw new ArgumentException("(CRM) Data de Inicio e Data de Fim de Vigencia devem ser preenchidas!");

            if(DateTime.Compare(produtoPoliticaComercial.DataInicioVigencia.Value, produtoPoliticaComercial.DataFimVigencia.Value) != -1)
                throw new ArgumentException("(CRM) Data de Inicio deve ser inferior a Data de Fim de Vigencia preenchidas.");
            
            if (produtoPoliticaComercial.QtdInicial <= 0)
                throw new ArgumentException("(CRM) Quantidade Inicial deve ser maior que 0");

            if (produtoPoliticaComercial.QtdInicial >= produtoPoliticaComercial.QtdFinal)
                throw new ArgumentException("(CRM) Quantidade Final deve ser maior que Quantidade Inicial.");

            return true;

        }

        private void JaExisteProdutoIntervalo(Domain.Model.ProdutoPoliticaComercial produtoPoliticaComercial)
        {
            if (PoliticaComercialServ.VerificarIntervaloQtd(produtoPoliticaComercial))
                throw new ArgumentException("(CRM) Não é possível realizar a operação: Já existe para este produto um fator registrado com a mesma validade e quantidade. Verificar os registros existentes");
        }
    }
}