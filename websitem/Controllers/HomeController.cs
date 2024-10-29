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

                        return Json(new { success = true, message = "Message sent successfully." });
                }
                catch (Exception ex)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, message = "Failed to send message.", errorDetails = ex.Message, errors = errors });
                }
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