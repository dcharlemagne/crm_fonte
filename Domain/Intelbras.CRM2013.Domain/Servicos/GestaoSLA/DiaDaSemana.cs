using System;

namespace Intelbras.CRM2013.Domain.Servicos.GestaoSLA
{
    public class DiaDaSemana
    {

        public DiaDaSemana(DayOfWeek dia)
        {
            this.dia = dia;
        }

        private TimeSpan inicio;
        private TimeSpan fim;
        private DayOfWeek dia;

        public DayOfWeek Dia
        {
            get { return dia; }
            set { dia = value; }
        }

        public TimeSpan TempoTotalDaJornadaDeTrabalho
        {
            get
            {
                return this.fim - this.inicio;
            }
        }

        public TimeSpan Inicio
        {
            get { return inicio; }
        }

        public TimeSpan Fim
        {
            get { return fim; }
        }

        public DiaDaSemana(DayOfWeek dia, TimeSpan inicio, TimeSpan fim)
        {

            this.dia = dia;
            this.inicio = inicio;
            this.fim = fim;
        }

        public override bool Equals(object obj)
        {

            var obj1 = (DiaDaSemana)obj;

            return this.inicio == obj1.inicio && this.fim == obj1.fim;
        }

    }
}
