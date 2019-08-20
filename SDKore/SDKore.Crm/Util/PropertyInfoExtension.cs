using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Reflection
{
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Obtém listas das propriedades decoradas no domínio
        /// </summary>
        /// <param name="props">Propriedades</param>
        /// <returns></returns>
        public static List<string> GetNameProperties(this PropertyInfo[] props)
        {
            var list = new List<string>();
            if (props == null)
                return list;

            for (int index = 0; index < props.Length; index++)
                list.Add(props[index].Name);

            return list;
        }
    }
}
