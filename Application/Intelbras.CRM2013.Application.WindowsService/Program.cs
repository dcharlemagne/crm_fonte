using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;


namespace Intelbras.CRM2013.Application.WindowsService
{
    static class Program
    {
        /// <summary>
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new RevalidaUsuarioEquipe()
			};

#if RUN_IN_VS

            ServiceBase.Run(ServicesToRun);
            RevalidaUsuarioEquipe services = new RevalidaUsuarioEquipe();
            services.RelavidarUsuariosEquipe();
#else
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
