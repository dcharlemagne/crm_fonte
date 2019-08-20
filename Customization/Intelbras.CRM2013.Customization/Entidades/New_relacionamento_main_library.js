function Form_onsave() {
    //Exportar Relacionamento para o EMS 
    Xrm.Page.getAttribute("new_exporta_erp").getValue() = "S"
    Xrm.Page.getAttribute("new_status_integracao").getValue() = "Integrando com o ERP"
    Xrm.Page.getControl("new_exporta_erp").setDisabled(false);
}
function Form_onload() {
    /**********************************
    Data:      01/10/2010
    Autor:     Gabriel Dias Junckes
    Descrição: Altera o FilterLookup padrão para o customizado.
    **********************************/
    crmForm.FilterLookup = function (attribute, url, param) {

        if (param != null)
            url += "?" + param + "&orgname=" + Xrm.Page.context.getOrgUniqueName();
        else
            url += "?orgname=" + Xrm.Page.context.getOrgUniqueName();

        oImg = eval('attribute' + '.parentNode.childNodes[0]');

        oImg.onclick = function () {

            retorno = openStdDlg(url, null, 600, 450);

            if (typeof (retorno) != "undefined") {

                strValues = retorno.split('*|*');
                var lookupData = new Array();
                lookupItem = new Object();
                lookupItem.id = "{" + strValues[1] + "}";
                lookupItem.type = parseInt(strValues[2]);
                lookupItem.name = strValues[0];
                lookupData[0] = lookupItem;
                attribute.setValue(lookupData);
                attribute.fireOnChange();
            }

        };
    };


    /**********************************
    Data:      01/10/2010
    Autor:     Gabriel Dias Junckes
    Descrição: Altera o FilterLookup padrão para o customizado.
    **********************************/
    crmForm.filtraCategoriadaUnidadeNegocio = function (campoUnidadeNegocio, campoCategoria) {

        var id = "";

        if (campoUnidadeNegocio.getValue())
            id = campoUnidadeNegocio.getValue()[0].id;

        var oParam = "objectTypeCode=10022&filterDefault=false&_new_unidadedenegocioid=" + id;
        crmForm.FilterLookup(campoCategoria, "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx", oParam);

        if (campoUnidadeNegocio.IsDirty)
            campoCategoria.setValue(null);

    }


    /**********************************
    Data:      01/10/2010
    Autor:     Gabriel Dias Junckes
    Descrição: Abre uma mensagem no cabeçario do Form. 
    **********************************/
    Notification = function (msg) {
        // Recupera local onde será exibida mensagem
        var element = 'Notifications';
        var id = 'divMessage';
        var src = document.getElementById(element);

        // Se mensagem desejada (parâmetro) nula ou vazia, ocultar
        if ((msg == null) || (msg == "")) {
            src.style.display = 'none';
        }
        else {
            // Cria NOVO elemento para inserir mensagem.
            var newcontent = document.createElement("span");
            newcontent.id = id;

            // E insere mensagem (parâmetro) a ser exibida
            newcontent.innerHTML = "<table><tr><td><img src='/_imgs/ico/16_info.gif' /></td><td valign='top'>" + msg + "</td></tr></table>";
            src.style.display = "";

            // Insere novo elemento no formulário
            var previous = src.firstChild;
            if (previous == null || previous.attributes['id'].nodeValue != id) {

                if (src.childNodes.length == 0)
                    src.appendChild(newcontent);
                else
                    src.insertBefore(newcontent, src.firstChild);
            }
            else
                src.replaceChild(newcontent, previous);
        }
    }


    /**********************************
    Data:      01/10/2010
    Autor:     Gabriel Dias Junckes
    Descrição: Abre uma mensagem no cabeçario do Form. 
    **********************************/
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


    /**********************************
    Ações do OnLoad.
    **********************************/

    //Filter Lookup no campo Unidade de Negócio
    Xrm.Page.getAttribute("new_unidadedenegociosid").fireOnChange();


    Notification(Xrm.Page.getAttribute("new_mensagem").getValue());


    // Desabilitar Campos.
    crmForm.all.new_mensagem.disabled = true
    crmForm.all.new_exporta_erp.disabled = true
    crmForm.all.new_status_integracao.disabled = true
    crmForm.all.new_chaveintegracao.disabled = true


    // Esconde Atividades e Historico.
    crmForm.disableVerticalMenuItem("crmNavBar", "Atividades");
    crmForm.disableVerticalMenuItem("crmNavBar", "Histórico");


    //Esconde Seção Hide.
    crmForm.all.new_sequencia_c.parentElement.parentElement.style.display = 'none';
}
function new_unidadedenegociosid_onchange() {
    //*Copiar o nome da Unidade de Negocio  para um campo Nome
    if (Xrm.Page.getAttribute("new_unidadedenegociosid").getValue() != null && Xrm.Page.getAttribute("new_name").getValue() == null) {
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_unidadedenegociosid").getValue()[0].name);
    }

    /*********** Filter Lookup nos campo Unidade de Negócio***********/
    // Função Filter Lookup
    crmForm.filtraCategoriadaUnidadeNegocio(crmForm.all.new_unidadedenegociosid, crmForm.all.new_categoriaid);
}
