function pricelevelid_setadditionalparams() {
    var oLookup = event.srcElement; AddTransactionCurrencyParam(oLookup)
}
function Form_onsave() {
    /*************************
    Ações OnSave
    *************************/

    //Preencher o campo Quantidade com a informacao igual 1
    if (crmForm.all.new_quantidade.DataValue == null) {
        crmForm.all.new_quantidade.DataValue = 1
    }
}
function Form_onload() {
    /**********************************
    Data:      14/03/2011
    Autor:     
    Descrição: Define varios intervalos para o RangePickList
    **********************************/

    function ArrayRangePickList(pPckObjeto, pRangeInicio, pRangeFim) {

        var arrayOpcoes = new Array();
        var quantidadeOpcoes = 0;

        for (var i = 0; i < pRangeInicio.length; i++) {
            for (var j = pRangeInicio[i]; j <= pRangeFim[i]; j++) {
                arrayOpcoes[quantidadeOpcoes] = j;
                quantidadeOpcoes++;
            }
        }

        var arrayIndexOpcoes = new Array();
        var k = 0;
        for (var i = 0; i < pPckObjeto.options.length; i++) {
            for (var j = 0; j < arrayOpcoes.length; j++) {
                if (pPckObjeto.options[i].value == arrayOpcoes[j]) {
                    arrayIndexOpcoes[k] = i;
                    k++;
                }
            }
        }

        var excluir = false;
        for (var i = (pPckObjeto.options.length - 1) ; i >= 0; i--) {
            excluir = true;
            for (var j = 0; j < arrayIndexOpcoes.length; j++) {
                if (i == arrayIndexOpcoes[j]) {
                    excluir = false;
                }
            }
            if (excluir) {
                pPckObjeto.DeleteOption(pPckObjeto.options[i].value);
            }
        }
    }

    /**********************************
    Data:      14/03/2011
    Autor:     
    Descrição: RangePickList
    **********************************/

    function RangePickList(pPckObjeto, pRangeInicio, pRangeFim) {

        if (pRangeInicio > -1 && pRangeFim > -1) {
            var tempArray = new Array();
            var index = 0;

            //Loop dos Valores do Picklist
            for (var i = 0; i < pPckObjeto.options.length; i++) {

                //Verifica se o Range é Valido conforme Parametro
                if (pPckObjeto.options[i].value >= pRangeInicio &&
                    pPckObjeto.options[i].value <= pRangeFim) {

                    //Adiciona o elemento no Picklist
                    tempArray[index] = pPckObjeto.options[i];
                    index++;
                }

            }

            for (var j = pPckObjeto.options.length - 1; j >= 0; j--)
                pPckObjeto.DeleteOption(pPckObjeto.options[j].value);


            pPckObjeto.AddOption("", "");

            for (var xi = 0; xi < tempArray.length; xi++)
                pPckObjeto.options[xi + 1] = tempArray[xi];



            // pPckObjeto.AddOption(tempArray[xi].innerText, tempArray[xi].value);
            //pPckObjeto.DataValue = tempArray;

        }
        else {
            pPckObjeto.DataValue = null;
        }

    }

    /**********************************
    Data:      14/03/2011
    Autor:     
    Descrição: Retorna unidade de negocio
    **********************************/
    crmForm.GetMyBusinessUnit = function () {
        var ContactID = Xrm.Page.context.getUserId();
        Cols = ["businessunitid"];
        var Retrieved = XrmServiceToolkit.Soap.Retrieve("systemuser", ContactID, Cols);

        var UnitObj = Retrieved.attributes["businessunitid"];
        var UnitName = UnitObj.name

        return (UnitName);
    }

    /*************************
    Ações OnLoad
    *************************/

    //Descrição: Esconder Seção Hide
    crmForm.all.new_quantidade_c.parentElement.parentElement.style.display = 'none';

    //Preencher o campo Quantidade com a informacao igual 1
    if (crmForm.all.new_quantidade.DataValue == null) {
        crmForm.all.new_quantidade.DataValue = 1
    }

    //Esconde o campo cliente final
    crmForm.all.new_cliente_finalid_c.style.display = 'none';
    crmForm.all.new_cliente_finalid_d.style.display = 'none';

    switch (UN_DO_USUARIO) {

        case "ISOL":
            RangePickList(crmForm.all.statuscode, 0, 200000);
            break;

        case "ISEC":
            RangePickList(crmForm.all.statuscode, 200010, 200014);
            //Habilita campo cliente final
            crmForm.all.new_cliente_finalid_c.style.display = 'block';
            crmForm.all.new_cliente_finalid_d.style.display = 'block';
            break;

        default:
            var inicio = new Array();
            inicio[0] = 1;
            inicio[1] = 200000;
            inicio[2] = 200010;

            var fim = new Array();
            fim[0] = 2;
            fim[1] = 200000;
            fim[2] = 200014;

            ArrayRangePickList(crmForm.all.statuscode, inicio, fim);
            break;
    }

    crmForm.ObtemInformacaoDeFechamentoDaOportunidade = function () {

        /*
        Returns:  array of values whereby:
        [0] = 'statecode';  //1 = won, 2 = lost                                      
        [1] = 'statuscode';  //status reason                                               
        [2] = 'actualrevenue'; //actual revenue                                        
        [3] = 'actualend';  //actual close date
        [4] = 'competitorid'; //competitorid
        [5] = 'description'; //description
        [6] = 'activityid';  //this is GUID for the new Close Opportunity activity
        */
        var state = crmFormSubmit.crNewState.value;
        var status = crmFormSubmit.crNewStatus.value;
        var arr = new Array();
        arr[0] = state;
        arr[1] = status;
        var arrFields = new Array(); //build array of fields on close opp. (in order of appearance)
        arrFields[0] = 'statecode';  //1 = won, 2 = lost                        
        arrFields[1] = 'statuscode'; //status reason                                                                                   
        arrFields[2] = 'actualrevenue'; //actual revenue                               
        arrFields[3] = 'actualend';  //actual close date
        arrFields[4] = 'competitorid'; //competitorid
        arrFields[5] = 'description'; //description
        arrFields[6] = 'activityid'; //this is GUID for the new Close Opportunity activity
        var xml = crmFormSubmit.crActivityXml.value;
        var XmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        XmlDoc.async = false;
        XmlDoc.loadXML(xml);

        for (var i = 0; i < arrFields.length; i++) {
            //get close out information
            if (i > 1) {
                var xmlnode = XmlDoc.selectSingleNode("//opportunityclose/" + arrFields[i]);
                if (xmlnode != null) {
                    arr[i] = xmlnode.nodeTypedValue;
                } else {
                    arr[i] = "";
                }
            }
            //alert(arrFields[i] + " - "+ arr[i]);
        }
        return arr;
    }
}
function new_quantidade_onchange() {

}
