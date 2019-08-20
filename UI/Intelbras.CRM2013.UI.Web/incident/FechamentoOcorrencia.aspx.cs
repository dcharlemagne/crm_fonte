using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Intelbras.CRM2013.UI.Web.Pages.incident
{
    public partial class FechamentoOcorrencia : System.Web.UI.Page
    {
        private Ocorrencia _ocorrencia = null;
        private Ocorrencia Ocorrencia
        {
            get
            {
                if (_ocorrencia == null)
                {
                    if (OcorrenciaId != Guid.Empty)
                        _ocorrencia = new CRM2013.Domain.Servicos.RepositoryService().Ocorrencia.Retrieve(OcorrenciaId);
                }
                return _ocorrencia;
            }
        }

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
        private class TipoPagamentoServico
        {
            public int val { get; set; }
            public string text { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Ocorrencia == null)
            {
                throw new ArgumentException("Nenhuma ocorrência encontrada!");
            }

            if (Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Aberta &&
                Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Em_Andamento  &&                
                Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Pendente &&
                Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.AguardandoFechamento &&
                Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Atendimento_Confirmado &&
                Ocorrencia.RazaoStatus.Value != (int)StatusDaOcorrencia.Fechamento_Cobrado)
            {
                if ((Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.AguardandoAprovacao) || (Ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Fechada))
                {
                    throw new ArgumentException("Olá, sua Ordem de Serviço já foi finalizada com sucesso, caso tenha alguma dúvida ou precise complementar, favor entrar em contato com (48) 2108 3131 Operação e Suporte - Intelbras.");
                }
                else
                {
                    throw new ArgumentException("Essa ocorrência não pode ser fechada.");

                }
            }

            textNumeroOcorrencia.Text = Ocorrencia.Numero;
            textDataInicio.Text = Ocorrencia.DataPrevistaParaVisita.Value.ToLocalTime().ToString("dd'/'MM'/'yyyy HH':'mm");
        }

        protected void buttonOK_Click(object sender, EventArgs e)
        {
            var ocorrencia = new Ocorrencia();
            ocorrencia.Id = OcorrenciaId;
            ocorrencia.DataSaidaTecnico = DateTime.ParseExact(textDataConclusao.Text, "dd/MM/yyyy H:mm", new CultureInfo("pt-BR"));
            //ocorrencia.DataDeConclusao = ocorrencia.DataSaidaTecnico;
            ocorrencia.AtividadeExecutada = textDetalhamento.Text;
            ocorrencia.DataInicioTecnico = DateTime.ParseExact(textDataChegada.Text, "dd/MM/yyyy H:mm", new CultureInfo("pt-BR"));
            ocorrencia.RazaoStatus = (int)StatusDaOcorrencia.AguardandoAprovacao;


            try
            {

                if (FileUploadAnexo.HasFile)
                {
                    if (FileUploadAnexo.PostedFile.ContentLength <= 2097152)
                    {
                        var anotacao = new Anotacao();
                        anotacao.Assunto = "Anexo";
                        anotacao.NomeArquivos = FileUploadAnexo.FileName;
                        anotacao.Body = Convert.ToBase64String(FileUploadAnexo.FileBytes);
                        anotacao.Tipo = @"application\ms-word";
                        anotacao.EntidadeRelacionada = new Lookup(OcorrenciaId, "incident");
                        new CRM2013.Domain.Servicos.RepositoryService().Anexo.Create(anotacao);
                    }
                    else
                    {
                        throw new ArgumentException("Valor do anexo limitado a 2MB.");
                    }
                }
                var listaDespesas = ListarValidarDespesas();
                foreach (var item in listaDespesas)
                {
                    new CRM2013.Domain.Servicos.RepositoryService().PagamentoServico.Create(item);
                }

                new CRM2013.Domain.Servicos.RepositoryService().Ocorrencia.Update(ocorrencia);

                Label_Message.CssClass = "MsgSucesso Validation";
                Label_Message.Text = @"O fechamento da Ocorrência foi realizado com sucesso, ele será analisado e validado pela área responsável. 
                                       <br /><br />
                                        Caso exista alguma divergência, entraremos em contato e iremos solicitar mais informações/detalhes complementares. 
                                        Tendo dúvida, favor entrar em contato no telefone (48) 2108 3131";
                ButtonOk.Enabled = false;
            }
            catch (Exception ex)
            {
                Label_Message.CssClass = "MsgErro Validation";
                Label_Message.Text = ex.Message;
            }
        }
        private List<PagamentoServico> ListarValidarDespesas()
        {
            int quantidade = 0;
            var lista = new List<PagamentoServico>();
            while (!string.IsNullOrEmpty(Request.Form["tipodespesa" + quantidade]) && !string.IsNullOrEmpty(Request.Form["valordespesa" + quantidade]))
            {
                decimal valorDespesa;
                if (!decimal.TryParse(Request.Form["valordespesa" + quantidade], out valorDespesa))
                {
                    throw new ArgumentException("O valor da despesa é inválido!");
                }
                //if (!int.TryParse(Request.Form["tipodespesa" + quantidade], out tipoDespesa))
                //{
                //    throw new ArgumentException("O tipo da despesa é inválido!");
                //}

                var result = JsonConvert.DeserializeObject<TipoPagamentoServico>(Request.Form["tipodespesa" + quantidade]);

                TipoPagamento pagamentoServico = new CRM2013.Domain.Servicos.RepositoryService().TipoPagamento.ObterPorNome(result.text);
                lista.Add(new PagamentoServico()
                {
                    OcorrenciaId = new Lookup(OcorrenciaId, "incident"),
                    Valor = valorDespesa,
                    Tipo = result.val,
                    TipoPagamentoId = new Lookup(pagamentoServico.Id, "new_pagamento_servico")
                });
                quantidade++;
            }
            return lista;
        }
    }
}