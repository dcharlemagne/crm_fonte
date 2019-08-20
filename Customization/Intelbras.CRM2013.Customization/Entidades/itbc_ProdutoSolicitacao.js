if (typeof (ProdutoSolicitacao) == "undefined") { ProdutoSolicitacao = {}; }

ProdutoSolicitacao = {

    SolicitacaoBeneficio: null,

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        String.prototype.capitalizeFirstLetter = function () {
            return this.charAt(0).toUpperCase() + this.slice(1);
        }

        ProdutoSolicitacao.CarregarSolicitacaoBeneficioRelacionada();

        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;

            case 1: //Create
                //controleUpdate para saber se é update ou não, é uma variavel que será lida pelo produtosdasolicitacao.js dentro de um iframe
                controleUpdate = false;
                break;

            case 2: //Update
                controleUpdate = true; //controleUpdate para saber se é update ou não, é uma variavel que será lida pelo produtosdasolicitacao.js dentro de um iframe
                //Xrm.Page.ui.refreshRibbon();
                //Util.funcao.formdisable(true);
                break;
        }
    },

    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    CalcularTotal: function () {
        var produto = ProdutoSolicitacao.ObterProduto();
    },

    MudarNome: function () {
        Xrm.Page.getAttribute("itbc_name").setValue(Xrm.Page.getAttribute("itbc_productid").getValue()[0].name);
    },

    ObterProduto: function () {
        var produtoid = Xrm.Page.getAttribute('itbc_productid').getValue()[0].id;
        if (produtoid == null || produtoid == '' || produtoid == undefined) return null;

        var retornoSucesso;
        var retornoErro;

        var produto = SDKore.OData.Retrieve(produtoid, "product");

        if (produto == null || produto == undefined || produto.length <= 0)
            return null;
        else
            return produto;
    },

    PriceProtectionRegras: function () {
        var beneficioDoPrograma = ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampoLookUp("itbc_beneficiodoprograma");
        var tipoPriceProtection = ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampo("itbc_TipodePriceProtection");

        if ((beneficioDoPrograma != null && tipoPriceProtection != null) &&
            (beneficioDoPrograma.name == "Price Protection" && tipoPriceProtection.Value == 993520000)) {
            Xrm.Page.getControl("itbc_quantidadeajustada").setVisible(true);
            Xrm.Page.getControl("itbc_faturaid").setVisible(false);
            Xrm.Page.getControl("itbc_valorunitario").setVisible(false);
            Xrm.Page.getControl("itbc_valorunitarioaprovado").setVisible(false);
            Xrm.Page.getControl("itbc_valorcancelado").setVisible(false);

            Xrm.Page.getControl('itbc_qtdsolicitado').setLabel('Quantidade em Estoque');
            Xrm.Page.getControl('itbc_qtdaprovada').setLabel('Quantidade Giro');
        }

    },

    AnaliseBeneficio: function () {//onChange Produto(lookup) - Quantidade(decimal) - NotaFiscal(lookup)
        if (controleUpdate) {
            var beneficioDoPrograma = ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampoLookUp("itbc_beneficiodoprograma");
            var canal = ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampoLookUp("itbc_accountid");
            var unidadeNegocio = ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampoLookUp("itbc_businessunitId");
            var quantidade = Xrm.Page.getAttribute("itbc_qtdsolicitado").getValue();

            var valorParametroGlobal = null;

            var solicitacaoDeBeneficio = Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue() ?
                Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue()[0] :
                    Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue();

            var produto = Xrm.Page.getAttribute("itbc_productid").getValue() ?
                Xrm.Page.getAttribute("itbc_productid").getValue()[0] :
                    Xrm.Page.getAttribute("itbc_productid").getValue();

            var notaFiscal = Xrm.Page.getAttribute("itbc_faturaid").getValue() ?
                Xrm.Page.getAttribute("itbc_faturaid").getValue()[0] :
                    Xrm.Page.getAttribute("itbc_faturaid").getValue();

            if (beneficioDoPrograma == null)
                return;

            if (produto == null)
                return;

            if (beneficioDoPrograma.name != "Stock Rotation") {
                if (quantidade == null)
                    return;
                // Willer refatorar
                var produtos = ProdutoSolicitacao.ObterPortifolioDeProdutos(canal.id);
                var isPortifolio = 0;

                /*$.each(produtos, function () {
                    if (this == produto.id)
                        isPortifolio++;
                });
                produtos.forEach(function (produto) {
                    if (this == produto.id)
                        isPortifolio++;
                });
    
                if (isPortifolio == 0) {
                    alert("(CRM) Produto informado não pertence ao portifólio do cliente.");
                    Xrm.Page.getAttribute("itbc_productid").setValue(null);
                    return;
                }*/
                // Willer refatorar

                var valorUnitario = ProdutoSolicitacao.ObterValorDoProdutoPelaListaDePreco(solicitacaoDeBeneficio.id, produto.id, canal.id);

                if (valorUnitario == "0") {
                    alert("(CRM) Não foi possível calcular o preço do produto " + produto.name + ". Verifique política comercial.");
                    return;
                }

                if (beneficioDoPrograma.name == "Showroom") {
                    valorParametroGlobal = ProdutoSolicitacao.ObterValorDoProdutoDoParametroGlobal(solicitacaoDeBeneficio.id, canal.id, unidadeNegocio.id);
                }
                else if (beneficioDoPrograma.name == "Backup") {
                    valorParametroGlobal = ProdutoSolicitacao.ObterValorDoProdutoDoParametroGlobal(solicitacaoDeBeneficio.id, canal.id, unidadeNegocio.id);
                }
                valorUnitario = valorUnitario.replace(".", "");
                valorUnitario = valorUnitario.replace(",", ".");
                valorUnitario = parseFloat(valorUnitario) - (parseFloat(valorUnitario) * parseFloat(valorParametroGlobal / 100));
                var valorTotal = valorUnitario * quantidade;

                Xrm.Page.getAttribute("itbc_valorunitario").setValue(valorUnitario);
                Xrm.Page.getAttribute("itbc_valortotal").setValue(valorTotal);
                Xrm.Page.ui.controls.get("itbc_valorunitarioaprovado").setDisabled(false);
                Xrm.Page.getAttribute("itbc_valortotalaprovado").setValue(valorTotal);
                Xrm.Page.getAttribute("itbc_valorunitarioaprovado").setValue(valorUnitario);
                Xrm.Page.data.entity.save();
                Xrm.Page.ui.controls.get("itbc_valorunitarioaprovado").setDisabled(true);

            }
            else {
                if (Xrm.Page.getAttribute("itbc_valorunitario").getValue())
                    return;

                var notaFiscal = Xrm.Page.getAttribute("itbc_faturaid").getValue() ?
                        Xrm.Page.getAttribute("itbc_faturaid").getValue()[0] :
                            Xrm.Page.getAttribute("itbc_faturaid").getValue();

                if (produto == null || quantidade == null || notaFiscal == null)
                    return;

                var valorUnitarioNotafiscal = ProdutoSolicitacao.ObterValorUnitarioDoProdutoDaFatura(produto.id, notaFiscal.id);
                var valorTotal = parseFloat(valorUnitarioNotafiscal) * quantidade;

                Xrm.Page.getAttribute("itbc_valorunitario").setValue(valorUnitarioNotafiscal);
                Xrm.Page.getAttribute("itbc_valortotal").setValue(valorTotal);
                Xrm.Page.getControl("itbc_valorunitarioaprovado").setValue(valorUnitario);
            }
        }
    },

    CaseShowroom: function (produto) {
        if (produto != null) {
            var productEntity = SDKore.OData.Retrieve(produto.id, produto.entityType.capitalizeFirstLetter(), null, function () { throw new Error("Não foi possível se conectar ao OData"); });
            var isShowRoom = productEntity.itbc_ShowRoom;

            if (!isShowRoom) {
                alert("(CRM) Produto informado não pode ser adquirido para Show Room.");
                Xrm.Page.getAttribute("itbc_productid").setValue(null);
                return false;
            }
        }
    },

    ObterValorUnitarioDoProdutoDaFatura: function (produtoId, notaFiscalId) {
        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ObterProdutoDaFatura");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("notaFiscalId", notaFiscalId);
        Util.funcao.SetParameter("produtoId", produtoId);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            resultado = $.parseJSON(retorno['ReturnValue'])
            if (resultado["Sucesso"] == false) {
                alert(resultado["Mensagem"]);
                throw new Error(resultado["Mensagem"]);
            }
            return resultado["ValorLiquido"];
        }
        else {
            Xrm.Utility.alertDialog(resultado.Mensagem);
            Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica. Não foi possível carregar o produto da fatura.', 'ERROR', 'showroomErroMsg');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }
    },

    ObterValorDoProdutoPelaListaDePreco: function (solicitacaoBeneficioId, produtoId, canalId, produtoSolicitacaoId) {
        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ObterValorDoProdutoPelaListaDePreco");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("produtoSolicitacaoId", produtoSolicitacaoId);
        Util.funcao.SetParameter("solicitacaoBeneficioID", solicitacaoBeneficioId);
        Util.funcao.SetParameter("produtoID", produtoID);
        Util.funcao.SetParameter("canalId", canalId);
        Util.funcao.SetParameter("classificacaoId", null);
        Util.funcao.SetParameter("unidadeNegocioId", null);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            resultado = $.parseJSON(retorno['ReturnValue'])
            if (resultado["Sucesso"] == false) {
                alert(resultado["Mensagem"]);
                throw new Error(resultado["Mensagem"]);
            }
            return resultado["ValorLiquido"];
        }
        else {
            Xrm.Utility.alertDialog(resultado.Mensagem);
            Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica. Não foi possível carregar o valor do produto pela lista de preço', 'ERROR', 'showroomErroMsg');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }
    },

    ObterValorDoProdutoDoParametroGlobal: function (solicitacaoBeneficioID, canalId, unidadeNegocioId) {
        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ObterValorParametroGlobalValidado");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("solicitacaoBeneficioID", solicitacaoBeneficioID);
        Util.funcao.SetParameter("canalId", canalId);
        Util.funcao.SetParameter("unidadeNegocioId", unidadeNegocioId);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            resultado = $.parseJSON(retorno['ReturnValue'])
            if (resultado["Sucesso"] == false) {
                alert(resultado["Mensagem"]);
                throw new Error(resultado["Mensagem"]);
            }

            return resultado["ValorParametroGlobal"];
        }
        else {
            Xrm.Utility.alertDialog(resultado.Mensagem);
            Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica. Não foi possível carregar o valor do parâmetro global', 'ERROR', 'showroomErroMsg');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }
    },

    ObterSolicitacaoBeneficio: function () {
        var solicitacaoBeneficio = Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue() ?
            Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue()[0] :
                Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue();

        if (solicitacaoBeneficio == null) {
            alert("Selecione uma solicitação de benefício.");
            Xrm.Page.getControl("itbc_solicitacaodebeneficioid").setFocus();
            return null;
        }

        var beneficioEntity = SDKore.OData.Retrieve(solicitacaoBeneficio.id, solicitacaoBeneficio.entityType, null, function () { throw new Error("Não foi possível se conectar ao OData"); });

        if (beneficioEntity != null)
            beneficioEntity.isEntity = true;

        return beneficioEntity;
    },

    CarregarSolicitacaoBeneficioRelacionada: function () {
        ProdutoSolicitacao.SolicitacaoBeneficio = window.top.opener == null ?
            ProdutoSolicitacao.ObterSolicitacaoBeneficio() :
                window.top.opener;

        ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampoLookUp = function (campo) {
            if (ProdutoSolicitacao.SolicitacaoBeneficio.isEntity) {
                var retorno = new Object();

                retorno.id = ProdutoSolicitacao.SolicitacaoBeneficio[campo].Id;
                retorno.name = ProdutoSolicitacao.SolicitacaoBeneficio[campo].Name;
                retorno.entityType = ProdutoSolicitacao.SolicitacaoBeneficio[campo].LogicalName;

                return retorno;
            }
            else {
                return ProdutoSolicitacao.SolicitacaoBeneficio.Xrm.Page.getAttribute(campo.toLowerCase()).getValue()[0];
            }
        }

        ProdutoSolicitacao.SolicitacaoBeneficio.ObterValorCampo = function (campo) {
            return ProdutoSolicitacao.SolicitacaoBeneficio.isEntity ?
                  ProdutoSolicitacao.SolicitacaoBeneficio[campo] :
                      ProdutoSolicitacao.SolicitacaoBeneficio.Xrm.Page.getAttribute(campo.toLowerCase()).getValue();
        }
    },

    ObterPortifolioDeProdutos: function (canalId) {
        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ObterProdutosPortfolio");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("canalGuid", canalGuid);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            resultado = $.parseJSON(retorno['ReturnValue'])
            if (resultado["Sucesso"] == false) {
                alert(resultado["Mensagem"]);
                throw new Error(resultado["Mensagem"]);
            }
            return resultado["Produtos"];
        }
        else {
            Xrm.Utility.alertDialog(resultado.Mensagem);
            Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica. Não foi possível carregar a lista de Produtos Showroom do Canal', 'ERROR', 'showroomErroMsg');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }
    },

    FiltrarFaturaPorClienteSolicitacao: function () {
        try {
            if (Util.Xrm.HasValue("itbc_solicitacaodebeneficioid")) {

                var solicitacaoId = Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue()[0].id;

                SDKore.OData.configurarParametros("$select=itbc_accountid");
                var canalId = SDKore.OData.Retrieve(solicitacaoId, "itbc_solicitacaodebeneficio").itbc_accountid.Id;

                var fetchQuery = "<filter type=\"and\">";
                fetchQuery += "<condition attribute=\"customerid\" operator=\"eq\" uitype=\"account\" value=\"{" + canalId + "}\" />";
                fetchQuery += "<condition attribute=\"statecode\" operator=\"in\">";
                fetchQuery += "<value>0</value>";
                fetchQuery += "<value>1</value>";
                fetchQuery += "<value>2</value>";
                fetchQuery += "</condition>";
                fetchQuery += "</filter>";

                Xrm.Page.getControl("itbc_faturaid").addPreSearch(function () {
                    Xrm.Page.getControl("itbc_faturaid").addCustomFilter(fetchQuery);
                });
            }
        } catch (e) {
            Xrm.Page.ui.setFormNotification("Não foi possível filtrar as notas fiscais do cliente!", "WARNING", "itbc_faturaid");
        }
    },

    FiltrarProdutoPorFatura: function () {
        try {
            if (Util.Xrm.HasValue("itbc_faturaid")) {

                var faturaId = Xrm.Page.getAttribute("itbc_faturaid").getValue()[0].id;

                var fetchQuery = "<filter type=\"and\">";
                fetchQuery += "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";
                fetchQuery += "</filter>";
                fetchQuery += "<link-entity name=\"invoicedetail\" from=\"productid\" to=\"productid\" alias=\"ac\">";
                fetchQuery += "<filter type=\"and\">";
                fetchQuery += "<condition attribute=\"invoiceid\" operator=\"eq\" value=\"" + faturaId + "\" />";
                fetchQuery += "</filter>";
                fetchQuery += "</link-entity>";

                Xrm.Page.getControl("itbc_productid").addPreSearch(function () {
                    Xrm.Page.getControl("itbc_productid").addCustomFilter(fetchQuery);
                });
            } else {
                if (Util.Xrm.HasValue("itbc_productid")) {
                    Xrm.Page.getAttribute("itbc_productid").setValue(null);
                }

                Xrm.Page.getControl("itbc_productid").removePreSearch();
            }
        } catch (e) {
            Xrm.Page.ui.setFormNotification("Não foi possível filtrar os produtos da nota fiscas!", "WARNING", "itbc_productid");
        }
    },

    FiltrarProdutoPorTipoSolicitacao: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_benefciodoprogramaid")) return;
            if (!Util.Xrm.HasValue("itbc_solicitacaodebeneficioid")) return;

            SDKore.OData.configurarParametros("$select=itbc_Codigo");
            var beneficio = Xrm.Page.getAttribute("itbc_benefciodoprogramaid").getValue()[0].id;
            var codigoBeneficioPrograma = SDKore.OData.Retrieve(beneficio, "itbc_beneficio").itbc_Codigo;

            SDKore.OData.configurarParametros("$select=itbc_businessunitId");
            var solicitacaoBeneficioId = Xrm.Page.getAttribute("itbc_solicitacaodebeneficioid").getValue()[0].id;
            var unidadeDeNegocio = SDKore.OData.Retrieve(solicitacaoBeneficioId, "itbc_solicitacaodebeneficio").itbc_businessunitId;



            var fetch = "<filter type=\"and\">";

            if (codigoBeneficioPrograma == Config.Entidade.BeneficioPrograma.PriceProtection.code) {
                SDKore.OData.configurarParametros("$select=itbc_TipodePriceProtection");
                var tipoPrice = SDKore.OData.Retrieve(solicitacaoBeneficioId, "itbc_solicitacaodebeneficio").itbc_TipodePriceProtection;
                if (tipoPrice.Value == Config.Entidade.SolicitacaoBeneficio.TipoPriceProtection.Consumo) {
                    fetch += "<condition attribute=\"itbc_permitiremsolbenef\" operator=\"eq\" value=\"1\" />";
                }
            }
            else {
                fetch += "<condition attribute=\"itbc_permitiremsolbenef\" operator=\"eq\" value=\"1\" />";
            }
            SDKore.OData.configurarParametros("$select=itbc_accountid");
            var canalId = SDKore.OData.Retrieve(solicitacaoBeneficioId, "itbc_solicitacaodebeneficio").itbc_accountid.Id;
            SDKore.OData.configurarParametros("$select=itbc_ClassificacaoId");
            var classificacao = SDKore.OData.Retrieve(canalId, "Account").itbc_ClassificacaoId;

            if (classificacao.Name != "Revendas") {
                fetch += "<condition attribute=\"itbc_businessunitid\" operator=\"eq\" uitype=\"businessunit\" value=\"{" + unidadeDeNegocio.Id + "}\" />";
            }

            if (codigoBeneficioPrograma == Config.Entidade.BeneficioPrograma.ShowRoom.code) {
                if (classificacao.Name == "Revendas") {
                    fetch += "<condition attribute=\"itbc_showroomrevenda\" operator=\"eq\" value=\"1\" />";
                } else if (classificacao.Name.match("Distribuidor")) {
                    fetch += "<condition attribute=\"itbc_showroom\" operator=\"eq\" value=\"1\" />";
                }

            } else if (codigoBeneficioPrograma == Config.Entidade.BeneficioPrograma.Backup.code) {
                if (classificacao.Name == "Revendas") {
                    fetch += "<condition attribute=\"itbc_backuprevenda\" operator=\"eq\" value=\"1\" />";
                } else if (classificacao.Name.match("Distribuidor")) {
                    fetch += "<condition attribute=\"itbc_backupdistribuidor\" operator=\"eq\" value=\"1\" />";
                }
            }


            fetch += "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";
            fetch += "</filter>";
            if (classificacao.Name != "Revendas") {
                fetch += "<link-entity name=\"itbc_proddoport\" from=\"itbc_productid\" to=\"productid\" alias=\"ag\">";
                fetch += "<filter type=\"and\">";
                fetch += "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";
                fetch += "</filter>";
                fetch += "<link-entity name=\"itbc_portfolio\" from=\"itbc_portfolioid\" to=\"itbc_portfolioid\" alias=\"ah\">";
                fetch += "<filter type=\"and\">";
                fetch += "<condition attribute=\"statecode\" operator=\"eq\" value=\"0\" />";
                fetch += "</filter>";
                fetch += "</link-entity>";
                fetch += "</link-entity>";
            }
            Xrm.Page.getControl("itbc_productid").addPreSearch(function () {
                Xrm.Page.getControl("itbc_productid").addCustomFilter(fetch);
            });
        } catch (e) {
            Xrm.Page.ui.setFormNotification('Não foi possível filtrar os produtos na solicitação, você pode continuar mesmo assim!', 'WARNING', 'FiltrarProdutoPorTipoSolicitacao');
        }
    },

    IntegraNoPlugin: function () {

        var integra = !(Xrm.Page.getAttribute("itbc_qtdsolicitado").getIsDirty() || Xrm.Page.getAttribute("itbc_productid").getIsDirty());
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(integra);
    }
}