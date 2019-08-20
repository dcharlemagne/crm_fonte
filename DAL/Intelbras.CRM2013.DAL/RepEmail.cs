using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepEmail<T> : CrmServiceRepository<T>, IEmail<T>
    {
        public List<T> ListarPor(Guid email)
        {
            var query = GetQueryExpression<T>(true);

            //Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, accountid);
            //query.Criteria.Conditions.Add(cond1);

            //Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            //query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(DateTime dataCriacaoInicial, DateTime dataCriacaoFinal, int status)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, status));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.GreaterEqual, dataCriacaoInicial));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.LessEqual, dataCriacaoFinal));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public void EnviarEmail(Guid emailId)
        {
            Microsoft.Crm.Sdk.Messages.SendEmailRequest request = new Microsoft.Crm.Sdk.Messages.SendEmailRequest();
            request.EmailId = emailId;
            request.TrackingToken = "";
            request.IssueSend = true;
            Microsoft.Crm.Sdk.Messages.SendEmailResponse response = (Microsoft.Crm.Sdk.Messages.SendEmailResponse)base.Provider.Execute(request);
        }

        public void EnviaEmailParaDestinatariosNaoCRM(Email email)
        {
            Entity eEmail = new Entity("email");
            eEmail.Attributes.Add("subject", email.Assunto);
            eEmail.Attributes.Add("description", email.Mensagem);
            eEmail.Attributes.Add("directioncode", true);            
            if (email.ReferenteA != null)
                eEmail.Attributes.Add("regardingobjectid", new EntityReference("incident", email.ReferenteA.Id));

            Entity sender = new Entity("activityparty");
            sender["partyid"] = new EntityReference("systemuser", email.De[0].Id);

            eEmail.Attributes.Add("from", new Entity[] { sender });

            EntityCollection colAP = new EntityCollection();

            foreach (string emailAddress in email.ParaEnderecos.Split(';'))
            {
                Entity activityParty = new Entity("activityparty");

                activityParty["addressused"] = emailAddress;
                colAP.Entities.Add(activityParty);
            }

            eEmail.Attributes.Add("to", colAP);

            eEmail.Id = this.Provider.Create(eEmail);

            if (email.Anexos != null)
            {
                foreach (var anexo in email.Anexos)
                {
                    Entity eAnexo = new Entity("activitymimeattachment");
                    eAnexo.Attributes.Add("objectid", new EntityReference("email", eEmail.Id));
                    eAnexo.Attributes.Add("objecttypecode", "email");
                    eAnexo.Attributes.Add("filename", anexo.FileName);
                    eAnexo.Attributes.Add("mimetype", anexo.MimeType);
                    eAnexo.Attributes.Add("body", anexo.BodyBase64);

                    eAnexo.Id = this.Provider.Create(eAnexo);
                }
            }

            this.EnviarEmail(eEmail.Id);
        }

        public Boolean AlterarStatus(Guid emailid, int statuscode)
        {

            int statecode = 1;
            switch (statuscode)
            {
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Rascunho:
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Falha:
                    statecode = (int)Intelbras.CRM2013.Domain.Enum.Email.StateCode.Aberto;
                    break;
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Concluida:
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Enviado:
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Recebido:
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.EnvioPendente:
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Enviando:
                    statecode = (int)Intelbras.CRM2013.Domain.Enum.Email.StateCode.Concluido;
                    break;
                case (int)Intelbras.CRM2013.Domain.Enum.Email.StatusEmail.Cancelada:
                    statecode = (int)Intelbras.CRM2013.Domain.Enum.Email.StateCode.Cancelado;
                    break;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("email", emailid),
                State = new OptionSetValue(statecode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public void EnviaEmailComLogdeRotinas(string textoEmail, string assunto, string nomeArquivo, string emailTo)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "outlook.intelbras.com.br";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("intelbras\\mail.crm2013", "EjPS2hK5VeCH2SBfdxxPjQ6Q");
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");
            mail.From = new MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");            
            mail.To.Add(new MailAddress(emailTo));
            mail.Subject = assunto;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

            if (nomeArquivo.Length > 0)
            {
                Attachment anexado = new Attachment("c:\\temp\\" + nomeArquivo, MediaTypeNames.Application.Octet);
                mail.Attachments.Add(anexado);
            }
            

            mail.Body = textoEmail;

            try
            {
                client.Send(mail);
            }
            catch (System.Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
            finally
            {
                mail = null;
            }
        }

        public void EnviaEmailExternos(string textoEmail, string assunto, string nomeArquivo, string emailTo, string remetente)
        {
            var TipoAmbiente = SDKore.Configuration.ConfigurationManager.GetSettingValue("Ambiente");
            if(TipoAmbiente == "develop" || TipoAmbiente == "Homologacao") {
                assunto+= assunto + "(" + TipoAmbiente + ")";
                textoEmail = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>"+
                    "<html xmlns='http://www.w3.org/1999/xhtml'><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><title>Intelbras</title></head>"+
                    "<body style='background-color: #FFFFFF;'><div style='width:72%;float: left;border-right: 1px solid #000;'>"+textoEmail+"</div>"+
                    "<div style='width:27%;float: left;padding-left: 5px;'><p>Ambiente: <strong>"+TipoAmbiente+"</strong></p><p>Em produção este e-mail seria enviado para: "+emailTo+"</p></div></body></html>";
                emailTo = SDKore.Configuration.ConfigurationManager.GetSettingValue("redirecionamentoEmails");
            }

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "outlook.intelbras.com.br";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("ems", "8Rj5r7H4");
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("nao_responda@intelbras.com.br", remetente);
            mail.From = new MailAddress("nao_responda@intelbras.com.br", remetente);
            mail.To.Add(new MailAddress(emailTo));
            mail.Subject = assunto;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(15));

            if (nomeArquivo.Length > 0)
            {
                Attachment anexado = new Attachment("c:\\temp\\" + nomeArquivo, MediaTypeNames.Application.Octet);
                mail.Attachments.Add(anexado);
            }
            
            mail.Body = textoEmail;

            try
            {
                client.Send(mail);
            }
            catch (System.Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
            finally
            {
                mail = null;
            }
        }

        public List<T> ListarPorReferenteA(Guid referenteA)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("regardingobjectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, referenteA);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void EnviaEmailComUsuariosDesativados(string textoEmail, string assunto)
        {
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Host = "outlook.intelbras.com.br";
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("intelbras\\mail.crm2013", "EjPS2hK5VeCH2SBfdxxPjQ6Q");
            MailMessage mail = new MailMessage();
            mail.Sender = new System.Net.Mail.MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");
            mail.From = new MailAddress("mail.crm2013@intelbras.com.br", "Administrador CRM Intelbras");
            mail.To.Add(new MailAddress("grupo.crm_ti@intelbras.com.br"));
            mail.To.Add(new MailAddress("alberto.squarca@intelbras.com.br"));
            mail.To.Add(new MailAddress("lucas.cruz@intelbras.com.br"));
            mail.Subject = assunto;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            mail.Body = textoEmail;

            try
            {
                client.Send(mail);
            }
            catch (System.Exception erro)
            {
                throw new ArgumentException(erro.Message);
            }
            finally
            {
                mail = null;
            }            
        }

        public Email ObterDataUltimoEmailEnviado(Guid ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            RetrieveMultipleRequest retrieveMultiple;

            StringBuilder fetch = new StringBuilder();
            fetch.Append(@"<fetch mapping='logical' count='1' aggregate='true'> 
                              <entity name='email'>
                                <attribute name='modifiedon' alias='modifiedon_max' aggregate='max' />
                                <filter>
                                   <condition attribute='subject' operator='like' value='%ALERTA ISOL –%'>                                  
                                  </condition>");

                fetch.AppendFormat("<condition attribute='regardingobjectid' operator='eq' value='{0}' />", ocorrencia);
            fetch.Append(@"  </filter>
                            </entity>                            
                           </fetch>");

            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch.ToString())
            };
            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
            if (collection.Entities.Count > 0)
            {
                var item = collection.Entities[0];
                var email = new Email(OrganizationName, IsOffline, Provider);
                if (item.Contains("modifiedon_max"))
                {
                    email.ModificadoEm = Convert.ToDateTime(((AliasedValue)item.Attributes["modifiedon_max"]).Value);
                    return email;
                }
            }
            return null;
        }

        public List<T> ListarPor(string nomeEntidade, string nomeCampo, string valorCampo)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();



            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='email'>");
            strFetchXml.Append("<all-attributes />   ");
            strFetchXml.Append("<order attribute='createdon' descending='true' />");
            strFetchXml.AppendFormat("<link-entity name='{0}' from='{0}id' to='regardingobjectid' alias='an'>", nomeEntidade);
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='{0}' operator='eq' value='{1}' />", nomeCampo, valorCampo);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }
    }
}
