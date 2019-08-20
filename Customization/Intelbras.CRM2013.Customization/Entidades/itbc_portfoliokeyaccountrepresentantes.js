if (typeof (PortfolioKeyRepresentante) == "undefined") { PortfolioKeyRepresentante = {}; }

PortfolioKeyRepresentante = {

    OnLoad: function () {

        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
        Xrm.Page.ui.controls.get("itbc_name").setVisible(false);
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    ConcatenarNome: function () {

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_contatoid,itbc_unidadedenegocioid,itbc_segmentoid,itbc_supervisordevendas,itbc_assistentedeadministracaodevendas", " - ").substring(1, 99));

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

        PortfolioKeyRepresentante.ConcatenarNome();
        PortfolioKeyRepresentante.DisabledEnabled(false);

        PortfolioKeyRepresentante.DisabledEnabled(true);
        PortfolioKeyRepresentante.ForceFieldSave();
    }

}