using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.Domain.Servicos.GestaoSLA
{

    [LogicalEntity("new_feriado_municipal_estadual")]
    public class Feriado
   {
        private RepositoryService RepositoryService { get; set; }

        public Feriado(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public Feriado(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        public Feriado(string organizacao, bool isOffline, string nome, DayOfWeek diaDaSemana, DateTime dataInicio, DateTime dataFim)
             : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
            this.nome = nome;
            this.dia = diaDaSemana;
            this.dataInicio = dataInicio;
            this.dataFim = dataFim;
        }

        private string nome = "";

        public bool DiaTodo {
            get {
                var restante = this.dataFim.Subtract(this.dataInicio);

                // Verificando se o feriado está marcado para o dia todo.
                return Convert.ToInt32(restante.TotalMinutes) == 1439 || Convert.ToInt32(restante.TotalMinutes) == 1440;
            }
        }

        private DayOfWeek dia;
        private DateTime dataInicio = DateTime.MinValue;
        private DateTime dataFim = DateTime.MinValue;
       
        public DayOfWeek Dia {
            get { return dia; }
            set { dia = value; }
        }

        public DateTime DataInicio {
            get { return dataInicio; }
            set { dataInicio = value; }
        }

        public DateTime DataFim {
            get { return dataFim; }
            set { dataFim = value; }
        }

        [LogicalAttribute("new_uf")]
        public String Uf { get; set; }

        [LogicalAttribute("new_cidade")]
        public String Cidade { get; set; }

        [LogicalAttribute("new_data")]
        public DateTime Data { get; set; }
        
        public static bool VerificaFeriado(DateTime data, List<Feriado> feriados)
        {
            Boolean dataehFeriado = false;
            foreach (Feriado feriado in feriados)
                if (feriado.DataInicio.Date.ToString("dd/MM/yyyy") == data.ToString("dd/MM/yyyy"))
                    dataehFeriado = true;

            return dataehFeriado;
        }
   }
}
