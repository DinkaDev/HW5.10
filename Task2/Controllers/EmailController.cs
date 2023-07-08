using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Mail;
using Task2.Models;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Task2.Controllers
{
    public class EmailController : Controller
    {
        private readonly IConfiguration _configuration;

        public EmailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(EmailMessage email)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(username, username));
            message.To.Add(new MailboxAddress("", email.To));
            message.Subject = email.Subject;
            message.Body = new TextPart("plain")
            {
                Text = email.Body
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            return RedirectToAction("Index");
        }
    }
}
