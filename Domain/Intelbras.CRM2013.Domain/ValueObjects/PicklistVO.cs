using System;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    [Serializable]
    public class PicklistVO
    {
        public PicklistVO() { }
        public PicklistVO(int valor) { this.Valor = valor; }
        public PicklistVO(int valor, string nome)
        {
            this.Valor = valor;
            this.Nome = nome;
        }

        public string Nome { get; set; }
        public int Valor { get; set; }
    }
}
