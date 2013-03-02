using AppfailReporting.Mvc;
using System.Web.Mvc;

namespace MailSendbox
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AppfailReportAttribute()); 
            filters.Add(new HandleErrorAttribute());
        }
    }
}