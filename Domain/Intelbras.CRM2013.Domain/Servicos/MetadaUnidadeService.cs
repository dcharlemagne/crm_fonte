using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetadaUnidadeService
    {
        #region Construtores

        public MetadaUnidadeService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        { }

        public MetadaUnidadeService(string organizacao, bool isOffline, object provider)
        {
            _organizationName = organizacao;
            _isOffline = isOffline;
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public MetadaUnidadeService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        #region objetos

        private RepositoryService RepositoryService { get; set; }

        private readonly string _organizationName;
        private readonly bool _isOffline;

        private string PathTemp
        {
            get
            {
                return SDKore.Configuration.ConfigurationManager.GetSettingValue("DirMetaOrcamento");
            }
        }
        
        #endregion

        #region Processar todas as matas

        public void MetaCanalGerar()
        {
            List<MetadaUnidade> lista = RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaManual.GerarMetaCanalManual);
            var service = new MetadoCanalService(RepositoryService);

            foreach (var item in lista)
            {
                if (item.NiveldaMeta.Value == (int)Enum.MetaUnidade.NivelMeta.Detalhado)
                {
                    service.CriarExtruturaMetaDetalhada(item, PathTemp);
                }
                else
                {
                    service.CriarExtruturaMetaResumida(item, PathTemp);
                }
            }
        }

        public void MetaCanalImportar()
        {
            List<MetadaUnidade> lista = RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaManual.ImportarPlanilhaMetaCanalManual);
            var service = new MetadoCanalService(RepositoryService);

            foreach (var item in lista)
            {
                if (item.NiveldaMeta.Value == (int)Enum.MetaUnidade.NivelMeta.Detalhado)
                {
                    service.ImportarMetaDetalhada(item, PathTemp);
                }
                else
                {
                    service.ImportarMetaResumida(item, PathTemp);
                }
            }
        }

        public void PotencialKAImportar()
        {
            List<MetadaUnidade> lista = RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ImportarPlanilhaMetaKARepresentante);
            var service = new PotencialdoKARepresentanteService(RepositoryService);

            foreach (var item in lista)
            {
                service.ImportarValores(item, PathTemp);
            }
        }

        public void PotencialKAGerar()
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.GerarMetaKARepresentante);
            var service = new PotencialdoKARepresentanteService(RepositoryService);

            foreach (var item in lstMetadaUnidade)
            {
                service.CriarExtruturaMetaKeyAccount(item, PathTemp);
            }
        }

        public void PotencialSupervisorImportar()
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ObterLerGerarPlanilhaSupervisor((int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportarPlanilhaMetaSupervisor);
            var service = new PotencialdoSupervisorService(RepositoryService);

            foreach (var item in lstMetadaUnidade)
            {
                service.ImportarValores(item, PathTemp);
            }
        }

        public void MetaUnidadeImportar()
        {
            List<MetadaUnidade> lista = RepositoryService.MetadaUnidade.ObterLerGerarPlanilha((int)Enum.MetaUnidade.StatusMetaUnidade.ImportarPlanilhaMeta);

            foreach (var item in lista)
            {
                ImportarValores(item, PathTemp);
            }
        }

        public void EndTaskOfWindows()
        {
            List<MetadaUnidade> lstMetadaUnidade = new List<MetadaUnidade>();
            #region Metas
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ObterLerGerarPlanilha((int)Enum.MetaUnidade.StatusMetaUnidade.GerandoModelodeMeta).ToArray());
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ObterLerGerarPlanilha((int)Enum.MetaUnidade.StatusMetaUnidade.ImportandoPlanilhaMeta).ToArray());
            foreach (MetadaUnidade item in lstMetadaUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemdeProcessamento = "Serviço foi interrompido.";

                if (item.StateCode == (int)Enum.MetaUnidade.StatusMetaUnidade.GerandoModelodeMeta)
                    item.StatusCode = (int)Domain.Enum.MetaUnidade.StatusMetaUnidade.ErroGeracaoModeloMeta;
                else if (item.StateCode == (int)Enum.MetaUnidade.StatusMetaUnidade.ImportandoPlanilhaMeta)
                    item.StatusCode = (int)Domain.Enum.MetaUnidade.StatusMetaUnidade.ErroImportarPlanilhaMeta;

                RepositoryService.MetadaUnidade.Update(item);
            }
            #endregion

            lstMetadaUnidade.Clear();

            #region Meta Manual
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaManual.GerandoMetaCanalManual).ToArray());
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaManual.ImportarPlanilhaMetaCanalManual).ToArray());

            foreach (MetadaUnidade item in lstMetadaUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemdeProcessamento = "Serviço foi interrompido.";

                if (item.RazaodoStatusMetaManual == (int)Enum.MetaUnidade.RazaodoStatusMetaManual.GerandoMetaCanalManual)
                    item.RazaodoStatusMetaManual = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaManual.ErroGerarMetaCanalManual;
                else if (item.RazaodoStatusMetaManual == (int)Enum.MetaUnidade.RazaodoStatusMetaManual.ImportandoPlanilhaMetaCanalManual)
                    item.RazaodoStatusMetaManual = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaManual.ErroImportarMetaCanalManual;

                RepositoryService.MetadaUnidade.Update(item);
            }
            #endregion

            lstMetadaUnidade.Clear();

            #region Meta Ka/Representante
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.GerandoMetaKARepresentante).ToArray());
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ListarPor(Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ImportandoPlanilhaMetaKARepresentante).ToArray());
            foreach (MetadaUnidade item in lstMetadaUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemdeProcessamento = "Serviço foi interrompido.";

                if (item.RazaodoStatusMetaKARepresentante == (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.GerandoMetaKARepresentante)
                    item.RazaodoStatusMetaKARepresentante = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroGerarMetaKARepresentante;
                else if (item.RazaodoStatusMetaKARepresentante == (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ImportandoPlanilhaMetaKARepresentante)
                    item.RazaodoStatusMetaKARepresentante = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroImportarMetaKARepresentante;

                RepositoryService.MetadaUnidade.Update(item);
            }
            #endregion

            lstMetadaUnidade.Clear();

            #region Meta Supervisor
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ObterLerGerarPlanilhaSupervisor((int)Domain.Enum.MetaUnidade.RazaodoStatusMetaSupervisor.GerandoMetaSupervisor).ToArray());
            lstMetadaUnidade.AddRange(RepositoryService.MetadaUnidade.ObterLerGerarPlanilhaSupervisor((int)Domain.Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportandoPlanilhaMetaSupervisor).ToArray());
            foreach (MetadaUnidade item in lstMetadaUnidade)
            {
                item.MensagemErro = "Serviço foi interrompido.";
                item.MensagemdeProcessamento = "Serviço foi interrompido.";

                if (item.RazaodoStatusMetaSupervisor == (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.GerandoMetaSupervisor)
                    item.RazaodoStatusMetaSupervisor = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ErroGerarMetaSupervisor;
                else if (item.RazaodoStatusMetaSupervisor == (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportandoPlanilhaMetaSupervisor)
                    item.RazaodoStatusMetaSupervisor = (int)Domain.Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ErroImportarMetaSupervisor;

                RepositoryService.MetadaUnidade.Update(item);
            }
            #endregion
        }

        #endregion

        #region Métodos Plugin

        public void PreCreate(MetadaUnidade mMetadaUnidade)
        {
            List<MetadaUnidade> lista = RepositoryService.MetadaUnidade.ListarMetas(mMetadaUnidade.UnidadedeNegocios.Id, mMetadaUnidade.Ano.Value);

            if (lista.Count > 0)
            {
                throw new ArgumentException("(CRM) Já existe Meta para Esta Unidade de Negocios.");
            }
        }

        #endregion
        
        #region Importar

        public void ImportarValores(int ano, Enum.OrcamentodaUnidade.Trimestres trimestre, string pathTemp)
        {
            var lista = RepositoryService.MetadaUnidade.ListarMetas(ano);

            foreach (var metaUnidade in lista)
            {
                int? status = RepositoryService.MetadaUnidade.Retrieve(metaUnidade.ID.Value, "statuscode").StatusCode;

                if (status.HasValue && status.Value != (int)Enum.MetaUnidade.StatusMetaUnidade.ImportarPlanilhaMeta)
                {
                    ImportarValores(metaUnidade, pathTemp, trimestre);
                }
            }
        }

        public void ImportarValores(MetadaUnidade metaUnidade, string pathTemp, Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            int quantidadePorLote = 1000;
            var trace = new Trace("Meta-Unidade-IMPORTAR" + metaUnidade.ID.Value);
            trace.Add(" --------------------------- Iniciando --------------------------- ");

            try
            {
                var listaErros = new List<string>();
                var mensagemErro = new StringBuilder();


                // Atualizar Status
                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando Importação da Unidade{1}{2}", DateTime.Now, Environment.NewLine + Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    StatusCode = (int)Enum.MetaUnidade.StatusMetaUnidade.ImportandoPlanilhaMeta,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                #region Criando níveis da meta

                AtualizarValoresMetaUnidade(metaUnidade);

                // Trimestre
                listaErros = AtualizarValoresPotencialTrimestre(metaUnidade, quantidadePorLote);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização de meta unidade por trimestre!");
                    mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                }

                // Segmento
                listaErros = AtualizarValoresPotencialSegmento(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização de meta unidade por segmento!");
                    mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                }

                // Familia
                listaErros = AtualizarValoresPotencialFamilia(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização de meta unidade por família!");
                    mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                }

                // Subfamilia
                listaErros = AtualizarValoresPotencialSubfamilia(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização de meta unidade por subfamília!");
                    mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                }



                // Produto
                if (trimestre.HasValue)
                {
                    listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote, trimestre.Value);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização de meta unidade por produto!");
                        mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                    }

                    listaErros = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote, trimestre.Value);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização de meta unidade por produto mês!");
                        mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                    }
                }
                else
                {
                    listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização de meta unidade por produto!");
                        mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                    }

                    listaErros = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização de meta unidade por produto mês!");
                        mensagemErro.AppendLine(string.Join(Environment.NewLine, listaErros));
                    }
                }

                #endregion



                if (mensagemErro.Length == 0)
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizando Importação da Unidade{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        StatusCode = (int)Enum.MetaUnidade.StatusMetaUnidade.PlanilhaMetaImportadaSucesso,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });
                }
                else
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação do Supervisor{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        StatusCode = (int)Enum.MetaUnidade.StatusMetaUnidade.ErroImportarPlanilhaMeta,
                        MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                    });

                    trace.Add("{0} - Criando arquivo de log", DateTime.Now);

                    string file = pathTemp + "Log Error Metas.txt";

                    new ArquivoService(RepositoryService).CriarArquivoLog(metaUnidade, mensagemErro.ToString(), file);
                }
            }
            catch (Exception ex)
            {
                string mensagem = Error.Handler(ex);
                trace.Add(ex);

                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Erro Importação: {1}{2}{3}", DateTime.Now, mensagem, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    StatusCode = (int)Enum.MetaUnidade.StatusMetaUnidade.ErroImportarPlanilhaMeta,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            finally
            {
                trace.SaveClear();
            }
        }

        private void AtualizarValoresMetaUnidade(MetadaUnidade metaUnidade)
        {
            var temp = RepositoryService.MetadaUnidade.ObterValoresPor(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            temp.ID = metaUnidade.ID;

            RepositoryService.MetadaUnidade.Update(temp);
        }

        private List<string> AtualizarValoresPotencialTrimestre(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaTodos = RepositoryService.MetadaUnidadeporTrimestre.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaExistentes = RepositoryService.MetadaUnidadeporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            var listaError = new List<string>();
            var listaCreate = new List<MetadaUnidadeporTrimestre>();
            var listaUpdate = new List<MetadaUnidadeporTrimestre>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value);

                if (itemExistente == null)
                {
                    var trimestre = (Enum.OrcamentodaUnidade.Trimestres)item.Trimestre.Value;

                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, trimestre.GetDescription());
                    item.MetadaUnidade = new Lookup(metaUnidade.ID.Value, metaUnidade.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaUnidade));
                    item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporTrimestre.StatusCode.Ativa;

                    listaCreate.Add(item);
                }
                else
                {
                    item.ID = itemExistente.ID;
                    listaUpdate.Add(item);
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeporTrimestre.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeporTrimestre.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialSegmento(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.MetadaUnidadeporSegmento.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.MetadaUnidadeporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaMetaUnidadeTrimestre = RepositoryService.MetadaUnidadeporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            var listaError = new List<string>();
            var listaCreate = new List<MetadaUnidadeporSegmento>();
            var listaUpdate = new List<MetadaUnidadeporSegmento>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Segmento.Id == item.Segmento.Id);

                if (itemExistente == null)
                {
                    var metaUnidadeTrimestre = listaMetaUnidadeTrimestre.Find(x => x.Trimestre.Value == item.Trimestre.Value);

                    item.UnidadedeNegocios = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Trimestre = item.Trimestre;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Segmento.Name);
                    item.MetaporTrimestre = new Lookup(metaUnidadeTrimestre.ID.Value, metaUnidadeTrimestre.Nome, SDKore.Crm.Util.Utility.GetEntityName(metaUnidadeTrimestre));
                    item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporSegmento.StatusCode.Ativa;

                    listaCreate.Add(item);
                }
                else
                {
                    item.ID = itemExistente.ID;
                    listaUpdate.Add(item);
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeporSegmento.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeporSegmento.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialFamilia(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.MetadaUnidadeporFamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.MetadaUnidadeporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaMetaUnidadeSegmento = RepositoryService.MetadaUnidadeporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new List<string>();
            var listaCreate = new List<MetadaUnidadeporFamilia>();
            var listaUpdate = new List<MetadaUnidadeporFamilia>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Familia.Id == item.Familia.Id);


                if (itemExistente == null)
                {
                    var metaSegmento = listaMetaUnidadeSegmento.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                                          && x.Segmento.Id == item.Segmento.Id);


                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Familia.Name);
                    item.Segmento = metaSegmento.Segmento;
                    item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporFamilia.StatusCode.Ativa;

                    listaCreate.Add(item);
                }
                else
                {
                    item.ID = itemExistente.ID;
                    listaUpdate.Add(item);
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeporFamilia.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeporFamilia.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialSubfamilia(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.MetadaUnidadeporSubfamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.MetadaUnidadeporSubfamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialFamilia = RepositoryService.MetadaUnidadeporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new List<string>();
            var listaCreate = new List<MetadaUnidadeporSubfamilia>();
            var listaUpdate = new List<MetadaUnidadeporSubfamilia>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Subfamilia.Id == item.Subfamilia.Id);


                if (itemExistente == null)
                {
                    var potencialFamilia = listaPotencialFamilia.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                                          && x.Familia.Id == item.Familia.Id);


                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Subfamilia.Name);
                    item.MetadaFamilia = new Lookup(potencialFamilia.ID.Value, potencialFamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialFamilia));
                    item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporSubfamilia.StatusCode.Ativa;

                    listaCreate.Add(item);
                }
                else
                {
                    item.ID = itemExistente.ID;
                    listaUpdate.Add(item);
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeporSubfamilia.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeporSubfamilia.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres trimeste)
        {
            var listaTodos = RepositoryService.MetadaUnidadeporProduto.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.MetadaUnidadeporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialSubfamilia = RepositoryService.MetadaUnidadeporSubfamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new List<string>();
            var listaCreate = new List<MetadaUnidadeporProduto>();
            var listaUpdate = new List<MetadaUnidadeporProduto>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Produto.Id == item.Produto.Id);


                if (itemExistente == null)
                {
                    var potencialSubfamilia = listaPotencialSubfamilia.Find(x => x.Subfamilia.Id == item.Subfamilia.Id);

                    item.UnidadeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Produto.Name);
                    item.MetadaSubfamilia = new Lookup(potencialSubfamilia.ID.Value, potencialSubfamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialSubfamilia));
                    item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporProduto.StatusCode.Ativa;

                    listaCreate.Add(item);
                }
                else
                {
                    item.ID = itemExistente.ID;
                    listaUpdate.Add(item);
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeporProduto.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeporProduto.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new List<string>();

            foreach (int trimeste in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var temp = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote, (Enum.OrcamentodaUnidade.Trimestres)trimeste);
                listaError.AddRange(temp);
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialProdutoMes(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new List<string>();

            foreach (int trimeste in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var temp = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote, (Enum.OrcamentodaUnidade.Trimestres)trimeste);
                listaError.AddRange(temp);
            }

            return listaError;
        }

        private List<string> AtualizarValoresPotencialProdutoMes(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres trimeste)
        {
            var listaPotencialProduto = RepositoryService.MetadaUnidadeporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new List<string>();
            var listaCreate = new List<MetaDetalhadadaUnidadeporProduto>();
            var listaUpdate = new List<MetaDetalhadadaUnidadeporProduto>();

            foreach (var mes in Helper.ListarMeses(trimeste))
            {
                var listaTodos = RepositoryService.MetadaUnidadeporProdutoMes.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, mes);
                var listaExistentes = RepositoryService.MetadaUnidadeporProdutoMes.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, mes);

                foreach (var item in listaTodos)
                {
                    var itemExistente = listaExistentes.Find(x => x.Produto.Id == item.Produto.Id);

                    if (itemExistente == null)
                    {
                        var potencialProduto = listaPotencialProduto.Find(x => x.Produto.Id == item.Produto.Id);


                        item.Nome = string.Format("{0} - {1} - {2}", metaUnidade.UnidadedeNegocios.Name, Helper.ConvertToInt(mes), item.Produto.Name).Truncate(100);
                        item.MetadoProduto = new Lookup(potencialProduto.ID.Value, potencialProduto.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialProduto));
                        item.Trimestre = (int)trimeste;
                        item.UnidadeNegocio = metaUnidade.UnidadedeNegocios;
                        item.RazaoStatus = (int)Domain.Enum.MetaUnidadeporProdutoMes.StatusCode.Ativa;

                        listaCreate.Add(item);
                    }
                    else
                    {
                        item.ID = itemExistente.ID;
                        listaUpdate.Add(item);
                    }
                }
            }

            // Update em lote
            for (int i = 0; i < listaUpdate.Count; i += quantidadePorLote)
            {
                var retorno = RepositoryService.MetadaUnidadeDetalhadaProduto.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key + " - " + item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.MetadaUnidadeDetalhadaProduto.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add("Create - " + item.Message);
                    }
                }
            }

            return listaError;
        }

        #endregion
    }
}