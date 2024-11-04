using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using websitem.Models;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Configuration;


namespace websitem.Controllers
{
    public class MailModel
    {
        public string Title { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string ContactMethod { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }

        // Date alanı burada yer almıyor çünkü SQL'de ayarlandı.
    }

    public class SuggestModel
    {
        [Required(ErrorMessage = "Topic is required.")]
        public string Topic { get; set; }
        [Required(ErrorMessage = "Message is required.")]
        public string Mesaj {  get; set; }


    }

    public class HomeController : Controller
    {
        private websitemEntities db = new websitemEntities();

        [HttpPost]
        public JsonResult SubmitContactForm(MailModel contact)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors)
                                        .Select(x => x.ErrorMessage)
                                        .ToList();
                return Json(new { success = false, message = "Validation errors occurred.", errors });
            }
            else
            {
                try
                {
                    // SQL komutu ile veritabanına ekleme
                    db.Database.ExecuteSqlCommand("INSERT INTO contact_tablo (title, name, surname, email, phone, contactmethod, message) " +
                                                  "VALUES (@Title, @Name, @Surname, @Email, @Phone, @ContactMethod, @Message)",
                        new SqlParameter("@Title", contact.Title),
                        new SqlParameter("@Name", contact.Name),
                        new SqlParameter("@Surname", contact.Surname),
                        new SqlParameter("@Email", contact.Email),
                        new SqlParameter("@Phone", contact.Phone),
                        new SqlParameter("@ContactMethod", contact.ContactMethod),
                        new SqlParameter("@Message", contact.Message));

                    // E-posta gönderme işlemi
                    SendEmail(contact);

                    return Json(new { success = true, message = "Message sent and saved successfully." });
                }
                catch (Exception ex)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Failed to send message.", errorDetails = ex.Message, errors = errors });
                }
            }
        }

        private void SendEmail(MailModel contact)
        {
            var message = new MailMessage
            {
                From = new MailAddress(contact.Email), // Gönderen adres
                Subject = "Websitem Contact Mesaji",
                Body = $"Title: {contact.Title}\nName: {contact.Name}\nSurname: {contact.Surname}\nEmail: {contact.Email}\nPhone: {contact.Phone}\nPreferred Communication Method: {contact.ContactMethod}\nMessage: {contact.Message}",
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress("sebahattin.kurtulus16@gmail.com"));  // Alıcı adresi

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                smtp.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        } 

        [HttpPost]
        public JsonResult SubmitSuggestForm(SuggestModel suggest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(x => x.Value.Errors)
                                        .Select(x => x.ErrorMessage)
                                        .ToList();
                return Json(new { success = false, message = "Validation errors occurred.", errors });
            }
            else
            {
                try
                {
                    // SQL komutu ile veritabanına ekleme
                    db.Database.ExecuteSqlCommand("INSERT INTO suggest_tablo (topic, mesaj) " +
                                                  "VALUES (@Topic , @Mesaj)",
                        new SqlParameter("@Topic", suggest.Topic),
                        new SqlParameter("@Mesaj", suggest.Mesaj));
                    // E-posta gönderme işlemi
                    SendSuggestion(suggest);

                    return Json(new { success = true, message = "Message sent successfully." });
                }
                catch (Exception ex)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Failed to send message.", errorDetails = ex.Message, errors = errors });
                }
            }
        }

        private void SendSuggestion(SuggestModel suggest)
        {
            var message = new MailMessage
            {
                Subject = "Websitem Suggest Mesajı",
                Body = $"Topic: {suggest.Topic}\nMessage: {suggest.Mesaj}",
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress("sebahattin.kurtulus16@gmail.com"));  // Alıcı adresi

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                smtp.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Skills()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}