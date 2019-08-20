using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Crm.Sdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class PostUpdateLog : IPlugin
    {
        public void Execute(IPluginExecutionContext context)
        {
            try
            {
                DynamicEntity preEntity = context.PreEntityImages["PreIncidentImage"] as DynamicEntity;
                DynamicEntity postEntity = context.PostEntityImages["PostIncidentImage"] as DynamicEntity;

                var incidentid = PluginHelper.GetEntityId(context);

                var usuario = new Usuario(new Organizacao(context.OrganizationName));
                usuario = usuario.PesquisaPor(context.UserId);

                if (postEntity != null && preEntity != null)
                    this.ValidarAlteracoes(incidentid, preEntity, postEntity, new Organizacao(context.OrganizationName), usuario);

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM Application", String.Format("Houve um problema ao executar o plugin 'incident.PostUpdateLog': Mensagem: {0} -- StackTrace: {1} \n--{2}", ex.Message, ex.StackTrace, ex.InnerException));
            }

        }

        private void Log(string mensagem, string stack, string inner)
        {
            EventLog.WriteEntry("CRM Application", String.Format("Houve um problema ao executar o plugin 'incident.PostUpdateLog': Mensagem: {0} -- StackTrace: {1} \n--{2}", mensagem, stack, inner));
        }

        private void ValidarAlteracoes(Guid incidentid, DynamicEntity preEntity, DynamicEntity postEntity, Organizacao organizacao, Usuario usuario)
        {
            var msgValidacao = new string[] { "Alterações: DE --> PARA", "" };

            foreach (Property p in preEntity.Properties)
            {
                var preItem = preEntity.Properties[p.Name];
                object posItem = null;
                if (!postEntity.Properties.Contains(p.Name))
                    EventLog.WriteEntry("CRM Application PostUpdateLog", "Campo não existe em PostIncidentImage: " + p.Name);
                else
                {
                    posItem = postEntity.Properties[p.Name];

                    switch (preItem.GetType().ToString())
                    {
                        case "System.String":
                            {
                                if (preItem.ToString() != posItem.ToString())
                                    msgValidacao[1] += "\n " + p.Name + ": " + preItem.ToString() + " --> " + posItem.ToString();
                            }
                            break;
                        case "Microsoft.Crm.Sdk.Customer":
                            {
                                var _preitem = preItem as Customer;
                                var _positem = posItem as Customer;

                                if (_preitem.Value != _positem.Value)
                                    msgValidacao[1] += "\n " + p.Name + ": " + _preitem.name + " --> " + _positem.name;
                            }
                            break;
                        case "Microsoft.Crm.Sdk.Picklist":
                            {
                                var _preitem = preItem as Picklist;
                                var _positem = posItem as Picklist;

                                if (_preitem.Value != _positem.Value)
                                    msgValidacao[1] += "\n " + p.Name + ": " + _preitem.name + " --> " + _positem.name;
                            }
                            break;
                        case "Microsoft.Crm.Sdk.CrmDateTime":
                            {
                                if (p.Name != "modifiedon")
                                {
                                    var _preitem = preItem as CrmDateTime;
                                    var _positem = posItem as CrmDateTime;

                                    if (_preitem.Value != _positem.Value)
                                        msgValidacao[1] += "\n " + p.Name + ": " + Convert.ToDateTime(_preitem.Value).ToString() + " --> " + Convert.ToDateTime(_positem.Value).ToString();
                                }
                            }
                            break;
                        case "Microsoft.Crm.Sdk.Lookup":
                            {
                                var _preitem = preItem as Lookup;
                                var _positem = posItem as Lookup;

                                if (_preitem.Value != _positem.Value)
                                    msgValidacao[1] += "\n " + p.Name + ": " + _preitem.name + " --> " + _positem.name;
                            }
                            break;
                        case "Microsoft.Crm.Sdk.CrmBoolean":
                            {
                                var _preitem = preItem as CrmBoolean;
                                var _positem = posItem as CrmBoolean;

                                if (_preitem.Value != _positem.Value)
                                    msgValidacao[1] += "\n " + p.Name + ": " + _preitem.name + " --> " + _positem.name;
                            }
                            break;
                        case "Microsoft.Crm.Sdk.CrmNumber":
                            {
                                var _preitem = preItem as CrmNumber;
                                var _positem = posItem as CrmNumber;

                                if (_preitem.Value != _positem.Value)
                                    msgValidacao[1] += "\n " + p.Name + ": " + _preitem.Value + " --> " + _positem.Value;
                            }
                            break;
                        case "Microsoft.Crm.Sdk.Owner":
                            {
                                if (p.Name == "ownerid")
                                {
                                    var _preitem = preItem as Owner;
                                    var _positem = posItem as Owner;
                                    if (_preitem.Value != _positem.Value)
                                    {
                                        var usuarioDestino = new Usuario(organizacao);
                                        usuarioDestino = usuarioDestino.PesquisaPor(_positem.Value);

                                        var log = new LogOcorrencia(organizacao);
                                        log.Usuario = string.Format("{0} {1}", usuario.Nome, usuario.Sobrenome);

                                        log.UsuarioId = usuario.Id;
                                        log.Nome = "Ocorrencia alterada";
                                        log.OcorrenciaId = incidentid;
                                        log.Alteracoes = "Proprietário Anterior: " + _preitem.name + " \n Proprietário Novo: " + _positem.name;
                                        log.Categoria = Domain.Model.Enum.CategoriaLogOcorrencia.PorProprietario;
                                        log.DataAlteracao = DateTime.Now;

                                        log.GrupoDestino = usuarioDestino.GrupoCallCenter;
                                        log.GrupoProprietario = usuario.GrupoCallCenter;

                                        log.Salvar();
                                    }
                                }
                            }
                            break;

                        case "Microsoft.Crm.Sdk.Status":
                            {
                                if (p.Name == "statuscode")
                                {
                                    var _preitem = preItem as Status;
                                    var _positem = posItem as Status;
                                    if (_preitem.Value != _positem.Value)
                                    {
                                        var log = new LogOcorrencia(organizacao);
                                        log.Usuario = string.Format("{0} {1}", usuario.Nome, usuario.Sobrenome);
                                        log.UsuarioId = usuario.Id;
                                        log.Nome = "Ocorrencia alterada";
                                        log.OcorrenciaId = incidentid;
                                        log.Status = _positem.name;
                                        log.Alteracoes = "Status Anterior: " + _preitem.name + " \n Status Novo: " + _positem.name;
                                        log.Categoria = Domain.Model.Enum.CategoriaLogOcorrencia.PorStatus;
                                        log.DataAlteracao = DateTime.Now;

                                        log.Salvar();
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            if (msgValidacao[1] != string.Empty)
            {
                var log = new LogOcorrencia(organizacao);
                log.Usuario = string.Format("{0} {1}", usuario.Nome, usuario.Sobrenome);
                log.Nome = "Ocorrencia alterada";
                log.UsuarioId = usuario.Id;
                log.OcorrenciaId = incidentid;
                log.Alteracoes = msgValidacao[0] + "\n" + msgValidacao[1];
                log.Categoria = Domain.Model.Enum.CategoriaLogOcorrencia.Diversos;
                log.DataAlteracao = DateTime.Now;
                log.Salvar();
            }
        }
    }
}
