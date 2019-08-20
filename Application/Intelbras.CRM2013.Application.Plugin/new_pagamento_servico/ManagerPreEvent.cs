using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.new_pagamento_servico
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    var e = context.GetContextEntity();
                    var pagamento = e.Parse<PagamentoServico>(context.OrganizationName, context.IsExecutingOffline);
                    if (pagamento.Ocorrencia == null)
                        pagamento.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Ocorrencia.Retrieve(pagamento.OcorrenciaId.Id);

                    if (pagamento.Ocorrencia != null)
                    {
                        if (pagamento.Ocorrencia.LimiteOrcamento.HasValue)
                        {
                            if ((pagamento.Ocorrencia.ObterSomaPagamentos(null) + pagamento.Valor) > pagamento.Ocorrencia.LimiteOrcamento)
                            {
                                throw new ArgumentException("Não é possível salvar. Valor que esta sendo inserido estoura o limite para Ocorrencia.");
                            }
                        }
                    }
                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var entityOld = context.PreEntityImages["imagem"];
                    var entityUpdate = (Entity)context.InputParameters["Target"];
                    var pagamentoOld = entityOld.Parse<PagamentoServico>(context.OrganizationName, context.IsExecutingOffline);
                    var pagamentoUpdate = entityUpdate.Parse<PagamentoServico>(context.OrganizationName, context.IsExecutingOffline);
                    if (pagamentoUpdate.Ocorrencia == null)
                        pagamentoUpdate.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Ocorrencia.Retrieve(pagamentoOld.OcorrenciaId.Id);

                    if (pagamentoUpdate.Ocorrencia != null)
                    {
                        if (pagamentoUpdate.Ocorrencia.LimiteOrcamento.HasValue)
                        {
                            if ((pagamentoUpdate.Ocorrencia.ObterSomaPagamentos(pagamentoOld) + pagamentoUpdate.Valor) > pagamentoUpdate.Ocorrencia.LimiteOrcamento)
                            {
                                throw new ArgumentException("Não é possível salvar. Valor que esta sendo inserido estoura o limite para Ocorrencia.");
                            }
                        }
                    }
                    break;
                    #endregion
            }
        }
    }
}

