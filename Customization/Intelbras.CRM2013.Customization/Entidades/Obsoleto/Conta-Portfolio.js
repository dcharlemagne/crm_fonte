if (typeof (Conta_port) == "undefined") { Conta_port = {}; }

Conta_port = {
    OnLoad: function () {
        if (Xrm.Page.data.entity.getId() != null && document.getElementById("IFRAME_PortfolioePrecos"))
            Xrm.Page.ui.controls.get("IFRAME_PortfolioePrecos").setSrc("/ISV/Web/BuscaProdutosPortfolio.aspx?id=" + Xrm.Page.data.entity.getId());
    }
}