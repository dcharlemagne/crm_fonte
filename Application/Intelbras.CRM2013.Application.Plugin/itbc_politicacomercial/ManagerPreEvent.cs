using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Application.Plugin.itbc_politicacomercial
{
    public class ManagerPreEvent : IPlugin
    {
        public Domain.Servicos.PoliticaComercialService PoliticaComercialServ { get; set; }

        public void Execute(IServiceProvider serviceProvider)
        {

            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            PoliticaComercialServ = new Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline);

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            try
            {
                Entity entidade = new Entity();
                Domain.Model.PoliticaComercial politicaComercialAtual = new Domain.Model.PoliticaComercial(context.OrganizationName, context.IsExecutingOffline);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                    {
                        case Domain.Enum.Plugin.MessageName.Create:

                            #region Popula o objeto politicaComercialAtual com o contexto

                            entidade = (Entity)context.InputParameters["Target"];
                            politicaComercialAtual = entidade.Parse<Domain.Model.PoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                            #endregion
                            if (politicaComercialAtual.Status.Value == (int)Domain.Enum.PoliticaComercial.Status.Ativo) 
                                JaExistePoliticaComercial(politicaComercialAtual);

                            break;

                        case Domain.Enum.Plugin.MessageName.Update:

                            if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                            {
                                #region Popula o objeto politicaComercial com a pre-image
                                
                                entidade = (Entity)context.PreEntityImages["imagem"];
                                Domain.Model.PoliticaComercial politicaOriginal = entidade.Parse<Domain.Model.PoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto politicaComercial com o contexto alterado

                                Entity entidadeAlterada = (Entity)context.InputParameters["Target"];
                                Domain.Model.PoliticaComercial politicaAlterada = entidadeAlterada.Parse<Domain.Model.PoliticaComercial>(context.OrganizationName, context.IsExecutingOffline);

                                #endregion

                                #region Popula objeto politicaComercial mesclando o que foi alterado com os dados originais, para efeito de validação

                                politicaComercialAtual.Categoria = politicaAlterada.Categoria != null ? politicaAlterada.Categoria : politicaOriginal.Categoria;
                                politicaComercialAtual.Classificacao = politicaAlterada.Classificacao != null ? politicaAlterada.Classificacao : politicaOriginal.Classificacao;
                                politicaComercialAtual.Estabelecimento = politicaAlterada.Estabelecimento != null ? politicaAlterada.Estabelecimento : politicaOriginal.Estabelecimento;
                                politicaComercialAtual.ID = politicaAlterada.ID.HasValue ? politicaAlterada.ID : politicaOriginal.ID;
                                politicaComercialAtual.AplicarPoliticaPara = politicaAlterada.AplicarPoliticaPara != null ? politicaAlterada.AplicarPoliticaPara : politicaOriginal.AplicarPoliticaPara;
                                politicaComercialAtual.UnidadeNegocio = politicaAlterada.UnidadeNegocio != null ? politicaAlterada.UnidadeNegocio : politicaOriginal.UnidadeNegocio;
                                politicaComercialAtual.TipoDePolitica = politicaAlterada.TipoDePolitica != null ? politicaAlterada.TipoDePolitica : politicaOriginal.TipoDePolitica;
                                politicaComercialAtual.DataInicio = politicaAlterada.DataInicio.HasValue ? politicaAlterada.DataInicio : politicaOriginal.DataInicio;
                                politicaComercialAtual.DataFim = politicaAlterada.DataFim.HasValue ? politicaAlterada.DataFim : politicaOriginal.DataFim;

                                politicaComercialAtual.Status = politicaAlterada.Status != null ? politicaAlterada.Status : politicaOriginal.Status;
                               
                                #endregion
                                if (politicaComercialAtual.Status.Value == (int)Domain.Enum.PoliticaComercial.Status.Ativo)
                                {
                                    List<Guid> lstPoliticaEstado = new List<Guid>(), lstPoliticaCanais = new List<Guid>();
                                    //Se ele tiver alterado o status ou a data de inicio ou de fim verifica as services de estado e canais relacionados
                                    if ((politicaAlterada.Status != null && politicaOriginal.Status != politicaAlterada.Status)
                                        || politicaAlterada.DataInicio != null && politicaOriginal.DataInicio != politicaAlterada.DataInicio
                                        || politicaAlterada.DataFim != null && politicaOriginal.DataFim != politicaAlterada.DataFim)
                                    {

                                        lstPoliticaEstado = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).ListarEstadosDaPoliticaComercial(politicaComercialAtual.ID.Value);
                                        lstPoliticaCanais = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).ListarCanaisDaPoliticaComercial(politicaComercialAtual.ID.Value);

                                        bool respostaEstados = false,respostaCanais = false;
                                        if (lstPoliticaEstado.Count > 0)
                                            respostaEstados = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).VerificarDuplicidadePoliticaRegistros(politicaComercialAtual, lstPoliticaEstado, "estado", false);
                                        if (lstPoliticaCanais.Count > 0)
                                            respostaCanais = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).VerificarDuplicidadePoliticaRegistros(politicaComercialAtual, lstPoliticaCanais, "conta", false);

                                        if (respostaEstados == true)
                                            throw new InvalidPluginExecutionException("(CRM)Não é possível realizar a operação: O estado informado já está vinculado à outra política comercial com o mesmo tipo, aplicação, estabelecimento, unidade de negócio e data de vigência");

                                        if (respostaCanais == true)
                                            throw new InvalidPluginExecutionException("(CRM)Não é possível realizar a operação: O canal informado já está vinculado à outra política comercial com o mesmo tipo, aplicação, estabelecimento, unidade de negócio e data de vigência");
                                        
                                        if (lstPoliticaEstado.Count == 0 && lstPoliticaCanais.Count == 0)
                                            JaExistePoliticaComercial(politicaComercialAtual);
                                    }
                                }
                            }

                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Handler(ex);
                throw;
            }
        }

        private bool ValidacoesFormulario(Domain.Model.PoliticaComercial politicaComercial)
        {

            switch (politicaComercial.PoliticaEspecifica) {
                case (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Nao:
                    if (politicaComercial.UnidadeNegocio == null || politicaComercial.Classificacao == null || politicaComercial.Categoria == null)
                    {
                        return false;
                    }
                    break;
                case (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Sim:
                    if (politicaComercial.Estabelecimento == null )
                //|| politicaComercial.Canal == null) retirei isso por causa do model do canal que virou n:M, caso isso for uma RN depois terá que implementar novamente
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }

            return true;

        }

        private void JaExistePoliticaComercial(Domain.Model.PoliticaComercial politicaComercial)
        {
            if (PoliticaComercialServ.VerificarExistenciaPoliticaComercial(politicaComercial))
                throw new ArgumentException("(CRM) Já existe outra Política Comercial cadastrada para essa mesma configuração.");
        }
    }
}
