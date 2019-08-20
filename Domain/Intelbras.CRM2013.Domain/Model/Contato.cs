using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("contact")]
    public class Contato : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Contato() { }

        public Contato(string organization, bool isOffline)
                : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Contato(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("contactid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("entityimage")]
        public Byte[] Imagem { get; set; }

        [LogicalAttribute("accountrolecode")]
        public Int32? Funcao { get; set; }

        [LogicalAttribute("address1_addresstypecode")]
        public Int32? Endereco1TipoEndereco { get; set; }

        //[LogicalAttribute("address1_city")]
        //public String Endereco1Cidade { get; set; }

        //[LogicalAttribute("address1_country")]
        //public String Endereco1Regiao { get; set; }

        [LogicalAttribute("address1_county")]
        public String Endereco1MunicipioId { get; set; }

        [LogicalAttribute("address1_county")]
        public String Endereco1Municipio { get; set; }

        [LogicalAttribute("address1_freighttermscode")]
        public Int32? Endereco1CondicoesFrete { get; set; }

        //[LogicalAttribute("address1_line1")]
        //public String Endereco1Rua1 { get; set; }

        [LogicalAttribute("address1_line2")]
        public String Endereco1Bairro { get; set; }

        [LogicalAttribute("address1_line3")]
        public String Endereco1Complemento { get; set; }

        [LogicalAttribute("address1_name")]
        public String Endereco1Nome { get; set; }

        [LogicalAttribute("address1_postalcode")]
        public String Endereco1CEP { get; set; }

        [LogicalAttribute("address1_postofficebox")]
        public String Endereco1CaixaPostal { get; set; }

        [LogicalAttribute("address1_shippingmethodcode")]
        public Int32? Endereco1MetodoEntrega { get; set; }

        [LogicalAttribute("address1_stateorprovince")]
        public String Endereco1Estado { get; set; }

        [LogicalAttribute("address1_telephone1")]
        public String Endereco1Telefone { get; set; }

        [LogicalAttribute("anniversary")]
        public DateTime? DatasEspeciais { get; set; }

        [LogicalAttribute("assistantname")]
        public String Assistente { get; set; }

        [LogicalAttribute("assistantphone")]
        public String TelefoneAssistente { get; set; }

        [LogicalAttribute("birthdate")]
        public DateTime? Aniversario { get; set; }

        [LogicalAttribute("childrensnames")]
        public String NomeFilhos { get; set; }

        [LogicalAttribute("creditlimit")]
        public Decimal? LimiteCredito { get; set; }

        [LogicalAttribute("creditonhold")]
        public Boolean? SuspensaoCredito { get; set; }

        [LogicalAttribute("customertypecode")]
        public Int32? TipoRelacao { get; set; }

        [LogicalAttribute("defaultpricelevelid")]
        public Lookup ListaPrecos { get; set; }

        [LogicalAttribute("department")]
        public String Departamento { get; set; }

        [LogicalAttribute("description")]
        public String Descricao { get; set; }

        [LogicalAttribute("donotbulkemail")]
        public Boolean? EmailMarketing { get; set; }

        //[LogicalAttribute("donotemail")]
        //public Boolean? NaoPermitirEmail { get; set; }

        //[LogicalAttribute("donotfax")]
        //public Boolean? NaoPermitirFax { get; set; }

        //[LogicalAttribute("donotphone")]
        //public Boolean? NaoPermitirTelefonema { get; set; }

        //[LogicalAttribute("donotpostalmail")]
        //public Boolean? NaoPermitirCorrespondencia { get; set; }

        //[LogicalAttribute("donotsendmm")]
        //public Boolean? EnviarMaterialMarketing { get; set; }

        [LogicalAttribute("emailaddress1")]
        public String Email1 { get; set; }

        [LogicalAttribute("emailaddress2")]
        public String Email2 { get; set; }

        [LogicalAttribute("familystatuscode")]
        public Int32? EstadoCivil { get; set; }

        [LogicalAttribute("fax")]
        public String Fax { get; set; }

        [LogicalAttribute("firstname")]
        public String PrimeiroNome { get; set; }

        [LogicalAttribute("fullname")]
        public String NomeCompleto { get; set; }
        [LogicalAttribute("fullname")]
        public String Nome { get; set; }

        [LogicalAttribute("gendercode")]
        public int? Sexo { get; set; }

        [LogicalAttribute("haschildrencode")]
        public int? TemFilhos { get; set; }

        [LogicalAttribute("itbc_address1_city")]
        public Lookup Endereco1Municipioid { get; set; }

        [LogicalAttribute("itbc_address1_country")]
        public Lookup Endereco1Pais { get; set; }

        [LogicalAttribute("itbc_address1_number")]
        public String Endereco1Numero { get; set; }

        [LogicalAttribute("itbc_address1_stateorprovince")]
        public Lookup Endereco1Estadoid { get; set; }

        [LogicalAttribute("itbc_address1_street")]
        public String Endereco1Rua { get; set; }

        [LogicalAttribute("itbc_area")]
        public Int32? Area { get; set; }

        public string NomeArea { get; set; }

        [LogicalAttribute("itbc_cargo")]
        public Int32? Cargo { get; set; }

        public string NomeCargo { get; set; }

        [LogicalAttribute("itbc_codigodoemitente")]
        public String CodigoRemetente { get; set; }

        [LogicalAttribute("itbc_codigodorepresentante")]
        public String CodigoRepresentante { get; set; }

        [LogicalAttribute("itbc_contactnumber")]
        public String NumeroContato { get; set; }

        [LogicalAttribute("itbc_contatonfe")]
        public Int32? ContatoNFE { get; set; }

        [LogicalAttribute("itbc_cpfoucnpj")]
        public String CpfCnpj { get; set; }

        [LogicalAttribute("itbc_docidentidade")]
        public String DocIdentidade { get; set; }

        [LogicalAttribute("itbc_emissordocidentidade")]
        public String EmissorDocIdentidade { get; set; }

        [LogicalAttribute("itbc_escolaridade")]
        public Int32? Escolaridade { get; set; }

        [LogicalAttribute("itbc_loja")]
        public Lookup Loja { get; set; }

        [LogicalAttribute("itbc_nacionalidade")]
        public String Nacionalidade { get; set; }

        [LogicalAttribute("itbc_naturalidade")]
        public String Naturalidade { get; set; }

        [LogicalAttribute("itbc_natureza")]
        public Int32? Natureza { get; set; }

        [LogicalAttribute("itbc_papelnocanal")]
        public Int32? PapelCanal { get; set; }

        [LogicalAttribute("itbc_descricaodopapelnocanal")]
        public String DescricaoPapelCanal { get; set; }

        [LogicalAttribute("itbc_areadeatuacao")]
        public Int32? AreaAtuacao { get; set; }

        [LogicalAttribute("itbc_areaatuacaotexto")]
        public String AreaAtuacaoTexto { get; set; }

        [LogicalAttribute("itbc_descricaodaareadeatuacao")]
        public String DescricaoAreaAtuacao { get; set; }

        [LogicalAttribute("itbc_ramal_fax")]
        public String RamalFax { get; set; }

        [LogicalAttribute("itbc_ramal_telefone1")]
        public String Ramal1 { get; set; }

        [LogicalAttribute("itbc_ramal_telefone2")]
        public String Ramal2 { get; set; }

        [LogicalAttribute("itbc_territory")]
        public Lookup Endereco1Regiaoid { get; set; }

        [LogicalAttribute("jobtitle")]
        public String Cargoid { get; set; }

        [LogicalAttribute("lastname")]
        public String Sobrenome { get; set; }

        //[LogicalAttribute("lastusedincampaign")]
        //public DateTime? DataUltimaInclusaoCampanha { get; set; }

        [LogicalAttribute("managername")]
        public String Gerente { get; set; }

        [LogicalAttribute("managerphone")]
        public String TelefoneGerente { get; set; }

        [LogicalAttribute("middlename")]
        public String SegundoNome { get; set; }

        [LogicalAttribute("mobilephone")]
        public String TelefoneCelular { get; set; }

        [LogicalAttribute("numberofchildren")]
        public int? NumeroFilhos { get; set; }

        [LogicalAttribute("websiteurl")]
        public String WebSite { get; set; }

        [LogicalAttribute("originatingleadid")]
        public Lookup ClientePotencialOriginador { get; set; }

        [LogicalAttribute("parentcustomerid")]
        public Lookup AssociadoA { get; set; }

        private Domain.Model.Conta _cliente = null;
        public Domain.Model.Conta Cliente
        {
            get
            {
                if (_cliente == null && this.AssociadoA != null)
                {
                    _cliente = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Conta.Retrieve(this.AssociadoA.Id);
                }
                return _cliente;
            }
            set { _cliente = value; }
        }

        //[LogicalAttribute("paymenttermscode")]
        //public int? CondicoesPagamento { get; set; }

        //[LogicalAttribute("preferredappointmentdaycode")]
        //public int? DiaPreferencial { get; set; }

        //[LogicalAttribute("preferredappointmenttimecode")]
        //public int? HorarioPreferencial { get; set; }

        //[LogicalAttribute("preferredcontactmethodcode")]
        //public int? FormaPreferencialContato { get; set; }

        //[LogicalAttribute("preferredequipmentid")]
        //public Lookup InstalacoesEquipamentos { get; set; }

        //[LogicalAttribute("preferredserviceid")]
        //public Lookup ServicoPreferencial { get; set; }

        //[LogicalAttribute("preferredsystemuserid")]
        //public Lookup UsuarioPreferencial { get; set; }

        [LogicalAttribute("salutation")]
        public String Saudacao { get; set; }

        [LogicalAttribute("spousesname")]
        public String NomeConjuge { get; set; }

        [LogicalAttribute("telephone1")]
        public String TelefoneComercial { get; set; }

        [LogicalAttribute("telephone2")]
        public String TelefoneResidencial { get; set; }

        [LogicalAttribute("telephone3")]
        public String TelefoneComercial2 { get; set; }

        ////[LogicalAttribute("TipoCustomer")]
        //public String TipoCliente { get; set; }

        [LogicalAttribute("transactioncurrencyid")]
        public Lookup Moeda { get; set; }

        [LogicalAttribute("itbc_guidcrm40")]
        public String GuidCrm40 { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_IntegraBarramento")]
        public Boolean? IntegrarNoBarramento { get; set; }

        [LogicalAttribute("itbc_acesso")]
        public Boolean? AcessoPortal { get; set; }

        [LogicalAttribute("itbc_imei")]
        public String IMEI { get; set; }

        [LogicalAttribute("itbc_acesso_app_isol")]
        public Boolean? AcessoAPPISOL { get; set; }

        [LogicalAttribute("itbc_treinamento")]
        public String Treinamento { get; set; }

        //CRM4
        [LogicalAttribute("new_login")]
        public string Login { get; set; }
        [LogicalAttribute("new_participafidelidade")]
        public bool? ParticipaFidelidade { get; set; }
        [LogicalAttribute("new_administrador_fidelidade")]
        public bool? AdministradorFidelidade { get; set; }
        [LogicalAttribute("new_data_atualizacao_b2b")]
        public DateTime? DataUltimaAtualizacao { get; set; }
        [LogicalAttribute("new_total_pontos_resgatados")]
        public int? TotalPontosResgatados { get; set; }
        [LogicalAttribute("new_saldo_pontos_fidelidade")]
        public int? SaldoPontosFidelidade { get; set; }
        [LogicalAttribute("new_nao_permitir_sms")]
        public bool? EnvioSMS { get; set; }
        [LogicalAttribute("new_acessoportal")]
        public bool? AcessoAoPortal { get; set; }
        public bool? ConsultaRelatorioB2B { get; set; }
        public bool? AcessoPortalTreinamentos { get; set; }

        private Regional regional;
        public Regional Regional
        {
            get { return regional; }
            set { regional = value; }
        }
        private Enum.Contato.TipoAcesso _acesso;
        public Enum.Contato.TipoAcesso Acesso
        {
            get
            {
                if (_acesso == Enum.Contato.TipoAcesso.NaoDefinido && this.ID.HasValue)
                {
                    _acesso = Enum.Contato.TipoAcesso.NaoDefinido;
                    if (this.AssociadoA != null)
                    {
                        if (this.Cliente.Grupo != null)
                        {
                            GrupoDoCliente grupoCliente = (new Domain.Servicos.RepositoryService()).GrupoDoCliente.ObterPor(this.Id);

                            if (grupoCliente.IsDistribuir)
                            {
                                if (this.AdministradorFidelidade.HasValue && this.AdministradorFidelidade.Value)
                                    _acesso = Enum.Contato.TipoAcesso.Distribuidor;
                                else if (this.Cargo.HasValue && this.Cargo.Value == (int)Enum.Contato.Cargo.Vendedor)
                                    _acesso = Enum.Contato.TipoAcesso.Vendedor;
                            }
                            else if (grupoCliente.IsRevenda)
                            {
                                _acesso = Enum.Contato.TipoAcesso.Revenda;
                            }

                            if (_acesso == Enum.Contato.TipoAcesso.NaoDefinido)
                            {
                                throw new ArgumentException("Tipo de acesso não definido");
                            }
                        }
                    }
                }

                return _acesso;
            }
            set { _acesso = value; }
        }

        [LogicalAttribute("itbc_acesso_solar")]
        public bool? AcessoSolar { get; set; }

        [LogicalAttribute("itbc_exibir_site")]
        public bool? ExibirSite { get; set; }

        [LogicalAttribute("itbc_canais_de_venda")]
        public String CanaisDeVenda { get; set; }
        #endregion

        #region Metodos

        public List<Contato> ListarContatosPor(Contato contato)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ListarContatosPor(contato);
        }

        public Contato PesquisarPorLogin(string login, Guid contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ObterPor(login, contatoId);
        }

        public int TipoAcessoPortal(string login)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.TipoAcessoPortal(login);
        }

        public void Mesclar(Contato contato)
        {
            if (this.TelefoneComercial == null) this.TelefoneComercial = contato.TelefoneComercial;
            if (this.TelefoneCelular == null) this.TelefoneCelular = contato.TelefoneCelular;
            if (this.Nome == null) this.Nome = contato.Nome;
            if (this.Sobrenome == null) this.Sobrenome = contato.Sobrenome;
            if (this.NomeCompleto == null) this.NomeCompleto = contato.NomeCompleto;
            if (this.Email1 == null) this.Email1 = contato.Email1;
            if (this.Email2 == null) this.Email2 = contato.Email2;
            if (this.CpfCnpj == null) this.CpfCnpj = contato.CpfCnpj;
            if (this.Endereco1CEP == null) this.Endereco1CEP = contato.Endereco1CEP;
            if (this.Endereco1Bairro == null) this.Endereco1Bairro = contato.Endereco1Bairro;
            if (this.Endereco1Rua == null) this.Endereco1Rua = contato.Endereco1Rua;
            if (this.Endereco1Numero == null) this.Endereco1Numero = contato.Endereco1Numero;
            if (this.Endereco1Complemento == null) this.Endereco1Complemento = contato.Endereco1Complemento;
            if (this.Endereco1Municipio == null) this.Endereco1Municipio = contato.Endereco1Municipio;
            if (this.Endereco1Estado == null) this.Endereco1Estado = contato.Endereco1Estado;
            if (this.Endereco1Pais == null) this.Endereco1Pais = contato.Endereco1Pais;
        }

        public void Update(Contato entity)
        {
            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.Update(entity);
        }

        public bool ExisteDuplicidade(out string mensagem)
        {
            Contato contatoAux = null;
            mensagem = "";

            // Valida CPF
            if (!String.IsNullOrEmpty(this.CpfCnpj))
            {
                contatoAux = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ObterPor(this.CpfCnpj);
                if (contatoAux != null)
                {
                    if (this.Id == null)  //create
                    {
                        mensagem = String.Format("Não foi possível Criar esse contato, CPF {0} já cadastrado", this.CpfCnpj);
                        return true;
                    }
                    else if (this.Id != contatoAux.Id)//update
                    {
                        mensagem = String.Format("Não foi possível Atualizar esse contato, CPF {0} já cadastrado", this.CpfCnpj);
                        return true;
                    }
                }
            }

            // Valida Login
            if (!String.IsNullOrEmpty(this.Login))
            {
                contatoAux = this.PesquisarPorLogin(this.Login, this.Id);
                if (contatoAux != null)
                {
                    if (this.Id == null) // Criação
                    {
                        mensagem = String.Format("Não foi possível Criar esse contato, Login {0} já cadastrado", this.Login);
                        return true;
                    }
                    else if (this.Id != contatoAux.Id) // Atualização
                    {
                        mensagem = String.Format("Não foi possível Atualizar esse contato, Login {0} já cadastrado", this.Login);
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateEmailFBA()
        {
            if (string.IsNullOrEmpty(this.Login))
                throw new ArgumentNullException("Login", "Login não pode ser vazio");

            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.UpdateEmailFBA(this);
        }

        public void EnviaEmailAcessoPortalCorporativo()
        {
            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.EnviaEmailAcessoPortalCorporativo(this);
        }

        public void CopiarDadosDe(Contato contatoACopiar)
        {
            if (contatoACopiar.Id != Guid.Empty) this.Id = contatoACopiar.Id;
            if (!String.IsNullOrEmpty(contatoACopiar.CpfCnpj)) this.CpfCnpj = contatoACopiar.CpfCnpj;
            if (!String.IsNullOrEmpty(contatoACopiar.Nome)) this.Nome = contatoACopiar.Nome;
            if (!String.IsNullOrEmpty(contatoACopiar.Email1)) this.Email1 = contatoACopiar.Email1;
            if (!String.IsNullOrEmpty(contatoACopiar.Endereco1Bairro)) this.Endereco1Bairro = contatoACopiar.Endereco1Bairro;
            if (!String.IsNullOrEmpty(contatoACopiar.Endereco1Rua)) this.Endereco1Rua = contatoACopiar.Endereco1Rua;
            if (!String.IsNullOrEmpty(contatoACopiar.Endereco1Numero)) this.Endereco1Numero = contatoACopiar.Endereco1Numero;
            if (!String.IsNullOrEmpty(contatoACopiar.Endereco1Complemento)) this.Endereco1Complemento = contatoACopiar.Endereco1Complemento;
            if (!String.IsNullOrEmpty(contatoACopiar.Endereco1CEP)) this.Endereco1CEP = contatoACopiar.Endereco1CEP;

            if (contatoACopiar.Endereco1Estado != null)
            {
                this.Endereco1Estadoid = new Lookup((new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Estado.ObterPor(contatoACopiar.Endereco1Estado).Id, contatoACopiar.Endereco1Estado, "itbc_estado");

                if (contatoACopiar.Endereco1Municipio != null)
                    this.Endereco1Municipioid = new Lookup((new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Municipio.ObterPor(this.Endereco1Estadoid.Id, contatoACopiar.Endereco1Municipio).Id, contatoACopiar.Endereco1Municipio, "itbc_municipio");
            }

            if (!String.IsNullOrEmpty(contatoACopiar.TelefoneComercial)) this.TelefoneComercial = contatoACopiar.TelefoneComercial;
            if (!String.IsNullOrEmpty(contatoACopiar.TelefoneCelular)) this.TelefoneCelular = contatoACopiar.TelefoneCelular;
            if (!String.IsNullOrEmpty(contatoACopiar.Fax)) this.Fax = contatoACopiar.Fax;

            this.Endereco1Pais = new Lookup((new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Pais.PesquisarPaisPor("BRASIL").Id, "BRASIL", "itbc_pais");
        }

        public void ExecutaWorkFlow(Guid WorkFlowId)
        {
            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ExecutaWorkFlow(this, WorkFlowId);
        }

        public Lookup Converter(string nome, string email, string cidade, string uf, string telefone)
        {
            Contato novoContato = new Contato(OrganizationName, IsOffline);
            novoContato.Nome = nome;
            novoContato.Email1 = email;
            novoContato.Endereco1Estado = uf;
            novoContato.Endereco1Estadoid = new Lookup((new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Estado.ObterPor(uf).Id, uf, "itbc_estado");
            if (novoContato.Endereco1Estadoid != null)
            {
                novoContato.Endereco1Municipio = cidade;
                novoContato.Endereco1Municipioid = new Lookup((new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Municipio.ObterPor(novoContato.Endereco1Estadoid.Id, cidade).Id, cidade, "itbc_municipio");
            }
            novoContato.TelefoneComercial = telefone;
            novoContato.Id = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.Create(novoContato);
            return new Lookup(novoContato.Id, novoContato.Nome, "contact");
        }

        public bool ObterSenha()
        {
            string senha = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.ObterSenha(this.Id);
            bool retorno = false;

            if (!String.IsNullOrEmpty(senha))
            {
                EnviarEmailParaEsteContato("Solicitação de reenvio de Senha", string.Format("<b>Solicitação de reenvio de Senha</b><br><br><b>Senha</b>: {0}", senha));
                retorno = true;
            }
            else
                EnviarEmailParaEsteContato("Solicitação de reenvio de Senha", "<b>Solicitação de reenvio de Senha</b><br><br><b>Usuário não encontrado. Inscreva-se.</b>");

            return retorno;
        }

        private void EnviarEmailParaEsteContato(string assunto, string mensagem)
        {
            var email = new Intelbras.CRM2013.Domain.Model.Email(OrganizationName, IsOffline);
            email.Assunto = assunto;

            email.Mensagem = mensagem;
            email.Para = new Lookup[1];
            email.Para[0] = new Lookup { Id = this.Id, Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>() };

            email.Direcao = false;
            email.ID = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Email.Create(email);

            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Email.EnviarEmail(email.ID.Value);
        }

        public bool PodeIntegrarComERP()
        {
            //if (this.Id == Guid.Empty) return false;
            if (this.AssociadoA == null || this.AssociadoA.Id == Guid.Empty) return false;
            if (string.IsNullOrEmpty(this.Email1)) return false;
            if (string.IsNullOrEmpty(this.Nome) || this.Nome.Length < 3) return false;

            return this.Nome.Substring(0, 3).ToUpper().Equals("NFE");
        }

        [LogicalAttribute("itbc_integraintelbraspontua")]
        public Boolean? IntegraIntelbrasPontua { get; set; }


        #endregion

        /// <summary>
        /// O método ainda não retorna nenhuma informação
        /// </summary>
        /// <param name="AcessoPortais"></param>
        /// <param name="guid"></param>
        /// <param name="nomecompleto"></param>
        /// <param name="login"></param>
        /// <param name="email"></param>
        //public void GerenciarPermissoesSharePoint(Dictionary<string, bool> AcessoPortais, Guid guid, string login)
        //{
        //    foreach (KeyValuePair<string, bool> item in AcessoPortais)
        //    {
        //        var _urlPortal = TrideaConfigurationManager.GetSettingValue(item.Key);
        //        var _grupoSharePoint = TrideaConfigurationManager.GetSettingValue(string.Format("Grupo{0}", item.Key));
        //        var _msgRetorno = string.Empty;
        //        /// caso seja necessário a mensagem de retorno estará armazenado na variavel _msgRetorno;
        //        /// 
        //        //var result = (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).Contato.AdicionarOuRemoverUsuarioAoGrupoSP(login, _grupoSharePoint, _urlPortal, item.Value, out _msgRetorno);
        //        //var result = SharepointService.AdicionarOuRemoverUsuario(_grupoSharePoint, login, _urlPortal, item.Value);

        //    }
        //}
    }
}
