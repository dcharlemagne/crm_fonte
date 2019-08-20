if (typeof (Oportunidade) == "undefined") { Oportunidade = {}; }

Oportunidade = {

    GenetecOnChange: function () {
        Xrm.Page.getAttribute("itbc_valor_genetec").setValue(null);
    },
}