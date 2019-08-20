/// <reference path="jquery-1.4.1.min.js"/>
/// <reference path="json2.js"/>

var GuidEmpty = "00000000-0000-0000-0000-000000000000";

String.format = function (formato) {
    for (var i = 1; i <= arguments.length; i++) {
        var argument = arguments[i];

        if (argument == null)
            argument = "";

        formato = formato.replace("{" + (i - 1) + "}", argument.toString());
    }

    return formato;
};

function CallPageMethod(methodName, parametros, callback) {
    var host = window.location.host;
    var pathName = window.location.pathname

    if (!host.endsWith("/"))
        host += "/";

    if (pathName.startsWith("/"))
        pathName = pathName.substring(1, pathName.length);

    $.ajax({
        type: "POST",
        async: true,
        url: String.format("{0}//{1}{2}/{3}", window.location.protocol, host, pathName, methodName),
        data: JSON.stringify(parametros),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (obj) {
            if (obj == null)
                callback(true, obj);
            else
                callback(true, obj.d);
        },
        error: function (msg) {
            var obj = JSON.parse(msg.responseText);
            alert(obj.Message);
            callback(false, null);
        }
    });
}
String.prototype.endsWith = function (str, ignoreCase) {
    var valor = this;
    var tam = str.length;

    if (ignoreCase == true) {
        valor = this.toLowerCase();
        str = this.toLocaleLowerCase();
    }

    var inicio = valor.length - tam;

    if (inicio < 0) {
        return valor == str;
    }

    return valor.substr(valor.length - tam, tam) == str;
}

String.prototype.startsWith = function (str, ignoreCase) {
    var valor = this;
    var tam = str.length;

    if (ignoreCase == true) {
        valor = this.toLowerCase();
        str = this.toLocaleLowerCase();
    }

    return valor.substr(0, tam) == str;
}

jQuery.fn.extend
({
    carregando: function () {
        var obj = $(this);

        switch (obj[0].tagName) {
            case "TABLE":
                obj.find("tbody").empty().html("<tr class='carregando'><td colspan='10'></td></tr>");
                break;
            case "TBODY":
                obj.html("<tr class='carregando'><td colspan='10'></td></tr>");
                break;
        }
    },
    carregado: function () {
        var obj = $(this);

        switch (obj[0].tagName) {
            case "TABLE":
            case "TBODY":
                $(this).find(".carregando").remove();
                break;
        }
    }
});