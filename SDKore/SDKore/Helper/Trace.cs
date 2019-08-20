using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SDKore.Helper
{
    public class Trace
    {
        public string Organization { get; set; }

        private StringBuilder _sb = null;
        public StringBuilder StringTrace
        {
            get
            {
                if (_sb == null)
                    _sb = new StringBuilder();
                return _sb;
            }
        }

        public Trace(string organization)
        {
            this.Organization = organization;
        }

        /// <summary>
        /// Adiciona o texto no arquivo de trace, caso a flag 'Trace' esteja com valor 'On'.
        /// <para>O Trace será armazenado no diretório que foi configurado no arquivo de configuração.</para>
        /// </summary>
        /// <param name="information">Informação que será gravada no Trace.</param>
        public void Add(string format, params object[] parameters)
        {
            string message = string.Format(format, parameters);
            this.StringTrace.AppendFormat("{0}{1}", message, Environment.NewLine);
        }

        /// <summary>
        /// Adiciona o texto no arquivo de trace, caso a flag 'Trace' esteja com valor 'On'.
        /// <para>O Trace será armazenado no diretório que foi configurado no arquivo de configuração.</para>
        /// </summary>
        /// <param name="ex">Exception do erro.</param>
        public void Add(Exception ex)
        {
            if (ex != null)
            {
                this.StringTrace.AppendFormat("Exception : {0}{1}", ex.GetType().FullName, Environment.NewLine);
                this.StringTrace.AppendFormat("Message   : {0}{1}", ex.Message, Environment.NewLine);
                this.StringTrace.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
                this.StringTrace.AppendFormat("---------------------------------------{0}", Environment.NewLine);
            }
        }

        public void SaveClear(bool error = false)
        {
            this.Save(error);
            this.StringTrace.Clear();
        }

        public void Save(bool error = false)
        {
            if (AddTraceIsValid())
            {
                var header = string.Format("[{0}] - Level: {2} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff"), Environment.NewLine, error ? "Error" : "Information");
                var directory = SDKore.Configuration.ConfigurationManager.GetTraceDirectory();

                using (StreamWriter sw = System.IO.File.AppendText(GetFileName(directory, this.Organization)))
                {
                    sw.WriteLine(string.Format("{0}{1}{2}", header, this.StringTrace.ToString(), Environment.NewLine));
                }
            }
        }

        private static bool AddTraceIsValid()
        {
            var enable = SDKore.Configuration.ConfigurationManager.GetTraceEnable();
            if (string.IsNullOrEmpty(enable) || enable.ToLower() != "on")
                return false;

            var directory = SDKore.Configuration.ConfigurationManager.GetTraceDirectory();
            if (string.IsNullOrEmpty(directory))
                return false;

            return true;
        }

        private static string GetFileName(string directory, string organization)
        {
            string fileNameResponse = string.Empty;
            string fileNameTmp = string.Format("{0}_trace_{1}_", organization, DateTime.Now.ToString("yyyyMMdd"));
            string format = string.Concat(directory, directory.EndsWith(@"\") ? fileNameTmp + "{0}.txt" : @"\" + fileNameTmp + "{0}.txt");
            int aux = 1;
            do
            {
                var tmp = string.Format(format, aux);
                if (!File.Exists(tmp))
                {
                    var fileTmp = File.Create(tmp);
                    fileTmp.Close();
                    fileTmp.Dispose();
                }
                FileInfo info = new FileInfo(tmp);
                if (info.Length >= 500000)
                {
                    aux++;
                    continue;
                }
                fileNameResponse = tmp;
                break;
            } while (aux < 1000);

            return fileNameResponse;
        }
    }
}
