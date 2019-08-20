using System;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class Error : System.Web.UI.Page
    {
        public string Mensagem { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            Mensagem = (string)Application["mensagem"];
        }
    }
}