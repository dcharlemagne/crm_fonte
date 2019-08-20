using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Xml.Linq;

namespace Intelbras.CRM2013.Application.Plugin.Account
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:

                    if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                    {
                        var entidade = (Entity)context.InputParameters["Target"];
                        Conta conta = entidade.Parse<Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        Classificacao classificacao = new Intelbras.CRM2013.Domain.Servicos.ClassificacaoService(context.OrganizationName, context.IsExecutingOffline, adminService).BuscaClassificacao(conta.Classificacao.Id);

                        //Se estiver fazendo adesão e o proprietário for um systemuser, cria equipe

                        // Executa somente se for participante do programa
                        if (conta.ParticipantePrograma == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                        {
                            //Adesao ao Programa
                            new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).AdesaoAoPrograma(conta);

                            //Integração com CRM 4.0
                            //new Intelbras.CRM2013.Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoCRM4(conta);
                        }

                        #region SellOut


                        if (conta.Classificacao != null && conta.Classificacao.Name != null)
                        {
                            String resultString = null;
                            //Verifica se a conta é distribuidor ou revenda
                            switch (conta.Classificacao.Name)
                            {
                                case Domain.Enum.Conta.Classificacao.Dist_BoxMover:
                                case Domain.Enum.Conta.Classificacao.Dist_VAD:

                                    resultString =
                                        new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(conta, context);

                                    if (!string.IsNullOrEmpty(resultString))
                                    {
                                        throw new ArgumentException("(CRM) " + resultString);
                                    }
                                    break;
                                case Domain.Enum.Conta.Classificacao.Atac_Dist:
                                    if(Domain.Enum.Conta.SubClassificacao.Atac_Distribuidor == conta.Subclassificacao.Name)
                                    {
                                        if(Domain.Enum.Conta.CategoriaConta.Completo == conta.Categoria.Name)
                                        {
                                            resultString = new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(conta, context);
                                            if (!string.IsNullOrEmpty(resultString))
                                            {
                                                throw new ArgumentException("(CRM) " + resultString);
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        #endregion

                        if (!conta.IntegrarNoPlugin || conta.IntegrarNoPlugin == null || conta.IntegrarNoPlugin.ToString().Equals(""))
                            new Intelbras.CRM2013.Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, adminService).IntegracaoBarramento(conta, ref entidade);

                        new Intelbras.CRM2013.Domain.Servicos.EnderecoService(context.OrganizationName, context.IsExecutingOffline, adminService).AtualizaEnderecosAdicionaisDaConta(conta);

                    }

                    break;

                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:
                    if (context.PostEntityImages.Contains("imagem")
                        && context.PostEntityImages["imagem"] is Entity
                        && context.PreEntityImages.Contains("imagem")
                        && context.PreEntityImages["imagem"] is Entity)
                    {

                        var CanalPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var CanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        Classificacao NomeClassificacao = new Intelbras.CRM2013.Domain.Servicos.ClassificacaoService(context.OrganizationName, context.IsExecutingOffline, adminService).BuscaClassificacao(CanalPost.Classificacao.Id);
                        // Valida ALteração do Atributo Forma de apuração dos Benefícios
                        if (CanalPre.ApuracaoBeneficiosCompromissos != CanalPost.ApuracaoBeneficiosCompromissos)
                        {
                            new Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, adminService).ValidaCanalApuracaoDeBeneficios(CanalPre, CanalPost);
                        }

                        #region Historico Categorias Conta
                        if(CanalPost.Categoria != null && CanalPre.Categoria != null)
                        {
                            if (CanalPre.Categoria.Id != CanalPost.Categoria.Id)
                            {
                                Intelbras.CRM2013.Domain.Servicos.RepositoryService RepositoryService = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);

                                var categoriaAtual = RepositoryService.Categoria.Retrieve(CanalPost.Categoria.Id);
                                var categoriaAnterior = RepositoryService.Categoria.Retrieve(CanalPre.Categoria.Id);

                                var historicoCategoria = new HistoricoCategoria(context.OrganizationName, context.IsExecutingOffline, adminService);
                                historicoCategoria.Conta = new SDKore.DomainModel.Lookup(CanalPost.ID.Value, CanalPost.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName(CanalPost));
                                historicoCategoria.CategoriaAnterior = new SDKore.DomainModel.Lookup(categoriaAnterior.ID.Value, categoriaAnterior.Nome, SDKore.Crm.Util.Utility.GetEntityName(categoriaAnterior));
                                historicoCategoria.CategoriaAtual = new SDKore.DomainModel.Lookup(categoriaAtual.ID.Value, categoriaAtual.Nome, SDKore.Crm.Util.Utility.GetEntityName(categoriaAtual));
                                historicoCategoria.DataCriacao = DateTime.Now;

                                RepositoryService.HistoricoCategoria.Create(historicoCategoria);
                            }
                        }
                        #endregion

                        #region SellOut

                        if (CanalPost.Classificacao != null && CanalPost.Classificacao.Name != null)
                        {
                            if (!String.IsNullOrEmpty(CanalPost.CodigoMatriz) && !String.IsNullOrEmpty(CanalPost.CpfCnpj))
                            {

                                //Verifica se a conta é distribuidor ou revenda
                                String resultString = null;
                                switch (CanalPost.Classificacao.Name)
                                {
                                    case Domain.Enum.Conta.Classificacao.Dist_BoxMover:
                                    case Domain.Enum.Conta.Classificacao.Dist_VAD:
                                        resultString =
                                            new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(CanalPost, context);
                                        if (!string.IsNullOrEmpty(resultString))
                                        {
                                            throw new ArgumentException("(CRM) " + resultString);
                                        }
                                        break;
                                    case Domain.Enum.Conta.Classificacao.Atac_Dist:
                                        if(Domain.Enum.Conta.SubClassificacao.Atac_Distribuidor == CanalPost.Subclassificacao.Name)
                                        {
                                            if(Domain.Enum.Conta.CategoriaConta.Completo == CanalPost.Categoria.Name)
                                            {
                                                resultString = new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(CanalPost, context);
                                                if (!string.IsNullOrEmpty(resultString))
                                                {
                                                    throw new ArgumentException("(CRM) " + resultString);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        #endregion
                    }

                    break;

                #endregion

                #region SetStateDynamicEntity

                case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                    if (context.PostEntityImages.Contains("imagem")
                        && context.PostEntityImages["imagem"] is Entity
                        && context.PreEntityImages.Contains("imagem")
                        && context.PreEntityImages["imagem"] is Entity)
                    {
                        var CanalPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var CanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        #region sellout

                        if (CanalPost.Classificacao != null && CanalPost.Classificacao.Name != null)
                        {
                            if (!String.IsNullOrEmpty(CanalPost.CodigoMatriz) && !String.IsNullOrEmpty(CanalPost.CpfCnpj))
                            {
                                String resultString = null;
                                //Verifica se a conta é distribuidor ou Atacado
                                switch (CanalPost.Classificacao.Name)
                                {
                                    case Domain.Enum.Conta.Classificacao.Dist_BoxMover:
                                    case Domain.Enum.Conta.Classificacao.Dist_VAD:
                                        resultString = new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(CanalPost, context);
                                        if (!string.IsNullOrEmpty(resultString))
                                        {
                                            throw new ArgumentException("(CRM) " + resultString);
                                        }
                                        break;
                                    case Domain.Enum.Conta.Classificacao.Atac_Dist:
                                        if(Domain.Enum.Conta.SubClassificacao.Atac_Distribuidor == CanalPost.Subclassificacao.Name)
                                        {
                                            if(Domain.Enum.Conta.CategoriaConta.Completo == CanalPost.Categoria.Name)
                                            {
                                                resultString = new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).PersistirDistribuidor(CanalPost, context);
                                                if (!string.IsNullOrEmpty(resultString))
                                                {
                                                    throw new ArgumentException("(CRM) " + resultString);
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }

                        #endregion

                    }
                    break;
                    #endregion
            }
        }
    }
}