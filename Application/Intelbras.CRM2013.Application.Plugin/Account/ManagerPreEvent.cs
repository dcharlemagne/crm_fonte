using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using System;

namespace Intelbras.CRM2013.Application.Plugin.Account
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var contaService = new Intelbras.CRM2013.Domain.Servicos.ContaService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create
                case Domain.Enum.Plugin.MessageName.Create:

                    var e = (Entity)context.InputParameters["Target"];
                    var conta = e.Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    if (e.Contains("itbc_cpfoucnpj") && conta.CpfCnpj != null)
                    {
                        if (contaService.BuscaContaPorCpfCnpj(conta.CpfCnpj, conta.TipoConstituicao.Value) != null)
                        {
                            throw new ArgumentException("(CRM) Ja Existe uma Conta com esse mesmo CPF/CNPJ.");
                        }
                        if (conta.TipoConstituicao != (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro)
                        {
                            e.Attributes["new_sem_masc_cnpj_cpf"] = conta.CpfCnpj.GetOnlyNumbers();
                            e.Attributes["itbc_cpfoucnpj"] = conta.CpfCnpj.InputMask();
                        }
                    }
                    else
                        throw new ArgumentException("(CRM) Campo CPF ou CNPJ não esta preenchido.");

                    PreenchimentoDeCampos(ref context, adminService);

                    if (conta.AssistenciaTecnica == null)
                    {
                        e.Attributes["itbc_isastec"] = false;
                    }

                    //Atualiza endereço Padrão
                    if (conta.EnderecoPadrao == "e")
                    {
                        e.Attributes["new_altera_endereco_padrao"] = "n";
                    }

                    break;
                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:
                    PreenchimentoDeCampos(ref context, adminService);
                    var entityTarget = (Entity)context.InputParameters["Target"];
                    var entityPre = context.PreEntityImages["imagem"];
                    var entityMerge = context.GetContextEntityMerge("imagem");
                    var canalMerge = ((Entity)entityMerge).Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var canalPre = new Domain.Servicos.CanalServices(context.OrganizationName, context.IsExecutingOffline, adminService).ObterCanalPorId(canalMerge.ID.Value);
                    var repositoryService = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, adminService);
                    var contaUp = entityTarget.Parse<Domain.Model.Conta>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    if (entityTarget.Contains("itbc_cpfoucnpj") && (canalPre.CpfCnpj.GetOnlyNumbers() != canalMerge.CpfCnpj.GetOnlyNumbers() || canalMerge.TipoConstituicao == (int)Domain.Enum.Conta.TipoConstituicao.Estrangeiro))
                    {
                        if (contaService.BuscaOutraContaPorCpfCnpj(contaUp.CpfCnpj, contaUp.ID.Value, canalMerge.TipoConstituicao.Value) != null)
                        {
                            throw new ArgumentException("(CRM) Ja Existe uma Conta com esse mesmo CPF/CNPJ.");
                        }
                        entityTarget.Attributes["new_sem_masc_cnpj_cpf"] = contaUp.CpfCnpj.GetOnlyNumbers();
                    }

                    foreach (var item in entityTarget.Attributes)
                    {
                        entityMerge.Attributes[item.Key] = item.Value;
                    }

                    if (canalMerge.AssistenciaTecnica == null)
                    {
                        entityTarget.Attributes["itbc_isastec"] = false;
                    }

                    //Atualiza endereço Padrão
                    if (canalMerge.EnderecoPadrao == "e")
                    {
                        new Intelbras.CRM2013.Domain.Servicos.EnderecoService(context.OrganizationName, context.IsExecutingOffline, adminService).AtualizaEnderecosAdicionaisDaConta(canalMerge);
                        entityTarget.Attributes["new_altera_endereco_padrao"] = "n";
                    }

                    //Atualizando informações de endereço
                    entityTarget.Attributes["address1_line1"] = canalMerge.Endereco1Rua;
                    entityTarget.Attributes["address1_city"] = canalMerge.Endereco1Cidade;
                    entityTarget.Attributes["address2_city"] = canalMerge.Endereco1Cidade;
                    entityTarget.Attributes["address1_county"] = canalMerge.Endereco1Bairro;
                    entityTarget.Attributes["address2_county"] = canalMerge.Endereco2Bairro;

                    entityTarget.Attributes["address1_stateorprovince"] = canalMerge.Endereco1Estado;
                    entityTarget.Attributes["new_sem_masc_cnpj_cpf"] = canalMerge.CpfCnpj.GetOnlyNumbers();

                    if (canalMerge.Classificacao != null && canalMerge.Classificacao.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Classificacao.Revenda"))
                    {
                        if ((!canalPre.ParticipantePrograma.HasValue || canalPre.ParticipantePrograma.Value != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim) && canalMerge.ParticipantePrograma.HasValue && canalMerge.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                        {
                            canalMerge.IntegrarNoPlugin = true;
                        }
                    }

                    //Valida Credenciamento
                    if (canalMerge.Classificacao != null && canalMerge.Classificacao.Id != SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Classificacao.Revenda"))
                    {
                        if (canalPre.ParticipantePrograma.HasValue
                        && canalMerge.ParticipantePrograma.HasValue
                        && canalPre.ParticipantePrograma.Value != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
                        && canalMerge.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                        {
                            new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).validaAdesaoaoPrograma(canalMerge);
                            entityTarget.Attributes["itbc_integraintelbraspontua"] = true;
                            canalMerge.IntegraIntelbrasPontua = true;
                        }
                    }

                    //Envia conta para Fielo
                    var IntegraPontua = (new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).validaIntegraPontuaFielo(canalPre, canalMerge));
                    entityTarget.Attributes["itbc_integraintelbraspontua"] = IntegraPontua;
                    canalMerge.IntegraIntelbrasPontua = IntegraPontua;

                    // Valida Descredenciamento ao programa
                    if (canalPre.ParticipantePrograma.HasValue
                        && canalMerge.ParticipantePrograma.HasValue
                        && canalPre.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
                        && (canalMerge.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Nao
                            || canalMerge.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Descredenciado))
                    {

                        // Se usuário tentando descredenciar.
                        if (context.Depth == 1)
                        {
                            new Domain.Servicos.BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).ValidarDescredenciamentoAoPrograma(canalMerge);
                            entityTarget.Attributes["itbc_integraintelbraspontua"] = true;
                            canalMerge.IntegraIntelbrasPontua = true;
                        }
                    }

                    //Envia Verba para Fielo
                    if (canalMerge.StatusEnvioVMC == (int)Domain.Enum.Conta.StatusEnvioVMC.AguardandoEnvio)
                    {
                        entityTarget.Attributes["itbc_statusenviovmc"] = (int)Domain.Enum.Conta.StatusEnvioVMC.Enviando;
                        canalMerge.IntegrarNoPlugin = true;
                    }

                    if (!canalMerge.IntegrarNoPlugin || canalMerge.IntegrarNoPlugin == null || canalMerge.IntegrarNoPlugin.ToString().Equals(""))
                        contaService.IntegracaoBarramento(canalMerge, ref entityTarget);

                    break;

                    #endregion
            }
        }

        private void PreenchimentoDeCampos(ref IPluginExecutionContext context, IOrganizationService adminService)
        {
            var target = context.GetContextEntity();

            if (target.Attributes.Contains("itbc_cnaeid"))
            {
                var cnaeCrm = target.Attributes["itbc_cnaeid"] as Microsoft.Xrm.Sdk.EntityReference;
                if (cnaeCrm != null)
                {
                    var cnae = new Domain.Servicos.CnaeService(context.OrganizationName, context.IsExecutingOffline, adminService).ObterPor(cnaeCrm.Id);


                    if (target.Attributes.Contains("itbc_atividadeeconmicaramodeatividade"))
                    {
                        target.Attributes.Remove("itbc_atividadeeconmicaramodeatividade");
                    }

                    if (target.Attributes.Contains("sic"))
                    {
                        target.Attributes.Remove("sic");
                    }

                    target.Attributes.Add("itbc_atividadeeconmicaramodeatividade", cnae.Denominacao);
                    target.Attributes.Add("sic", cnae.Classe);
                }
            }
        }
    }
}