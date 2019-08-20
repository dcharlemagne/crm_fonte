using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.Account
{
    public class ManagerPostEventAssync : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:

                    var entidade = (Entity)context.InputParameters["Target"];
                    Domain.Model.Conta Conta = entidade.Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline);

                    #region Sharepoint

                    if (Conta != null && !string.IsNullOrEmpty(Conta.RazaoSocial))
                        new SharepointServices(context.OrganizationName, context.IsExecutingOffline, adminService).CriarDiretorio<Domain.Model.Conta>(Conta.RazaoSocial, Conta.ID.Value);

                    var enderecoService = new EnderecoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                    enderecoService.AtualizaEnderecosAdicionaisDaConta(Conta);                    

                    #endregion


                    break;
                case Domain.Enum.Plugin.MessageName.Update:
                    var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var CanalPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var CanalPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    
                    if (CanalPost.Classificacao != null && CanalPost.Classificacao.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Classificacao.Revenda"))
                    {
                        if ((!CanalPre.ParticipantePrograma.HasValue || CanalPre.ParticipantePrograma.Value != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim) && CanalPost.ParticipantePrograma.HasValue && CanalPost.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                        {
                            var entityTarget = (Entity)context.InputParameters["Target"];
                            var contaService = new Intelbras.CRM2013.Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, adminService);
                            CanalPost.IntegrarNoPlugin = false;
                            AdesaoRevenda(CanalPost, repositoryService);
                        }
                    }

                    //Adesão ao programa
                    if (CanalPre.ParticipantePrograma != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
                    && CanalPost.ParticipantePrograma == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                    {
                        new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).AdesaoAoPrograma(CanalPost);
                        var integraPontua = (new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).validaIntegraPontuaFielo(CanalPre, CanalPost));
                        var contatos = repositoryService.Contato.ListarAssociadosA(CanalPost.ID.ToString());
                        var contatoService = new Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                        foreach (var contato in contatos)
                        {
                            contato.IntegraIntelbrasPontua = true;
                            contatoService.Persistir(contato);
                        }

                    }

                    //Envia contatos para Fielo na troca de categoria da revenda
                    if (CanalPre.Categoria != CanalPost.Categoria && CanalPost.Categoria.Name != "Registrada")
                    {
                        var contatos = repositoryService.Contato.ListarAssociadosA(CanalPost.ID.ToString());
                        var contatoService = new Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                        foreach (var contato in contatos)
                        {
                            contato.IntegraIntelbrasPontua = true;
                            contatoService.Persistir(contato);
                        }

                    }


                    //Descredenciamento ao programa
                    if (CanalPre.ParticipantePrograma == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
                        && (CanalPost.ParticipantePrograma == (int)Domain.Enum.Conta.ParticipaDoPrograma.Nao
                            || CanalPost.ParticipantePrograma == (int)Domain.Enum.Conta.ParticipaDoPrograma.Descredenciado))
                    {
                        new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).DescredenciamentoAoPrograma(CanalPost);
                    }

                    //Envia Verba para Fielo
                    if (CanalPost.StatusEnvioVMC == (int)Domain.Enum.Conta.StatusEnvioVMC.Enviando)
                    {
                        string retorno = new Domain.Servicos.VerbaVmcService(context.OrganizationName, context.IsExecutingOffline, adminService).EnviaVerbaFielo(CanalPost, CanalPost.CpfCnpj);
                        var contaService = new Intelbras.CRM2013.Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, adminService);

                        Conta CanalUpdate = new Conta(context.OrganizationName, context.IsExecutingOffline);
                        CanalUpdate.ID = CanalPost.ID;

                        if (!retorno.Contains("false"))
                        {
                            CanalUpdate.StatusEnvioVMC = (int)Domain.Enum.Conta.StatusEnvioVMC.Enviado;
                            CanalUpdate.DataUltimoEnvioVMC = DateTime.Now.AddHours(3);
                            CanalUpdate.TemDrireitoVMC = false;
                            CanalUpdate.IntegrarNoPlugin = true;
                            contaService.Persistir(CanalUpdate);
                        }
                        else
                        {
                            CanalUpdate.StatusEnvioVMC = (int)Domain.Enum.Conta.StatusEnvioVMC.ErroAoEnviar;
                            CanalUpdate.TemDrireitoVMC = false;
                            CanalUpdate.IntegrarNoPlugin = true;
                            contaService.Persistir(CanalUpdate);
                        }
                    }
                    break;
            }
        }
        private Domain.Model.Conta AdesaoRevenda(Domain.Model.Conta conta, RepositoryService repository)
        {

            var listaUnidadeNegocioProgramaPci = repository.UnidadeNegocio.ListarPorParticipaProgramaPci(true, "businessunitid", "name");

            var parametroGlobal = new ParametroGlobalService(repository).ObterPor((int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas);

            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas + ") não encontrado!");
            }

            if (parametroGlobal.Categoria == null)
            {
                throw new ArgumentException("(CRM) A Categoria não está peenchida no Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas + ") não encontrado!");
            }

            var listaCategoriasDoCanal = repository.CategoriasCanal.ListarPor(conta);


            foreach (var item in listaUnidadeNegocioProgramaPci)
            {
                var inativar = listaCategoriasDoCanal.Find(x => x.UnidadeNegocios.Id == item.ID.Value
                                                            && x.Status.Value == (int)Domain.Enum.CategoriaCanal.StateCode.Ativado
                                                            && (x.Categoria.Id != parametroGlobal.Categoria.Id || x.Classificacao.Id != conta.Classificacao.Id || x.SubClassificacao.Id != conta.Subclassificacao.Id));

                if (inativar != null)
                {
                    repository.CategoriasCanal.AtualizarStatus(inativar.ID.Value, (int)Domain.Enum.CategoriaCanal.StateCode.Desativado, 2);
                }

                var existente = listaCategoriasDoCanal.Find(x => x.UnidadeNegocios.Id == item.ID.Value && x.Categoria.Id == parametroGlobal.Categoria.Id && x.Classificacao.Id == conta.Classificacao.Id && x.SubClassificacao.Id == conta.Subclassificacao.Id);

                if (existente == null)
                {
                    repository.CategoriasCanal.Create(new CategoriasCanal(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider)
                    {
                        Canal = new SDKore.DomainModel.Lookup(conta.ID.Value, conta.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName(conta)),
                        Categoria = parametroGlobal.Categoria,
                        Classificacao = conta.Classificacao,
                        Nome = item.Nome,
                        SubClassificacao = conta.Subclassificacao,
                        UnidadeNegocios = new SDKore.DomainModel.Lookup(item.ID.Value, item.Nome, SDKore.Crm.Util.Utility.GetEntityName(item))
                    });
                }
                else
                {
                    if (existente.Status == (int)Domain.Enum.CategoriaCanal.StateCode.Desativado)
                    {
                        repository.CategoriasCanal.AtualizarStatus(existente.ID.Value, (int)Domain.Enum.CategoriaCanal.StateCode.Ativado, 1);
                    }
                }
            }

            repository.Conta.Update(new Conta(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider)
            {
                ID = conta.ID,
                ParticipantePrograma = (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
            });

            return conta;
        }
    }
}
