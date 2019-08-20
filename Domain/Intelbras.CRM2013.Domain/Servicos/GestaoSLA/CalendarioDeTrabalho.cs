using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Globalization;

namespace Intelbras.CRM2013.Domain.Servicos.GestaoSLA {

   [Serializable]
   public class CalendarioDeTrabalho {

        private RepositoryService RepositoryService { get; set; }

        public CalendarioDeTrabalho(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CalendarioDeTrabalho(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        private Dictionary<DayOfWeek, DiaDaSemana> diasDaSemana = new Dictionary<DayOfWeek, DiaDaSemana>();

        private DiaDaSemana segunda = null;
        private DiaDaSemana terca = null;
        private DiaDaSemana quarta = null;
        private DiaDaSemana quinta = null;
        private DiaDaSemana sexta = null;
        private DiaDaSemana sabado = null;
        private DiaDaSemana domingo = null;

        public DiaDaSemana Segunda {
            get { return segunda; }
            set {
                segunda = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Terca {
            get { return terca; }
            set {
                terca = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Quarta {
            get { return quarta; }
            set {
                quarta = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Quinta {
            get { return quinta; }
            set {
                quinta = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Sexta {
            get { return sexta; }
            set {
                sexta = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Sabado {
            get { return sabado; }
            set {
                sabado = value;
                this.AdicionarNovo(value);
            }
        }
        
        public DiaDaSemana Domingo {
            get { return domingo; }
            set {
                domingo = value;
                this.AdicionarNovo(value);
            }
        }

        private void AdicionarNovo(DiaDaSemana diaDaSemana) {

            if (this.diasDaSemana.ContainsKey(diaDaSemana.Dia)) {
                this.diasDaSemana.Remove(diaDaSemana.Dia);
            }

            this.diasDaSemana.Add(diaDaSemana.Dia, diaDaSemana);
        }


        public DiaDaSemana ObterDiaDaSemana(DateTime date) {
            return this.diasDaSemana[new GregorianCalendar().GetDayOfWeek(date)];
        }

        public TimeSpan ObterTempoRestanteDaJornadaDeTrabalho(DateTime data, TimeSpan hora) {

            var dayOfWeek = this.ObterDiaDaSemana(data);

            // Tempo que resta para encerrar o ambiente.
            return dayOfWeek.Fim.Subtract(hora);
           
        }


        public Dictionary<DayOfWeek, DiaDaSemana> ObterDiasDeDescanso() {

            var days = new Dictionary<DayOfWeek, DiaDaSemana>();

            var result =
                from day in this.diasDaSemana
                where (day.Value.Fim == TimeSpan.MinValue || day.Value.Inicio == TimeSpan.MinValue)
                select day;

            var enumerator = result.GetEnumerator();

            while (enumerator.MoveNext()) {
                days.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            return days;
            
        }

   }
}
