namespace SDKore.Helper
{
    public class Log
    {
        /// <summary>
        /// Criar log em arquivo texto.
        /// <para>Nome do arquivo: log.txt</para>
        /// </summary>
        /// <param name="value">texto</param>
        public static void Logar(string nomearquivo, string value)
        {
            string strDir = "C:\\TrideaByAlfa\\logs\\";//System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "") + @"\";
            System.IO.StreamWriter sw = System.IO.File.AppendText(strDir + nomearquivo);
            sw.WriteLine(value);
            sw.Close();
        }
    }
}
