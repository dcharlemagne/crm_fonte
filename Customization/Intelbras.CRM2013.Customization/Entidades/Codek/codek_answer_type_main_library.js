function IFRAME_proximapergunta_onload()
{
 
}
function IFRAME_TipoRespostaesperada_onload()
{
 
}
function Form_onload()
{
/*-----Inicio das Funções----*/

crmForm.PosicionaProximaPergunta = function()
{
   switch(Xrm.Page.ui.getFormType())
   {
       case 1:
          crmForm.all.tab1Tab.style.display="none";
          break;

       default:
          crmForm.all.tab1Tab.style.display="inline";

          var tipoSelecionado = Xrm.Page.getAttribute("codek_types.options[crmForm.all.codek_types.selectedIndex].value");

          var sUrl = "/ISV/CodeK.ScriptTools/ProximaPergunta.aspx";

          sUrl = sUrl + "?idPergunta=" + crmForm.getQueryVariable(window.opener.location.search.substring(1), 'oId');
          sUrl = sUrl + "&tipoRespostaId=" + Xrm.Page.data.entity.getId();
          sUrl = sUrl + "&tipoSelecionado=" + tipoSelecionado;
          sUrl = sUrl + "&otc=" + ;
          sUrl = sUrl + "&orgname=" + crmForm.GetOrganizationName();

          Xrm.Page.getControl("IFRAME_proximapergunta").setSrc(sUrl);
          break;
   }
}

crmForm.ValorNumerico = function(caracter)
{
    if(document.all) 
    { // Internet Explorer
        var tecla = caracter.keyCode;
    }
    else 
    {
        if(document.layers) 
        { // Nestcape
            var tecla = caracter.which;
        }
    }

    if(tecla > 47 && tecla < 58) { // numeros de 0 a 9
        return true;
    }
    else {
        if (tecla != 8) { // backspace
            return false;
        }
        else {
            return true;
        }
 }
}



crmForm.getQueryVariable = function(query, variable)
{
    var vars = query.split("&");
    for (var i=0;i<vars.length;i++)
    {
        var pair = vars[i].split("=");
        if (pair[0] == variable)
            return pair[1];
    }
}

crmForm.UpdateLabel = function(campoId, texto)
{
   var newtext = '<LABEL for=' + campoId + '>' + texto + '</LABEL>';
   return newtext;
}


crmForm.GetOrganizationName = function()
{
    xmlDoc = new ActiveXObject('Microsoft.XMLDOM');
    xmlDoc.async=false; 
    authenticationheader = Xrm.Page.context.getAuthenticationHeader();
    authenticationheader = authenticationheader.replace('soap:Header', 'soapHeader');
    authenticationheader = authenticationheader.replace('soap:Header', 'soapHeader');
    xmlDoc.loadXML(authenticationheader);
    organizationName = xmlDoc.getElementsByTagName('OrganizationName')[0].childNodes[0].nodeValue;
    return organizationName;
}

crmForm.PosicionarTipoResposta = function()
{
   /*
   Tratamento para as opções do PickList
   1 - Texto
   2 - Opção
   3 - Escala da classificação
   4 - Número
   5 - Data e Hora
   6 - Sim/Não (Seleção)
   7 - Multipla Escolha
   */

   crmForm.all.codek_attribute_answer_type.style.visibility = 'hidden';
   crmForm.all.codek_attribute_answer_type_c.style.visibility = 'hidden';
   crmForm.all.codek_default_value.style.visibility = 'hidden';
   crmForm.all.codek_default_value_c.style.visibility = 'hidden';

   
   crmForm.all.codek_text_interval1.style.visibility = 'hidden';
   crmForm.all.codek_text_interval1.style.height = '0px';
   crmForm.all.codek_text_interval1_c.style.visibility = 'hidden';
   crmForm.all.codek_text_interval1_c.style.height = '0px';
   crmForm.all.codek_text_interval2.style.visibility = 'hidden';
   crmForm.all.codek_text_interval2.style.height = '0px';
   crmForm.all.codek_text_interval2_c.style.visibility = 'hidden';
   crmForm.all.codek_text_interval2_c.style.height = '0px';
   crmForm.all.codek_text_interval3.style.visibility = 'hidden';
   crmForm.all.codek_text_interval3.style.height = '0px';
   crmForm.all.codek_text_interval3_c.style.visibility = 'hidden';
   crmForm.all.codek_text_interval3_c.style.height = '0px';

   // Próxima Pergunta
   crmForm.all.tab1Tab.style.display="none";

   crmForm.SetFieldReqLevel('codek_attribute_answer_type',0);

   crmForm.all.codek_attribute_answer_type.onkeypress = null;
   crmForm.all.codek_attribute_answer_type.onblur = null;

   crmForm.PosicionaTipoRespostaEsperada();

   switch(Xrm.Page.getAttribute("codek_types").getValue())
   {
      case '1':
         crmForm.all.codek_attribute_answer_type.style.visibility = 'visible';
         crmForm.all.codek_attribute_answer_type_c.style.visibility = 'visible';
         crmForm.all.codek_attribute_answer_type_c.innerHTML = crmForm.UpdateLabel('codek_attribute_answer_type', 'No Max Caracteres');
         crmForm.all.codek_default_value.style.visibility = 'visible';
         crmForm.all.codek_default_value_c.style.visibility = 'visible';

         crmForm.all.codek_attribute_answer_type.onkeypress = function (e)
         {
            return crmForm.ValorNumerico(window.event);
         }
         break;

      case '2':
         crmForm.all.codek_default_value.style.visibility = 'visible';
         crmForm.all.codek_default_value_c.style.visibility = 'visible';
         crmForm.PosicionaProximaPergunta();
         break;

      case '3':
      crmForm.all.codek_attribute_answer_type.style.visibility = 'visible';
      crmForm.all.codek_attribute_answer_type_c.style.visibility = 'visible';
      crmForm.all.codek_attribute_answer_type_c.innerHTML = crmForm.UpdateLabel('codek_attribute_answer_type', 'Intervalo (3 a 20)');
      
      crmForm.all.codek_text_interval1.style.visibility = 'visible';
      crmForm.all.codek_text_interval1.style.height = 'auto';
      crmForm.all.codek_text_interval1_c.style.visibility = 'visible';
      crmForm.all.codek_text_interval1_c.style.height = 'auto';
      crmForm.all.codek_text_interval2.style.visibility = 'visible';
      crmForm.all.codek_text_interval2.style.height = 'auto';
      crmForm.all.codek_text_interval2_c.style.visibility = 'visible';
      crmForm.all.codek_text_interval2_c.style.height = 'auto';
      crmForm.all.codek_text_interval3.style.visibility = 'visible';
      crmForm.all.codek_text_interval3.style.height = 'auto';
      crmForm.all.codek_text_interval3_c.style.visibility = 'visible';
      crmForm.all.codek_text_interval3_c.style.height = 'auto';

      // Próxima Pergunta
      crmForm.all.tab1Tab.style.display="none";
      crmForm.all.codek_attribute_answer_type.onkeypress = function (e)
      {
         return crmForm.ValorNumerico(window.event);
      }
     crmForm.all.codek_attribute_answer_type.onblur = function (e)
     {
        if (crmForm.IntervaloEscala(this))
           return true;
        else
        {
           this.setValue('');
           this.focus();
           alert('Valor do Intervalo deve estar entre 3 e 20.');
           return false;
        }
     }
      crmForm.SetFieldReqLevel('codek_attribute_answer_type',true);
     break;

      case '4':
      crmForm.all.codek_attribute_answer_type.style.visibility = 'visible';
      crmForm.all.codek_attribute_answer_type_c.style.visibility = 'visible';
      crmForm.all.codek_attribute_answer_type_c.innerHTML = crmForm.UpdateLabel('codek_attribute_answer_type',  'Formato (ex: 999,99)');
      crmForm.all.codek_default_value.style.visibility = 'visible';
      crmForm.all.codek_default_value_c.style.visibility = 'visible';
      

      crmForm.all.codek_attribute_answer_type.onblur = function (e)
      {
         if (crmForm.FormatoNumero(this))
            return true;
         else
         {
            this.setValue('');
            this.focus();
            alert('Formato Incorreto');
            return false;
         }
      }

      crmForm.PosicionaProximaPergunta();

      break;

      case '5':
         crmForm.all.codek_default_value.style.visibility = 'visible';
         crmForm.all.codek_default_value_c.style.visibility = 'visible';
         break;

      case '6':
         crmForm.all.codek_default_value.style.visibility = 'visible';
         crmForm.all.codek_default_value_c.style.visibility = 'visible';

         crmForm.PosicionaProximaPergunta();
         break;

        }
}

crmForm.IntervaloEscala = function(tipoResposta)
{
   if (tipoResposta.value=='') return true;
        return (tipoResposta.value > 2 && tipoResposta.value < 21);
}

crmForm.FormatoNumero = function(obj)
{
   /*
   Aceitará formatos com os valores:
   - 0 a 9;
   - sinais de "." ou "," .
   */
   if (obj.value=='') return true;
   var ret = true;

   for(i=0;i<obj.value.length;i++)
   {
      var carac = obj.value.substr(i,1);
      var keycode = carac.charCodeAt(0);
      var ret = ((keycode > 47 && keycode < 58) || (keycode==44) || (keycode==46) );
      if (!ret) break;
   }
   return ret;
}

crmForm.PosicionaTipoRespostaEsperada = function()
{

   switch(Xrm.Page.ui.getFormType())
   {
       case 1:
       crmForm.all.tab2Tab.style.display="none";
       break;

       default:
       var tipoResposta = Xrm.Page.getAttribute("codek_types.options[crmForm.all.codek_types.selectedIndex].value");

       if (tipoResposta==1 || tipoResposta==5)
       {
           crmForm.all.tab2Tab.style.display="none";
       }
       else
       {
          crmForm.all.tab2Tab.style.display="inline";

          var sUrl = "/ISV/CodeK.ScriptTools/RespostaEsperada.aspx";
          sUrl = sUrl + "?id=" + Xrm.Page.data.entity.getId() + "&otc=" + ;
          sUrl = sUrl + "&tipoResposta=" + tipoResposta;
          sUrl = sUrl + "&orgname=" + crmForm.GetOrganizationName();
          Xrm.Page.getControl("IFRAME_TipoRespostaesperada").setSrc(sUrl);
       }

       break;
   }

}





if (window.opener.document.all['crmGrid'].InnerGrid.SelectedRecords.length > 0 && Xrm.Page.ui.getFormType()==1)
{
   alert('Atenção,\nJá existe uma TipoResposta cadastrada.');
   window.close();
}
/*-----Fim das funções----*/


crmForm.PosicionarTipoResposta();
Xrm.Page.getControl("codek_name").setDisabled(true);
Xrm.Page.getControl("codek_expected_type").setDisabled(true);
Xrm.Page.getControl("codek_next_question").setDisabled(true);

if (Xrm.Page.ui.getFormType()==2) 
    crmForm.all.codek_types.disabled = true;
}
function Form_onsave()
{
Xrm.Page.getControl("codek_name").setDisabled(false);
Xrm.Page.getControl("codek_expected_type").setDisabled(false);
Xrm.Page.getControl("codek_next_question").setDisabled(false);
}
function codek_types_onchange()
{
if (window.opener.document.all['crmGrid'].InnerGrid.SelectedRecords.length > 0 && Xrm.Page.ui.getFormType()==1)
{
   alert('Atenção,\nJá existe um Tipo de Resposta cadastrada.');
   window.close();
}

crmForm.PosicionarTipoResposta();

Xrm.Page.getControl("codek_name").setDisabled(true);
Xrm.Page.getControl("codek_expected_type").setDisabled(true);
Xrm.Page.getControl("codek_next_question").setDisabled(true);

if (Xrm.Page.ui.getFormType()==2) 
    crmForm.all.codek_types.disabled = true;


Xrm.Page.getAttribute("codek_name").setValue(crmForm.all.codek_types.options[crmForm.all.codek_types.selectedIndex].text);

if (crmForm.all.codek_attribute_answer_type)
{
   Xrm.Page.getAttribute("codek_attribute_answer_type").setValue('');
}

if (crmForm.all.codek_default_value)
{
   Xrm.Page.getAttribute("codek_default_value").setValue('');
}

Xrm.Page.getAttribute("codek_expected_type").setValue('');
Xrm.Page.getAttribute("codek_next_question").setValue('');
}
