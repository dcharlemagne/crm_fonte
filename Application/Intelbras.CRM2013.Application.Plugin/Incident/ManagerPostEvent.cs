using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.Incident
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            try
            {
                switch (context.GetMessageName())
                {
                    case PluginBase.MessageName.Create:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            Ocorrencia ocorrencia = entidade.Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);

                            //Verifica se tem intervenção técnica
                            var e = context.GetContextEntity();
                            Ocorrencia ocorrenciacreate = (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Ocorrencia.Retrieve(e.Id);
                            VerificaIntervencao(ocorrenciacreate, context, service);
                            VerificaDataDeIntalacao(ocorrenciacreate, context, service);

                            if (ocorrencia.IntegraAstec == (int)IntegrarASTEC.Sim)
                            {
                                string lstResposta = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(ocorrencia);
                            }

                            if (ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Fechada)
                                return;
                            var ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline);
                            ocorrenciaService.Ocorrencia = ocorrencia;
                            Areas area = ocorrenciaService.IdentificarAreaDeAtendimento();
                            if (area == Areas.ISOL)
                                ocorrenciaService.AtualizarVigenciaContrato();
                        }



                        break;
                    case PluginBase.MessageName.Update:
                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            Ocorrencia ocorrencia = entidade.Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);

                            //Verifica se tem intervenção técnica
                            var eupdate = context.GetContextEntity();
                            Ocorrencia ocorrenciaupdate = (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Ocorrencia.Retrieve(eupdate.Id);
                            VerificaIntervencao(ocorrenciaupdate, context, service);
                            VerificaDataDeIntalacao(ocorrenciaupdate, context, service);

                            if (ocorrencia.IntegraAstec == (int)IntegrarASTEC.Sim)
                            {
                                string lstResposta = new Domain.Servicos.OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service).IntegracaoBarramento(ocorrencia);
                            }


                            Ocorrencia entidadePre = ((Entity)context.PreEntityImages["imagem"]).Parse<Ocorrencia>(context.OrganizationName, context.IsExecutingOffline, adminService);
                            if (ocorrencia.EmpresaExecutanteId == null && entidadePre.EmpresaExecutanteId != null)
                            {
                                ocorrencia.AtualizarOperacoesSuporte = true;
                                ocorrencia.Atualizar();
                            }

                            if (ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Fechada)
                                return;
                            var ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline);
                            ocorrenciaService.Ocorrencia = ocorrencia;
                            Areas area = ocorrenciaService.IdentificarAreaDeAtendimento();
                            if (area == Areas.ISOL)
                                ocorrenciaService.AtualizarVigenciaContrato();



                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
        private void VerificaIntervencao(Ocorrencia ocorrencia, IPluginExecutionContext context, object service)
        {
            if (ocorrencia.ProdutoId == null)
                return;

            if (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Intervencao.ListarPor(ocorrencia).Count > 0)
                return;

            if (ocorrencia.Produto == null)
                ocorrencia.Produto = (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Produto.Retrieve(ocorrencia.ProdutoId.Id);

            if (ocorrencia.Produto.IntervencaoTecnica != null && ocorrencia.Produto.IntervencaoTecnica.Value)
            {
                if (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Intervencao.ListarPor(ocorrencia).Count == 0)
                {
                    (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Intervencao.Create(
                        new IntervencaoTecnica(context.OrganizationName, false, service)
                        {
                            OcorrenciaId = new SDKore.DomainModel.Lookup(ocorrencia.Id, "incident"),
                            Nome = "Produto da Ocorrência esta em Intervenção Técnica!",
                            RazaoStatus = (int)IntervencaoTecnicaEnum.StatusCode.AguardandoAnalise
                        });
                }
                return;
            }

            LinhaComercial linhaComercial = (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).LinhaComercial.ObterPor(ocorrencia.Produto);

            if (linhaComercial == null)
                return;

            if (linhaComercial.NumeroDeDiasParaReincidencia.HasValue && linhaComercial.NumeroDeDiasParaReincidencia.Value > 0 && ocorrencia.OcorrenciaPaiId != null)
            {
                var ocorrenciaPai = (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Ocorrencia.Retrieve(ocorrencia.OcorrenciaPaiId.Id);
                if (ocorrenciaPai.CriadoEm.Value.AddDays(linhaComercial.NumeroDeDiasParaReincidencia.Value) >= ocorrencia.CriadoEm.Value)
                {
                    if (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Intervencao.ListarPor(ocorrencia).Count == 0)
                    {
                        (new Domain.Servicos.RepositoryService(context.OrganizationName, false, service)).Intervencao.Create(
                        new IntervencaoTecnica(context.OrganizationName, false, service)
                        {
                            OcorrenciaId = new SDKore.DomainModel.Lookup(ocorrencia.Id, "incident"),
                            Nome = "Intervenção Técnica por reincidente!",
                            RazaoStatus = (int)IntervencaoTecnicaEnum.StatusCode.AguardandoAnalise
                        });
                    }
                    return;
                }
            }
        }

        private void VerificaDataDeIntalacao(Ocorrencia ocorrencia, IPluginExecutionContext context, object service)
        {
            if(ocorrencia.DataDeConclusao != null && ocorrencia.VeiculoId != null) {
                var veiculo = new CRM2013.Domain.Servicos.RepositoryService(context.OrganizationName, false, service).Veiculo.Retrieve(ocorrencia.VeiculoId.Id);
                if(veiculo != null && veiculo.DataDeInstalacao == null) {
                    veiculo.DataDeInstalacao = ocorrencia.DataDeConclusao;
                    var veiculoTemp = new Domain.Servicos.VeiculoService(context.OrganizationName, false, service).Persistir(veiculo);
                }
            }
            return;
        }
    }
}
