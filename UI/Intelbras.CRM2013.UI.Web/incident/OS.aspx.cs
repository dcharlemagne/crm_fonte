using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain.ValueObjects;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;

namespace Intelbras.CRM2013.UI.Web.Pages.incident
{
    public partial class OS : System.Web.UI.Page
    {
        private Guid OcorrenciaId
        {
            get
            {
                try
                {
                    return new Guid(Request.QueryString["id"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.OcorrenciaId == Guid.Empty)
                    throw new ArgumentNullException("OcorrenciaId", "Não foi informado o parametro!");

                Ocorrencia ocorrenciaIsol = new CRM2013.Domain.Servicos.RepositoryService().Ocorrencia.ObterImpressaoOcorrenciaIsol(this.OcorrenciaId);
                Usuario proprietario = new CRM2013.Domain.Servicos.RepositoryService().Usuario.BuscarProprietario("incident", "incidentid", ocorrenciaIsol.Id);

                if (ocorrenciaIsol == null)
                    throw new ArgumentException("Ocorrência não encontrada!");

                // String
                txt_numeroOs.Text = ocorrenciaIsol.Numero;
                txt_rec.Text = ocorrenciaIsol.OsCliente;
                txt_contato.Text = ocorrenciaIsol.ContatoVisita;
                txt_kilometragemPercorrida.Text = ocorrenciaIsol.KilometragemPercorrida;
                txt_defeitoAlegado.Text = ocorrenciaIsol.DefeitoAlegado;
                txt_atividadeExecultada.Text = ocorrenciaIsol.AtividadeExecutada;
                txt_numeroNfFatura.Text = ocorrenciaIsol.NumeroNotaFiscalDeCompra;

                //Coloca empresa executante sempre como Intelbras, chamado 138030
                txt_nomeEmpresaExecutante.Text = "INTELBRAS S A IND DE TELEC ELET BRAS";
                txt_nomeEmpresaExecutante.ReadOnly = true;

                //Coloca o texto dos TextBox em negrito
                txt_defeitoAlegado.Font.Bold = true;
                txt_equipamentosInstalados.Font.Bold = true;
                txt_atividadeExecultada.Font.Bold = true;
                txt_obs.Font.Bold = true;

                if (ocorrenciaIsol.Contrato != null)
                {
                    var clienteParticipanteContrato = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.ListarPor(ocorrenciaIsol.Contrato);
                    foreach (var cliente in clienteParticipanteContrato)
                    {
                        bool infClientePArticipante = false;
                        if (ocorrenciaIsol.ClienteId.Id == cliente.Cliente.Id)
                        {
                            var enderecos = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.ListarPor(cliente);
                            foreach (var endereco in enderecos)
                            {
                                if (!string.IsNullOrEmpty(endereco.ProdutoEndereco))
                                {
                                    txt_equipamentosInstalados.Text += Regex.Replace(endereco.ProdutoEndereco, " {2,}", "");
                                }
                                else if (!string.IsNullOrEmpty(cliente.Descricao) && !infClientePArticipante)
                                {
                                    txt_equipamentosInstalados.Text += Regex.Replace(cliente.Descricao, " {2,}", "");
                                    infClientePArticipante = true;
                                }
                            }
                        }
                    }
                }
                if (ocorrenciaIsol.Contrato != null)
                    txt_obs.Text = ocorrenciaIsol.Contrato.ObservacaoOs;

                if (ocorrenciaIsol.DataDeReagendamentoVisita.HasValue)
                    txt_dataPrevistaParaVisita.Text = ocorrenciaIsol.DataDeReagendamentoVisita.Value.ToLocalTime().ToString();
                else
                {
                    if (ocorrenciaIsol.DataPrevistaParaVisita != null)
                        txt_dataPrevistaParaVisita.Text = ocorrenciaIsol.DataPrevistaParaVisita.Value.ToLocalTime().ToString();
                }

                if (proprietario.NomeCompleto != null)
                    txt_criadoPor.Text = proprietario.NomeCompleto;

                if (ocorrenciaIsol.TipoDeOcorrencia != null)
                    txt_tipoOcorrencia.Text = ((TipoDeOcorrencia)ocorrenciaIsol.TipoDeOcorrencia.Value).ToString() ;

                if (ocorrenciaIsol.Cliente != null)
                {
                    txt_nomeCliente.Text = ocorrenciaIsol.Cliente.RazaoSocial;
                    txt_localDeIntelacaoCliente.Text = ocorrenciaIsol.Cliente.NomeFantasia;

                    txt_telefoneCliente.Text = ocorrenciaIsol.Cliente.Telefone;
                }

                if (!string.IsNullOrEmpty(ocorrenciaIsol.SolicitantePortal))
                {
                    txt_nomeSolicitante.Text = ocorrenciaIsol.SolicitantePortal;
                }
                else if (ocorrenciaIsol.Solicitante != null)
                {
                    txt_nomeSolicitante.Text = ocorrenciaIsol.Solicitante.Nome;
                    txt_telefoneSolicitante.Text = ocorrenciaIsol.Solicitante.TelefoneComercial;
                }

                if (ocorrenciaIsol.ProdutosDoCliente == null)
                {
                    panel_produtosDoCliente.Visible = false;
                }
                else
                {
                    txt_produtosDoCliente.Text = ocorrenciaIsol.ProdutosDoCliente;
                }

                if (ocorrenciaIsol.TecnicoDaVisita != null)
                {
                    txt_nomeTecnicoExecutante.Text = ocorrenciaIsol.TecnicoDaVisita.Nome;
                    txt_telefoneTecnicoExecultante.Text = ocorrenciaIsol.TecnicoDaVisita.TelefoneComercial;
                    txt_rgTecnicoExecultante.Text = ocorrenciaIsol.TecnicoDaVisita.DocIdentidade;
                }

                if (ocorrenciaIsol.TecnicoResponsavel != null)
                {
                    txt_nomeTecnicoResponsavel.Text = ocorrenciaIsol.TecnicoResponsavel.Nome;
                    txt_telefoneTecnicoResponsavel.Text = ocorrenciaIsol.TecnicoResponsavel.TelefoneComercial;
                    txt_rgTecnicoResponsavel.Text = ocorrenciaIsol.TecnicoResponsavel.DocIdentidade;
                }

                // Endereco
                if (ocorrenciaIsol.EnderecoId != null)
                {
                    /*
                     * No campo new_guid_enredeco na ocorrencia esta salvando pelo CRM,
                     * o id do endereco e pelo portal esta salvando o new_cliente_participanteid
                     * precisa corrigir o portal.
                     */
                    var clienteParticipanteEndereco = new ClienteParticipanteEndereco() { Id = new Guid(ocorrenciaIsol.EnderecoId) };
                    CRM2013.Domain.Model.Endereco endereco = new CRM2013.Domain.Servicos.RepositoryService().Endereco.ObterPor(clienteParticipanteEndereco);
                    PreencherEndereco(endereco);
                }

                #region Obter NFs
                CRM2013.Domain.Model.Fatura notaFiscalRemessa = null;
                CRM2013.Domain.Model.Fatura notaFiscalFatura = null;

                if (ocorrenciaIsol.Contrato != null && ocorrenciaIsol.Cliente != null)
                {
                    notaFiscalRemessa = new CRM2013.Domain.Servicos.RepositoryService().Fatura.ObterRemessaPor(ocorrenciaIsol.Contrato, ocorrenciaIsol.Cliente);

                    if (notaFiscalRemessa == null)
                        notaFiscalRemessa = ocorrenciaIsol.Contrato.NotaFiscalRemessa;

                    notaFiscalFatura = new CRM2013.Domain.Servicos.RepositoryService().Fatura.ObterFaturaPor(ocorrenciaIsol.Contrato, ocorrenciaIsol.Cliente);
                }
                #endregion

                if (notaFiscalRemessa != null)
                    txt_numeroNfRemessa.Text = notaFiscalRemessa.IDFatura;


                if (notaFiscalFatura != null)
                {
                    txt_numeroNfFatura.Text = notaFiscalFatura.IDFatura;

                    var listaProdutos = new CRM2013.Domain.Servicos.RepositoryService().Fatura.ListarItensDaNotaFiscalPor(notaFiscalFatura.Id);

                    var listaProdutosGrid = (from item in listaProdutos
                             select new { Código = item.Produto.Codigo, Descrição = item.Produto.Nome, Quantidade = item.Quantidade }).Skip(0).Take(10);

                    tb_produtos.DataSource = listaProdutosGrid;
                    tb_produtos.DataBind();
                }
                CriarQRCode(ocorrenciaIsol.Id);
            }
            catch (Exception ex)
            {
                //string mensagem = LogHelper.Process(ex, ClassificacaoLog.PaginaOcorrencia);
                Response.Write("<script>alert('" + ex.Message + "');window.close()</script>");                
            }
        }

        private void PreencherEndereco(CRM2013.Domain.Model.Endereco endereco)
        {
            if (endereco == null)
                return;

            txt_ufCliente.Text = endereco.Uf;
            txt_cidadeCliente.Text = endereco.Cidade;
            txt_bairroCliente.Text = endereco.Bairro;
            txt_ruaCliente.Text = endereco.Logradouro +" , "+ endereco.Numero +" , "+ endereco.Complemento;
            txt_CEP.Text = endereco.Cep;
        }

        private void CriarQRCode(Guid OcorrenciaId)
        {
            var qrCode = new ZXing.BarcodeWriter();
            qrCode.Format = ZXing.BarcodeFormat.QR_CODE;
            //string a ser gerada
            String data = SDKore.Configuration.ConfigurationManager.GetSettingValue("LINK_QRCODE_OS") + "etn=incident&pagetype=entityrecord&id=" + OcorrenciaId.ToString();

            //cria um arquivo temporário com o QRCode
            String nomeArquivo = nomeAleatorio(10) + ".bmp";
            String caminhoArquivo = Path.GetTempPath() + nomeArquivo;
            new Bitmap(qrCode.Write(data)).Save(caminhoArquivo);

            //converte em base 64 para exibir na página
            byte[] objByte = File.ReadAllBytes(caminhoArquivo);
            String base64 = Convert.ToBase64String(objByte);

            img_qrcode.ImageUrl = "data:image/png;base64," + base64;

            //apaga arquivo temporário
            File.Delete(caminhoArquivo);
        }

        private static string nomeAleatorio(int tamanho)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, tamanho)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }

    }
}