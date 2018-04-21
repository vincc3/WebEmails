using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebEmails.Models;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace WebEmails.Controllers
{
    public class EmailItemsController : ApiController
    {
        private WebEmailsContext db = new WebEmailsContext();

        // GET: api/EmailItems
        public IHttpActionResult GetEmailItems()
        {
            return BadRequest();
        }

        // GET: api/EmailItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult GetEmailItem(int id)
        {
            return BadRequest();
        }

        // PUT: api/EmailItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEmailItem(int id, EmailItem emailItem)
        {
            return BadRequest();
        }

        // POST: api/EmailItems
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> PostEmailItem(EmailItem emailItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool bSent = await SendOneEmail(emailItem);

            if (!bSent)
                return Ok("Failed to send email");
            return Ok("The mail is sent successfully");
        }

        // DELETE: api/EmailItems/5
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteEmailItem(int id)
        {
            return BadRequest();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private async Task<bool> SendOneEmail(EmailItem item)
        {

            Response responseSendGrid = await SendSendGridMessage(item);
            if (responseSendGrid.StatusCode == HttpStatusCode.OK || responseSendGrid.StatusCode == HttpStatusCode.Accepted)
                return true;


            IRestResponse responseMailgun = SendMailgunMessage(item);
            if (responseMailgun.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }
        
        public static IRestResponse SendMailgunMessage(EmailItem item)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3", UriKind.Absolute);
            client.Authenticator =
            new HttpBasicAuthenticator("api",
                                      "key-24a7017e23319bc7f734bbb356380287");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandboxbe96dda245f84804bdc04cd1caaf7483.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxbe96dda245f84804bdc04cd1caaf7483.mailgun.org>");
            request.AddParameter("to", item.EmailTO);
            if (!String.IsNullOrEmpty(item.EmailCC))
                request.AddParameter("cc", item.EmailCC);
            if (!String.IsNullOrEmpty(item.EmailBCC))
                request.AddParameter("bcc", item.EmailBCC);
            request.AddParameter("subject", String.IsNullOrEmpty(item.Subject) ? "Hello" : item.Subject);
            request.AddParameter("text", String.IsNullOrEmpty(item.Contents) ? "Congratulations! You have just sent an email!" : item.Contents);
            request.Method = Method.POST;

            return client.Execute(request);
        }

        static async Task<Response> SendSendGridMessage(EmailItem item)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("test@example.com", "Testing"),
                Subject = (String.IsNullOrEmpty(item.Subject) ? "Hello" : item.Subject),
                PlainTextContent = (String.IsNullOrEmpty(item.Contents) ? "Congratulations! You have just sent an email!" : item.Contents)
            };
            msg.AddTo(new EmailAddress(item.EmailTO, item.EmailTO));
            if (!String.IsNullOrEmpty(item.EmailCC))
                msg.AddCc(new EmailAddress(item.EmailCC, item.EmailCC));
            if (!String.IsNullOrEmpty(item.EmailBCC))
                msg.AddBcc(new EmailAddress(item.EmailBCC, item.EmailBCC));
            Response response = await client.SendEmailAsync(msg);
            return response;
        }


    }
}