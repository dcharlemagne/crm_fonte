function Form_onsave()
{
//Copiar o campo Pergunta Extendia para Pergunta
 Xrm.Page.getAttribute("new_pergunta").setValue(Xrm.Page.getAttribute("new_pergunta_extendida").getValue());
}
function Form_onload()
{
//Esconder pasta Hide
Xrm.Page.ui.tabs.get(1).setVisible(false);
}
