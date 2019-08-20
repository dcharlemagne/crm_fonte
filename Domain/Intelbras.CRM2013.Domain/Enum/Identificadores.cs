// Enum customizado para atendimento pontual de integrações
// Desenvolvido por FCJ em 20/03/2014

using sistema = System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Enum
{
    public class Sistemas
    {
        public enum Sistema
        {
            [Description("AE6D5BEA-1C3C-4469-A0F5-CED0D8475F10")]
            Pollux = 0,
            [Description("DBFC273E-4811-40C4-8A4E-1629731ADD9A")]
            CRM = 1,
            [Description("64546C2E-6DAB-4311-A74A-5ACA96134AFF")]
            EMS = 2,
            [Description("95061229-FF31-4FD1-A875-96A98D67280C")]
            Extranet = 3,
            [Description("11111111-AA11-1AA1-A111-11A11A11111A")]
            Teste = 4,
            [Description("6642D034-E70F-4215-8318-C760E238AB06")]
            SellOut = 5,
            [Description("73B7A760-9170-484A-8904-D9B126B4B018")]
            Senior = 6,
            [Description("249E87D7-0AE4-455C-8AED-741BB12ECE4C")]
            API = 7
        }

        public static string RetornaSistema(sistema.Enum en)
        {
            sistema.Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }
    }
}
