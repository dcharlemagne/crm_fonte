if (typeof (Beneficio) == "undefined") { Beneficio = {}; }

Beneficio = {

    ativo: Xrm.Page.getAttribute('itbc_beneficioativo').getValue(),

    OnLoad: function () {

    },

    BeneficioAtivo: function () {
        Xrm.Utility.confirmDialog("Ao Ativar/Desativar este Beneficio, \n todos os Beneficios dos Canais serão Ativados/Desativados \n deseja continuar?",

        function () {
            Beneficio.ativo = Xrm.Page.getAttribute('itbc_beneficioativo').getValue();
            Xrm.Page.data.entity.save();
        },
        function () {
            Xrm.Page.getAttribute("itbc_beneficioativo").setValue(Beneficio.ativo);

        });
    },

    OnSave: function (context) {

    }
}