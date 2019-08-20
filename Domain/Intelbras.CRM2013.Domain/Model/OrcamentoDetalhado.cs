using Intelbras.CRM2013.Domain.Servicos;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    public class Trimestre
    {
        #region propetys
        public Guid? Id { get; set; }

        public int? trimestre { get; set; }

        public int? Mes1 { get; set; }
        public decimal? Mes1Vlr { get; set; }
        public Int64? Mes1Qtde { get; set; }

        public int? Mes2 { get; set; }
        public decimal? Mes2Vlr { get; set; }
        public Int64? Mes2Qtde { get; set; }

        public int? Mes3 { get; set; }
        public decimal? Mes3Vlr { get; set; }
        public Int64? Mes3Qtde { get; set; }
        #endregion
    }

    /// <summary>
    /// Classe criada para alimentar a planilha excel detalhada e criar a 
    /// árvore de registro de orçamento detalhado
    /// </summary>
    public class OrcamentoDetalhado
    {
        #region Objetos de Serviços
        private ContaService _ServiceConta = null;
        private ContaService ServiceConta
        {
            get
            {
                if (_ServiceConta == null)
                    _ServiceConta = new ContaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceConta;
            }
        }

        private UsuarioService _ServiceUsuario = null;
        private UsuarioService ServiceUsuario
        {
            get
            {
                if (_ServiceUsuario == null)
                    _ServiceUsuario = new UsuarioService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceUsuario;
            }
        }

        private ContatoService _ServiceContato = null;
        private ContatoService ServiceContato
        {
            get
            {
                if (_ServiceContato == null)
                    _ServiceContato = new ContatoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceContato;
            }
        }

        private ProdutoService _ServiceProduto = null;
        private ProdutoService ServiceProduto
        {
            get
            {
                if (_ServiceProduto == null)
                    _ServiceProduto = new ProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ServiceProduto;
            }
        }
        #endregion

        #region Atributos

        public Guid? Id { get; set; }

        public DateTime DatadoArquivo { get; set; }

        public int? Ano { get; set; }

        #region Propertys de controle
        public Guid? ProdutoID { get; set; }

        public Guid? SegmentoID { get; set; }

        public Guid? FamiliaID { get; set; }

        public Guid? SubFamiliaID { get; set; }

        public Guid? CanalID { get; set; }
        #endregion

        #region propertys de geração da planilha
        public Lookup UnidadeNegocio { get; set; }

        public Lookup Segmento { get; set; }
        public Lookup PotencialRepresentanteKA { get; set; }

        public Lookup Familia { get; set; }

        public Lookup SubFamilia { get; set; }

        public Lookup Produto { get; set; }

        private Product _Product = null;
        public Product Product
        {
            get
            {
                if (_Product == null && Produto.Id != null && Produto.Id != Guid.Empty)
                    _Product = ServiceProduto.ObterPor(Produto.Id);

                return _Product;
            }
        }

        public string StatusProduto { get; set; }

        public Lookup Canal { get; set; }

        private Conta _conta = null;
        public Conta Account
        {
            get
            {
                if (Canal.Type == SDKore.Crm.Util.Utility.GetEntityName<Conta>())
                {
                    if (_conta == null && Canal.Id != null && Canal.Id != Guid.Empty)
                        _conta = ServiceConta.BuscaConta(Canal.Id);
                }

                return _conta;
            }
        }

        private Usuario _usuario = null;
        public Usuario User
        {
            get
            {
                if (Canal.Type == SDKore.Crm.Util.Utility.GetEntityName<Usuario>())
                {
                    if (_usuario == null && Canal.Id != null && Canal.Id != Guid.Empty)
                        _usuario = ServiceUsuario.ObterPor(Canal.Id);
                }

                return _usuario;
            }
        }

        private Contato _contato = null;

        public Contato Contact
        {
            get
            {
                if (Canal.Type == SDKore.Crm.Util.Utility.GetEntityName<Contato>())
                {
                    if (_contato == null && Canal.Id != null && Canal.Id != Guid.Empty)
                        _contato = ServiceContato.BuscaContato(Canal.Id);
                }

                return _contato;
            }
        }

        public string StatusParticipacao { get; set; }

        public Decimal? QtdePlanejada { get; set; }

        public Decimal? OrcamentoPlanejado { get; set; }

        #endregion

        public bool AtualizarTrimestre1
        {
            get
            {
                return true;
                #region comentado conforme gabriel e souza solicitaram
                //if (!_atualiza1.HasValue)
                //{
                //    DateTime i1Trimestre = new DateTime(this.Ano.Value, 1, 1);
                //    DateTime f1Trimestre = new DateTime(this.Ano.Value, 3, 31);
                //    _atualiza1 = false;

                //    //if (this.DatadoArquivo.Date <= i1Trimestre.Date)
                //    if (this.Ano.Value <= DateTime.Now.Year)
                //    {
                //        if (this.DatadoArquivo.Date >= i1Trimestre.Date && this.DatadoArquivo.Date <= f1Trimestre.Date)
                //        {
                //            _atualiza1 = true;
                //            _atualiza2 = true;
                //            _atualiza3 = true;
                //            _atualiza4 = true;
                //        }
                //    }
                //    else
                //    {
                //        _atualiza1 = true;
                //        _atualiza2 = true;
                //        _atualiza3 = true;
                //        _atualiza4 = true;
                //    }

                //}
                //return _atualiza1.Value;
                #endregion
            }
        }
        public Trimestre Trimestre1 { get; set; }

        public bool AtualizarTrimestre2
        {
            get
            {
                return true;
                #region comentado conforme gabriel e souza solicitaram
                //if (!_atualiza2.HasValue)
                //{
                //    DateTime i2Trimestre = new DateTime(this.Ano.Value, 4, 1);
                //    DateTime f2Trimestre = new DateTime(this.Ano.Value, 6, 30);
                //    _atualiza2 = false;

                //    //if (this.DatadoArquivo.Date <= i2Trimestre.Date)
                //    if (this.Ano.Value <= DateTime.Now.Year)
                //    {
                //        if (this.DatadoArquivo.Date >= i2Trimestre.Date && this.DatadoArquivo.Date <= f2Trimestre.Date)
                //        {
                //            _atualiza2 = true;
                //            _atualiza3 = true;
                //            _atualiza4 = true;
                //        }
                //    }
                //    else
                //    {
                //        _atualiza2 = true;
                //        _atualiza3 = true;
                //        _atualiza4 = true;
                //    }

                //}
                //return _atualiza2.Value;
                #endregion
            }
        }
        public Trimestre Trimestre2 { get; set; }

        public bool AtualizarTrimestre3
        {
            get
            {
                return true;
                #region comentado conforme gabriel e souza solicitaram
                //if (!_atualiza3.HasValue)
                //{
                //    DateTime i3Trimestre = new DateTime(this.Ano.Value, 7, 1);
                //    DateTime f3Trimestre = new DateTime(this.Ano.Value, 9, 30);
                //    _atualiza3 = false;

                //    //if (this.DatadoArquivo.Date <= i3Trimestre.Date)
                //    if (this.Ano.Value <= DateTime.Now.Year)
                //    {
                //        if (this.DatadoArquivo.Date >= i3Trimestre.Date && this.DatadoArquivo.Date <= f3Trimestre.Date)
                //        {
                //            _atualiza3 = true;
                //            _atualiza4 = true;
                //        }
                //    }
                //    else
                //    {
                //        _atualiza3 = true;
                //        _atualiza4 = true;
                //    }
                //}
                //return _atualiza3.Value;
                #endregion
            }
        }
        public Trimestre Trimestre3 { get; set; }

        public bool AtualizarTrimestre4
        {
            get
            {
                return true;
                #region comentado conforme gabriel e souza solicitaram
                //if (!_atualiza4.HasValue)
                //{
                //    DateTime i4Trimestre = new DateTime(this.Ano.Value, 10, 1);
                //    DateTime f4Trimestre = new DateTime(this.Ano.Value, 12, 31);
                //    _atualiza4 = false;

                //    //if (this.DatadoArquivo.Date <= i4Trimestre.Date)
                //    if (this.Ano.Value <= DateTime.Now.Year)
                //    {
                //        if (this.DatadoArquivo.Date >= i4Trimestre.Date && this.DatadoArquivo.Date <= f4Trimestre.Date)
                //            _atualiza4 = true;
                //    }
                //    else
                //    {
                //        _atualiza4 = true;
                //    }
                //}
                //return _atualiza4.Value;
                #endregion
            }
        }
        public Trimestre Trimestre4 { get; set; }
        #endregion
    }
}
