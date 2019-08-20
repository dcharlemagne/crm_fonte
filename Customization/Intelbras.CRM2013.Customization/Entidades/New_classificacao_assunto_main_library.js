function Form_onsave()
{
var assunto = Xrm.Page.getAttribute("new_assuntoid").getValue()[0].name;
var tipoAssunto = Xrm.Page.getAttribute("new_tipo_assunto").getSelectedOption().text;
Xrm.Page.getAttribute("new_name").setValue((assunto + " - " + tipoAssunto));
}
function new_name_onchange()
{

}
