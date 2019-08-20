using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos.GestaoSLA
{
   
    [Serializable]
    public class CalendarioDeFeriados
    {

        private RepositoryService RepositoryService { get; set; }

        public CalendarioDeFeriados(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CalendarioDeFeriados(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public CalendarioDeFeriados() { }


        private static List<Feriado> feriados = null;
        public List<Feriado> Feriados
        {
            get
            {
                if (feriados == null)
                    feriados = new RepositoryService().Feriado.ListarNacionais();

                return feriados;
            }
        }


        public List<Feriado> ObterFeriadosPor(string estado, string cidade)
        {
            List<Feriado> retornoFeriados = new List<Feriado>();
            
            // Feriados Municipais
            var retornoFeriadosMunicipal = new RepositoryService().Feriado.ListarPor(estado, cidade);
            foreach (Feriado feriado in retornoFeriadosMunicipal)
                retornoFeriados.Add(feriado);

            // Feriados Estaduais
            List<Feriado> retornoFeriadosEstaduais = new RepositoryService().Feriado.ListarPor(estado);
            foreach (Feriado feriado in retornoFeriadosEstaduais)
                retornoFeriados.Add(feriado);

            List<Feriado> feriadosNacionais = new RepositoryService().Feriado.ListarNacionais();
            foreach (Feriado feriado in feriadosNacionais)
                retornoFeriados.Add(feriado);

            return retornoFeriados;
        }



        public Feriado ObterFeriadoPor(DateTime data)
        {

            var list = from feriado in this.Feriados
                       where (feriado.DataInicio.Date == data)
                       select feriado;

            if (null != list)
            {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }

            }
            return null;
        }



    }
}
