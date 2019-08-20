using System;
using SDKore.DomainModel;
using System.IO;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.AtualizaDistribuidorPreferencialRevenda
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("PCI-Log de Atualização do Distribuidor Preferencial das Revendas");
            Console.WriteLine("Inicio do processo.");

            string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            var parametroGlobalService = new ParametroGlobalService(organizationName, false);

            var parametroGlobal = parametroGlobalService.ObterPor((int)TipoParametroGlobal.DataHistoricoDistribuidorRevenda);
            if (parametroGlobal == null || string.IsNullOrEmpty(parametroGlobal.Valor))
            {
                Console.WriteLine("Erro: (CRM) Parametro Global Data Histórico Distribuidor não configurado.");
                Console.WriteLine("Fim do processo.");
                GravaLog("Erro: (CRM) Parametro Global Data Histórico Distribuidor não configurado.");
                return;
            }

            var datasExecucao = parametroGlobal.Valor;
            string[] datas = datasExecucao.Split(';');

            foreach (string data in datas)
            {
                Console.WriteLine("Processando.");
                DateTime dataProcessar = Convert.ToDateTime(String.Format("{0}/{1}", data, DateTime.Now.Year.ToString()));
                if (dataProcessar == DateTime.Now.Date)
                {
                    var service = new HistoricoDistribuidorService(organizationName, false);
                    var contaService = new ContaService(organizationName, false);
                    var historicos = service.ListarPorPeriodo(dataProcessar);

                    foreach (var historico in historicos)
                    {
                        try
                        {
                            //2- Nas revendas relacionadas à esses registros, alterar o valor do atributo "Distribuidor Preferencial" para registrar o novo distribuidor. 
                            var conta = contaService.BuscaConta(historico.Revenda.Id);
                            Console.WriteLine("Processando: " + conta.RazaoSocial);
                            conta.ID = historico.Revenda.Id;
                            conta.IntegrarNoPlugin = true;
                            conta.DistribuidorPrincipal = new Lookup(historico.Distribuidor.Id, "");
                            contaService.Persistir(conta);

                            //Cada alteração deverá disparar uma mensagem "MSG0072 - REGISTRA_CONTA" para atualizar os sistemas envolvidos.
                            string nomeAbrevMatriEconom = String.Empty;
                            string nomeAbrevRet = String.Empty;
                            string codigoClienteRet = String.Empty;
                            var mensagem = new Domain.Integracao.MSG0072(organizationName, false);
                            mensagem.Enviar(conta, ref nomeAbrevRet, ref codigoClienteRet, ref nomeAbrevMatriEconom);

                            service.AlterarStatus(historico.ID.Value, 993520000); //Fluxo Concluído 993.520.000
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro : " + ex.Message);
                            GravaLog(ex.Message);
                        }
                    }
                }
            }
        }

        protected static void GravaLog(string log)
        {
            using (StreamWriter w = File.AppendText(@"c:\TrideaByAlfa\logs\loghistoricodistribuidor.txt"))
            {
                w.WriteLine("================================");
                w.WriteLine(log);
            }
        }
    }
}
