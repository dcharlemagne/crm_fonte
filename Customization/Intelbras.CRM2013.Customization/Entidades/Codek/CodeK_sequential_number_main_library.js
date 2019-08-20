function Form_onload()
{
if (Xrm.Page.ui.getFormType() == 1)
     Xrm.Page.getAttribute("codek_actual_number").setValue(1);
else
{
    crmForm.codek_entity_name.Disabled = true;
    crmForm.codek_sequential_global.Disabled= true;
}
}
function Form_onsave()
{
Xrm.Page.getAttribute("codek_entity_name").setSubmitMode("always");
Xrm.Page.getAttribute("codek_name").setValue(Xrm.Page.getAttribute("codek_entity_name").getValue() + " - Sequential Number");
crmForm.all.codek_name.ForceSubmit= true;
}
function codek_sequential_global_onchange()
{
if (Xrm.Page.getAttribute("codek_sequential_global").getValue() == 1) {
         Xrm.Page.getAttribute("codek_entity_name").setValue("AllEntities");
         Xrm.Page.getControl("codek_entity_name").setDisabled(true);
    }
    else
         Xrm.Page.getControl("codek_entity_name").setDisabled(false);
}
