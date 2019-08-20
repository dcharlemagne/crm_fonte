function Form_onload() {
    crmForm.disableVerticalMenuItem = function (navBar, menuItem) {
        menuItem = menuItem.toLowerCase().replace(/^\s+|\s+$/g, '');
        il = document.getElementById(navBar).getElementsByTagName('li');

        for (i = 0; i < il.length; i++) {
            liItem = il[i].innerText.toLowerCase().replace(/^\s+|\s+$/g, '');

            if (liItem == menuItem) {
                anchor = il[i].getElementsByTagName('a')[0];
                anchor.parentNode.removeChild(anchor);
            }
        }
    }
    crmForm.disableVerticalMenuItem("crmNavBar", "Faturas");

    //Esconder a pasta Não Utilizar
    Xrm.Page.ui.tabs.get(7).setVisible(false);
}
