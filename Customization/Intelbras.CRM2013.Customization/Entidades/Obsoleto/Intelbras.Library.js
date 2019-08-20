var SERVER_URL = Xrm.Page.context.getServerUrl();
var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";


var RESULTMULTIPLE = new Array();

// Se o objeto AlfaPeople não existir, o mesmo será criado
if (typeof (AlfaPeople) == "undefined") {
    Intelbras = {};
}

Intelbras.Library = {

    RetrieveRecord: function (id, type, select, expand) {
        try {
            var systemQueryOptions = "";

            if (select != null || expand != null) {
                systemQueryOptions = "?";
                if (select != null) {
                    var selectString = "$select=" + select;
                    if (expand != null) {
                        selectString = selectString + "," + expand;
                    }
                    systemQueryOptions = systemQueryOptions + selectString;
                }
                if (expand != null) {
                    systemQueryOptions = systemQueryOptions + "&$expand=" + expand;
                }
            }

            var request = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/OrganizationData.svc/" + type + "Set(guid'" + id + "')" + systemQueryOptions;

            var retrieveRequest = new XMLHttpRequest();
            retrieveRequest.open("GET", request, false);
            retrieveRequest.setRequestHeader("Accept", "application/json");
            retrieveRequest.setRequestHeader("Content-Type", "application/json; charset=utf-8");

            retrieveRequest.send();

            var resultXml = retrieveRequest.responseXML;
            var result = JSON.parse(retrieveRequest.responseText).d;

            return result;

        } catch (e) {
            alert(e.Message);
        }
    },

    RetrieveMultipleRecord: function (type, select, expand, filter) {
        try {
            var systemQueryOptions = "";

            if (select != null || expand != null || filter != null) {
                systemQueryOptions = "?";
                if (select != null) {
                    var selectString = "$select=" + select;
                    if (expand != null) {
                        selectString = selectString + "," + expand;
                    }
                    systemQueryOptions = systemQueryOptions + selectString;
                }
                if (expand != null) {
                    systemQueryOptions = systemQueryOptions + "&$expand=" + expand;
                }
                if (filter != null) {
                    systemQueryOptions = systemQueryOptions + "&$filter=" + filter;
                }
            }

            var request = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/OrganizationData.svc/" + type + "Set()" + systemQueryOptions;

            var retrieveRequest = new XMLHttpRequest();
            retrieveRequest.open("GET", request, false);
            retrieveRequest.setRequestHeader("Accept", "application/json");
            retrieveRequest.setRequestHeader("Content-Type", "application/json; charset=utf-8");

            retrieveRequest.send();

            var resultXml = retrieveRequest.responseXML;
            var result = JSON.parse(retrieveRequest.responseText).d;

            if (result != null) {
                return result;
            }

        } catch (e) {
            alert(e.Message);
        }
    },

    DynamicSearchRecord: function (odataUri) {
        var result;
        //Asynchronous AJAX function to Retrieve CRM records using OData
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataUri,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.             
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {

                if (data.d.results[0] != null || data.d.results[0] != undefined) {

                    result = data.d.results;
                }
                else {

                    result = null;
                }
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {

                result = "An error ocurred for dynamic search record.\n" +
                     "Please contact the Administrator and inform this message.\n " +
                     "Error : " + textStatus + ": " + XmlHttpRequest.statusText + " - errorThrow " + errorThrown;

            }
        });

        return result;
    },

    LeaveField: function (field, canLeaveField) {

        if (canLeaveField == false) {

            Xrm.Page.getControl(field).setFocus();
        }
    },

    FormatCNPJ: function (value) {

        var cnpj_Pt1 = value.substring(0, 2);
        var cnpj_Pt2 = value.substring(2, 5);
        var cnpj_Pt3 = value.substring(5, 8);
        var cnpj_Pt4 = value.substring(8, 12);
        var cnpj_Pt5 = value.substring(12, 14);
        var ponto = ".";
        var traco = "-";
        var barra = "/";
        return cnpj_Pt1 + ponto + cnpj_Pt2 + ponto + cnpj_Pt3 + barra + cnpj_Pt4 + traco + cnpj_Pt5;

    },

    FormatCPF: function (value) {

        var cpf_Pt1 = value.substring(0, 3);
        var cpf_Pt2 = value.substring(3, 6);
        var cpf_Pt3 = value.substring(6, 9);
        var cpf_Pt4 = value.substring(9, 11);
        var ponto = ".";
        var traco = "-";
        return cpf_Pt1 + ponto + cpf_Pt2 + ponto + cpf_Pt3 + traco + cpf_Pt4;

    },

    UnFormatNumber: function (value) {
        try {
            var desformated = "";
            while (value.indexOf(".") != -1) {
                value = value.replace('.', '');
            }
            while (value.indexOf("-") != -1) {
                value = value.replace('-', '');
            }
            while (value.indexOf("/") != -1) {
                value = value.replace('/', '');
            }
            while (value.indexOf("\\") != -1) {
                value = value.replace('\\', '');
            }
        }
        catch (e) {
        }
        return value;
    },

    CheckCpf: function (valueWithNoFormat) {

        strcpf = valueWithNoFormat;
        str_aux = "";

        for (i = 0; i <= strcpf.length - 1; i++) {

            if ((strcpf.charAt(i)).match(/\d/))

                str_aux += strcpf.charAt(i);

            else if (!(strcpf.charAt(i)).match(/[\.\-]/)) {

                return false;
            }
        }
        if (str_aux.length != 11) {

            return false;
        }

        soma1 = soma2 = 0;

        for (i = 0; i <= 8; i++) {

            soma1 += str_aux.charAt(i) * (10 - i);
            soma2 += str_aux.charAt(i) * (11 - i);
        }

        d1 = ((soma1 * 10) % 11) % 10;
        d2 = (((soma2 + (d1 * 2)) * 10) % 11) % 10;

        if ((d1 != str_aux.charAt(9)) || (d2 != str_aux.charAt(10))) {

            return false;

        }
        return true;
    },

    CheckCnpj: function (valueWithNoFormat) {

        try {

            if (valueWithNoFormat.length < 14) {

                return false;
            }

            var i;
            var c = valueWithNoFormat.substr(0, 12);
            var dv = valueWithNoFormat.substr(12, 2);
            var d1 = 0;
            for (i = 0; i < 12; i++) {

                d1 += c.charAt(11 - i) * (2 + (i % 8));
            }

            if (d1 == 0) {

                return false;
            };

            d1 = 11 - (d1 % 11);
            if (d1 > 9) {

                d1 = 0;
            }

            if (dv.charAt(0) != d1) {

                return false;
            }

            d1 *= 2;

            for (i = 0; i < 12; i++) {

                d1 += c.charAt(11 - i) * (2 + ((i + 1) % 8));
            }

            d1 = 11 - (d1 % 11);

            if (d1 > 9) {

                d1 = 0;
            }

            if (dv.charAt(1) != d1) {

                return false;
            }

            return true;
        }
        catch (e) {
            return false;
        }
    },

    Alert: function (msg) {
        alert(msg);
    },

    ExecuteWebServiceCall: function (url, xmlEnvelope) {
        // Abre conexão com WebService
        //Execute XmlHttpRequest
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open('POST', url, false);
        xmlHttp.setRequestHeader('Content-Type', 'text/xml; charset=utf-8');
        xmlHttp.send(xmlEnvelope);

        if (AlfaPeople.Scania.Script.Library.ValidateWebServiceAnswer(xmlHttp)) {
            return xmlHttp;
        }
        else {
            return null;
        }
    },

    ValidateWebServiceAnswer: function (xmlHttp) {
        //Validates object XmlHttpResponse
        if (xmlHttp.status == 200) // Sucesso na requisição
        {
            // Verifica se resposta está no formato correto
            if (xmlHttp.responseXML.xml != null &&
            xmlHttp.responseXML.xml.toString().indexOf('an error') == (-1)) {
                return true;
            }
            else {
                // Falha na requisição
                alert('Ocorreu um erro no sistema. Contate a área de suporte.\n\nDescrição do erro: ' +
                'O WebService retornou uma resposta inválida.');
                return false;
            }
        }
        else {
            // Status de requisição inválido
            alert('Ocorreu um erro no sistema. Contate a área de suporte.\n\nDescrição do erro: ' +
            'Houve um erro ao chamar o WebService. Status: ' + xmlHttp.status);
            return false;
        }
    },

    CreateRecord: function (object, type, successCallback, errorCallback) {
        ///<summary>
        /// Sends an asynchronous request to create a new record.
        ///</summary>
        ///<param name="object" type="Object">
        /// A JavaScript object with properties corresponding to the Schema name of
        /// entity attributes that are valid for create operations.
        ///</param>
        this._parameterCheck(object, "SDK.REST.createRecord requires the object parameter.");
        ///<param name="type" type="String">
        /// The Schema Name of the Entity type record to create.
        /// For an Account record, use "Account"
        ///</param>
        this._stringParameterCheck(type, "SDK.REST.createRecord requires the type parameter is a string.");
        ///<param name="successCallback" type="Function">
        /// The function that will be passed through and be called by a successful response. 
        /// This function can accept the returned record as a parameter.
        /// </param>
        this._callbackParameterCheck(successCallback, "SDK.REST.createRecord requires the successCallback is a function.");
        ///<param name="errorCallback" type="Function">
        /// The function that will be passed through and be called by a failed response. 
        /// This function must accept an Error object as a parameter.
        /// </param>
        this._callbackParameterCheck(errorCallback, "SDK.REST.createRecord requires the errorCallback is a function.");
        var req = new XMLHttpRequest();
        req.open("POST", this._ODataPath() + type + "Set", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState == 4 /* complete */) {
                if (this.status == 201) {
                    successCallback(JSON.parse(this.responseText, AlfaPeople.Scania.Script.Library._dateReviver).d);
                }
                else {
                    errorCallback(AlfaPeople.Scania.Script.Library.ErrorHandler(this));
                }
            }
        };
        req.send(JSON.stringify(object));
    },

    UpdateRecord: function (id, object, type, successCallback, errorCallback) {
        ///<summary>
        /// Sends an asynchronous request to update a record.
        ///</summary>
        ///<param name="id" type="String">
        /// A String representing the GUID value for the record to retrieve.
        ///</param>
        this._stringParameterCheck(id, "SDK.REST.updateRecord requires the id parameter.");
        ///<param name="object" type="Object">
        /// A JavaScript object with properties corresponding to the Schema Names for
        /// entity attributes that are valid for update operations.
        ///</param>
        this._parameterCheck(object, "SDK.REST.updateRecord requires the object parameter.");
        ///<param name="type" type="String">
        /// The Schema Name of the Entity type record to retrieve.
        /// For an Account record, use "Account"
        ///</param>
        this._stringParameterCheck(type, "SDK.REST.updateRecord requires the type parameter.");
        ///<param name="successCallback" type="Function">
        /// The function that will be passed through and be called by a successful response. 
        /// Nothing will be returned to this function.
        /// </param>
        this._callbackParameterCheck(successCallback, "SDK.REST.updateRecord requires the successCallback is a function.");
        ///<param name="errorCallback" type="Function">
        /// The function that will be passed through and be called by a failed response. 
        /// This function must accept an Error object as a parameter.
        /// </param>
        this._callbackParameterCheck(errorCallback, "SDK.REST.updateRecord requires the errorCallback is a function.");
        var req = new XMLHttpRequest();

        req.open("POST", this._ODataPath() + type + "Set(guid'" + id + "')", false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("X-HTTP-Method", "MERGE");
        req.onreadystatechange = function () {
            if (this.readyState == 4 /* complete */) {
                if (this.status == 204 || this.status == 1223) {
                    successCallback();
                }
                else {
                    errorCallback(AlfaPeople.Scania.Script.Library.ErrorHandler(this));
                }
            }
        };
        req.send(JSON.stringify(object));
    },

    DeleteRecord: function (id, type, successCallback, errorCallback) {
        ///<summary>
        /// Sends an asynchronous request to delete a record.
        ///</summary>
        ///<param name="id" type="String">
        /// A String representing the GUID value for the record to delete.
        ///</param>
        this._stringParameterCheck(id, "SDK.REST.deleteRecord requires the id parameter.");
        ///<param name="type" type="String">
        /// The Schema Name of the Entity type record to delete.
        /// For an Account record, use "Account"
        ///</param>
        this._stringParameterCheck(type, "SDK.REST.deleteRecord requires the type parameter.");
        ///<param name="successCallback" type="Function">
        /// The function that will be passed through and be called by a successful response. 
        /// Nothing will be returned to this function.
        /// </param>
        this._callbackParameterCheck(successCallback, "SDK.REST.deleteRecord requires the successCallback is a function.");
        ///<param name="errorCallback" type="Function">
        /// The function that will be passed through and be called by a failed response. 
        /// This function must accept an Error object as a parameter.
        /// </param>
        this._callbackParameterCheck(errorCallback, "SDK.REST.deleteRecord requires the errorCallback is a function.");
        var req = new XMLHttpRequest();
        req.open("POST", this._ODataPath() + type + "Set(guid'" + id + "')", true);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("X-HTTP-Method", "DELETE");
        req.onreadystatechange = function () {

            if (this.readyState == 4 /* complete */) {
                if (this.status == 204 || this.status == 1223) {
                    successCallback();
                }
                else {
                    errorCallback(AlfaPeople.Scania.Script.Library.ErrorHandler(this));
                }
            }
        };
        req.send();

    },

    _parameterCheck: function (parameter, message) {
        ///<summary>
        /// Private function used to check whether required parameters are null or undefined
        ///</summary>
        ///<param name="parameter" type="Object">
        /// The parameter to check;
        ///</param>
        ///<param name="message" type="String">
        /// The error message text to include when the error is thrown.
        ///</param>
        if ((typeof parameter === "undefined") || parameter === null) {
            throw new Error(message);
        }
    },

    _stringParameterCheck: function (parameter, message) {
        ///<summary>
        /// Private function used to check whether required parameters are null or undefined
        ///</summary>
        ///<param name="parameter" type="String">
        /// The string parameter to check;
        ///</param>
        ///<param name="message" type="String">
        /// The error message text to include when the error is thrown.
        ///</param>
        if (typeof parameter != "string") {
            throw new Error(message);
        }
    },

    _callbackParameterCheck: function (callbackParameter, message) {
        ///<summary>
        /// Private function used to check whether required callback parameters are functions
        ///</summary>
        ///<param name="callbackParameter" type="Function">
        /// The callback parameter to check;
        ///</param>
        ///<param name="message" type="String">
        /// The error message text to include when the error is thrown.
        ///</param>
        if (typeof callbackParameter != "function") {
            throw new Error(message);
        }
    },

    ErrorHandler: function (error) {
        writeMessage(error.message);
    },

    _ODataPath: function () {
        ///<summary>
        /// Private function to return the path to the REST endpoint.
        ///</summary>
        ///<returns>String</returns>
        return this._getServerUrl() + "/XRMServices/2011/OrganizationData.svc/";
    },

    _getServerUrl: function () {
        ///<summary>
        /// Private function to return the server URL from the context
        ///</summary>
        ///<returns>String</returns>
        var serverUrl = this._context().getServerUrl()
        if (serverUrl.match(/\/$/)) {
            serverUrl = serverUrl.substring(0, serverUrl.length - 1);
        }
        return serverUrl;
    },

    _context: function () {
        ///<summary>
        /// Private function to the context object.
        ///</summary>
        ///<returns>Context</returns>
        if (typeof GetGlobalContext != "undefined")
        { return GetGlobalContext(); }
        else {
            if (typeof Xrm != "undefined") {
                return Xrm.Page.context;
            }
            else { throw new Error("Context is not available."); }
        }
    },

    _dateReviver: function (key, value) {
        ///<summary>
        /// Private function to convert matching string values to Date objects.
        ///</summary>
        ///<param name="key" type="String">
        /// The key used to identify the object property
        ///</param>
        ///<param name="value" type="String">
        /// The string value representing a date
        ///</param>
        var a;
        if (typeof value === 'string') {
            a = /Date\(([-+]?\d+)\)/.exec(value);
            if (a) {
                return new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
            }
        }
        return value;
    },


    UserHasRole: function (roleName) {

        var currentUserRoles = Xrm.Page.context.getUserRoles();

        for (var i = 0; i < currentUserRoles.length; i++) {
            var userRole = currentUserRoles[i];

            var odataSetName = "RoleSet";
            var odataFilter = "?$filter=RoleId eq (guid'" + userRole + "')";
            var odataUri = SERVER_URL + ODATA_ENDPOINT + "/" + odataSetName + odataFilter;

            var Roles = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);

            if (Roles != null) {

                var Role = Roles[0];

                if (Role.Name != null && Role.Name != undefined) {
                    if (AlfaPeople.Scania.Script.Library.GuidsAreEqual(Role.Name, roleName)) {
                        return true;
                    }
                }
            }
        }

        return false;
    },


    SetLookupValue: function (lookupFieldName, entityIdValue, entityNameValue, entityTypeValue) {
        ///<summary>
        /// setLookupValue
        ///</summary>
        ///<param name="lookupFieldName" Type="String">
        /// The attribute lookup name
        ///</param>
        ///<param name="entityIdValue" Type="object">
        /// The value entity id
        ///</param>   
        ///</param>
        ///<param name="entityNameValue" Type="object">
        /// The value entity name
        ///</param> 
        ///<param name="entityTypeValue" Type="object">
        /// The entity type name
        ///</param>   
        if (lookupFieldName != null) {
            var lookupValue = new Array();
            lookupValue[0] = new Object();
            lookupValue[0].id = entityIdValue;
            lookupValue[0].name = entityNameValue;
            lookupValue[0].entityType = entityTypeValue;
            Xrm.Page.getAttribute(lookupFieldName).setValue(lookupValue);
        }
    },

    SetAttributeValue: function (attribute, value) {
        ///<summary>
        /// setValueField
        ///</summary>
        ///<param name="attribute" Type="String">
        /// The attribute name
        ///</param>
        ///<param name="value" Type="object">
        /// The set value attribute
        ///</param> 
        Xrm.Page.getAttribute(attribute).setValue(value);
    },

    SetAttributeDisabled: function (attribute, disabled) {
        ///<summary>
        /// setDisabledField
        ///</summary>
        ///<param name="attribute" Type="String">
        /// The attribute name
        ///</param>
        ///<param name="disabled" Type="Boolean">
        /// The attribute disable
        ///</param>  
        var control = Xrm.Page.ui.controls.get(attribute);
        control.setDisabled(disabled);
    },

    SetAttributeRequired: function (attribute, required) {
        ///<summary>
        /// setRequiredField
        ///</summary>
        ///<param name="attribute" Type="String">
        /// The attribute name
        ///</param>
        ///<param name="required" Type="Boolean">
        /// The set required attribute
        ///</param> 
        var Attribute = Xrm.Page.getAttribute(attribute);
        if (required) {
            Attribute.setRequiredLevel('required');
        } else {
            Attribute.setRequiredLevel('none');
        }
    },

    SetTabVisible: function (tabName, visible) {
        ///<summary>
        /// SetTabVisible
        ///</summary>
        ///<param name="tabName" Type="String">
        /// The tab name
        ///</param>
        ///<param name="visible" Type="Boolean">
        /// The set visible attribute
        ///</param> 
        Xrm.Page.ui.tabs.get(tabName).setSection.setVisible(visible);
    },

    SetSectionVisible: function (sectionName, visible) {
        ///<summary>
        /// SetSectionVisible
        ///</summary>
        ///<param name="sectionName" Type="String">
        /// The tab name
        ///</param>
        ///<param name="visible" Type="Boolean">
        /// The set visible attribute
        ///</param> 
        Xrm.Page.ui.tabs.get(sectionName).setVisible(visible);
    },

    SetAttributeVisible: function (attribute, visible) {
        ///<summary>
        /// SetAttributeVisible
        ///</summary>
        ///<param name="attribute" Type="String">
        /// The tab name
        ///</param>
        ///<param name="visible" Type="Boolean">
        /// The set visible attribute
        ///</param> 
        Xrm.Page.ui.controls.get(attribute).setVisible(visible);
    },

    xmlDthToDate: function (crmDateTimeString) {
        ///<summary>
        /// Converting an XML date string to a JavaScript Date object
        ///</summary>
        ///<param name="crmDateTimeString" Type="String">
        ///</param> 
        var dateTimeParts = crmDateTimeString.split("T");
        var dateString = dateTimeParts[0];
        var timeString = dateTimeParts[1];
        var dateParts = dateString.split("-");
        var timeZoneSeparator = (timeString.indexOf("-") != -1) ? "-" : "+";
        var timeZoneParts = timeString.split(timeZoneSeparator);
        var timeParts = timeZoneParts[0].split(":");

        var date = new Date(AlfaPeople.Scania.Script.Library.SwParseInt(dateParts[0])
                           , AlfaPeople.Scania.Script.Library.SwParseInt(dateParts[1]) - 1
                           , AlfaPeople.Scania.Script.Library.SwParseInt(dateParts[2])
                           , AlfaPeople.Scania.Script.Library.SwParseInt(timeParts[0])
                           , AlfaPeople.Scania.Script.Library.SwParseInt(timeParts[1])
                           , AlfaPeople.Scania.Script.Library.SwParseInt(timeParts[2]));
        return date;
    },

    SwParseInt: function (value) {
        ///<summary>
        /// Parsing Int Date object
        ///</summary>
        ///<param name="value" Type="Date">
        ///</param> 
        if ((value.length == 2) && (value.substr(0, 1) == "0")) {
            value = value.substr(1, 1);
        }
        return parseInt(value);
    },

    SwConvertDateTimeToXml: function (dateTime) {
        ///<summary>
        /// Converting a JavaScript Date object into the proper XML string
        ///</summary>
        ///<param name="dateTime" Type="DateTime">
        ///</param>
        if (dateTime == null) { return null; }

        var offset = (ORG_TIMEZONE_OFFSET < 0) ? -ORG_TIMEZONE_OFFSET : ORG_TIMEZONE_OFFSET;
        var timezoneOffsetHours = Math.floor(offset / 60);
        var timezoneOffsetMinutes = offset - timezoneOffsetHours * 60;

        var s = dateTime.getYear().toString() + "-" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(dateTime.getMonth() + 1) + "-" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(dateTime.getDate()) + "T" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(dateTime.getHours()) + ":" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(dateTime.getMinutes()) + ":" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(dateTime.getSeconds()) +
                ((ORG_TIMEZONE_OFFSET < 0) ? "-" : "+") +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(timezoneOffsetHours) + ":" +
                AlfaPeople.Scania.Script.Library.SwGetFormattedDatePart(timezoneOffsetMinutes);

        return s;
    },

    SwGetFormattedDatePart: function (value) {
        ///<summary>
        /// Format String Date Parts 
        ///</summary>
        ///<param name="value" Type="String">
        ///</param> 
        return (value < 10) ? ("0" + value.toString()) : value.toString();
    },

    SetReadOnlyFields: function (trueOrFalse) {

        var controls = Xrm.Page.ui.controls.get();
        for (var i in controls) {
            try {
                var control = controls[i];

                control.setDisabled(trueOrFalse);
            } catch (err) {
                // alert(err.description);
            }
        }
    },

    GetParameterText: function (keyValue) {
        var USERLCID = Xrm.Page.context.getUserLcid();
        var scrm_key = keyValue + USERLCID;
        var odataSetName = 'scrm_parameterSet';
        var odataFilter = "?$filter=scrm_key eq '" + scrm_key + "'";
        odataFilter += '&$select=scrm_Value';
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);

        if (resultObj != null) {
            return resultObj[0].scrm_Value;
        } else {
            //if the retrieve result is null, the message will be displayed in English
            //LCID in english, 1033.
            USERLCID = "1033";
            return AlfaPeople.Scania.Script.Library.GetParameterText(keyValue);
        }
    },

    GetParameterValue: function (keyValue) {

        var scrm_key = keyValue;
        var odataSetName = 'scrm_parameterSet';
        var odataFilter = "?$filter=scrm_key eq '" + scrm_key + "'";
        odataFilter += '&$select=scrm_Value';
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);

        if (resultObj != null) { return resultObj[0].scrm_Value; }
    },

    RetrieveMultipleRecords: function (type, options, successCallback, errorCallback, OnComplete) {
        ///<summary>
        /// Sends an asynchronous request to retrieve records.
        ///</summary>
        ///<param name="type" type="String">
        /// The Schema Name of the Entity type record to retrieve.
        /// For an Account record, use "Account"
        ///</param>
        this._stringParameterCheck(type, "SDK.REST.retrieveMultipleRecords requires the type parameter is a string.");
        ///<param name="options" type="String">
        /// A String representing the OData System Query Options to control the data returned
        ///</param>
        if (options != null)
            this._stringParameterCheck(options, "SDK.REST.retrieveMultipleRecords requires the options parameter is a string.");
        ///<param name="successCallback" type="Function">
        /// The function that will be passed through and be called for each page of records returned.
        /// Each page is 50 records. If you expect that more than one page of records will be returned,
        /// this function should loop through the results and push the records into an array outside of the function.
        /// Use the OnComplete event handler to know when all the records have been processed.
        /// </param>
        this._callbackParameterCheck(successCallback, "SDK.REST.retrieveMultipleRecords requires the successCallback parameter is a function.");
        ///<param name="errorCallback" type="Function">
        /// The function that will be passed through and be called by a failed response. 
        /// This function must accept an Error object as a parameter.
        /// </param>
        this._callbackParameterCheck(errorCallback, "SDK.REST.retrieveMultipleRecords requires the errorCallback parameter is a function.");
        ///<param name="OnComplete" type="Function">
        /// The function that will be called when all the requested records have been returned.
        /// No parameters are passed to this function.
        /// </param>
        this._callbackParameterCheck(OnComplete, "SDK.REST.retrieveMultipleRecords requires the OnComplete parameter is a function.");
        var optionsString;

        if (options != null) {
            if (options.charAt(0) != "?") {
                optionsString = "?" + options;
            }
            else { optionsString = options; }
        }

        var req = new XMLHttpRequest();
        req.open("GET", this._ODataPath() + type + "Set" + optionsString, false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            //while (this.readyState != 4) { window.}
            if (this.readyState === 4 /* complete */) {
                if (this.status == 200) {
                    var returned = JSON.parse(this.responseText, AlfaPeople.Scania.Script.Library._dateReviver).d;
                    if (RESULTMULTIPLE.length == 0) {
                        RESULTMULTIPLE = JSON.parse(this.responseText, AlfaPeople.Scania.Script.Library._dateReviver).d.results;
                    } else {
                        RESULTMULTIPLE = RESULTMULTIPLE.concat(JSON.parse(this.responseText, AlfaPeople.Scania.Script.Library._dateReviver).d.results);
                    }
                    successCallback(returned.results);
                    if (returned.__next != null) {
                        var queryOptions = returned.__next.substring((AlfaPeople.Scania.Script.Library._ODataPath() + type + "Set").length);
                        AlfaPeople.Scania.Script.Library.RetrieveMultipleRecords(type, queryOptions, successCallback, errorCallback, OnComplete);
                    }
                    else { OnComplete(); }
                }
                else {
                    errorCallback(AlfaPeople.Scania.Script.Library.ErrorHandler(this));
                }
            }
        };
        req.send();
        return RESULTMULTIPLE;
    },

    S4: function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    },

    RandonGUID: function () {
        // then to call it, plus stitch in '4' in the third group
        guid = '{';
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += "-";
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += "-4";
        guid += AlfaPeople.Scania.Script.Library.S4().substr(0, 3);
        guid += "-";
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += "-";
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += AlfaPeople.Scania.Script.Library.S4();
        guid += '}';

        return guid.toLowerCase();
    },

    TxtBoxFormat: function (Atributo, sMask, evtKeyPress) {
        var i, nCount, sValue, fldLen, mskLen, bolMask, sCod, nTecla;

        if (document.all) { // Internet Explorer
            nTecla = evtKeyPress.keyCode;
        } else if (document.layers) { // Nestcape
            nTecla = evtKeyPress.which;
        } else {
            nTecla = evtKeyPress.which;
            if (nTecla == 8) {
                return true;
            }
        }

        if (Xrm.Page.getAttribute(Atributo).getValue() != null) {
            sValue = Xrm.Page.getAttribute(Atributo).getValue();

            // Limpa todos os caracteres de formatação que
            // já estiverem no campo.
            sValue = sValue.toString().replace("-", "");
            sValue = sValue.toString().replace("-", "");
            sValue = sValue.toString().replace(".", "");
            sValue = sValue.toString().replace(".", "");
            sValue = sValue.toString().replace("/", "");
            sValue = sValue.toString().replace("/", "");
            sValue = sValue.toString().replace(":", "");
            sValue = sValue.toString().replace(":", "");
            sValue = sValue.toString().replace("(", "");
            sValue = sValue.toString().replace("(", "");
            sValue = sValue.toString().replace(")", "");
            sValue = sValue.toString().replace(")", "");
            sValue = sValue.toString().replace(" ", "");
            sValue = sValue.toString().replace(" ", "");
            sValue = sValue.toString().replace("+", "");
            fldLen = sValue.length;
            mskLen = sMask.length;

            i = 0;
            nCount = 0;
            sCod = "";
            mskLen = fldLen;

            while (i <= mskLen) {
                bolMask = ((sMask.charAt(i) == "-") || (sMask.charAt(i) == ".") || (sMask.charAt(i) == "/") || (sMask.charAt(i) == ":"));
                bolMask = bolMask || ((sMask.charAt(i) == "(") || (sMask.charAt(i) == ")") || (sMask.charAt(i) == " ") || (sMask.charAt(i) == "+"));

                if (bolMask) {
                    sCod += sMask.charAt(i);
                    mskLen++;
                }
                else {
                    sCod += sValue.charAt(nCount);
                    nCount++;
                }

                i++;
            }

            Xrm.Page.getAttribute(Atributo).setValue(sCod);

            if (nTecla != 8) { // backspace
                if (sMask.charAt(i - 1) == "9") { // apenas números...
                    return ((nTecla > 47) && (nTecla < 58));
                }
                else { // qualquer caracter...
                    return true;
                }
            }
            else {
                return true;
            }
        } else {
            return false;
        }
    },

    TxtBoxFormatOnBlur: function (sValue) {

        if (sValue == null) { return null; }

        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), '-', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), '.', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), '/', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), ':', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), '(', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), ')', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), ' ', '');
        sValue = AlfaPeople.Scania.Script.Library.ReplaceAll(sValue.toString(), '+', '');

        return sValue;
    },

    SetRequiredFieldByValueSelected: function (fieldSelected, setField, valueSelected) {

        var fieldSelected = Xrm.Page.getAttribute(fieldSelected).getValue();
        var setFieldName = setField;
        var setField = Xrm.Page.getAttribute(setField);

        if (fieldSelected == valueSelected) {

            setField.setRequiredLevel('required');
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(setFieldName, false);
        }
        else {
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(setFieldName, true);
            Xrm.Page.data.entity.attributes.get(setFieldName).setValue(null);
            setField.setRequiredLevel('none');
        }
    },

    SetStateOfRecord: function (entityName, entityId, stateCode, statusCode) {
        var URL = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/Organization.svc/web";

        if (Xrm.Page.data.entity.getIsDirty()) {
            alert(AlfaPeople.Scania.Script.Library.GetParameterText("MessageForDisplayOnFormIsDirty_"));
            return;
        }
        // create the request
        var request = "<s:Envelope xmlns:s='http://schemas.xmlsoap.org/soap/envelope/'>";
        request += "<s:Body>";
        request += "<Execute xmlns='http://schemas.microsoft.com/xrm/2011/Contracts/Services' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'>";
        request += "<request i:type='b:SetStateRequest' xmlns:a='http://schemas.microsoft.com/xrm/2011/Contracts' xmlns:b='http://schemas.microsoft.com/crm/2011/Contracts'>";
        request += "<a:Parameters xmlns:c='http://schemas.datacontract.org/2004/07/System.Collections.Generic'>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<c:key>EntityMoniker</c:key>";
        request += "<c:value i:type='a:EntityReference'>";
        request += "<a:Id>" + entityId + "</a:Id>";
        request += "<a:LogicalName>" + entityName + "</a:LogicalName>";
        request += "<a:Name i:nil='true' />";
        request += "</c:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<c:key>State</c:key>";
        request += "<c:value i:type='a:OptionSetValue'>";
        request += "<a:Value>" + stateCode + "</a:Value>";
        request += "</c:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<c:key>Status</c:key>";
        request += "<c:value i:type='a:OptionSetValue'>";
        request += "<a:Value>" + statusCode + "</a:Value>";
        request += "</c:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "</a:Parameters>";
        request += "<a:RequestId i:nil='true' />";
        request += "<a:RequestName>SetState</a:RequestName>";
        request += "</request>";
        request += "</Execute>";
        request += "</s:Body>";
        request += "</s:Envelope>";

        //send set state request
        $.ajax({
            type: "POST",
            contentType: "text/xml; charset=utf-8",
            datatype: "xml",
            url: URL,
            data: request,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/xml, text/xml, */*");
                XMLHttpRequest.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                //Xrm.Page.ui.close();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        });
    },

    ParseJsonDate: function (jsonDate) {
        if (jsonDate == null) { return null; }

        var offset = new Date().getTimezoneOffset() * 60000;
        var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);

        if (parts[2] == undefined)
            parts[2] = 0;

        if (parts[3] == undefined)
            parts[3] = 0;

        return new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
    },

    GetUser: function (idUser) {
        var odataSetName = "SystemUserSet";
        var odataFilter = "?$filter=SystemUserId eq (guid'" + idUser + "')";
        var odataUri = SERVER_URL + ODATA_ENDPOINT + "/" + odataSetName + odataFilter;
        var result = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);
        if (result.length == 0) {
            return null;
        }
        else {
            return result[0];
        }
    },

    GuidsAreEqual: function (guid1, guid2) {
        var isEqual = false;

        if (guid1 == null || guid2 == null) {
            isEqual = false;
        }
        else {
            isEqual = guid1.toLowerCase() == guid2.toLowerCase();
        }

        return isEqual;
    },


    GetBusinessUnit: function (guidDealer) {

        var odataSetName = 'BusinessUnitSet';
        var odataFilter = "?$filter=BusinessUnitId eq (guid'" + guidDealer + "')";
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);

        if (resultObj != null) { return resultObj[0]; } else { return null; }
    },

    ReplaceAll: function (string, token, newtoken) {
        while (string.indexOf(token) != -1) {
            string = string.replace(token, newtoken);
        }
        return string;
    },

    ColorizeOptionSet: function (field, option, color) {

        document.getElementById(field).options[option].style.backgroundColor = color;
    },

    ColorizeField: function (field, color) {

        document.getElementById(field).style.backgroundColor = color;
    },

    DateWithin: function (beginDate, endDate, checkDate) {
        //var b = Date.parse(beginDate);
        var b = beginDate.setHours(0);
        //var e = Date.parse(endDate);
        var e = endDate.setHours(0);
        //var c = Date.parse(checkDate);
        var c = checkDate.setHours(0);
        if ((c <= e && c >= b)) {
            return true;
        }
        return false;
    },

    OpenReport: function (reportName) {

        var odataSetName = 'ReportSet';
        var odataFilter = "Name eq '" + reportName + "'";
        odataFilter += '&$ReportId';
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);
        if (resultObj != null) {
            var id = Xrm.Page.data.entity.getId();
            var etc = Xrm.Page.context.getQueryStringParameters().etc;
            var callReportId = id.replace('{', ").replace('}', ");
            var reportId = data[0].ReportId.replace('{', ").replace('}', ");
            var url = SERVER_URL + "/crmreports/viewer/viewer.aspx?action=run&context=records&helpID=" + reportName + ".rdl&id=%7b" + reportId + "%7d&records=%7b" + callReportId + "%7d&recordstype=" + etc;
            window.open(url, "reportwindow", "resizable=1,width=950,height=700");
        }
    },

    SetAttributeFocus: function (attributeName) {
        ///<summary>
        /// SetAttributeFocus
        ///</summary>
        ///<param name="attributeName" Type="String">
        /// The attribute name
        ///</param> 
        Xrm.Page.getControl(attributeName).setFocus(true);
        clearTimeout(setFocusWaitTime)
    },

    ProductTypeOnChange: function (productType, category, brand, model, categoryFieldName, brandFieldName, modelFieldName) {
        Xrm.Page.getAttribute(categoryFieldName).setValue(null);
        Xrm.Page.getAttribute(brandFieldName).setValue(null);
        Xrm.Page.getAttribute(modelFieldName).setValue(null);
        if (productType.getValue() != null) {
            // Habilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(categoryFieldName, false);
            // Desabilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(modelFieldName, true);
        } else {
            // Desabilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(categoryFieldName, true);
            // Habilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(modelFieldName, false);
            // Apaga o valor do Campo abaixo
            Xrm.Page.getAttribute(categoryFieldName).setValue(null);
        }
        category.fireOnChange();
    },

    CategoryOnChange: function (category, brand, model, brandFieldName, modelFieldName) {
        Xrm.Page.getAttribute(brandFieldName).setValue(null);
        Xrm.Page.getAttribute(modelFieldName).setValue(null);
        if (category.getValue() != null) {
            // Habilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(brandFieldName, false);
        } else {
            // Desabilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(brandFieldName, true);
            // Apaga o valor do Campo abaixo
            Xrm.Page.getAttribute(brandFieldName).setValue(null);
        }
        brand.fireOnChange();

    },

    BrandOnChange: function (productType, brand, model, modelFieldName) {
        Xrm.Page.getAttribute(modelFieldName).setValue(null);
        if (brand.getValue() != null) {
            // Habilita Campos
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(modelFieldName, false);
        } else {
            if (model.getValue() == null && productType.getValue() != null) {
                // Desabilita Campos
                AlfaPeople.Scania.Script.Library.SetAttributeDisabled(modelFieldName, true);
            }
            // Apaga o valor do Campo abaixo
            Xrm.Page.getAttribute(modelFieldName).setValue(null);
        }
        model.fireOnChange();
    },

    ModelOnChange: function (productType, category, brand, model, brandFieldName, categoryFieldName, productTypeFieldName, tractionFieldName, enginePowerFieldName, adaptationOfTheChassisFieldName, modelFieldName, defaultViewId) {

        if (model.getValue() != null) {
            //First Of All, the model type will be searched in the Model
            var modelReturned = AlfaPeople.Scania.Script.Library.RetrieveRecord(model.getValue()[0].id, "scrm_model", "scrm_ProductMakerId, scrm_Category, scrm_ProductType, scrm_Traction, scrm_Powerengine, scrm_AdaptationofChassis");
            //The Model Details will be informed on attributes in Form
            AlfaPeople.Scania.Script.Library.SetLookupValue(brandFieldName, modelReturned.scrm_ProductMakerId.Id, modelReturned.scrm_ProductMakerId.Name, modelReturned.scrm_ProductMakerId.LogicalName);
            AlfaPeople.Scania.Script.Library.SetLookupValue(categoryFieldName, modelReturned.scrm_Category.Id, modelReturned.scrm_Category.Name, modelReturned.scrm_Category.LogicalName);
            AlfaPeople.Scania.Script.Library.SetLookupValue(productTypeFieldName, modelReturned.scrm_ProductType.Id, modelReturned.scrm_ProductType.Name, modelReturned.scrm_ProductType.LogicalName);
            Xrm.Page.getAttribute(tractionFieldName).setValue(modelReturned.scrm_Traction);
            Xrm.Page.getAttribute(enginePowerFieldName).setValue(modelReturned.scrm_Powerengine);
            Xrm.Page.getAttribute(adaptationOfTheChassisFieldName).setValue(modelReturned.scrm_AdaptationofChassis.Value);
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(productTypeFieldName, false);
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(categoryFieldName, false);
            AlfaPeople.Scania.Script.Library.SetAttributeDisabled(brandFieldName, false);
        } else {
            Xrm.Page.getAttribute(tractionFieldName).setValue(null);
            Xrm.Page.getAttribute(enginePowerFieldName).setValue(null);
            Xrm.Page.getAttribute(adaptationOfTheChassisFieldName).setValue(null);
            //AlfaPeople.Scania.Script.Fleet.ViewOnLoad('scrm_producttype', 'scrm_categoryid', 'scrm_productmakerid', 'scrm_modelid', defaultViewId);
        }
        AlfaPeople.Scania.Script.Library.SetModelLookup(productTypeFieldName, categoryFieldName, brandFieldName, modelFieldName, false, defaultViewId);
    },

    SetModelLookup: function (producttypeFieldName, categoryFieldName, productmakerFieldName, lookupFieldName, resetSelection, defaultViewId) {

        // Get the selected Model Id in the indicated controls  
        var producttype = Xrm.Page.getAttribute('producttypeFieldName').getValue();
        var category = Xrm.Page.getAttribute('categoryFieldName').getValue();
        var productmaker = Xrm.Page.getAttribute('productmakerFieldName').getValue();

        if (producttype != null && category != null && productmaker != null) {

            var producttypeid = producttype[0].id;
            var producttypename = producttype[0].name;

            var categoryid = category[0].id;
            var categoryname = category[0].name;

            var productmakerid = productmaker[0].id;
            var productmakername = productmaker[0].name;

            //inicio
            var ViewColum = Scania.Filtros.CustomView.MakeStruct("SchemaName Width");
            var customViewId = "{" + Scania.Filtros.CustomView.Guid() + "}";
            var customViewName = "ModelViewName_";
            var lookupFieldName = "scrm_modelid";
            var entityName = "scrm_model";
            var primaryKeyName = "scrm_modelid";
            var primaryFieldName = "scrm_name";
            var orderBy = "scrm_name";
            var viewColumns = [new ViewColumn("scrm_name", 250)];
            var meuFiltro = "";


            meuFiltro += "<link-entity name='' from='scrm_model' to='scrm_model' visible='false' intersect='true'>";
            meuFiltro += "<filter type='and'>";
            meuFiltro += "<condition attribute='scrm_producttype' operator='eq' uitype='scrm_producttype' value='" + producttypeid[0].id + "' />";
            meuFiltro += "<condition attribute='scrm_category' operator='eq' uitype='scrm_category' value='" + categoryid[0].id + "' />";
            meuFiltro += "<condition attribute='scrm_productmakerid' operator='eq'uitype='scrm_productmaker' value='" + productmakerid[0].id + "' />";
            meuFiltro += "<condition attribute='statecode' operator='eq' uitype='contact' value='0' />";
            meuFiltro += "</filter>";
            meuFiltro += "</link-entity>";
            meuFiltro += "</link-entity>";

            Scania.Filtros.CustomView.FilterGlobal(customViewId, customViewName, lookupFieldName, entityName, primaryKeyName, primaryFieldName, orderBy, null, viewColumns, meuFiltro, onload);

        }
    },

    ConfigureAjaxLoading: function (text) {
        $('body').append('<div id="loadingDiv"></div>');
        $('#loadingDiv').append('<p id="loadingText">' + text + '</p>')
                    .css('background', 'url(/_imgs/AdvFind/progress.gif) no-repeat center')
                    .css('background-color', '#CCCCCC')
                    .css('height', '100px')
                    .css('width', '300px')
        //.center()
                    .hide()  // hide it initially 
                    .ajaxStart(function () {
                        $(this).show();
                    })
                    .ajaxStop(function () {
                        $(this).hide();
                    });

        $('#loadingText').css('text-align', 'center')
                     .css('font', '20px bolder')
                     .css('font-family', 'Segoe UI, Tahoma, Arial');
    },

    UserHasRoleOld: function (roleName) {
        var odataSetName = 'RoleSet';
        //var odataFilter = "?$top=1&$filter=Name eq '" + roleName + "'";
        var odataFilter = "?$filter=Name eq '" + roleName + "'";
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);

        if (resultObj != null && resultObj.length >= 1) {

            for (var j = 0; j < resultObj.length; j++) {
                var role = resultObj[j];
                var id = role.RoleId;
                var currentUserRoles = Xrm.Page.context.getUserRoles();
                for (var i = 0; i < currentUserRoles.length; i++) {
                    var userRole = currentUserRoles[i];
                    if (AlfaPeople.Scania.Script.Library.GuidsAreEqual(userRole, id)) {
                        return true;
                    }
                }
            }
        }

        return false;
    },

    GuidsAreEqual: function (guid1, guid2) {
        var isEqual = false;

        if (guid1 == null || guid2 == null) {
            isEqual = false;
        }
        else {
            isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
        }

        return isEqual;
    },

    UserHasRole: function (roleName) {
        var currentUserRoles = Xrm.Page.context.getUserRoles();
        //alert('Total: ' + currentUserRoles.length);
        for (var i = 0; i < currentUserRoles.length; i++) {
            //alert('Record:' + (i+1))
            var userRole = currentUserRoles[i];
            if (AlfaPeople.Scania.Script.Library.RolesEqual(userRole, roleName)) {
                return true;
            }
        }

        return false;
    },


    RetrieveRecordRole: function (odataUri) {
        var result;
        //Asynchronous AJAX function to Retrieve CRM records using OData
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            url: odataUri,
            async: false,
            beforeSend: function (XMLHttpRequest) {
                //Specifying this header ensures that the results will be returned as JSON.             
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                if (data.d != null || data.d != undefined) {
                    result = data.d;
                }
                else {

                    result = null;
                }
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {

                result = "An error ocurred for dynamic search record.\n" +
                     "Please contact the Administrator and inform this message.\n " +
                     "Error : " + textStatus + ": " + XmlHttpRequest.statusText + " - errorThrow " + errorThrown;

                result = null;
            }
        });

        return result;
    },

    RolesEqual: function (userRole, roleName) {
        var isEqual = false;

        var odataSetName = 'RoleSet';
        var odataFilter = "(guid'" + userRole + "')";
        var odataUri = SERVER_URL + ODATA_ENDPOINT + '/' + odataSetName + odataFilter;
        //var resultObj = AlfaPeople.Scania.Script.Library.DynamicSearchRecord(odataUri);
        var resultObj = AlfaPeople.Scania.Script.Library.RetrieveRecordRole(odataUri);

        if (resultObj != null) {
            var RoleNameResult = resultObj.Name.toString().toLowerCase().trim();
            var RoleGrantAccess = roleName.toString().toLowerCase().trim();
            isEqual = (RoleNameResult == RoleGrantAccess)
        }

        return isEqual;
    }

    //HideRibbonButtonEnableRole: function () {
    //    var Retorno = false;

    //    var JScriptWebResourceUrl = "../WebResources/scrm_jquery1.4.1.min";
    //    var xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
    //    xmlHttp.open("GET", JScriptWebResourceUrl, false);
    //    xmlHttp.send();
    //    eval(xmlHttp.responseText);

    //    if (AlfaPeople.Scania.Script.Library.UserHasRole("Scania Sales Manager")
    //    || AlfaPeople.Scania.Script.Library.UserHasRole("Dealer Sales Manager")
    //    || AlfaPeople.Scania.Script.Library.UserHasRole("System Administrator")) {
    //        Retorno = true;
    //    }

    //    return Retorno;
    //}
};