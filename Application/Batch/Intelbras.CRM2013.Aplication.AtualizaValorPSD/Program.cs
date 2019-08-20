using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.AtualizarValorPSD
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main()
        {
            try
            {
                string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
                var relacionamentoCanalService = new Intelbras.CRM2013.Domain.Servicos.RelacionamentoCanalService(organizationName, false);

                #region Recupera valor do parâmetro global.
                Intelbras.CRM2013.Domain.Servicos.RepositoryService RepositoryService = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(organizationName, false);
                var parametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.DataExecucaoValorPSD);
                if (parametroGlobal == null)
                {
                    throw new ArgumentException("(CRM) Não foi encontrado Parametro Global [" + (int)(int)Domain.Enum.TipoParametroGlobal.DataExecucaoValorPSD + "].");
                }
                var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
                #endregion

                if (dataConsulta.Date == DateTime.Now.Date)
                {
                    ProdutoService ProdutoServices = new ProdutoService(organizationName, false);
                    ProdutoServices.EnviarValorPSD();

                    #region Atualiza data parâmetro global
                    parametroGlobal.Valor = Helper.ProximoMes().Day.ToString() + "/" + Helper.ProximoMes().Month.ToString() + "/" + Helper.ProximoMes().Year.ToString();
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);
                    #endregion                   
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message);
            }
            return 0;
        }       
    }
}
