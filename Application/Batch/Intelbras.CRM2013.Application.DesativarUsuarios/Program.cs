using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Application.DesativarUsuarios
{
    static class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;
        static void Main()
        {
            new UsuarioService(OrganizationName, IsOffline).DesativaUsuario();
        }
    }
}
