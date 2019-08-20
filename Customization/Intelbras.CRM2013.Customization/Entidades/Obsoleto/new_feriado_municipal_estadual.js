function new_cidade_onchange() {
    if (Xrm.Page.getAttribute("new_cidade").getValue() == null)

        Xrm.Page.getAttribute("new_uf").setRequiredLevel("none");

    else

        Xrm.Page.getAttribute("new_uf").setRequiredLevel("required");
}