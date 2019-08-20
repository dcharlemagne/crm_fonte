using System;
using Intelbras.CRM2013.Domain.Servicos;
using System.Diagnostics;

namespace Intelbras.CRM2013.Aplicacao.Categorizacao
{
    class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;

        static void Main(string[] args)
        {
            // Pega o primeiro dia do trimestre anterior
             DateTime dt = Helper.GetDataTrimestreAnterior(args);
            //string acao = "CATEGORIZACAO_REVENDAS";
            ContaService ContaServices = new ContaService(OrganizationName, IsOffline);

            switch (args[0].ToUpper())
            {
                case "CATEGORIZACAO_REVENDAS":
                    //UC13 - Categorização de Revendas
                    ContaServices.AtualizaContasCategorizacao();
                    break;
                case "RECATEGORIZACAO_REVENDAS":
                    //UC14 - Recategorização de Revendas
                    ContaServices.AtualizaContasReCategorizacao();
                    break;
                case "ALTEREACAO_ACESSO_EXTRANET":
                    //UC15 - Alteração de Acesso à Extranet
                    ContaServices.AlteraAcessosExtranet();
                    break;
                case "MARCA_REVENDA_CATEGORIZAR":
                    //UC16 - Marca Revendas para Categorização
                    ContaServices.AtualizaContasCategorizar(Helper.GetDataMesAnterior(args), Helper.GetDataMesAtual(args));
                    break;
                case "MARCA_REVENDA_RECATEGORIZAR":
                    //UC17 - Marca Revendas para Recategorização
                    ContaServices.AtualizaContasRecategorizar();
                    break;
                case "MARCA_VMC_REVENDA":
                    //UC22 - VMC - Apuração VMC Revendas
                    ContaServices.MarcaRevendasVMC();
                    break;
                case "ENVIA_REGISTRO_SELLOUT":
                    //UC24 - INTELBRAS PONTUA - Envio de registro de venda e cálculo dos pontos
                    //ContaServices.EnviaRegistroSelloutFielo();
                    break;
                case "MARCA_REVENDA_RECATEGORIZAR_MENSAL":
                    ContaServices.MarcaRevendaRecategorizarMensal();
                    break;
                case "RECATEGORIZACAO_REVENDAS_MENSAL":
                    ContaServices.AtualizaContasReCategorizacaoMensal();
                    break;
            }
        }
    }
}
