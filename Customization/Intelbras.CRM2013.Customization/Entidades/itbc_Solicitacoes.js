if (typeof (Solicitacoes) == "undefined") { Solicitacoes = {}; }

Solicitacoes = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;

            case 1: //Create
                debugger;
                var xrmObject = Xrm.Page.context.getQueryStringParameters();

                if (xrmObject["itbc_tipodesolicitacaoidname"] != null && xrmObject["itbc_tipodesolicitacaoid"] != null && xrmObject["itbc_tipodesolicitacaoidtype"] != null) {

                    var lookupData = new Array();
                    lookupData[0] = new Object();
                    lookupData[0].id =  xrmObject["itbc_tipodesolicitacaoid"].toString();
                    lookupData[0].name = xrmObject["itbc_tipodesolicitacaoidname"].toString();
                    lookupData[0].entityType = xrmObject["itbc_tipodesolicitacaoidtype"].toString();
                    Xrm.Page.getAttribute("itbc_tipodesolicitacaoid").setValue(lookupData);
                }
                break;

            case 2: //Update
                Util.funcao.formdisable(true);
                break;
        }
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },


    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        Solicitacoes.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_necessidade,itbc_tipodesolicitacaoid", " - "));

        Solicitacoes.DisabledEnabled(true);
        Solicitacoes.ForceFieldSave();
    }


}