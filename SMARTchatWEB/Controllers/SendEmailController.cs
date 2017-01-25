using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMARTchat.BLL.Services;
using SMARTchat.BLL.DTOs;
using System.IO;
using SMARTchatWEB.Models;


namespace SMARTchatWEB.Controllers
{
    public class SendEmailController : Controller
    {
        private static string Id { get; set; }

        [HttpGet]
        public ActionResult Create(ChannelDTO channel)
        {
            var model=new EmailViewModel();
            model.Channel = channel;
            Id = channel.ChannelId;
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(EmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                string smtpUserName = "kpi_ep01@mail.ru";
                string smtpPassword = "kpiep01";
                string smtpHost = "smtp.mail.ru";
                int smtpPort = 2525;

                string emailTo = model.EmailTo;
                string subject = model.Subject;
                StreamReader reader = new StreamReader(Server.MapPath("~/Views/SendEmail/MessageBody.html"));
                string bodyHtml = reader.ReadToEnd();
                string body = string.Format("Hello,<b>{0} sended invit for you!</b><br/>Email:{1}<br/>" + "Channel name:<strong> {3}</strong><br/>" +
                    "For join fill the channel name field and click the button.</br><br/>{2}</br>" + bodyHtml, model.UserName, model.EmailFrom, model.Message,model.Channel.Name);

                EmailServices serv = new EmailServices();

                bool kq = serv.Send(smtpUserName, smtpPassword, smtpHost, smtpPort, emailTo, subject, body);
                if (kq) ModelState.AddModelError("", "Sended");
                else ModelState.AddModelError("", "Not sended");
            }
            return View(model);
        }
    }
}