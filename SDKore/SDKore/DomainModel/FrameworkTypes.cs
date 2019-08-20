using System;

namespace SDKore.DomainModel
{
    public class Lookup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public Lookup()
        {
            this.Id = Guid.Empty;
            this.Name = string.Empty;
            this.Type = string.Empty;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="id">Identificador do registro</param>
        /// <param name="name">Nome do registro</param>
        /// <param name="type">Tipo de registro<para>Nome lógico da entidade no CRM.</para></param>
        public Lookup(Guid id, string name, string type)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="id">Identificador do registro</param>
        /// <param name="type">Tipo de registro<para>Nome lógico da entidade no CRM.</para></param>
        public Lookup(Guid id, string type)
        {
            this.Id = id;
            this.Name = string.Empty;
            this.Type = type;
        }

        /// <summary>
        /// Compara a instancia atual de lookup com outro lookup
        /// </summary>
        /// <param name="target">Alvo que irá comparar o valor</param>
        /// <returns></returns>
        public bool Comparar(Lookup target)
        {
            //Acho que nao tem como acontecer,inserido por precaução
            if (target.Id == null || this.Id == null)
                throw new ArgumentNullException("Id do campo de lookup vazio");

            //Se ele nao preencher o Type só compara o ID, se preencher compara o type tambem
            if ((target.Id == this.Id && (String.IsNullOrEmpty(target.Type) || String.IsNullOrEmpty(this.Type)))
                || (target.Id == this.Id && (!string.IsNullOrEmpty(target.Type) && !string.IsNullOrEmpty(this.Type)) && target.Type.Equals(this.Type)))
                return true;

            return false;
        }
        
        public static bool operator ==(Lookup a, Lookup b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }
            
            return a.Id == b.Id;
        }

        public static bool operator !=(Lookup a, Lookup b)
        {
            return !(a == b);
        }
    }
}