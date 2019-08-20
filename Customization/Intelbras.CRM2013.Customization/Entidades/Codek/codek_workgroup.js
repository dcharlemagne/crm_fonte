function Form_onload() {
    crmForm.EnableDisableFields = function (enable, field1, field2) {
        field1.Disabled = (!enable);
        field2.Disabled = (!enable);
        crmForm.SetFieldReqLevel(field1.id, enable);
        crmForm.SetFieldReqLevel(field2.id, enable);

        if (!enable) {
            field1.setValue(null);
            field2.setValue(null);
        }
    };

    crmForm.EnableDisableFields(Xrm.Page.getAttribute("codek_saturday_works").getValue() == 1, crmForm.all.codek_saturday_start_time, crmForm.all.codek_saturday_end_time);
    crmForm.EnableDisableFields(Xrm.Page.getAttribute("codek_sunday_works").getValue() == 1, crmForm.all.codek_sunday_start_time, crmForm.all.codek_sunday_end_time);

    crmForm.ValidateTime = function (field) {
        var blnOk = false;
        var value = field.getValue();

        if (value != null) {
            var aHourMinute = value.split(":");
            if (aHourMinute.length == 2) {
                try {
                    if (parseInt(aHourMinute[0]) >= 0 && parseInt(aHourMinute[0]) <= 23)
                        if (parseInt(aHourMinute[1]) >= 0 && parseInt(aHourMinute[1]) <= 59)
                            blnOk = true;
                }
                catch (e) {

                }
            }

            if (!blnOk) {
                alert("Insira uma hora válida!");
                field.setValue(null);
                field.SetFocus();
                return false;
            }
        }
    };
}
function Form_onsave() {
    Xrm.Page.getAttribute("codek_saturday_start_time").setSubmitMode("always");
    Xrm.Page.getAttribute("codek_saturday_end_time").setSubmitMode("always");
    Xrm.Page.getAttribute("codek_sunday_start_time").setSubmitMode("always");
    Xrm.Page.getAttribute("codek_sunday_end_time").setSubmitMode("always");
}
function codek_start_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_start_time);
}
function codek_end_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_end_time);
}
function codek_saturday_works_onchange() {
    crmForm.EnableDisableFields(Xrm.Page.getAttribute("codek_saturday_works").getValue() == 1, crmForm.all.codek_saturday_start_time, crmForm.all.codek_saturday_end_time);
}
function codek_saturday_start_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_saturday_start_time);
}
function codek_saturday_end_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_saturday_end_time);
}
function codek_sunday_works_onchange() {
    crmForm.EnableDisableFields(Xrm.Page.getAttribute("codek_sunday_works").getValue() == 1, crmForm.all.codek_sunday_start_time, crmForm.all.codek_sunday_end_time);
}
function codek_sunday_start_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_sunday_start_time);
}
function codek_sunday_end_time_onchange() {
    crmForm.ValidateTime(crmForm.all.codek_sunday_end_time);
}
