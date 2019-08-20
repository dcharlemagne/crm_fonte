using System;
using NUnit.Framework;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain;
using System.Web;
using System.Web.Services;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.Collections.Generic;
using SDKore.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;
using Intelbras.CRM2013.DAL;
using System.IO;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesMonitoramento : Base
    {
        [Test]
        public void testarMonitoramento()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.CompromissosDoCanal comp = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarPorGuid(new Guid("32AF988F-9349-E411-9424-00155D013D3A"));
            comp.StatusCompromisso.Id = new Guid("41725811-75ED-E311-9407-00155D013D38");

            new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).AtualizarBeneficiosECompromissosCascata(comp);
        }
        [Test]
        public void testeMonitoramentoHistCanal()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.CompromissosDoCanal comp = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarPorGuid(new Guid("4ae8cd67-e53d-e411-9421-00155d013d39"));
            comp.StatusCompromisso.Id = new Guid("41725811-75ED-E311-9407-00155D013D38");
            comp.StatusCompromisso.Name = "Não Cumprido";
            //new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).CriarTarefaPendenciaCanalCompromissosCanal(comp, string.Empty);
            new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).Atualizar(comp);
        }
        [Test]
        public void testeMonitoramento2()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.CompromissosDoCanal comp = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarPorGuid(new Guid("32E8CD67-E53D-E411-9421-00155D013D39"));
            int codigo = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarCompromissoDoPrograma(comp.Compromisso.Id).Codigo.Value;
        }
        [TestMethod]
        public void testeTarefas()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Servicos.TarefaService ServiceTarefas = new Domain.Servicos.TarefaService(organizationName, false);

            Domain.Model.CompromissosDoCanal CompromissoCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarPorGuid(new Guid("{801D7A52-1B1E-E811-80D4-0050568F3AB2}"));

            List<string> lstAtividades = ServiceTarefas.ListarAtividadesCheckup(CompromissoCanal.Compromisso.Id);

            if (lstAtividades.Count > 0)
            {
                string atividade = ServiceTarefas.ObterProximaAtividadeCheckup(lstAtividades, "DRE");

                if (!string.IsNullOrEmpty(atividade))
                {
                    Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(organizationName, false).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", CompromissoCanal.Id);
                    if (proprietario != null)
                    {
                        new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).GerarAtividadeChecklist(atividade, CompromissoCanal, proprietario);
                    }
                }
            }
                                                
        }
        [Test]
        public void TestarTarefaVisitaComercial()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Servicos.TarefaService ServiceTarefas = new Domain.Servicos.TarefaService(organizationName, false);
            Domain.Model.Tarefa Tarefa = ServiceTarefas.BuscaTarefa(new Guid("BEC979ED-A74F-E411-93F5-00155D013E70"));

            if (Tarefa.ReferenteA.Type.Equals("account"))
            {
                Domain.Model.Conta canal = new Intelbras.CRM2013.Domain.Servicos.ContaService(organizationName,false).BuscaConta(Tarefa.ReferenteA.Id);
                if (canal == null || canal.Classificacao == null)
                    throw new ArgumentException("Conta cadastrada no campo 'Referente a' não encontrada!");

                Domain.Model.ParametroGlobal paramGlobal = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist, null, canal.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);
                Domain.Model.ParametroGlobal paramGlobalListaAtividades = new Intelbras.CRM2013.Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.AtividadesChecklist, null, canal.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);

                List<String> lstAtividades = new Intelbras.CRM2013.Domain.Servicos.TarefaService(organizationName, false).ConverterParametroParaLista(paramGlobalListaAtividades.Valor);

                if (lstAtividades.Count > 0)
                {
                    string atividade = ServiceTarefas.ObterProximaAtividadeCheckup(lstAtividades, Tarefa.Assunto);

                    if (!string.IsNullOrEmpty(atividade))
                    {
                        Domain.Model.Tarefa novaTarefa = new Domain.Model.Tarefa(organizationName, false);

                        novaTarefa.Assunto = atividade;

                        Domain.Model.TipoDeAtividade tipoAtividade = new Domain.Servicos.TarefaService(organizationName, false).BuscarTipoTarefa("Checklist");
                        if (tipoAtividade != null)
                        {
                            novaTarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
                        }
                        novaTarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt16(paramGlobal.Valor));

                        novaTarefa.ReferenteA = new SDKore.DomainModel.Lookup(canal.ID.Value, "account");
                        
                        novaTarefa.ID = new Domain.Servicos.TarefaService(organizationName, false).Persistir(novaTarefa);
                    }
                }
            }
        }
        [Test]
        public void SolicitacaoShowroom()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Intelbras.CRM2013.Domain.Model.SolicitacaoBeneficio SolicitBeneficioPost = new Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(new Guid("501E2383-194E-E411-93F5-00155D013E70"));


            Domain.Model.CompromissosDoCanal compromissoCanal = new Domain.Model.CompromissosDoCanal(organizationName,false);

            Domain.Model.Conta canal = new Domain.Servicos.ContaService(organizationName, false)
                                        .BuscaConta(SolicitBeneficioPost.Canal.Id);

            Domain.Model.CompromissosDoPrograma compromissoPrograma = new Domain.Servicos.CompromissosDoCanalService(organizationName, false)
                                                                        .BuscarCompromissoDoPrograma((int)Domain.Enum.CompromissoPrograma.Codigo.Showroom);

            if (canal == null)
                throw new ArgumentException("Canal não encontrado.Operação cancelada.");
            if (compromissoPrograma == null)
                throw new ArgumentException("Compromisso do Programa não encontrado.Operação cancelada.");

            Domain.Model.CompromissosDoCanal compCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarCompromissoCanal(compromissoPrograma.ID.Value, SolicitBeneficioPost.UnidadedeNegocio.Id, canal.ID.Value);
            if (compCanal == null)
                throw new ArgumentException("Compromisso do Canal para o compromisso : " + compromissoPrograma.Nome + " não encontrado.Operação cancelada.");

            List<string> lstAtividades = new TarefaService(organizationName, false).ListarAtividadesCheckup(compCanal.Compromisso.Id);

            if (lstAtividades == null || lstAtividades.Count <= 0)
                throw new ArgumentException("Lista de atividades não encontrada para o Compromisso : " + compromissoPrograma.Nome + " .Operação cancelada.");

            string atividade = new TarefaService(organizationName, false).ObterProximaAtividadeCheckup(lstAtividades, null);

            if (!string.IsNullOrEmpty(atividade))
            {
                Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(organizationName, false).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", compCanal.Id);
                if (proprietario != null)
                {
                    new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).GerarAtividadeChecklist(atividade, compCanal, proprietario);
                }
            }
        }

        [Test]
        public void VisitaComercial()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Intelbras.CRM2013.Domain.Model.Conta imagemConta = new Domain.Servicos.ContaService(organizationName, false).BuscaConta(new Guid("D66A88B1-BC0D-E411-9420-00155D013D39"));

            new Intelbras.CRM2013.Domain.Servicos.TarefaService(organizationName, false).GerarAtividadesVisitaComercial(imagemConta);
        }
        [Test]
        public void testeMonitoramento()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Intelbras.CRM2013.Domain.Model.Tarefa imagemTarefa = new Domain.Servicos.TarefaService(organizationName, false).BuscaTarefa(new Guid("463D2AC8-AF48-E411-93F5-00155D013E70"));
            Intelbras.CRM2013.Domain.Model.Tarefa mTarefa = new Domain.Model.Tarefa(organizationName, false);
            Domain.Servicos.CompromissosDoCanalService ServiceCompromissosDoCanal = new Domain.Servicos.CompromissosDoCanalService(organizationName, false);
            Domain.Servicos.StatusCompromissoService StatusCompromissoService = new Domain.Servicos.StatusCompromissoService(organizationName, false);
            //mTarefa = imagemTarefa;
            mTarefa.Resultado = null;
            
            if (imagemTarefa.TipoDeAtividade.Name.Contains("Checklist"))
            {
                if (mTarefa.Resultado.HasValue)
                {
                    switch (mTarefa.Resultado)
                    {
                        case (int)Domain.Enum.Tarefa.Resultado.Reprovada:
                            Domain.Model.CompromissosDoCanal mCompromissoCanal = ServiceCompromissosDoCanal.BuscarPorGuid(mTarefa.ReferenteA.Id);
                            if (mCompromissoCanal != null)
                            {
                                Domain.Model.StatusCompromissos statusComp = StatusCompromissoService.ObterPorNome("Não cumprido");
                                if (statusComp != null)
                                {
                                    //Só atualiza o status do compromisso se ele for diferente do status que será mudado
                                    if (mCompromissoCanal.StatusCompromisso.Id != statusComp.ID.Value)
                                    {
                                        mCompromissoCanal.StatusCompromisso.Id = statusComp.ID.Value;
                                        ServiceCompromissosDoCanal.Atualizar(mCompromissoCanal);
                                    }
                                }
                            }
                            break;
                        case (int)Domain.Enum.Tarefa.Resultado.Aprovada:
                            Domain.Model.CompromissosDoCanal mCompromissoCanalAprovado = ServiceCompromissosDoCanal.BuscarPorGuid(mTarefa.ReferenteA.Id);
                            if (mCompromissoCanalAprovado != null)
                            {
                                Domain.Model.StatusCompromissos statusComp = StatusCompromissoService.ObterPorNome("Cumprido");
                                if (statusComp != null)
                                {
                                    //Só atualiza o status do compromisso se ele for diferente do status que será mudado
                                    if (mCompromissoCanalAprovado.StatusCompromisso.Id != statusComp.ID.Value)
                                    {
                                        mCompromissoCanalAprovado.StatusCompromisso.Id = statusComp.ID.Value;
                                        ServiceCompromissosDoCanal.Atualizar(mCompromissoCanalAprovado);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }


    }
}
