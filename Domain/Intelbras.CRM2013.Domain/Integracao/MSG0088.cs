using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Pollux = Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0088 : Base, IBase<Intelbras.Message.Helper.MSG0088, Domain.Model.Product>
    {

        #region Propriedades

        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        private Domain.Model.Usuario usuarioIntegracao;
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private RepositoryService RepositoryService { get; set; }

        #endregion

        #region Construtor

        public MSG0088(string org, bool isOffline)
            : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
            RepositoryService = new RepositoryService(org, isOffline);
        }
        #endregion

        #region trace

        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }

        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Domain.Model.Usuario usuario)
        {
            try
            {
                retorno.Add("Resultado", this.resultadoPersistencia);
                usuarioIntegracao = usuario;
                var xml = this.CarregarMensagem<Pollux.MSG0088>(mensagem);
                var objeto = this.DefinirPropriedades(xml);

                if (!resultadoPersistencia.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + resultadoPersistencia.Mensagem);
                }

                objeto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.Organizacao, this.IsOffline).Persistir(objeto);

                if (objeto == null)
                {
                    throw new ArgumentException("(CRM) Erro de persistência do Produto!");
                }

                var listaProdutosKit = this.DefinirPropriedades(objeto.ID.Value, xml.ProdutosFilhos);
                new Servicos.ProdutoKitService(RepositoryService).ExcluirCriarPorProdutoPai(objeto.ID.Value, listaProdutosKit);

                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
                return CriarMensagemRetorno<Pollux.MSG0088R1>(numeroMensagem, retorno);
            }
            catch (Exception ex)
            {
                resultadoPersistencia.Mensagem = SDKore.Helper.Error.Handler(ex);
                resultadoPersistencia.Sucesso = false;

                return CriarMensagemRetorno<Pollux.MSG0088R1>(numeroMensagem, retorno);
            }
        }
        #endregion

        #region Definir Propriedades

        public Model.Product DefinirPropriedades(Intelbras.Message.Helper.MSG0088 xml)
        {
            RetornaGuid retornaGUID = new RetornaGuid(this.Organizacao, this.IsOffline);

            var crm = new Product(this.Organizacao, this.IsOffline);

            #region Propriedades Crm->Xml

            if (!String.IsNullOrEmpty(xml.CodigoProduto))
                crm.Codigo = xml.CodigoProduto;

            if (xml.DataAlteracaoPrecoVenda.HasValue)
                crm.DataUltAlteracaoPVC = xml.DataAlteracaoPrecoVenda.Value;

            Product prodRet = new Servicos.ProdutoService(this.Organizacao, this.IsOffline).BuscaPorCodigo(xml.CodigoProduto);
            if (prodRet != null)
                crm.ID = prodRet.ID;


            crm.IntegrarNoPlugin = true;

            if (!String.IsNullOrEmpty(xml.Nome))
                crm.Nome = xml.Nome;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Nome não enviado.";
                return crm;
            }

            crm.Status = xml.Situacao;

            if (xml.Situacao == 0)
                crm.RazaoStatus = 1;
            else
                crm.RazaoStatus = (int)Enum.Produto.StatusCode.Descontinuado;

            if (xml.TipoProduto.HasValue && System.Enum.IsDefined(typeof(Enum.Produto.TipoProduto), xml.TipoProduto))
            {
                crm.TipoProdutoid = xml.TipoProduto;
            }
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Tipo de produto não enviado ou inválido.";
                return crm;
            }

            if (xml.NaturezaProduto.HasValue && System.Enum.IsDefined(typeof(Enum.Produto.NaturezaProduto), xml.NaturezaProduto))
                crm.NaturezaProduto = xml.NaturezaProduto;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Natureza Produto não enviada ou inválida.";
                return crm;
            }

            crm.GrupoEstoque = new Lookup(retornaGUID.GrupoEstoque(xml.GrupoEstoque.Value, ref resultadoPersistencia), "");
            if (crm.GrupoEstoque.Id == Guid.Empty)
                return crm;

            crm.UnidadeNegocio = new Lookup(retornaGUID.UnidadeNegocio(xml.UnidadeNegocio, ref resultadoPersistencia), "");
            if (crm.UnidadeNegocio.Id == Guid.Empty)
                return crm;

            crm.Segmento = new Lookup(retornaGUID.Segmento(xml.Segmento, ref resultadoPersistencia), "");
            if (crm.Segmento.Id == Guid.Empty)
                return crm;

            crm.FamiliaProduto = new Lookup(retornaGUID.FamiliaProduto(xml.Familia, ref resultadoPersistencia), "");
            if (crm.FamiliaProduto.Id == Guid.Empty)
                return crm;

            crm.SubfamiliaProduto = new Lookup(retornaGUID.SubfamiliaProduto(xml.SubFamilia, ref resultadoPersistencia), "");
            if (crm.SubfamiliaProduto.Id == Guid.Empty)
                return crm;

            crm.Origem = new Lookup(retornaGUID.Origem(xml.Origem, ref resultadoPersistencia), "");
            if (crm.Origem.Id == Guid.Empty)
                return crm;

            crm.UnidadePadrao = new Lookup(retornaGUID.UnidadeMedida(xml.UnidadeMedida, ref resultadoPersistencia), "");
            if (crm.UnidadePadrao.Id == Guid.Empty)
                return crm;

            crm.GrupoUnidades = new Lookup(retornaGUID.GrupoUnidadeMedida(xml.GrupoUnidadeMedida, ref resultadoPersistencia), "");
            if (crm.GrupoUnidades.Id == Guid.Empty)
                return crm;

            crm.FamiliaMaterial = new Lookup(retornaGUID.FamiliaMaterial(xml.FamiliaMaterial, ref resultadoPersistencia), "");
            if (crm.FamiliaMaterial.Id == Guid.Empty)
                return crm;

            crm.FamiliaComercial = new Lookup(retornaGUID.FamiliaComercial(xml.FamiliaComercial, ref resultadoPersistencia), "");
            if (crm.FamiliaComercial.Id == Guid.Empty)
                return crm;

            crm.ListaPrecoPadrao = new Lookup(retornaGUID.ListaPreco(xml.ListaPreco, ref resultadoPersistencia), "");
            if (crm.ListaPrecoPadrao.Id == Guid.Empty)
                return crm;

            crm.Moeda = new Lookup(retornaGUID.Moeda(xml.Moeda, ref resultadoPersistencia), "");
            if (crm.Moeda.Id == Guid.Empty)
                return crm;

            if (xml.QuantidadeDecimal.HasValue)
                crm.QuantidadeDecimal = xml.QuantidadeDecimal.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "Quantidade Decimal não enviado.";
                return crm;
            }

            if (!String.IsNullOrEmpty(xml.Descricao))
                crm.Descricao = xml.Descricao;
            else
                crm.AddNullProperty("Descricao");

            if (xml.PesoEstoque.HasValue)
                crm.PesoEstoque = xml.PesoEstoque.Value;
            else
                crm.AddNullProperty("PesoEstoque");

            if (!String.IsNullOrEmpty(xml.Fabricante))
                crm.NomeFornecedor = xml.Fabricante;
            else
                crm.AddNullProperty("NomeFornecedor");

            if (!String.IsNullOrEmpty(xml.NumeroPecaFabricante))
                crm.NumeroPecaFabricante = xml.NumeroPecaFabricante;
            else
                crm.AddNullProperty("NumeroPecaFabricante");

            if (xml.VolumeEstoque.HasValue)
                crm.VolumeEstoque = xml.VolumeEstoque;
            else
                crm.AddNullProperty("VolumeEstoque");

            if (!String.IsNullOrEmpty(xml.ComplementoProduto))
                crm.Complemento = xml.ComplementoProduto;
            else
                crm.AddNullProperty("Complemento");

            if (!String.IsNullOrEmpty(xml.URL))
                crm.Url = xml.URL;
            else
                crm.AddNullProperty("Url");

            if (xml.QuantidadeDisponivel.HasValue)
                crm.QuantidadeDisponivel = xml.QuantidadeDisponivel;
            else
                crm.AddNullProperty("QuantidadeDisponivel");

            if (!String.IsNullOrEmpty(xml.Fornecedor))
                crm.Fornecedor = xml.Fornecedor;
            else
                crm.AddNullProperty("Fornecedor");

            if (!String.IsNullOrEmpty(xml.EAN))
                crm.EAN = xml.EAN;
            else
                crm.AddNullProperty("EAN");

            if (!String.IsNullOrEmpty(xml.NCM))
                crm.NCM = xml.NCM;
            else
                crm.AddNullProperty("NCM");

            if (xml.AliquotaIPI.HasValue)
                crm.PercentualIPI = xml.AliquotaIPI;
            else
                crm.AddNullProperty("PercentualIPI");

            if (!crm.ID.HasValue)
            {
                if (xml.ConsiderarOrcamentoMeta.HasValue)
                    crm.ConsideraOrcamentoMeta = xml.ConsiderarOrcamentoMeta;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "ConsiderarOrcamentoMeta não enviado.";
                    return crm;
                }
                if (xml.FaturamentoOutroProduto.HasValue)
                    crm.FaturamentoOutroProduto = xml.FaturamentoOutroProduto;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "FaturamentoOutroProduto não enviado.";
                    return crm;
                }
                if (xml.QuantidadeMultipla.HasValue)
                    crm.QuantidadeMultiplaProduto = xml.QuantidadeMultipla;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "QuantidadeMultipla não enviada.";
                    return crm;
                }
                if (xml.ExigeTreinamento.HasValue)
                    crm.ExigeTreinamento = (bool)xml.ExigeTreinamento;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Exige Treinamento não enviado.";
                    return crm;
                }

                //if (xml.PassivelSolicitacaoBeneficio.HasValue)
                //    crm.PermitirEmSolBenef = (bool)xml.PassivelSolicitacaoBeneficio;
                //else
                //{
                //    resultadoPersistencia.Sucesso = false;
                //    resultadoPersistencia.Mensagem = "Passivel Solicitacao de Beneficio não enviado.";
                //    return crm;
                //}

                if (xml.RebateAtivado.HasValue)
                    crm.RebatePosVendaAtivado = (bool)xml.RebateAtivado;
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Rebate Ativado não enviado.";
                    return crm;
                }

                //if (xml.PassivelSolicitacaoBeneficio.HasValue)
                //{
                //    crm.PermitirEmSolBenef = (bool)xml.PassivelSolicitacaoBeneficio;
                //}
                //else
                //{
                //    resultadoPersistencia.Sucesso = false;
                //    resultadoPersistencia.Mensagem = "Passível Solicitação de Benefício não enviado.";
                //    return crm;
                //}

                //if (xml.TemMensagem.HasValue)
                //{
                //    if (xml.TemMensagem == true)
                //    {
                //        if (xml.DescricaoMensagem != null && xml.DescricaoMensagem != String.Empty)
                //            crm.Mensagem = xml.DescricaoMensagem;
                //        else
                //        {
                //            resultadoPersistencia.Sucesso = false;
                //            resultadoPersistencia.Mensagem = "Descrição de Mensagem não enviada quando Tem Mensagem = Sim";
                //            return crm;
                //        }
                //    }
                //    crm.TemMensagem = (bool)xml.TemMensagem;
                //}
                //else
                //{
                //    resultadoPersistencia.Sucesso = false;
                //    resultadoPersistencia.Mensagem = "Tem Mensagem não enviado.";
                //    return crm;
                //}

                if (xml.ShowRoom.HasValue)
                    crm.Showroom = xml.ShowRoom;

                if (xml.ShowRoomRevendas.HasValue)
                    crm.ShowroomRevenda = xml.ShowRoomRevendas;

                if (xml.Backup.HasValue)
                    crm.BackupDistribuidor = xml.Backup;

                if (xml.BackupRevendas.HasValue)
                    crm.BackupRevenda = xml.BackupRevendas;

                if (!String.IsNullOrEmpty(xml.DepositoPadrao))
                    crm.DepositoPadrao = xml.DepositoPadrao;
                else
                    crm.AddNullProperty("DepositoPadrao");

                if (xml.CodigoTipoDespesa.HasValue)
                    crm.CodigoTipoDespesa = xml.CodigoTipoDespesa;

                if (xml.DestaqueNCM.HasValue)
                    crm.DestaqueNCM = xml.DestaqueNCM;

                if (!String.IsNullOrEmpty(xml.NVE))
                    crm.NVE = xml.NVE;
                else
                    crm.AddNullProperty("NVE");

                if (!String.IsNullOrEmpty(xml.CodigoUnidadeFamilia))
                    crm.CodigoUnidadeFamilia = xml.CodigoUnidadeFamilia;
                else
                    crm.AddNullProperty("NVE");

                if (xml.TemMensagem.HasValue)
                {
                    if (xml.TemMensagem == true)
                    {
                        if (xml.DescricaoMensagem != null && xml.DescricaoMensagem != String.Empty)
                            crm.Mensagem = xml.DescricaoMensagem;
                        else
                        {
                            resultadoPersistencia.Sucesso = false;
                            resultadoPersistencia.Mensagem = "Descrição de Mensagem não enviada quando Tem Mensagem = Sim";
                            return crm;
                        }
                    }
                    crm.TemMensagem = (bool)xml.TemMensagem;
                }
                else
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Tem Mensagem não enviado.";
                    return crm;
                }

            }
            else
            {
                crm.ConsideraOrcamentoMeta = null;
                crm.QuantidadeMultiplaProduto = null;
                crm.FaturamentoOutroProduto = null;
                crm.ExigeTreinamento = null;
                crm.CustoAtual = null;
                crm.RebatePosVendaAtivado = null;
                crm.Showroom = null;
            }

            if (xml.EKit.HasValue)
                crm.EKit = xml.EKit.Value;
            else
            {
                resultadoPersistencia.Sucesso = false;
                resultadoPersistencia.Mensagem = "É kit não enviado.";
                return crm;
            }

            if (xml.EKit.Value)
            {
                if (xml.ProdutosFilhos == null || xml.ProdutosFilhos.Count == 0)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Produto definido como Kit mas não foram enviados os produtos filhos.";
                    return crm;
                }
            }
            else
            {
                if (xml.ProdutosFilhos != null && xml.ProdutosFilhos.Count > 0)
                {
                    resultadoPersistencia.Sucesso = false;
                    resultadoPersistencia.Mensagem = "Produto definido como Kit = Não, mas foram enviados os produtos filhos.";
                    return crm;
                }
            }

            if (xml.TipoItem.HasValue)
                crm.TipoItem = xml.TipoItem.Value;

            #endregion

            return crm;
        }

        private Intelbras.Message.Helper.MSG0088 DefinirPropriedades(Product crm)
        {
            Intelbras.Message.Helper.MSG0088 xml = new Pollux.MSG0088(Domain.Enum.Sistemas.RetornaSistema(Domain.Enum.Sistemas.Sistema.CRM), Helper.Truncate(crm.Nome, 40));

            if (crm.Codigo != null)
                xml.CodigoProduto = crm.Codigo;

            if (crm.DataUltAlteracaoPVC.HasValue)
                xml.DataAlteracaoPrecoVenda = crm.DataUltAlteracaoPVC.Value.ToLocalTime();

            if (crm.Showroom.HasValue)
                xml.ShowRoom = crm.Showroom;

            if (crm.Nome != null)
                xml.Nome = crm.Nome;

            if (crm.Descricao != null)
                xml.Descricao = crm.Descricao;

            if (crm.PesoEstoque != null)
                xml.PesoEstoque = crm.PesoEstoque;

            xml.Situacao = (crm.Status.HasValue ? crm.Status.Value : (int)Enum.Produto.StateCode.ativo);

            if (crm.TipoProdutoid.HasValue)
                xml.TipoProduto = crm.TipoProdutoid.Value;

            if (crm.GrupoEstoque != null)
            {
                GrupoEstoque grpEstoq = new Servicos.GrupoEstoqueService(this.Organizacao, this.IsOffline).ObterPor(crm.GrupoEstoque.Id);
                if (grpEstoq != null && grpEstoq.Codigo.HasValue)
                {
                    xml.GrupoEstoque = grpEstoq.Codigo.Value;
                }
            }

            if (crm.UnidadeNegocio != null)
            {
                UnidadeNegocio unidadeNegocio = new Servicos.UnidadeNegocioService(this.Organizacao, this.IsOffline).BuscaUnidadeNegocio(crm.UnidadeNegocio.Id);
                if (unidadeNegocio != null)
                {
                    xml.UnidadeNegocio = unidadeNegocio.ChaveIntegracao;
                    xml.NomeUnidadeNegocio = unidadeNegocio.Nome;
                }
            }

            if (crm.Segmento != null)
            {
                Model.Segmento segmento = new Servicos.SegmentoService(this.Organizacao, this.IsOffline).ObterPor(crm.Segmento.Id);
                if (segmento != null && !String.IsNullOrEmpty(segmento.CodigoSegmento))
                {
                    xml.Segmento = segmento.CodigoSegmento;
                    xml.NomeSegmento = segmento.Nome;
                }
            }

            if (crm.FamiliaProduto != null)
            {
                Model.FamiliaProduto familiaProduto = new Servicos.FamiliaProdutoService(this.Organizacao, this.IsOffline).ObterPor(crm.FamiliaProduto.Id);
                if (familiaProduto != null && !String.IsNullOrEmpty(familiaProduto.Codigo))
                {
                    xml.Familia = familiaProduto.Codigo;
                }
            }

            if (crm.SubfamiliaProduto != null)
            {
                SubfamiliaProduto subFamiliaProduto = new Servicos.SubFamiliaProdutoService(this.Organizacao, this.IsOffline).ObterPor(crm.SubfamiliaProduto.Id);
                if (subFamiliaProduto != null && !String.IsNullOrEmpty(subFamiliaProduto.Codigo))
                {
                    xml.SubFamilia = subFamiliaProduto.Codigo;
                }
            }


            if (crm.Origem != null)
            {
                Origem Origem = new Servicos.OrigemService(this.Organizacao, this.IsOffline).ObterPor(crm.Origem.Id);
                if (Origem != null && !String.IsNullOrEmpty(Origem.Codigo))
                {
                    xml.Origem = Origem.Codigo;
                }
            }


            if (crm.UnidadePadrao != null)
            {
                Unidade unidade = new Servicos.UnidadeService(this.Organizacao, this.IsOffline).BuscaPor(crm.UnidadePadrao.Id);
                if (unidade != null && !String.IsNullOrEmpty(unidade.Nome))
                {
                    xml.UnidadeMedida = unidade.Nome;
                }
            }

            if (crm.GrupoUnidades != null)
            {
                GrupoUnidade grupoUnidade = new Servicos.GrupoUnidadeMedidaService(this.Organizacao, this.IsOffline).ObterPor(crm.GrupoUnidades.Id);
                if (grupoUnidade != null)
                {
                    xml.GrupoUnidadeMedida = grupoUnidade.Nome;
                }
            }

            if (crm.FamiliaComercial != null)
            {
                FamiliaComercial familiaComercial = new Servicos.FamiliaComercialService(this.Organizacao, this.IsOffline).ObterPor(crm.FamiliaComercial.Id);
                if (familiaComercial != null && !String.IsNullOrEmpty(familiaComercial.Codigo))
                {
                    xml.FamiliaComercial = familiaComercial.Codigo;
                }
            }

            if (crm.QuantidadeMultiplaProduto.HasValue)
                xml.QuantidadeMultipla = crm.QuantidadeMultiplaProduto.Value;

            if (crm.NaturezaProduto != null)
                xml.NaturezaProduto = crm.NaturezaProduto.Value;


            if (crm.FamiliaMaterial != null)
            {
                FamiliaMaterial familiaMaterial = new Servicos.FamiliaMaterialService(this.Organizacao, this.IsOffline).ObterPor(crm.FamiliaMaterial.Id);
                if (familiaMaterial != null && !String.IsNullOrEmpty(familiaMaterial.Codigo))
                {
                    xml.FamiliaMaterial = familiaMaterial.Codigo;
                }
            }

            if (crm.FamiliaMaterial != null)
            {
                FamiliaMaterial familiaMaterial = new Servicos.FamiliaMaterialService(this.Organizacao, this.IsOffline).ObterPor(crm.FamiliaMaterial.Id);
                if (familiaMaterial != null && !String.IsNullOrEmpty(familiaMaterial.Codigo))
                {
                    xml.FamiliaMaterial = familiaMaterial.Codigo;
                }
            }


            if (crm.ListaPrecoPadrao != null)
                xml.ListaPreco = crm.ListaPrecoPadrao.Name;

            if (crm.Moeda != null)
                xml.Moeda = crm.Moeda.Name;

            if (crm.QuantidadeDecimal.HasValue)
                xml.QuantidadeDecimal = crm.QuantidadeDecimal.Value;

            if (crm.ExigeTreinamento.HasValue)
                xml.ExigeTreinamento = crm.ExigeTreinamento.Value;

            if (crm.RebatePosVendaAtivado.HasValue)
                xml.RebateAtivado = crm.RebatePosVendaAtivado.Value;

            if (crm.NomeFornecedor != null)
                xml.Fabricante = crm.NomeFornecedor;

            if (crm.NumeroPecaFabricante != null)
                xml.NumeroPecaFabricante = crm.NumeroPecaFabricante;

            if (crm.VolumeEstoque != null)
                xml.VolumeEstoque = crm.VolumeEstoque;

            if (crm.Complemento != null)
                xml.ComplementoProduto = crm.Complemento;

            if (crm.Url != null)
                xml.URL = crm.Url;

            if (crm.QuantidadeDisponivel.HasValue)
                xml.QuantidadeDisponivel = crm.QuantidadeDisponivel.Value;

            if (crm.Fornecedor != null)
                xml.Fornecedor = crm.Fornecedor;

            if (crm.ConsideraOrcamentoMeta.HasValue)
                xml.ConsiderarOrcamentoMeta = crm.ConsideraOrcamentoMeta.Value;

            if (crm.FaturamentoOutroProduto.HasValue)
                xml.FaturamentoOutroProduto = crm.FaturamentoOutroProduto.Value;

            xml.TemMensagem = crm.TemMensagem;
            if (crm.TemMensagem == true)
            {
                if (crm.Mensagem != null && crm.Mensagem != String.Empty)
                    xml.DescricaoMensagem = crm.Mensagem;
            }
            else
            {
                xml.TemMensagem = false;
            }

            xml.AliquotaIPI = crm.PercentualIPI;

            xml.EAN = crm.EAN;
            xml.NCM = crm.NCM;

            xml.CustoPadrao = (crm.CustoAtual.HasValue) ? crm.CustoAtual.Value : 0;
            xml.CustoAtual = crm.CustoAtual;
            xml.BloquearComercializacao = crm.BloquearComercializacao;
            xml.ComercializadoForaKit = crm.ComercializadoForaKit;
            xml.PoliticaPosVendas = crm.PoliticaPosVenda;

            if (crm.PoliticaPosVenda != null)
                xml.DescricaoPoliticaPosVendas = DescricaoPoliticaPosVenda(crm.PoliticaPosVenda.Value);

            xml.TempoGarantia = crm.TempoGarantia;
            xml.EKit = crm.EKit;
            xml.PossuiSubstituto = crm.PossuiSubstituto;
            xml.PassivelSolicitacaoBeneficio = crm.PermitirEmSolBenef;

            xml.Backup = crm.BackupDistribuidor;
            xml.BackupRevendas = crm.BackupRevenda;
            xml.ShowRoomRevendas = crm.ShowroomRevenda;


            if (!String.IsNullOrEmpty(crm.DepositoPadrao))
                xml.DepositoPadrao = crm.DepositoPadrao;

            if (crm.CodigoTipoDespesa.HasValue)
                xml.CodigoTipoDespesa = crm.CodigoTipoDespesa;

            if (crm.DestaqueNCM.HasValue)
                xml.DestaqueNCM = crm.DestaqueNCM;

            if (!String.IsNullOrEmpty(crm.NVE))
                xml.NVE = crm.NVE;

            if (!String.IsNullOrEmpty(crm.CodigoUnidadeFamilia))
                xml.CodigoUnidadeFamilia = crm.CodigoUnidadeFamilia;

            if (crm.ProdutoSubstituto != null)
            {
                var produtoSubstituto = RepositoryService.Produto.Retrieve(crm.ProdutoSubstituto.Id, "productnumber");

                if (produtoSubstituto == null)
                {
                    throw new ArgumentException("(CRM) Produto substituto não encontrado!");
                }

                xml.CodigoProdutoSubstituto = produtoSubstituto.Codigo;
            }

            xml.TipoItem = crm.TipoItem;

            if (crm.LinhaProduto != null)
            {
                LinhaComercial linhaProduto = (new CRM2013.Domain.Servicos.RepositoryService()).LinhaComercial.Retrieve(crm.LinhaProduto.Id);
                if (linhaProduto != null)
                {
                    xml.LinhaProduto = linhaProduto.Id.ToString();
                    xml.NomeLinhaProduto = linhaProduto.Nome;
                }
            }

            return xml;
        }

        public List<Model.ProdutoKit> DefinirPropriedades(Guid produtoID, List<Pollux.Entities.ProdutoFilho> listaProdutoKitBarramento)
        {
            List<Model.ProdutoKit> listaProdutoKitNovos = new List<Model.ProdutoKit>();

            if (listaProdutoKitBarramento != null)
            {
                List<String> listaCodigoFilhos = (from item in listaProdutoKitBarramento
                                                  select item.CodigoProduto).ToList<String>();

                if (listaCodigoFilhos.Count > 0)
                {
                    List<Model.Product> listaProdutoFilhos = RepositoryService.Produto.ListarPor(listaCodigoFilhos, false, "productid", "productnumber");


                    foreach (var item in listaProdutoKitBarramento)
                    {
                        var produto = listaProdutoFilhos.FirstOrDefault(i => i.Codigo == item.CodigoProduto);

                        listaProdutoKitNovos.Add(new Model.ProdutoKit(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ProdutoPai = new Lookup(produtoID, ""),
                            ProdutoFilho = new Lookup(produto.ID.Value, ""),
                            Quantidade = item.QuantidadeProdutoFilho.Value
                        });
                    }
                }
            }

            return listaProdutoKitNovos;
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Product objModel)
        {
            string resposta;
            Intelbras.Message.Helper.MSG0088 mensagem = DefinirPropriedades(objModel);

            Domain.Servicos.Integracao integracao = new Servicos.Integracao(this.Organizacao, this.IsOffline);
            string msgEnvio = mensagem.GenerateMessage(true);

            if (integracao.EnviarMensagemBarramento(msgEnvio, "1", "1", out resposta))
            {
                Intelbras.Message.Helper.MSG0088R1 retorno = CarregarMensagem<Pollux.MSG0088R1>(resposta);
                if (!retorno.Resultado.Sucesso)
                {
                    throw new ArgumentException("(CRM) " + retorno.Resultado.Mensagem);
                }
                return retorno.Resultado.Mensagem;
            }
            else
            {
                Intelbras.Message.Helper.ERR0001 retorno = CarregarMensagem<Pollux.ERR0001>(resposta);
                throw new ArgumentException("(CRM) " + retorno.DescricaoErro);
            }
        }

        private string DescricaoPoliticaPosVenda(int politicaPosVenda)
        {
            switch (politicaPosVenda)
            {
                case ((int)PoliticaPosVenda.Troca_Produto_Via_Distribuidor):
                    return "Troca de Produto Via Distribuidor";
                case ((int)PoliticaPosVenda.Reparo_Fabrica_SC_Via_Autorizada):
                    return "Reparo na Fábrica SC via Autorizada";
                case ((int)PoliticaPosVenda.Reparo_Via_Autorizada):
                    return "Reparo via Autorizada";
                case ((int)PoliticaPosVenda.Troca_Expressa_Via_Distribuidor_Reparo_Via_Autorizada):
                    return "Troca Expressa Distribuidor / Reparo Autorizada";
                case ((int)PoliticaPosVenda.Reparo_Fabrica_SC_Via_Distribuidor):
                    return "Reparo na Fábrica SC via Distribuidor";
                case ((int)PoliticaPosVenda.Troca_Produto_Via_Rede_Autorizada):
                    return "Troca de Produto via Rede Autorizada";
                case ((int)PoliticaPosVenda.Troca_Expressa_Via_Fabrica):
                    return "Troca Expressa via Fábrica";
            }

            return "Valor inválido";
        }
        #endregion
    }
}
