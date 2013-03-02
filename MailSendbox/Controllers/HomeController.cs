using System.Linq;
using MailSendbox.Code;
using System.Web.Mvc;

namespace MailSendbox.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var repo = new MailRepository(new Pop3Client());
            var mails = repo.Get();
            return View(mails);
        }
    }
}
