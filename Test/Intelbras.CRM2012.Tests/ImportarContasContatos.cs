using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class ImportarContasContatos : Base
    {
        private StreamWriter logEnvioConta = new StreamWriter(@"C:\Users\mcosta\Desktop\Intelbras\Importar Planilha\LogEnvioConta.txt");
        private StreamWriter logEnvioContato = new StreamWriter(@"C:\Users\mcosta\Desktop\Intelbras\Importar Planilha\LogEnvioContato.txt");
        private string nomeConta = String.Empty;
        private string guidConta = String.Empty;
        private string nomeContato = String.Empty;
        private string guidContato = String.Empty;
        
        [Test]
        public void ImportarContatos()
        {
            List<Domain.Model.Contato> lstContatos = new Domain.Servicos.ContatoService(this.OrganizationName, false).ListarTodosContatos();
            int contador = 0;
            
            foreach (var item in lstContatos)
            {
                try
                {
                    nomeContato = item.NomeCompleto;
                    guidContato = item.ID.Value.ToString();
                    string lstResposta = new Domain.Servicos.ContatoService(this.OrganizationName, false).IntegracaoBarramento(item);
                    nomeContato = String.Empty;
                    guidContato = String.Empty;
                    contador++;
                }
                catch (Exception ex)
                {
                    logEnvioContato.WriteLine("Contato : " + nomeConta + " / " + "Guid - " + " Erro: " + ex.Message);
                }
            }

            logEnvioContato.Close();
            logEnvioContato.Dispose();
        }
    }
}
