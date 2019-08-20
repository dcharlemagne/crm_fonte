if (typeof (Conta) == "undefined") { Conta = {}; }

Conta = {
    /**********************************
        Variáveis Globais.
        **********************************/

    podeBuscarOCep : true,
    VENDAS_SERVICE_URL : "/ISV/Intelbras/WebServices/Vendas/",
    SERVICE_NAME : "IsolService",
    tmp_rua : null,
    tmp_bairro : null,
    tmp_cidade : null,
    tmp_uf : null,
    tmp_cep : null,
    tmp_pais : null,

    /**********************************
        Descrição: Pegar Valor WebConfig
        **********************************/

    PegarValorWebConfig: function(valor_chave) {

        if (valor_chave == "")
            return "";

        var comando = new RemoteCommand(SERVICE_NAME, "PegarValorWebConfig", VENDAS_SERVICE_URL);
        comando.SetParameter("valor_chave", valor_chave);


        var retorno = comando.Execute();

        return retorno.ReturnValue;
    },

    OnLoad: function () {
        
        with (crmForm.all) {
            CampoMatriz = parentaccountid;
        }

        /**********************************
        Ações OnLoad
        **********************************/
        //crmForm.verificacaoDoCampoGerarPedido();

        if (document.getElementById('_MBcrmFormSaveAndClose') != null) {
            document.getElementById('_MBcrmFormSaveAndClose').style.display = 'none';
        }

        crmForm.all.new_mensagem_c.parentElement.parentElement.style.display = 'none';

        AutorizarUsuario(userid, crmForm.all.new_grupo_clienteid);
        crmForm.validaçãoDaNatureza();

        switch (Xrm.Page.ui.getFormType()) {

            case 1: // Create Form
                Xrm.Page.getAttribute("new_altera_endereco_padrao").setValue("c");
                break;

            case 2: // Update Form
                Xrm.Page.getAttribute("new_altera_endereco_padrao").setValue("n");
                CampoMatriz.Disabled = true;

                DesabilitarCamposQuandoEMSRetornarCodigoDoCliente();
                crmForm.StatusDoCadastroOnChange();
                Xrm.Page.ui.setFormNotification(Xrm.Page.getAttribute("new_mensagem").getValue(), "INFORMATION");
                crmForm.InscricaoEstadual();
                InicializarMonitoramentoDaAlteracaoDoEndereco();

                if (crmForm.ParticipaProgramaCanais()) {
                    alert("Esse cliente faz parte do programa de Canais, a alteração de algumas informações devem ser feitas no CRM 2013!");
                    ValidacoesProgramaCanais();
                }
                break;

            case 6: // Bulk Edit Form
                ValidacoesProgramaCanais();
                break;
        }

        /***********
        BUSCAR INFORMAÇÕES DE CONTEXTO DA PÁGINA
        -- SERVE PARA BUSCAR AS INFORMAÇÕES DE USUÁRIO
        ************/
  
        var xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
        xmlhttp.open("POST", Xrm.Page.context.getServerUrl() + "/mscrmservices/2007/crmservice.asmx", false);
        xmlhttp.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        xmlhttp.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2007/WebServices/Execute");
        //http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute

        var soapBody = "<soap:Body>" +
            "<Execute xmlns='http://schemas.microsoft.com/crm/2007/WebServices'>" +
            "<Request xsi:type='WhoAmIRequest' />" +
            "</Execute></soap:Body>";

        var soapXml = "<soap:Envelope " +
            "xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' " +
            "xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' " +
            "xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";

        soapXml += Xrm.Page.context.getAuthenticationHeader();
        soapXml += soapBody;
        soapXml += "</soap:Envelope>";

        xmlhttp.send(soapXml);
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = false;
        xmlDoc.loadXML(xmlhttp.responseXML.xml);

        /// VARIÁVEIS GLOBAIS PARA OS DADOS DE USUARIO
        var userid = xmlDoc.getElementsByTagName("UserId")[0].childNodes[0].nodeValue;
        var businessUnitid = xmlDoc.getElementsByTagName("BusinessUnitId")[0].childNodes[0].nodeValue;
        var organizationid = xmlDoc.getElementsByTagName("OrganizationId")[0].childNodes[0].nodeValue;

        crmForm.disableVerticalMenuItem = function (navBar, menuItem) {
            menuItem = menuItem.toLowerCase().replace(/^\s+|\s+$/g, '');
            il = document.getElementById(navBar).getElementsByTagName('li');

            for (i = 0; i < il.length; i++) {
                liItem = il[i].innerText.toLowerCase().replace(/^\s+|\s+$/g, '');

                if (liItem == menuItem) {
                    anchor = il[i].getElementsByTagName('a')[0];
                    anchor.parentNode.removeChild(anchor);
                }
            }
        }
    },

    InscricaoEstadual: function () {

        if (Xrm.Page.getAttribute("new_natureza").getValue() != 2) return;

        var inscricaoEstadual = Xrm.Page.getAttribute("new_inscricaoestadual").getValue();

        if (Xrm.Page.getAttribute("new_contribuinte_icms").getValue() == 1) {

            if (inscricaoEstadual == null || inscricaoEstadual == "ISENTO")
                alert("Favor Informar Inscrição Estadual");

        } else {

            if (inscricaoEstadual != "ISENTO" || inscricaoEstadual == null)
                if (confirm("ATENÇÃO: CLIENTE NÃO CONTRIBUINTE, DESEJA ALTERAR A INSCRIÇÃO ESTADUAL PARA ISENTO?")) {
                    Xrm.Page.getAttribute("new_inscricaoestadual").setValue("ISENTO");
                }
        }

    },

    /**********************************
    Descrição: Desabilitar alguns campos quando o ems retornar o codigo do cliente
    **********************************/
    DesabilitarCamposQuandoEMSRetornarCodigoDoCliente: function () {

        if (Xrm.Page.getAttribute("accountnumber").getValue() != null) {
            crmForm.all.accountnumber.disabled = true;
            crmForm.all.new_nome_abreviado_erp.disabled = true;
            Xrm.Page.getControl("new_data_limite_credito").setDisabled(true);
            crmForm.all.creditlimit.disabled = true;
            Xrm.Page.getControl("new_data_implantacao").setDisabled(true);
            crmForm.all.new_identificacao.disabled = true;
            crmForm.all.new_natureza.disabled = true;
            crmForm.all.new_banco.disabled = true;
            crmForm.all.new_agencia.disabled = true;
            Xrm.Page.getControl("new_portadorid").setDisabled(true);
            crmForm.all.new_modalidade.disabled = true;
            crmForm.all.new_gera_aviso_credito.disabled = true;
            crmForm.all.new_recebe_informacao_sci.disabled = true;
            crmForm.all.new_emite_bloqueto.disabled = true;
            crmForm.all.new_calcula_multa.disabled = true;
            Xrm.Page.getControl("new_condicao_pagamentoid").setDisabled(true);
            crmForm.all.new_saldo_credito.disabled = true;
            crmForm.all.new_contacorrente.disabled = true;
            crmForm.all.new_cnpj.disabled = true;
            crmForm.all.new_cpf.disabled = true;
        }
    },

    phoneFormat: function (campo) {

        if (campo.getValue() == null) 
            return;

        var phone = filtraNumeros(campo.getValue());

        if (phone.length < 10) { alert('Atenção! Informe o Número com o DDD.'); campo.setValue(null); return; };

        switch (phone.length) {
            case 10:
                phone = '(' + phone.substr(0, 2) + ') ' + phone.substr(2, 4) + '-' + phone.substr(6);
                break;

            case 11:
                phone = '(' + phone.substr(0, 2) + ') ' + phone.substr(2, 5) + '-' + phone.substr(7);
                break;
        }

        campo.setValue(phone);
    },

    filtraNumeros: function(numero) {
        numero += ""; // Converte qualquer valor de "var numero" em String
        var numValidado = ""; // numero validado
        var temp; // Cria var temporaria

        for (var i = 0; i < numero.length; i++) { // Laço que percorre todos os caractere de "var numero"
            temp = numero.charAt(i); // Caractere atual
            if ((temp >= "0") && (temp <= "9"))
                numValidado += temp; // Checa cada caracter; se for nº adiciona em "var numValidado"
        }

        return numValidado; // Retorno somente números
    },

    FormatarCep: function(cep) {

        var c = cep.getValue();
        var cepFormatado = "";

        if (c.match("-") == null)
            cepFormatado = c.substr(0, 5) + "-" + c.substr(5, 3);
        else
            cepFormatado = c;

        if (cepFormatado.length != 9) {
            alert("Formato do CEP é inválido.");
            cep.focus();
            return null;
        }

        return cepFormatado;
    },

    /********************************** 
    Descrição: Pesquisa endereco Principal por CEP
    **********************************/
    PesquisarEnderecoPrincipalPor: function(cep) {

        Xrm.Page.getAttribute("address1_line1").setValue(null);
        Xrm.Page.getAttribute("address1_line3").setValue(null);
        Xrm.Page.getAttribute("address1_city").setValue(null);
        Xrm.Page.getAttribute("address1_stateorprovince").setValue(null);
        Xrm.Page.getAttribute("address1_country").setValue(null);

        if (cep != null) {
            var endereco = crmForm.PesquisarEnderecoPor(cep);

            if (null != endereco && endereco.Achou) {

                Xrm.Page.getAttribute("address1_line1").setValue(endereco.Logradouro);
                Xrm.Page.getAttribute("address1_line3").setValue(endereco.Bairro);
                Xrm.Page.getAttribute("address1_city").setValue(endereco.Cidade);
                Xrm.Page.getAttribute("address1_stateorprovince").setValue(endereco.UF);
                Xrm.Page.getAttribute("address1_country").setValue("BRASIL");

                Xrm.Page.getAttribute("new_vendas_alc").setValue(((endereco.ZonaFranca) ? null : true));

                var aceitouCopiarEndereco = confirm("Deseja copiar este endereço para cobrança também?");

                if (aceitouCopiarEndereco) {
                    Xrm.Page.getAttribute("address2_postalcode").setValue(endereco.Cep);
                    Xrm.Page.getAttribute("address2_line1").setValue(endereco.Logradouro);
                    Xrm.Page.getAttribute("address2_line3").setValue(endereco.Bairro);
                    Xrm.Page.getAttribute("address2_city").setValue(endereco.Cidade);
                    Xrm.Page.getAttribute("address2_stateorprovince").setValue(endereco.UF);
                    Xrm.Page.getAttribute("address2_country").setValue("BRASIL");
                }
            }
            else {
                alert("CEP não encontrado.");
            }
        }
    },

    /**********************************
    Data:      
    Autor:     
    Descrição: Pesquisa endereco de cobrança pelo cep
    **********************************/
    PesquisarEnderecoDeCobrancaPor: function(cep) {

        Xrm.Page.getAttribute("address2_line1").setValue(null);
        Xrm.Page.getAttribute("address2_line3").setValue(null);
        Xrm.Page.getAttribute("address2_city").setValue(null);
        Xrm.Page.getAttribute("address2_stateorprovince").setValue(null);
        Xrm.Page.getAttribute("address2_country").setValue(null);

        if (cep != null) {
            var endereco = crmForm.PesquisarEnderecoPor(cep);

            if (null != endereco && endereco.Achou) {
                Xrm.Page.getAttribute("address2_postalcode").setValue(endereco.Cep);
                Xrm.Page.getAttribute("address2_line1").setValue(endereco.Logradouro);
                Xrm.Page.getAttribute("address2_line3").setValue(endereco.Bairro);
                Xrm.Page.getAttribute("address2_city").setValue(endereco.Cidade);
                Xrm.Page.getAttribute("address2_stateorprovince").setValue(endereco.UF);
                Xrm.Page.getAttribute("address2_country").setValue("BRASIL");
            }
            else {
                alert("CEP não encontrado.");
            }
        }
    },

    /**********************************
    Data:      
    Autor:     
    Descrição: Pesquisa o cliente pelo nome do mesmo
    **********************************/
    PesquisarClientePorNome: function(nomecliente) {

        if (null == nomecliente)
            return;

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarClientePorNome", VENDAS_SERVICE_URL);
        comando.SetParameter("nomecliente", nomecliente);
        comando.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        return resultado;
    },


    VerificaPermissao: function(usuarioGuid, grupoGuid) {

        if (null == usuarioGuid)
            return;
        if (null == grupoGuid)
            return;

        var resultado = null;

        // Clausio - 09/11/2010
        var comando = new RemoteCommand(SERVICE_NAME, "VerificaPermissaoUsuarioGrupoCliente", VENDAS_SERVICE_URL);
        comando.SetParameter("guidUsuario", usuarioGuid);
        comando.SetParameter("guidGrupoCliente", grupoGuid);
        comando.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        return resultado;
    },

    esquisarClientePor: function(codigo) {

        if (null == codigo) return;

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarClientePorCodigo", VENDAS_SERVICE_URL);
        comando.SetParameter("codigo", codigo);
        comando.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }
        alert('resultado: ' + resultado);
        return resultado;
    },


    PesquisarClientePor: function(documento, natureza) {

        if (null == documento || null == natureza)
            return;

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarClientePorDocumento", VENDAS_SERVICE_URL);
        comando.SetParameter("documento", documento);
        comando.SetParameter("natureza", natureza);
        comando.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        return resultado;
    },

    PesquisarEnderecoPor: function(cep) {

        if (null == cep)
            return;

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarEnderecoDoClientePorCep", VENDAS_SERVICE_URL);
        comando.SetParameter("cep", cep);

        var execucao = comando.Execute();

        if (execucao.Success) {

            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return;
            }

        }

        return resultado;
    },

    ValidarCnpj: function(cnpj) {
        var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
        digitos_iguais = 1;
        if (cnpj.length < 14 && cnpj.length < 15)
            return false;
        for (i = 0; i < cnpj.length - 1; i++)
            if (cnpj.charAt(i) != cnpj.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }
        if (!digitos_iguais) {
            tamanho = cnpj.length - 2
            numeros = cnpj.substring(0, tamanho);
            digitos = cnpj.substring(tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0))
                return false;
            tamanho = tamanho + 1;
            numeros = cnpj.substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;
            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1))
                return false;
            return true;
        }
        else
            return false;
    },

    FormatarCnpj: function(campo) {

        var exp = /\-|\.|\/|\(|\)| /g
        var cnpjFormatado = campo.getValue();

        if (cnpjFormatado == null)
            return;

        /*Somente números*/
        var cnpjFormatado = cnpjFormatado.replace(exp, "");

        var cnpjEValido = crmForm.ValidarCnpj(cnpjFormatado);

        if (cnpjEValido) {
            cnpjFormatado = cnpjFormatado.replace(exp, "");
            cnpjFormatado = cnpjFormatado.substr(0, 2) + '.' + cnpjFormatado.substr(2, 3) + '.' + cnpjFormatado.substr(5, 3) + '/' + cnpjFormatado.substr(8, 4) + '-' + cnpjFormatado.substr(12, 2);
        }
        else {
            alert("CNPJ Inválido.");
            campo.focus();
            return;
        }

        return cnpjFormatado;
    },

    ValidarCpf: function(cpf) {
        var numeros, digitos, soma, i, resultado, digitos_iguais;
        digitos_iguais = 1;
        if (cpf.length < 11)
            return false;
        for (i = 0; i < cpf.length - 1; i++)
            if (cpf.charAt(i) != cpf.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }
        if (!digitos_iguais) {
            numeros = cpf.substring(0, 9);
            digitos = cpf.substring(9);
            soma = 0;
            for (i = 10; i > 1; i--)
                soma += numeros.charAt(10 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0))
                return false;
            numeros = cpf.substring(0, 10);
            soma = 0;
            for (i = 11; i > 1; i--)
                soma += numeros.charAt(11 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1))
                return false;
            return true;
        }
        else
            return false;
    },

    FormatarCpf: function(campo) {
        var exp = /\-|\.|\/|\(|\)| /g

        var cpfFormatado = campo.getValue();
        if (cpfFormatado == null)
            return;

        cpfFormatado = cpfFormatado.replace(exp, "");

        var cpfEValido = crmForm.ValidarCpf(cpfFormatado);

        if (cpfEValido) {
            cpfFormatado = cpfFormatado.substr(0, 3) + '.' + cpfFormatado.substr(3, 3) + '.' + cpfFormatado.substr(6, 3) + '-' + cpfFormatado.substr(9, 2);
        } else {
            alert("Número de CPF inválido.");
            campo.SetFocus();
            return;
        }

        return cpfFormatado;
    },

    FormatarDocumentoPorNaturezaDoCliente: function(natureza) {

        var documentoFormatado;

        // Verifica qual item foi selecionado
        switch (parseInt(natureza)) {
            case 1:
                documentoFormatado = crmForm.FormatarCpf(crmForm.all.new_cpf);
                break;
            case 2:
            case 4:
                documentoFormatado = crmForm.FormatarCnpj(crmForm.all.new_cnpj);
                break;
        }

        return documentoFormatado;
    },

    EmEstadoDeEdicao: function () {
        return Xrm.Page.ui.getFormType() == 2;
    },

    EmEstadoDeCriacao: function () {
        return Xrm.Page.ui.getFormType() == 1;
    },

    VerificaMatrizNoCrm: function(clienteMatriz) {
        var codMatriz = clienteMatriz.CodigoDaMatriz;
        //var cliente = crmForm.PesquisarClientePor(codMatriz);

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarClientePorCodigo", VENDAS_SERVICE_URL);
        comando.SetParameter("codigo", codMatriz);
        comando.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();
        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        if (resultado != null) {
            if (resultado.JaEstaNoCrm) {
                return CreateLookup(resultado.Id, resultado.Nome, "account");
            }
            else {
                alert('A Matriz não foi previamente cadastrada como Cliente. Codigo da Matriz: ' + clienteMatriz.CodigoDaMatriz);
                return null;
            }
        }
        alert('A Matriz não foi previamente cadastrada como Cliente.. Codigo da Matriz: ' + clienteMatriz.CodigoDaMatriz);
        return null;
    },

    ValidarExistenciaDoClientePor: function(documento, natureza) {

        if (null == documento.getValue() || null == natureza.getValue())
            return;

        var cliente = crmForm.PesquisarClientePor(documento.getValue(), natureza.getValue());

        if (null != cliente) {
            if (cliente.JaEstaNoCrm) {
                if (crmForm.EmEstadoDeCriacao()) {
                    documento.setValue(null);
                    natureza.setValue(null);

                    alert("Cliente já cadastrado com este cnpj/cpf.");
                    window.open("/sfa/accts/edit.aspx?id=" + cliente.Id, "account", "height=600,width=800,status=yes,resizable=yes");
                    window.close();
                }
                else {
                    var id1 = cliente.Id;
                    var id2 = Xrm.Page.data.entity.getId().toLowerCase().replace("{", "").replace("}", "");

                    if (crmForm.EmEstadoDeEdicao() && id1 != id2) {
                        alert("Cliente já cadastrado com este cnpj/cpf.");
                        window.open("/sfa/accts/edit.aspx?id=" + cliente.Id, "account", "height=600,width=800,status=yes,resizable=yes");
                        window.close();
                    }
                }
            }
            else {

                if (cliente.CodigoMatriz > 0) {
                    if (cliente.Cnpj != undefined || cliente.Cpf != undefined) {
                        alert("Cliente já cadastrado com este cnpj/cpf no EMS.");
                        if (confirm("Deseja recuperar os dados deste fornecedor?")) { // reescrever esta mensagem
                            // Recurso para não disparar a pesquisa pelo CEP retornado na pesquisa.
                            podeBuscarOCep = false;

                            if (typeof (cliente.Pais) != 'object')
                                Xrm.Page.getAttribute("address1_country").setValue(cliente.Pais);

                            if (typeof (cliente.Bairro) != 'object')
                                Xrm.Page.getAttribute("address1_line3").setValue(cliente.Bairro);

                            if (typeof (cliente.Cep) != 'object')
                                Xrm.Page.getAttribute("address1_postalcode").setValue(cliente.Cep.toString());

                            if (typeof (cliente.Cidade) != 'object')
                                Xrm.Page.getAttribute("address1_city").setValue(cliente.Cidade);

                            if (typeof (cliente.Logradouro) != 'object')
                                Xrm.Page.getAttribute("address1_line1").setValue(cliente.Logradouro);

                            if (typeof (cliente.TipoDoEndereco) != 'object')
                                Xrm.Page.getAttribute("address1_addresstypecode").setValue(cliente.TipoDoEndereco);

                            if (typeof (cliente.Uf) != 'object')
                                Xrm.Page.getAttribute("address1_stateorprovince").setValue(cliente.Uf);

                            if (typeof (cliente.PaisCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_country").setValue(cliente.PaisCobranca);

                            if (typeof (cliente.BairroCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_line3").setValue(cliente.BairroCobranca);

                            if (typeof (cliente.CepCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_postalcode").setValue(cliente.CepCobranca.toString());

                            if (typeof (cliente.CidadeCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_city").setValue(cliente.CidadeCobranca);

                            if (typeof (cliente.LogradouroCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_line1").setValue(cliente.LogradouroCobranca);

                            if (typeof (cliente.TipoDoEnderecoDeCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_addresstypecode").setValue(cliente.TipoDoEnderecoDeCobranca);

                            if (typeof (cliente.UfCobranca) != 'object')
                                Xrm.Page.getAttribute("address2_stateorprovince").setValue(cliente.UfCobranca);

                            if (typeof (cliente.Nome) != 'object')
                                Xrm.Page.getAttribute("name").setValue(cliente.Nome);

                            //if (typeof (cliente.NomeDaMatriz) != 'object')
                            //Xrm.Page.getAttribute("parentaccountid").setValue(CreateLookup(cliente.MatrizID, cliente.NomeDaMatriz, "account"));

                            if (typeof (cliente.CodigoDaMatriz) != 'object') {
                                if (cliente.CodigoDaMatriz != 0) {
                                    //alert('to aqui antes');
                                    Xrm.Page.getAttribute("parentaccountid").setValue(crmForm.VerificaMatrizNoCrm(cliente));
                                    // CreateLookup(cliente.MatrizID, cliente.CodigoDaMatriz, "account");
                                }
                            }

                            if (typeof (cliente.Tipo) != 'object')
                                Xrm.Page.getAttribute("new_identificacao").setValue(cliente.Tipo);

                            if (typeof (cliente.Natureza) != 'object')
                                Xrm.Page.getAttribute("new_natureza").setValue(cliente.Natureza);

                            if (typeof (cliente.CodigoMatriz) != 'object')
                                Xrm.Page.getAttribute("accountnumber").setValue(cliente.CodigoMatriz.toString());

                            if (typeof (cliente.Telefone) != 'object')
                                Xrm.Page.getAttribute("telephone1").setValue(cliente.Telefone.toString());

                            if (typeof (cliente.Ramal) != 'object')
                                Xrm.Page.getAttribute("new_ramal1").setValue(cliente.Ramal.toString());

                            if (typeof (cliente.Fax) != 'object')
                                Xrm.Page.getAttribute("fax").setValue(cliente.Fax.toString());

                            if (typeof (cliente.RamalFax) != 'object')
                                crmForm.all.new_ramal_fax = cliente.RamalFax.toString();

                            if (typeof (cliente.OutroTelefone) != 'object')
                                Xrm.Page.getAttribute("telephone2").setValue(cliente.OutroTelefone.toString());

                            if (typeof (cliente.OutroRamal) != 'object')
                                Xrm.Page.getAttribute("new_ramal2").setValue(cliente.OutroRamal.toString());

                            if (typeof (cliente.Email) != 'object')
                                Xrm.Page.getAttribute("emailaddress1").setValue(cliente.Email.toString());

                            if (typeof (cliente.NomeRepresentante) != 'object')
                                Xrm.Page.getAttribute("new_representanteid").setValue(CreateLookup(cliente.RepresentanteId, cliente.NomeRepresentante, "new_representante"));

                            if (typeof (cliente.NomeDoGrupo) != 'object')
                                Xrm.Page.getAttribute("new_grupo_clienteid").setValue(CreateLookup(cliente.GrupoId, cliente.NomeDoGrupo, "new_grupo_cliente"));

                            if (typeof (cliente.EcontribuinteDeICMS) != 'object')
                                Xrm.Page.getAttribute("new_contribuinte_icms").setValue(cliente.EcontribuinteDeICMS);

                            if (typeof (cliente.CodigoSufurama) != 'object')
                                Xrm.Page.getAttribute("new_codigo_suframa").setValue(cliente.CodigoSufurama.toString());

                            if (typeof (cliente.NomeTransportadora) != 'object')
                                Xrm.Page.getAttribute("new_transportadoraid").setValue(CreateLookup(cliente.TransportoraId, cliente.NomeTransportadora, "new_transportadora"));

                            if (typeof (cliente.NomeDoCanalDeVenda) != 'object')
                                Xrm.Page.getAttribute("new_canal_vendaid").setValue(CreateLookup(cliente.CanalDeVendaId, cliente.NomeDoCanalDeVenda, "new_canal_venda"));

                            if (typeof (cliente.InscricaoAuxiliarDeSubstituicao) != 'object')
                                Xrm.Page.getAttribute("new_isnc_subs_trib").setValue(cliente.InscricaoAuxiliarDeSubstituicao);

                            if (typeof (cliente.OptanteDeSuspencaoIPI) != 'object')
                                Xrm.Page.getAttribute("new_optante_ipi").setValue(cliente.OptanteDeSuspencaoIPI);

                            if (typeof (cliente.AgenteRetencao) != 'object')
                                Xrm.Page.getAttribute("new_agente_retencao").setValue(cliente.AgenteRetencao);

                            if (typeof (cliente.PisConfinsPorUnidade) != 'object')
                                Xrm.Page.getAttribute("new_pis_cofins_unidade").setValue(cliente.PisConfinsPorUnidade);

                            if (typeof (cliente.RecebeNFE) != 'object')
                                Xrm.Page.getAttribute("new_recebe_nfe").setValue(cliente.RecebeNFE);

                            if (typeof (cliente.FormaDeTributacao) != 'object')
                                Xrm.Page.getAttribute("new_forma_tributacao_manaus").setValue(cliente.FormaDeTributacao);

                            if (typeof (cliente.DescontoCAT) != 'object')
                                Xrm.Page.getAttribute("new_desconto_cat").setValue(cliente.DescontoCAT);

                            if (typeof (cliente.TipoDeEmbalagem) != 'object')
                                Xrm.Page.getAttribute("new_tipo_embalagem").setValue(cliente.TipoDeEmbalagem);

                            if (typeof (cliente.Observacao) != 'object')
                                Xrm.Page.getAttribute("new_observacao_pedido").setValue(cliente.Observacao);

                            if (typeof (cliente.DispositivoLegal) != 'object')
                                Xrm.Page.getAttribute("new_dispositivo_legal").setValue(cliente.DispositivoLegal);

                            if (typeof (cliente.Incoterm) != 'object')
                                Xrm.Page.getAttribute("new_incoterm").setValue(cliente.Incoterm);

                            if (typeof (cliente.LocalEmbarque) != 'object')
                                Xrm.Page.getAttribute("new_local_embarque").setValue(cliente.LocalEmbarque);

                            if (typeof (cliente.EmbarqueVia) != 'object')
                                Xrm.Page.getAttribute("new_embarque_via").setValue(cliente.EmbarqueVia);

                            if (cliente.DataVencimentoConcessao != "[NO DATE]")
                                Xrm.Page.getAttribute("new_data_vencimento_concessao").setValue(cliente.DataVencimentoConcessao);

                            if (cliente.DataDeImplantacao != "[NO DATE]")
                                Xrm.Page.getAttribute("new_data_implantacao").setValue(cliente.DataDeImplantacao);

                            if (cliente.DataLimiteDeCredito != "[NO DATE]")
                                Xrm.Page.getAttribute("new_data_limite_credito").setValue(cliente.DataLimiteDeCredito);

                            if (typeof (cliente.LimiteDeCredito) != 'object')
                                Xrm.Page.getAttribute("creditlimit").setValue(cliente.LimiteDeCredito);

                            if (typeof (cliente.NomeDoPortador) != 'object')
                                Xrm.Page.getAttribute("new_portadorid").setValue(CreateLookup(cliente.PortadorId, cliente.NomeDoPortador, "new_portador"));

                            if (typeof (cliente.ContaCorrente) != 'object')
                                Xrm.Page.getAttribute("new_contacorrente").setValue(cliente.ContaCorrente.toString());

                            if (typeof (cliente.Agencia) != 'object')
                                Xrm.Page.getAttribute("new_agencia").setValue(cliente.Agencia.toString());

                            if (typeof (cliente.Banco) != 'object')
                                Xrm.Page.getAttribute("new_banco").setValue(cliente.Banco.toString());

                            if (typeof (cliente.NomeDaReceitaPadrao) != 'object')
                                Xrm.Page.getAttribute("new_receita_padraoid").setValue(CreateLookup(cliente.PortadorId, cliente.NomeDaReceitaPadrao, "new_portador"));

                            if (typeof (cliente.CalculaMulta) != 'object')
                                Xrm.Page.getAttribute("new_calcula_multa").setValue(cliente.CalculaMulta);

                            if (typeof (cliente.NomeDaCondicaoDePagamento) != 'object')
                                Xrm.Page.getAttribute("new_condicao_pagamentoid").setValue(CreateLookup(cliente.CondicaoDePagamentoId, cliente.NomeDaCondicaoDePagamento, "new_condicao_pagamento"));

                            if (typeof (cliente.EmiteBloquete) != 'object')
                                Xrm.Page.getAttribute("new_emite_bloqueto").setValue(cliente.EmiteBloquete);

                            if (typeof (cliente.GeraAvisoDeCredito) != 'object')
                                Xrm.Page.getAttribute("new_gera_aviso_credito").setValue(cliente.GeraAvisoDeCredito);

                            if (typeof (cliente.RecebeInformacaoSCI) != 'object')
                                Xrm.Page.getAttribute("new_recebe_informacao_sci").setValue(cliente.RecebeInformacaoSCI);

                            if (typeof (cliente.Modalidade) != 'object')
                                Xrm.Page.getAttribute("new_modalidade").setValue(cliente.Modalidade);

                            if (typeof (cliente.Cpf) != 'object')
                                Xrm.Page.getAttribute("new_cpf").setValue(cliente.Cpf.toString());

                            if (typeof (cliente.Cnpj) != 'object')
                                Xrm.Page.getAttribute("new_cnpj").setValue(cliente.Cnpj.toString());

                            if (typeof (cliente.InscricaoEstadual) == 'object') {
                                var inscest = cliente.InscricaoEstadual.toString().split(",");
                                if (inscest != null)
                                    if (inscest[0] != 'object' && inscest[0] != '[object Object]')
                                        Xrm.Page.getAttribute("new_inscricaoestadual").setValue(inscest[0]);
                            }

                            if (typeof (cliente.NomeAbreviado) != 'object')
                                Xrm.Page.getAttribute("new_nome_abreviado_erp").setValue(cliente.NomeAbreviado.toString());


                            podeBuscarOCep = true;

                        } // fim CONFIRM
                    }

                    else {
                        Xrm.Page.getAttribute("new_cpf").setValue("");
                        Xrm.Page.getAttribute("new_cnpj").setValue("");
                        Xrm.Page.getControl("new_cnpj").setFocus(true);

                    }

                } // fim IF Cnpj != 'Undefined'
                else {
                    if (typeof (cliente.CodigoDaMatriz) != 'object') {
                        if (cliente.CodigoDaMatriz != 0) {
                            if (confirm('Localizada a Matriz deste CNPJ. Deseja relacionar ao cliente?'))
                                Xrm.Page.getAttribute("parentaccountid").setValue(crmForm.VerificaMatrizNoCrm(cliente));
                        }
                    }
                }
            }

        } else { // Caso nao encontre um fornecedor no EMS
            Xrm.Page.getAttribute("accountnumber").setValue(null);
        }
    },


    ValidarMatrizSelecionado: function(matrizselecionado) {

        var matriz = matrizselecionado.getValue();
        var cliente = crmForm.PesquisarClientePorNome(matriz[0].name);

        // fazer comparacao do cnpj
        var troncoCnpjMatriz = cliente.Cnpj.split("/")[0];
        var troncoCnpjSelecionado = Xrm.Page.getAttribute("new_cnpj").getValue().split("/")[0];
        if (troncoCnpjMatriz != troncoCnpjSelecionado) {
            return false;
        }
        return true;
    },

    validaMatrizComCNPJ: function () {

        var matrizSelecionada = Xrm.Page.getAttribute("parentaccountid");

        if (Xrm.Page.getAttribute("new_cnpj").getValue() != null && matrizSelecionada.getValue() != null && Xrm.Page.getAttribute("new_natureza").getValue() != null && Xrm.Page.getAttribute("new_natureza").getValue() == 2) {

            var cnpjRaiz = crmForm.ValidarMatrizSelecionado(matrizSelecionada);

            if (!cnpjRaiz)
                if (!confirm("O cliente informado com a Matriz tem a raiz do CNPJ diferente do cliente que está sendo relacionado. Deseja relacioná-lo mesmo assim?"))
                    matrizSelecionada.setValue("");
        }
    },

    CreateLookup: function(id, name, type) {
        var lookupData = new Array();
        var lookupItem = new Object();

        lookupItem.id = id;
        lookupItem.entityType = type;
        lookupItem.name = name;

        lookupData[0] = lookupItem;

        return lookupData;
    },


    // Verifica qual item foi selecionado
    validaçãoDaNatureza: function () {

        if (Xrm.Page.ui.getFormType() != 2) {

            Xrm.Page.getAttribute("new_inscricaoestadual").setValue(" ");
            Xrm.Page.getAttribute("new_inscricaomunicipal").setValue(" ");
            Xrm.Page.getAttribute("new_codigo_suframa").setValue(" ");
            Xrm.Page.getAttribute("new_cnpj").setValue(" ");
            Xrm.Page.getAttribute("new_cpf").setValue(" ");
            Xrm.Page.getAttribute("new_rg").setValue(" ");

        }

        switch (parseInt(Xrm.Page.getAttribute("new_natureza").getValue())) {

            case 1:
                Xrm.Page.getAttribute("new_cnpj").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_postalcode").setRequiredLevel("recommended");
                Xrm.Page.getControl("new_cnpj").setDisabled(true);
                Xrm.Page.getControl("new_inscricaoestadual").setDisabled(true);
                Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(true);
                Xrm.Page.getControl("new_codigo_suframa").setDisabled(true);
                Xrm.Page.getControl("new_rg").setDisabled(false);
                Xrm.Page.getControl("new_cpf").setDisabled(false);
                break;
            case 2:
                Xrm.Page.getAttribute("new_cnpj").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_postalcode").setRequiredLevel("recommended");
                Xrm.Page.getControl("new_cnpj").setDisabled(false);
                Xrm.Page.getControl("new_inscricaoestadual").setDisabled(false);
                Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(false);
                Xrm.Page.getControl("new_codigo_suframa").setDisabled(false);
                Xrm.Page.getControl("new_rg").setDisabled(true);
                Xrm.Page.getControl("new_cpf").setDisabled(true);
                break;
            case 3:
                Xrm.Page.getAttribute("new_cnpj").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_postalcode").setRequiredLevel("none");
                Xrm.Page.getControl("new_cnpj").setDisabled(false);
                Xrm.Page.getControl("new_inscricaoestadual").setDisabled(false);
                Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(false);
                Xrm.Page.getControl("new_codigo_suframa").setDisabled(false);
                Xrm.Page.getControl("new_rg").setDisabled(true);
                Xrm.Page.getControl("new_cpf").setDisabled(true);
                break;
            case 4:
                Xrm.Page.getAttribute("new_cnpj").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_postalcode").setRequiredLevel("none");
                Xrm.Page.getControl("new_cnpj").setDisabled(false);
                Xrm.Page.getControl("new_inscricaoestadual").setDisabled(false);
                Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(false);
                Xrm.Page.getControl("new_codigo_suframa").setDisabled(false);
                Xrm.Page.getControl("new_rg").setDisabled(true);
                Xrm.Page.getControl("new_cpf").setDisabled(true);
                break;

            default:
                Xrm.Page.getControl("new_cnpj").setDisabled(true);
                Xrm.Page.getControl("new_inscricaoestadual").setDisabled(true);
                Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(true);
                Xrm.Page.getControl("new_codigo_suframa").setDisabled(true);
                Xrm.Page.getControl("new_rg").setDisabled(true);
                Xrm.Page.getControl("new_cpf").setDisabled(true);
                break;
        }

    },


    AutorizarUsuario: function(usuarioGuid, grupoGuid) {
        if (grupoGuid.getValue() != null) {
            //alert(usuarioGuid);
            //alert(grupoGuid.getValue()[0].id);

            var resultado = crmForm.VerificaPermissao(usuarioGuid, grupoGuid.getValue()[0].id);
            //alert(resultado.TemAutorizacao);

            if (resultado.TemAutorizacao == false) {
                DesabilitaFormulario();
            }
        }
    },


    DesabilitaFormulario: function () {

        for (var index = 0; index < crmForm.all.length; index++)
            if (crmForm.all[index].Disabled != null)
                crmForm.all[index].Disabled = true;
    },

    ObterRepresentante: function(codigoGrupoCliente) {

        if (null == codigoGrupoCliente)
            return;

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "PesquisarRepresentantePorCodigo", VENDAS_SERVICE_URL);
        comando.SetParameter("codigoId", codigoGrupoCliente);
        comando.SetParameter("organizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        return resultado;
    },

    CreateLookupInterno: function(id, name, type) {
        return CreateLookup(id, name, type);
    },

    toUpperCase: function(campo) {
        if (campo.getValue() != null)
            campo.setValue(campo.getValue().toUpperCase());
    },

    /**********************************
    Descrição: Validacao de alteracao do endereço principal para fazer alteração no endereço padrão
    **********************************/

    InicializarMonitoramentoDaAlteracaoDoEndereco: function () {
        tmp_bairro = Xrm.Page.getAttribute("address1_line3").getValue();
        tmp_cep = Xrm.Page.getAttribute("address1_postalcode").getValue();
        tmp_cidade = Xrm.Page.getAttribute("address1_city").getValue();
        tmp_rua = Xrm.Page.getAttribute("address1_line1").getValue();
        tmp_uf = Xrm.Page.getAttribute("address1_stateorprovince").getValue();
        tmp_pais = Xrm.Page.getAttribute("address1_country").getValue();
    },

    EnderecoFoiAlterado: function () {

        if (crmForm.all.address1_postalcode.IsDirty) return true;
        if (crmForm.all.address1_line3.IsDirty) return true;
        if (crmForm.all.address1_city.IsDirty) return true;
        if (crmForm.all.address1_line1.IsDirty) return true;
        if (crmForm.all.address1_stateorprovince.IsDirty) return true;
        if (crmForm.all.address1_country.IsDirty) return true;

        return false;
    },

    /**********************************
    Data:      19/11/2010
    Autor:     Clausio
    Descrição: Declarando funções para validação da role para os usuários
    **********************************/
    UserHasRole: function(roleName) {
        var oXml = GetCurrentUserRoles();
        if (oXml != null) {
            var roles = oXml.selectNodes("//BusinessEntity/q1:name");
            if (roles != null) {
                for (i = 0; i < roles.length; i++) {
                    if (roles[i].text == roleName) {
                        return true;
                    }
                }
            }
        }
        return false;
    },

    GetCurrentUserRoles: function () {
        var xml = "" +
         "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
         "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
         Xrm.Page.context.getAuthenticationHeader() +
         " <soap:Body>" +
         " <RetrieveMultiple xmlns=\"http://schemas.microsoft.com/crm/2007/WebServices\">" +
         " <query xmlns:q1=\"http://schemas.microsoft.com/crm/2009/Query\" xsi:type=\"q1:QueryExpression\">" +
         " <q1:EntityName>role</q1:EntityName>" +
         " <q1:ColumnSet xsi:type=\"q1:ColumnSet\">" +
         " <q1:Attributes>" +
         " <q1:Attribute>name</q1:Attribute>" +
         " </q1:Attributes>" +
         " </q1:ColumnSet>" +
         " <q1:Distinct>false</q1:Distinct>" +
         " <q1:LinkEntities>" +
         " <q1:LinkEntity>" +
         " <q1:LinkFromAttributeName>roleid</q1:LinkFromAttributeName>" +
         " <q1:LinkFromEntityName>role</q1:LinkFromEntityName>" +
         " <q1:LinkToEntityName>systemuserroles</q1:LinkToEntityName>" +
         " <q1:LinkToAttributeName>roleid</q1:LinkToAttributeName>" +
         " <q1:JoinOperator>Inner</q1:JoinOperator>" +
         " <q1:LinkEntities>" +
         " <q1:LinkEntity>" +
         " <q1:LinkFromAttributeName>systemuserid</q1:LinkFromAttributeName>" +
         " <q1:LinkFromEntityName>systemuserroles</q1:LinkFromEntityName>" +
         " <q1:LinkToEntityName>systemuser</q1:LinkToEntityName>" +
         " <q1:LinkToAttributeName>systemuserid</q1:LinkToAttributeName>" +
         " <q1:JoinOperator>Inner</q1:JoinOperator>" +
         " <q1:LinkCriteria>" +
         " <q1:FilterOperator>And</q1:FilterOperator>" +
         " <q1:Conditions>" +
         " <q1:Condition>" +
         " <q1:AttributeName>systemuserid</q1:AttributeName>" +
         " <q1:Operator>EqualUserId</q1:Operator>" +
         " </q1:Condition>" +
         " </q1:Conditions>" +
         " </q1:LinkCriteria>" +
         " </q1:LinkEntity>" +
         " </q1:LinkEntities>" +
         " </q1:LinkEntity>" +
         " </q1:LinkEntities>" +
         " </query>" +
         " </RetrieveMultiple>" +
         " </soap:Body>" +
         "</soap:Envelope>" +
         "";

        var xmlHttpRequest = new ActiveXObject("Msxml2.XMLHTTP");
        xmlHttpRequest.Open("POST", "/mscrmservices/2007/CrmService.asmx", false);
        xmlHttpRequest.setRequestHeader("SOAPAction", " http://schemas.microsoft.com/crm/2007/WebServices/RetrieveMultiple");
        xmlHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        xmlHttpRequest.setRequestHeader("Content-Length", xml.length);
        xmlHttpRequest.send(xml);
        var resultXml = xmlHttpRequest.responseXML;
        return (resultXml);
    },

    /**********************************
    Data:      
    Autor:     
    Descrição: Busca o id do usuario
    **********************************/

    getUserId: function () {
        var xml = "" +
    "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
    "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" +
    Xrm.Page.context.getAuthenticationHeader() +
    " <soap:Body>" +
    " <RetrieveMultiple xmlns=\"http://schemas.microsoft.com/crm/2007/WebServices\">" +
    " <query xmlns:q1=\"http://schemas.microsoft.com/crm/2009/Query\" xsi:type=\"q1:QueryExpression\">" +
    " <q1:EntityName>systemuser</q1:EntityName>" +
    " <q1:ColumnSet xsi:type=\"q1:ColumnSet\">" +
    " <q1:Attributes>" +
    " <q1:Attribute>businessunitid</q1:Attribute>" +
    " <q1:Attribute>firstname</q1:Attribute>" +
    " <q1:Attribute>fullname</q1:Attribute>" +
    " <q1:Attribute>lastname</q1:Attribute>" +
    " <q1:Attribute>organizationid</q1:Attribute>" +
    " <q1:Attribute>systemuserid</q1:Attribute>" +
    " </q1:Attributes>" +
    " </q1:ColumnSet>" +
    " <q1:Distinct>false</q1:Distinct>" +
    " <q1:Criteria>" +
    " <q1:FilterOperator>And</q1:FilterOperator>" +
    " <q1:Conditions>" +
    " <q1:Condition>" +
    " <q1:AttributeName>systemuserid</q1:AttributeName>" +
    " <q1:Operator>EqualUserId</q1:Operator>" +
    " </q1:Condition>" +
    " </q1:Conditions>" +
    " </q1:Criteria>" +
    " </query>" +
    " </RetrieveMultiple>" +
    " </soap:Body>" +
    "</soap:Envelope>" +
    "";

        var xmlHttpRequest = new ActiveXObject("Msxml2.XMLHTTP");

        xmlHttpRequest.Open("POST", "/mscrmservices/2007/CrmService.asmx", false);
        xmlHttpRequest.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2007/WebServices/RetrieveMultiple");
        xmlHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        xmlHttpRequest.setRequestHeader("Content-Length", xml.length);
        xmlHttpRequest.send(xml);



        var resultXml = xmlHttpRequest.responseXML;

        var entityNode = resultXml.selectSingleNode("//RetrieveMultipleResult/Entities/BusinessEntity");

        var fullNameNode = entityNode.selectSingleNode("q1:fullname");
        var systemUserIdNode = entityNode.selectSingleNode("q1:systemuserid");

        return systemUserIdNode.text;
    },


    /**********************************
    Data:      19/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Verifica se o campo "Vendar Para ALC" deve ser preenchido,
    essa validação é feita de acordo com a cidade informada, 
    envia a cidade para o ERP e retorna 0 ou 1. 
    **********************************/

    buscaCidadeZonaFranca: function(cidade, uf) {

        var resultado = null;

        var comando = new RemoteCommand(SERVICE_NAME, "CidadeZonaFranca", VENDAS_SERVICE_URL);
        comando.SetParameter("cidade", cidade);
        comando.SetParameter("uf", uf);
        comando.SetParameter("organizacao", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return null;
            }
        }

        return resultado;
    },

    ValidaVendasALC: function () {

        if (Xrm.Page.getAttribute("address1_city").getValue() == null) return;
        if (Xrm.Page.getAttribute("address1_stateorprovince").getValue() == null) return;

        var cidade = Xrm.Page.getAttribute("address1_city").getValue();
        var uf = Xrm.Page.getAttribute("address1_stateorprovince").getValue();
        var resultado = crmForm.buscaCidadeZonaFranca(cidade, uf);

        if (resultado == null) return;

        Xrm.Page.getAttribute("new_vendas_alc").setValue(((resultado.ZonaFranca) ? null : true));
    },

    /**********************************
    Data:      21/03/2011
    Autor:     Cleto May
    Descrição: Retira mascara
    **********************************/
    retiraMascara: function(campo) {
        if (campo.getValue() == null) return;

        var exp = /\-|\.|\/|\(|\)| /g
        var valorCampo = campo.getValue();
        valorCampo = valorCampo.replace(exp, "");

        return valorCampo;
    },

    /**********************************
    Data:      19/05/2011
    Autor:     Cleto May
    Descrição: Verificacao e funções relacionadas ao campo Gerar Pedido
    **********************************/
    verificacaoDoCampoGerarPedido: function () {
        if (Xrm.Page.getAttribute("new_geracao_pedido_posto").getValue() != null) {
            switch (Xrm.Page.getAttribute("new_geracao_pedido_posto").getValue()) {
                case "1":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "10":
                    Xrm.Page.getAttribute("new_parametro_pedido_posto_servico").setValue(null);
                    Xrm.Page.getControl("new_parametro_pedido_posto_servico").setDisabled(true);
                    break;
                case "2":
                case "3":
                case "4":
                    Xrm.Page.getAttribute("new_parametro_pedido_posto_servico").setValue(null);
                    Xrm.Page.getControl("new_parametro_pedido_posto_servico").setDisabled(false);
                    break;
            }
        } else {
            Xrm.Page.getAttribute("new_parametro_pedido_posto_servico").setValue(null);
            Xrm.Page.getControl("new_parametro_pedido_posto_servico").setDisabled(true);
        }
    },

    ValidacoesProgramaCanais: function () {

        // Geral
        Xrm.Page.getControl("new_natureza").setDisabled(true);
        Xrm.Page.getControl("new_inscricaoestadual").setDisabled(true);
        Xrm.Page.getControl("new_inscricaomunicipal").setDisabled(true);
        Xrm.Page.getControl("itbc_nomefantasia").setDisabled(true);
        Xrm.Page.getControl("new_nome_abreviado_erp").setDisabled(true);
        Xrm.Page.getControl("name").setDisabled(true);
        Xrm.Page.getControl("telephone1").setDisabled(true);
        Xrm.Page.getControl("new_ramal1").setDisabled(true);
        Xrm.Page.getControl("telephone2").setDisabled(true);
        Xrm.Page.getControl("new_ramal2").setDisabled(true);
        Xrm.Page.getControl("fax").setDisabled(true);
        Xrm.Page.getControl("emailaddress1").setDisabled(true);
        Xrm.Page.getControl("websiteurl").setDisabled(true);

        // Detalhes
        Xrm.Page.getControl("new_grupo_clienteid").setDisabled(true);
        Xrm.Page.getControl("new_incoterm").setDisabled(true);
        Xrm.Page.getControl("new_tipo_embalagem").setDisabled(true);
        Xrm.Page.getControl("new_local_embarque").setDisabled(true);
        Xrm.Page.getControl("new_embarque_via").setDisabled(true);

        // Endereço Principal
        Xrm.Page.getControl("address1_postalcode").setDisabled(true);
        Xrm.Page.getControl("new_numero_endereco_principal").setDisabled(true);
        Xrm.Page.getControl("address1_line1").setDisabled(true);
        Xrm.Page.getControl("address1_line2").setDisabled(true);
        Xrm.Page.getControl("address1_line3").setDisabled(true);
        Xrm.Page.getControl("address1_city").setDisabled(true);
        Xrm.Page.getControl("address1_stateorprovince").setDisabled(true);
        Xrm.Page.getControl("address1_country").setDisabled(true);

        // Endereço de Cobrança
        Xrm.Page.getControl("address2_postalcode").setDisabled(true);
        Xrm.Page.getControl("address2_line1").setDisabled(true);
        Xrm.Page.getControl("new_numero_endereco_cobranca").setDisabled(true);
        Xrm.Page.getControl("address2_line2").setDisabled(true);
        Xrm.Page.getControl("address2_line3").setDisabled(true);
        Xrm.Page.getControl("address2_stateorprovince").setDisabled(true);
        Xrm.Page.getControl("address2_country").setDisabled(true);
        Xrm.Page.getControl("address2_city").setDisabled(true);


        // Fiscal
        Xrm.Page.getControl("new_optante_ipi").setDisabled(true);
        Xrm.Page.getControl("new_codigo_suframa").setDisabled(true);
        Xrm.Page.getControl("new_agente_retencao").setDisabled(true);
        Xrm.Page.getControl("new_isnc_subs_trib").setDisabled(true);
        Xrm.Page.getControl("new_contribuinte_icms").setDisabled(true);
        Xrm.Page.getControl("new_forma_tributacao_manaus").setDisabled(true);
        Xrm.Page.getControl("new_pis_cofins_unidade").setDisabled(true);
        Xrm.Page.getControl("new_desconto_cat").setDisabled(true);
        Xrm.Page.getControl("new_recebe_nfe").setDisabled(true);
        Xrm.Page.getControl("new_vendas_alc").setDisabled(true);
        Xrm.Page.getControl("new_observacao_pedido").setDisabled(true);
        Xrm.Page.getControl("new_dispositivo_legal").setDisabled(true);
        Xrm.Page.getControl("new_data_vencimento_concessao").setDisabled(true);

        // Fiscal
        Xrm.Page.getControl("numberofemployees").setDisabled(true);
        Xrm.Page.getControl("numberofemployees").setDisabled(true);
    },

    ParticipaProgramaCanais: function () {

        var value = Xrm.Page.getAttribute("new_crm2013.value");

        return (value != "" && value != "0");
    },

    StatusDoCadastroOnChange: function () {

        switch (parseInt(Xrm.Page.getAttribute("new_status_cadastro").getValue())) {
            case 1:
                Xrm.Page.getAttribute("new_natureza").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_nome_abreviado_erp").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_representanteid").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_grupo_clienteid").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_city").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_stateorprovince").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_country").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_city").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_stateorprovince").setRequiredLevel("none");
                Xrm.Page.getAttribute("address2_country").setRequiredLevel("none");
                break;
            case 2:
                Xrm.Page.getAttribute("new_natureza").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_nome_abreviado_erp").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_representanteid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_grupo_clienteid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_city").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_stateorprovince").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_country").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_line1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_city").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_stateorprovince").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address2_country").setRequiredLevel("recommended");
                break;
        }
    },

    OnSave: function () {

        var campoCpfCnpjSemMascara = null;

        /* Envia Cliente para o EMS */
        switch (parseInt(Xrm.Page.getAttribute("new_status_cadastro").getValue())) {

            /* aguardando encerramento */ 
            case 2:
                Xrm.Page.getAttribute("new_exporta_erp").setValue("S");
                break;
            case 3:
                Xrm.Page.getAttribute("new_exporta_erp").setValue("S");
                break;
        }

        if (parseInt(Xrm.Page.getAttribute("new_status_cadastro").getValue()) != 1) 
            if (Xrm.Page.ui.getFormType() == 2) 
                if (crmForm.EnderecoFoiAlterado()) 
                    if (confirm('Deseja alterar o endereço padrão?')) 
                        Xrm.Page.getAttribute("new_altera_endereco_padrao").setValue("u");
            
        
        switch (Xrm.Page.getAttribute("new_natureza").getValue()) {

            case '1': // Pessoa Fisica
                var campoCpfCnpjSemMascara = crmForm.retiraMascara(crmForm.all.new_cpf);
                break;

            case '2': //Pessoa Juridica
                var campoCpfCnpjSemMascara = crmForm.retiraMascara(crmForm.all.new_cnpj);
                break;
        }


        Xrm.Page.getAttribute("new_sem_masc_cnpj_cpf").setValue(campoCpfCnpjSemMascara);
        Xrm.Page.getControl("new_altera_endereco_padrao").setDisabled(false);
        Xrm.Page.getControl("new_exporta_erp").setDisabled(false);
        Xrm.Page.getControl("new_inscricaoestadual").setDisabled(false);

        if (!Xrm.Page.data.entity.getIsDirty()() && Xrm.Page.ui.getFormType() == 2 /*UPDATE*/)
            alert("O cliente não foi salvo e integrado, você precisa fazer a alteração em algum campo e depois salvar!");
    },

    new_natureza_onchange: function () {
        crmForm.validaçãoDaNatureza();

        Xrm.Page.getAttribute("new_inscricaoestadual").setValue(null);
        Xrm.Page.getAttribute("new_inscricaomunicipal").setValue(null);
        Xrm.Page.getAttribute("new_codigo_suframa").setValue(null);
        Xrm.Page.getAttribute("new_cnpj").setValue(null);
        Xrm.Page.getAttribute("new_cpf").setValue(null);
        Xrm.Page.getAttribute("new_rg").setValue(null);
        Xrm.Page.getAttribute("new_sem_masc_cnpj_cpf").setValue(null);
    },

    new_cnpj_onchange: function () {
        switch (Xrm.Page.getAttribute("new_natureza").getValue()) {

            case '2': //Pessoa Juridica
                Xrm.Page.getAttribute("new_cnpj").setValue(crmForm.FormatarDocumentoPorNaturezaDoCliente(Xrm.Page.getAttribute("new_natureza").getValue()));

                crmForm.ValidarExistenciaDoClientePor(crmForm.all.new_cnpj, crmForm.all.new_natureza);

                crmForm.validaMatrizComCNPJ();
                break;

            case '3': //Estrangeiro
                crmForm.ValidarExistenciaDoClientePor(crmForm.all.new_cnpj, crmForm.all.new_natureza);
                break;
        }
    },

    new_cpf_onchange: function () {
        Xrm.Page.getAttribute("new_cpf").setValue(crmForm.FormatarDocumentoPorNaturezaDoCliente(Xrm.Page.getAttribute("new_natureza").getValue()));
        crmForm.ValidarExistenciaDoClientePor(crmForm.all.new_cpf, crmForm.all.new_natureza);
    },

    new_inscricaoestadual_onchange: function () {
        crmForm.toUpperCase ( crmForm.all.new_inscricaoestadual );
    },

    new_inscricaomunicipal_onchange: function () {
        crmForm.toUpperCase ( crmForm.all.new_inscricaomunicipal );
    },

    accountnumber_onchange: function () {
        if(crmForm.accountnumber.getValue() != null)
            Xrm.Page.getAttribute("new_altera_endereco_padrao").setValue("S");
    },

    new_status_cadastro_onchange: function () {
        crmForm.StatusDoCadastroOnChange();
    },

    new_nome_abreviado_erp_onchange: function () {
        crmForm.toUpperCase ( crmForm.all.new_nome_abreviado_erp );
    },
    
    parentaccountid_onchange: function () {
        crmForm.validaMatrizComCNPJ();
    },
    
    telephone1_onchange: function () {
        crmForm.phoneFormat(crmForm.all.telephone1);
    },
    
    telephone2_onchange: function () {
        crmForm.phoneFormat(crmForm.all.telephone2);
    },
    
    fax_onchange: function () {
        crmForm.phoneFormat(crmForm.all.fax);
    },
    
    new_grupo_clienteid_onchange: function () {
        if (crmForm.all.new_grupo_clienteid != null && Xrm.Page.getAttribute("new_grupo_clienteid").getValue() != null) {
            if (Xrm.Page.getAttribute("new_representanteid").getValue() == null) {

                var grupoClienteId = Xrm.Page.getAttribute("new_grupo_clienteid").getValue()[0].id;
                var resultado = crmForm.ObterRepresentante(grupoClienteId);
                if (resultado != null) {
                    if (resultado.TemRepresentante) {
                        Xrm.Page.getAttribute("new_representanteid").setValue(crmForm.CreateLookupInterno(resultado.CodigoRepresentante, resultado.NomeRepresentante, 'new_representante'));
                    }
                }
            }
        }
    },

    new_geracao_pedido_posto_onchange: function () {

    },

    address1_telephone1_onchange: function () {
        crmForm.phoneFormat(crmForm.all.address1_telephone1);
    },
    
    address1_postalcode_onchange: function () {
        if (Xrm.Page.getAttribute("address1_postalcode").getValue() != null){
            if (podeBuscarOCep) {
                Xrm.Page.getAttribute("address1_postalcode").getValue() =
            crmForm.FormatarCep(crmForm.all.address1_postalcode);

                crmForm.PesquisarEnderecoPrincipalPor(Xrm.Page.getAttribute("address1_postalcode").getValue());
            }
        }

        crmForm.ValidaVendasALC();
    },
    
    address1_line1_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null) {
            if (cep == cepCob) {
                if (confirm("Copiar o este endereço para o endereço de cobrança?")) {
                    Xrm.Page.getAttribute("address2_line1").setValue(Xrm.Page.getAttribute("address1_line1").getValue());
                }
            }
        }
    },
    
    new_numero_endereco_principal_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null) 
            if (cep == cepCob) 
                if (confirm("Copiar o este numero para o numero de cobrança?")) 
                    Xrm.Page.getAttribute("new_numero_endereco_cobranca").setValue(Xrm.Page.getAttribute("new_numero_endereco_principal").getValue());
    },
    
    address1_line2_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null) 
            if (cep == cepCob) 
                if (confirm("Copiar o este complemento para o complemento de cobrança?")) 
                    Xrm.Page.getAttribute("address2_line2").setValue(Xrm.Page.getAttribute("address1_line2").getValue()); // Complemento
    },
    
    address1_line3_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null) 
            if (cep == cepCob) 
                if (confirm("Copiar o este endereço para o endereço de cobrança?")) 
                    Xrm.Page.getAttribute("address2_line3").setValue(Xrm.Page.getAttribute("address1_line3").getValue());
    },
    
    address1_city_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null)
            if (cep == cepCob)
                if (confirm("Copiar o este endereço para o endereço de cobrança?"))
                    Xrm.Page.getAttribute("address2_city").setValue(Xrm.Page.getAttribute("address1_city").getValue());


        crmForm.ValidaVendasALC();
    },
    
    address1_stateorprovince_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null)
            if (cep == cepCob)
                if (confirm("Copiar o este endereço para o endereço de cobrança?"))
                    Xrm.Page.getAttribute("address2_stateorprovince").setValue(Xrm.Page.getAttribute("address1_stateorprovince").getValue());

        crmForm.ValidaVendasALC();
    },
    
    address1_country_onchange: function () {
        var cepCob = Xrm.Page.getAttribute("address2_postalcode").getValue();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        if (cepCob != null)
            if (cep == cepCob)
                if (confirm("Copiar o este endereço para o endereço de cobrança?"))
                    Xrm.Page.getAttribute("address2_country").setValue(Xrm.Page.getAttribute("address1_country").getValue());
    },
    
    address2_telephone1_onchange: function () {
        crmForm.phoneFormat(crmForm.all.address2_telephone1);
    },
    
    address2_postalcode_onchange: function () {
        if(Xrm.Page.getAttribute("address2_postalcode").getValue() != null){
            Xrm.Page.getAttribute("address2_postalcode").getValue() = 
            crmForm.FormatarCep(crmForm.all.address2_postalcode);

            crmForm.PesquisarEnderecoDeCobrancaPor(Xrm.Page.getAttribute("address2_postalcode").getValue());
        }
    },
    
    new_contribuinte_icms_onchange: function () {
        //Funcao no onload do Formulario
        crmForm.InscricaoEstadual (crmForm.all.new_contribuinte_icms);
    },
    
    donotphone_onchange: function () {
        crmForm.phoneFormat(crmForm.all.donotphone);
    },
    
    donotfax_onchange: function () {
        crmForm.phoneFormat(crmForm.all.donotfax);
    }
}