using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PotencialdoSupervisorService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoSupervisorService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public PotencialdoSupervisorService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        #region Importar

        public void ImportarValores(MetadaUnidade metaUnidade, string pathTemp, Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            int quantidadePorLote = 1000;
            var trace = new Trace("Meta-Supervisor-IMPORTAR" + metaUnidade.ID.Value);
            trace.Add(" --------------------------- Iniciando --------------------------- ");


            try
            {
                var listaErros = new Dictionary<Guid, string>();
                var mensagemErro = new StringBuilder();


                // Atualizar Status
                metaUnidade.MensagemdeProcessamento = string.Format("{0} - Iniciando Importação do Supervisor{1}{2}", DateTime.Now, Environment.NewLine + Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                {
                    ID = metaUnidade.ID,
                    RazaodoStatusMetaSupervisor = (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportandoPlanilhaMetaSupervisor,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });

                #region Criando níveis da meta
                               

                // Supervisor
                listaErros = AtualizarValoresPotencialSupervisor(metaUnidade, quantidadePorLote);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Supervisor!");
                    foreach (var item in listaErros)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                }


                // Trimestre
                listaErros = AtualizarValoresPotencialTrimestre(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Trimestre!");
                    foreach (var item in listaErros)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                }


                // Segmento
                listaErros = AtualizarValoresPotencialSegmento(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Segmento!");
                    foreach (var item in listaErros)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                }

                // Familia
                listaErros = AtualizarValoresPotencialFamilia(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Supervisor!");
                    foreach (var item in listaErros)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                }

                // Subfamilia
                listaErros = AtualizarValoresPotencialSubfamilia(metaUnidade, quantidadePorLote, trimestre);
                if (listaErros.Count > 0)
                {
                    mensagemErro.AppendLine("Atualização do Potencial por Supervisor!");
                    foreach (var item in listaErros)
                    {
                        mensagemErro.AppendLine(item.Key + " - " + item.Value);
                    }
                }



                // Produto
                if (trimestre.HasValue)
                {
                    listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote, trimestre.Value);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização do Potencial por Produto!");
                        foreach (var item in listaErros)
                        {
                            mensagemErro.AppendLine(item.Key + " - " + item.Value);
                        }
                    }

                    listaErros = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote, trimestre.Value);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização do Potencial por Produto Mes!");
                        foreach (var item in listaErros)
                        {
                            mensagemErro.AppendLine(item.Key + " - " + item.Value);
                        }
                    }
                }
                else
                {
                    listaErros = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização do Potencial por Produto!");
                        foreach (var item in listaErros)
                        {
                            mensagemErro.AppendLine(item.Key + " - " + item.Value);
                        }
                    }

                    listaErros = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote);
                    if (listaErros.Count > 0)
                    {
                        mensagemErro.AppendLine("Atualização do Potencial por Produto Mes!");
                        foreach (var item in listaErros)
                        {
                            mensagemErro.AppendLine(item.Key + " - " + item.Value);
                        }
                    }
                }

                #endregion



                if (mensagemErro.Length == 0)
                {
                    trace.Add("{0} - Alterando status de registro do CRM", DateTime.Now);

                    metaUnidade.MensagemdeProcessamento = string.Format("{0} - Finalizando Importação do Supervisor{1}{2}", DateTime.Now, Environment.NewLine, metaUnidade.MensagemdeProcessamento).Truncate(1000);
                    RepositoryService.MetadaUnidade.Update(new MetadaUnidade(metaUnidade.OrganizationName, metaUnidade.IsOffline)
                    {
                        ID = metaUnidade.ID,
                        RazaodoStatusMetaSupervisor = (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.PlanilhaMetaSupervisorImportadaSucesso,
                        StatusCode = (int)Enum.OrcamentodaUnidade.StatusCodeOrcamento.ImportarPlanilhaOrcamento,
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
                        RazaodoStatusMetaKARepresentante = (int)Enum.MetaUnidade.RazaodoStatusMetaKARepresentante.ErroImportarMetaKARepresentante,
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
                    RazaodoStatusMetaSupervisor = (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ErroImportarMetaSupervisor,
                    MensagemdeProcessamento = metaUnidade.MensagemdeProcessamento
                });
            }
            finally
            {
                trace.SaveClear();
            }
        }

        public void ImportarValores(int ano, Enum.OrcamentodaUnidade.Trimestres trimestre, string pathTemp)
        {
            var lista = RepositoryService.MetadaUnidade.ListarMetas(ano);

            foreach (var metaUnidade in lista)
            {
                int? status = RepositoryService.MetadaUnidade.Retrieve(metaUnidade.ID.Value, "itbc_razodostatusmetasupervisor").RazaodoStatusMetaSupervisor;

                if (status.HasValue && status.Value != (int)Enum.MetaUnidade.RazaodoStatusMetaSupervisor.ImportandoPlanilhaMetaSupervisor)
                {
                    ImportarValores(metaUnidade, pathTemp, trimestre);
                }
            }
        }
                
        private Dictionary<Guid, string> AtualizarValoresPotencialSupervisor(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisor.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);
            var listaExistentes = RepositoryService.PotencialdoSupervisor.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisor>();
            var listaUpdate = new List<PotencialdoSupervisor>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Supervisor.Id == item.Supervisor.Id);

                if (itemExistente == null)
                {
                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Supervisor.Name);
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisor.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisor.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisor.Create(listaEmLote);


                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        if (item.IsFaulted)
                        {
                            listaError.Add(Guid.Empty, item.Message);
                        }
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialTrimestre(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisorporTrimestre.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.PotencialdoSupervisorporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialSupervisor = RepositoryService.PotencialdoSupervisor.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporTrimestre>();
            var listaUpdate = new List<PotencialdoSupervisorporTrimestre>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Supervisor.Id == item.Supervisor.Id);

                if (itemExistente == null)
                {
                    var potencialSupervisor = listaPotencialSupervisor.Find(x => x.Supervisor.Id == item.Supervisor.Id);

                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Supervisor.Name);
                    item.PotencialSupervisor = new Lookup(potencialSupervisor.ID.Value, potencialSupervisor.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialSupervisor));
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporTrimestre.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisorporTrimestre.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisorporTrimestre.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialSegmento(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisorporSegmento.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.PotencialdoSupervisorporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialTrimestre = RepositoryService.PotencialdoSupervisorporTrimestre.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporSegmento>();
            var listaUpdate = new List<PotencialdoSupervisorporSegmento>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Segmento.Id == item.Segmento.Id
                                                           && x.Supervisor.Id == item.Supervisor.Id);

                if (itemExistente == null)
                {
                    var potencialSupervisorTrimestre = listaPotencialTrimestre.Find(x => x.Trimestre.Value == item.Trimestre.Value && x.Supervisor.Id == item.Supervisor.Id);

                    item.UnidadedeNegocios = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Segmento.Name);
                    item.PotencialdoTrimestreSupervisor = new Lookup(potencialSupervisorTrimestre.ID.Value, potencialSupervisorTrimestre.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialSupervisorTrimestre));
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporSegmento.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisorporSegmento.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisorporSegmento.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialFamilia(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisorporFamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.PotencialdoSupervisorporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialSegmento = RepositoryService.PotencialdoSupervisorporSegmento.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporFamilia>();
            var listaUpdate = new List<PotencialdoSupervisorporFamilia>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.FamiliadoProduto.Id == item.FamiliadoProduto.Id
                                                           && x.Supervisor.Id == item.Supervisor.Id);


                if (itemExistente == null)
                {
                    var potencialSegmento = listaPotencialSegmento.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                                          && x.Segmento.Id == item.Segmento.Id
                                                                          && x.Supervisor.Id == item.Supervisor.Id);


                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.FamiliadoProduto.Name);
                    item.PotencialdoSupervisorporSegmento = new Lookup(potencialSegmento.ID.Value, potencialSegmento.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialSegmento));
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporFamilia.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisorporFamilia.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisorporFamilia.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialSubfamilia(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres? trimeste)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisorporSubfamilia.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.PotencialdoSupervisorporSubfamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialFamilia = RepositoryService.PotencialdoSupervisorporFamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporSubfamilia>();
            var listaUpdate = new List<PotencialdoSupervisorporSubfamilia>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.SubfamiliadeProduto.Id == item.SubfamiliadeProduto.Id
                                                           && x.Supervisor.Id == item.Supervisor.Id);


                if (itemExistente == null)
                {
                    var potencialFamilia = listaPotencialFamilia.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                                          && x.FamiliadoProduto.Id == item.FamiliadoProduto.Id
                                                                          && x.Supervisor.Id == item.Supervisor.Id);


                    item.UnidadedeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Ano = metaUnidade.Ano;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.SubfamiliadeProduto.Name);
                    item.PotencialdoSupervisorporSubfamiliaID = new Lookup(potencialFamilia.ID.Value, potencialFamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialFamilia));
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporSubFamilia.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisorporSubfamilia.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisorporSubfamilia.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            foreach (int trimeste in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var temp = AtualizarValoresPotencialProduto(metaUnidade, quantidadePorLote, (Enum.OrcamentodaUnidade.Trimestres)trimeste);
                listaError = listaError.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProduto(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres trimeste)
        {
            var listaTodos = RepositoryService.PotencialdoSupervisorporProduto.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaExistentes = RepositoryService.PotencialdoSupervisorporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);
            var listaPotencialSubfamilia = RepositoryService.PotencialdoSupervisorporSubfamilia.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporProduto>();
            var listaUpdate = new List<PotencialdoSupervisorporProduto>();

            foreach (var item in listaTodos)
            {
                var itemExistente = listaExistentes.Find(x => x.Trimestre.Value == item.Trimestre.Value
                                                           && x.Produto.Id == item.Produto.Id
                                                           && x.Supervisor.Id == item.Supervisor.Id);


                if (itemExistente == null)
                {
                    var potencialSubfamilia = listaPotencialSubfamilia.Find(x => x.SubfamiliadeProduto.Id == item.Subfamilia.Id
                                                                          && x.Supervisor.Id == item.Supervisor.Id);


                    item.UnidadeNegocio = metaUnidade.UnidadedeNegocios;
                    item.Nome = string.Format("{0} - {1}", metaUnidade.UnidadedeNegocios.Name, item.Produto.Name);
                    item.PotencialdoSupervisorPorProduto = new Lookup(potencialSubfamilia.ID.Value, potencialSubfamilia.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialSubfamilia));
                    item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporProduto.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialdoSupervisorporProduto.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialdoSupervisorporProduto.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProdutoMes(MetadaUnidade metaUnidade, int quantidadePorLote)
        {
            var listaError = new Dictionary<Guid, string>();

            foreach (int trimeste in System.Enum.GetValues(typeof(Enum.OrcamentodaUnidade.Trimestres)))
            {
                var temp = AtualizarValoresPotencialProdutoMes(metaUnidade, quantidadePorLote, (Enum.OrcamentodaUnidade.Trimestres)trimeste);
                listaError = listaError.Concat(temp).ToDictionary(x => x.Key, x => x.Value);
            }

            return listaError;
        }

        private Dictionary<Guid, string> AtualizarValoresPotencialProdutoMes(MetadaUnidade metaUnidade, int quantidadePorLote, Enum.OrcamentodaUnidade.Trimestres trimeste)
        {
            var listaPotencialProduto = RepositoryService.PotencialdoSupervisorporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, trimeste);

            var listaError = new Dictionary<Guid, string>();
            var listaCreate = new List<PotencialdoSupervisorporProdutoDetalhado>();
            var listaUpdate = new List<PotencialdoSupervisorporProdutoDetalhado>();

            foreach (var mes in Helper.ListarMeses(trimeste))
            {
                var listaTodos = RepositoryService.PotencialDetalhadodoSupervisorporProduto.ListarValoresPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, mes);
                var listaExistentes = RepositoryService.PotencialDetalhadodoSupervisorporProduto.ListarPorUnidadeNegocio(metaUnidade.UnidadedeNegocios.Id, metaUnidade.Ano.Value, mes);

                foreach (var item in listaTodos)
                {
                    var itemExistente = listaExistentes.Find(x => x.Produto.Id == item.Produto.Id
                                                               && x.Supervisor.Id == item.Supervisor.Id);


                    if (itemExistente == null)
                    {
                        var potencialProduto = listaPotencialProduto.Find(x => x.Produto.Id == item.Produto.Id
                                                                              && x.Supervisor.Id == item.Supervisor.Id);


                        item.Nome = string.Format("{0} - {1} - {2}", metaUnidade.UnidadedeNegocios.Name, Helper.ConvertToInt(mes), item.Produto.Name).Truncate(100);
                        item.PotencialdoSupervisorPorProduto = new Lookup(potencialProduto.ID.Value, potencialProduto.Nome, SDKore.Crm.Util.Utility.GetEntityName(potencialProduto));
                        item.UnidadeNegocio = metaUnidade.UnidadedeNegocios;
                        item.Trimestre = (int)trimeste;
                        item.RazaoStatus = (int)Domain.Enum.PotencialdoSupervisorporProdutoMes.StatusCode.Ativa;

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
                var retorno = RepositoryService.PotencialDetalhadodoSupervisorporProduto.Update(listaUpdate.Skip(i).Take(quantidadePorLote).ToList());

                foreach (var item in retorno)
                {
                    listaError.Add(item.Key, item.Value);
                }
            }

            // Create em lote
            for (int i = 0; i < listaCreate.Count; i += quantidadePorLote)
            {
                var listaEmLote = listaCreate.Skip(i).Take(quantidadePorLote).ToList();

                DomainExecuteMultiple retorno = RepositoryService.PotencialDetalhadodoSupervisorporProduto.Create(listaEmLote);

                if (retorno.IsFaulted)
                {
                    for (int x = 0; x < retorno.List.Length; x++)
                    {
                        var item = retorno.List[x];

                        listaError.Add(Guid.Empty, item.Message);
                    }
                }
            }

            return listaError;
        }

        #endregion
    }
}