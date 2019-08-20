function Form_onload() {
    try {
        crmForm.all.new_pesquisa.value = parent.opener.window.top.resultRender.FetchXml.value;
    }
    catch (e) {
        alert("Ops! Aconteceu um erro, feche o formulário e tente novamente!");
    }
}