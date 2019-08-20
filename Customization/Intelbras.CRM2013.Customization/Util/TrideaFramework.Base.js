if (typeof (Tridea) == "undefined") { Tridea = {}; }

Tridea.Framework = {
    CallService: function (url, method) {
        /// <summary>Executa chamada ao método do WCF informado.</summary>
        /// <param name="url" type="string">A url do WCF.</param>
        /// <param name="method" type="string">O nome do método a ser executado.</param>
        /// <returns type="Objeto">O retorno oferecido pelo método executado.</returns>

        var servico = this;
        var parameter = "";
        var obj = new Object();
        obj.Success = true;
        obj.ReturnValue = null;

        servico.SetParameter = function (name, value) {
            parameter += '"' + name + '": "' + value + '",';
        }

        servico.Execute = function () {
            $.ajax({
                async: false,
                type: "POST",
                url: url + "/" + method,
                data: configureParameter(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processdata: true,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    obj.ReturnValue = data;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Falha na chamada do método:" + method);
                    obj.Success = false;
                }
            });
            return configureReturn(obj);
        }

        function configureReturn() {
            if (obj.ReturnValue == undefined || obj.ReturnValue == null)
                return obj;

            if (obj.ReturnValue.hasOwnProperty("d"))
                obj.ReturnValue = obj.ReturnValue.d;

            return obj;
        }

        function configureParameter() {
            parameter = parameter.length == 0 ? "," : parameter;
            return "{" + parameter.substr(0, parameter.length - 1) + "}";
        }
    },

    CreateLookup: function (id, name, type) {
        /// <summary>Cria objeto de Lookup.</summary>
        /// <param name="id" type="string">O guid do lookup a ser criado.</param>
        /// <param name="name" type="string">O nome do registro do lookup a ser criado.</param>
        /// <param name="type" type="string">O tipo (logical name) do objeto a ser criado.</param>
        /// <returns type="Objeto">Objeto Lookup.</returns>

        var value = new Array();
        value[0] = new Object();
        value[0].id = id;
        value[0].name = name;
        value[0].entityType = type;

        return value;
    },

    DisableAllFormFields: function (onOff) {
        /// <summary>Desabilita ou Habilita todos os campos do formulário.</summary>
        /// <param name="onOff" type="bool">'true' para desabilitar, 'false' para habilitar.</param>

        Xrm.Page.ui.controls.forEach(function (control, index) {
            if (Tridea.Framework.VerificaSeControleTemAtributos(control)) {
                control.setDisabled(onOff);
            }
        });
    },

    VerificaSeControleTemAtributos: function (controle) {
        var tipoDeControle = controle.getControlType();
        return tipoDeControle != "iframe" && tipoDeControle != "webresource" && tipoDeControle != "subgrid";
    },

    DesabilitarCampos: function (atributos, bloqueado) {
        /// <summary>Desabilita ou Habilita o campo do formulário.</summary>
        /// <param name="atributos" type="array">recebe um array com o nome dos atributos</param>
        /// <param name="bloqueado" type="bool">'true' para bloquear , 'false' para desbloquear.</param>
        for (var i = 0; i < atributos.length; i++) {
            if (bloqueado)
                Xrm.Page.ui.controls.get(atributos[i]).setDisabled(true);
            else
                Xrm.Page.ui.controls.get(atributos[i]).setDisabled(false);
        }
    }
}

if (typeof (Tridea.Framework) == "undefined") { Tridea.Framework = {}; }

