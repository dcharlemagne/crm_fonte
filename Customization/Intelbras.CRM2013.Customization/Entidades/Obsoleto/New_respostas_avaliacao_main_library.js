function Form_onload()
{
//Esconde Guia hide
Xrm.Page.ui.tabs.get(1).setVisible(false);
}
function new_respostaid_onchange()
{
//*Copiar a Resposta para um campo Nome*//
if (Xrm.Page.getAttribute("new_respostaid").getValue() != null)
{
    Xrm.Page.getAttribute("new_resposta").setValue(Xrm.Page.getAttribute("new_respostaid").getValue()[0].name);
}
}
