using System;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    [Serializable]
    public class LookupVO
    {
        public LookupVO() { }
        public LookupVO(Guid Id) { this.Id = Id; }

        public LookupVO(Guid Id, string nome, string tipo) 
        {
            this.Id = Id;
            this.Nome = nome;
            this.Tipo = tipo; 
        }


        public Guid Id { get; set; }
        public String Nome { get; set; }
        public String Tipo { get; set; }
    }
}
