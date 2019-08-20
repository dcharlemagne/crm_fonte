using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class ImportarPlanilhaCompromisso : Base
    {

        private OleDbConnection _olecon;
        private OleDbCommand _oleCmd;
        //Mudar endereço
        private static String _Arquivo = @"C:\Users\mcosta\Desktop\Intelbras\Importar Planilha\ImportarCRM10empresas.xlsx";
        private String _StringConexao = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;ReadOnly=False';", _Arquivo);

        private StreamWriter logProcessamento = new StreamWriter(@"C:\Users\mcosta\Desktop\Intelbras\Importar Planilha\LogProcessamento10empresas.txt");

        [Test]
        public void ImportarPlanilha()
        {

            _olecon = new OleDbConnection(_StringConexao);
            _olecon.Open();

            _oleCmd = new OleDbCommand();
            _oleCmd.Connection = _olecon;
            _oleCmd.CommandType = CommandType.Text;

            try
            {
                _oleCmd.CommandText = "SELECT * FROM [Plan1$] ";
                OleDbDataReader reader = _oleCmd.ExecuteReader();

                while (reader.Read())
                {
                    string codigoEmitente = reader.GetValue(0).ToString();

                    Domain.Model.Conta conta = new Domain.Servicos.ContaService(this.OrganizationName, false).BuscarPorCodigoEmitente(codigoEmitente);

                    string unidadeNeg = reader.GetString(1).ToString();
                    //if (!unidadeNeg.ToUpper().Contains("ISEC"))
                    //    continue;

                    Domain.Model.UnidadeNegocio unidadeNegocio = new Domain.Servicos.UnidadeNegocioService(this.OrganizationName, false).BuscaUnidadeNegocioPorNome(unidadeNeg);//"ISEC/MG");

                    List<Domain.Model.CompromissosDoCanal> lstCompromissosDoCanal = new List<Domain.Model.CompromissosDoCanal>();

                    if (conta != null && unidadeNegocio != null)
                    {
                        //lstBeneficioCanal = new Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, false).ListarPorContaUnidadeNegocio(conta.ID.Value, unidadeNegocio.ID.Value);
                        lstCompromissosDoCanal = new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, false).ListaCompromissoCanalPorContaUnidade(conta.ID.Value, unidadeNegocio.ID.Value);

                        //if (lstCompromissosDoCanal.Count == 0 && unidadeNeg.Contains("ISEC"))
                        //{
                        //    unidadeNegocio = new Domain.Servicos.UnidadeNegocioService(this.OrganizationName, false).BuscaUnidadeNegocioPorNome("ISEC/MG");
                        //    if(unidadeNegocio != null)
                        //    lstCompromissosDoCanal = new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, false).ListaCompromissoCanalPorContaUnidade(conta.ID.Value, unidadeNegocio.ID.Value);
                        //}
                    }

                    string linhaDeCorte = reader.GetString(2).ToString().ToUpper();
                    string meta = reader.GetString(3).ToString().ToUpper();

                    if (lstCompromissosDoCanal != null && lstCompromissosDoCanal.Count > 0)
                    {
                        foreach (var compromissoCanal in lstCompromissosDoCanal)
                        {
                            try
                            {
                                if (meta.Equals("SIM"))
                                    compromissoCanal.StatusCompromisso = new SDKore.DomainModel.Lookup(new Guid("31FFD7FA-74ED-E311-9407-00155D013D38"), "");// Cumprido
                                else
                                    compromissoCanal.StatusCompromisso = new SDKore.DomainModel.Lookup(new Guid("41725811-75ED-E311-9407-00155D013D38"), ""); // NaoCumprido

                                new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, false).Atualizar(compromissoCanal);

                                //logProcessamento.WriteLine("Conta : " + conta.ID.Value.ToString() + "/" + conta.RazaoSocial +
                                //                           " - Compromisso : " + compromissoCanal.ID.Value.ToString() + "/" + compromissoCanal.Nome +
                                //                           "Meta :" + meta);
                            }
                            catch (Exception ex)
                            {
                                logProcessamento.WriteLine("Conta : " + conta.ID.Value.ToString() + " / " + conta.RazaoSocial);
                                logProcessamento.WriteLine("Compromisso : " + compromissoCanal.ID.Value.ToString() + " / " + compromissoCanal.Nome);
                                logProcessamento.WriteLine("Meta :" + meta + " - Erro : " + ex.Message);
                                logProcessamento.WriteLine();
                                logProcessamento.WriteLine("----------------------------------------------------------------------------");
                                logProcessamento.WriteLine();
                            }


                            //if (!beneficioCanal.UnidadeDeNegocio.Name.ToUpper().Equals("ISEC/MG"))
                            //    continue;
                            //if (meta.Equals("SIM")) 
                            //beneficioCanal.StatusBeneficio = new SDKore.DomainModel.Lookup(new Guid("35FC4A26-75ED-E311-9407-00155D013D38"), ""); // Ativo
                            //else
                            //beneficioCanal.StatusBeneficio = new SDKore.DomainModel.Lookup(new Guid("E1654A30-75ED-E311-9407-00155D013D38"), ""); // Suspenso

                            //new Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, false).Salvar(beneficioCanal);
                        }
                    }
                }

                logProcessamento.Close();
                logProcessamento.Dispose();

                reader.Close();
            }
            catch { }

        }

        [Test]
        public void SuspenderBeneficiosCanal()
        {
            List<Domain.Model.BeneficioDoCanal> lstBeneficioCanal = new Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, false).ListarPorContaUnidadeNegocioPlanilha();//ListarPorContaUnidadeNegocio(conta.ID.Value, unidadeNegocio.ID.Value);
            int contador = 0;
            foreach (var beneficioCanal in lstBeneficioCanal)
            {
                beneficioCanal.StatusBeneficio = new SDKore.DomainModel.Lookup(new Guid("E1654A30-75ED-E311-9407-00155D013D38"), ""); // Suspenso

                //Somente descomentar quando for rodar
                new Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, false).Atualizar(beneficioCanal);
                contador++;
            }
        }


        [Test]
        public void DescumprirCompromissoCanal()
        {
            int contador = 0;
            List<Domain.Model.CompromissosDoCanal> lstCompromissosDoCanal = new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, false).ListaCompromissoCanalPlanilha();
            foreach (var compromissoCanal in lstCompromissosDoCanal)
            {
                compromissoCanal.StatusCompromisso = new SDKore.DomainModel.Lookup(new Guid("41725811-75ED-E311-9407-00155D013D38"), ""); // NaoCumprido
                compromissoCanal.Validade = new DateTime(2014, 12, 31);
                //Somente descomentar quando for rodar
                new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, false).Atualizar(compromissoCanal);
                contador++;
            }
        }




    }
}
