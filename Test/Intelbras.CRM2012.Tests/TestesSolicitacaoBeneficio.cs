using Intelbras.CRM2013.Domain.Model;
using Intelbras.Message.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    [TestFixture]
    public class TestesSolicitacaoBeneficio : Base
    {
        [TestMethod]
        public void CalculoDoValorProdutoSolicitacao()
        {
            CalculoValorProdutoSolicitacaoStockRotation(new Guid("4D66B9A2-AE83-E511-8C4B-0050568D7C5E"));
        }

        private void CalculoValorProdutoSolicitacaoStockRotation(Guid produtoSolicitacaoStockRotarionId)
        {
            var produtoSolicitacaoBeneficioService = new Domain.Servicos.ProdutosdaSolicitacaoService(OrganizationName, IsOffline);
            var solicitacaoBeneficioService = new Domain.Servicos.SolicitacaoBeneficioService(OrganizationName, IsOffline);

            var produtoSolicitacaoStockRotarion = produtoSolicitacaoBeneficioService.ObterPor(produtoSolicitacaoStockRotarionId);
            //ProdutoPortfolio produtoPortfolio;

            //produtoSolicitacaoBeneficioService.ValidaNaCriacao(produtoSolicitacaoStockRotarion, out produtoPortfolio);
            //var valores = produtoSolicitacaoBeneficioService.CalculoValorUnitarioETotal(produtoSolicitacaoStockRotarion, produtoPortfolio);
        }

        [Test]
        public void testeChamado35335()
        {
            SolicitacaoBeneficio SolBenef = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("C4E36532-BB5A-E411-A6A3-00155D013D51"));
            #region Após integração
            Guid tipoAtividadeExecucao;
            

            if (!Guid.TryParse(ConfigurationManager.GetSettingValue("TipoAtividadeExecucao"), out tipoAtividadeExecucao))
                throw new ArgumentException("Faltando parâmetro TipoAtividadeExecucao no SDKore");

            if (SolBenef.TipoSolicitacao != null && SolBenef.AjusteSaldo.Value)
            {
                //Informações que iremos usar para pesquisar tarefa
                SDKore.DomainModel.Lookup referenteA = new SDKore.DomainModel.Lookup(SolBenef.ID.Value, "");
                SDKore.DomainModel.Lookup tipoAtividade = new SDKore.DomainModel.Lookup(tipoAtividadeExecucao, "");

                Domain.Model.Tarefa mTarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).ObterPor(referenteA.Id, tipoAtividade.Id, 0);

                if (mTarefa != null)
                {
                    mTarefa.Resultado = (int)Domain.Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;
                    mTarefa.PareceresAnteriores = "Validada/Aprovada";
                    mTarefa.State = 1;
                    //Atualiza o resultado
                    string retorno;
                    new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).Persistir(mTarefa, out retorno);
                }
                else
                {
                    // seta pagamento realizado
                    //e.Attributes["itbc_status"] = 1;//pagamento realizado
                }
            }
            #endregion
        }

        [TestMethod]
        public void testeChamado33715()
        {
            Guid organizationName = ConfigurationManager.GetSettingValue<Guid>("Intelbras.Pedido.RepresentantePadrao");


            SolicitacaoBeneficio mSolicitacaoBeneficio = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("0E8DB2E9-2760-E411-9424-00155D013D3A"));
            new Intelbras.CRM2013.Domain.Servicos.ProcessoDeSolicitacoesService(this.OrganizationName, this.IsOffline).CriarTarefasSolicitacaoBeneficio(mSolicitacaoBeneficio, new Guid("6F376A49-3444-E411-9424-00155D013D3A"), 1);
        }

        [Test]
        public void testeChamado34596()
        {
            
            Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(OrganizationName, false);

            String teste = String.Empty;
            string xml = "<?xml version=\"1.0\"?><MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">  <CABECALHO>    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>    <NumeroOperacao>Solicitação de Rebate para ICON</NumeroOperacao>    <CodigoMensagem>MSG0154</CodigoMensagem>    <LoginUsuario />  </CABECALHO>  <CONTEUDO>    <MSG0154>      <CodigoSolicitacaoBeneficio>22906ef2-5780-e411-9400-00155d013d64</CodigoSolicitacaoBeneficio>      <NomeSolicitacaoBeneficio>Solicitação de Rebate para ICON</NomeSolicitacaoBeneficio>      <CodigoTipoSolicitacao>31bc3b7e-f938-e411-9421-00155d013d39</CodigoTipoSolicitacao>      <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>      <BeneficioCodigo>37</BeneficioCodigo>      <CodigoBeneficioCanal>0a139d47-e43d-e411-9421-00155d013d39</CodigoBeneficioCanal>      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>      <CodigoConta>e49cb808-2708-e411-9420-00155d013d39</CodigoConta>      <ValorSolicitado>375.0000</ValorSolicitado>      <DescricaoSolicitacao>.....</DescricaoSolicitacao>      <SolicitacaoIrregular>true</SolicitacaoIrregular>      <DescricaoSituacaoIrregular>Canal com Benefícios Suspensos</DescricaoSituacaoIrregular>      <CodigoFormaPagamento>978d0713-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>      <SituacaoSolicitacaoBeneficio>993520004</SituacaoSolicitacaoBeneficio>      <RazaoStatusSolicitacaoBeneficio>993520004</RazaoStatusSolicitacaoBeneficio>      <Situacao>0</Situacao>      <Proprietario>e69cb808-2708-e411-9420-00155d013d39</Proprietario>      <TipoProprietario>team</TipoProprietario>      <CodigoAssistente>36</CodigoAssistente>      <CodigoSupervisorEMS>th046210</CodigoSupervisorEMS>      <CodigoFilial xsi:nil=\"true\" />      <StatusPagamento>993520002</StatusPagamento>      <SolicitacaoAjuste>false</SolicitacaoAjuste>      <ValorAbater>300.0000</ValorAbater>    </MSG0154>  </CONTEUDO></MENSAGEM>";
            MSG0154 msg = MessageBase.LoadMessage<MSG0154>(XDocument.Parse(xml));
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            integ.Postar(usuario, senha, msg.GenerateMessage(false), out teste);

        }
        [Test]
        public void testeChange46()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            Domain.Model.SolicitacaoBeneficio solBenef = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(new Guid("5DB30163-7063-E411-A6A3-00155D013D51"));

            new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).IntegracaoBarramento(solBenef);
        }
        [Test]
        public void testeChange27()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            Domain.Model.Tarefa Task = new Intelbras.CRM2013.Domain.Servicos.TarefaService(organizationName, false).BuscaTarefa(new Guid("8147C775-3D59-E411-93F5-00155D013E70"));
            //Teste do erro de cast que estava dando
            if (!Task.ReferenteA.Type.ToLower().Equals(SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoBeneficio>().ToLower())) //  "itbc_solicitacaodecadastro")
                return;

            SolicitacaoBeneficio mSolicitacaoBeneficio = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(Task.ReferenteA.Id);


            if (Task.Resultado.Value == (int)Domain.Enum.Tarefa.Resultado.PagamentoAutorizado && mSolicitacaoBeneficio.State.HasValue)
            {

            }
                mSolicitacaoBeneficio.StatusSolicitacao = (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente;
                mSolicitacaoBeneficio.Status = (int)Domain.Enum.SolicitacaoBeneficio.RazaoStatusAtivo.ReembolsoPendente;
                mSolicitacaoBeneficio.IntegrarNoPlugin = false;
            new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).Persistir(mSolicitacaoBeneficio);
                //new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).AlterarStatus(mSolicitacaoBeneficio.ID.Value, mSolicitacaoBeneficio.State.Value, mSolicitacaoBeneficio.Status.Value);
        }
        [Test]
        public void testarSolicitacaoMensagens()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            Tarefa taref = new Intelbras.CRM2013.Domain.Servicos.TarefaService(organizationName, false).BuscaTarefa(new Guid(""));
            //taref.State
            new Intelbras.CRM2013.Domain.Servicos.RepositoryService(organizationName, false).Tarefa.Update(taref);
        }
        [Test]
        public void testarSolicitacaoAjuste()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

            List<SolicitacaoBeneficio> lstSolicitacaoBeneficioCrm = new Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ListarPorBeneficioCanalEAjusteSaldo(new Guid("5A9C1985-2143-E411-A6A3-00155D013D51"), false);
        }

        [Test]
        public void enviarEmail()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            //Guid ProdutoSolic = new Guid("0513C03C-943D-E411-803A-00155D013E44");

            Lookup solici = new Lookup(new Guid("3E797ED0-A63D-E411-803A-00155D013E44"), "itbc_solicitacaodebeneficio");
            Lookup pessoa = new Lookup(new Guid("8ADD05CD-4D17-E411-9233-00155D013E44"), "systemuser");

            new Domain.Servicos.ProcessoDeSolicitacoesService(organizationName, false).CriarEmail(solici, new ParticipantesDoProcesso(organizationName, false));
        }
        [Test]
        public void testeSolicitacaoShowroom()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Guid ProdutoSolic = new Guid("0513C03C-943D-E411-803A-00155D013E44");


            new Intelbras.CRM2013.Domain.Servicos.ProdutosdaSolicitacaoService(organizationName, false).ProdutoSolicitacaoShowRoom(new Intelbras.CRM2013.Domain.Servicos.ProdutosdaSolicitacaoService(organizationName, false).ObterPor(ProdutoSolic));

        }
        [Test]
        public void testeObterValorProduto()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.SolicitacaoBeneficio solBeneficio = new Domain.Model.SolicitacaoBeneficio(organizationName, false);
            System.Web.Script.Serialization.JavaScriptSerializer jsonConverter = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            try
            {
                string produtoID = "47887A73-E705-E411-9420-00155D013D39";
                string notaFiscalID = null;
                string solicitacaoBeneficioID = "32777BBA-4822-E411-9233-00155D013E44";

                Domain.Model.ProdutoFatura prodFatura = new Domain.Model.ProdutoFatura(organizationName, false);
                Guid produtoGuid = Guid.Parse(produtoID);
                Guid? notaFiscalGuid = (notaFiscalID == null) ? (Guid?)null : Guid.Parse(notaFiscalID);
                Guid solicitacaoBeneficioGuid = Guid.Parse(solicitacaoBeneficioID);
                Decimal ValorRetorno;
                decimal? quantidade;

                solBeneficio = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(solicitacaoBeneficioGuid);

                if (solBeneficio == null)
                    throw new ArgumentException("Não foi possível encontrar a solicitação de benefício");

                if (solBeneficio.BeneficioPrograma == null)
                    throw new ArgumentException("Campo Benefício do Programa Vazio.");

                if (notaFiscalGuid != null)
                {
                    prodFatura = new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(organizationName, false).ObterPorProdutoEfatura(produtoGuid, notaFiscalGuid.Value);

                    if (prodFatura == null)
                        throw new ArgumentException("Produto não encontrado na Nota Fiscal");

                    if (!prodFatura.ValorLiquido.HasValue)
                        throw new ArgumentException("Produto sem valor líquido.");

                    ValorRetorno = prodFatura.ValorLiquido.Value;
                    quantidade = prodFatura.Quantidade;
                }
                else
                {
                    List<Domain.Model.PrecoProduto> lstPreco = new List<Domain.Model.PrecoProduto>();
                    Domain.Model.PrecoProduto precoProdutoItem = new Domain.Model.PrecoProduto(organizationName, false);
                    precoProdutoItem.ContaId = solBeneficio.Canal.Id;
                    //Change
                    Intelbras.CRM2013.Domain.Model.Product produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(organizationName, false).ObterPor(produtoGuid);
                    if (produto == null)
                        throw new ArgumentException("Produto não encontrado");
                    precoProdutoItem.Produto = produto;
                    lstPreco.Add(precoProdutoItem);
                    lstPreco = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(organizationName, false).ListarPor(lstPreco);

                    quantidade = lstPreco[0].Quantidade;
                    ValorRetorno = lstPreco[0].ValorProduto;
                }

                Domain.Model.Beneficio benefPrograma = new Intelbras.CRM2013.Domain.Servicos.BeneficioService(organizationName, false).ObterPor(solBeneficio.BeneficioPrograma.Id);

                if (benefPrograma == null)
                    throw new ArgumentException("Benefício não encontrado.");

                dictResposta.Add("Resultado", true);
                if (benefPrograma.Codigo == (int)Domain.Enum.BeneficiodoPrograma.Codigos.PriceProtection)
                {
                    dictResposta.Add("TipoBeneficio", "PriceProtection");
                }
                else
                {
                    dictResposta.Add("TipoBeneficio", "Outros");
                }
                if (solBeneficio.BeneficioCanal != null)
                    dictResposta.Add("BeneficioCanalGuid", solBeneficio.BeneficioCanal.Id);
                if (quantidade != null)
                    dictResposta.Add("quantidade", quantidade.ToString());
                dictResposta.Add("ValorLiquido", ValorRetorno.ToString());
            }
            catch (FormatException)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception e)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", e.Message);
            }

            string resposta = jsonConverter.Serialize(dictResposta);

        }

        [Test]
        public void TesteMsg0152()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Solicitação de Beneficio VMC teste envia</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0152</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0152>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Beneficio VMC teste enviado pela Extranet</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoTipoSolicitacao>5f9feeea-f838-e411-9421-00155d013d39</CodigoTipoSolicitacao>");
            sb.AppendLine("      <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <BeneficioCodigo>21</BeneficioCodigo>");
            sb.AppendLine("      <CodigoBeneficioCanal>b1fcc6c8-e33d-e411-9421-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>f7e8e1af-d500-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>150.00</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Apenas teste</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>true</SolicitacaoIrregular>");
            sb.AppendLine("      <DescricaoSituacaoIrregular>Canal com Benefícios Suspensos</DescricaoSituacaoIrregular>");
            sb.AppendLine("      <CodigoAcaoSubsidiadaVMC>164d6023-adf0-e311-9420-00155d013d39</CodigoAcaoSubsidiadaVMC>");
            sb.AppendLine("      <DataPrevistaRetornoAcao>2014-10-24</DataPrevistaRetornoAcao>");
            sb.AppendLine("      <ValorAcao>15.50</ValorAcao>");
            sb.AppendLine("      <CodigoFormaPagamento>978d0713-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <CodigoAssistente>43</CodigoAssistente>");
            sb.AppendLine("      <CodigoSupervisorEMS>al027000</CodigoSupervisorEMS>");
            sb.AppendLine("      <SolicitacaoAjuste>false</SolicitacaoAjuste>");
            sb.AppendLine("      <ValorAbater>187.5</ValorAbater>");
            sb.AppendLine("    </MSG0152>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0153()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>bbd126e1-d435-4f8c-9c3c-325139d6faea</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0153</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0153>");
            sb.AppendLine("    </MSG0153>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0154()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version='1.0' encoding='UTF-8'?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>104010c / REBATE / ISEC/MG / 159,48</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0154</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>Carlos Eduardo Pires</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0154>");
            sb.AppendLine("      <CodigoSolicitacaoBeneficio>3febefae-0b27-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Rebate /ISEC/MG / 159,48</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoTipoSolicitacao>31bc3b7e-f938-e411-9421-00155d013d39</CodigoTipoSolicitacao>");
            sb.AppendLine("      <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <BeneficioCodigo>37</BeneficioCodigo>");
            sb.AppendLine("      <CodigoBeneficioCanal>03ba2d74-b465-e411-940b-00155d014212</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>IMG</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>a1cf0928-b10e-e411-9408-00155d013d38</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>159.4800</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Teste Paulo</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>false</SolicitacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>c67df108-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520006</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>2</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>1</Situacao>");
            sb.AppendLine("      <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <CodigoAssistente>36</CodigoAssistente>");
            sb.AppendLine("      <CodigoSupervisorEMS>al027000</CodigoSupervisorEMS>");
            sb.AppendLine("      <SolicitacaoAjuste>false</SolicitacaoAjuste>");
            sb.AppendLine("      <ValorAbater>159.4800</ValorAbater>");
            sb.AppendLine("      <ProdutoSolicitacaoItens>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProdutoSolicitacao>66f9f6ab-c324-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>61f9f6ab-c324-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4990296</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>22.8100</ValorUnitario>");
            sb.AppendLine("          <Quantidade>2</Quantidade>");
            sb.AppendLine("          <ValorTotal>45.6200</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>22.8100</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>45.6200</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>103</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProdutoSolicitacao>6af9f6ab-c324-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>61f9f6ab-c324-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4990284</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>21.9500</ValorUnitario>");
            sb.AppendLine("          <Quantidade>1</Quantidade>");
            sb.AppendLine("          <ValorTotal>21.9500</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>21.9500</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>1</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>21.9500</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>103</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProdutoSolicitacao>6ef9f6ab-c324-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>61f9f6ab-c324-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4990287</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>19.8600</ValorUnitario>");
            sb.AppendLine("          <Quantidade>2</Quantidade>");
            sb.AppendLine("          <ValorTotal>39.7200</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>19.8600</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>39.7200</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>103</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProdutoSolicitacao>72f9f6ab-c324-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>61f9f6ab-c324-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4990282</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>11.6100</ValorUnitario>");
            sb.AppendLine("          <Quantidade>2</Quantidade>");
            sb.AppendLine("          <ValorTotal>23.2200</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>11.6100</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>23.2200</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>103</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProdutoSolicitacao>76f9f6ab-c324-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>61f9f6ab-c324-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4990272</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>01a337a5-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>28.9700</ValorUnitario>");
            sb.AppendLine("          <Quantidade>1</Quantidade>");
            sb.AppendLine("          <ValorTotal>28.9700</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>28.9700</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>1</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>28.9700</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>a3cf0928-b10e-e411-9408-00155d013d38</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>103</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("      </ProdutoSolicitacaoItens>");
            sb.AppendLine("    </MSG0154>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            var resp = integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0155()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Solicitação de Price Protection para ICO</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0155</CodigoMensagem>");
            sb.AppendLine("      </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0155>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Price Protection para ICON</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoBeneficio>4dbc9991-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <CodigoBeneficioCanal>4d87dbe7-cb0d-e411-9420-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <TipoPriceProtection>993520001</TipoPriceProtection>");
            sb.AppendLine("      <ValorSolicitado>50000.5</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Teste descricao alterada VMC</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>false</SolicitacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>fdbdc11e-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <AlteradaStockRotation>false</AlteradaStockRotation>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <ProdutoSolicitacaoItens>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProduto>4000020</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>500.5</ValorUnitario>");
            sb.AppendLine("          <Quantidade>4</Quantidade>");
            sb.AppendLine("          <ValorTotal>125.4</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>555.5</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>3</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>12400.5</ValorTotalAprovado>");
            sb.AppendLine("          <ChaveIntegracaoNotaFiscal>920274</ChaveIntegracaoNotaFiscal>");
            sb.AppendLine("          <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("      </ProdutoSolicitacaoItens>");
            sb.AppendLine("    </MSG0155>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>Solicitação de Price Protec para ICON</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0155</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0155>");
            //sb.AppendLine("      <CodigoSolicitacaoBeneficio></CodigoSolicitacaoBeneficio>");
            //sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Price Protection para ICON</NomeSolicitacaoBeneficio>");
            //sb.AppendLine("      <CodigoBeneficio>4DBC9991-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            //sb.AppendLine("      <CodigoBeneficioCanal>27aa9980-768b-4c91-86a6-82331d898af1</CodigoBeneficioCanal>");
            //sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            //sb.AppendLine("      <CodigoConta>D66A88B1-BC0D-E411-9420-00155D013D39</CodigoConta>");
            //sb.AppendLine("      <TipoPriceProtection>993520000</TipoPriceProtection>");
            //sb.AppendLine("      <ValorSolicitado>1265.1234</ValorSolicitado>");
            //sb.AppendLine("      <DescricaoSolicitacao>Descrição da solicitação</DescricaoSolicitacao>");
            //sb.AppendLine("      <SolicitacaoIrregular>true</SolicitacaoIrregular>");
            //sb.AppendLine("      <DescricaoSituacaoIrregular>Descrição da solicitação irregular</DescricaoSituacaoIrregular>");
            //sb.AppendLine("      <CodigoFormaPagamento>978D0713-9C03-E411-9420-00155D013D39</CodigoFormaPagamento>");
            //sb.AppendLine("      <AlteradaStockRotation>false</AlteradaStockRotation>");
            //sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520000</SituacaoSolicitacaoBeneficio>");
            //sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            //sb.AppendLine("      <Situacao>0</Situacao>");
            //sb.AppendLine("      <Proprietario>2c03bb4b-d6c8-4dbd-86da-7c3c5cd9d197</Proprietario>");
            //sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("      <ProdutoSolicitacaoItens>");
            //sb.AppendLine("        <ProdutoSolicitacaoItem>");
            //sb.AppendLine("          <CodigoSolicitacaoBeneficio></CodigoSolicitacaoBeneficio>");
            //sb.AppendLine("          <CodigoProduto>4450000</CodigoProduto>");
            //sb.AppendLine("          <CodigoBeneficio>4DBC9991-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            //sb.AppendLine("          <ValorUnitario>6523.1234</ValorUnitario>");
            //sb.AppendLine("          <Quantidade>4</Quantidade>");
            //sb.AppendLine("          <ValorTotal>6754.1234</ValorTotal>");
            //sb.AppendLine("          <ValorUnitarioAprovado>4563.1234</ValorUnitarioAprovado>");
            //sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            //sb.AppendLine("          <ValorTotalAprovado>5436.1234</ValorTotalAprovado>");
            //sb.AppendLine("          <ChaveIntegracaoNotaFiscal>104,1,0999999</ChaveIntegracaoNotaFiscal>");
            //sb.AppendLine("          <Proprietario>666655f0-7783-4b12-9a90-f312b3858d7c</Proprietario>");
            //sb.AppendLine("          <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("        </ProdutoSolicitacaoItem>");
            //sb.AppendLine("      </ProdutoSolicitacaoItens>");
            //sb.AppendLine("    </MSG0155>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0156()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Solicitação de Stock Rotation para ICON</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0156</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0156>");
            sb.AppendLine("      <CodigoSolicitacaoBeneficio></CodigoSolicitacaoBeneficio>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Stock Rotation para ICON</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoBeneficio>1D68E3C4-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            sb.AppendLine("      <CodigoBeneficioCanal>9e1b35a0-f534-4071-bdaa-8e00bcc8542a</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>D66A88B1-BC0D-E411-9420-00155D013D39</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>1265.1234</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Descrição da solicitação</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>true</SolicitacaoIrregular>");
            sb.AppendLine("      <DescricaoSituacaoIrregular>Descrição da solicitação irregular</DescricaoSituacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>978D0713-9C03-E411-9420-00155D013D39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520000</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>83f3ee85-a4ae-42aa-a1b7-e3eb08aff1b7</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("      <ProdutoSolicitacaoItens>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>14bb726c-8fdc-440c-bd25-655cc43c67ed</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4450000</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>1D68E3C4-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>6523.1234</ValorUnitario>");
            sb.AppendLine("          <Quantidade>4</Quantidade>");
            sb.AppendLine("          <ValorTotal>6754.1234</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>4563.1234</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>5436.1234</ValorTotalAprovado>");
            sb.AppendLine("          <ChaveIntegracaoNotaFiscal>104,1,0999999</ChaveIntegracaoNotaFiscal>");
            sb.AppendLine("          <Proprietario>dbd5fcb8-0704-4962-b605-0157b4d6031a</Proprietario>");
            sb.AppendLine("          <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("      </ProdutoSolicitacaoItens>");
            sb.AppendLine("    </MSG0156>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0157()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Solicitação de Show Room para ICON alter</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0157</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0157>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Show Room para ICON alterada</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoBeneficio>005794b6-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <CodigoBeneficioCanal>5187dbe7-cb0d-e411-9420-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>1.5</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Teste show room alterada</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>false</SolicitacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>fdbdc11e-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <ProdutoSolicitacaoItens>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoProduto>4000020</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>500.5</ValorUnitario>");
            sb.AppendLine("          <Quantidade>4</Quantidade>");
            sb.AppendLine("          <ValorTotal>125.4</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>555.5</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>3</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>12400.5</ValorTotalAprovado>");
            sb.AppendLine("          <ChaveIntegracaoNotaFiscal>920274sdad</ChaveIntegracaoNotaFiscal>");
            sb.AppendLine("          <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("          <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("      </ProdutoSolicitacaoItens>");
            sb.AppendLine("    </MSG0157>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>Solicitação de Show Room para ICON</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0157</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0157>");
            //sb.AppendLine("      <CodigoSolicitacaoBeneficio></CodigoSolicitacaoBeneficio>");
            //sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Show Room para ICON</NomeSolicitacaoBeneficio>");
            //sb.AppendLine("      <CodigoBeneficio>005794B6-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            //sb.AppendLine("      <CodigoBeneficioCanal>62f786e2-3ff2-4047-b57c-1d4dd6d0c035</CodigoBeneficioCanal>");
            //sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            //sb.AppendLine("      <CodigoConta>D66A88B1-BC0D-E411-9420-00155D013D39</CodigoConta>");
            //sb.AppendLine("      <ValorSolicitado>1265.1234</ValorSolicitado>");
            //sb.AppendLine("      <DescricaoSolicitacao>Descrição da solicitação</DescricaoSolicitacao>");
            //sb.AppendLine("      <SolicitacaoIrregular>true</SolicitacaoIrregular>");
            //sb.AppendLine("      <DescricaoSituacaoIrregular>Descrição da solicitação irregular</DescricaoSituacaoIrregular>");
            //sb.AppendLine("      <CodigoFormaPagamento>978D0713-9C03-E411-9420-00155D013D39</CodigoFormaPagamento>");
            //sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520000</SituacaoSolicitacaoBeneficio>");
            //sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            //sb.AppendLine("      <Situacao>0</Situacao>");
            //sb.AppendLine("      <Proprietario>727911d9-b86c-469a-8801-8c607b1859c8</Proprietario>");
            //sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("      <ProdutoSolicitacaoItens>");
            //sb.AppendLine("        <ProdutoSolicitacaoItem>");
            //sb.AppendLine("          <CodigoSolicitacaoBeneficio></CodigoSolicitacaoBeneficio>");
            //sb.AppendLine("          <CodigoProduto>4450000</CodigoProduto>");
            //sb.AppendLine("          <CodigoBeneficio>005794B6-73ED-E311-9407-00155D013D38</CodigoBeneficio>");
            //sb.AppendLine("          <ValorUnitario>6523.1234</ValorUnitario>");
            //sb.AppendLine("          <Quantidade>4</Quantidade>");
            //sb.AppendLine("          <ValorTotal>6754.1234</ValorTotal>");
            //sb.AppendLine("          <ValorUnitarioAprovado>4563.1234</ValorUnitarioAprovado>");
            //sb.AppendLine("          <QuantidadeAprovado>2</QuantidadeAprovado>");
            //sb.AppendLine("          <ValorTotalAprovado>5436.1234</ValorTotalAprovado>");
            //sb.AppendLine("          <ChaveIntegracaoNotaFiscal>104,1,0999999</ChaveIntegracaoNotaFiscal>");
            //sb.AppendLine("          <Proprietario>8b810806-1c01-4232-a192-1a12dca01067</Proprietario>");
            //sb.AppendLine("          <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("        </ProdutoSolicitacaoItem>");
            //sb.AppendLine("      </ProdutoSolicitacaoItens>");
            //sb.AppendLine("    </MSG0157>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0158()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Solicitação de Stock Backup ICON</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0158</CodigoMensagem>");
            sb.AppendLine("      </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0158>");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Stock Backup ICON</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoBeneficio>13ab29ea-72ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <CodigoBeneficioCanal>5987dbe7-cb0d-e411-9420-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>123131.5</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Teste stock backup</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>false</SolicitacaoIrregular>");
            sb.AppendLine("      <DescricaoSituacaoIrregular>Solicitação com benefícios suspensos</DescricaoSituacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>fdbdc11e-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("    </MSG0158>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0148()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>4251c1ba-7850-e411-93f5-00155d013e70</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0148</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0148>");
            sb.AppendLine("      <CodigoSolicitacaoBeneficio>4251c1ba-7850-e411-93f5-00155d013e70</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("    </MSG0148>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
        
        [Test]
        public void TesteMsg0159()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>0dfa177e-e53d-e411-9421-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0159</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario />");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0159>");
            sb.AppendLine("      <CodigoBeneficioCanal>0dfa177e-e53d-e411-9421-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("      <VerbaReembolsada>0.0000</VerbaReembolsada>");
            sb.AppendLine("      <VerbaCalculadaPeriodoAtual>555.5555</VerbaCalculadaPeriodoAtual>");
            sb.AppendLine("    </MSG0159>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }


        [Test]
        public void TestePluginSolicitacao()
        {
            Domain.Integracao.MSG0154 msg0154 = new Domain.Integracao.MSG0154(this.OrganizationName, this.IsOffline);

            SolicitacaoBeneficio solicitacao = new Domain.Servicos.SolicitacaoBeneficioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("7537EC10-B55A-E411-A6A3-00155D013D51"));

            msg0154.DefinirPropriedadesPlugin(solicitacao);
        
        }



        [Test]
        public void TestarObterPrecoPorLista()
        {
            List<Domain.Model.PrecoProduto> lstPreco = new List<Domain.Model.PrecoProduto>();
            Domain.Model.PrecoProduto precoProdutoItem = new Domain.Model.PrecoProduto(this.OrganizationName, false);
            precoProdutoItem.ContaId = new Guid("f7e8e1af-d500-e411-9420-00155d013d39");
            precoProdutoItem.ProdutoId = new Guid("08BC0401-BEF0-E311-9420-00155D013D39");
            lstPreco.Add(precoProdutoItem);
            lstPreco = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.OrganizationName, false).ListarPor(lstPreco);
        
        }

        [Test]
        public void InativarProdutosETarefas()
        {
            Guid solicitacaoId = new Guid("13BEA374-B8A8-E411-942C-00155D013D5D");

            var produtoSolicitacaoService = new Domain.Servicos.ProdutosdaSolicitacaoService(this.OrganizationName, this.IsOffline);
            produtoSolicitacaoService.InativarTodosPorSolicitacao(solicitacaoId);

            var tarefaService = new Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline);
            tarefaService.CancelarTarefasPorReferenteA(solicitacaoId);
        }
        [Test]
        public void TesteMsg0173()
        {
            StringBuilder sb = new StringBuilder();


            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>183167 / REBATE_POSVENDA / ISEC / 2.332,</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0173</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>Thuane</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0173>");
            sb.AppendLine("  <!--    <CodigoSolicitacaoBeneficio>D2DC4BED-071C-E511-9902-00155D013F21</CodigoSolicitacaoBeneficio>  -->");
            sb.AppendLine("      <NomeSolicitacaoBeneficio>Solicitação de Rebate Pós-Venda para ISEC</NomeSolicitacaoBeneficio>");
            sb.AppendLine("      <CodigoTipoSolicitacao>44b50f47-8b5f-e411-9424-00155d013d3a</CodigoTipoSolicitacao>");
            sb.AppendLine("      <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>");
            sb.AppendLine("      <BeneficioCodigo>66</BeneficioCodigo>");
            sb.AppendLine("      <CodigoBeneficioCanal>1f0b4662-105e-e411-9424-00155d013d3a</CodigoBeneficioCanal>");
            sb.AppendLine("      <CodigoUnidadeNegocio>SEC</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoConta>b2eaf028-4c5d-e411-940b-00155d014212</CodigoConta>");
            sb.AppendLine("      <ValorSolicitado>2332.04</ValorSolicitado>");
            sb.AppendLine("      <DescricaoSolicitacao>Beneficio Rebate Pos Venda referente ao Primeiro Trimestre</DescricaoSolicitacao>");
            sb.AppendLine("      <SolicitacaoIrregular>false</SolicitacaoIrregular>");
            sb.AppendLine("      <CodigoFormaPagamento>c67df108-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>");
            sb.AppendLine("      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>");
            sb.AppendLine("      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <CodigoAssistente>36</CodigoAssistente>");
            sb.AppendLine("      <CodigoSupervisorEMS>al027000</CodigoSupervisorEMS>");
            sb.AppendLine("      <SolicitacaoAjuste>false</SolicitacaoAjuste>");
            sb.AppendLine("      <ValorAbater>2332.04</ValorAbater>");
            sb.AppendLine("      <ProdutoSolicitacaoItens>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>15d75f45-9084-e411-9416-00155d013e7d</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4562012</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>80.8</ValorUnitario>");
            sb.AppendLine("          <Quantidade>17</Quantidade>");
            sb.AppendLine("          <ValorTotal>1373.6</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>80.8</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>17</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>1373.6</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>I</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>105</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("        <ProdutoSolicitacaoItem>");
            sb.AppendLine("          <CodigoSolicitacaoBeneficio>15d75f45-9084-e411-9416-00155d013e7d</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("          <CodigoProduto>4562022</CodigoProduto>");
            sb.AppendLine("          <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>");
            sb.AppendLine("          <ValorUnitario>79.87</ValorUnitario>");
            sb.AppendLine("          <Quantidade>12</Quantidade>");
            sb.AppendLine("          <ValorTotal>958.44</ValorTotal>");
            sb.AppendLine("          <ValorUnitarioAprovado>79.87</ValorUnitarioAprovado>");
            sb.AppendLine("          <QuantidadeAprovado>12</QuantidadeAprovado>");
            sb.AppendLine("          <ValorTotalAprovado>958.44</ValorTotalAprovado>");
            sb.AppendLine("          <Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>");
            sb.AppendLine("          <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("          <Acao>I</Acao>");
            sb.AppendLine("          <CodigoEstabelecimento>105</CodigoEstabelecimento>");
            sb.AppendLine("          <Situacao>0</Situacao>");
            sb.AppendLine("        </ProdutoSolicitacaoItem>");
            sb.AppendLine("      </ProdutoSolicitacaoItens>");
            sb.AppendLine("    </MSG0173>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            var retorno = integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
    }
}
