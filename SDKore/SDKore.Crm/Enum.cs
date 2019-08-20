using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.Crm
{
    public class Enum
    {
        public enum AuthenticationType { AD, Passport, SPLA };
        public enum TypeXrmSdk { None = 0, EntityReference, OptionSetValue, Money, PartyList, Datetime };

    }
}
