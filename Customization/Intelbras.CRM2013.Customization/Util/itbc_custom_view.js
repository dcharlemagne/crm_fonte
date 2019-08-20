CustomView = {
    LookupSchemaName: null,
    ViewId: null,
    EntityName: null,
    PrimaryKeyName: null,
    PrimaryFieldName: null,
    ViewDisplayName: null,
    FilterBy: null,
    OrderBy: null,
    ViewColumns: null,
    CustonConditional: null,

    Operator: {
        'Equal': { Value: 0, Operation: 'eq' },
        'EqualGuid': { Value: 0, Operation: 'eq guid' },
        'NotEqual': { Value: 1, Operation: 'ne' },
        'NotNull': { Value: 2, Operation: 'not-null' },
        'Null': { Value: 3, Operation: 'null' },
        'Like': { Value: 4, Operation: 'like' },
        'NotLike': { Value: 5, Operation: 'not-like' },
        'BeginWith': { Value: 6, Operation: 'like' },
        'DoesNotBeginWith': { Value: 7, Operation: 'not-like' },
        'EndWith': { Value: 8, Operation: 'like' },
        'DoesNotEndWith': { Value: 9, Operation: 'not-like' }
    },

    CriarEstruturaFiltrosParaLookup: function (numeroLinhas) {
        /// <summary>Retorna uma Matriz de N linhas e 3 Colunas</summary>
        /// <param name="numeroLinhas" type="int">Número de Linhas para retorno</param>
        /// <returns type="Array">A primeira coluna se refere ao Nome do atributo para o filtro. A segunda é o tipo de operador. A terceira é o valor a ser comparado</returns>

        matriz = new Array(numeroLinhas);
        for (i = 0; i < matriz.length; i++)
            matriz[i] = new Array(3);

        return matriz;
    },

    AdvancedFilteredLookup: function (lookupSchemaName, viewId, entityName, primaryKeyName, primaryFieldName, viewDisplayName, filterBy, orderBy, viewColumns, custonConditional) {
        var fetchXmlOriginal = custonConditional;
        var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                    "<entity name='" + entityName + "'>" +
                                        "<attribute name='" + primaryFieldName + "' />" +

                                        "<order attribute='" + orderBy + "' descending='false' />"
        /////////////////////////////////////////Filtros
        if (filterBy[0] != null) {
            fetchXml += "<filter type='and'>";
            for (var i = 0; i < filterBy.length; i++)
                fetchXml += "<condition attribute='" + filterBy[i].SchemaName + "' operator='" + filterBy[i].Operator + "' value='" + filterBy[i].Value + "' />";
        }

        if (custonConditional != null)
            fetchXml += custonConditional;

        if (filterBy[0] != null)
            fetchXml += "</filter>";

        fetchXml += "</entity>";
        fetchXml += "</fetch>";
        /////////////////////////////////////////Filtros

        ///////////////////////////////////////////Layout XML
        var layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'><row name='result' id='" + primaryKeyName + "'>";

        for (var i = 0; i < viewColumns.length; i++)
            layoutXml += "<cell name='" + viewColumns[i].SchemaName + "' width='" + viewColumns[i].Width.toString() + "' />";

        layoutXml += "</row>";
        layoutXml += "</grid>";
        /////////////////////////////////////////Layout XML

        try {
            debugger
            var lookupControl = Xrm.Page.getControl(lookupSchemaName);
            var defaulView = Xrm.Page.getControl(lookupSchemaName).getDefaultView();
            lookupControl.addCustomView(defaulView, entityName, viewDisplayName, fetchXml, layoutXml, false);
            //Xrm.Page.getControl(arg).getDefaultView()
            //lookupControl.setDefaultView(viewId);

            //Xrm.Page.getControl(lookupSchemaName).addCustomFilter(fetchXmlOriginal, entityName)
        } catch (err) {
            alert("Erro ao Atribuir a função AdvancedFilteredLookup: " + err);
        }
    },

    FilterGlobal: function (customViewId, customViewName, lookupFieldName, entityName, primaryKeyName, primaryFieldName, orderBy, userArrayfilters, viewColumns, custonConditional, onload) {
        var FilterBy = this.MakeStruct("SchemaName Operator Value");
        try {
            var filters = new Array();
            var index = 0;
            if (userArrayfilters != null) {
                for (var row = 0; row < userArrayfilters.length; ++row) {
                    switch (userArrayfilters[row][1]) {
                        case this.Operator.Like.Operation: userArrayfilters[row][2] = "%" + userArrayfilters[row][2] + "%"; break;
                        case this.Operator.NotLike.Operation: userArrayfilters[row][2] = "%" + userArrayfilters[row][2] + "%"; break;
                        case this.Operator.BeginWith.Operation: userArrayfilters[row][2] = userArrayfilters[row][2] + "%"; break;
                        case this.Operator.DoesNotBeginWith.Operation: userArrayfilters[row][2] = userArrayfilters[row][2] + "%"; break;
                        case this.Operator.EndWith.Operation: userArrayfilters[row][2] = "%" + userArrayfilters[row][2]; break;
                        case this.Operator.DoesNotEndWith.Operation: userArrayfilters[row][2] = "%" + userArrayfilters[row][2]; break;
                    }

                    filters[index++] = new FilterBy(userArrayfilters[row][0], userArrayfilters[row][1], userArrayfilters[row][2]);
                }
            }

            //this.AdvancedFilteredLookup(lookupFieldName, customViewId, entityName, primaryKeyName, primaryFieldName, customViewName, filters, orderBy, viewColumns, custonConditional);

            this.LookupSchemaName = lookupFieldName;
            this.ViewId = customViewId;
            this.EntityName = entityName;
            this.PrimaryKeyName = primaryKeyName;
            this.PrimaryFieldName = primaryFieldName;
            this.ViewDisplayName = customViewName;
            this.FilterBy = filters;
            this.OrderBy = orderBy;
            this.ViewColumns = viewColumns;
            this.CustonConditional = custonConditional;

            Xrm.Page.getControl(lookupFieldName).addPreSearch(
                function () {
                    var fetchXmlOriginal = CustomView.CustonConditional;
                    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                                "<entity name='" + CustomView.EntityName + "'>" +
                                                    "<attribute name='" + CustomView.PrimaryFieldName + "' />" +

                                                    "<order attribute='" + CustomView.OrderBy + "' descending='false' />"
                    /////////////////////////////////////////Filtros
                    if (CustomView.FilterBy[0] != null) {
                        fetchXml += "<filter type='and'>";
                        for (var i = 0; i < CustomView.FilterBy.length; i++)
                            fetchXml += "<condition attribute='" + CustomView.FilterBy[i].SchemaName + "' operator='" + CustomView.FilterBy[i].Operator + "' value='" + CustomView.FilterBy[i].Value + "' />";
                    }

                    if (CustomView.CustonConditional != null)
                        fetchXml += CustomView.CustonConditional;

                    if (CustomView.FilterBy[0] != null)
                        fetchXml += "</filter>";

                    fetchXml += "</entity>";
                    fetchXml += "</fetch>";
                    /////////////////////////////////////////Filtros

                    ///////////////////////////////////////////Layout XML
                    var layoutXml = "<grid name='resultset' object='1' jump='name' select='1' icon='1' preview='1'><row name='result' id='" + CustomView.PrimaryKeyName + "'>";

                    for (var i = 0; i < CustomView.ViewColumns.length; i++)
                        layoutXml += "<cell name='" + CustomView.ViewColumns[i].SchemaName + "' width='" + CustomView.ViewColumns[i].Width.toString() + "' />";

                    layoutXml += "</row>";
                    layoutXml += "</grid>";
                    /////////////////////////////////////////Layout XML

                    try {
                        var lookupControl = Xrm.Page.getControl(CustomView.LookupSchemaName);
                        var defaulView = Xrm.Page.getControl(CustomView.LookupSchemaName).getDefaultView();

                        Xrm.Page.getControl(CustomView.LookupSchemaName).addCustomFilter(fetchXmlOriginal, CustomView.EntityName)
                    } catch (err) {
                        alert("Erro ao Atribuir a função AdvancedFilteredLookup: " + err);
                    }
                }
            );
        }
        catch (err) {
            alert("Erro ao Atribuir a Função FilterGlobal: " + err);
        }
    },

    MakeStruct: function (names) {
        var names = names.split(' ');
        var count = names.length;

        function constructor() {
            for (var i = 0; i < count; i++) {
                this[names[i]] = arguments[i];
            }
        }
        return constructor;
    },

    Guid: function () {
        var S4 = function () { return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1); };
        return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    }
}