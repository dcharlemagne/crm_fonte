using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.ViewModels;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Excel = ClosedXML.Excel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ArquivoService
    {
        private int TimeOutSharepoint { get { return int.Parse(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Sharepoint.TimeOut", true)); } }
        private string UsuarioSharePoint = Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        private string SenhaSharePoint = Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaSharePoint"));

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ArquivoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ArquivoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ArquivoService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        public string DownLoadArquivo(string diretorio, string filename, string documentbody, string extensao)
        {
            string arquivoretorno = diretorio + filename + extensao;

            if (File.Exists(arquivoretorno))
            {
                File.Delete(arquivoretorno);
            }

            using (FileStream fileStream = new FileStream(arquivoretorno, FileMode.OpenOrCreate))
            {
                byte[] fileContent = Convert.FromBase64String(documentbody);
                fileStream.Write(fileContent, 0, fileContent.Length);
                fileStream.Close();
            }

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            return arquivoretorno;
        }

        public void AnexaArquivo(string arquivo, string Assunto, string NomeFile, Lookup entity, out Guid anexoId)
        {
            FileStream stream = File.OpenRead(arquivo);

            byte[] byteData = new byte[stream.Length];
            stream.Read(byteData, 0, byteData.Length);
            stream.Close();

            string encodedData = System.Convert.ToBase64String(byteData);

            Anotacao anexo = new Anotacao(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            anexo.Assunto = Assunto;
            anexo.Body = encodedData;
            anexo.Tipo = @"application\ms-excel";

            anexo.NomeArquivos = NomeFile;
            anexo.EntidadeRelacionada = entity;
            anexo.ID = Guid.NewGuid();
            anexoId = anexo.ID.Value;

            RepositoryService.Anexo.Create(anexo);
        }

        public Guid AnexaArquivo(string filePath, string assunto, string mimetype, Lookup entity)
        {
            FileStream stream = File.OpenRead(filePath);

            byte[] byteData = new byte[stream.Length];
            stream.Read(byteData, 0, byteData.Length);
            stream.Close();

            string encodedData = Convert.ToBase64String(byteData);

            Anotacao anexo = new Anotacao(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            anexo.Assunto = assunto;
            anexo.Body = encodedData;
            anexo.Tipo = mimetype;
            anexo.NomeArquivos = "Anexo" + Path.GetExtension(filePath);
            anexo.EntidadeRelacionada = entity;

            return RepositoryService.Anexo.Create(anexo);
        }

        public Guid AnexaArquivo(MetadaUnidade metaUnidade, string arquivo, string NomeFile)
        {
            FileStream stream = File.OpenRead(arquivo);

            byte[] byteData = new byte[stream.Length];
            stream.Read(byteData, 0, byteData.Length);
            stream.Close();

            string encodedData = Convert.ToBase64String(byteData);

            Anotacao anexo = new Anotacao(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            anexo.Assunto = NomeFile;
            anexo.Body = encodedData;
            anexo.Tipo = @"application\ms-excel";

            anexo.NomeArquivos = NomeFile + ".xlsx";
            anexo.EntidadeRelacionada = new Lookup(metaUnidade.ID.Value, metaUnidade.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaUnidade));

            return RepositoryService.Anexo.Create(anexo);
        }

        #region Métodos de Geração/Leitura de Planilha de Orçamento

        #region Orçamento Detalhado/Resumido
        public List<Model.OrcamentoDetalhado> lerPlanilhaOrcamento(DateTime datearquivo, string arquivo)
        {
            #region váriaveis e objetos
            object ValorExcel;
            object QtdeExcel;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();
            OrcamentoDetalhado item;
            Guid trimestre1 = Guid.Empty;
            Guid trimestre2 = Guid.Empty;
            Guid trimestre3 = Guid.Empty;
            Guid trimestre4 = Guid.Empty;
            Excel.XLWorkbook xlWorkbook;

            Decimal dVlrExcel = 0;
            int iQtdeExcel = 0;
            bool bdVlrExcel;
            bool diQtdeExcel;
            #endregion
            List<string> lstDescricaoItens = new List<string>();
            bool ErroLinha = false;

            try
            {
                xlWorkbook = new Excel.XLWorkbook(arquivo);
            }
            catch (Exception erroopenexcel)
            {
                throw new ArgumentException("Erro ao abrir o arquivo:" + arquivo + "  Erro:" + erroopenexcel.Message);
            }

            try
            {
                Excel.IXLWorksheet xlWorksheet = xlWorkbook.Worksheet(1);
                Excel.IXLRange xlRange = xlWorksheet.RangeUsed();

                foreach (Excel.IXLRangeRow row in xlRange.Rows())
                {
                    if (row.RowNumber() == 4)
                    {
                        #region get id of Trimetres
                        // {15,21,27,33} orcamento
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                        if (ValorExcel != null)
                            trimestre1 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 24).Value;
                        if (ValorExcel != null)
                            trimestre2 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 30).Value;
                        if (ValorExcel != null)
                            trimestre3 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 36).Value;
                        if (ValorExcel != null)
                            trimestre4 = Guid.Parse(ValorExcel.ToString());
                        #endregion
                    }
                    if (row.RowNumber() > 6)
                    {
                        #region
                        item = new OrcamentoDetalhado();
                        item.Trimestre1 = new Trimestre();
                        item.Trimestre2 = new Trimestre();
                        item.Trimestre3 = new Trimestre();
                        item.Trimestre4 = new Trimestre();

                        item.Ano = Convert.ToInt32(xlWorksheet.Cell(row.RowNumber(), 1).Value);
                        item.DatadoArquivo = datearquivo;
                        #region propertys de controle
                        item.SegmentoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 17).Value.ToString());
                        item.FamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 16).Value.ToString());
                        item.SubFamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 15).Value.ToString());
                        item.ProdutoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 14).Value.ToString());
                        item.CanalID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 13).Value.ToString());
                        #endregion

                        #region 1 Trimestre
                        item.Trimestre1.Id = trimestre1;
                        item.Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                        #region Mes1
                        item.Trimestre1.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 19).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre1.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 20).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 21).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre1.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 22).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 23).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 2 Trimestre
                        item.Trimestre2.Id = trimestre2;
                        item.Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                        #region Mes1
                        item.Trimestre2.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 24).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 25).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre2.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 26).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 27).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre2.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 28).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 29).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 3 Trimestre
                        item.Trimestre3.Id = trimestre3;
                        item.Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                        #region Mes1
                        item.Trimestre3.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 30).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 31).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre3.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 32).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 33).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre3.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 34).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 35).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 4 Trimestre
                        item.Trimestre4.Id = trimestre4;
                        item.Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                        #region Mes1
                        item.Trimestre4.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 36).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 37).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre4.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 38).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 39).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre4.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 40).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 41).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        if (ErroLinha)
                            throw new ArgumentException("Existem valores ou quantidade preenchidos incorretamente, verifique a linha " + row.RowNumber().ToString());

                        lstOrcamentoDetalhado.Add(item);
                        #endregion
                    }
                }

                return lstOrcamentoDetalhado;
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }

        }

        public void GerarPlanilhaOrcamento(Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Excel.IXLWorksheet ws)
        {
            int iLinha = 5;
            int zero = 0;

            foreach (OrcamentoDetalhado itemProd in lstOrcamentoDetalhado)
            {
                iLinha++;
                
                #region Gerando Colunas da planilha do excel
                #region Colunas de Unidade,Ano,Segmento,Familia,SubFamilia,Produtos
                ws.Cell(iLinha + 1, 1).Value = itemProd.Ano;

                ws.Cell(iLinha + 1, 2).Value = itemProd.UnidadeNegocio.Name;

                ws.Cell(iLinha + 1, 3).Value = itemProd.Segmento.Name;

                ws.Cell(iLinha + 1, 4).Value = itemProd.Familia.Name;

                ws.Cell(iLinha + 1, 5).Value = itemProd.SubFamilia.Name;

                ws.Cell(iLinha + 1, 6).Value = itemProd.StatusProduto;

                ws.Cell(iLinha + 1, 7).Value = itemProd.Product == null ? "" : itemProd.Product.Codigo;

                ws.Cell(iLinha + 1, 8).Value = itemProd.Produto.Name;

                ws.Cell(iLinha + 1, 9).Value = itemProd.StatusParticipacao;

                ws.Cell(iLinha + 1, 10).Value = itemProd.Account == null ? "" : itemProd.Account.CodigoMatriz;

                ws.Cell(iLinha + 1, 11).Value = itemProd.Account == null ? "" : itemProd.Account.CpfCnpj;

                ws.Cell(iLinha + 1, 12).Value = itemProd.Canal.Name;
                #endregion
                #region Colunas com os id's de controle
                ws.Cell(iLinha + 1, 13).Value = itemProd.Canal.Id.ToString();

                ws.Cell(iLinha + 1, 14).Value = itemProd.Produto.Id.ToString();

                ws.Cell(iLinha + 1, 15).Value = itemProd.SubFamilia.Id.ToString();

                ws.Cell(iLinha + 1, 16).Value = itemProd.Familia.Id.ToString();

                ws.Cell(iLinha + 1, 17).Value = itemProd.Segmento.Id.ToString();
                #endregion
                #region Valores e Quantidades se ja existe o item
                if (itemProd.Trimestre1 != null)
                {
                    #region Janeiro
                    ws.Cell(iLinha + 1, 18).Value = itemProd.Trimestre1.Mes1Vlr.HasValue ? itemProd.Trimestre1.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 19).Value = itemProd.Trimestre1.Mes1Qtde.HasValue ? itemProd.Trimestre1.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Fevereiro
                    ws.Cell(iLinha + 1, 20).Value = itemProd.Trimestre1.Mes2Vlr.HasValue ? itemProd.Trimestre1.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 21).Value = itemProd.Trimestre1.Mes2Qtde.HasValue ? itemProd.Trimestre1.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Março
                    ws.Cell(iLinha + 1, 22).Value = itemProd.Trimestre1.Mes3Vlr.HasValue ? itemProd.Trimestre1.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 23).Value = itemProd.Trimestre1.Mes3Qtde.HasValue ? itemProd.Trimestre1.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                if (itemProd.Trimestre2 != null)
                {
                    #region Abril
                    ws.Cell(iLinha + 1, 24).Value = itemProd.Trimestre2.Mes1Vlr.HasValue ? itemProd.Trimestre2.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 25).Value = itemProd.Trimestre2.Mes1Qtde.HasValue ? itemProd.Trimestre2.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Maio
                    ws.Cell(iLinha + 1, 26).Value = itemProd.Trimestre2.Mes2Vlr.HasValue ? itemProd.Trimestre2.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 27).Value = itemProd.Trimestre2.Mes2Qtde.HasValue ? itemProd.Trimestre2.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Junho
                    ws.Cell(iLinha + 1, 28).Value = itemProd.Trimestre2.Mes3Vlr.HasValue ? itemProd.Trimestre2.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 29).Value = itemProd.Trimestre2.Mes3Qtde.HasValue ? itemProd.Trimestre2.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                if (itemProd.Trimestre3 != null)
                {
                    #region Julho
                    ws.Cell(iLinha + 1, 30).Value = itemProd.Trimestre3.Mes1Vlr.HasValue ? itemProd.Trimestre3.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 31).Value = itemProd.Trimestre3.Mes1Qtde.HasValue ? itemProd.Trimestre3.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Agosto
                    ws.Cell(iLinha + 1, 32).Value = itemProd.Trimestre3.Mes2Vlr.HasValue ? itemProd.Trimestre3.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 33).Value = itemProd.Trimestre3.Mes2Qtde.HasValue ? itemProd.Trimestre3.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Setembro
                    ws.Cell(iLinha + 1, 34).Value = itemProd.Trimestre3.Mes3Vlr.HasValue ? itemProd.Trimestre3.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 35).Value = itemProd.Trimestre3.Mes3Qtde.HasValue ? itemProd.Trimestre3.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                if (itemProd.Trimestre4 != null)
                {
                    #region Outubro
                    ws.Cell(iLinha + 1, 36).Value = itemProd.Trimestre4.Mes1Vlr.HasValue ? itemProd.Trimestre4.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 37).Value = itemProd.Trimestre4.Mes1Qtde.HasValue ? itemProd.Trimestre4.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Novembro
                    ws.Cell(iLinha + 1, 38).Value = itemProd.Trimestre4.Mes2Vlr.HasValue ? itemProd.Trimestre4.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 39).Value = itemProd.Trimestre4.Mes2Qtde.HasValue ? itemProd.Trimestre4.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Dezembro
                    ws.Cell(iLinha + 1, 40).Value = itemProd.Trimestre4.Mes3Vlr.HasValue ? itemProd.Trimestre4.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 41).Value = itemProd.Trimestre4.Mes3Qtde.HasValue ? itemProd.Trimestre4.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                #endregion
                #endregion
            }

        }
        #endregion

        #region Orçamento Manual
        public List<Model.OrcamentoDetalhado> lerPlanilhaOrcamentoManual(DateTime datearquivo, string arquivo)
        {
            #region váriaveis e objetos
            object ValorExcel;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();
            OrcamentoDetalhado item;
            Guid trimestre1 = Guid.Empty;
            Guid trimestre2 = Guid.Empty;
            Guid trimestre3 = Guid.Empty;
            Guid trimestre4 = Guid.Empty;

            Excel.XLWorkbook xlWorkbook; 

            Decimal dVlrExcel = 0;
            bool bdVlrExcel;
            #endregion
            List<string> lstDescricaoItens = new List<string>();
            bool ErroLinha = false;

            try
            {
                xlWorkbook = new Excel.XLWorkbook(arquivo); 
            }
            catch (Exception erroopenexcel)
            {
                throw new ArgumentException("Erro ao abrir o arquivo:" + arquivo + "  Erro:" + erroopenexcel.Message);
            }

            try
            {
                Excel.IXLWorksheet xlWorksheet = xlWorkbook.Worksheet(1);
                Excel.IXLRange xlRange = xlWorksheet.RangeUsed();

                foreach (Excel.IXLRangeRow row in xlRange.Rows())
                {
                    if (row.RowNumber() == 4)
                    {
                        #region get id of Trimetres
                        // {6,9,12,15} orcamento
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 8).Value;
                        if (ValorExcel != null)
                            trimestre1 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 11).Value;
                        if (ValorExcel != null)
                            trimestre2 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 14).Value;
                        if (ValorExcel != null)
                            trimestre3 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 17).Value;
                        if (ValorExcel != null)
                            trimestre4 = Guid.Parse(ValorExcel.ToString());
                        #endregion
                    }
                    if (row.RowNumber() > 6)
                    {
                        #region
                        item = new OrcamentoDetalhado();
                        item.Trimestre1 = new Trimestre();
                        item.Trimestre2 = new Trimestre();
                        item.Trimestre3 = new Trimestre();
                        item.Trimestre4 = new Trimestre();

                        item.Ano = Convert.ToInt32(xlWorksheet.Cell(row.RowNumber(), 1).Value);
                        item.DatadoArquivo = datearquivo;

                        item.CanalID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 5).Value.ToString());

                        #region 1 Trimestre
                        item.Trimestre1.Id = trimestre1;
                        item.Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                        #region Mes1
                        item.Trimestre1.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 8).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre1.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre1.Mes1Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre1.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 9).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre1.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre1.Mes2Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre1.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 10).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre1.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre1.Mes3Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 2 Trimestre
                        item.Trimestre2.Id = trimestre2;
                        item.Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                        #region Mes1
                        item.Trimestre2.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 11).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre2.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre2.Mes1Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre2.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 12).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre2.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre2.Mes2Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre2.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 13).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre2.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre2.Mes3Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 3 Trimestre
                        item.Trimestre3.Id = trimestre3;
                        item.Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                        #region Mes1
                        item.Trimestre3.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 14).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre3.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre3.Mes1Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre3.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 15).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre3.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre3.Mes2Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre3.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 16).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre3.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre3.Mes3Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 4 Trimestre
                        item.Trimestre4.Id = trimestre4;
                        item.Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                        #region Mes1
                        item.Trimestre4.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 17).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre4.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre4.Mes1Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre4.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre4.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre4.Mes2Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre4.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 19).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                    item.Trimestre4.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                else
                                    item.Trimestre4.Mes3Vlr = 0;
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        if (ErroLinha)
                            throw new ArgumentException("Existem valores ou quantidade preenchidos incorretamente, verifique a linha " + row.RowNumber().ToString());

                        lstOrcamentoDetalhado.Add(item);
                        #endregion
                    }
                }

                return lstOrcamentoDetalhado;
            }
            catch (Exception erro)
            {
                throw new ArgumentException(string.Concat("Ocorreu erro ao ler a planilha:", erro.Message), erro);
            }

        }

        public void GerarPlanilhaManualOrcamento(Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Excel.IXLWorksheet ws)
        {
            int iLinha = 5;
            int zero = 0;

            foreach (OrcamentoDetalhado itemProd in lstOrcamentoDetalhado)
            {
                iLinha++;
               
                #region Gerando Colunas da planilha do excel
                ws.Cell(iLinha + 1, 1).Value = itemProd.Ano;

                ws.Cell(iLinha + 1, 2).Value = itemProd.UnidadeNegocio.Name;

                ws.Cell(iLinha + 1, 3).Value = itemProd.StatusParticipacao == string.Empty ? (itemProd.Account.ParticipantePrograma.HasValue ? (itemProd.Account.ParticipantePrograma.Value == 993520001 ? "Sim" : (itemProd.Account.ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "") : itemProd.StatusParticipacao;
                //Linha.Value2 = itemProd.StatusParticipacao;
                //= prodCanal.First().ParticipantePrograma.HasValue ? (prodCanal.First().ParticipantePrograma.Value == 993520001 ? "Sim" : (prodCanal.First().ParticipantePrograma.Value == 993520000 ? "Não" : "Descredenciado")) : "";

                ws.Cell(iLinha + 1, 4).Value = itemProd.Account == null ? "" : itemProd.Account.CodigoMatriz;

                ws.Cell(iLinha + 1, 5).Value = itemProd.Account == null ? "" : itemProd.Account.CpfCnpj;

                ws.Cell(iLinha + 1, 6).Value = itemProd.Account == null ? "" : itemProd.Account.NomeFantasia;// itemProd.Canal.Name;

                ws.Cell(iLinha + 1, 7).Value = itemProd.Canal.Id.ToString();
                #endregion
                #region Valores e Quantidades se ja existe o item
                if (itemProd.Trimestre1 != null)
                {
                    #region Janeiro
                    ws.Cell(iLinha + 1, 8).Value = itemProd.Trimestre1.Mes1Vlr.HasValue ? itemProd.Trimestre1.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Fevereiro
                    ws.Cell(iLinha + 1, 9).Value = itemProd.Trimestre1.Mes2Vlr.HasValue ? itemProd.Trimestre1.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Março
                    ws.Cell(iLinha + 1, 10).Value = itemProd.Trimestre1.Mes3Vlr.HasValue ? itemProd.Trimestre1.Mes3Vlr.Value.ToString("c") : "0";//Valor
                    #endregion
                }
                if (itemProd.Trimestre2 != null)
                {
                    #region Abril
                    ws.Cell(iLinha + 1, 11).Value = itemProd.Trimestre2.Mes1Vlr.HasValue ? itemProd.Trimestre2.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Maio
                    ws.Cell(iLinha + 1, 12).Value = itemProd.Trimestre2.Mes2Vlr.HasValue ? itemProd.Trimestre2.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Junho
                    ws.Cell(iLinha + 1, 13).Value = itemProd.Trimestre2.Mes3Vlr.HasValue ? itemProd.Trimestre2.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion
                }

                if (itemProd.Trimestre3 != null)
                {
                    #region Julho
                    ws.Cell(iLinha + 1, 14).Value = itemProd.Trimestre3.Mes1Vlr.HasValue ? itemProd.Trimestre3.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Agosto
                    ws.Cell(iLinha + 1, 15).Value = itemProd.Trimestre3.Mes2Vlr.HasValue ? itemProd.Trimestre3.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Setembro
                    ws.Cell(iLinha + 1, 16).Value = itemProd.Trimestre3.Mes3Vlr.HasValue ? itemProd.Trimestre3.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion
                }

                if (itemProd.Trimestre4 != null)
                {
                    #region Outubro
                    ws.Cell(iLinha + 1, 17).Value = itemProd.Trimestre4.Mes1Vlr.HasValue ? itemProd.Trimestre4.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Novembro
                    ws.Cell(iLinha + 1, 18).Value = itemProd.Trimestre4.Mes2Vlr.HasValue ? itemProd.Trimestre4.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion

                    #region Dezembro
                    ws.Cell(iLinha + 1, 19).Value = itemProd.Trimestre4.Mes3Vlr.HasValue ? itemProd.Trimestre4.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//valor
                    #endregion
                }

                #endregion
            }

        }
        #endregion

        #endregion

        #region Métodos de Geração/Leitura de Planilha de Metas

        #region Metas Resumida/Detalhada
        public List<Model.OrcamentoDetalhado> lerPlanilhaMetas(DateTime datearquivo, string arquivo)
        {
            #region váriaveis e objetos
            object ValorExcel;
            object QtdeExcel;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();
            OrcamentoDetalhado item;
            Guid trimestre1 = Guid.Empty;
            Guid trimestre2 = Guid.Empty;
            Guid trimestre3 = Guid.Empty;
            Guid trimestre4 = Guid.Empty;
            Excel.XLWorkbook xlWorkbook;

            Decimal dVlrExcel = 0;
            int iQtdeExcel = 0;
            bool bdVlrExcel;
            bool diQtdeExcel;
            #endregion
            List<string> lstDescricaoItens = new List<string>();
            bool ErroLinha = false;

            try
            {
                xlWorkbook = new Excel.XLWorkbook(arquivo);
            }
            catch (Exception erroopenexcel)
            {
                throw new ArgumentException("Erro ao abrir o arquivo:" + arquivo + "  Erro:" + erroopenexcel.Message);
            }

            try
            {
                Excel.IXLWorksheet xlWorksheet = xlWorkbook.Worksheet(1);
                Excel.IXLRange xlRange = xlWorksheet.RangeUsed();

                foreach (Excel.IXLRangeRow row in xlRange.Rows())
                {
                    if (row.RowNumber() == 4)
                    {
                        #region get id of Trimetres
                        //tipoarquivo == 2 {15,27,39,51} meta
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                        if (ValorExcel != null)
                            trimestre1 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 30).Value;
                        if (ValorExcel != null)
                            trimestre2 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 42).Value;
                        if (ValorExcel != null)
                            trimestre3 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 54).Value;
                        if (ValorExcel != null)
                            trimestre4 = Guid.Parse(ValorExcel.ToString());
                        #endregion
                    }
                    if (row.RowNumber() > 6)
                    {
                        #region
                        item = new OrcamentoDetalhado();
                        item.Trimestre1 = new Trimestre();
                        item.Trimestre2 = new Trimestre();
                        item.Trimestre3 = new Trimestre();
                        item.Trimestre4 = new Trimestre();
                        
                        item.Ano = Convert.ToInt32(xlWorksheet.Cell(row.RowNumber(), 1).Value);
                        item.DatadoArquivo = datearquivo;
                        #region propertys de controle
                        item.SegmentoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 17).Value.ToString());
                        item.FamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 16).Value.ToString());
                        item.SubFamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 15).Value.ToString());
                        item.ProdutoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 14).Value.ToString());
                        item.CanalID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 13).Value.ToString());
                        #endregion

                        #region 1 Trimestre
                        item.Trimestre1.Id = trimestre1;
                        item.Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                        #region Mes1
                        item.Trimestre1.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 19).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre1.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 22).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 23).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre1.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 26).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 27).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre1.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 2 Trimestre
                        item.Trimestre2.Id = trimestre2;
                        item.Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                        #region Mes1
                        item.Trimestre2.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 30).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 31).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre2.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 34).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 35).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre2.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 38).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 39).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre2.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 3 Trimestre
                        item.Trimestre3.Id = trimestre3;
                        item.Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                        #region Mes1
                        item.Trimestre3.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 42).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 43).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre3.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 46).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 47).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre3.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 50).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 51).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre3.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 4 Trimestre
                        item.Trimestre4.Id = trimestre4;
                        item.Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                        #region Mes1
                        item.Trimestre4.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 54).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 55).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre4.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 58).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 59).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre4.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 62).Value;
                        ValorExcel = ValorExcel == null ? 0 : ValorExcel;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel >= 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 63).Value;
                                    QtdeExcel = QtdeExcel == null ? 0 : QtdeExcel;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel >= 0)
                                            {
                                                item.Trimestre4.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        if (ErroLinha)
                            throw new ArgumentException("Existem valores ou quantidade preenchidos incorretamente, verifique a linha  " + row.RowNumber().ToString());

                        lstOrcamentoDetalhado.Add(item);
                        #endregion
                    }
                }

                return lstOrcamentoDetalhado;
            }
            catch (Exception erro)
            {

                throw new ArgumentException(erro.Message);
            }

        }

        public void GerarPlanilhaMetas(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstMetaDetalhado, Excel.IXLWorksheet ws)
        {
            int iLinha = 5;
            int zero = 0;
            List<OrcamentodaUnidadeDetalhadoporProduto> lstOrcamentodaUnidadeDetalhadoporProduto;
            List<OrcamentodoCanalDetalhadoporProduto> lstOrcamentodoCanalDetalhadoporProduto;
            OrcamentodaUnidade mOrcamentodaUnidade = null;
            if (mMetadaUnidade.OrcamentodaUnidade != null)
                mOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.Retrieve(mMetadaUnidade.OrcamentodaUnidade.Id);

            foreach (OrcamentoDetalhado itemProd in lstMetaDetalhado)
            {
                iLinha++;
                
                #region Gerando Colunas da planilha do excel
                #region Colunas de Unidade,Ano,Segmento,Familia,SubFamilia,Produtos
                ws.Cell(iLinha + 1, 1).Value = itemProd.Ano;

                ws.Cell(iLinha + 1, 2).Value = itemProd.UnidadeNegocio.Name;

                ws.Cell(iLinha + 1, 3).Value = itemProd.Segmento.Name;

                ws.Cell(iLinha + 1, 4).Value = itemProd.Familia.Name;

                ws.Cell(iLinha + 1, 5).Value = itemProd.SubFamilia.Name;

                ws.Cell(iLinha + 1, 6).Value = itemProd.StatusProduto;

                ws.Cell(iLinha + 1, 7).Value = itemProd.Product == null ? "" : itemProd.Product.Codigo;

                ws.Cell(iLinha + 1, 8).Value = itemProd.Produto.Name;

                ws.Cell(iLinha + 1, 9).Value = itemProd.StatusParticipacao;

                ws.Cell(iLinha + 1, 10).Value = itemProd.Account == null ? "" : itemProd.Account.CpfCnpj;

                ws.Cell(iLinha + 1, 11).Value = itemProd.Account == null ? "" : itemProd.Account.CodigoMatriz;

                ws.Cell(iLinha + 1, 12).Value = itemProd.Canal.Name;
                #endregion
                #region Colunas com os id's de controle
                ws.Cell(iLinha + 1, 13).Value = itemProd.Canal.Id.ToString();

                ws.Cell(iLinha + 1, 14).Value = itemProd.Produto.Id.ToString();

                ws.Cell(iLinha + 1, 15).Value = itemProd.SubFamilia.Id.ToString();

                ws.Cell(iLinha + 1, 16).Value = itemProd.Familia.Id.ToString();

                ws.Cell(iLinha + 1, 17).Value = itemProd.Segmento.Id.ToString();
                #endregion
                #region caso tenha orçamento obtem os valores do orçamento
                if (mOrcamentodaUnidade != null)
                {
                    if (mOrcamentodaUnidade.NiveldoOrcamento == (int)Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                    {
                        lstOrcamentodoCanalDetalhadoporProduto = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ListarDetalheProdutosPorCanal(mMetadaUnidade.OrcamentodaUnidade.Id, itemProd.Canal.Id, itemProd.Produto.Id);
                        if (lstOrcamentodoCanalDetalhadoporProduto != null)
                        {
                            #region
                            foreach (OrcamentodoCanalDetalhadoporProduto item in lstOrcamentodoCanalDetalhadoporProduto)
                            {
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Janeiro)
                                {
                                    #region Janeiro
                                    ws.Cell(iLinha + 1, 20).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 21).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Fevereiro)
                                {
                                    #region Fevereiro
                                    ws.Cell(iLinha + 1, 24).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 25).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Marco)
                                {
                                    #region Março
                                    ws.Cell(iLinha + 1, 28).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 29).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Abril)
                                {
                                    #region Abril
                                    ws.Cell(iLinha + 1, 32).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 33).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Maio)
                                {
                                    #region Maio
                                    ws.Cell(iLinha + 1, 36).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 37).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Junho)
                                {
                                    #region Junho
                                    ws.Cell(iLinha + 1, 40).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 41).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Julho)
                                {
                                    #region Julho
                                    ws.Cell(iLinha + 1, 44).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 45).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Agosto)
                                {
                                    #region Agosto
                                    ws.Cell(iLinha + 1, 48).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 49).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Setembro)
                                {
                                    #region Setembro
                                    ws.Cell(iLinha + 1, 52).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 53).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Outubro)
                                {
                                    #region Outubro
                                    ws.Cell(iLinha + 1, 56).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 57).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Novembro)
                                {
                                    #region Novembro
                                    ws.Cell(iLinha + 1, 60).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 61).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Dezembro)
                                {
                                    #region Dezembro
                                    ws.Cell(iLinha + 1, 64).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 65).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        lstOrcamentodaUnidadeDetalhadoporProduto = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ObterOrcDetalhadoProdutos(itemProd.Produto.Id, mMetadaUnidade.OrcamentodaUnidade.Id);
                        if (lstOrcamentodaUnidadeDetalhadoporProduto != null)
                        {
                            #region
                            foreach (OrcamentodaUnidadeDetalhadoporProduto item in lstOrcamentodaUnidadeDetalhadoporProduto)
                            {
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Janeiro)
                                {
                                    #region Janeiro
                                    ws.Cell(iLinha + 1, 20).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 21).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Fevereiro)
                                {
                                    #region Fevereiro
                                    ws.Cell(iLinha + 1, 24).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 25).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Marco)
                                {
                                    #region Março
                                    ws.Cell(iLinha + 1, 28).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 29).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Abril)
                                {
                                    #region Abril
                                    ws.Cell(iLinha + 1, 32).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 33).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Maio)
                                {
                                    #region Maio
                                    ws.Cell(iLinha + 1, 36).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 37).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Junho)
                                {
                                    #region Junho
                                    ws.Cell(iLinha + 1, 40).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 41).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Julho)
                                {
                                    #region Julho
                                    ws.Cell(iLinha + 1, 44).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 45).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Agosto)
                                {
                                    #region Agosto
                                    ws.Cell(iLinha + 1, 48).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 49).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Setembro)
                                {
                                    #region Setembro
                                    ws.Cell(iLinha + 1, 52).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 53).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Outubro)
                                {
                                    #region Outubro
                                    ws.Cell(iLinha + 1, 56).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 57).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Novembro)
                                {
                                    #region Novembro
                                    ws.Cell(iLinha + 1, 60).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 61).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                                if (item.Mes.Value == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Dezembro)
                                {
                                    #region Dezembro
                                    ws.Cell(iLinha + 1, 64).Value = item.OrcamentoPlanejado.HasValue ? item.OrcamentoPlanejado.Value.ToString("c") : zero.ToString("c");//Valor
                                    ws.Cell(iLinha + 1, 65).Value = item.QtdePlanejada.HasValue ? item.QtdePlanejada.Value : 0;//Quantidade
                                    #endregion
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                #region Valores e Quantidades se ja existe o item
                if (itemProd.Trimestre1 != null)
                {
                    #region Janeiro
                    ws.Cell(iLinha + 1, 18).Value = itemProd.Trimestre1.Mes1Vlr.HasValue ? itemProd.Trimestre1.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 19).Value = itemProd.Trimestre1.Mes1Qtde.HasValue ? itemProd.Trimestre1.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Fevereiro
                    ws.Cell(iLinha + 1, 22).Value = itemProd.Trimestre1.Mes2Vlr.HasValue ? itemProd.Trimestre1.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 23).Value = itemProd.Trimestre1.Mes2Qtde.HasValue ? itemProd.Trimestre1.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Março
                    ws.Cell(iLinha + 1, 26).Value = itemProd.Trimestre1.Mes3Vlr.HasValue ? itemProd.Trimestre1.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 27).Value = itemProd.Trimestre1.Mes3Qtde.HasValue ? itemProd.Trimestre1.Mes3Qtde.Value : 0;//Quantidade
                    #endregion

                }
                if (itemProd.Trimestre2 != null)
                {
                    #region Abril
                    ws.Cell(iLinha + 1, 30).Value = itemProd.Trimestre2.Mes1Vlr.HasValue ? itemProd.Trimestre2.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 31).Value = itemProd.Trimestre2.Mes1Qtde.HasValue ? itemProd.Trimestre2.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Maio
                    ws.Cell(iLinha + 1, 34).Value = itemProd.Trimestre2.Mes2Vlr.HasValue ? itemProd.Trimestre2.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 35).Value = itemProd.Trimestre2.Mes2Qtde.HasValue ? itemProd.Trimestre2.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Junho
                    ws.Cell(iLinha + 1, 38).Value = itemProd.Trimestre2.Mes3Vlr.HasValue ? itemProd.Trimestre2.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 39).Value = itemProd.Trimestre2.Mes3Qtde.HasValue ? itemProd.Trimestre2.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                if (itemProd.Trimestre3 != null)
                {
                    #region Julho
                    ws.Cell(iLinha + 1, 42).Value = itemProd.Trimestre3.Mes1Vlr.HasValue ? itemProd.Trimestre3.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 43).Value = itemProd.Trimestre3.Mes1Qtde.HasValue ? itemProd.Trimestre3.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Agosto
                    ws.Cell(iLinha + 1, 46).Value = itemProd.Trimestre3.Mes2Vlr.HasValue ? itemProd.Trimestre3.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 47).Value = itemProd.Trimestre3.Mes2Qtde.HasValue ? itemProd.Trimestre3.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Setembro
                    ws.Cell(iLinha + 1, 50).Value = itemProd.Trimestre3.Mes3Vlr.HasValue ? itemProd.Trimestre3.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 51).Value = itemProd.Trimestre3.Mes3Qtde.HasValue ? itemProd.Trimestre3.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                if (itemProd.Trimestre4 != null)
                {
                    #region Outubro
                    ws.Cell(iLinha + 1, 54).Value = itemProd.Trimestre4.Mes1Vlr.HasValue ? itemProd.Trimestre4.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 55).Value = itemProd.Trimestre4.Mes1Qtde.HasValue ? itemProd.Trimestre4.Mes1Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Novembro
                    ws.Cell(iLinha + 1, 58).Value = itemProd.Trimestre4.Mes2Vlr.HasValue ? itemProd.Trimestre4.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 59).Value = itemProd.Trimestre4.Mes2Qtde.HasValue ? itemProd.Trimestre4.Mes2Qtde.Value : 0;//Quantidade
                    #endregion

                    #region Dezembro
                    ws.Cell(iLinha + 1, 62).Value = itemProd.Trimestre4.Mes3Vlr.HasValue ? itemProd.Trimestre4.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    ws.Cell(iLinha + 1, 63).Value = itemProd.Trimestre4.Mes3Qtde.HasValue ? itemProd.Trimestre4.Mes3Qtde.Value : 0;//Quantidade
                    #endregion
                }
                #endregion
                #endregion
            }

        }
        #endregion

        #region Meta Manual
        public List<Model.OrcamentoDetalhado> lerPlanilhaMetasManual(MetadaUnidade mMetadaUnidade, DateTime datearquivo, string arquivo)
        {
            #region váriaveis e objetos
            object ValorExcel;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();
            OrcamentoDetalhado item;
            Guid trimestre1 = Guid.Empty;
            Guid trimestre2 = Guid.Empty;
            Guid trimestre3 = Guid.Empty;
            Guid trimestre4 = Guid.Empty;

            Excel.XLWorkbook workbook = null;
           
            Decimal dVlrExcel = 0;
            bool bdVlrExcel;

            List<string> lstDescricaoItens = new List<string>();
            bool ErroLinha = false;
            #endregion

            try
            {
                workbook = new Excel.XLWorkbook(arquivo);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Erro ao abrir o arquivo: " + arquivo, ex);
            }

            try
            {
                Excel.IXLWorksheet worksheet = workbook.Worksheet(1);
                Excel.IXLRange xlRange = worksheet.RangeUsed();

                foreach (Excel.IXLRangeRow row in xlRange.Rows())
                {
                    if (row.RowNumber() == 4)
                    {
                        #region get id of Trimetres
                        //tipoarquivo == 2 {6,9,12,15} meta
                        ValorExcel = worksheet.Cell(row.RowNumber(), 8).Value;
                        if (ValorExcel != null)
                            trimestre1 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = worksheet.Cell(row.RowNumber(), 11).Value;
                        if (ValorExcel != null)
                            trimestre2 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = worksheet.Cell(row.RowNumber(), 14).Value;
                        if (ValorExcel != null)
                            trimestre3 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = worksheet.Cell(row.RowNumber(), 17).Value;
                        if (ValorExcel != null)
                            trimestre4 = Guid.Parse(ValorExcel.ToString());
                        #endregion
                    }
                    if (row.RowNumber() > 6)
                    {
                        try
                        {
                            #region
                            item = new OrcamentoDetalhado();
                            item.Trimestre1 = new Trimestre();
                            item.Trimestre2 = new Trimestre();
                            item.Trimestre3 = new Trimestre();
                            item.Trimestre4 = new Trimestre();

                            item.Ano = mMetadaUnidade.Ano.Value;
                            item.DatadoArquivo = datearquivo;
                            item.CanalID = Guid.Parse(worksheet.Cell(row.RowNumber(), 7).Value.ToString());

                            #region 1 Trimestre
                            item.Trimestre1.Id = trimestre1;
                            item.Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                            #region mes1
                            item.Trimestre1.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 8).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre1.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre1.Mes1Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes2
                            item.Trimestre1.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 9).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre1.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre1.Mes2Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes3
                            item.Trimestre1.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 10).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre1.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre1.Mes3Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion
                            #endregion

                            #region 2 Trimestre
                            item.Trimestre2.Id = trimestre2;
                            item.Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                            #region mes1
                            item.Trimestre2.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 11).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre2.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre2.Mes1Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes2
                            item.Trimestre2.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 12).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre2.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre2.Mes2Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes3
                            item.Trimestre2.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 13).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre2.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre2.Mes3Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion
                            #endregion

                            #region 3 Trimestre
                            item.Trimestre3.Id = trimestre3;
                            item.Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                            #region mes1
                            item.Trimestre3.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 14).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre3.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre3.Mes1Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes2
                            item.Trimestre3.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 15).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre3.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre3.Mes2Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes3
                            item.Trimestre3.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 16).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre3.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre3.Mes3Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion
                            #endregion

                            #region 4 Trimestre
                            item.Trimestre4.Id = trimestre4;
                            item.Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                            #region mes1
                            item.Trimestre4.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 17).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre4.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre4.Mes1Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes2
                            item.Trimestre4.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 18).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre4.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre4.Mes2Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion

                            #region mes3
                            item.Trimestre4.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                            ValorExcel = worksheet.Cell(row.RowNumber(), 19).Value;
                            if (ValorExcel != null)
                            {
                                bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                                if (bdVlrExcel)
                                {
                                    if (dVlrExcel > 0)
                                        item.Trimestre4.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                    else
                                        item.Trimestre4.Mes3Vlr = 0;
                                }
                                else
                                    ErroLinha = true;
                            }
                            #endregion
                            #endregion

                            if (ErroLinha)
                                throw new ArgumentException("Existem valores ou quantidade preenchidos incorretamente, verifique a linha " + row.RowNumber().ToString());

                            lstOrcamentoDetalhado.Add(item);
                            #endregion
                        }
                        catch { }
                    }
                }

                return lstOrcamentoDetalhado;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook = null;
                }
            }

        }

        public void GerarPlanilhaManualMetas(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstMetaDetalhado, Excel.IXLWorksheet ws)
        {
            int iLinha = 5;
            int zero = 0;

            foreach (OrcamentoDetalhado itemProd in lstMetaDetalhado)
            {
                iLinha++;

                #region Gerando Colunas da planilha do excel
                ws.Cell(iLinha + 1, 1).Value = itemProd.Ano;

                ws.Cell(iLinha + 1, 2).Value = itemProd.UnidadeNegocio.Name;

                ws.Cell(iLinha + 1, 3).Value = itemProd.StatusParticipacao;

                ws.Cell(iLinha + 1, 4).Value = itemProd.Account == null ? "" : itemProd.Account.CpfCnpj;

                ws.Cell(iLinha + 1, 5).Value = itemProd.Account == null ? "" : itemProd.Account.CodigoMatriz;

                ws.Cell(iLinha + 1, 6).Value = itemProd.Canal.Name;

                ws.Cell(iLinha + 1, 7).Value = itemProd.Canal.Id.ToString();
                #endregion
                #region Valores e Quantidades se ja existe o item
                if (itemProd.Trimestre1 != null)
                {
                    #region Janeiro
                    ws.Cell(iLinha + 1, 8).Value = itemProd.Trimestre1.Mes1Vlr.HasValue ? itemProd.Trimestre1.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Fevereiro
                    ws.Cell(iLinha + 1, 9).Value = itemProd.Trimestre1.Mes2Vlr.HasValue ? itemProd.Trimestre1.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Março
                    ws.Cell(iLinha + 1, 10).Value = itemProd.Trimestre1.Mes3Vlr.HasValue ? itemProd.Trimestre1.Mes3Vlr.Value.ToString("c") : zero.ToString("c");
                    #endregion
                }

                if (itemProd.Trimestre2 != null)
                {
                    #region Abril
                    ws.Cell(iLinha + 1, 11).Value = itemProd.Trimestre2.Mes1Vlr.HasValue ? itemProd.Trimestre2.Mes1Vlr.Value.ToString("c") : zero.ToString("c");
                    #endregion

                    #region Maio
                    ws.Cell(iLinha + 1, 12).Value = itemProd.Trimestre2.Mes2Vlr.HasValue ? itemProd.Trimestre2.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Junho
                    ws.Cell(iLinha + 1, 13).Value = itemProd.Trimestre2.Mes3Vlr.HasValue ? itemProd.Trimestre2.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion
                }

                if (itemProd.Trimestre3 != null)
                {
                    #region Julho
                    ws.Cell(iLinha + 1, 14).Value = itemProd.Trimestre3.Mes1Vlr.HasValue ? itemProd.Trimestre3.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Agosto
                    ws.Cell(iLinha + 1, 15).Value = itemProd.Trimestre3.Mes2Vlr.HasValue ? itemProd.Trimestre3.Mes2Vlr.Value.ToString("c") : zero.ToString("c");
                    #endregion

                    #region Setembro
                    ws.Cell(iLinha + 1, 16).Value = itemProd.Trimestre3.Mes3Vlr.HasValue ? itemProd.Trimestre3.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion
                }

                if (itemProd.Trimestre4 != null)
                {
                    #region Outubro
                    ws.Cell(iLinha + 1, 17).Value = itemProd.Trimestre4.Mes1Vlr.HasValue ? itemProd.Trimestre4.Mes1Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Novembro
                    ws.Cell(iLinha + 1, 18).Value = itemProd.Trimestre4.Mes2Vlr.HasValue ? itemProd.Trimestre4.Mes2Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion

                    #region Dezembro
                    ws.Cell(iLinha + 1, 19).Value = itemProd.Trimestre4.Mes3Vlr.HasValue ? itemProd.Trimestre4.Mes3Vlr.Value.ToString("c") : zero.ToString("c");//Valor
                    #endregion
                }
                #endregion
            }

        }
        #endregion

        #region Metas Supervisor
        public void GerarPlanilhaMetasSupervisor(Model.MetadaUnidade mMetadaUnidade, List<Model.OrcamentoDetalhado> lstMetaDetalhado, Excel.IXLWorksheet ws)
        {
            int iLinha = 5;
            int Zero = 0;

            foreach (OrcamentoDetalhado itemProd in lstMetaDetalhado)
            {
                iLinha++;

                #region Gerando Colunas da planilha do excel
                #region Colunas de Unidade,Ano,Segmento,Familia,SubFamilia,Produtos
                ws.Cell(iLinha + 1, 1).Value = itemProd.Ano;

                ws.Cell(iLinha + 1, 2).Value = itemProd.UnidadeNegocio.Name;

                ws.Cell(iLinha + 1, 3).Value = itemProd.Segmento.Name;

                ws.Cell(iLinha + 1, 4).Value = itemProd.Familia.Name;

                ws.Cell(iLinha + 1, 5).Value = itemProd.SubFamilia.Name;

                ws.Cell(iLinha + 1, 6).Value = itemProd.StatusProduto;

                ws.Cell(iLinha + 1, 7).Value = itemProd.Product == null ? "" : itemProd.Product.Codigo;

                ws.Cell(iLinha + 1, 8).Value = itemProd.Produto.Name;

                ws.Cell(iLinha + 1, 9).Value = itemProd.StatusParticipacao;

                ws.Cell(iLinha + 1, 10).Value = itemProd.User == null ? "" : itemProd.User.CodigoSupervisorEMS;

                ws.Cell(iLinha + 1, 11).Value = itemProd.Canal.Name;
                #endregion
                #region Colunas com os id's de controle
                ws.Cell(iLinha + 1, 12).Value = itemProd.Canal.Id.ToString();

                ws.Cell(iLinha + 1, 13).Value = itemProd.Produto.Id.ToString();

                ws.Cell(iLinha + 1, 14).Value = itemProd.SubFamilia.Id.ToString();

                ws.Cell(iLinha + 1, 15).Value = itemProd.Familia.Id.ToString();

                ws.Cell(iLinha + 1, 16).Value = itemProd.Segmento.Id.ToString();
                #endregion
                #region Valores e Quantidades se ja existe o item

                if (itemProd.Trimestre1 != null)
                {
                    #region Trimestre1
                    ws.Cell(iLinha + 1, 17).Value = itemProd.Trimestre1.Mes1Vlr.HasValue ? itemProd.Trimestre1.Mes1Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 18).Value = itemProd.Trimestre1.Mes1Qtde.HasValue ? itemProd.Trimestre1.Mes1Qtde : Zero;

                    ws.Cell(iLinha + 1, 19).Value = itemProd.Trimestre1.Mes2Vlr.HasValue ? itemProd.Trimestre1.Mes2Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 20).Value = itemProd.Trimestre1.Mes2Qtde.HasValue ? itemProd.Trimestre1.Mes2Qtde : Zero;

                    ws.Cell(iLinha + 1, 21).Value = itemProd.Trimestre1.Mes3Vlr.HasValue ? itemProd.Trimestre1.Mes3Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 22).Value = itemProd.Trimestre1.Mes3Qtde.HasValue ? itemProd.Trimestre1.Mes3Qtde : Zero;
                    #endregion
                }

                if (itemProd.Trimestre2 != null)
                {
                    #region Trimestre2
                    ws.Cell(iLinha + 1, 23).Value = itemProd.Trimestre2.Mes1Vlr.HasValue ? itemProd.Trimestre2.Mes1Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 24).Value = itemProd.Trimestre2.Mes1Qtde.HasValue ? itemProd.Trimestre2.Mes1Qtde : Zero;

                    ws.Cell(iLinha + 1, 25).Value = itemProd.Trimestre2.Mes2Vlr.HasValue ? itemProd.Trimestre2.Mes2Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 26).Value = itemProd.Trimestre2.Mes2Qtde.HasValue ? itemProd.Trimestre2.Mes2Qtde : Zero;

                    ws.Cell(iLinha + 1, 27).Value = itemProd.Trimestre2.Mes3Vlr.HasValue ? itemProd.Trimestre2.Mes3Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 28).Value = itemProd.Trimestre2.Mes3Qtde.HasValue ? itemProd.Trimestre2.Mes3Qtde.Value : Zero;
                    #endregion
                }

                if (itemProd.Trimestre3 != null)
                {
                    #region Trimestre3
                    ws.Cell(iLinha + 1, 29).Value = itemProd.Trimestre3.Mes1Vlr.HasValue ? itemProd.Trimestre3.Mes1Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 30).Value = itemProd.Trimestre3.Mes1Qtde.HasValue ? itemProd.Trimestre3.Mes1Qtde : Zero;

                    ws.Cell(iLinha + 1, 31).Value = itemProd.Trimestre3.Mes2Vlr.HasValue ? itemProd.Trimestre3.Mes2Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 32).Value = itemProd.Trimestre3.Mes2Qtde.HasValue ? itemProd.Trimestre3.Mes2Qtde : Zero;

                    ws.Cell(iLinha + 1, 33).Value = itemProd.Trimestre3.Mes3Vlr.HasValue ? itemProd.Trimestre3.Mes3Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 34).Value = itemProd.Trimestre3.Mes3Qtde.HasValue ? itemProd.Trimestre3.Mes3Qtde : Zero;
                    #endregion
                }

                if (itemProd.Trimestre4 != null)
                {
                    #region Trimestre4
                    ws.Cell(iLinha + 1, 35).Value = itemProd.Trimestre4.Mes1Vlr.HasValue ? itemProd.Trimestre4.Mes1Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 36).Value = itemProd.Trimestre4.Mes1Qtde.HasValue ? itemProd.Trimestre4.Mes1Qtde : Zero;

                    ws.Cell(iLinha + 1, 37).Value = itemProd.Trimestre4.Mes2Vlr.HasValue ? itemProd.Trimestre4.Mes2Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 38).Value = itemProd.Trimestre4.Mes2Qtde.HasValue ? itemProd.Trimestre4.Mes2Qtde : Zero;

                    ws.Cell(iLinha + 1, 39).Value = itemProd.Trimestre4.Mes3Vlr.HasValue ? itemProd.Trimestre4.Mes3Vlr.Value.ToString("c") : Zero.ToString("c");
                    ws.Cell(iLinha + 1, 40).Value = itemProd.Trimestre4.Mes3Qtde.HasValue ? itemProd.Trimestre4.Mes3Qtde : Zero;
                    #endregion
                }
                #endregion

                #endregion
            }

        }

        public List<Model.OrcamentoDetalhado> lerPlanilhaMetasSupervisor(DateTime datearquivo, string arquivo)
        {
            #region váriaveis e objetos
            object ValorExcel;
            object QtdeExcel;
            List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado = new List<OrcamentoDetalhado>();
            OrcamentoDetalhado item;
            Guid trimestre1 = Guid.Empty;
            Guid trimestre2 = Guid.Empty;
            Guid trimestre3 = Guid.Empty;
            Guid trimestre4 = Guid.Empty;
            Excel.XLWorkbook xlWorkbook;

            Decimal dVlrExcel = 0;
            int iQtdeExcel = 0;
            bool bdVlrExcel;
            bool diQtdeExcel;
            #endregion
            List<string> lstDescricaoItens = new List<string>();
            bool ErroLinha = false;

            try
            {
                xlWorkbook = new Excel.XLWorkbook(arquivo);
            }
            catch (Exception erroopenexcel)
            {
                throw new ArgumentException("Erro ao abrir o arquivo:" + arquivo + "  Erro:" + erroopenexcel.Message);
            }

            try
            {
                Excel.IXLWorksheet xlWorksheet = xlWorkbook.Worksheet(1);
                Excel.IXLRange xlRange = xlWorksheet.RangeUsed();

                foreach (Excel.IXLRangeRow row in xlRange.Rows())
                {
                    if (row.RowNumber() == 4)
                    {
                        #region get id of Trimetres
                        //tipoarquivo == 2 {15,27,39,51} meta
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 17).Value;
                        if (ValorExcel != null)
                            trimestre1 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 23).Value;
                        if (ValorExcel != null)
                            trimestre2 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 29).Value;
                        if (ValorExcel != null)
                            trimestre3 = Guid.Parse(ValorExcel.ToString());

                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 35).Value;
                        if (ValorExcel != null)
                            trimestre4 = Guid.Parse(ValorExcel.ToString());
                        #endregion
                    }
                    if (row.RowNumber() > 6)
                    {
                        #region
                        item = new OrcamentoDetalhado();
                        item.Trimestre1 = new Trimestre();
                        item.Trimestre2 = new Trimestre();
                        item.Trimestre3 = new Trimestre();
                        item.Trimestre4 = new Trimestre();

                        item.Ano = Convert.ToInt32(xlWorksheet.Cell(row.RowNumber(), 1).Value);
                        item.DatadoArquivo = datearquivo;
                        #region propertys de controle
                        item.SegmentoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 16).Value.ToString());
                        item.FamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 15).Value.ToString());
                        item.SubFamiliaID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 14).Value.ToString());
                        item.ProdutoID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 13).Value.ToString());
                        item.CanalID = Guid.Parse(xlWorksheet.Cell(row.RowNumber(), 12).Value.ToString());
                        #endregion

                        #region 1 Trimestre
                        item.Trimestre1.Id = trimestre1;
                        item.Trimestre1.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;

                        #region Mes1
                        item.Trimestre1.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 17).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 18).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre1.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre1.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 19).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 20).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre1.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre1.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 21).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 22).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre1.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre1.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 2 Trimestre
                        item.Trimestre2.Id = trimestre2;
                        item.Trimestre2.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;

                        #region Mes1
                        item.Trimestre2.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 23).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 24).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre2.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre2.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 25).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 26).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre2.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre2.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 27).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 28).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre2.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre2.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 3 Trimestre
                        item.Trimestre3.Id = trimestre3;
                        item.Trimestre3.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;

                        #region Mes1
                        item.Trimestre3.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 29).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 30).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre3.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre3.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 31).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 32).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre3.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre3.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 33).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 34).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre3.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre3.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        #region 4 Trimestre
                        item.Trimestre4.Id = trimestre4;
                        item.Trimestre4.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;

                        #region Mes1
                        item.Trimestre4.Mes1 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 35).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 36).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre4.Mes1Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes1Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes2
                        item.Trimestre4.Mes2 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 37).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 38).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre4.Mes2Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes2Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion

                        #region Mes3
                        item.Trimestre4.Mes3 = (int)Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro;
                        ValorExcel = xlWorksheet.Cell(row.RowNumber(), 39).Value;
                        if (ValorExcel != null)
                        {
                            bdVlrExcel = Decimal.TryParse(ValorExcel.ToString().Replace("R$ ", ""), out dVlrExcel);
                            if (bdVlrExcel)
                            {
                                if (dVlrExcel > 0)
                                {
                                    QtdeExcel = xlWorksheet.Cell(row.RowNumber(), 40).Value;
                                    if (QtdeExcel != null)
                                    {
                                        diQtdeExcel = int.TryParse(QtdeExcel.ToString(), out iQtdeExcel);
                                        if (diQtdeExcel)
                                        {
                                            if (iQtdeExcel > 0)
                                            {
                                                item.Trimestre4.Mes3Vlr = Convert.ToDecimal(ValorExcel.ToString().Replace("R$ ", ""));
                                                item.Trimestre4.Mes3Qtde = Convert.ToInt64(Convert.ToDecimal(QtdeExcel));
                                            }
                                        }
                                        else
                                            ErroLinha = true;
                                    }
                                }
                            }
                            else
                                ErroLinha = true;
                        }
                        #endregion
                        #endregion

                        if (ErroLinha)
                            throw new ArgumentException("Existem valores ou quantidade preenchidos incorretamente, verifique a linha " + row.RowNumber().ToString());

                        lstOrcamentoDetalhado.Add(item);
                        #endregion
                    }
                }

                return lstOrcamentoDetalhado;
            }
            catch (Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }

        }
        #endregion

        #endregion

        /*private void EncerrarExcel(ref Excel.Application app, ref Excel.Workbook workbook, ref Excel._Worksheet worksheet)
        {
            if (worksheet != null)
            {
                if (System.Runtime.InteropServices.Marshal.IsComObject(worksheet))
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(worksheet);
                }
            }

            if (workbook != null)
            {
                // Se a conexão já estiver fechada apresenta um exception
                try { workbook.Close(); }
                catch { }

                if (System.Runtime.InteropServices.Marshal.IsComObject(workbook))
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(workbook);
                }
            }

            if (app != null)
            {
                app.Quit();

                if (System.Runtime.InteropServices.Marshal.IsComObject(app))
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(app);
                }
            }

            workbook = null;
            app = null;
        }*/

        public bool ExcluirArquivo(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        public List<ModeloMetaKeyAccountViewModel> ConvertDadosMetaKeyAccount(string path, out List<string> listaErros)
        {
            List<ModeloMetaKeyAccountViewModel> lista = new List<ModeloMetaKeyAccountViewModel>();
            listaErros = new List<string>();
            
            using (Excel.XLWorkbook excel = new Excel.XLWorkbook(path))
            {
                var ws = excel.Worksheet(1);
                foreach (Excel.IXLRow row in ws.Rows())
                {
                    if (row.RowNumber() > 6)
                    {
                        var item = new ModeloMetaKeyAccountViewModel();
                        var i = 0;
                        // percorre as colunas dos meses
                        foreach (Excel.IXLCell cell in row.Cells("11,13,15,17,19,21,23,25,27,29,31,33"))
                        {
                            item.ListaProdutosMes[i] = new ModeloMetaProdutoMesViewModel();
                            item.ListaProdutosMes[i].Mes = Helper.ConvertToMes(i + 1);

                            try { item.ListaProdutosMes[i].Valor = Convert.ToDecimal(row.Cell(cell.Address.ColumnNumber).Value); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + cell.Address.ColumnNumber + "]"); }
                            try { item.ListaProdutosMes[i].Quantidade = Convert.ToDecimal(row.Cell(cell.Address.ColumnNumber + 1).Value); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + cell.Address.ColumnNumber + "]"); }

                            i++;
                        }

                        try { item.KeyAccount = new Lookup(new Guid(row.Cell(35).Value.ToString()), SDKore.Crm.Util.Utility.GetEntityName<Contato>()); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + 35 + "]"); }
                        try { item.Produto = new Lookup(new Guid(row.Cell(36).Value.ToString()), SDKore.Crm.Util.Utility.GetEntityName<Product>()); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + 36 + "]"); }

                        lista.Add(item);
                    }
                }
            }
            return lista;
        }

        public List<ModeloMetaDetalhadaClienteViewModel> ConvertDadosMetaDetalhadaCanal(string path, out List<string> listaErros)
        {
            List<ModeloMetaDetalhadaClienteViewModel> lista = new List<ModeloMetaDetalhadaClienteViewModel>();
            listaErros = new List<string>();
  
            using (Excel.XLWorkbook excel = new Excel.XLWorkbook(path))
            {
                var ws = excel.Worksheet(1);
                foreach (Excel.IXLRow row in ws.Rows())
                {
                    if (row.RowNumber() > 5)
                    {
                        var item = new ModeloMetaDetalhadaClienteViewModel();
                        var i = 0;
                        // percorre as colunas dos meses
                        foreach (Excel.IXLCell cell in row.Cells("10:33"))
                        {
                            item.ListaProdutosMes[i] = new ModeloMetaProdutoMesViewModel();
                            item.ListaProdutosMes[i].Mes = Helper.ConvertToMes(i + 1);

                            try { item.ListaProdutosMes[i].Valor = Convert.ToDecimal(cell.Value); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + cell.Address.ColumnNumber + "]"); }
                            try { item.ListaProdutosMes[i].Quantidade = Convert.ToDecimal(cell.Value); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + cell.Address.ColumnNumber + "]"); }

                            i++;
                        }

                        try { item.Canal = new Lookup(new Guid(row.Cell(34).Value.ToString()), SDKore.Crm.Util.Utility.GetEntityName<Conta>()); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + 34 + "]"); }
                        try { item.Produto = new Lookup(new Guid(row.Cell(35).Value.ToString()), SDKore.Crm.Util.Utility.GetEntityName<Product>()); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + 35 + "]"); }

                        lista.Add(item);
                    }
                }
            }
            return lista;
        }

        public List<ModeloMetaResumidaClienteViewModel> ConvertDadosMetaResumidaCanal(string path, out List<string> listaErros)
        {
            var lista = new List<ModeloMetaResumidaClienteViewModel>();
            listaErros = new List<string>();

            using (Excel.XLWorkbook excel = new Excel.XLWorkbook(path))
            {
                var ws = excel.Worksheet(1);
                foreach (Excel.IXLRow row in ws.Rows())
                {
                    if (row.RowNumber() > 5)
                    {
                        var item = new ModeloMetaResumidaClienteViewModel();
                        var i = 0;
                        // percorre as colunas dos meses
                        foreach (Excel.IXLCell cell in row.Cells("5:16"))
                        {
                            item.ListaProdutosMes[i] = new ModeloMetaProdutoMesViewModel();
                            item.ListaProdutosMes[i].Mes = Helper.ConvertToMes(i + 1);

                            try { item.ListaProdutosMes[i].Valor = Convert.ToDecimal(cell.Value); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + cell.Address.ColumnNumber + "]"); }

                            i++;
                        }

                        try { item.Canal = new Lookup(new Guid(row.Cell(17).Value.ToString()), SDKore.Crm.Util.Utility.GetEntityName<Conta>()); } catch (Exception ex) { listaErros.Add(SDKore.Helper.Error.Handler(ex) + " [Linha: " + row.RowNumber() + ", Coluna: " + 17 + "]"); }

                        lista.Add(item);
                    }
                }
            }
            return lista;
        }

        public string CriarExcelMetaKeyAccount(MetadaUnidade metaUnidade, List<ModeloMetaKeyAccountViewModel> lista, string pathTemp)
        {
            var wb = new Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Plan1");
            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                Observacao observacao = new ObservacaoService(RepositoryService).ObterObservacao(Enum.TipoParametroGlobal.ModeloPlanilhaMetaManualRepresentante);

                if (observacao == null)
                {
                    throw new ArgumentException("(CRM) Não foi Encontrado a Planilha em Parametro Global, para este nivel de Orçamento.");
                }

                pathFile = ServiceArquivo.DownLoadArquivo(pathTemp, "MetaKaRepresentante_" + metaUnidade.Nome.Replace("/", ""), observacao.Body, ".xlsx");

                object _missingValue = System.Reflection.Missing.Value;

                wb = new Excel.XLWorkbook(pathFile);
                ws = wb.Worksheet(1);
                ws.Name = "Plan1";

                ws.Cell(1, 5).Value = metaUnidade.UnidadedeNegocios.Name;
                ws.Cell(2, 4).Value = metaUnidade.Ano;
                ws.Cell(3, 4).Value = "Resumido";

                int numeroLinha = 7;

                foreach (var item in lista)
                {
                    int numeroColuna = 1;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Segmento.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Familia.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.SubFamilia.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.StatusProduto;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CodigoProduto;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Produto.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.StatusKeyAccount;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CodigoKeyAccount;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CnpjKeyAccount;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.KeyAccount.Name;

                    for (int i = 0; i < item.ListaProdutosMes.Length; i++)
                    {
                        if(item.ListaProdutosMes[i] != null)
                        {
                            ws.Cell(numeroLinha, numeroColuna++).Value = item.ListaProdutosMes[i].Valor.Value;
                            ws.Cell(numeroLinha, numeroColuna++).Value = item.ListaProdutosMes[i].Quantidade.Value;
                        }
                        else
                        {
                            ws.Cell(numeroLinha, numeroColuna++).Value = 0;
                            ws.Cell(numeroLinha, numeroColuna++).Value = 0;
                        }
                    }

                    ws.Cell(numeroLinha, 35).Value = item.KeyAccount.Id.ToString();
                    ws.Cell(numeroLinha, 36).Value = item.Produto.Id.ToString();

                    numeroLinha++;
                }

                wb.Save();
            }
            finally
            {
                if (wb != null)
                {
                    wb = null;
                }    
            }

            return pathFile;
        }

        public string CriarExcelMetaDetalhadaCanal(MetadaUnidade metaUnidade, List<ModeloMetaDetalhadaClienteViewModel> lista, string pathTemp)
        {
            var wb = new Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Plan1");
            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                Observacao observacao = new ObservacaoService(RepositoryService).ObterObservacao(Enum.TipoParametroGlobal.ModeloPlanilhaMetaDetalhada);

                if (observacao == null)
                {
                    throw new ArgumentException("(CRM) Não foi Encontrado a Planilha em Parametro Global.");
                }

                pathFile = ServiceArquivo.DownLoadArquivo(pathTemp, "MetaCanal_" + metaUnidade.Nome.Replace("/", ""), observacao.Body, ".xlsx");

                object _missingValue = System.Reflection.Missing.Value;

                wb = new Excel.XLWorkbook(pathFile);
                ws = wb.Worksheet(1);
                ws.Name = "Plan1";

                ws.Cell(1, 4).Value = metaUnidade.UnidadedeNegocios.Name;
                ws.Cell(2, 4).Value = metaUnidade.Ano;
                ws.Cell(3, 4).Value = "Detalhada";

                int numeroLinha = 6;

                foreach (var item in lista)
                {
                    int numeroColuna = 1;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Segmento.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Familia.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.SubFamilia.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.StatusProduto.GetDescription();
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CodigoProduto;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Produto.Name;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.StatusCanal.GetDescription();
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CodigoCanal;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Canal.Name;

                    for (int i = 0; i < item.ListaProdutosMes.Length; i++)
                    {
                        if (item.ListaProdutosMes[i] != null)
                        {
                            ws.Cell(numeroLinha, numeroColuna++).Value = item.ListaProdutosMes[i].Valor.Value;
                            ws.Cell(numeroLinha, numeroColuna++).Value = item.ListaProdutosMes[i].Quantidade.Value;
                        }
                        else
                        {
                            ws.Cell(numeroLinha, numeroColuna++).Value = 0;
                            ws.Cell(numeroLinha, numeroColuna++).Value = 0;
                        }
                    }

                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Canal.Id.ToString();
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Produto.Id.ToString();

                    numeroLinha++;
                }

                wb.Save();
            }
            finally
            {
                if (wb != null)
                {
                   wb = null;
                }           
            }

            return pathFile;
        }

        public string CriarExcelMetaResumidoCanal(MetadaUnidade metaUnidade, List<ModeloMetaResumidaClienteViewModel> lista, string pathTemp)
        {
            var wb = new Excel.XLWorkbook();
            var ws = wb.Worksheets.Add("Plan1");
            string pathFile = null;
            var ServiceArquivo = new ArquivoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            try
            {
                Observacao observacao = new ObservacaoService(RepositoryService).ObterObservacao(Enum.TipoParametroGlobal.ModeloPlanilhaMetaManualCanal);

                if (observacao == null)
                {
                    throw new ArgumentException("(CRM) Não foi Encontrado a Planilha em Parametro Global.");
                }

                pathFile = ServiceArquivo.DownLoadArquivo(pathTemp, "MetaCanal_" + metaUnidade.Nome.Replace("/", ""), observacao.Body, ".xlsx");

                object _missingValue = System.Reflection.Missing.Value;

                wb = new Excel.XLWorkbook(pathFile);
                ws = wb.Worksheet(1);
                ws.Name = "Plan1";

                ws.Cell(1, 2).Value = metaUnidade.UnidadedeNegocios.Name;
                ws.Cell(2, 2).Value = metaUnidade.Ano;

                int numeroLinha = 6;

                foreach (var item in lista)
                {
                    int numeroColuna = 1;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.StatusCanal.GetDescription();
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CnpjCanal;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.CodigoCanal;
                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Canal.Name;

                    for (int i = 0; i < item.ListaProdutosMes.Length; i++)
                    {
                        ws.Cell(numeroLinha, numeroColuna++).Value = item.ListaProdutosMes[i].Valor.Value;
                    }

                    ws.Cell(numeroLinha, numeroColuna++).Value = item.Canal.Id.ToString();

                    numeroLinha++;
                }

                wb.Save();
            }
            finally
            {
                if (wb != null)
                {
                   wb = null;
                }
            }

            return pathFile;
        }

        public bool ObterUrlArquivo(string GuidEntidade, string OrganizationName, out string url)
        {
            Guid sharepointGuid;

            try
            {
                if (!Guid.TryParse(GuidEntidade, out sharepointGuid))
                {
                    throw new ArgumentException("(CRM) Guid em formato inválido.");
                }

                var sharePointSiteService = new Intelbras.CRM2013.Domain.Servicos.SharePointSiteService(OrganizationName, false);
                Domain.Model.SharePointSite sharepoint = sharePointSiteService.ObterPor(sharepointGuid);

                if (string.IsNullOrEmpty(sharepoint.UrlAbsoluta))
                {
                    throw new ApplicationException("(CRM) Url sharepoint não definida.");
                }

                List<DocumentoSharePoint> lstDocSharePoint = sharePointSiteService.ListarPorIdRegistro(sharepointGuid);

                if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
                {
                    if (!string.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                    {
                        url = lstDocSharePoint[0].UrlAbsoluta;
                    }
                    else
                    {
                        url = sharepoint.UrlAbsoluta + "/itbc_arquivodesellout/" + lstDocSharePoint[0].UrlRelativa;
                    }
                }
                else
                {
                    throw new ApplicationException("(CRM) Entidade enviada não possui local de documento no sharepoint.");
                }

                return true;
            }
            catch (Exception ex)
            {
                url = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        public void CriarArquivosErros(List<String> lista, string urlPastaSharepoint, string NameFileLog)
        {
            StringBuilder logErros = new StringBuilder();

            foreach (var item in lista)
            {
                logErros.AppendLine(item);
            }

            DateTime dateNow = DateTime.Now;
            CriarArquivoSharepoint(logErros.ToString(), urlPastaSharepoint + "/" + NameFileLog);
        }

        public string CriarExcelRecalculo(List<ProdutoFaturaViewModel> lista, string mesNome, string url, string NameFileTable, string nameOrg, bool isOffline)
        {
            StringBuilder csvData = new StringBuilder();
            try
            {
                csvData.AppendLine("Item;Descricao do Item;Nota Fiscal;Emissão;Preço;Qtd Fatura;Preço " + mesNome + ";Qtd p/ Cal;Diferença;Price;IPI;TOTAL");

                foreach (var item in lista)
                {
                    string line = item.CodigoProduto + ";";
                    line += (item.DescProduto + ";");
                    line += (item.CodigoFaturaEMS + ";");
                    line += (item.DataEmissaoFatura.ToString("dd/MM/yyyy") + ";");
                    line += (item.PrecoFatura + ";");
                    line += (item.QtdFatura + ";");
                    line += (item.PrecoCalculoAtual + ";");
                    line += (item.QtdCalculo + ";");
                    line += (item.SaldoDiferenca + ";");
                    line += (item.TotalDiferenca + ";");
                    line += (item.ValorIPIProduto.ToString() + "%;");
                    line += (item.TotalComIPI + ";");
                    csvData.AppendLine(line);
                }

            }
            finally
            {
                if (csvData.Length > 0 && lista.Count > 0)
                {
                    var dateNow = DateTime.Now;
                    url = url + "/" + NameFileTable;

                    CriarArquivoSharepoint(csvData.ToString(), url);

                }
            }

            return "";
        }

        private void CriarArquivoSharepoint(string mensagem, string url)
        {
            Uri destUri = new Uri(url);

            using (MemoryStream inStream = new System.IO.MemoryStream(Encoding.Default.GetBytes(mensagem)))
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = new NetworkCredential(UsuarioSharePoint, SenhaSharePoint);
                request.Method = "PUT";
                request.Headers.Add("Overwrite", "T");
                request.Timeout = TimeOutSharepoint;
                request.ContentType = "text/plain;charset=utf-8";
                using (Stream outStream = request.GetRequestStream())
                {
                    inStream.CopyTo(outStream);
                }
                WebResponse res = request.GetResponse();
            }
        }

  /*      private T GetValueExcel<T>(Excel.IXLWorksheet xlWorksheet, int linha, int coluna)
        {
            try
            {
                //var range = xlWorksheet.Range(linha, coluna);
                var range = xlWorksheet.Cell(linha, coluna);
                if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                {
                    return Convert.ToDecimal(range.Value);
                }

                var value = Convert.ToString(range.Value);
                var foo = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));

                return (T)(foo.ConvertFromInvariantString(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) Não foi possível recuperar o valor da posição", ex);
            }
        }*/

        public void CriarArquivoLog(MetadaUnidade metaUnidade, string mensagem, string pathTemp)
        {
            string file = pathTemp + "Log Error Metas.txt";
            System.IO.File.WriteAllText(file, mensagem);
            try
            {
                AnexaArquivo(file, "Log de Erros", @"application/plain", new Lookup(metaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(metaUnidade)));
            }
            finally
            {
                ExcluirArquivo(file);
            }
        }
    }
}