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
using System.Threading.Tasks;
using ChartJSCore.Models;
using TheConsultancyFirm.Repositories;


namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _environment;



        //private string keyFilePath = @"/TheConsultancyFirm-9dac4a07fd51.p12";
        private string serviceAccountEmail = "tcf-178@theconsultancyfirm-191708.iam.gserviceaccount.com";
        private string keyPassword = "notasecret";
        private string websiteCode = "167536233";
        private AnalyticsService service = null;
        private List<ChartRecord> visitsData = new List<ChartRecord>();


        private readonly IDownloadLogRepository _downloadLogRepository;

         public HomeController(IHostingEnvironment environment, IDownloadLogRepository downloadLogRepository)
        {
            _environment = environment;
            _downloadLogRepository = downloadLogRepository;
        }

        public async Task<IActionResult> Index()
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
            
            
            var downloads = await _downloadLogRepository.GetDownloadsLastWeek();

            var chart = new Chart {Type = "line"};

            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var days = new List<string>();
            var values = new List<double>();

            for (var i = 0; i <= 7; i++)
            {
                days.Add(lastWeek.ToString("dd MMMM"));
                var found = false;
                foreach (var date in downloads.Keys)
                {
                    if (date.ToString("dd MMMM") != lastWeek.ToString("dd MMMM")) continue;
                    values.Add(downloads[date]);
                    found = true;
                    break;
                }
                if (!found) values.Add(0);
                
                lastWeek = lastWeek.AddDays(1);
            }

            var data = new ChartJSCore.Models.Data
            {
                Labels = days
            };

            var dataset = new LineDataset
            {
                Label = "Totaal aantal downloads",
                Data = values,
                Fill = false,
                CubicInterpolationMode = "monotone",
                LineTension = 0.2,
                BackgroundColor = "rgba(93, 194, 196, 0.4)",
                BorderColor = "rgba(75,192,192,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(93, 194, 196, 1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(93, 194, 196, 1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 3 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset> {dataset};

            chart.Data = data;

            ViewBag.chart = chart;
            return View();
        }
    }
}
