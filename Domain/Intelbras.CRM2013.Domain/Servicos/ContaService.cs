using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos.Docs;
using Intelbras.CRM2013.Domain.ViewModels;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using SDKore.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using Excel = ClosedXML.Excel;
using Microsoft.Xrm.Sdk.Messages;
using SDKore.Crm.Util;
using Intelbras.CRM2013.Domain.Servicos;
using System.Linq;
using System.Collections;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ContaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ContaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ContaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ContaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public Model.Conta Persistir(Model.Conta conta)
        {
            var TmpConta = (conta.ID.HasValue)
                ? RepositoryService.Conta.Retrieve(conta.ID.Value)
                : RepositoryService.Conta.ObterPor(conta.CpfCnpj, conta.TipoConstituicao.Value);

            #region Adesão de Revenda

            /*if (conta.ParticipantePrograma.Value == (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
            {
                if (conta.Classificacao != null && conta.Classificacao.Id == SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Classificacao.Revenda"))
                {
                    if (TmpConta == null)
                    {
                        conta.ParticipantePrograma = (int)Domain.Enum.Conta.ParticipaDoPrograma.Nao;
                        conta.ID = RepositoryService.Conta.Create(conta);
                        return AdesaoRevenda(conta);
                    }
                    else
                    {
                        if (!TmpConta.ParticipantePrograma.HasValue || TmpConta.ParticipantePrograma.Value != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                        {
                            conta.ID = TmpConta.ID;
                            conta.ParticipantePrograma = null;
                            RepositoryService.Conta.Update(conta);

                            return AdesaoRevenda(conta);
                        }
                    }
                }
            }*/

            #endregion

            if (TmpConta != null)
            {
                conta.ID = TmpConta.ID;
                //Deixar atualizar a classificação e subclassificação quando o campo da conta no CRM participa do pci = Não e origem conta = ITEC.
                //Verificar os valores que estão no CRM, não os que foram enviados

                //Após conversa com José Luiz no dia 09/09/2016 foi acordado que seria melhor deixar fazer a atualização da Classificacao e Subclassificacao para não impactar nas revendas sem Categorias
                /*if (TmpConta.OrigemConta.Value != (int)Enum.Conta.OrigemConta.ITEC || TmpConta.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Sim)
                {
                    conta.Classificacao = null;
                    conta.Subclassificacao = null;
                }*/
                RepositoryService.Conta.Update(conta);

                if (!TmpConta.Status.Equals(conta.Status) && conta.Status.HasValue)
                {
                    if (conta.Status.Value == (int)Enum.StateCode.Ativo)
                    {
                        this.MudarStatusConta(TmpConta.ID.Value, Enum.Conta.StateCode.Ativo, Enum.Conta.StatusCode.Ativo);
                    }
                    else
                    {
                        this.MudarStatusConta(TmpConta.ID.Value, Enum.Conta.StateCode.Inativo, Enum.Conta.StatusCode.Inativo);
                    }

                }

                return conta;
            }
            else
            {
                conta.ID = RepositoryService.Conta.Create(conta);
            }

            return conta;
        }

        public void ColocarMascara()
        {
            List<Domain.Model.Conta> contas = new List<Domain.Model.Conta>();
            int count;
            do
            {
                contas = RepositoryService.Conta.ListarContasSemMascara();
                count = 0;
                foreach (var item in contas)
                {
                    if (item.CpfCnpj.Length >= 11)
                    {
                        item.CpfCnpj = item.CpfCnpj.InputMask();
                        if (item.CpfCnpj.Contains("."))
                        {
                            item.IntegrarNoPlugin = true;
                            this.Persistir(item);
                            count++;
                        }
                    }
                }
            } while (contas.Count >= 5000 || count > 0);
        }

        private Model.Conta AdesaoRevenda(Model.Conta conta)
        {
            var listaUnidadeNegocioProgramaPci = RepositoryService.UnidadeNegocio.ListarPorParticipaProgramaPci(true, "businessunitid", "name");

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas);

            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas + ") não encontrado!");
            }

            if (parametroGlobal.Categoria == null)
            {
                throw new ArgumentException("(CRM) A Categoria não está peenchida no Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.CategoriaParaAdesaoRevendas + ") não encontrado!");
            }

            var listaCategoriasDoCanal = RepositoryService.CategoriasCanal.ListarPor(conta);


            foreach (var item in listaUnidadeNegocioProgramaPci)
            {
                var inativar = listaCategoriasDoCanal.Find(x => x.UnidadeNegocios.Id == item.ID.Value
                                                            && x.Status.Value == (int)Domain.Enum.CategoriaCanal.StateCode.Ativado
                                                            && (x.Categoria.Id != parametroGlobal.Categoria.Id || x.Classificacao.Id != conta.Classificacao.Id || x.SubClassificacao.Id != conta.Subclassificacao.Id));

                if (inativar != null)
                {
                    RepositoryService.CategoriasCanal.SetState(inativar, (int)Domain.Enum.CategoriaCanal.StateCode.Desativado);
                }

                var existente = listaCategoriasDoCanal.Find(x => x.UnidadeNegocios.Id == item.ID.Value && x.Categoria.Id == parametroGlobal.Categoria.Id && x.Classificacao.Id == conta.Classificacao.Id && x.SubClassificacao.Id == conta.Subclassificacao.Id);

                if (existente == null)
                {
                    RepositoryService.CategoriasCanal.Create(new CategoriasCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                    {
                        Canal = new SDKore.DomainModel.Lookup(conta.ID.Value, conta.RazaoSocial, SDKore.Crm.Util.Utility.GetEntityName(conta)),
                        Categoria = parametroGlobal.Categoria,
                        Classificacao = conta.Classificacao,
                        Nome = item.Nome,
                        SubClassificacao = conta.Subclassificacao,
                        UnidadeNegocios = new SDKore.DomainModel.Lookup(item.ID.Value, item.Nome, SDKore.Crm.Util.Utility.GetEntityName(item))
                    });
                }
                else
                {
                    if (existente.Status.Value == (int)Domain.Enum.CategoriaCanal.StateCode.Desativado)
                    {
                        RepositoryService.CategoriasCanal.SetState(existente, (int)Domain.Enum.CategoriaCanal.StateCode.Ativado);
                    }
                }
            }

            RepositoryService.Conta.Update(new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
            {
                ID = conta.ID,
                ParticipantePrograma = (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim
            });

            return conta;
        }

        public Guid Criar(Model.Conta conta)
        {
            return RepositoryService.Conta.Create(conta);
        }

        public Model.Conta BuscaConta(Guid contaId)
        {
            return RepositoryService.Conta.Retrieve(contaId);
        }

        public Model.Conta BuscaContaPorCpfCnpj(string cpfCnpj, int tipoConstituicao)
        {
            return RepositoryService.Conta.ObterPor(cpfCnpj, tipoConstituicao);
        }

        public Model.Conta BuscaContaPorCpfCnpj(string cpfCnpj)
        {
            return RepositoryService.Conta.ObterPor(cpfCnpj);
        }

        public Model.Conta BuscaOutraContaPorCpfCnpj(string cpfCnpj, Guid id, int tipoConstituicao)
        {
            return RepositoryService.Conta.ObterOutraContaPorCpfCnpj(cpfCnpj, id, tipoConstituicao);
        }


        public Model.Conta CriarRevendaSellout(Model.Conta conta, Domain.Integracao.MSG0164 msg0164)
        {
            if (conta != null)
            {
                Model.Conta contaCadastrada = BuscaContaPorCpfCnpj(conta.CpfCnpj, conta.TipoConstituicao.Value);
                bool atualizou = false;

                if (contaCadastrada != null)
                {
                    return contaCadastrada;
                }

                var sefazViewModel = Preencher(conta);

                try
                {
                    sefazViewModel = msg0164.Enviar(sefazViewModel);

                    atualizou = Preencher(sefazViewModel, ref conta);
                }
                catch { }

                if (!atualizou)
                {
                    AtualizaInformacoesCep(ref conta);
                    atualizou = true;
                }

                if (atualizou)
                {
                    conta.ID = Criar(conta);
                }
            }

            return conta;
        }

        public bool AtualizaInformacoesCep(ref Model.Conta conta)
        {
            if (!string.IsNullOrEmpty(conta.Endereco1CEP))
            {
                var enderecoCep = new EnderecoServices(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).BuscaCep(conta.Endereco1CEP);

                if (enderecoCep != null)
                {
                    conta.Endereco1Bairro = enderecoCep.Bairro.Truncate(30);
                    conta.Endereco1Bairro1 = enderecoCep.Bairro.Truncate(30);
                    conta.Endereco2Bairro = enderecoCep.Bairro.Truncate(30);
                    conta.Endereco2Bairro2 = enderecoCep.Bairro.Truncate(30);
                    conta.Endereco1CEP = enderecoCep.CEP;
                    conta.Endereco2CEP = enderecoCep.CEP;
                    conta.Endereco1Cidade = enderecoCep.NomeCidade.Truncate(25);
                    conta.Endereco2Cidade = enderecoCep.NomeCidade.Truncate(25);
                    conta.Endereco1Rua = enderecoCep.Endereco.Truncate(35);
                    conta.Endereco1Rua1 = enderecoCep.Endereco.Truncate(35);
                    conta.Endereco2Rua = enderecoCep.Endereco.Truncate(35);
                    conta.Endereco2Rua2 = enderecoCep.Endereco.Truncate(35);

                    if (enderecoCep.CodigoIBGE.HasValue)
                    {
                        bool atualizou = AtualizarInformacoesIBGE(enderecoCep.CodigoIBGE.Value, ref conta);

                        return atualizou;
                    }
                }
            }

            return false;
        }

        private bool AtualizarInformacoesIBGE(int codigoIbge, ref Model.Conta conta)
        {
            IbgeViewModel ibgeViewModel = RepositoryService.Municipio.ObterIbgeViewModelPor(codigoIbge);

            if (ibgeViewModel != null)
            {
                conta.Endereco1Municipioid = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.CidadeId,
                    Name = ibgeViewModel.CidadeNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco2Municipioid = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.CidadeId,
                    Name = ibgeViewModel.CidadeNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco1Estadoid = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.EstadoId,
                    Name = ibgeViewModel.EstadoNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco2Estadoid = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.EstadoId,
                    Name = ibgeViewModel.EstadoNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco1Pais = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.PaisId,
                    Name = ibgeViewModel.PaisNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco2Pais = new SDKore.DomainModel.Lookup()
                {
                    Id = ibgeViewModel.PaisId,
                    Name = ibgeViewModel.PaisNome,
                    Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Municipio>()
                };

                conta.Endereco1Estado = ibgeViewModel.EstadoNome;
                conta.Endereco1Pais1 = ibgeViewModel.PaisNome;
                conta.Endereco2Estado = ibgeViewModel.EstadoNome;
                conta.Endereco2Pais2 = ibgeViewModel.PaisNome;

                return true;
            }

            return false;
        }

        public Model.Conta BuscarPorCodigoEmitente(string codEmitente)
        {
            return RepositoryService.Conta.ObterPorCodigoEmitente(codEmitente);
        }

        public bool MudarStatusConta(Guid id, Intelbras.CRM2013.Domain.Enum.Conta.StateCode stateCode, Intelbras.CRM2013.Domain.Enum.Conta.StatusCode statusCode)
        {
            return RepositoryService.Conta.AlterarStatus(id, stateCode, statusCode);
        }

        public List<Model.Conta> ListarTodasContas(params string[] columns)
        {
            return RepositoryService.Conta.ListarTudo(columns);
        }

        public List<Model.Conta> ListarDistribuidoresSellOut(Guid id, DateTime dataInicio, DateTime? dataFim, params string[] columns)
        {
            return RepositoryService.Conta.ListarTudo(columns);
        }

        public List<Model.Conta> ListarContas(ref int pagina, int contagem, out bool moreRecords)
        {
            return RepositoryService.Conta.ListarContas(ref pagina, contagem, out moreRecords);
        }

        public List<Model.Conta> ListarContasParticipantes()
        {
            return RepositoryService.Conta.ListarMatrizesParticipantes();
        }

        public List<Model.Conta> ListarContasParticipantesMAtrizEFilial()
        {
            return RepositoryService.Conta.ListarContasParticipantesMAtrizEFilial();
        }

        public List<Model.Conta> ListarContasParticipantesMatriz()
        {
            return RepositoryService.Conta.ListarContasParticipantesMAtrizEFilial();
        }
        public void AtualizaContasRecategorizar()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            var dtMesAnterior = DateTime.Today.AddMonths(-2);
            dtMesAnterior = new DateTime(dtMesAnterior.Year, dtMesAnterior.Month, DateTime.DaysInMonth(dtMesAnterior.Year, dtMesAnterior.Month));

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            var contas = RepositoryService.Conta.ListarContasRecategorizar(dtMesAnterior);
            textoEmail += "<b> Contas para Recategorizar: </b>" + contas.Count + " <br />";

            if (contas.Count > 0)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Recategorização");

                ws.Cell(1, 1).Value = "Conta";
                ws.Cell(1, 2).Value = "Sucesso";
                ws.Cell(1, 3).Value = "Erro";

                int linha = 2;

                string dataAtual = DateTime.Now.ToString();
                dataAtual = dataAtual.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "RECATEGORIZA_REVENDA_" + dataAtual + ".xlsx";

                string data = DateTime.Now.ToString();
                foreach (var conta in contas)
                {
                    //Excel - Coluna Conta
                    ws.Cell(linha, 1).Value = conta.NomeFantasia;
                    try
                    {
                        conta.IntegrarNoPlugin = true;
                        conta.Categorizar = (int)Enum.Conta.Categorizacao.Recategorizar;
                        RepositoryService.Conta.Update(conta);
                        //Excel - Coluna Sucesso
                        ws.Cell(linha, 2).Value = "Sim";
                    }
                    catch (Exception ex)
                    {
                        SDKore.Helper.Error.Create("Problemas ao marcar recategorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        erros += "<b> - </b> Problemas ao marcar para Recategorizar a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";
                        qtderro++;
                        //Excel - Coluna Erro
                        ws.Cell(linha, 3).Value = ex.Message;
                    }
                    linha++;
                }


                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                excel.SaveAs("c:\\temp\\" + nomeArquivo);
                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Marcar para Recategorizar", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
        }
        public void AtualizaContasCategorizar(DateTime dtMesAnterior, DateTime dtMesAtual)
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            var contas = RepositoryService.Conta.ListarContasCategorizar(dtMesAnterior, dtMesAtual);
            textoEmail += "<b> Contas para categorizar: </b>" + contas.Count + " <br />";

            if (contas.Count > 0)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Categorização");

                ws.Cell(1, 1).Value = "Conta";
                ws.Cell(1, 2).Value = "Sucesso";
                ws.Cell(1, 3).Value = "Erro";

                int linha = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "CATEGORIZACAO_REVENDA_" + data + ".xlsx";

                foreach (var conta in contas)
                {
                    //Excel - Coluna Conta
                    ws.Cell(linha, 1).Value = conta.NomeFantasia;
                    try
                    {
                        conta.IntegrarNoPlugin = true;
                        conta.Categorizar = (int)Enum.Conta.Categorizacao.Categorizar;
                        RepositoryService.Conta.Update(conta);
                        //Excel - Coluna Sucesso
                        ws.Cell(linha, 2).Value = "Sim";
                    }
                    catch (Exception ex)
                    {
                        SDKore.Helper.Error.Create("Problemas ao marcar categorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        erros += "<b> - </b> Problemas ao marcar para categorizar a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";

                        //Excel - Coluna Sucesso
                        ws.Cell(linha, 2).Value = "Não";

                        //Excel - Coluna Erro
                        ws.Cell(linha, 3).Value = ex.Message;
                        qtderro++;
                    }


                    linha++;
                }
                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Marcar para Categorizar", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
        }

        public void AtualizaContasCategorizacao()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            int qtdsucesso = 0;
            int revendasAlteradas = 0;
            int revendasIguais = 0;
            string razaoSocial = "";
            int contContas = 0;
            int contContasTotal = 0;

            bool bCategorizou = false;
            bool trocouCategoria = false;

            //Busca qual ambiente está sendo executado e anexo no e-mail
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion
            
            #region Recupera valor do parâmetro global para buscar o Número Mínimo de Colaboradores Treinados para Revenda Ouro
            var parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.NumeroMinimoColaboradoresTreinadosRevendaOuro);
            #endregion

            #region Recupera valor do parâmetro global "Data da Próxima Caterização Revenda".

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.DataPróximaCaterizaçãoRevenda);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.DataPróximaCaterizaçãoRevenda + ") não encontrado!");
                erros += "<b> - </b> Problemas ao encontrar Parâmetro Global: " + (int)Domain.Enum.TipoParametroGlobal.DataPróximaCaterizaçãoRevenda + " não foi encontrado.<br />";
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Categorização");

                ws.Cell(1, 1).Value = "Razão social";
                ws.Cell(1, 2).Value = "Cnpj";
                ws.Cell(1, 3).Value = "Código Emitente";
                ws.Cell(1, 4).Value = "Estado";
                ws.Cell(1, 5).Value = "Nº Técnicos Treinados";
                ws.Cell(1, 6).Value = "Assistência Técnica";
                ws.Cell(1, 7).Value = "Total Faturamento";
                ws.Cell(1, 8).Value = "Linha de Corte";
                ws.Cell(1, 9).Value = "Categoria";
                ws.Cell(1, 10).Value = "Categoria após categorização";
                ws.Cell(1, 11).Value = "Sucesso";
                ws.Cell(1, 12).Value = "Erro";

                int linha = 2;
                int linhaUpdate = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "CATEGORIZACAO_REVENDA_" + data + ".xlsx";

                if (dataConsulta.Date == DateTime.Now.Date)
                {
                    #region Recupera valor das categorias Ouro, Prata e Bronze".
                    var categoriaOuro = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Ouro");
                    var categoriaPrata = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Prata");
                    var categoriaBronze = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Bronze");
                    var categoriaRegistrada = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Registrada");


                    #endregion

                    #region Recuperar registros na entidade "Conta" em que o atributo Categorizar seja igual a "Categorizar".
                    var contas = RepositoryService.Conta.ListarContasCategorizacao();
                    textoEmail += "<b> Contas para serem categorizadas: </b>" + contas.Count + " <br />";
                    #endregion

                    DataTable faturamentoGlobal = new DataTable();
                    DataTable faturamentoLinhasCorte = new DataTable();
                    if (contas.Count > 0)
                    {
                        string contaId = "";
                        foreach (var conta in contas)
                        {
                            contaId = contaId + "'" + conta.ID + "'" + ",";
                        }
                        contaId = contaId.Remove(contaId.Length - 1);
                        //Busca faturamento Global das revendas no trimestre
                        faturamentoGlobal = RepositoryService.LinhaCorteDistribuidor.FaturamentoGlobalTrimestreDW(Helper.TrimestreAnteriorAno(),
                                                                                                                    Helper.TrimestreAnterior(),
                                                                                                                    contaId);

                        //Busca as linhas de corte por unidade e global (unidade ADM) das revendas
                        faturamentoLinhasCorte = RepositoryService.LinhaCorteDistribuidor.FaturamentoLinhasCorteDWTrimestre(contaId, "1");
                    }
                    
                    var lstConta = new List<Model.Conta>();

                    foreach (var conta in contas)
                    {
                        try
                        {
                            bCategorizou = false;
                            trocouCategoria = false;
                            razaoSocial = conta.RazaoSocial;
                            var categoriaAnterior = conta.Categoria.Id;

                            //Excel - Coluna Conta
                            ws.Cell(linha, 1).Value = razaoSocial;
                            ws.Cell(linha, 2).Value = conta.CpfCnpj;
                            ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                            ws.Cell(linha, 4).Value = conta.Endereco1Estado;
                            ws.Cell(linha, 6).Value = ((bool)conta.AssistenciaTecnica) ? "Sim" : "Não";

                            if (conta.Categoria != null)
                            {
                                var query1 = from p in faturamentoGlobal.AsEnumerable()
                                             where p.Field<Guid>("CD_Revenda_SellOut") == conta.ID
                                             select p;

                                decimal faturamentoTotalTrimestre = 0;
                                foreach (var i in query1)
                                {
                                    faturamentoTotalTrimestre = i.Field<decimal>("FaturamentoTOTAL");
                                }

                                ws.Cell(linha, 7).Value = faturamentoTotalTrimestre;

                                //Excel - Coluna Categoria
                                ws.Cell(linha, 9).Value = conta.Categoria.Name;

                                //Retorna Valor Unidade Negócio ADM das categorias
                                decimal linhadecorteAdmOuro = 0;
                                decimal linhadecorteAdmPrata = 0;
                                decimal linhadecorteAdmBronze = 0;

                                var query = from p in faturamentoLinhasCorte.AsEnumerable()
                                            where p.Field<Guid>("cd_guid") == conta.ID && p.Field<string>("CD_Unidade_Negocio") == "ADM"
                                            select p;

                                foreach (var i in query)
                                {
                                    if (i.Field<string>("TX_Categoria_Canal") == "Ouro")
                                    {
                                        linhadecorteAdmOuro = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                    else
                                    if (i.Field<string>("TX_Categoria_Canal") == "Prata")
                                    {
                                        linhadecorteAdmPrata = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                    else
                                    {
                                        linhadecorteAdmBronze = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                }

                                if(linhadecorteAdmBronze == 0 || linhadecorteAdmOuro == 0 || linhadecorteAdmPrata == 0)
                                {
                                    throw new System.ArgumentException("Não encontrado linha de corte no DW para a revenda");
                                }

                                conta.IntegrarNoPlugin = true;
                                /**
                                 * Chamado 125468  - Adicionado nava regra para torna-se
                                 * Ter pelo menos 2 técnicos treinados, com a modalidade do treinamento como Presencial, 
                                 * onde Status de Aprovação = Aprovado e que o campo Validade seja maior do que a data atual. 
                                 */
                                List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificado = RepositoryService.ColaboradorTreinadoCertificado.ListarPorCanalTreinamentosAprovadosValidos(conta.ID.Value);    
                                var tecnicosTreinados = lstColaboradorTreinadoCertificado.GroupBy(x => x.Contato.Id).ToArray().Count();
                                ws.Cell(linha, 5).Value = tecnicosTreinados;

                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria ouro
                                if (faturamentoTotalTrimestre >= linhadecorteAdmOuro)
                                {
                                    ws.Cell(linha, 8).Value = linhadecorteAdmOuro;
                                    if (tecnicosTreinados >= Int32.Parse(parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro.Valor))
                                    {
                                        bCategorizou = true;
                                        if (conta.Categoria.Id != categoriaOuro)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaOuro;
                                            conta.Categoria.Name = "Ouro";
                                            //Excel - Coluna Categoria após categorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                    }
                                }
                                #endregion
                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria prata
                                if (!bCategorizou)
                                {
                                    if (faturamentoTotalTrimestre >= linhadecorteAdmPrata)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmPrata;
                                        bCategorizou = true;
                                        if (conta.Categoria.Id != categoriaPrata)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaPrata;
                                            conta.Categoria.Name = "Prata";
                                            //Excel - Coluna Categoria após categorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;

                                        }
                                    }
                                }
                                #endregion
                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria bronze
                                if (!bCategorizou)
                                {
                                    if (faturamentoTotalTrimestre >= linhadecorteAdmBronze)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                        bCategorizou = true;
                                        if (conta.Categoria.Id != categoriaBronze)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaBronze;
                                            conta.Categoria.Name = "Bronze";
                                            //Excel - Coluna Categoria após categorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;

                                        }
                                    }
                                }
                                #endregion
                                //Registrada
                                if (!bCategorizou)
                                {
                                    if ((bool)conta.AssistenciaTecnica)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                        if (conta.Categoria.Id != categoriaBronze)
                                        {
                                            trocouCategoria = true;
                                            bCategorizou = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaBronze;
                                            conta.Categoria.Name = "Bronze";
                                        }
                                    }
                                    else
                                    {
                                        if (conta.Categoria.Id != categoriaRegistrada)
                                        {
                                            trocouCategoria = true;
                                            bCategorizou = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Name = "Registrada";
                                            conta.Categoria.Id = RepositoryService.Categoria.ObterPor("4").ID.Value;
                                        }
                                    }

                                    //Excel - Coluna Categoria após categorização
                                    ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                }

                                conta.Categorizar = (int)Enum.Conta.Categorizacao.Categorizada;

                                if (trocouCategoria)
                                {

                                    var contatos = RepositoryService.Contato.ListarAssociadosA(conta.ID.ToString());
                                    var contatoService = new Domain.Servicos.ContatoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                    foreach (var contato in contatos)
                                    {
                                        contatoService.EnviaContatoFielo(contato, true);
                                    }
                                    conta.IntegraIntelbrasPontua = true;
                                    revendasAlteradas++;
                                }
                                else
                                {
                                    conta.IntegrarNoPlugin = true;
                                    revendasIguais++;
                                }
                                #region Altera revenda para categorizada

                                if (conta.Categoria != null)
                                {
                                    //Excel - Coluna Categoria após Categorização
                                    ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                }
                                else
                                {
                                    //Excel - Coluna Categoria após Categorização
                                    ws.Cell(linha, 10).Value = "Sem categoria";

                                    //Excel - Coluna ERRO
                                    ws.Cell(linha, 12).Value = "Canal com a Categoria em branco";
                                }
                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                linha++;
                                #endregion
                            }
                        }
                        catch (Exception ex)
                        {
                            SDKore.Helper.Error.Create("Problemas ao categorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                            erros += "<b> - </b> Problemas ao categorizar a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";

                            //Excel - Coluna Sucesso
                            ws.Cell(linha, 11).Value = "Não";

                            qtderro++;
                            //Excel - Coluna Sucesso
                            ws.Cell(linha, 12).Value = "(CRM)ERRO Categorizacao: " + ex.Message;
                            linha++;
                        }
                        contContas++;
                        contContasTotal++;
                        lstConta.Add(conta); //Adiciona as contas na lista

                        if (contContas == 10 || contas.Count == contContasTotal) //Retriever multiplo tem limite de 1000 registros, controla a quantidade de registros no objeto
                        {
                            ExecuteMultipleResponse responseWithContinueOnError = RepositoryService.Conta.UpdateMultiplos(lstConta);

                            contContas = 0;
                            lstConta.Clear();
                            //Verificar quais contas deram erros
                            if (responseWithContinueOnError.Responses.Count > 0)
                            {
                                int i = 0;
                                foreach (var responseItem in responseWithContinueOnError.Responses)
                                {
                                    if (responseItem.Fault != null)
                                    {
                                        Model.Conta contaErro = contas[i];
                                        SDKore.Helper.Error.Create("Problemas ao Categorizar a conta " + contaErro.NomeFantasia + responseItem.Fault.Message, System.Diagnostics.EventLogEntryType.Error);
                                        erros += "<b> - </b> Problemas ao Categorizar a conta " + contaErro.NomeFantasia + ". <b> Erro: </b>" + responseItem.Fault.Message + "<br />";

                                        //Excel - Coluna Sucesso
                                        ws.Cell(linhaUpdate, 11).Value = "Não";
                                        //Excel - Coluna Sucesso
                                        ws.Cell(linhaUpdate, 12).Value = "(CRM)ERRO Categorização: " + responseItem.Fault.Message;
                                        qtderro++;
                                    }
                                    else
                                    {
                                        //Excel - Coluna Sucesso
                                        ws.Cell(linhaUpdate, 11).Value = "Sim";
                                        qtdsucesso++;
                                    }
                                    linhaUpdate++;
                                    i++;
                                }
                            }
                        }
                    }

                    #region Atualiza data da próxima categorização

                    parametroGlobal.Valor = Helper.ProximoMes().Day.ToString() + "/" + Helper.ProximoMes().Month.ToString() + "/" + Helper.ProximoMes().Year.ToString();
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);

                    #endregion

                    excel.SaveAs("c:\\temp\\" + nomeArquivo);

                    textoEmail += "<b> Erros: </b><br />";
                    textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                    textoEmail += erros;
                    textoEmail += "<b> Revendas que trocaram de categoria: </b>" + revendasAlteradas + "<br />";
                    textoEmail += "<b> Revendas que permaneceram na mesma categoria: </b>" + revendasIguais + "<br />";
                    textoEmail += "<b> Total de contas categorizadas: </b>" + qtdsucesso + "<br />";
                    textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                    RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Categoriza Revendas (" + TipoAmbiente + ")", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));

                }
            }
        }

        public void AtualizaContasReCategorizacao()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            int qtdsucesso = 0;
            int revendasAlteradas = 0;
            int revendasIguais = 0;
            string razaoSocial = "";
            int contContas = 0;
            int contContasTotal = 0;

            bool bCategorizou = false;
            bool trocouCategoria = false;

            //Busca qual ambiente está sendo executado e anexo no e-mail
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            #region Recupera valor do parâmetro global para buscar o Número Mínimo de Colaboradores Treinados para Revenda Ouro
            var parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.NumeroMinimoColaboradoresTreinadosRevendaOuro);
            #endregion

            #region Recupera valor do parâmetro global "Data da Próxima ReCaterização Revenda".

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.DataPróximaReCaterizaçãoRevenda);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.DataPróximaReCaterizaçãoRevenda + ") não encontrado!");
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion

            if (dataConsulta.Date == DateTime.Now.Date)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Recategorização");
                ws.Cell(1, 1).Value = "Razão social";
                ws.Cell(1, 2).Value = "Cnpj";
                ws.Cell(1, 3).Value = "Código Emitente";
                ws.Cell(1, 4).Value = "Estado";
                ws.Cell(1, 5).Value = "Nº Técnicos Treinados";
                ws.Cell(1, 6).Value = "Assistência Técnica";
                ws.Cell(1, 7).Value = "Total Faturamento";
                ws.Cell(1, 8).Value = "Linha de Corte";
                ws.Cell(1, 9).Value = "Categoria";
                ws.Cell(1, 10).Value = "Categoria após categorização";
                ws.Cell(1, 11).Value = "Sucesso";
                ws.Cell(1, 12).Value = "Erro";

                int linha = 2;
                int linhaUpdate = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "RECATEGORIZACAO_REVENDA_" + data + ".xlsx";

                if (dataConsulta.Date == DateTime.Now.Date)
                {
                    #region Recupera valor das categorias Ouro, Prata e Bronze".
                    var categoriaOuro = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Ouro");
                    var categoriaPrata = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Prata");
                    var categoriaBronze = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Bronze");
                    var categoriaRegistrada = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Registrada");
                    #endregion

                    #region Recuperar registros na entidade "Conta" em que o atributo Categorizar seja igual a "ReCategorizar".
                    var contas = RepositoryService.Conta.ListarContasReCategorizacao();

                    //Retorna faturamento Ultimo Semestre DW
                    DataTable faturamentoGlobal = new DataTable();
                    DataTable faturamentoLinhasCorte = new DataTable();
                    if (contas.Count > 0)
                    {
                        string contaId = "";
                        foreach (var conta in contas)
                        {
                            contaId = contaId + "'" + conta.ID + "'" + ",";
                        }
                        contaId = contaId.Remove(contaId.Length - 1);
                        //Busca faturamento Global das revendas no semestre
                        faturamentoGlobal = RepositoryService.LinhaCorteDistribuidor.FaturamentoGlobalSemestreDW(Helper.SemestreAnteriorAno(),
                                                                                                                        Helper.TrimestreAnteriorAno(),
                                                                                                                        Helper.SemestreAnterior(),
                                                                                                                        contaId);

                        //Busca as linhas de corte por unidade e global (unidade ADM) das revendas
                        faturamentoLinhasCorte = RepositoryService.LinhaCorteDistribuidor.FaturamentoLinhasCorteDWTrimestre(contaId, "2");
                    }
                    if (faturamentoGlobal.Rows.Count > 0)
                    {
                        textoEmail += "<b> Contas para serem Recategorizadas: </b>" + contas.Count + " <br />";
                        #endregion

                        var lstConta = new List<Model.Conta>();

                        foreach (var conta in contas)
                        {
                            try
                            {
                                bCategorizou = false;
                                trocouCategoria = false;
                                razaoSocial = conta.RazaoSocial;
                                var categoriaAnterior = conta.Categoria.Id;

                                //Excel - Coluna Conta
                                ws.Cell(linha, 1).Value = razaoSocial;
                                ws.Cell(linha, 2).Value = conta.CpfCnpj;
                                ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                                ws.Cell(linha, 4).Value = conta.Endereco1Estado;
                                ws.Cell(linha, 6).Value = ((bool)conta.AssistenciaTecnica) ? "Sim" : "Não";

                                if (conta.Categoria != null)
                                {
                                    var query1 = from p in faturamentoGlobal.AsEnumerable()
                                                 where p.Field<Guid>("CD_Revenda_SellOut") == conta.ID
                                                 select p;

                                    decimal faturamentoTotalTrimestre = 0;
                                    foreach (var i in query1)
                                    {
                                        faturamentoTotalTrimestre = i.Field<decimal>("FaturamentoTOTAL");
                                    }

                                    ws.Cell(linha, 7).Value = faturamentoTotalTrimestre;

                                    //Excel - Coluna Categoria
                                    ws.Cell(linha, 9).Value = conta.Categoria.Name;

                                    //Retorna Valor Unidade Negócio ADM das categorias
                                    decimal linhadecorteAdmOuro = 0;
                                    decimal linhadecorteAdmPrata = 0;
                                    decimal linhadecorteAdmBronze = 0;

                                    var query = from p in faturamentoLinhasCorte.AsEnumerable()
                                                where p.Field<Guid>("cd_guid") == conta.ID && p.Field<string>("CD_Unidade_Negocio") == "ADM"
                                                select p;

                                    foreach (var i in query)
                                    {
                                        if (i.Field<string>("TX_Categoria_Canal") == "Ouro")
                                        {
                                            linhadecorteAdmOuro = i.Field<decimal>("NM_Vl_Linha_Corte");
                                        }
                                        else
                                        if (i.Field<string>("TX_Categoria_Canal") == "Prata")
                                        {
                                            linhadecorteAdmPrata = i.Field<decimal>("NM_Vl_Linha_Corte");
                                        }
                                        else
                                        {
                                            linhadecorteAdmBronze = i.Field<decimal>("NM_Vl_Linha_Corte");
                                        }
                                    }

                                    if(linhadecorteAdmBronze == 0 || linhadecorteAdmOuro == 0 || linhadecorteAdmPrata == 0)
                                    {
                                        throw new System.ArgumentException("Não encontrado linha de corte no DW para a revenda");
                                    }

                                    /**
                                     * Chamado 125468  - Adicionado nava regra para torna-se
                                     * Ter pelo menos 2 técnicos treinados, com a modalidade do treinamento como Presencial, 
                                     * onde Status de Aprovação = Aprovado e que o campo Validade seja maior do que a data atual. 
                                     */
                                    List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificado = RepositoryService.ColaboradorTreinadoCertificado.ListarPorCanalTreinamentosAprovadosValidos(conta.ID.Value);    
                                    var tecnicosTreinados = lstColaboradorTreinadoCertificado.GroupBy(x => x.Contato.Id).ToArray().Count();
                                    ws.Cell(linha, 5).Value = tecnicosTreinados;

                                    #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria ouro
                                    //Verifica se o faturamento da revenda é maior ou igual que a meta global Ouro
                                    if (faturamentoTotalTrimestre >= linhadecorteAdmOuro)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmOuro;
                                        if (tecnicosTreinados >= Int32.Parse(parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro.Valor))
                                        {
                                            bCategorizou = true;
                                            if (conta.Categoria.Id != categoriaOuro)
                                            {
                                                trocouCategoria = true;
                                                conta.IntegrarNoPlugin = false;
                                                conta.Categoria.Id = categoriaOuro;
                                                conta.Categoria.Name = "Ouro";
                                                //Excel - Coluna Categoria após Recategorização
                                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria prata
                                    if (!bCategorizou)
                                    {
                                        if (faturamentoTotalTrimestre >= linhadecorteAdmPrata)
                                        {
                                            ws.Cell(linha, 8).Value = linhadecorteAdmPrata;
                                            bCategorizou = true;
                                            var codigoCategoriaPrata = Convert.ToInt32(RepositoryService.Categoria.ObterPor(conta.Categoria.Id).CodigoCategoria);
                                            if (VerificaDataAdesao(conta.DataAdesao.Value) || codigoCategoriaPrata > 2)
                                            {
                                                trocouCategoria = true;
                                                conta.IntegrarNoPlugin = false;
                                                conta.Categoria.Id = categoriaPrata;
                                                conta.Categoria.Name = "Prata";                                                                
                                                //Excel - Coluna Categoria após recategorização
                                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                            }
                                            else
                                            {
                                                bCategorizou = true;
                                                //Excel - Coluna Categoria após recategorização
                                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria bronze
                                    if (!bCategorizou)
                                    {
                                        if (faturamentoTotalTrimestre >= linhadecorteAdmBronze)
                                        {
                                            ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                            bCategorizou = true;
                                            var codigoCategoriaBronze = Convert.ToInt32(RepositoryService.Categoria.ObterPor(conta.Categoria.Id).CodigoCategoria);
                                            if (VerificaDataAdesao(conta.DataAdesao.Value) || codigoCategoriaBronze > 3)
                                            {
                                                trocouCategoria = true;
                                                conta.IntegrarNoPlugin = false;
                                                conta.Categoria.Id = categoriaBronze;
                                                conta.Categoria.Name = "Bronze";
                                                //Excel - Coluna Categoria após recategorização
                                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                            }
                                            else
                                            {
                                                //Excel - Coluna Categoria após recategorização
                                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                            }
                                        }
                                    }
                                    #endregion
                                    //Registrada
                                    if (!bCategorizou)
                                    {
                                        if (VerificaDataAdesao(conta.DataAdesao.Value))
                                        {
                                            if ((bool)conta.AssistenciaTecnica)
                                            {
                                                ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                                trocouCategoria = true;
                                                bCategorizou = true;
                                                conta.IntegrarNoPlugin = false;
                                                conta.Categoria.Id = categoriaBronze;
                                                conta.Categoria.Name = "Bronze";
                                            }
                                            else
                                            {
                                                if (conta.Categoria.Id != categoriaRegistrada)
                                                {
                                                    trocouCategoria = true;
                                                    bCategorizou = true;
                                                    conta.IntegrarNoPlugin = false;
                                                    conta.Categoria.Name = "Registrada";
                                                    conta.Categoria.Id = RepositoryService.Categoria.ObterPor("4").ID.Value;
                                                }
                                            }
                                            //Excel - Coluna Categoria após recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                    }
                                    #region Altera revenda para Categorizada
                                    conta.Categorizar = (int)Enum.Conta.Categorizacao.Categorizada;
                                    #endregion

                                    if (trocouCategoria)
                                    {
                                        var contatos = RepositoryService.Contato.ListarAssociadosA(conta.ID.ToString());
                                        var contatoService = new Domain.Servicos.ContatoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                        foreach (var contato in contatos)
                                        {
                                            contatoService.EnviaContatoFielo(contato, true);
                                        }

                                        conta.IntegraIntelbrasPontua = true;
                                        revendasAlteradas++;
                                    }
                                    else
                                    {
                                        conta.IntegrarNoPlugin = true;
                                        revendasIguais++;
                                    }
                                }

                                //Excel - Coluna Categoria após recategorização
                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                //Excel - Coluna Sucesso                                
                                linha++;
                            }
                            catch (Exception ex)
                            {
                                SDKore.Helper.Error.Create("Problemas ao Recategorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                                erros += "<b> - </b> Problemas ao Recategorizar a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";

                                //Excel - Coluna com Erro
                                ws.Cell(linha, 11).Value = "Não";

                                qtderro++;
                                //Excel - Coluna Erro
                                ws.Cell(linha, 12).Value = "(CRM)ERRO ReCategorizacao: " + ex.Message;
                                linha++;
                            }
                            contContas++;
                            contContasTotal++;
                            lstConta.Add(conta); //Adiciona as contas na lista

                            if (contContas == 10 || contas.Count == contContasTotal) //Retriever multiplo tem limite de 1000 registros, controla a quantidade de registros no objeto
                            {
                                ExecuteMultipleResponse responseWithContinueOnError = RepositoryService.Conta.UpdateMultiplos(lstConta);

                                contContas = 0;
                                lstConta.Clear();
                                //Verificar quais contas deram erros
                                if (responseWithContinueOnError.Responses.Count > 0)
                                {
                                    int i = 0;
                                    foreach (var responseItem in responseWithContinueOnError.Responses)
                                    {
                                        if (responseItem.Fault != null)
                                        {
                                            Model.Conta contaErro = contas[i];
                                            SDKore.Helper.Error.Create("Problemas ao Recategorizar a conta " + contaErro.NomeFantasia + responseItem.Fault.Message, System.Diagnostics.EventLogEntryType.Error);
                                            erros += "<b> - </b> Problemas ao Recategorizar a conta " + contaErro.NomeFantasia + ". <b> Erro: </b>" + responseItem.Fault.Message + "<br />";

                                            //Excel - Coluna Sucesso
                                            ws.Cell(linhaUpdate, 11).Value = "Não";
                                            //Excel - Coluna Sucesso
                                            ws.Cell(linhaUpdate, 12).Value = "(CRM)ERRO Recategorizacao: " + responseItem.Fault.Message;
                                            qtderro++;
                                        }
                                        else
                                        {
                                            //Excel - Coluna Sucesso
                                            ws.Cell(linhaUpdate, 11).Value = "Sim";
                                            qtdsucesso++;
                                        }
                                        linhaUpdate++;
                                        i++;
                                    }
                                }
                            }
                        }
                    }

                    #region Altera o valor do parâmetro global "Data da Próxima Recategorização de Revendas" para o primeiro dia do próxima semestre.

                    parametroGlobal.Valor = Helper.ProximoSemestre().Day.ToString() + "/" + Helper.ProximoTrimestre().Month.ToString() + "/" + Helper.ProximoTrimestre().Year.ToString();
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);

                    #endregion
                }

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Revendas que trocaram de categoria: </b>" + revendasAlteradas + "<br />";
                textoEmail += "<b> Revendas que permaneceram na mesma categoria: </b>" + revendasIguais + "<br />";
                textoEmail += "<b> Total de contas categorizadas: </b>" + qtdsucesso + "<br />";
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Recategoriza Revendas (" + TipoAmbiente + ")", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
        }

        private bool isAtualizaCategoriaConta(Model.Conta conta, string codigoCategoria)
        {
            var atualizou = false;
            var categoria = RepositoryService.Categoria.ObterPor(codigoCategoria);
            if (conta.Categoria.Id != categoria.ID.Value)
            {
                conta.IntegrarNoPlugin = false;
                conta.Categoria.Id = categoria.ID.Value;
                conta.Categoria.Name = categoria.Nome;
                RepositoryService.Conta.Update(conta);
                atualizou = true;
            }
            return atualizou;
        }

        private void atualizaRelacionamentosConta(Model.Conta conta, bool isCategorizacao)
        {
            #region Inativa todas as categorias do canal associadas cujo campo canal igual a Revenda     
            var categoriaCanalService = new CategoriaCanalService(this.RepositoryService);
            categoriaCanalService.InativarCategoriaDoCanal(conta);
            #endregion

            #region Inativa benefícios do canal
            var beneficiosCanalService = new BeneficioDoCanalService(this.RepositoryService);
            var beneficios = beneficiosCanalService.ListarPorConta(conta.ID.Value);
            foreach (var b in beneficios)
            {
                beneficiosCanalService.InativaBeneficioDoCanal(b);
            }
            #endregion

            #region inativa compromissos do canal
            CompromissosDoCanalService compromissosCanalService = new CompromissosDoCanalService(this.RepositoryService);
            compromissosCanalService.InativarCompromissosDoCanal(conta);
            #endregion

            #region Cria\atualiza registro de categoria do canal
            var categoriaCanal = categoriaCanalService.AtivarCategoriaDoCanal(conta);
            #endregion

            #region Cria beneficios e compromissos para o canal
            beneficiosCanalService.CriarBeneficiosECompromissosParaOCanal(conta, categoriaCanal);
            #endregion

            // executa somente na categorização 
            // OBS: Já estava assim na categorização, porém na recategorização não existia este trecho, resolvi não mexer nesta regra
            if (isCategorizacao)
            {
                #region Inativa os beneficios suspensos (categoria anterior)
                categoriaCanalService.InativarCategoriaDoCanal(conta);
                categoriaCanalService.AtivarCategoriaDoCanal(conta);
                #endregion
            }

        }

        private List<LinhaCorteDistribuidor> RetornaLinhasCortePelaConta(Model.Conta conta, Guid? categoriaId)
        {
            var municipio = RepositoryService.Municipio.ObterPor(conta.Endereco1Municipioid.Id);
            var capitalOuInterior = municipio.CapitalOuInterior.HasValue ? municipio.CapitalOuInterior : null;

            return RepositoryService.LinhaCorteDistribuidor.ListarPort(conta.Classificacao.Id, conta.Endereco1Estadoid.Id, capitalOuInterior, categoriaId);
        }

        private bool VerificaDataAdesao(DateTime dt)
        {
            var aux = DateTime.Now.Subtract(dt);

            if (aux.Days >= 365)
            {
                return true;
            }

            return false;
        }
        private decimal RetornaTotalFaturamentoUltimoTrimestreRevenda(DataTable dt)
        {
            decimal total = 0;

            foreach (DataRow item in dt.Rows)
            {
                total = total + Convert.ToDecimal(item["NM_Vl_Liquido"].ToString());
            }

            return total;
        }
        private decimal RetornaTotalFaturamentoUltimoSemestreRevenda(DataTable dt)
        {
            decimal total = 0;

            foreach (DataRow item in dt.Rows)
            {
                total = total + Convert.ToDecimal(item["NM_Vl_Liquido"].ToString());
            }

            return total;
        }
        private LinhaCorteDistribuidor RetornalinhaDeCorteAdministrativo(List<LinhaCorteDistribuidor> linhaDeCorte, string Categoria)
        {
            List<LinhaCorteDistribuidor> ll = new List<LinhaCorteDistribuidor>();
            foreach (var l in linhaDeCorte)
            {
                if (l.UnidadeNegocios.Name.Equals("ADMINISTRATIVO") && l.Categoria.Name.Equals(Categoria))
                {
                    ll.Add(l);
                }
            }
            return ll[0];
        }

        private bool VerificaLinhaDeCorteCategoria(DataTable faturamentoDW, List<LinhaCorteDistribuidor> lLinhaDeCorte)
        {
            bool atingiuMeta = false;

            foreach (var lcorte in lLinhaDeCorte)
            {
                foreach (DataRow item in faturamentoDW.Rows)
                {
                    if (item["CD_Unidade_Negocio"].ToString().Equals(RepositoryService.UnidadeNegocio.Retrieve(lcorte.UnidadeNegocios.Id, "itbc_chave_integracao").ChaveIntegracao))
                    {
                        if (Convert.ToDecimal(item["NM_Vl_Liquido"].ToString()) >= lcorte.LinhaCorteTrimestral)
                        {
                            atingiuMeta = true;
                        }
                    }
                }
            }

            return atingiuMeta;
        }
        public void AlteraAcessosExtranet()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            int qtdsucesso = 0;
            int totalContatos = 0;
            string contaAtual = "";
            string contatoAtual = "";
            Guid idAcessoContaAdesao = Guid.Empty;
            //Busca qual ambiente está sendo executado e anexo no e-mail

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            try
            {
                idAcessoContaAdesao = RepositoryService.AcessoExtranet.ObterAdesao().ID.Value;
            }
            catch
            (Exception ex)
            {
                SDKore.Helper.Error.Create("Problemas ao tentar recuperar a conta adesão" + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            //pega todas as contas atualizadas nos ultimos dias participantes do pci (discutir)
            var ContasAtualizar = RepositoryService.Conta.ListarContasAcessoExtranet();
            textoEmail += "<b> Contas que foram modificadas nos últimos 2 dias: </b>" + ContasAtualizar.Count + " <br />";
            if (ContasAtualizar.Count > 0)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Acessos a Extranet");

                ws.Cell(1, 1).Value = "Contato";
                ws.Cell(1, 2).Value = "Conta";
                ws.Cell(1, 3).Value = "Acesso a Extranet";
                ws.Cell(1, 4).Value = "Sucesso";
                ws.Cell(1, 5).Value = "Erro";

                int linha = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "ALTERA_ACESSO_EXTRANET_" + data + ".xlsx";
                // Recupera a lista de contatos associados a conta 
                foreach (var conta in ContasAtualizar)
                {
                    contaAtual = conta.NomeFantasia;
                    try
                    {
                        var contatosAssociados = RepositoryService.Contato.ListarAssociadosA(conta.ID.Value.ToString());
                        totalContatos += contatosAssociados.Count;

                        foreach (var contato in contatosAssociados)
                        {
                            contatoAtual = contato.NomeCompleto;

                            //Excel - Coluna Contato
                            ws.Cell(linha, 1).Value = contatoAtual;

                            //Excel - Coluna Conta
                            ws.Cell(linha, 2).Value = contaAtual;

                            // Recuperar o acesso da extranet ativo
                            var contatoAcessoExtranet = RepositoryService.AcessoExtranetContato.ListarPor(conta.ID.Value, contato.ID.Value);
                            //verifica se o contato tem acesso a extranet
                            if (contatoAcessoExtranet.Count > 0)
                            {
                                var accessoExtranet = RepositoryService.AcessoExtranet.ObterPor(contatoAcessoExtranet[0].AcessoExtranetid.Id);

                                // Verifica se a Categoria Classificação do Acesso á Extranet(itbc_acessoextranet) do Acesso à Extranet do Contato recuperado 
                                // é igual a Categoria e Classificação da Conta associada ao Contato
                                if (conta.Classificacao.Id != accessoExtranet.Classificacao.Id ||
                                    conta.Categoria.Id != accessoExtranet.Categoria.Id)
                                {
                                    //var acessoExtranet = contatoAcessoExtranet.a
                                    //Verifica se o Nome do Acesso à Extranet (itbc_acessoextranet) do Acessop à Extranet do Contato recuperado contém a palavra "Gestor"
                                    if (contatoAcessoExtranet[0].AcessoExtranetid.Name.Contains("Gestor"))
                                    {
                                        //Recupera acesso à extranet Gestor para classificação e categoria do canal
                                        if (RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Gestor") != null)
                                        {
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Gestor").ID.Value;
                                            //Excel - Coluna Acesso a Extranet
                                            ws.Cell(linha, 3).Value = "Gestor";
                                        }
                                        else
                                        {
                                            //se não encontrar acesso extranet para a classificacao e categoria atual da conta seta o acesso como Conta Adesão 
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = idAcessoContaAdesao;
                                            ws.Cell(linha, 3).Value = "Conta Adesão";
                                        }
                                    }
                                    else if (contatoAcessoExtranet[0].AcessoExtranetid.Name.Contains("Operacional"))
                                    {
                                        if (RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Operacional") != null)
                                        {
                                            //Recupera Acesso à Extranet (itbc_acessoextranet)utilizando os seguintes parâmetros:
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Operacional").ID.Value;
                                            //Excel - Coluna Acesso a Extranet
                                            ws.Cell(linha, 3).Value = "Operacional";
                                        }
                                        else
                                        {
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = idAcessoContaAdesao;
                                            ws.Cell(linha, 3).Value = "Conta Adesão";
                                        }
                                    }
                                    else
                                    {
                                        if (RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Basico") != null)
                                        {
                                            //Recupera Acesso à Extranet (itbc_acessoextranet)utilizando os seguintes parâmetros:
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = RepositoryService.AcessoExtranet.ObterPor(conta.Classificacao.Id, conta.Categoria.Id, "Basico").ID.Value;
                                            //Excel - Coluna Acesso a Extranet
                                            ws.Cell(linha, 3).Value = "Basico";
                                        }
                                        else
                                        {
                                            contatoAcessoExtranet[0].AcessoExtranetid.Id = idAcessoContaAdesao;
                                            ws.Cell(linha, 3).Value = "Conta Adesão";
                                        }
                                    }

                                    //Atualiza o campo Acesso à Extranet do registro de Acesso à Extranet do Contato com o valor localizado e envia a mensagem "MSG0119 - REGISTRA_ACESSO_EXTRANET
                                    RepositoryService.AcessoExtranetContato.Update(contatoAcessoExtranet[0]);

                                    Domain.Integracao.MSG0119 registraAcessoExtranet = new Domain.Integracao.MSG0119(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                    registraAcessoExtranet.Enviar(contatoAcessoExtranet[0]);
                                }
                                //Excel - Coluna Acesso a Extranet
                                ws.Cell(linha, 3).Value = contatoAcessoExtranet[0].TipoAcesso.Name;
                            }
                            if (contatoAcessoExtranet.Count <= 0)
                                //Excel - Coluna Erro
                                ws.Cell(linha, 5).Value = "Contato sem acesso a Extranet";

                            //Excel - Coluna Sucesso
                            ws.Cell(linha, 4).Value = "Sim";
                            linha++;
                            qtdsucesso++;
                        }
                    }
                    catch (Exception ex)
                    {
                        SDKore.Helper.Error.Create("Problemas ao Alterar o acesso a extranet conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        qtderro++;
                        linha++;
                        erros += "<b> - </b> Problemas ao alterar acesso a Extranet do contato: " + contatoAtual + ". <b> Erro: </b>" + ex.Message + "<br />";
                        //Excel - Coluna Erro
                        ws.Cell(linha, 5).Value = ex.Message;
                    }
                }

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                textoEmail += "<b> Total de contatos a serem alterados: </b>" + totalContatos + "<br />";
                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Total de contatos alterados: </b>" + qtdsucesso + "<br />";
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Alterar ACesso a Extranet(" + TipoAmbiente + ")", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
        }

        public List<Model.Conta> ListarContasFiliaisPorMatriz(Guid contaMatriz)
        {
            return RepositoryService.Conta.ListarContasFiliaisPorMatriz(contaMatriz);
        }

        public List<Model.Conta> ListarMatrizFiliais(Guid contaId, params string[] columns)
        {
            var conta = RepositoryService.Conta.Retrieve(contaId, "accountid", "parentaccountid");
            Guid id = (conta.ContaPrimaria != null) ? conta.ContaPrimaria.Id : conta.ID.Value;

            return RepositoryService.Conta.ListarFiliaisMatriz(id, columns);
        }

        private bool IntegracaoBarramento(Model.Conta objConta, ref string nomeAbrevRet, ref string codigoClienteRet, ref string nomeAbrevMatriEconom)
        {

            if (objConta.IntegrarNoPlugin)
            {
                return false;
            }

            /*if (objConta.StatusIntegracaoSefaz != (int)Domain.Enum.Conta.StatusIntegracaoSefaz.Validado
                && objConta.StatusIntegracaoSefaz != (int)Domain.Enum.Conta.StatusIntegracaoSefaz.ValidadoManualmente)
            {
                return false;
            }*/

            Domain.Integracao.MSG0072 msgConta = new Domain.Integracao.MSG0072(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            msgConta.Enviar(objConta, ref nomeAbrevRet, ref codigoClienteRet, ref nomeAbrevMatriEconom);

            return true;
        }

        public bool IntegracaoBarramento(Model.Conta conta, ref Entity entity)
        {
            string nomeAbrevMatriEconom = String.Empty;
            string nomeAbrevRet = String.Empty;
            string codigoClienteRet = String.Empty;

            if (IntegracaoBarramento(conta, ref nomeAbrevRet, ref codigoClienteRet, ref nomeAbrevMatriEconom))
            {
                if (!String.IsNullOrEmpty(nomeAbrevRet))
                {
                    entity.Attributes["itbc_nomeabreviado"] = nomeAbrevRet;
                    entity.Attributes["itbc_acaocrm"] = true;
                }

                if (!String.IsNullOrEmpty(codigoClienteRet))
                {
                    entity.Attributes["accountnumber"] = codigoClienteRet;
                    entity.Attributes["itbc_acaocrm"] = true;
                }

                return true;
            }

            return false;
        }


        public Model.Conta BuscarContaIntegracaoCrm4(string guidCrm40)
        {
            return RepositoryService.Conta.ObterPorIntegracaoCrm4(guidCrm40);
        }

        public void ValidaCanalApuracaoDeBeneficios(Model.Conta canalPre, Model.Conta canalPost)
        {
            // Participante do Programa == Não e Filial
            if (canalPost.ParticipantePrograma != (int)Enum.Conta.ParticipaDoPrograma.Sim
                && canalPost.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz
                && canalPost.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial)
            {
                if (canalPost.ContaPrimaria != null)
                {
                    var matriz = this.RepositoryService.Conta.Retrieve(canalPost.ContaPrimaria.Id);

                    if (matriz.ParticipantePrograma == (int)Enum.Conta.ParticipaDoPrograma.Sim)
                    {

                        if (matriz.Classificacao.Id != canalPost.Classificacao.Id
                            || matriz.Subclassificacao.Id != canalPost.Subclassificacao.Id)
                        {
                            throw new ArgumentException("(CRM) A classificação e subclassificação de uma filial deve ser igual a da sua matriz.");
                        }
                    }
                }
            }
        }

        private SefazViewModel Preencher(Model.Conta conta)
        {
            var cpfCnpj = conta.CpfCnpj.GetOnlyNumbers();

            var sefazViewModel = new SefazViewModel();

            if (conta.TipoConstituicao.HasValue)
            {
                if (conta.TipoConta.Value == (int)Enum.Conta.TipoConstituicao.Cnpj)
                    sefazViewModel.CNPJ = cpfCnpj;
                else
                    sefazViewModel.CPF = cpfCnpj;
            }

            if (!string.IsNullOrEmpty(conta.Endereco1Estado))
                sefazViewModel.UF = conta.Endereco1Estado;

            return sefazViewModel;
        }

        private bool Preencher(SefazViewModel sefazViewModel, ref Model.Conta conta)
        {
            bool atualizou = false;

            if (sefazViewModel.EnderecoContribuinte != null)
            {
                if (sefazViewModel.EnderecoContribuinte.CodigoIBGE.HasValue)
                {
                    conta.CpfCnpj = !string.IsNullOrEmpty(sefazViewModel.CNPJ) ? sefazViewModel.CNPJ : sefazViewModel.CPF;
                    conta.InscricaoEstadual = sefazViewModel.InscricaoEstadual;
                    if (sefazViewModel.CNAE.HasValue) conta.CNAE = sefazViewModel.CNAE.Value.ToString();
                    if (sefazViewModel.ContribuinteIcms.HasValue) conta.ContribuinteICMS = Convert.ToBoolean(sefazViewModel.ContribuinteIcms.Value);
                    conta.DataBaixaContribuinte = sefazViewModel.DataBaixa;
                    if (!string.IsNullOrEmpty(sefazViewModel.Nome)) conta.RazaoSocial = sefazViewModel.Nome;
                    if (!string.IsNullOrEmpty(sefazViewModel.NomeFantasia)) conta.NomeFantasia = sefazViewModel.NomeFantasia;


                    atualizou = AtualizarInformacoesIBGE(sefazViewModel.EnderecoContribuinte.CodigoIBGE.Value, ref conta);
                }
            }

            return atualizou;
        }

        public void AtualizarInformacoesRevendaSellOut(Classificacao classificacao, Subclassificacoes subClassificacoes)
        {
            bool continua = false;
            int count = 100;
            int pageNumber = 1;
            string pagingCookie = string.Empty;

            do
            {
                var lista = RepositoryService.Conta.ListarPor(classificacao, subClassificacoes, count, pagingCookie, pageNumber);

                foreach (var item in lista.List)
                {
                    Console.WriteLine("Listando próximo");

                    bool atualizar = false;
                    var contaUpdate = new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        ID = item.ID
                    };

                    if (item.NivelPosVendas == null)
                    {
                        contaUpdate.NivelPosVendas = new SDKore.DomainModel.Lookup(new Guid("37E3A262-75ED-E311-9407-00155D013D38"), SDKore.Crm.Util.Utility.GetEntityName<NivelPosVenda>());
                        atualizar = true;
                    }

                    if (!item.AssistenciaTecnica.HasValue)
                    {
                        contaUpdate.AssistenciaTecnica = false;
                        atualizar = true;
                    }

                    if (!item.ApuracaoBeneficiosCompromissos.HasValue)
                    {
                        contaUpdate.ApuracaoBeneficiosCompromissos = (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz;
                        atualizar = true;
                    }

                    if (!item.TipoConta.HasValue)
                    {
                        contaUpdate.TipoConta = (int)Enum.Conta.MatrizOuFilial.Matriz;
                        atualizar = true;
                    }

                    if (atualizar)
                    {
                        Console.WriteLine("Atualizando.");
                        RepositoryService.Conta.Update(contaUpdate);
                    }
                }

                pageNumber++;
                pagingCookie = lista.PagingCookie;
                continua = lista.MoreRecords;
            }
            while (continua);
        }

        public void AtualizarCepRevendaSellOut(Classificacao classificacao, Subclassificacoes subClassificacoes)
        {
            bool continua = false;
            int count = 100;
            int pageNumber = 1;
            string pagingCookie = string.Empty;
            var enderecoService = new EnderecoServices(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            do
            {
                var lista = RepositoryService.Conta.ListarPor(classificacao, subClassificacoes, count, pagingCookie, pageNumber);

                foreach (var item in lista.List)
                {
                    Console.WriteLine("{0} - Listando próximo", DateTime.Now);

                    if (string.IsNullOrEmpty(item.CodigoMatriz) && !string.IsNullOrEmpty(item.Endereco1CEP))
                    {
                        var contaUpdate = new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = item.ID
                        };

                        try
                        {
                            Console.WriteLine("{0} - Pesquisando CEP", DateTime.Now);

                            var ibge = enderecoService.BuscaCep(item.Endereco1CEP);

                            contaUpdate.Endereco1Rua = ibge.Endereco.Truncate(35);
                            contaUpdate.Endereco1Rua1 = ibge.Endereco.Truncate(35);
                            contaUpdate.Endereco2Rua = ibge.Endereco.Truncate(35);
                            contaUpdate.Endereco2Rua2 = ibge.Endereco.Truncate(35);

                            contaUpdate.Endereco1Bairro = ibge.Bairro;
                            contaUpdate.Endereco1Bairro1 = ibge.Bairro;
                            contaUpdate.Endereco2Bairro = ibge.Bairro;
                            contaUpdate.Endereco2Bairro2 = ibge.Bairro;

                            contaUpdate.Endereco1Cidade = ibge.NomeCidade;
                            contaUpdate.Endereco2Cidade = ibge.NomeCidade;
                            contaUpdate.Endereco1Municipioid = ibge.Municipio;
                            contaUpdate.Endereco2Municipioid = ibge.Municipio;

                            contaUpdate.Endereco1Estado = ibge.Estado.Name;
                            contaUpdate.Endereco2Estado = ibge.Estado.Name;
                            contaUpdate.Endereco1Estadoid = ibge.Estado;
                            contaUpdate.Endereco2Estadoid = ibge.Estado;

                            contaUpdate.Endereco1Pais = ibge.Pais;
                            contaUpdate.Endereco2Pais = ibge.Pais;
                            contaUpdate.Endereco1Pais1 = ibge.Pais.Name;
                            contaUpdate.Endereco2Pais2 = ibge.Pais.Name;


                            if (ibge != null)
                            {
                                Console.WriteLine("{0} - Atualizando", DateTime.Now);

                                RepositoryService.Conta.Update(contaUpdate);
                            }
                        }
                        catch (ArgumentException) { }
                    }
                }

                pageNumber++;
                pagingCookie = lista.PagingCookie;
                continua = lista.MoreRecords;
            }
            while (continua);
        }

        public void AtualizarIbgeRevendaSellOut(Classificacao classificacao, Subclassificacoes subClassificacoes)
        {
            bool continua = false;
            int count = 100;
            int pageNumber = 1;
            string pagingCookie = string.Empty;
            var enderecoService = new EnderecoServices(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            do
            {
                var lista = RepositoryService.Conta.ListarPor(classificacao, subClassificacoes, count, pagingCookie, pageNumber);

                foreach (var item in lista.List)
                {
                    Console.WriteLine("{0} - Listando próximo", DateTime.Now);

                    if (item.Endereco1Estadoid != null && !string.IsNullOrEmpty(item.RazaoSocial) && item.TipoConstituicao.HasValue)
                    {
                        var contaUpdate = new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = item.ID
                        };

                        try
                        {
                            Console.WriteLine("{0} - Pesquisando Sefaz", DateTime.Now);

                            var msg0164 = new Domain.Integracao.MSG0164(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                            var sefazViewModel = new SefazViewModel();
                            sefazViewModel.UF = RepositoryService.Estado.Retrieve(item.Endereco1Estadoid.Id).SiglaUF;
                            if (item.TipoConstituicao.Value == (int)Enum.Conta.TipoConstituicao.Cnpj)
                            {
                                sefazViewModel.CNPJ = item.CpfCnpj;
                            }
                            else
                            {
                                sefazViewModel.CPF = item.CpfCnpj;
                            }

                            sefazViewModel = msg0164.Enviar(sefazViewModel);

                            contaUpdate.InscricaoEstadual = sefazViewModel.InscricaoEstadual;
                            if (sefazViewModel.CNAE.HasValue) contaUpdate.CNAE = sefazViewModel.CNAE.Value.ToString();
                            if (sefazViewModel.ContribuinteIcms.HasValue) contaUpdate.ContribuinteICMS = Convert.ToBoolean(sefazViewModel.ContribuinteIcms.Value);
                            contaUpdate.DataBaixaContribuinte = sefazViewModel.DataBaixa;
                            if (!string.IsNullOrEmpty(sefazViewModel.Nome)) contaUpdate.RazaoSocial = sefazViewModel.Nome;
                            if (!string.IsNullOrEmpty(sefazViewModel.NomeFantasia)) contaUpdate.NomeFantasia = sefazViewModel.NomeFantasia;

                            Console.WriteLine("{0} - Atualizando", DateTime.Now);

                            RepositoryService.Conta.Update(contaUpdate);
                        }
                        catch (Exception) { }
                    }
                }

                pageNumber++;
                pagingCookie = lista.PagingCookie;
                continua = lista.MoreRecords;
            }
            while (continua);
        }

        public Boolean CidadeZonaFranca(string cidade, string uf)
        {
            return RepositoryService.Conta.CidadeZonaFranca(cidade, uf);
        }

        //public bool ExisteDuplicidade()
        //{
        //    if (string.IsNullOrEmpty(this.CodigoMatriz)
        //        && string.IsNullOrEmpty(this.Cpf)
        //        && string.IsNullOrEmpty(this.Cnpj)
        //        && string.IsNullOrEmpty(this.CpfCnpjSemMascara))
        //        return false;

        //    return RepositoryService.Conta.ExisteDuplicidade(this);
        //}

        //public string ObterCpfCnpjSemMascara()
        //{
        //    string resultado = string.Empty;

        //    if (!string.IsNullOrEmpty(this.Cnpj))
        //    {
        //        resultado = new Intelbras.Helper.Helper().GetOnlyNumbers(this.Cnpj);
        //    }
        //    else if (!string.IsNullOrEmpty(this.Cpf))
        //    {
        //        resultado = new Intelbras.Helper.Helper().GetOnlyNumbers(this.Cpf);
        //    }

        //    return resultado;
        //}

        public Model.Conta PesquisaAutorizadaPor(Guid ocorrenciaId)
        {
            return RepositoryService.Conta.ObterAutorizadaPor(ocorrenciaId);
        }

        public Categoria PesquisarCategoriaCliente(int codigo, UnidadeNegocio unidadeNegocio)
        {
            return RepositoryService.Conta.PesquisarCategoriaCliente(codigo, unidadeNegocio);
        }

        public void AtualizaEnderecoPadrao(Model.Endereco endereco)
        {
            RepositoryService.Endereco.Update(endereco);
        }

        public Model.Conta PesquisarClientePor(string nomecliente)
        {
            return RepositoryService.Conta.PesquisarPor(nomecliente);
        }

        public Model.Conta PesquisarClientePor(string documento, NaturezaDoCliente natureza)
        {

            Documento doc = null;
            switch (natureza)
            {
                case NaturezaDoCliente.PessoaFisica:
                    doc = new CPF(documento);
                    break;
                case NaturezaDoCliente.PessoaJuridica:
                    doc = new CNPJ(documento);
                    break;
                case NaturezaDoCliente.Estrangeiro:
                    doc = new Estrangeiro(documento);
                    break;
            }


            if (!doc.EValido())
                throw new Exception("O documento informado para a pesquisa é inválido.");

            return RepositoryService.Conta.PesquisarPor(doc);
        }

        public Model.Conta PesquisarClientePor(int codigo)
        {
            return RepositoryService.Conta.PesquisarPor(codigo);
        }

        public Model.Conta PesquisarClientePor(Model.Conta cliente)
        {
            return RepositoryService.Conta.Retrieve(cliente.Id);
        }

        public void MarcaRevendasVMC()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            int qtderro = 0;
            int qtdsucesso = 0;
            string contasSucesso = "";


            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            #region Recupera Revendas Ouro, Prata e Bronze dos Participantes do PCI

            string[] categorias = { "1", "2", "3" };
            var lista = RepositoryService.Conta.ListarPorCategoria(categorias);

            textoEmail += "<b> Conta Ouro, Prata e Bronze: </b>" + lista.Count + " <br />";
            #endregion

            //Busca qual ambiente está sendo executado e anexo no e-mail
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            if (lista.Count > 0)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Contas direito VMC");

                ws.Cell(1, 1).Value = "Razão Social";
                ws.Cell(1, 2).Value = "Nome Fantasia";
                ws.Cell(1, 3).Value = "Codigo Emitente";
                ws.Cell(1, 4).Value = "CNPJ";
                ws.Cell(1, 5).Value = "Direito a VMC";
                ws.Cell(1, 6).Value = "Sucesso";
                ws.Cell(1, 7).Value = "Erro";


                int linha = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "CONTAS_DIREITO_VMC_" + data + ".xlsx";

                foreach (var conta in lista)
                {
                    try
                    {
                        #region Recupera meta global por região do canal
                        var linhaDeCorte = RetornaLinhasCortePelaConta(conta, conta.Categoria.Id);
                        #endregion

                        #region Recupera faturamento ultimo trimestre DW
                        var faturamentoDW = RepositoryService.LinhaCorteDistribuidor.FaturamentoUltimoTrimestreRevendaDW(Helper.TrimestreAnteriorAno(),
                                                                                                                         Helper.TrimestreAnterior(),
                                                                                                                          conta.ID.Value);

                        #region Recupera faturamento total ultimo trimestre

                        var totalFaturamentoUltimoTrimestreRevenda = RetornaTotalFaturamentoUltimoTrimestreRevenda(faturamentoDW);

                        #endregion
                        #endregion

                        #region Verifica se fatuamento da revenda é maior que meta global

                        Model.Conta contaUpdate = new Model.Conta(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        contaUpdate.ID = conta.ID;

                        if (totalFaturamentoUltimoTrimestreRevenda >= RetornalinhaDeCorteAdministrativo(linhaDeCorte, conta.Categoria.Name).LinhaCorteTrimestral.Value)
                        {

                            if (VerificaLinhaDeCorteCategoria(faturamentoDW, linhaDeCorte))
                            {
                                contaUpdate.TemDrireitoVMC = true;
                                contaUpdate.IntegrarNoPlugin = true;
                                RepositoryService.Conta.Update(contaUpdate);
                                ws.Cell(linha, 1).Value = conta.RazaoSocial;
                                ws.Cell(linha, 2).Value = conta.NomeFantasia;
                                ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                                ws.Cell(linha, 4).Value = conta.CpfCnpj.ToString();
                                ws.Cell(linha, 5).Value = "Sim";
                                ws.Cell(linha, 6).Value = "Sim";
                            }
                            else
                            {
                                contaUpdate.TemDrireitoVMC = false;
                                contaUpdate.IntegrarNoPlugin = true;
                                RepositoryService.Conta.Update(contaUpdate);
                                ws.Cell(linha, 1).Value = conta.RazaoSocial;
                                ws.Cell(linha, 2).Value = conta.NomeFantasia;
                                ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                                ws.Cell(linha, 4).Value = conta.CpfCnpj.ToString();
                                ws.Cell(linha, 5).Value = "Não";
                                ws.Cell(linha, 6).Value = "Sim";
                            }
                        }
                        else
                        {
                            contaUpdate.TemDrireitoVMC = false;
                            contaUpdate.IntegrarNoPlugin = true;
                            RepositoryService.Conta.Update(contaUpdate);
                            ws.Cell(linha, 1).Value = conta.RazaoSocial;
                            ws.Cell(linha, 2).Value = conta.NomeFantasia;
                            ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                            ws.Cell(linha, 4).Value = conta.CpfCnpj.ToString();
                            ws.Cell(linha, 5).Value = "Não";
                            ws.Cell(linha, 6).Value = "Sim";
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        SDKore.Helper.Error.Create("Problemas ao marcar categorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        ws.Cell(linha, 1).Value = conta.RazaoSocial;
                        ws.Cell(linha, 2).Value = conta.NomeFantasia;
                        ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                        ws.Cell(linha, 4).Value = conta.CpfCnpj.ToString();
                        ws.Cell(linha, 5).Value = "Não";
                        ws.Cell(linha, 6).Value = "Não";
                        ws.Cell(linha, 7).Value = ex.Message;
                        qtderro++;
                    }
                    qtdsucesso++;
                    linha++;
                }

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += "<b> Revendas foram alteradas: </b>" + qtdsucesso + "<br />";
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Marca Revendas para receber VMC(" + TipoAmbiente + ").", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));

            }
        }

        public XDocument MontaXmlRevendasFielo()
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "no"));

            #region

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)TipoParametroGlobal.DataEnvioRegistroSelloutFielo);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)TipoParametroGlobal.DataEnvioRegistroSelloutFielo + ") não encontrado!");
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion

            try
            {
                if (dataConsulta.Date != DateTime.Now.Date)
                {
                    throw new ArgumentException("(CRM) Data inválida para processamento! (" + dataConsulta.Date.ToShortDateString() + ")");
                }

                List<Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout> filtroContas = new List<Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout>();
                List<string> subClassificacoes = new List<string>();
                List<string> categorias = new List<string>();

                #region Filtros Revendas
                    categorias = new List<string>();
                    categorias.Add(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Ouro);
                    categorias.Add(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Prata);
                    categorias.Add(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Bronze);

                    var filtroRevendas = new Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout();
                    filtroRevendas.Classificacao = Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Revendas;
                    filtroRevendas.SubClassificacao = null;
                    filtroRevendas.Categorias = categorias;

                    filtroContas.Add(filtroRevendas);
                #endregion

                #region Filtros Revendas Soluções
                    subClassificacoes = new List<string>();
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Integrador);
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Bancaria);
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Incendio);
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Monitoramento);
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Telecom);
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Rev_Revendas);

                    categorias = new List<string>();
                    categorias.Add(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Rev_Sol);

                    var filtroRevendasSolucoes = new Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout();
                    filtroRevendasSolucoes.Classificacao = Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Sol;
                    filtroRevendasSolucoes.SubClassificacao = subClassificacoes;
                    filtroRevendasSolucoes.Categorias = categorias;

                    filtroContas.Add(filtroRevendasSolucoes);
                #endregion

                #region Filtros Provedores
                    subClassificacoes = new List<string>();
                    subClassificacoes.Add(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Provedores);

                    categorias = new List<string>();
                    categorias.Add(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Provedores);

                    var filtroProvedores = new Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout();
                    filtroProvedores.Classificacao = Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Provedores;
                    filtroProvedores.SubClassificacao = subClassificacoes;
                    filtroProvedores.Categorias = categorias;

                    filtroContas.Add(filtroProvedores);
                #endregion

                var parametroGlobalMeses = new ParametroGlobalService(RepositoryService).ObterPor((int)TipoParametroGlobal.MesesAEnviarParaFielo);
                if (parametroGlobalMeses == null)
                {
                    throw new ArgumentException("(CRM) Parâmetro Global(" + (int)TipoParametroGlobal.MesesAEnviarParaFielo + ") não encontrado!");
                }

                var contas = RepositoryService.Conta.ListarGuidContasParaEnviarRegistroFielo(filtroContas);

                var listElement = new List<XElement>();
                foreach (var conta in contas)
                {
                    listElement.Add(new XElement("ListaGuid", conta.ID.Value.ToString()));
                }

                int mes = Convert.ToInt32(parametroGlobalMeses.Valor);
                var dataInicial = DateTime.Now;
                #region Atualiza data da próxima categorização

                if (DateTime.Now.Month == 12)
                {
                    parametroGlobal.Valor = DateTime.Now.Day + "/" + 01 + "/" + (DateTime.Now.Year + 1);
                }
                else
                {
                    parametroGlobal.Valor = DateTime.Now.Day + "/" + (DateTime.Now.Month + 1) + "/" + DateTime.Now.Year;
                }

                RepositoryService.ParametroGlobal.Update(parametroGlobal);

                xml = new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XElement("ContasFielo",
                                new XElement("ListaRevendasSelloutFielo", listElement),
                                new XElement("Meses", parametroGlobalMeses.Valor)
                            )
                        );
                #endregion
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) ERRO Categorizacao : " + ex.Message);
            }
            return xml;
        }

        public Model.Conta BuscaMatrizEconomica(string raizCNPJ)
        {
            return RepositoryService.Conta.BuscaMatrizEconomica(raizCNPJ);
        }

        public void MarcaRevendaRecategorizarMensal()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            var datatual = DateTime.Today;
            var dtMesAnterior = datatual.AddMonths(-2);
            dtMesAnterior = new DateTime(dtMesAnterior.Year, dtMesAnterior.Month, DateTime.DaysInMonth(dtMesAnterior.Year, dtMesAnterior.Month));

            //Busca qual ambiente está sendo executado e anexo no e-mail
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            var contas = RepositoryService.Conta.ListarContasRecategorizarMensal(dtMesAnterior);
            textoEmail += "<b> Contas para categorizar: </b>" + contas.Count + " <br />";

            if (contas.Count > 0)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Recategorização Mensal");

                ws.Cell(1, 1).Value = "Conta";
                ws.Cell(1, 2).Value = "Sucesso";
                ws.Cell(1, 3).Value = "Erro";

                int linha = 2;
                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");
                string nomeArquivo = "RECATEGORIZACAO_MENSAL_REVENDA_" + data + ".xlsx";

                foreach (var conta in contas)
                {
                    //Excel - Coluna Conta
                    ws.Cell(linha, 1).Value = conta.NomeFantasia;
                    try
                    {
                        conta.IntegrarNoPlugin = true;
                        conta.Categorizar = (int)Enum.Conta.Categorizacao.RecategorizarMensal;
                        RepositoryService.Conta.Update(conta);
                        //Excel - Coluna Sucesso
                        ws.Cell(linha, 2).Value = "Sim";
                    }
                    catch (Exception ex)
                    {
                        SDKore.Helper.Error.Create("Problemas ao marcar recategorizar mensal a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                        erros += "<b> - </b> Problemas ao marcar para recategorizar mensal a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";
                        //Excel - Coluna Sucesso
                        ws.Cell(linha, 2).Value = "Não";
                        //Excel - Coluna Erro
                        ws.Cell(linha, 3).Value = ex.Message;
                        qtderro++;
                    }
                    linha++;
                }
                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Marcar para Recategorizar Mensals (" + TipoAmbiente + ")", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
            #endregion
        }
    
        public void AtualizaContasReCategorizacaoMensal()
        {
            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            string erros = "";
            int qtderro = 0;
            int qtdsucesso = 0;
            int revendasAlteradas = 0;
            int revendasIguais = 0;
            string razaoSocial = "";
            int contContas = 0;
            int contContasTotal = 0;

            bool bCategorizou = false;
            bool trocouCategoria = false;

            //Busca qual ambiente está sendo executado e anexo no e-mail
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoRotinasPCI);
            #endregion

            #region Recupera valor do parâmetro global para buscar o Número Mínimo de Colaboradores Treinados para Revenda Ouro
            var parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.NumeroMinimoColaboradoresTreinadosRevendaOuro);
            #endregion

            #region Recupera valor do parâmetro global "Data da Próxima ReCaterização Revenda".

            var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)Domain.Enum.TipoParametroGlobal.DataProximaReCategorizacaoMensalRevenda);
            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Domain.Enum.TipoParametroGlobal.DataProximaReCategorizacaoMensalRevenda + ") não encontrado!");
            }
            var dataConsulta = Convert.ToDateTime(parametroGlobal.Valor);
            #endregion

            if (dataConsulta.Date == DateTime.Now.Date)
            {
                var excel = new Excel.XLWorkbook();
                var ws = excel.Worksheets.Add("Recategorização");
                ws.Cell(1, 1).Value = "Razão social";
                ws.Cell(1, 2).Value = "Cnpj";
                ws.Cell(1, 3).Value = "Código Emitente";
                ws.Cell(1, 4).Value = "Estado";
                ws.Cell(1, 5).Value = "Nº Técnicos Treinados";
                ws.Cell(1, 6).Value = "Assistência Técnica";
                ws.Cell(1, 7).Value = "Total Faturamento";
                ws.Cell(1, 8).Value = "Linha de Corte";
                ws.Cell(1, 9).Value = "Categoria";
                ws.Cell(1, 10).Value = "Categoria após categorização";
                ws.Cell(1, 11).Value = "Sucesso";
                ws.Cell(1, 12).Value = "Erro";

                int linha = 2;
                int linhaUpdate = 2;

                string data = DateTime.Now.ToString();
                data = data.Replace("/", "-").Replace(":", "-");

                string nomeArquivo = "RECATEGORIZACAO_MENSAL_REVENDA_" + data + ".xlsx";

                #region Recupera valor das categorias Ouro, Prata e Bronze".
                var categoriaOuro = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Ouro");
                var categoriaPrata = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Prata");
                var categoriaBronze = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Bronze");
                var categoriaRegistrada = SDKore.Configuration.ConfigurationManager.GetSettingValue<Guid>("Intelbras.Categoria.Registrada");
                #endregion

                #region Recuperar registros na entidade "Conta" em que o atributo Categorizar seja igual a "ReCategorizar".
                var contas = RepositoryService.Conta.ListarContasRecategorizacaoMensal();
                #endregion

                //Retorna faturamento Ultimo Semestre DW
                DataTable faturamentoGlobal = new DataTable();
                DataTable faturamentoLinhasCorte = new DataTable();
                if (contas.Count > 0)
                {
                    string contaId2 = "";
                    List<string> contasConsulta = new List<string>();
                    foreach (var conta in contas)
                    {
                        contasConsulta.Add("'"+conta.ID+"'");
                        if(contasConsulta.Count == 5000 || contas.Last().ID == conta.ID)
                        {
                            contaId2 = String.Join(",", contasConsulta.ToArray());
                            if(faturamentoGlobal.Rows.Count == 0)
                            {
                                //Busca faturamento Global das revendas no semestre
                                faturamentoGlobal = RepositoryService.LinhaCorteDistribuidor.FaturamentoGlobalSemestreDW(DateTime.Now.Year,
                                                                                                                                DateTime.Now.Year,
                                                                                                                                Helper.SemestreAtual(),
                                                                                                                                contaId2);

                                //Busca as linhas de corte por unidade e global (unidade ADM) das revendas
                                faturamentoLinhasCorte = RepositoryService.LinhaCorteDistribuidor.FaturamentoLinhasCorteDWTrimestre(contaId2, "2");
                            }
                            else
                            {
                                //Busca faturamento Global das revendas no semestre
                                faturamentoGlobal.Merge(RepositoryService.LinhaCorteDistribuidor.FaturamentoGlobalSemestreDW(DateTime.Now.Year,
                                                                                                                                DateTime.Now.Year,
                                                                                                                                Helper.SemestreAtual(),
                                                                                                                                contaId2));
                                //Busca as linhas de corte por unidade e global (unidade ADM) das revendas
                                faturamentoLinhasCorte.Merge(RepositoryService.LinhaCorteDistribuidor.FaturamentoLinhasCorteDWTrimestre(contaId2, "2"));
                            }
                            contasConsulta = new List<string>();
                        }
                    }
                    
                }

                if (faturamentoGlobal.Rows.Count > 0)
                {
                    textoEmail += "<b> Contas para serem Recategorizadas: </b>" + contas.Count + " <br />";

                    foreach (var conta in contas)
                    {
                        try
                        {
                            bCategorizou = false;
                            trocouCategoria = false;
                            razaoSocial = conta.RazaoSocial;
                            int categoria = 0;

                            //Define um peso para a categoria atual
                            switch (conta.Categoria.Name)
                            {
                                case "Ouro":
                                    categoria = 3;
                                    break;
                                case "Prata":
                                    categoria = 2;
                                    break;
                                case "Bronze":
                                    categoria = 1;
                                    break;
                                default:
                                    categoria = 0;
                                    break;
                            }

                            //Excel - Coluna Conta
                            ws.Cell(linha, 1).Value = razaoSocial;
                            ws.Cell(linha, 2).Value = conta.CpfCnpj;
                            ws.Cell(linha, 3).Value = conta.CodigoMatriz;
                            ws.Cell(linha, 4).Value = conta.Endereco1Estado;
                            ws.Cell(linha, 6).Value = ((bool)conta.AssistenciaTecnica) ? "Sim" : "Não";

                            if (conta.Categoria != null)
                            {
                                var query1 = from p in faturamentoGlobal.AsEnumerable()
                                                where p.Field<Guid>("CD_Revenda_SellOut") == conta.ID
                                                select p;

                                decimal faturamentoTotalTrimestre = 0;
                                foreach (var i in query1)
                                {
                                    faturamentoTotalTrimestre = i.Field<decimal>("FaturamentoTOTAL");
                                }

                                ws.Cell(linha, 7).Value = faturamentoTotalTrimestre;

                                //Excel - Coluna Categoria
                                ws.Cell(linha, 9).Value = conta.Categoria.Name;

                                //Retorna Valor Unidade Negócio ADM das categorias
                                decimal linhadecorteAdmOuro = 0;
                                decimal linhadecorteAdmPrata = 0;
                                decimal linhadecorteAdmBronze = 0;

                                var query = from p in faturamentoLinhasCorte.AsEnumerable()
                                            where p.Field<Guid>("cd_guid") == conta.ID && p.Field<string>("CD_Unidade_Negocio") == "ADM"
                                            select p;

                                foreach (var i in query)
                                {
                                    if (i.Field<string>("TX_Categoria_Canal") == "Ouro")
                                    {
                                        linhadecorteAdmOuro = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                    else
                                    if (i.Field<string>("TX_Categoria_Canal") == "Prata")
                                    {
                                        linhadecorteAdmPrata = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                    else
                                    {
                                        linhadecorteAdmBronze = i.Field<decimal>("NM_Vl_Linha_Corte");
                                    }
                                }

                                if(linhadecorteAdmBronze == 0 || linhadecorteAdmOuro == 0 || linhadecorteAdmPrata == 0)
                                {
                                    throw new System.ArgumentException("Não encontrado linha de corte no DW para a revenda");
                                }

                                /**
                                 * Chamado 125468  - Adicionado nava regra para torna-se
                                 * Ter pelo menos 2 técnicos treinados, com a modalidade do treinamento como Presencial, 
                                 * onde Status de Aprovação = Aprovado e que o campo Validade seja maior do que a data atual. 
                                 */
                                List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificado = RepositoryService.ColaboradorTreinadoCertificado.ListarPorCanalTreinamentosAprovadosValidos(conta.ID.Value);    
                                var tecnicosTreinados = lstColaboradorTreinadoCertificado.GroupBy(x => x.Contato.Id).ToArray().Count();
                                ws.Cell(linha, 5).Value = tecnicosTreinados;

                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria ouro
                                //Verifica se o faturamento da revenda é maior ou igual que a meta global Ouro
                                if (faturamentoTotalTrimestre >= linhadecorteAdmOuro)
                                {
                                    ws.Cell(linha, 8).Value = linhadecorteAdmOuro;
                                    if (tecnicosTreinados >= Int32.Parse(parametroGlobalNumeroMinimoColaboradoresTreinadosRevendaOuro.Valor))
                                    {
                                        bCategorizou = true;
                                        if (conta.Categoria.Id != categoriaOuro)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaOuro;
                                            conta.Categoria.Name = "Ouro";
                                            //Excel - Coluna Categoria após Recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                    }
                                }
                                #endregion
                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria prata
                                if (!bCategorizou)
                                {
                                    if (faturamentoTotalTrimestre >= linhadecorteAdmPrata && categoria <= 2)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmPrata;
                                        bCategorizou = true;
                                        if (categoria < 2)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaPrata;
                                            conta.Categoria.Name = "Prata";                                                                
                                            //Excel - Coluna Categoria após recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                        else
                                        {
                                            //Excel - Coluna Categoria após recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                    }
                                }
                                #endregion
                                #region Verifica se fatuamento da revenda é maior que meta global e pelo menos de uma linha de corte da categoria bronze
                                if (!bCategorizou)
                                {
                                    if (faturamentoTotalTrimestre >= linhadecorteAdmBronze && categoria <= 1)
                                    {
                                        ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                        bCategorizou = true;
                                       
                                        if (categoria < 1)
                                        {
                                            trocouCategoria = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaBronze;
                                            conta.Categoria.Name = "Bronze";
                                            //Excel - Coluna Categoria após recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                        else
                                        {
                                            //Excel - Coluna Categoria após recategorização
                                            ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                        }
                                    }
                                }
                                #endregion
                                //Registrada
                                if (!bCategorizou)
                                {
                                    if (categoria == 0)
                                    {
                                        if ((bool)conta.AssistenciaTecnica)
                                        {
                                            ws.Cell(linha, 8).Value = linhadecorteAdmBronze;
                                            trocouCategoria = true;
                                            bCategorizou = true;
                                            conta.IntegrarNoPlugin = false;
                                            conta.Categoria.Id = categoriaBronze;
                                            conta.Categoria.Name = "Bronze";
                                        }
                                        else
                                        {
                                            if (conta.Categoria.Id != categoriaRegistrada)
                                            {
                                                trocouCategoria = true;
                                                bCategorizou = true;
                                                conta.IntegrarNoPlugin = false;
                                                conta.Categoria.Name = "Registrada";
                                                conta.Categoria.Id = RepositoryService.Categoria.ObterPor("4").ID.Value;
                                            }
                                        }
                                        //Excel - Coluna Categoria após recategorização
                                        ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                    }
                                }
                                #region Altera revenda para Categorizada
                                conta.Categorizar = (int)Enum.Conta.Categorizacao.Categorizada;
                                #endregion

                                if (trocouCategoria)
                                {
                                    var contatos = RepositoryService.Contato.ListarAssociadosA(conta.ID.ToString());
                                    var contatoService = new Domain.Servicos.ContatoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                    foreach (var contato in contatos)
                                    {
                                        contatoService.EnviaContatoFielo(contato, true);
                                    }

                                    conta.IntegraIntelbrasPontua = true;
                                    revendasAlteradas++;
                                }
                                else
                                {
                                    conta.IntegrarNoPlugin = true;
                                    revendasIguais++;
                                }
                                
                                //Excel - Coluna Categoria após recategorização
                                ws.Cell(linha, 10).Value = conta.Categoria.Name;
                                //Excel - Coluna Sucesso                                
                                linha++;

                                RepositoryService.Conta.Update(conta);
                                qtdsucesso++;
                            }
                        }
                        catch (Exception ex)
                        {
                            SDKore.Helper.Error.Create("Problemas ao Recategorizar a conta " + conta.NomeFantasia + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                            erros += "<b> - </b> Problemas ao Recategorizar a conta " + conta.NomeFantasia + ". <b> Erro: </b>" + ex.Message + "<br />";

                            //Excel - Coluna com Erro
                            ws.Cell(linha, 11).Value = "Não";

                            qtderro++;
                            //Excel - Coluna Erro
                            ws.Cell(linha, 12).Value = "(CRM)ERRO ReCategorizacao: " + ex.Message;
                            linha++;
                        }

                        contContas++;
                        contContasTotal++;
                    }
                    
                    #region Altera o valor do parâmetro global "Data da Próxima Recategorização de Revendas" para o primeiro dia do próxima semestre.
                    var proximaData = DateTime.Today.AddMonths(1);
                    parametroGlobal.Valor = 1 + "/" + proximaData.Month.ToString() + "/" + proximaData.Year.ToString();
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);
                    #endregion
                }

                excel.SaveAs("c:\\temp\\" + nomeArquivo);

                textoEmail += "<b> Erros: </b><br />";
                textoEmail += "<b> Total de erros: </b>" + qtderro + "<br />";
                textoEmail += erros;
                textoEmail += "<b> Revendas que trocaram de categoria: </b>" + revendasAlteradas + "<br />";
                textoEmail += "<b> Revendas que permaneceram na mesma categoria: </b>" + revendasIguais + "<br />";
                textoEmail += "<b> Total de contas categorizadas: </b>" + qtdsucesso + "<br />";
                textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

                RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Recategoriza Mensal Revendas (" + TipoAmbiente + ")", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
            }
        }
    }
}