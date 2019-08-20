using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_produtosdalistapsdid
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            try
            {
                var e = context.GetContextEntity();
                Intelbras.CRM2013.Domain.Model.ProdutoListaPSDPPPSCF mProdutoListaPSD = e.Parse<Intelbras.CRM2013.Domain.Model.ProdutoListaPSDPPPSCF>(context.OrganizationName, context.IsExecutingOffline);
                ProdutoListaPSDService ServiceProdutoListaPSD = new ProdutoListaPSDService(context.OrganizationName, context.IsExecutingOffline);

                switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
                {
                    case MessageName.Create:
                        {
                            switch ((Stage)context.Stage)
                            {
                                case Stage.PreOperation:
                                    if (ServiceProdutoListaPSD.ValidarExistenciaPreCreate(ref mProdutoListaPSD))
                                        throw new ArgumentException("(CRM) Produto já vinculado a Lista corrente.");
                                    break;
                            }
                            break;
                        }
                    case MessageName.Update:
                        {
                            switch ((Stage)context.Stage)
                            {
                                case Stage.PreOperation:
                                    if (ServiceProdutoListaPSD.ValidarExistencia(ref mProdutoListaPSD))
                                        throw new ArgumentException("(CRM) Produto já vinculado a Lista corrente.");

                                    Domain.Model.ProdutoListaPSDPPPSCF mProdutoListaPSDImage = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.ProdutoListaPSDPPPSCF>(context.OrganizationName, context.IsExecutingOffline, context.UserId);

                                    var listaPrecoPSDPPPSCF_Entity = new Entity(SDKore.Crm.Util.Utility.GetEntityName<ListaPrecoPSDPPPSCF>());
                                    listaPrecoPSDPPPSCF_Entity["statuscode"] = new OptionSetValue((int)Domain.Enum.ListaPrecoPSDPPPSCF.StatusIntegracao.NaoIntegrado);

                                    if(mProdutoListaPSDImage.PSD != null)
                                    {
                                        listaPrecoPSDPPPSCF_Entity.Id = mProdutoListaPSDImage.PSD.Id;
                                        adminService.Update(listaPrecoPSDPPPSCF_Entity);
                                    }                          

                                    break;
                            }
                            break;
                        }
                }
            }
            catch (Exception erro)
            {
                throw new InvalidPluginExecutionException(SDKore.Helper.Error.GetMessageError(erro));
            }

        }

        //public void Execute(IServiceProvider serviceProvider)
        //{
        //    var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        //    ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

        //    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        //    IOrganizationService service = serviceFactory.CreateOrganizationService(null);
        //    try
        //    {
        //        switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
        //        {
        //            case Domain.Enum.Plugin.MessageName.Create:
        //            case Domain.Enum.Plugin.MessageName.Update:
        //                Entity entidade = (Entity)context.InputParameters["Target"];

        //                if (new Domain.Servicos.ProdutoListaPSDService(context.OrganizationName, context.IsExecutingOffline, service).ValidarExistencia(
        //                    entidade.Parse<Domain.Model.ProdutoListaPSD>(context.OrganizationName, context.IsExecutingOffline)))
        //                    throw new ArgumentException("Produto já vinculado a Lista corrente.");
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "itbc_produtosdalistapsdid", DateTime.Now));
        //        trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
        //        throw new InvalidPluginExecutionException(ex.Message);
        //    }
        //}
    }
}
