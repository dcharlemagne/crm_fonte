using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDKore.Crm.Util
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class LogicalAttribute : System.Attribute
    {
        private string _name;
        /// <summary>
        /// Nome lógico do atributo no CRM.
        /// </summary>
        public string Name
        {
            get
            {
                return _name.ToLower();
            }
        }
       
        private string _entity;
        /// <summary>
        /// Nome lógico da Entidade no CRM
        /// </summary>
        public string Entity
        {
            get
            {
                return _entity.ToLower();
            }
            
        }

        /// <summary>
        /// </summary>
        /// <param name="name">Nome lógico do atributo no CRM</param>
        public LogicalAttribute(string name)
        {
            if (name == "id")
                _name = _entity + name;
            //else if (name == "name")
            //    _name = "itbc_name";
            else
                _name = name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Nome lógico do atributo no CRM</param>
        /// <param name="entity">Nome lógico da Entidade no CRM</param>
        public LogicalAttribute(string name, string entity)
        {
            _entity = entity;
            if (name == "id")
                _name = _entity + name;
            //else if (name == "name")
            //    _name = "itbc_name";
            else
                _name = name;
        }
    }
}
