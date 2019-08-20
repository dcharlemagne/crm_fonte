using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.WalMart;
using Intelbras.CRM2013.Domain.WalMart.Service;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class HelperWS
    {
        #region WebServices
        private static integracaoERPService _intelbrasService = null;
        public static integracaoERPService IntelbrasService
        {
            get
            {
                if (null == _intelbrasService)
                {
                    _intelbrasService = new integracaoERPService();
                    _intelbrasService.Url = SDKore.Configuration.ConfigurationManager.GetSettingValue("EMSService");
                    _intelbrasService.Timeout = 99999999;
                }

                return _intelbrasService;
            }
        }

        private static integracaoSPService _sharepointService = null;
        public static integracaoSPService SharepointService
        {
            get
            {
                if (null == _sharepointService)
                {
                    _sharepointService = new integracaoSPService();
                    _sharepointService.Url = SDKore.Configuration.ConfigurationManager.GetSettingValue("SPService");
                    _sharepointService.Timeout = 99999999;
                }

                return _sharepointService;
            }
        }

        private static ParceiroIntegracao _walmartService = null;
        public static ParceiroIntegracao WalmartService
        {
            get
            {
                if (null == _walmartService)
                {
                    _walmartService = new ParceiroIntegracao();
                    _walmartService.Url = SDKore.Configuration.ConfigurationManager.GetSettingValue("URLServico.br.com.walmartb2b.api");
                    _walmartService.Timeout = 99999999;
                }

                return _walmartService;
            }
        }
        #endregion
    }
    public static class Helper
    {
        #region Method

        public static string RemoverBarraDoFinal(string endereco)
        {
            if (string.IsNullOrEmpty(endereco))
                return endereco;

            if (endereco.EndsWith("/"))
                return RemoverBarraDoFinal(endereco.Substring(0, endereco.Length - 1));
            else
                return endereco;
        }

        public static string Truncate(string texto, int length)
        {
            if (texto.Length > length)
            {
                texto = texto.Substring(0, length);
            }
            return texto;
        }

        /// <summary>
        /// Formata uma string com o formato de CPF
        /// </summary>
        /// <param name="cpf">String sem a formatação</param>
        /// <returns>Strinf formatada</returns>
        public static string FormatarCpf(string cpf)
        {
            string cpfFormatado = cpf.Replace(".", "").Replace("-", "").Trim();
            if (Intelbras.CRM2013.Domain.Servicos.Helper.somenteNumeros(cpf))
            {
                cpfFormatado = cpfFormatado.Insert(3, ".");
                cpfFormatado = cpfFormatado.Insert(7, ".");
                cpfFormatado = cpfFormatado.Insert(11, "-");

                if (cpfFormatado.Length != 11)
                    return cpfFormatado;
            }
            else
            {
                if (cpfFormatado.Length == 11)
                    return cpfFormatado;
            }

            return cpf;
        }

        /// <summary>
        /// Formata um numero na casa de milhares
        /// </summary>
        /// <param name="valor">string</param>
        /// <returns>String formatada</returns>
        public static string formatarCasasMilhar(string valor)
        {
            return String.Format("{0:#.##0}", valor);
        }

        /// <summary>
        /// Formata uma string com o formato de CNPJ
        /// </summary>
        /// <param name="cnpj">String sem a formatação</param>
        /// <returns>Strinf formatada</returns>
        public static string FormatarCnpj(string cnpj)
        {
            string cnpjFormatado = cnpj.Replace(".", "").Replace("-", "").Replace("/", "").Trim();

            if (Intelbras.CRM2013.Domain.Servicos.Helper.somenteNumeros(cnpj))
            {
                cnpjFormatado = cnpjFormatado.Insert(2, ".");
                cnpjFormatado = cnpjFormatado.Insert(6, ".");
                cnpjFormatado = cnpjFormatado.Insert(10, "/");
                cnpjFormatado = cnpjFormatado.Insert(15, "-");

                if (cnpjFormatado.Length != 14)
                    return cnpjFormatado;
            }
            else
            {
                if (cnpjFormatado.Length == 14)
                    return cnpjFormatado;
            }

            return cnpj;
        }

        /// <summary>
        /// Verifica se o CPF é válido.
        /// </summary>
        /// <param name="cpf">Número do CPF</param>
        /// <returns></returns>
        public static bool ValidarCPF(string cpf)
        {
            string valor = cpf.Replace(".", "").Replace("-", "");

            if (valor.Length != 11)
                return false;

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
                if (valor[i] != valor[0])
                    igual = false;

            if (igual || valor == "12345678909")
                return false;

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
                numeros[i] = int.Parse(
                  valor[i].ToString());

            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            int resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }

            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }

            else
                if (numeros[10] != 11 - resultado)
                    return false;

            return true;
        }

        /// <summary>
        /// Verifica se o CNPF é válido
        /// </summary>
        /// <param name="cnpj">Número do CNPJ</param>
        /// <returns></returns>
        public static bool ValidarCNPJ(string cnpj)
        {
            string numero = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            int[] digitos, soma, resultado;
            int nrDig;
            string ftmt;
            bool[] CNPJOk;

            ftmt = "6543298765432";
            digitos = new int[14];
            soma = new int[2];
            soma[0] = 0;
            soma[1] = 0;
            resultado = new int[2];
            resultado[0] = 0;
            resultado[1] = 0;
            CNPJOk = new bool[2];
            CNPJOk[0] = false;
            CNPJOk[1] = false;

            try
            {
                for (nrDig = 0; nrDig < 14; nrDig++)
                {
                    digitos[nrDig] = int.Parse(numero.Substring(nrDig, 1));

                    if (nrDig <= 11)
                        soma[0] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig + 1, 1)));

                    if (nrDig <= 12)
                        soma[1] += (digitos[nrDig] * int.Parse(ftmt.Substring(nrDig, 1)));
                }

                for (nrDig = 0; nrDig < 2; nrDig++)
                {
                    resultado[nrDig] = (soma[nrDig] % 11);

                    if ((resultado[nrDig] == 0) || (resultado[nrDig] == 1))
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == 0);
                    else
                        CNPJOk[nrDig] = (digitos[12 + nrDig] == (11 - resultado[nrDig]));
                }
                return (CNPJOk[0] && CNPJOk[1]);
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Formata uma string com o formato de CEP
        /// </summary>
        /// <param name="cep">String sem a formatação</param>
        /// <returns>Strinf formatada</returns>
        public static string FormatarCep(string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return cep;

            string cepFormatado = cep.Replace("-", "").Trim();
            if (Intelbras.CRM2013.Domain.Servicos.Helper.somenteNumeros(cep))
            {
                cepFormatado = cepFormatado.Insert(5, "-");
            }
            else
            {
                if (cepFormatado.Length == 7)
                    return cepFormatado;
            }
            return cep;
        }

        public static bool somenteNumeros(string numero)
        {
            List<char> num = new List<char>("0123456789");
            CharEnumerator enumerator = numero.GetEnumerator();

            try
            {
                while (enumerator.MoveNext())
                {
                    if (!num.Contains(enumerator.Current))
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna em qual trimestre estamos
        /// pela data atual da máquina
        /// posição 0 retorna 1,2,3 ou 4 Trimestre
        /// posição 1 retorna o valor do enum
        /// </summary>
        /// <returns></returns>
        public static int[] TrimestreAtual()
        {
            int[] retorno = new int[2];

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
            {
                retorno[0] = 1;
                retorno[1] = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
            }
            else if (DateTime.Now.Date.Month >= 4 && DateTime.Now.Date.Month <= 6)
            {
                retorno[0] = 2;
                retorno[1] = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
            }
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 9)
            {
                retorno[0] = 3;
                retorno[1] = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
            }
            else if (DateTime.Now.Date.Month >= 10 && DateTime.Now.Date.Month <= 12)
            {
                retorno[0] = 4;
                retorno[1] = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
            }

            return retorno;
        }
        /// <summary>
        /// Retorna o primeiro dia do proximo trimeste
        /// pela data atual da máquina
        /// posição 0 retorna 1,2,3 ou 4 Trimestre
        /// posição 1 retorna o valor do enum
        /// </summary>
        /// <returns></returns>        
        public static DateTime ProximoTrimestre()
        {
            DateTime dt = new DateTime();

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
            {
                dt = new DateTime(DateTime.Now.Year, 04, 01);
            }
            else if (DateTime.Now.Date.Month >= 4 && DateTime.Now.Date.Month <= 6)
            {
                dt = new DateTime(DateTime.Now.Year, 07, 01);
            }
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 9)
            {
                dt = new DateTime(DateTime.Now.Year, 10, 01);
            }
            else if (DateTime.Now.Date.Month >= 10 && DateTime.Now.Date.Month <= 12)
            {
                dt = new DateTime(DateTime.Now.Year + 1, 01, 01);
            }

            return dt;
        }
        public static DateTime ProximoMes()
        {
            DateTime dt = new DateTime();

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 11)
            {
                var month = DateTime.Now.Date.AddMonths(+1).Month;
                dt = new DateTime(DateTime.Now.Year, month, 01);
            }
            else if (DateTime.Now.Date.Month == 12)
            {
                dt = new DateTime(DateTime.Now.Year + 1, 01, 01);
            }

            return dt;
        }
        public static DateTime ProximoSemestre()
        {
            DateTime dt = new DateTime();

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 6)
            {
                dt = new DateTime(DateTime.Now.Year, 07, 01);
            }
            
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 12)
            {
                dt = new DateTime(DateTime.Now.Year + 1, 01, 01);
            }
            
            return dt;
        }
        public static int TrimestreAnterior()
        {
            
            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
            {            
                return 4;
            }
            else if (DateTime.Now.Date.Month >= 4 && DateTime.Now.Date.Month <= 6)
            {
                return 1;
            }
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 9)
            {
                return 2;
            }
            else if (DateTime.Now.Date.Month >= 10 && DateTime.Now.Date.Month <= 12)
            {
                return 3;                
            }
            return 0;
        }
        public static int TrimestreAnteriorAno()
        {

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
            {
                return DateTime.Now.Year -1;
            }
            else
            {
                return DateTime.Now.Year;
            }

            return 0;
        }
        public static string SemestreAnterior()
        {

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 6)
            {
                return "3,4";
            }
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 12)
            {
                return "1,2";
            }
            return "";
        }
        public static string SemestreAtual()
        {

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 6)
            {
                return "1,2";
            }
            else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 12)
            {
                return "3,4";
            }
            return "";
        }
        public static int SemestreAnteriorAno()
        {

            if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 6)
            {
                return DateTime.Now.Year - 1;
            }
            else
            {
                return DateTime.Now.Year;
            }

            return 0;
        }
        public static DateTime GetDataMesAnterior(string[] args)
        {
            DateTime dt = new DateTime();

            if (args != null && args.Length > 1)
            {
                dt = new DateTime(Convert.ToInt32(args[1].Substring(0, 4)), Convert.ToInt32(args[1].Substring(4, 2)), Convert.ToInt32(args[1].Substring(6, 2)));
            }
            else
            {
                if (args[0].ToUpper().Equals("MARCA_REVENDA_CATEGORIZAR"))
                {
                    if (DateTime.Now.Date.Month == 1)
                    {
                        var year = DateTime.Now.AddYears(-1).Year;
                        dt = new DateTime(year, 12, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 2 && DateTime.Now.Date.Month <= 12)
                    {
                        var month = DateTime.Now.Date.AddMonths(-1).Month;
                        dt = new DateTime(DateTime.Now.Year, month, 01);
                    }
                }
            }
            return dt;
        }
        public static DateTime GetDataMesAtual(string[] args)
        {
            DateTime dt = new DateTime();

            if (args != null && args.Length > 1)
            {
                dt = new DateTime(Convert.ToInt32(args[1].Substring(0, 4)), Convert.ToInt32(args[1].Substring(4, 2)), Convert.ToInt32(args[1].Substring(6, 2)));
            }
            else
            {
                if (args[0].ToUpper().Equals("MARCA_REVENDA_CATEGORIZAR"))
                {
                    if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 12)
                    {
                        dt = new DateTime(DateTime.Now.Year, DateTime.Now.Date.Month, 01);
                    }
                }
            }
            return dt;
        }
        public static DateTime GetDataTrimestreAnterior(string[] args)
        {
            DateTime dt = new DateTime();

            if (args != null && args.Length > 1)
            {
                dt = new DateTime(Convert.ToInt32(args[1].Substring(0, 4)), Convert.ToInt32(args[1].Substring(4, 2)), Convert.ToInt32(args[1].Substring(6, 2)));
            }
            else
            {
                if (args[0].ToUpper().Equals("MARCA_REVENDA_RECATEGORIZAR") ||
                    args[0].ToUpper().Equals("MARCA_REVENDA_CATEGORIZAR"))
                {
                    if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
                    {
                        var year = DateTime.Now.AddYears(-1).Year;
                        dt = new DateTime(year, 10, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 4 && DateTime.Now.Date.Month <= 6)
                    {
                        dt = new DateTime(DateTime.Now.Year, 01, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 9)
                    {
                        dt = new DateTime(DateTime.Now.Year, 04, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 10 && DateTime.Now.Date.Month <= 12)
                    {
                        dt = new DateTime(DateTime.Now.Year, 07, 01);
                    }
                }
            }
            return dt;
        }
        public static DateTime GetDataTrimestreAtual(string[] args)
        {
            DateTime dt = new DateTime();

            if (args != null && args.Length > 1)
            {
                dt = new DateTime(Convert.ToInt32(args[1].Substring(0, 4)), Convert.ToInt32(args[1].Substring(4, 2)), Convert.ToInt32(args[1].Substring(6, 2)));
            }
            else
            {
                if (args[0].ToUpper().Equals("MARCA_REVENDA_RECATEGORIZAR") ||
                    args[0].ToUpper().Equals("MARCA_REVENDA_CATEGORIZAR"))
                {
                    if (DateTime.Now.Date.Month >= 1 && DateTime.Now.Date.Month <= 3)
                    {
                        dt = new DateTime(DateTime.Now.Year, 01, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 4 && DateTime.Now.Date.Month <= 6)
                    {
                        dt = new DateTime(DateTime.Now.Year, 04, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 7 && DateTime.Now.Date.Month <= 9)
                    {
                        dt = new DateTime(DateTime.Now.Year, 07, 01);
                    }
                    else if (DateTime.Now.Date.Month >= 10 && DateTime.Now.Date.Month <= 12)
                    {
                        dt = new DateTime(DateTime.Now.Year, 10, 01);
                    }
                }
            }
            return dt;
        }

        public static Enum.OrcamentodaUnidade.Trimestres ConverterTrimestreOrcamentoUnidade(int trimestre)
        {
            switch (trimestre)
            {
                case 1:
                    return Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                case 2:
                    return Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                case 3:
                    return Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                default:
                    return Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
            }
        }

        public static string ConverterMesEnumParaExtenso(string mesEnum)
        {
            switch (mesEnum)
            {
                case "993520000":
                    return "Janeiro";
                case "993520001":
                    return "Fevereiro";
                case "993520002":
                    return "Março";
                case "993520003":
                    return "Abril";
                case "993520004":
                    return "Maio";
                case "993520005":
                    return "Junho";
                case "993520006":
                    return "Julho";
                case "993520007":
                    return "Agosto";
                case "993520008":
                    return "Setembro";
                case "993520009":
                    return "Outubro";
                case "993520010":
                    return "Novembro";
                case "993520011":
                    return "Dezembro";
                default:
                    return "";
            }
        }

        public static Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes[] ListarMeses(Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            switch (trimestre)
            {
                case Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    return new Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes[]
                    {
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco
                    };


                case Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    return new Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes[]
                    {
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho
                    };


                case Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    return new Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes[]
                    {
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro
                    };


                case Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    return new Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes[]
                    {
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro,
                        Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro
                    };

                default:
                    throw new ArgumentException("(CRM) Opção de trimestre inválida! Valor: " + trimestre);
            }
        }

        public static Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes ConvertToMes(int mes)
        {
            switch (mes)
            {
                case 1: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;
                case 2: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                case 3: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                case 4: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                case 5: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                case 6: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                case 7: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                case 8: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                case 9: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                case 10: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                case 11: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                case 12: return Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                default: throw new ArgumentException("(CRM) Opção para mês inválido! Mes: " + mes);
            }
        }

        public static int ConvertToInt(Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes)
        {
            switch (mes)
            {
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro: return 1;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro: return 2;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco: return 3;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril: return 4;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio: return 5;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho: return 6;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho: return 7;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto: return 8;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro: return 9;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro: return 10;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro: return 11;
                case Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro: return 12;
            }

            throw new ArgumentException("(CRM) Opção inválida para conversar mês! Valor: " + mes);
        }

        public static T CopyObject<T>(T entidadeOrigem)
        {
            T entidadeDestino = (T)Activator.CreateInstance(typeof(T));

            foreach (PropertyDescriptor propOrigem in TypeDescriptor.GetProperties(entidadeOrigem))
                foreach (PropertyDescriptor propDestino in TypeDescriptor.GetProperties(entidadeDestino))
                    if (propDestino.Name == propOrigem.Name)
                    {
                        propDestino.SetValue(entidadeDestino, propOrigem.GetValue(entidadeOrigem));
                        break;
                    }

            return entidadeDestino;
        }

        public static bool TryParseGuid(this string s, out Guid guid)
        {
            try
            {
                guid = new Guid(s);
                return true;
            }
            catch
            {
                guid = Guid.Empty;
                return false;
            }
        }

        public static DateTime ObterProximoDiaUtil(this DateTime d, DateTime data, GestaoSLA.CalendarioDeTrabalho calendario, List<GestaoSLA.Feriado> feriados)
        {

            // Objeto que vai gerenciar o calendario de datas.
            var calendar = new GregorianCalendar();

            while (true)
            {

                // Recuperando o dia da semana que representa a data de vencimento.
                DayOfWeek diaDaSemana = calendar.GetDayOfWeek(data);

                // Verificando se a data é feriado
                var feriado = feriados.Find(h => h.DataInicio.Date == data.Date);

                var isHoliday = feriado != null;

                // Verificando se é um dia de descanso do execution responsible.
                var isRestDay = calendario.ObterDiasDeDescanso().ContainsKey(diaDaSemana);

                if (isRestDay)
                {
                    data = data.AddDays(1);
                }
                else
                {
                    if (isHoliday)
                    {

                        if (!feriado.DiaTodo)
                        {

                            var day = calendario.ObterDiaDaSemana(data);

                            var start = feriado.DataInicio;
                            var end = feriado.DataFim;

                            if (feriado.DataInicio.Hour < day.Inicio.Hours)
                            {
                                start = new DateTime(feriado.DataInicio.Year, feriado.DataInicio.Month, feriado.DataInicio.Day, day.Inicio.Hours, day.Inicio.Minutes, day.Inicio.Seconds);
                            }

                            if (feriado.DataFim.Hour > day.Fim.Hours)
                            {
                                end = new DateTime(feriado.DataFim.Year, feriado.DataFim.Month, feriado.DataFim.Day, day.Fim.Hours, day.Fim.Minutes, day.Fim.Seconds);
                            }

                            var remmaingBalance = end.Subtract(start);
                            data = data.AddMinutes(remmaingBalance.TotalMinutes);

                            break;
                        }
                        else
                        {
                            data = data.AddDays(1);
                        }

                    }
                    else
                    {
                        break;
                    }
                }

            }

            return data;

        }
        #endregion

        #region Enum

        public static string GetDescription(System.Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        #endregion
    }
}
