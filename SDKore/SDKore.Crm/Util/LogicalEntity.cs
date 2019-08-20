using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.Crm.Util
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicalEntity : Attribute
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="name">Nome lógico da entidade do CRM</param>
        public LogicalEntity(string name)
        {
            this._name = name;
        }
    }
}
