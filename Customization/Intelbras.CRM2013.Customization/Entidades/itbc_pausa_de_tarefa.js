var FormularioEstaEmModoDeCriacao = Xrm.Page.ui.getFormType() == 1;
var FormularioEstaSendoAlterado = Xrm.Page.ui.getFormType() == 2;

function AoCarregar() {

    if (FormularioEstaEmModoDeCriacao) {
        if (Xrm.Page.getAttribute("itbc_data_de_incio") != null) {
            Xrm.Page.getAttribute("itbc_data_de_incio").setValue(new Date());
        }
    }

}

function AoSalvar() {

    if (FormularioEstaEmModoDeCriacao) {
        //criando

    } else {
        //verifica se o campo foi alterado
        if (!Util.Xrm.HasValue("itbc_data_de_termino"))
            Xrm.Page.getAttribute("itbc_data_de_termino").setValue(new Date());
    }

}

function AoAlterarDataTermino() {
    debugger;
    if (Xrm.Page.getAttribute("itbc_data_de_termino") != null) {
        var dataLimite1 = new Date();
        dataLimite1.setMinutes(new Date().getMinutes() - 1);
        if (Xrm.Page.getAttribute("itbc_data_de_termino").getValue() < (dataLimite1)) {
            alert("A Data de Término não pode ser menor que a data atual.");
            Xrm.Page.getAttribute("itbc_data_de_termino").setValue("");
        } else {
            var dataLimite2 = new Date();
            dataLimite2.setMinutes(new Date().getMinutes() + 5);

            if (Xrm.Page.getAttribute("itbc_data_de_termino").getValue() > (dataLimite2)) {
                alert("A Data de Término não pode ser maior que a data atual.");
                Xrm.Page.getAttribute("itbc_data_de_termino").setValue("");
            }
        }
    }
}