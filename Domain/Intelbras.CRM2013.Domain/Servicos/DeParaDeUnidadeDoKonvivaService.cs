using System;
using System.Linq;
using SDKore.Crm.Util;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk.Messages;
using System.IO;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DeParaDeUnidadeDoKonvivaService
    {
        #region Contrutores

        private string[] _camposCrm = { "itbc_categoriaid", "itbc_classificacaoid", "itbc_papelnocanalintelbras", "itbc_subclassificacaoid", "itbc_tipodedepara", "itbc_tipoderelacao", "itbc_unidadedokonvivaid" };

        private RepositoryService RepositoryService { get; set; }

        public DeParaDeUnidadeDoKonvivaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public DeParaDeUnidadeDoKonvivaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public DeParaDeUnidadeDoKonvivaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public List<KeyValuePair<ExecuteMultipleResponse, ExecuteMultipleRequest>> AtualizarAcessosKonvivaInconsistentes(DateTime dataInicial)
        {
            var listaDePara = RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(new Dictionary<string, object>(), dataInicial, false);
            var listaAcessosKonviva = new List<AcessoKonviva>();
            var listaAcessosKonvivaPadrao = new List<AcessoKonviva>();
            var novaListaKonvivaUpdate = new List<AcessoKonviva>();
            var novaListaKonvivaAdd = new List<AcessoKonviva>();
            var retornosPaginacao = new List<KeyValuePair<ExecuteMultipleResponse, ExecuteMultipleRequest>>();
            List<object> deParaUnidadesKonviva = new List<object>();

            if (listaDePara.Count > 0)
            {
                foreach (var item in listaDePara)
                {
                    deParaUnidadesKonviva.Add(item.ID.Value);
                }
                listaAcessosKonviva = RepositoryService.AcessoKonviva.ListarPor(deParaUnidadesKonviva.ToArray());

                var aKService = new AcessoKonvivaService(RepositoryService);
                #region Correção de entradas de Acesso do Konviva que tiveram critérios alterados

                foreach (var item in listaAcessosKonviva)
                {
                    if (item.Contato != null)
                    {
                        var contato = new ContatoService(RepositoryService).BuscaContato(item.Contato.Id);
                        AcessoKonviva akTmp = this.ProcessaAcessoKonviva(contato, item);
                        if (akTmp != null)
                        {
                            novaListaKonvivaUpdate.Add(akTmp);
                        }
                    }
                }
                #endregion

                #region Correção de entradas de acesso padrão do Konviva, avaliando se algum dos novos critérios se enquadram

                listaDePara = RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(new Dictionary<string, object>(), dataInicial, true);
                foreach (var item in listaDePara)
                {
                    var listaMudar = new List<AcessoKonviva>();
                    if (item.Categoria != null || item.TipoDeRelacao == null)
                    {
                        listaMudar = RepositoryService.AcessoKonviva.ListarPorCriterioDeParaContas(item, this.unidadeKonvivaPadrao.Value);
                    }
                    else
                    {
                        if (item.PapelNoCanalIntelbras.HasValue)
                            listaMudar = RepositoryService.AcessoKonviva.ListarPorCriterioDeParaContatos(item, this.unidadeKonvivaPadrao.Value);
                    }

                    foreach(var itemMudar in listaMudar)
                    {
                        itemMudar.UnidadeKonviva = item.UnidadeDoKonviva;
                        itemMudar.DeParaUnidadeKonviva = new Lookup(item.ID.Value, "");
                        novaListaKonvivaUpdate.Add(itemMudar);
                    }
                    
                }
                #endregion

                List<AcessoKonviva> acessosTimeout = new List<AcessoKonviva>();
                for(int i = 0; i < novaListaKonvivaUpdate.Count; i += 10)
                {
                    try
                    {
                       retornosPaginacao.Add(RepositoryService.AcessoKonviva.ExecuteMultiple(novaListaKonvivaUpdate.Skip(i).Take(10).ToList(), new UpdateRequest()));
                    }
                    catch(TimeoutException e)
                    {
                        Console.WriteLine("Erro de Timeout nos seguintes GUIDs:");
                        foreach (var itemTimeout in novaListaKonvivaUpdate.Skip(i).Take(10).ToList())
                        {
                            Console.WriteLine(itemTimeout.ID.ToString());
                        }
                        Console.WriteLine("------------------------------------");
                        acessosTimeout.AddRange(novaListaKonvivaUpdate.Skip(i).Take(10).ToList());
                    }
                    
                }
                for (int i = 0; i < acessosTimeout.Count; i += 10)
                {
                    try
                    {
                        retornosPaginacao.Add(RepositoryService.AcessoKonviva.ExecuteMultiple(acessosTimeout.Skip(i).Take(10).ToList(), new UpdateRequest()));
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine("Erro de Timeout nos seguintes GUIDs (2a):");
                        foreach (var itemTimeout in novaListaKonvivaUpdate.Skip(i).Take(10).ToList())
                        {
                            Console.WriteLine(itemTimeout.ID.ToString());
                        }
                        Console.WriteLine("------------------------------------");
                    }

                }

            }

            return retornosPaginacao;
        }

        public void AtualizarUnidadeKonvivaDosContatosDoCanal(Guid? canalId)
        {
            if (canalId.HasValue)
            {
                var aKService = new AcessoKonvivaService(RepositoryService);
                Conta canal = RepositoryService.Conta.Retrieve(canalId.Value);
                AcessoKonviva acessoKonviva = new AcessoKonviva(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                var listaDeContatos = RepositoryService.Contato.ListarPor(canal);

                var novo = new AcessoKonviva(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                novo = ObterUnidadeKonvivaDeParaCom(acessoKonviva, canal, null);
                foreach (var contato in listaDeContatos)
                {
                    acessoKonviva = aKService.ObterPorContato(contato.ID.Value, Enum.StateCode.Ativo);
                    acessoKonviva.UnidadeKonviva = novo.UnidadeKonviva;
                    acessoKonviva.DeParaUnidadeKonviva = novo.DeParaUnidadeKonviva;
                    aKService.Persistir(acessoKonviva);
                }
            }
        }

        public bool RegistroDuplicado(List<KeyValuePair<string, object>> colecaoAtributos)
        {
            var listaRegistrosDePara = BuscarUnidadeKonvivaDeParaCom(colecaoAtributos, _camposCrm);

            if (listaRegistrosDePara.Count > 0)
                return true;
            else
                return false;
        }

        public AcessoKonviva ObterUnidadeKonvivaDeParaCom(AcessoKonviva acessoKonviva, Conta canal, Contato contato)
        {
            if(acessoKonviva == null) { return null; }

            List<DeParaDeUnidadeDoKonviva> listaRegistrosDePara = null;

            if (canal != null)
                listaRegistrosDePara = RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(CapturarCampos(canal), null);
            else
                listaRegistrosDePara = RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(CapturarCampos(contato), null);


            AcessoKonviva acessoKonvivaNovo = (AcessoKonviva)acessoKonviva.Clone();
            if (listaRegistrosDePara.Count > 0)
            {
                acessoKonvivaNovo.UnidadeKonviva = listaRegistrosDePara.First().UnidadeDoKonviva;
                acessoKonvivaNovo.DeParaUnidadeKonviva = new Lookup(listaRegistrosDePara.First().ID.Value, "");
            }
            else if (this.unidadeKonvivaPadrao.HasValue)
            {
                acessoKonvivaNovo.UnidadeKonviva = new Lookup(this.unidadeKonvivaPadrao.Value, "");
                acessoKonvivaNovo.DeParaUnidadeKonviva = null;
            }
            else
            {
                acessoKonvivaNovo.UnidadeKonviva = null;
                acessoKonvivaNovo.DeParaUnidadeKonviva = null;
            }

            return acessoKonvivaNovo;
        }

        public List<DeParaDeUnidadeDoKonviva> ObterDeParaPorUnidade(UnidadeKonviva unKonviva)
        {
            List<DeParaDeUnidadeDoKonviva> listaRegistrosDePara = null;

            listaRegistrosDePara = RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(CapturarCampos(unKonviva), null);

            return listaRegistrosDePara;
        }

        #endregion

        #region Métodos Auxiliares
        private Dictionary<string, object> CapturarCampos(List<KeyValuePair<string, object>> colecaoAtributos, string[] camposCrm)
        {
            var listaCamposCrm = camposCrm.ToList();
            var conjuntoCampos = RetornarDicionarioDados(colecaoAtributos, listaCamposCrm);

            return conjuntoCampos;
        }

        private Dictionary<string, object> CapturarCampos(Conta canal)
        {
            // { "itbc_categoriaid", "itbc_classificacaoid", "itbc_subclassificacaoid"}
            string[] camposCrm = _camposCrm.Except(new string[] { "itbc_papelnocanalintelbras", "itbc_tipodedepara", "itbc_tipoderelacao", "itbc_unidadedokonvivaid" }).ToArray();
            var conjuntoCampos = RetornarDicionarioDados(ListarCamposFactory(canal), camposCrm.ToList());

            return conjuntoCampos;
        }

        private Dictionary<string, object> CapturarCampos(UnidadeKonviva unKonviva)
        {
            string[] camposCrm = _camposCrm.Except(new string[] {"itbc_tipoderelacao", "itbc_papelnocanalintelbras", "itbc_categoriaid", "itbc_classificacaoid", "itbc_subclassificacaoid", "itbc_tipodedepara"}).ToArray();
            var conjuntoCampos = RetornarDicionarioDados(ListarCamposFactory(unKonviva), camposCrm.ToList());

            return conjuntoCampos;
        }

        private Dictionary<string, object> CapturarCampos(Contato contato)
        {
            //{ "itbc_papelnocanalintelbras", "itbc_tipoderelacao",};
            string[] camposCrm = _camposCrm.Except(new string[] { "itbc_categoriaid", "itbc_classificacaoid", "itbc_subclassificacaoid", "itbc_tipodedepara", "itbc_unidadedokonvivaid" }).ToArray();
            var conjuntoCampos = RetornarDicionarioDados(ListarCamposFactory(contato), camposCrm.ToList());

            return conjuntoCampos;
        }

        private AcessoKonviva ProcessaAcessoKonviva(Contato contatoAtual, AcessoKonviva entradaExistente)
        {
            AcessoKonviva novo = null;
            //if (contatoAtual.AssociadoA != null && entradaExistente.Conta != null)
            if (contatoAtual.AssociadoA != null)
            {
                Conta canal = RepositoryService.Conta.Retrieve(contatoAtual.AssociadoA.Id);
                novo = ObterUnidadeKonvivaDeParaCom(entradaExistente, canal, null); 
            }
            else
            {
                novo = ObterUnidadeKonvivaDeParaCom(entradaExistente, null, contatoAtual);
            }
            using (StreamWriter w = File.AppendText(@"c:\logkonviva.txt"))
            {
                w.WriteLine("ATUAL;" + contatoAtual.NomeCompleto + ";" + entradaExistente.UnidadeKonviva.Id + ";" + entradaExistente.DeParaUnidadeKonviva.Id + ";" + novo.UnidadeKonviva.Id + ";" + (novo.DeParaUnidadeKonviva != null ? novo.DeParaUnidadeKonviva.Id.ToString() : "NULL"));
            }
            if (entradaExistente.UnidadeKonviva != novo.UnidadeKonviva
                || entradaExistente.DeParaUnidadeKonviva != novo.DeParaUnidadeKonviva)
            {
                entradaExistente.UnidadeKonviva = novo.UnidadeKonviva;
                entradaExistente.DeParaUnidadeKonviva = novo.DeParaUnidadeKonviva;
                return entradaExistente;
            }
            else
            {
                return null;
            }
           
  
        }

        private List<KeyValuePair<string, object>> ListarCamposFactory(Conta canal)
        {
            var listaCampos = new List<KeyValuePair<string, object>>();
            //var categoriaLookUp = new CategoriaCanalService(RepositoryService).ObterCategoriaPrincipalDoCanal(canal);
            var categoriaLookUp = canal.Categoria;

            if (categoriaLookUp != null)
                listaCampos.Add(new KeyValuePair<string, object>("itbc_categoriaid", categoriaLookUp.Id));

            if (canal.Classificacao != null)
                listaCampos.Add(new KeyValuePair<string, object>("itbc_classificacaoid", canal.Classificacao.Id));

            if (canal.Subclassificacao != null)
                listaCampos.Add(new KeyValuePair<string, object>("itbc_subclassificacaoid", canal.Subclassificacao.Id));

            return listaCampos;
        }

        private List<KeyValuePair<string, object>> ListarCamposFactory(Contato contato)
        {
            var listaCampos = new List<KeyValuePair<string, object>>();

            if (contato.PapelCanal.HasValue)
            {
                if (contato.PapelCanal.Value != (int)Enum.Contato.TipoRelacao.ClienteFinal)
                    listaCampos.Add(new KeyValuePair<string, object>("itbc_papelnocanalintelbras", contato.PapelCanal.Value));
            }

            if (contato.TipoRelacao.HasValue)
                listaCampos.Add(new KeyValuePair<string, object>("itbc_tipoderelacao", contato.TipoRelacao.Value));

            return listaCampos;
        }

        private List<KeyValuePair<string, object>> ListarCamposFactory(UnidadeKonviva unKonviva)
        {
            var listaCampos = new List<KeyValuePair<string, object>>();

            if (unKonviva != null)
            {
                listaCampos.Add(new KeyValuePair<string, object>("itbc_unidadedokonvivaid", unKonviva.ID.Value));
            }

            return listaCampos;
        }

        private List<DeParaDeUnidadeDoKonviva> BuscarUnidadeKonvivaDeParaCom(List<KeyValuePair<string, object>> colecaoAtributos, string[] camposCrm)
        {
            var conjuntoCampos = CapturarCampos(colecaoAtributos, camposCrm);
            return RepositoryService.DeParaDeUnidadeDoKonviva.ListarPor(conjuntoCampos, null);
        }

        private Dictionary<string, object> RetornarDicionarioDados(List<KeyValuePair<string, object>> colecaoAtributos, List<string> listaCamposCrm)
        {
            var retorno = new Dictionary<string, object>();
            foreach (var atributo in colecaoAtributos)
            {
                if (listaCamposCrm.Contains(atributo.Key))
                    retorno.Add(atributo.Key, Utility.GetCrmAttributeValueForSearches(atributo.Value));
            }

            return retorno;
        }

        #endregion

        #region Propriedades
        private Guid? unidadeKonvivaPadrao
        {
            get
            {
                Guid temp;
                if (_unidadeKonvivaPadrao == null)
                {
                    if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("organizacaoPadraoKonviva"), out temp))
                        throw new ArgumentException("Variável organizacaoPadraoKonviva do SDKore.config não foi configurada corretamente.");

                    _unidadeKonvivaPadrao = temp;
                }

                return _unidadeKonvivaPadrao;
            }
        }
        private Guid? _unidadeKonvivaPadrao { get; set; }

        #endregion
    }
}
