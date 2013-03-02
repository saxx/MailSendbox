using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MailSendbox.Code;
using MailSendbox.Hubs;

namespace MailSendbox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapHubs();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var mailRepository = new MailRepository(new Pop3Client());
            MailFetcherFactory.Current = new MailFetcher(mailRepository);
            MailFetcherFactory.Current.StartAsyncFetching();
            MailsHubEventListener.Current.Init();
        }

        protected void Application_Stop()
        {
            MailFetcherFactory.Current.StopAsyncFetching();
        }
    }
}