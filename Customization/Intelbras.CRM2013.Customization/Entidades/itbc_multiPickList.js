// var_sc_optionset >> nome do campo Conjunto de opções
// var_sc_optionsetvalue >> nome do campo que irá armazenar os valores selecionados do conjunto de opções
// OS >> objeto Conjunto de opções
// OSV >> objeto de campo texto que armazena os valores selecionados

// método para converter o conjunto de opções em um checkbox
function ConvertToMultiSelect(var_sc_optionset, var_sc_optionsetvalue, OS, OSV) {

    if (OS != null && OSV != null) {

        document.getElementById(var_sc_optionset).style.display = "none";

        // Criar um DIV container
        var addDiv = document.createElement("div");
        addDiv.id = var_sc_optionsetvalue + "_m";
        addDiv.style.width = "100%";
        addDiv.style.height = "80px";
        addDiv.style.background = "#ffffff";
        addDiv.style.color = "white";
        addDiv.style.overflow = "auto";
        addDiv.style.border = "1px #6699cc solid";
        document.getElementById(var_sc_optionset).parentNode.appendChild(addDiv);

        //declaração das variáveis de loop de acordo com o browser utilizado
        var initialValue = 0;
        var maxValue = 0;

        var nAgt = navigator.userAgent;

        if (nAgt.indexOf("Firefox") != -1) {  // se broswer é "Firefox" 

            initialValue = 1;
            maxValue = OS.length;

        }
        else if (nAgt.indexOf("Chrome") != -1 || nAgt.indexOf("IE") != -1) { // se browser é Chrome ou IE

            initialValue = 0;
            maxValue = OS.length - 1;
        }
        else if (nAgt.indexOf("Safari") != -1) {  //se browser é "Safari"

            initialValue = 1;
            maxValue = OS.length;
        }

        // inicializar controle checkbox
        for (var i = initialValue; i < maxValue; i++) {
            var pOption = OS[i];
            if (!IsChecked(pOption.value, var_sc_optionsetvalue)) {
                var addInput = document.createElement("input");
                addInput.type = "checkbox";
                addInput.style.border = "none";
                addInput.style.width = "25px";
                addInput.style.align = "left";
                addInput.style.color = "#000000";
                addInput.onclick = function () {

                    OnSave(var_sc_optionset, var_sc_optionsetvalue);
                    createTable(var_sc_optionsetvalue);
                }
            }
            else {

                var addInput = document.createElement("input");
                addInput.type = "checkbox";
                addInput.checked = true;
                addInput.setAttribute("checked", true);
                addInput.checked = "checked";
                addInput.defaultChecked = true;
                addInput.style.border = "none";
                addInput.style.width = "25px";
                addInput.style.align = "left";
                addInput.style.color = "#000000";
                addInput.onclick = function () {

                    OnSave(var_sc_optionset, var_sc_optionsetvalue);
                    createTable(var_sc_optionsetvalue);
                }
            }

            //Criar Label
            var addLabel = document.createElement("label");
            addLabel.style.color = "#000000";
            addLabel.innerHTML = pOption.text;

            var addBr = document.createElement("br"); // it's a 'br' flag

            document.getElementById(var_sc_optionset).nextSibling.appendChild(addInput);
            document.getElementById(var_sc_optionset).nextSibling.appendChild(addLabel);
            document.getElementById(var_sc_optionset).nextSibling.appendChild(addBr);
        }
    }
}

//verificar se opção está marcada
function IsChecked(pText, optionSetValue) {
    var selectedValue = Xrm.Page.getAttribute(optionSetValue).getValue();

    if (selectedValue != "" && selectedValue != null) {
        var OSVT = selectedValue.split(",");

        for (var i = 0; i < OSVT.length; i++) {
            if (OSVT[i] == pText) {
                return true;
            }
        }
    }

    return false;
}

// var_sc_optionsetvalue >>nome lógico do campo que armazena opções escolhidas
// optionSet>> nome lógico do campo Conjunto de opções
function OnSave(optionSet, var_sc_optionsetvalue) {

    var OS = document.getElementById(optionSet);
    var options = Xrm.Page.getAttribute(optionSet).getOptions();
    var getInput = OS.nextSibling.getElementsByTagName("input");
    var result = "";
    var result1 = "";
    var nAgt = navigator.userAgent;

    for (var i = 0; i < getInput.length; i++) {
        if (getInput[i].checked) {
            result += getInput[i].nextSibling.innerHTML + ",";

            if (nAgt.indexOf("Firefox") != -1) {  //se broswer é "Firefox"
                result1 += options[i + 1].value + ",";
            }

            else if (nAgt.indexOf("Chrome") != -1 || nAgt.indexOf("IE") != -1) { //se browser é Chrome or IE

                result1 += options[i].value + ",";
            }

            else if (nAgt.indexOf("Safari") != -1) {  //se browser é "Safari"

                result1 += options[i + 1].value + ",";

            }
        }
    }
    //salvar valores escolhidos
    Xrm.Page.getAttribute(var_sc_optionsetvalue).setValue(result1);
}

// var_sc_optionsetvalue >> nome lógico do campo que armazena as opções escolhidas
function createTable(var_sc_optionsetvalue) {

    // Get OptionSet value
    var OptionValue = Xrm.Page.getAttribute(var_sc_optionsetvalue);
    var c_OptionValue = Xrm.Page.getControl(var_sc_optionsetvalue);
    var d_OptionValue = var_sc_optionsetvalue + "_d";

    if (OptionValue.getValue() != null) {
        //se o campo outros esta marcado habilita o campo descrição caso contrario desabilita
        if (OptionValue.getValue().indexOf("993520005") != -1) {
            Xrm.Page.getAttribute("itbc_descricaodaareadeatuacao").setRequiredLevel("required");
            Xrm.Page.getControl("itbc_descricaodaareadeatuacao").setVisible(true);
        } else {
            Xrm.Page.getAttribute("itbc_descricaodaareadeatuacao").setRequiredLevel("none");
            Xrm.Page.getAttribute("itbc_descricaodaareadeatuacao").setValue("");
            Xrm.Page.getControl("itbc_descricaodaareadeatuacao").setVisible(false);
        }

        var OptionValueHtml = "<div style=\"overflow-y:auto;width:100%;display: none; min-height: 5em; max-height: 1000px;\">";

        OptionValueHtml += "<table style='width:100%;height: 100%;'>";
        var OptionValueV = OptionValue.getValue();

        var OptionValueT = OptionValueV.split(",");
        var cols = 0;
        for (var row = 0; row < OptionValueT.length - 1; row++) {
            OptionValueHtml += "<tr  style='height:20px;'>";
            for (var i = cols; i < cols + 3; i++) {
                OptionValueHtml += "<td style='width:33%;'>";
                if (OptionValueT[i] != null || OptionValueT[i] != undefined) {

                    OptionValueHtml += OptionValueT[i];
                }
                OptionValueHtml += "</td>";
            }
            cols = cols + 3;
            OptionValueHtml += "</tr>";
            if (cols >= OptionValueT.length) {
                break;
            }
        }

        OptionValueHtml += "</table>";
        OptionValueHtml += "</div>";
        document.getElementById(d_OptionValue).innerHTML = OptionValueHtml;
    } else {
        Xrm.Page.getAttribute("itbc_descricaodaareadeatuacao").setRequiredLevel("none");
        Xrm.Page.getAttribute("itbc_descricaodaareadeatuacao").setValue("");
        Xrm.Page.getControl("itbc_descricaodaareadeatuacao").setVisible(false);
    }
}