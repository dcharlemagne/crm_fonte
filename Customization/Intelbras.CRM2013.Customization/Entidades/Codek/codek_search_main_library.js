function Form_onload()
{
window.GetEntityList = function()
{
    var request = "<Request xsi:type='RetrieveAllEntitiesRequest'>";
    request += "<MetadataItems>EntitiesOnly</MetadataItems>";
    request += "<RetrieveAsIfPublished>true</RetrieveAsIfPublished>";
    request += "</Request>";
    var result = window.QueryMetadataService(request);
   var xmlEntities = result.selectNodes("//CrmMetadata/CrmMetadata")
   var sXmlEntities = "<entities>";
   var blnIsCustomizable;
   var strLogicalName;

   for (i=0; i < xmlEntities.length; i++)
   {
     if (xmlEntities[i])
     {
     otc = xmlEntities[i].getElementsByTagName('ObjectTypeCode')[0].childNodes[0].nodeValue;
     blnIsCustomizable = (xmlEntities[i].getElementsByTagName('IsCustomizable')[0].childNodes[0].nodeValue  == "true");
     strLogicalName = xmlEntities[i].getElementsByTagName('LogicalName')[0].childNodes[0].nodeValue;
     oName = xmlEntities[i].selectSingleNode('DisplayName/LocLabels/LocLabel/Label');
      if (oName && blnIsCustomizable)
      {
            sXmlEntities += "<entity>";
            sXmlEntities += "<objecttypecode>" + otc + "</objecttypecode>";
            sXmlEntities += "<name>" + oName.text + "</name>";
            sXmlEntities += "<logicalname>" + strLogicalName + "</logicalname>";
            sXmlEntities += "</entity>";
      }
     }
   }
   sXmlEntities += "</entities>";

    var oXml = new ActiveXObject("Microsoft.XMLDOM");
    oXml.loadXML(sXmlEntities);
    return oXml;

   }

window.QueryMetadataService = function(request)
{ 
}

function addDropDownListOption(list, text, value)
{
 var optn = document.createElement("OPTION");
 optn.text = text;
 optn.value = value;
 list.options.add(optn);
}

function ClearDropDownList(list)
{
 for(i=list.options.length-1; i>=0 ;i--)
 {
  list.remove(i);
 }
}

function FillPickList(oDoc)
{
   var dataArray = oDoc.getElementsByTagName('entity');
   var dataArrayLen = dataArray.length;
   crmForm.all.codek_crm_entity.Sort = Xrm.Page.getAttribute("codek_crm_entity.SortingEnum.Ascending");

   for (var i=0; i < dataArrayLen; i++)
   {
      objectTypeCode = dataArray[i].getElementsByTagName('objecttypecode').item(0).firstChild.data;
      logicalname = dataArray[i].getElementsByTagName('logicalname').item(0).firstChild.data;
      name = dataArray[i].getElementsByTagName('name').item(0).firstChild.data;
      crmForm.all.codek_crm_entity.AddOption(name, objectTypeCode + "|" + logicalname);
   }
}

function  SetEntityList()
{
   if (Xrm.Page.getAttribute("codek_otc_crm_entity").getValue() == "") return;

   crmForm.all.codek_crm_entity.selectedIndex = 0;
   var strValor = Xrm.Page.getAttribute("codek_otc_crm_entity").getValue() + "|" + Xrm.Page.getAttribute("codek_logical_entity_name").getValue();

   for(i=0; i<=crmForm.all.codek_crm_entity.options.length-1; i++)
   {
      if (strValor == crmForm.all.codek_crm_entity.options[i].value)
      {
         crmForm.all.codek_crm_entity.selectedIndex = i;
         break;
      }
   }
}

Xrm.Page.getControl("codek_otc_crm_entity").setDisabled(true);
var oXml = window.GetEntityList();
FillPickList(oXml);

if (Xrm.Page.ui.getFormType()==2) SetEntityList();
}
function Form_onsave()
{
Xrm.Page.getControl("codek_otc_crm_entity").setDisabled(false);
crmForm.all.codek_crm_entity.selectedIndex = 0;
Xrm.Page.getAttribute("codek_logical_entity_name").setSubmitMode("always");
}
function codek_crm_entity_onchange()
{
if (Xrm.Page.getAttribute("codek_crm_entity").getValue())
{
    var aValues = Xrm.Page.getAttribute("codek_crm_entity").getValue().split("|");
    Xrm.Page.getAttribute("codek_otc_crm_entity").setValue(aValues[0]);
    Xrm.Page.getAttribute("codek_logical_entity_name").setValue(aValues[1]);
}
else
{
    Xrm.Page.getAttribute("codek_otc_crm_entity").setValue(null);
    Xrm.Page.getAttribute("codek_logical_entity_name").setValue(null);
}

if (crmForm.all.codek_crm_entity != null)
Xrm.Page.getAttribute("codek_target_audience_string").setValue(crmForm.all.codek_crm_entity.options[crmForm.all.codek_crm_entity.selectedIndex].text);
}
