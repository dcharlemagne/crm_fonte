using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    class TestesPlugin : Base
    {

        [Test]
        public void TestePluginCompromissoCanal()
        {
            //Variaveis para criacao da tarefa
            string assunto = string.Empty;
            string tipoTarefa = "Pendência do Canal";
            DateTime dtConclusao = DateTime.Now;
            int tipoParametro = (int)Domain.Enum.TipoParametroGlobal.NumeroDiasParaCumprimento;

            Domain.Model.CompromissosDoCanal compromissoCanalPost = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                 .BuscarPorGuid(new Guid("EA8DD4C0-1432-E411-940F-00155D013D31"));

            if (compromissoCanalPost.StatusCompromisso != null && compromissoCanalPost.StatusCompromisso.Name == Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido)
            {
                //Verifica o tipo de compromisso e muda as variaveis necessarias para criar a tarefa com base no compromisso
                switch (compromissoCanalPost.Compromisso.Name)
                {
                    case Domain.Enum.CompromissoCanal.Compromisso.Showroom:
                        assunto = "Envio de evidências de Showroom";
                        dtConclusao = DateTime.Now.AddMonths(6);
                        break;
                    case Domain.Enum.CompromissoCanal.Compromisso.RenovacaoContatos:
                    case Domain.Enum.CompromissoCanal.Compromisso.Sellout:
                    case Domain.Enum.CompromissoCanal.Compromisso.Documentacao:
                        assunto = compromissoCanalPost.Nome;
                        //Pega a data de conclusao da tarefa nos parametros global com base no nome do tipoParametro + Guid compromisso
                        dtConclusao = DateTime.Now.AddMonths(Convert.ToInt32(new Domain.Servicos.ParametroGlobalService(this.OrganizationName, this.IsOffline)
                                                                                        .ObterPor(tipoParametro,null,null,null,null, compromissoCanalPost.Compromisso.Id,null,null).Valor));
                        break;
                }

                Domain.Model.Tarefa tarefa = new Domain.Model.Tarefa(this.OrganizationName, this.IsOffline);

                //Pegamos o canal para verificar o proprietario
                Domain.Model.Conta canal = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline)
                                            .BuscaConta(compromissoCanalPost.Canal.Id);

                tarefa.Assunto = assunto;

                Domain.Model.TipoDeAtividade tipoAtividade = new Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).BuscarTipoTarefa(tipoTarefa);
                if (tipoAtividade != null)
                {
                    tarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
                }

                tarefa.Conclusao = dtConclusao;

                tarefa.ReferenteA = new SDKore.DomainModel.Lookup(compromissoCanalPost.ID.Value, "itbc_compdocanal");

                tarefa.ID = new Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).Persistir(tarefa);
            }
        }
        [Test]
        public void TestePluginTarefa()
        {
            Domain.Model.Tarefa tarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).BuscaTarefa(new Guid("1D4EFE17-55F5-E311-91F5-00155D013E44"));

            if (tarefa.TipoDeAtividade != null && tarefa.TipoDeAtividade.Name == "Atividade de Checklist")
            {
                Domain.Model.CompromissosDoCanal CompromissoTarget = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                .BuscarPorGuid(tarefa.ReferenteA.Id);

                List<string> lstAtividades = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).ListarAtividadesCheckup(CompromissoTarget.Compromisso.Id);

                if (lstAtividades.Count > 0)
                {
                    string atividade = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).ObterProximaAtividadeCheckup(lstAtividades, tarefa.Assunto);

                    if (!string.IsNullOrEmpty(atividade))
                    {
                        Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.OrganizationName, this.IsOffline).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", CompromissoTarget.Id);
                        if (proprietario != null)
                        {
                            new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline).GerarAtividadeChecklist(atividade, CompromissoTarget, proprietario);
                        }
                    }
                }

            }
        }

        [Test]
        public void TestePluginAtividadesCheckList()
        {
            Domain.Model.CompromissosDoCanal CompromissoTarget = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                 .BuscarPorGuid(new Guid("57D38025-60F1-E311-91F5-00155D013E44"));

            List<string> lstAtividades = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).ListarAtividadesCheckup(CompromissoTarget.Compromisso.Id);

            if (lstAtividades.Count > 0)
            {
                string atividade = new Intelbras.CRM2013.Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).ObterProximaAtividadeCheckup(lstAtividades, null);

                Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.OrganizationName, this.IsOffline).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", CompromissoTarget.Id);
                if (proprietario != null)
                {
                    new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline).GerarAtividadeChecklist(atividade, CompromissoTarget, proprietario);
                }

            }
        }
        [Test]
        public void TestePluginSolicitacaoBeneficio()
        {
            Domain.Model.SolicitacaoBeneficio SolicitBeneficioPost = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(this.OrganizationName, this.IsOffline)
                .ObterPor(new Guid("116274FE-DBEF-E311-91F5-00155D013E44"));
            Domain.Model.SolicitacaoBeneficio SolBeneficioTarget = SolicitBeneficioPost;

            if (SolBeneficioTarget.BeneficioPrograma != null && SolBeneficioTarget.BeneficioPrograma.Name == "Showroom")
            {
                Domain.Model.CompromissosDoCanal compromissoCanal = new Domain.Model.CompromissosDoCanal(this.OrganizationName, this.IsOffline);

                #region Criacao Compromisso do Canal

                //Pegamos o canal para verificar o proprietario
                Domain.Model.Conta canal = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline)
                                            .BuscaConta(SolicitBeneficioPost.Canal.Id);

                compromissoCanal.Canal = new SDKore.DomainModel.Lookup(canal.ID.Value, "");

                Domain.Model.CompromissosDoPrograma compromissoPrograma = new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                                                                            .BuscarCompromissoDoPrograma("Showroom");

                compromissoCanal.Compromisso = new SDKore.DomainModel.Lookup(compromissoPrograma.ID.Value, "");

                compromissoCanal.Nome = "Ter aderência às regras de utilização de identidade visual/showroom";

                compromissoCanal.ID = new Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline).Persistir(compromissoCanal);
                if (compromissoCanal.ID.HasValue)
                {
                    Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(this.OrganizationName, this.IsOffline).BuscarProprietario("account", "accountid", canal.Id);
                    if (proprietario != null)
                    {
                        new Domain.Servicos.UtilService(this.OrganizationName, this.IsOffline).MudarProprietarioRegistro("systemuser", proprietario.ID.Value, "itbc_compdocanal", compromissoCanal.ID.Value);
                    }
                }

                #endregion

                Domain.Model.Tarefa tarefa = new Domain.Model.Tarefa(this.OrganizationName, this.IsOffline);

                tarefa.Assunto = "Envio de evidências de Showroom";
                Domain.Model.TipoDeAtividade tipoAtividade = new Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).BuscarTipoTarefa("Pendência do Canal");
                if (tipoAtividade != null)
                {
                    tarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
                }
                tarefa.Conclusao = DateTime.Now.AddMonths(6);

                tarefa.ReferenteA = new SDKore.DomainModel.Lookup(compromissoCanal.ID.Value, "itbc_compdocanal");

                tarefa.ID = new Domain.Servicos.TarefaService(this.OrganizationName, this.IsOffline).Persistir(tarefa);
            }
        }
        [Test]
        public void testeJson()
        {
            string resposta = string.Empty;
            System.Web.Script.Serialization.JavaScriptSerializer jsonConverter = new System.Web.Script.Serialization.JavaScriptSerializer();
            Dictionary<string, object> obj = new Dictionary<string, object>();
            List<string> lst = new List<string>();
            lst.Add("item1");
            lst.Add("item2");
            obj.Add("ae", lst);
            resposta = jsonConverter.Serialize(obj);
        }
        [Test]
        public void testeSolicBenefPreCreate()
        {

        }
        [Test]
        public void TesteCrmWebServiceProdutoSolicitacao()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.SolicitacaoBeneficio solBeneficio = new Domain.Model.SolicitacaoBeneficio(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            try
            {
                string produtoID = "08bc0401-bef0-e311-9420-00155d013d39";
                string notaFiscalID = "2D109EDA-AC18-E411-9233-00155D013E44";
                string solicitacaoBeneficioID = "F9A8E1BB-A418-E411-9233-00155D013E44";

                Domain.Model.ProdutoFatura prodFatura = new Domain.Model.ProdutoFatura(organizationName, false);
                Guid produtoGuid = Guid.Parse(produtoID);
                Guid notaFiscalGuid = Guid.Parse(notaFiscalID);
                Guid solicitacaoBeneficioGuid = Guid.Parse(solicitacaoBeneficioID);

                solBeneficio = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(solicitacaoBeneficioGuid);

                if (solBeneficio == null)
                    throw new ArgumentException("Não foi possível encontrar a solicitação de benefício");

                if (solBeneficio.BeneficioPrograma.Name != "Stock Rotation")
                {
                    dictResposta.Add("Resultado", false);
                    dictResposta.Add("Mensagem", "Benefício do programa não é Stock Rotation");
                    dictResposta.Add("Ignorar", true);
                }
                else
                {
                    prodFatura = new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(organizationName, false).ObterPorProdutoEfatura(produtoGuid, notaFiscalGuid);

                    if (prodFatura == null)
                        throw new ArgumentException("Produto não encontrado na Nota Fiscal");

                    if (!prodFatura.ValorTotal.HasValue)
                        throw new ArgumentException("Produto sem valor total");

                    dictResposta.Add("Resultado", true);
                    dictResposta.Add("ValorTotal", prodFatura.ValorTotal.Value.ToString());
                }
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

            var json = jsonConverter.Serialize(dictResposta);
        }
        [Test]
        public void TesteCrmWebService()
        {
            Guid? categoria = Guid.Empty;
            Guid? unidNeg = Guid.Empty;
            Guid? benefPrograma = Guid.Empty;
            Guid? classifCanal = Guid.Empty;
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            string resposta = string.Empty;
            Domain.Model.Conta canal = new Domain.Model.Conta(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            try
            {
                Intelbras.CRM2013.Domain.Model.BeneficioDoCanal benefCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(organizationName, false).ObterPor(new Guid("88DD05CD-4D17-E411-9233-00155D013E44"));

                if (benefCanal == null || benefCanal.StatusBeneficio == null || (benefCanal.StatusBeneficio.Name != "Suspenso" && benefCanal.StatusBeneficio.Name != "Bloqueado"))
                {
                    dictResposta.Add("Resultado", false);
                    dictResposta.Add("Mensagem", "Não possui benefícios Suspensos ou Bloqueados");
                    resposta = jsonConverter.Serialize(dictResposta);
                }
                #region Se tiver algum atributo no beneficio do canal e canal vazio, seta a variavel como null se não,pega o Guid dela
                categoria = (benefCanal.Categoria == null) ? (Guid?)null : benefCanal.Categoria.Id;
                unidNeg = (benefCanal.UnidadeDeNegocio == null) ? (Guid?)null : benefCanal.UnidadeDeNegocio.Id;
                benefPrograma = (benefCanal.Beneficio == null) ? (Guid?)null : benefCanal.Beneficio.Id;

                if (benefCanal.Canal != null)
                {
                    canal = new Intelbras.CRM2013.Domain.Servicos.ContaService(organizationName, false).BuscaConta(benefCanal.Canal.Id);
                    if (canal != null && canal.Classificacao != null)
                        classifCanal = canal.Classificacao.Id;
                    else
                        classifCanal = null;
                }

                #endregion

                #region Pega a lista de Beneficio x Compromisso com base no perfil(Unidade negocio,classificacao canal,categoria)
                Domain.Model.Perfil perfil = new Intelbras.CRM2013.Domain.Servicos.PerfilServices(organizationName, false).BuscarPerfil(unidNeg, classifCanal, categoria, null);

                List<Domain.Model.BeneficiosCompromissos> benefCompr = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(organizationName, false).BuscaBeneficiosCompromissos(perfil.ID.Value, null, benefPrograma.Value);
                #endregion

                List<string> lstCompromissos = new List<string>();
                foreach (Domain.Model.BeneficiosCompromissos item in benefCompr)
                {
                    if (item.Compromisso != null)
                    {
                        Domain.Model.CompromissosDoCanal comproDoCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarCompromissoCanal(item.Compromisso.Id, unidNeg.Value, canal.ID.Value);
                        if (comproDoCanal != null && comproDoCanal.StatusCompromisso != null && comproDoCanal.StatusCompromisso.Name == "Não Cumprido")
                        {
                            lstCompromissos.Add(comproDoCanal.Nome);
                        }
                    }
                }
                dictResposta.Add("Resultado", true);
                dictResposta.Add("Compromissos", lstCompromissos);
                resposta = jsonConverter.Serialize(dictResposta);
            }
            catch (Exception e)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", e.Message);
                resposta = jsonConverter.Serialize(dictResposta);
            }
        }
        [Test]
        public void TestePluginMonitoramento()
        {
            Domain.Model.CompromissosDoCanal CompromissoTarget = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                                    .BuscarPorGuid(new Guid("2612525C-1432-E411-940F-00155D013D31"));

            //perfil = B1486E56-13F6-E311-91F5-00155D013E44
            //check if status changed on plugin
            Guid? UnidadeNeg = null;
            Guid? Classificacao = null;
            Guid? Categoria = null;
            Boolean? Exclusividade = null;

            Domain.Model.Conta canal = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline)
                            .BuscaConta(CompromissoTarget.Canal.Id);

            if (canal != null)
            {
                if (CompromissoTarget.UnidadeDeNegocio != null)
                    UnidadeNeg = CompromissoTarget.UnidadeDeNegocio.Id;

                if (canal.Classificacao != null)
                    Classificacao = canal.Classificacao.Id;

                Domain.Model.CategoriasCanal categoriaCanal = new Domain.Servicos.CategoriaCanalService(this.OrganizationName, this.IsOffline)
                            .ListarPor(canal.ID.Value, UnidadeNeg).FirstOrDefault();

                if (categoriaCanal != null && categoriaCanal.Categoria != null)
                    Categoria = categoriaCanal.Categoria.Id;

                if (canal.Exclusividade != null)
                    Exclusividade = canal.Exclusividade.Value;

                Domain.Model.Perfil perfil = new Intelbras.CRM2013.Domain.Servicos.PerfilServices(this.OrganizationName, this.IsOffline).BuscarPerfil(Classificacao, UnidadeNeg, Categoria, Exclusividade);

                if (perfil != null)
                {
                    List<Domain.Model.BeneficiosCompromissos> benefCompr = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(this.OrganizationName, this.IsOffline).BuscaBeneficiosCompromissos(perfil.ID.Value, CompromissoTarget.Compromisso.Id, null);

                    if (benefCompr.Count > 0)
                    {
                        foreach (Domain.Model.BeneficiosCompromissos item in benefCompr)
                        {
                            bool flagAtualizarBeneficio = true;
                            Lookup statusBenef = (Lookup)new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(this.OrganizationName, this.IsOffline)
                            .BuscarBeneficioCorrespondentePorCodigoStatus(item, CompromissoTarget.StatusCompromisso.Id);

                            if (statusBenef != null)
                            {
                                if (statusBenef.Name != Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido)
                                {
                                    //fluxo alternativo 1
                                    List<Domain.Model.BeneficiosCompromissos> benefComprAlternativo = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(this.OrganizationName, this.IsOffline).BuscaBeneficiosCompromissos(perfil.ID.Value, null, item.Beneficio.Id);
                                    if (benefComprAlternativo.Count > 0)
                                    {
                                        foreach (Domain.Model.BeneficiosCompromissos registro in benefComprAlternativo)
                                        {
                                            Domain.Model.CompromissosDoCanal comproCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline)
                                                                            .BuscarCompromissoCanal(registro.Compromisso.Id, UnidadeNeg.Value, canal.ID.Value);

                                            if (comproCanal != null)
                                            {
                                                if (comproCanal.StatusCompromisso.Name == Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido)
                                                {
                                                    flagAtualizarBeneficio = false;
                                                    break;
                                                }
                                            }
                                            else
                                                throw new ArgumentException("O compromisso "+registro.Compromisso.Name+" não existe para este Canal");
                                        }
                                    }
                                }
                                if (flagAtualizarBeneficio)
                                {
                                    Domain.Model.BeneficioDoCanal benefCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline)
                                                                            .BuscarBeneficioCanal(item.Beneficio.Id, UnidadeNeg.Value, canal.ID.Value);
                                    benefCanal.StatusBeneficio = statusBenef;
                                    new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline).AlterarBeneficioCanal(benefCanal);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
