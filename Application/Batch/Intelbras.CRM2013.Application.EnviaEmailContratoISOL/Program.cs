using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;
using Excel = ClosedXML.Excel;

namespace Intelbras.CRM2013.Application.EnviaEmailContratoISOL
{
    class Program
    {

        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;

        static void Main(string[] args)
        {
            var RepositoryService = new RepositoryService(OrganizationName, IsOffline);

            #region Busca os contratos com de termino daqui a 90 dias
            var dataTermino = DateTime.Today.AddDays(90);
            List<Contrato> lstContratosProximos = new ContratoService(OrganizationName, IsOffline).ObterContratosProximoVencimento(dataTermino);
            #endregion

            #region Busca os contratos com de termino hoje
            dataTermino = DateTime.Today;
            List<Contrato> lstContratosHoje = new ContratoService(OrganizationName, IsOffline).ObterContratosProximoVencimento(dataTermino);
            #endregion

            #region Recuperar os email para enviar os contratos que estão proximos a expirar.
            var email = new ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.EmailContratoVencimento);
            #endregion

            if (lstContratosHoje.Count > 0)
            {
                string data = DateTime.Today.GetDateTimeFormats('d')[5];
                string textoEmail = "<b> Contratos com vencimento para: </b>" + data + " <br />";
                textoEmail += "<b> Rotina retornou: </b>" + lstContratosHoje.Count + " contratos. <br />";

                textoEmail = "<table width='100%'>";
                textoEmail += "<thead><tr><td align='center'>ID do Contrato</td><td align='center'>Nome do Contrato</td><td align='center'>Data de Término</td><td align='center'>Tipo de Vigência</td></tr></thead>";
                textoEmail += "<tbody>";

                foreach (var contrato in lstContratosHoje)
                {
                    textoEmail += "<tr>";
                    textoEmail += "<td align='center'>" + contrato.NumeroContrato + "</td>";
                    textoEmail += "<td align='center'>" + contrato.NomeContrato + "</td>";
                    textoEmail += "<td align='center'>" + data + "</td>";
                    textoEmail += "<td align='center'>" + contrato.TipoVigencia.ToString() + "</td>";
                    textoEmail += "</tr>";
                }
                textoEmail += "</tbody></table>";

                var emails = email.Valor.Split(';');
                foreach(var emailTemp in emails)
                {
                    RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Contratos com vencimento para hoje, " + data + ".", "", emailTemp);
                }
            }

            if (lstContratosProximos.Count > 0)
            {
                string data = DateTime.Today.AddDays(60).GetDateTimeFormats('d')[5];
                string textoEmail = "<b> Contratos com vencimento para: </b>" + data + " <br />";
                textoEmail += "<b> Rotina retornou: </b>" + lstContratosProximos.Count + " contratos. <br />";

                string nomeArquivo = "Contratos_" + data + ".xlsx";
                textoEmail = "<style>padding-left: 15px;border-bottom: 1px solid;</style><table width='100%'>";
                textoEmail += "<thead><tr><td align='center'>ID do Contrato</td><td align='center'>Nome do Contrato</td><td align='center'>Data de Término</td><td align='center'>Tipo de Vigência</td></tr></thead>";
                textoEmail += "<tbody>";

                foreach (var contrato in lstContratosProximos)
                {
                    textoEmail += "<tr>";
                    textoEmail += "<td align='center'>" + contrato.NumeroContrato + "</td>";
                    textoEmail += "<td align='center'>" + contrato.NomeContrato + "</td>";
                    textoEmail += "<td align='center'>" + data + "</td>";
                    textoEmail += "<td align='center'>" + contrato.TipoVigencia.ToString() + "</td>";
                    textoEmail += "</tr>";
                }
                textoEmail += "</tbody></table>";

                var emails = email.Valor.Split(';');
                foreach(var emailTemp in emails)
                {
                    RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Contratos com vencimento para daqui 60 dias, " + data + ".", "", emailTemp);
                }
            }
        }
    }
}