Tridea.Framework.OData = {
    /* 
    * Os métodos OData utilizados foram adaptados de jqueryrestdataoperationactions.js 
    * Francisco Avelino
    */

    Retrieve: function (id, nomeEntidade) {
        /// <summary>Obtém registro do CRM.</summary>
        /// <param name="id" type="string">O guid do registro a ser obtido.</param>
        /// <param name="nomeEntidade" type="string">O tipo (schema name) do registro a ser obtido.</param>

        if (!id) {
            alert("Por favor, informe o Id.");
            return;
        }

        if (!nomeEntidade) {
            alert("Por favor, informe o Nome da Entidade.");
            return;
        }

        var returnObject;
        var serverAddress = Xrm.Page.context.getServerUrl();
        var serverUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + "XRMServices/2011/OrganizationData.svc" + "/" + nomeEntidade + "Set(guid'" + id + "')";
        $.support.cors = true;
        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            url: serverUrl,
            beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
            success: function (data, textStatus, XmlHttpRequest) {
                returnObject = data.d;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                Tridea.Framework.OData.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
            }
        });

        return returnObject;
    },

    RetrieveMultiple: function (nomeEntidade, filtro) {
        /// <summary>Obtém lista de registros do CRM.</summary>
        /// <param name="nomeEntidade" type="string">O tipo (logical name) dos registros a serem obtidos.</param>
        /// <param name="filtro" type="string">O filtro OData a ser executado para obter os registros.</param>

        if (!nomeEntidade) {
            alert("Por favor, informe o Nome da Entidade.");
            return;
        }

        var returnObject;
        //var crmserver = Xrm.Page.context.getServerUrl();
        //var dominioURL = crmserver.substring((parseInt(crmserver.indexOf("/")) + 2), crmserver.indexOf("."));

        var serverAddress = Xrm.Page.context.getServerUrl();
        var serverUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + "XRMServices/2011/OrganizationData.svc" + "/" + nomeEntidade + "Set";
        if (filtro) serverUrl += "?$filter=" + filtro;

        $.ajax({
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            url: serverUrl,
            beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
            success: function (data, textStatus, XmlHttpRequest) {
                if (data && data.d && data.d.results)
                    returnObject = data.d.results;
                else if (data && data.d)
                    returnObject = data.d;
                else
                    returnObject = data;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                Tridea.Framework.OData.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
            }
        });

        return returnObject;
    },

    Create: function (entidade, nomeEntidade) {
        /// <summary>Cria registro no CRM com o serviço de OData.</summary>
        /// <param name="entidade" type="object">Objeto a ser criado no CRM.</param>
        /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser criado.</param>

        if (!entidade) {
            alert("Por favor, informe a Entidade.");
            return;
        }

        if (!nomeEntidade) {
            alert("Por favor, informe o Nome da Entidade.");
            return;
        }

        var returnObject;
        var jsonEntity = window.JSON.stringify(entidade);
        var serverAddress = Xrm.Page.context.getServerUrl();
        var serverUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + "XRMServices/2011/OrganizationData.svc" + "/" + nomeEntidade + "Set";

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            url: serverUrl,
            data: jsonEntity,
            beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
            success: function (data, textStatus, XmlHttpRequest) {
                returnObject = data.d;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                Tridea.Framework.OData.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
            }
        });

        return returnObject;
    },

    Update: function (id, entidade, nomeEntidade) {
        /// <summary>Atualiza registro no CRM.</summary>
        /// <param name="id" type="string">O guid do registro a ser atualizado.</param>
        /// <param name="entidade" type="object">Objeto a ser atualizado no CRM.</param>
        /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser atualizado.</param>

        if (!id) {
            alert("Por favor, informe o Id.");
            return;
        }

        if (!entidade) {
            alert("Por favor, informe a Entidade.");
            return;
        }

        if (!nomeEntidade) {
            alert("Por favor, informe o Nome da Entidade.");
            return;
        }

        var returnObject;
        var jsonEntity = window.JSON.stringify(entidade);
        var serverAddress = Xrm.Page.context.getServerUrl();
        var serverUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + "XRMServices/2011/OrganizationData.svc" + "/" + nomeEntidade + "Set(guid'" + id + "')";

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            data: jsonEntity,
            url: serverUrl,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                data = new Object();
                data.id = id;
                returnObject = data;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                Tridea.Framework.OData.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
            }
        });

        return returnObject;
    },

    Delete: function (id, nomeEntidade) {
        /// <summary>Remove registro do CRM.</summary>
        /// <param name="id" type="string">O guid do registro a ser removido.</param>
        /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser removido.</param>

        if (!id) {
            alert("Por favor, informe o Id.");
            return;
        }

        if (!nomeEntidade) {
            alert("Por favor, informe a Entidade.");
            return;
        }

        var returnObject;
        var serverAddress = Xrm.Page.context.getServerUrl();
        var serverUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + "XRMServices/2011/OrganizationData.svc" + "/" + nomeEntidade + "Set(guid'" + id + "')";

        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            async: false,
            url: serverUrl,
            beforeSend: function (XMLHttpRequest) {
                XMLHttpRequest.setRequestHeader("Accept", "application/json");
                XMLHttpRequest.setRequestHeader("X-HTTP-Method", "DELETE");
            },
            success: function (data, textStatus, XmlHttpRequest) {
                returnObject = data.d;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                Tridea.Framework.OData.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
            }
        });

        return returnObject;
    },

    ErrorHandler: function (xmlHttpRequest, textStatus, errorThrow) {
        alert("Operação não realizada. Ocorreu o seguinte erro: \n" + textStatus + ": " + xmlHttpRequest.statusText);
    }
}