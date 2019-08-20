function Form_onload() {
    /**************************
    Campo "referente a" obrigatório
    ***************************/
    Xrm.Page.getAttribute("regardingobjectid").setRequiredLevel("recommended");

    document.getElementById("{81A8A668-2716-4129-846D-E663546EC4FE}").style.height = "300px";
    document.getElementById("content_notescontrol").style.height = "300px";
}