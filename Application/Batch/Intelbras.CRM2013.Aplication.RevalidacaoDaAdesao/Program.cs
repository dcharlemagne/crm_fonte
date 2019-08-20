using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Servicos;
using System.IO;
using System.Globalization;

namespace Intelbras.CRM2013.Aplication.RevalidacaoDaAdesao
{
    class Program
    {
        static string statusFileName = "status.txt";
        static DateTime dataInicio = DateTime.Now;

        static int Main(string[] args)
        {
            try
            { 
                var f = CriaArquivoStatus();

                if (f == null)
                {
                    Console.WriteLine("Já existe um processo em andamento.");
                    return 0;
                }

                string organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                if (args != null && args.Length > 0)
                    organizationName = args[0].ToString();

                var logger = new SDKore.Helper.Logger("PCI-Log de Revalidação da Adesão", false);
                logger.Log("Inicio do processo.", true, true);

                var parametroGlobalService = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(organizationName, false);

                var parametroGLobal = parametroGlobalService.ObterEmailContatosAdministrativos();
                if (parametroGLobal == null
                    || !parametroGLobal.ID.HasValue
                    || parametroGLobal.ID.Value == Guid.Empty
                    || string.IsNullOrEmpty(parametroGLobal.Valor))
                {
                    logger.Log("Erro: (CRM) Parametro Global Contatos Administrativos não configurado.", true, true);
                    logger.Log("Fim do processo.", true, true);
                    File.Delete(statusFileName);
                    return 6666;
                }

                logger.Log("Obtendo contas participantes do programa de benefícios.", true, true);

                var beneficioDoCanalService = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(organizationName, false);

                var lstMatrizParticipante = beneficioDoCanalService.ListarContasParticipantes();

                logger.Log(lstMatrizParticipante.Count.ToString() + " contas participantes do programa de benefícios.", true, true);

                int count = 0;
                foreach (var matriz in lstMatrizParticipante)
                {
                    count++;

                    logger.Log(string.Format("{0} - {1} - {2}", count, matriz.RazaoSocial, matriz.CpfCnpj), true, true);

                    try
                    {
                        AtualizaStatus(f, dataInicio, count, lstMatrizParticipante.Count);

                        beneficioDoCanalService.AdesaoAoPrograma(matriz);
                    }
                    catch (Exception ex)
                    {
                        logger.Log(string.Format("\nErro: {0}\n Stack: {1}\n", ex.Message, ex.StackTrace), true, true);
                    }
                }

                f.Flush();

                f.Close();

                File.Delete(statusFileName);
                logger.Log("Fim do processo.", true, true);

                EnviaEmail(organizationName, parametroGLobal.Valor, logger.FileName);
                return 0;
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                Console.WriteLine(mensagem);
                return ex.GetHashCode();
            }
        }

        private static void EnviaEmail(string organizationName, string destinatarios, string fileName)
        {
            var emailService = new Intelbras.CRM2013.Domain.Servicos.EmailService(organizationName, false);

            string corpo = string.Format("\n\tO Processo de revalidação da Adesão foi finalizado em: {0}\n\tEm anexo segue o log.", DateTime.Now);

            destinatarios = destinatarios.Replace(" ", "");
            string[] para;

            if (destinatarios.Contains(";"))
            {
                para = destinatarios.Split(';');
            }
            else if (destinatarios.Contains(","))
            { 
                para = destinatarios.Split(',');
            }
            else
            {
                para = new string[1];
                para[0] = destinatarios;
            }

            emailService.EnviaEmailComAnexo("Revalidação da Adesão", corpo, para, fileName);
        }

        private static StreamWriter CriaArquivoStatus()
        {
            if (File.Exists(statusFileName))
            {
                try
                {
                    File.Delete(statusFileName);
                }
                catch
                { }

                if (File.Exists(statusFileName))
                    return null;
            }

            var f = File.CreateText(statusFileName);
            f.AutoFlush = true;

            AtualizaStatus(f, dataInicio, 0, 0);

            return f;
        }

        private static void AtualizaStatus(StreamWriter f, DateTime dataInicio, int count, int total)
        {
            f.BaseStream.Position = 0;
            f.BaseStream.SetLength(0);

            decimal percentualCompletado = 0M;
            if (total > 0)
                percentualCompletado = decimal.Round(decimal.Divide(count, total) * 100, 1);

            double tempoRestante = 0;
            if (count >= 3)
            {
                var tempoMedio = (DateTime.Now - dataInicio).TotalSeconds / count;
                tempoRestante = Math.Truncate((total - count) * tempoMedio);                
            }

            var msg = string.Format("Iniciado em: {0}\nProcessando: {1} / {2}\nPercentual processado: {3}%", dataInicio, count, total, percentualCompletado);

            if (tempoRestante > 0)
            {
                long minutos = 0;
                long horas = Math.DivRem((int)(tempoRestante/60), 60, out minutos);
                if (horas == 0)
                    msg = string.Format("{0}\nTempo restante estimado: {1} minutos", msg, minutos);
                else if (horas == 1)
                    msg = string.Format("{0}\nTempo restante estimado: {1} hora e {2} minutos", msg, horas, minutos);

                else
                    msg = string.Format("{0}\nTempo restante estimado: {1} horas e {2} minutos", msg, horas, minutos);

            }

            f.Write(msg.ToString(CultureInfo.InvariantCulture));
        }
    }
}
