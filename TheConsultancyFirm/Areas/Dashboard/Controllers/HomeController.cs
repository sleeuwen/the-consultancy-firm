using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        //private string keyFilePath = @"/TheConsultancyFirm-9dac4a07fd51.p12";
        private string serviceAccountEmail = "tcf-178@theconsultancyfirm-191708.iam.gserviceaccount.com";
        private string keyPassword = "notasecret";
        private string websiteCode = "167536233";
        private AnalyticsService service = null;
        private List<ChartRecord> visitsData = new List<ChartRecord>();

        public IActionResult Index()
        {
            var scopes =
                new string[] {
                    AnalyticsService.Scope.Analytics,              // view and manage your analytics data    
                    AnalyticsService.Scope.AnalyticsEdit,          // edit management actives    
                    AnalyticsService.Scope.AnalyticsManageUsers,   // manage users    
                    AnalyticsService.Scope.AnalyticsReadonly};
            //using (var stream = new FileStream("~/TheConsultancyFirm-033bc6305f69.json", FileMode.Open, FileAccess.Read))
            //{
            //    var credential = GoogleCredential.FromStream(stream)
            //        .CreateScoped(scopes)
            //        .UnderlyingCredential as ServiceAccountCredential;


            //}
            var keyFilePath = _environment.WebRootPath + "/TheConsultancyFirm-9dac4a07fd51.p12";

            var certificate = new X509Certificate2(keyFilePath, keyPassword, X509KeyStorageFlags.Exportable);
            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = scopes
            }.FromCertificate(certificate));
            service = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            DataResource.GaResource.GetRequest request = service.Data.Ga.Get(
                "ga:" + websiteCode,
                DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd"),
                DateTime.Today.ToString("yyyy-MM-dd"),
                "ga:sessions");
            request.Dimensions = "ga:year,ga:month,ga:day";
            var data = request.Execute();

            foreach (var row in data.Rows)
            {
                visitsData.Add(new ChartRecord(new DateTime(int.Parse(row[0]), int.Parse(row[1]), int.Parse(row[2])).ToString("MM-dd-yyyy"), int.Parse(row[3])));
            }
            var a = 0;
            return View();
        }
    }
}
