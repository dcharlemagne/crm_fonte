/// <reference path="SDK.Metadata.js" />
if (typeof (SDKore) == "undefined") { SDKore = {}; }
SDKore = {

    _parameterCheck: function (parametro, mensagem) {
        ///<summary>
        /// Funcao Private para checar se o parametro é null ou undefined
        ///</summary>
        ///<param name="parametro" type="Object">
        /// O parâmetro para checar;
        ///</param>
        ///<param name="mensagem" type="String">
        /// A mensagem que será enviada para o throw
        ///</param>
        if ((typeof parametro === "undefined") || parametro === null) {
            throw new Error(mensagem);
        }
    },
    _stringParameterCheck: function (parametro, mensagem) {
        ///<summary>
        /// Funcao Private para checar se o parametro é string
        ///</summary>
        ///<param name="parametro" type="String">
        /// O parametro para checar
        ///</param>
        ///<param name="mensagem" type="String">
        /// A mensagem que será enviada para o throw
        ///</param>
        if (typeof parametro != "string") {
            throw new Error(mensagem);
        }
    },

    _functionParameterCheck: function (functionParameter, mensagem) {
        ///<summary>
        /// Funcao Private para checar se o parametro é Uma funcao ou undefined
        ///</summary>
        ///<param name="functionParameter" type="Function">
        /// A funcao para ser checada
        ///</param>
        ///<param name="mensagem" type="String">
        /// A mensagem que será enviada para o throw
        ///</param>
        if (typeof functionParameter != "function") {
            throw new Error(mensagem);
        }
    },
    _argumentosMaiorQue: function (argumentos, valor, mensagem) {
        ///<summary>
        /// Funcao Private para checar se o numero de argumentos é maior que um valor
        ///</summary>
        ///<param name="argumentos" type="Array">
        /// Array de argumentos
        ///</param>
        ///<param name="valor" type="int">
        /// Valor que sera comparado
        ///</param>
        ///<param name="mensagem" type="string">
        /// A mensagem que será enviada para o throw
        ///</param>
        if (argumentos.length > valor)
            throw new Error(mensagem);
    },
    _argumentosMenorQue: function (argumentos, valor, mensagem) {
        ///<summary>
        /// Funcao Private para checar se o numero de argumentos é menor que um valor
        ///</summary>
        ///<param name="argumentos" type="Array">
        /// Array de argumentos
        ///</param>
        ///<param name="valor" type="int">
        /// Valor que sera comparado
        ///</param>
        ///<param name="mensagem" type="string">
        /// A mensagem que será enviada para o throw
        ///</param>
        if (argumentos.length < valor)
            throw new Error(mensagem);
    },

    CallService: function (url, method) {
        /// <summary>Executa chamada ao método do WCF informado.</summary>
        /// <param name="url" type="string">A url do WCF.</param>
        /// <param name="method" type="string">O nome do método a ser executado.</param>
        /// <returns type="Objeto">O retorno oferecido pelo método executado.</returns>

        var servico = this;
        var parameter = "";
        var obj = new Object();
        obj.Success = true;
        obj.ReturnValue = null;

        servico.SetParameter = function (name, value) {
            parameter += '"' + name + '": "' + value + '",';
        }

        servico.Execute = function () {
            $.ajax({
                async: false,
                type: "POST",
                url: url + "/" + method,
                data: configureParameter(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processdata: true,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    obj.ReturnValue = data;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Falha na chamada do método:" + method + textStatus + " erro : " + errorThrown);
                    obj.Success = false;
                    
                }
            });
            return configureReturn(obj);
        }

        function configureReturn() {
            if (obj.ReturnValue == undefined || obj.ReturnValue == null)
                return obj;

            if (obj.ReturnValue.hasOwnProperty("d"))
                obj.ReturnValue = obj.ReturnValue.d;

            return obj;
        }

        function configureParameter() {
            parameter = parameter.length == 0 ? "," : parameter;
            return "{" + parameter.substr(0, parameter.length - 1) + "}";
        }
    },
    
    OData: new function () {

        /* 
        * Os métodos OData utilizados foram adaptados de jqueryrestdataoperationactions.js 
        * Francisco Avelino
        */

        // url usada como base, sem parametros apenas o Protocolo+host+orgName+CRMURL
        var baseUrl = "";

        //Referencia do OData
        var that = this;

        //url final que sera feita o request
        this.url = "";

        //parametros que serão adicionados na url de request
        this.stringParametros = "";

        //nome da entidade alvo
        this.entidade = "";

        //Constante da url que o ODATA fica
        var CRMURL = "XRMServices/2011/OrganizationData.svc/";

        this.setBaseUrl = function (tipo) {
            switch (tipo) {
                //Permite expandir as formas que será obtida a url base
                //Pode adicionar um case para setar a url manualmente por exemplo
                case "local":
                    //Pega a url com base no navegador do usuario,é necessário pois pelo getServerUrl ele sempre pega o dominio, impossibilitando testes em dev por exemplo
                    var host = window.location.host;
                    if (!host.endsWith("/"))
                        host += "/";
                    baseUrl = window.location.protocol + "//" + host + Xrm.Page.context.getOrgUniqueName() + "/" + CRMURL;
                    break;
                default:
                    var serverAddress = Xrm.Page.context.getClientUrl();
                    baseUrl = serverAddress + (serverAddress.charAt(serverAddress.length - 1) == "/" ? "" : "/") + CRMURL;
                    break;
            }
        };

        /// <summary>Configura os parâmetros da request</summary>
        /// <param name="parametros" type="array">Array contendo objetos de parametros</param>
        /// <param name="parametros" type="string">String contendo parametros</param>
        /// Exemplo de utilizacao em array: [{"select":["name","createdon"]},{"orderby":"Price desc"}]
        /// Exemplo de utilizacao em string: $select=PriceLevelId&$top=1
        this.configurarParametros = function (parametros) {
                var retornoStringParametros;//string builder temporária que sera usada no foreach para retornar o stringParametros
                if (parametros instanceof Array) {
                    throw new Error("Metodo não implementado(SDKore.OData.configurarParametros(array)");

                    parametros.forEach(function (registro) {
                        if (typeof registro == "object")
                            $.each(registro, function (key, value) {
                                //por enquanto como estou sem tempo pra testar e nao irei utilizar, nao adicionei
                                //console.log(registro + key + value);
                                //TODO: falta fazer a função que transforma o registro + key + value em uma string de parametros que será inserida na url
                            });
                    });
                }
                //Simula uma sobrecarga de métodos
                else if (typeof parametros == "string") {
                    if (parametros.length > 0)
                        that.stringParametros = "?" + parametros;
                    else
                        that.stringParametros = "";

                    SDKore._stringParameterCheck(that.stringParametros, "baseUrl deve ser uma String");
                }
                else
                    throw new Error("Parametros em formato desconhecido.Formatos aceitos: Array de objetos,String");
        };

        //Executa na construcao do objeto para nao precisar repetir toda hora
        this.setBaseUrl();

        this.ErrorHandler = function (xmlHttpRequest, textStatus, errorThrow) {
            alert("Operação não realizada. Ocorreu o seguinte erro: \n" + textStatus + ": " + xmlHttpRequest.statusText);
        };

        this.Retrieve = function (id, nomeEntidade, FuncaoSucesso, FuncaoErro) {
            /// <summary>Obtém registro do CRM.</summary>
            /// <param name="id" type="string">O guid do registro a ser obtido.</param>
            /// <param name="nomeEntidade" type="string">O tipo (schema name) do registro a ser obtido.</param>
            /// <param name="FuncaoSucesso" type="function">Permite estender o sucess do ajax mandando uma função personalizada para tratar a var data.</param>
            /// <param name="FuncaoErro" type="function">Permite estender o error do ajax mandando uma função personalizada para tratar o erro</param>

            SDKore._argumentosMenorQue(arguments, 2, "Função Retrieve requer 2 parâmetros");
            SDKore._stringParameterCheck(nomeEntidade, "Nome Entidade deve ser uma String");
            SDKore._stringParameterCheck(id, "Guid deve ser uma String");

            //para nao perder a referencia do arguments dentro do ajax
            var args = arguments;
            var returnObject;

            this.url = baseUrl + nomeEntidade + "Set(guid'" + id + "')" + this.stringParametros;
            SDKore._stringParameterCheck(this.url, "Url deve ser uma String");

            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                url: this.url,
                beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
                success: function (data, textStatus, XmlHttpRequest) {
                    //é checado se é != de null para poder chamar FuncaoErro sem enviar FuncaoSucesso exemplo: Retrieve(guid,entidade,null,funcErro)
                    if (args.length > 2 && args[2] !== null) {
                        SDKore._functionParameterCheck(FuncaoSucesso, "FuncaoSucesso deve ser uma função");
                        returnObject = FuncaoSucesso(data.d);
                    }
                    else
                        returnObject = data.d;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    if (args.length > 3) {
                        SDKore._functionParameterCheck(FuncaoErro, "FuncaoError deve ser uma função");
                        FuncaoErro(XmlHttpRequest, textStatus, errorThrown);
                    }
                    else
                        that.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
                }
            });
            return returnObject;
        };

        this.RetrieveMultiple = function (nomeEntidade, filtroLegado, FuncaoSucesso, FuncaoErro) {
            /// <summary>Obtém lista de registros do CRM.</summary>
            /// <param name="nomeEntidade" type="string">O tipo (logical name) dos registros a serem obtidos.</param>
            /// <param name="filtroLegado" type="string">O filtro OData a ser executado para obter os registros.Parametro Legado</param>
            /// <param name="FuncaoSucesso" type="function">Permite estender o sucess do ajax mandando uma função personalizada para tratar a var data.</param>
            /// <param name="FuncaoErro" type="function">Permite estender o error do ajax mandando uma função personalizada para tratar o erro</param>

            SDKore._argumentosMenorQue(arguments, 1, "Função Retrieve requer 1 parâmetro");
            SDKore._stringParameterCheck(nomeEntidade, "Nome Entidade deve ser uma String");

            //para nao perder a referencia do arguments dentro do ajax
            var args = arguments;
            var returnObject;

            if (filtroLegado) {
                SDKore._stringParameterCheck(filtroLegado, "Filtro deve ser uma String");
                this.stringParametros = "?$filter=" + filtroLegado;
            }

            this.url = baseUrl + nomeEntidade + "Set" + this.stringParametros;
            SDKore._stringParameterCheck(this.url, "Url deve ser uma String");

            $.ajax({
                type: "GET",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                url: this.url,
                beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
                success: function (data, textStatus, XmlHttpRequest) {
                    //é checado se é != de null para poder chamar FuncaoErro sem enviar FuncaoSucesso exemplo: Retrieve(guid,entidade,null,funcErro)
                    if (args.length > 2 && args[2] !== null) {
                        SDKore._functionParameterCheck(FuncaoSucesso, "FuncaoSucesso deve ser uma função");
                        returnObject = FuncaoSucesso(data.d);
                    }
                    else {
                        if (data && data.d && data.d.results)
                            returnObject = data.d.results;
                        else if (data && data.d)
                            returnObject = data.d;
                        else
                            returnObject = data;
                    }
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    if (args.length > 3) {
                        SDKore._functionParameterCheck(FuncaoErro, "FuncaoError deve ser uma função");
                        FuncaoErro(XmlHttpRequest, textStatus, errorThrown);
                    }
                    else {
                        that.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
                    }
                }
            });
            return returnObject;
        };

        this.Create = function (entidade, nomeEntidade, FuncaoSucesso, FuncaoErro) {
            /// <summary>Cria registro no CRM com o serviço de OData.</summary>
            /// <param name="entidade" type="object">Objeto a ser criado no CRM.</param>
            /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser criado.</param>
            /// <param name="FuncaoSucesso" type="function">Permite estender o sucess do ajax mandando uma função personalizada para tratar a var data.</param>
            /// <param name="FuncaoErro" type="function">Permite estender o error do ajax mandando uma função personalizada para tratar o erro</param>

            SDKore._argumentosMenorQue(arguments, 2, "Função Retrieve requer 2 parâmetros");
            SDKore._stringParameterCheck(nomeEntidade, "Nome Entidade deve ser um String");

            var args = arguments;
            var returnObject;
            var jsonEntity = window.JSON.stringify(entidade);

            SDKore._stringParameterCheck(jsonEntity, "Erro ao transformar a entidade em String");

            this.url = baseUrl + nomeEntidade + "Set" + this.stringParametros;
            SDKore._stringParameterCheck(this.url, "Url de Request deve ser uma String");

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                url: this.url,
                data: jsonEntity,
                beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
                success: function (data, textStatus, XmlHttpRequest) {
                    //é checado se é != de null para poder chamar FuncaoErro sem enviar FuncaoSucesso exemplo: Retrieve(guid,entidade,null,funcErro)
                    if (args.length > 2 && args[2] !== null) {
                        SDKore._functionParameterCheck(FuncaoSucesso, "FuncaoSucesso deve ser uma função");
                        returnObject = FuncaoSucesso(data.d);
                    }
                    else
                        returnObject = data.d;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    if (args.length > 3) {
                        SDKore._functionParameterCheck(FuncaoErro, "FuncaoErro deve ser uma função");
                        FuncaoErro(XmlHttpRequest, textStatus, errorThrown);
                    }
                    else
                        that.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
                }
            });
            return returnObject;
        }

        this.Update = function (id, entidade, nomeEntidade, FuncaoSucesso, FuncaoErro) {
            /// <summary>Atualiza registro no CRM.</summary>
            /// <param name="id" type="string">O guid do registro a ser atualizado.</param>
            /// <param name="entidade" type="object">Objeto a ser atualizado no CRM.</param>
            /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser atualizado.</param>
            /// <param name="FuncaoSucesso" type="function">Permite estender o sucess do ajax mandando uma função personalizada para tratar a var data.</param>
            /// <param name="FuncaoErro" type="function">Permite estender o error do ajax mandando uma função personalizada para tratar o erro</param>
            SDKore._argumentosMenorQue(arguments, 3, "Função Update requer 3 parâmetros");
            SDKore._stringParameterCheck(nomeEntidade, "Nome Entidade deve ser uma String");
            SDKore._stringParameterCheck(id, "Guid Entidade deve ser uma String");

            var args = arguments;
            var returnObject;
            var jsonEntity = window.JSON.stringify(entidade);

            SDKore._stringParameterCheck(jsonEntity, "JsonEntity deve ser uma String");

            this.url = baseUrl + nomeEntidade + "Set(guid'" + id + "')" + this.stringParametros;
            SDKore._stringParameterCheck(this.url, "Url deve ser uma String");

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                data: jsonEntity,
                url: this.url,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                    XMLHttpRequest.setRequestHeader("X-HTTP-Method", "MERGE");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    //é checado se é != de null para poder chamar FuncaoErro sem enviar FuncaoSucesso exemplo: Retrieve(guid,entidade,null,funcErro)
                    if (args.length > 3 && args[3] !== null) {
                        SDKore._functionParameterCheck(FuncaoSucesso, "FuncaoSucesso deve ser uma função");
                        returnObject = FuncaoSucesso(data.d);
                    }
                    else {
                        data = new Object();
                        data.id = id;
                        returnObject = data;
                    }
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    if (args.length > 4) {
                        SDKore._functionParameterCheck(FuncaoErro, "FuncaoErro deve ser uma função");
                        FuncaoErro(XmlHttpRequest, textStatus, errorThrown);
                    }
                    else
                        that.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
                }
            });

            return returnObject;
        };

        this.Delete = function (id, nomeEntidade, FuncaoSucesso, FuncaoErro) {
            /// <summary>Remove registro do CRM.</summary>
            /// <param name="id" type="string">O guid do registro a ser removido.</param>
            /// <param name="nomeEntidade" type="string">O tipo (logical name) do registro a ser removido.</param>
            /// <param name="FuncaoSucesso" type="function">Permite estender o sucess do ajax mandando uma função personalizada para tratar a var data.</param>
            /// <param name="FuncaoErro" type="function">Permite estender o error do ajax mandando uma função personalizada para tratar o erro</param>
            SDKore._argumentosMenorQue(arguments, 2, "Função Delete requer 2 parâmetros");
            SDKore._stringParameterCheck(id, "Guid deve ser uma String");
            SDKore._stringParameterCheck(nomeEntidade, "Nome Entidade deve ser uma String");

            var args = arguments;
            var returnObject;

            this.url = baseUrl + nomeEntidade + "Set(guid'" + id + "')" + this.stringParametros;
            SDKore._stringParameterCheck(this.url, "Url deve ser uma String");

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                datatype: "json",
                async: false,
                url: serverUrl,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                    XMLHttpRequest.setRequestHeader("X-HTTP-Method", "DELETE");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    //é checado se é != de null para poder chamar FuncaoErro sem enviar FuncaoSucesso exemplo: Retrieve(guid,entidade,null,funcErro)
                    if (args.length > 2 && args[2] !== null) {
                        SDKore._functionParameterCheck(FuncaoSucesso, "FuncaoSucesso deve ser uma função");
                        returnObject = FuncaoSucesso(data.d);
                    }
                    else
                        returnObject = data.d;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    if (args.length > 3) {
                        SDKore._functionParameterCheck(FuncaoErro, "FuncaoErro deve ser uma função");
                        FuncaoErro(XmlHttpRequest, textStatus, errorThrown);
                    }
                    else
                        that.ErrorHandler(XmlHttpRequest, textStatus, errorThrown);
                }
            });
            return returnObject;
        };
    },

    CreateLookup: function (id, name, type) {
        /// <summary>Cria objeto de Lookup.</summary>
        /// <param name="id" type="string">O guid do lookup a ser criado.</param>
        /// <param name="name" type="string">O nome do registro do lookup a ser criado.</param>
        /// <param name="type" type="string">O tipo (logical name) do objeto a ser criado.</param>
        /// <returns type="Objeto">Objeto Lookup.</returns>

        var value = new Array();
        value[0] = new Object();
        value[0].id = id;
        value[0].name = name;
        value[0].entityType = type;

        return value;
    },

    CreateLookupWithLookUp: function (lookUp) {
        /// <summary>Cria objeto de Lookup.</summary>
        /// <param name="lookUp" type="object">Lookup criado no WebService.</param>
        /// <returns type="Objeto">Objeto Lookup.</returns>

        var value = new Array();
        value[0] = new Object();
        value[0].id = lookUp.Id;
        value[0].name = lookUp.Name;
        value[0].entityType = lookUp.Type;

        return value;
    },

    DisableAllFormFields: function (onOff) {
        /// <summary>Desabilita ou Habilita todos os campos do formulário.</summary>
        /// <param name="onOff" type="bool">'true' para desabilitar, 'false' para habilitar.</param>

        Xrm.Page.ui.controls.forEach(function (control, index) {
            if (SDKore.VerificaSeControleTemAtributos(control)) {
                control.setDisabled(onOff);
            }
        });
    },

    VerificaSeControleTemAtributos: function (controle) {
        var tipoDeControle = controle.getControlType();
        return tipoDeControle != "iframe" && tipoDeControle != "webresource" && tipoDeControle != "subgrid";
    },

    DesabilitarCampos: function (atributos, bloqueado) {
        /// <summary>Desabilita ou Habilita o campo do formulário.</summary>
        /// <param name="atributos" type="array">recebe um array com o nome dos atributos</param>
        /// <param name="bloqueado" type="bool">'true' para bloquear , 'false' para desbloquear.</param>
        for (var i = 0; i < atributos.length; i++) {
            if (bloqueado)
                Xrm.Page.ui.controls.get(atributos[i]).setDisabled(true);
            else
                Xrm.Page.ui.controls.get(atributos[i]).setDisabled(false);
        }
    },

    RetrieveOptionSetLabel: function (entity, optionSetAttributeName, optionValue) {
        var entityLogicalName = entity;

        var RetrieveAttributeName = optionSetAttributeName;

        var stateValue = optionValue;

        if (typeof (SDK.Metadata) == "undefined") { alert('Biblioteca "SDK.Metadata.js" não encontrada.') }


        var label = '';
        var success = false;

        SDK.Metadata.RetrieveEntity(
            SDK.Metadata.EntityFilters.Attributes,
            entityLogicalName,
            null,
            false,
            false,
            function (entityMetadata) {
                //successRetrieveEntity(entityLogicalName, entityMetadata, RetrieveAttributeName, stateValue, AssignAttributeName);
                for (var i = 0; i < entityMetadata.Attributes.length; i++) {
                    var AttributeMetadata = entityMetadata.Attributes[i];
                    if (success) break;
                    if (AttributeMetadata.SchemaName.toLowerCase() == RetrieveAttributeName.toLowerCase()) {
                        for (var o = 0; o < AttributeMetadata.OptionSet.Options.length; o++) {
                            var option = AttributeMetadata.OptionSet.Options[o];
                            if (option.Value == optionValue) {
                                label = option.Label.UserLocalizedLabel.Label;
                                success = true;
                                break;
                            }
                        }
                    }
                }
            },
            function (XmlHttpRequest, textStatus, errorThrown) {
                alert(errorThrown);
            }
        );

        return label;
    }
}

