using System;

namespace Intelbras.CRM2013.Util.CustomException
{
    public class BarramentoException : Exception
    {
        public string CodeErro { get; set; }
        
        public BarramentoException()
        {
        }

        public BarramentoException(string message)
            : base(message)
        {
        }

        public BarramentoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BarramentoException(string message, string codeErro)
            : base(message)
        {
            CodeErro = codeErro;
        }
    }
}