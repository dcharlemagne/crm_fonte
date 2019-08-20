if (typeof (FornecedorCanal) == "undefined") { FornecedorCanal = {}; }

FornecedorCanal = {

    OnLoad: function () {
        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;

            case 1: //Create
                break;

            case 2: //Update
                
                break;
        }
    },


    itbc_telefone_onchange: function () {
        Util.funcao.Mascara("itbc_telefone", "(00)-0000-0000");
    }
}