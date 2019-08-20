function Form_onload() {
    if (typeof (Intelbras) == "undefined") { Intelbras = {}; }

    Intelbras.OnLoad = {
        Inicializar: function () {
            //forçar o submit de todos os campos necessários para validação da vigência


        },

        AdicionarNome: function () {
            var produto = Xrm.Page.getAttribute("new_produtoid").getValue();
            if (produto == null)
                Xrm.Page.getAttribute("new_name").setValue(null);
            else
                Xrm.Page.getAttribute("new_name").setValue(produto[0].name);
        }
    }

    Intelbras.OnLoad.Inicializar();
}
function Form_onsave() {
    Intelbras.OnLoad.ForcarSalvamento();
}
function new_produtoid_onchange() {
    Intelbras.OnLoad.AdicionarNome();
}