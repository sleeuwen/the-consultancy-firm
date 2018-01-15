﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Models;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class HomeController : Controller
    {
        private const string WebsiteCode = "167536233";
        private readonly IDownloadLogRepository _downloadLogRepository;
        private readonly IHostingEnvironment _environment;
        private AnalyticsService _service;

        public HomeController(IHostingEnvironment environment, IDownloadLogRepository downloadLogRepository)
        {
            _environment = environment;
            _downloadLogRepository = downloadLogRepository;
            SetupGoogleCredentials();
        }

        public async Task<IActionResult> Index()
        {
            await SetDownloadsGraph();
            SetSessionGraph();
            return View();
        }

        [Route("api/dashboard/[controller]/[action]")]
        private int GetCurrentActiveUsers()
        {
            var request = _service.Data.Realtime.Get("ga:" + WebsiteCode, "rt:activeUsers");

            var data = request.Execute();
            return int.Parse(data.Rows[0][0]);
        }

        private void SetSessionGraph()
        {
            var request = _service.Data.Ga.Get(
                "ga:" + WebsiteCode,
                DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd"),
                DateTime.Today.ToString("yyyy-MM-dd"),
                "ga:sessions");
            request.Dimensions = "ga:year,ga:month,ga:day";
            var data = request.Execute();

            var days = EachDay(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow)
                .Select(day => day.ToString("dd MMMM"))
                .ToList();

            var values = data.Rows
                .Select(row => double.Parse(row[3]))
                .ToList();

            var datas = new ChartJSCore.Models.Data
            {
                Labels = days,
                Datasets = new List<Dataset> {CreateDataset(values, "Totaal aantal sessie's")}
            };

            var chart = new Chart
            {
                Type = "line",
                Data = datas
            };

            ViewBag.sessions = chart;
        }

        private async Task SetDownloadsGraph()
        {
            var downloads = await _downloadLogRepository.GetDownloadsLastWeek();

            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var values = new List<double>();

            for (var i = 0; i <= 7; i++)
            {
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

            var labels = EachDay(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow)
                .Select(day => day.ToString("dd MMMM"))
                .ToList();

            var datas = new ChartJSCore.Models.Data
            {
                Labels = labels,
                Datasets = new List<Dataset> {CreateDataset(values, "Totaal aantal downloads")}
            };

            var chart = new Chart
            {
                Type = "line",
                Data = datas
            };

            ViewBag.chart = chart;
        }

        private LineDataset CreateDataset(List<double> values, string title)
        {
            return new LineDataset
            {
                Label = title,
                Data = values,
                Fill = false,
                CubicInterpolationMode = "monotone",
                LineTension = 0.2,
                BackgroundColor = "rgba(93, 194, 196, 0.4)",
                BorderColor = "rgba(75,192,192,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int>(),
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string> {"rgba(93, 194, 196, 1)"},
                PointBackgroundColor = new List<string> {"#fff"},
                PointBorderWidth = new List<int> {1},
                PointHoverRadius = new List<int> {5},
                PointHoverBackgroundColor = new List<string> {"rgba(93, 194, 196, 1)"},
                PointHoverBorderColor = new List<string> {"rgba(220,220,220,1)"},
                PointHoverBorderWidth = new List<int> {2},
                PointRadius = new List<int> {3},
                PointHitRadius = new List<int> {10},
                SpanGaps = false
            };
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private void SetupGoogleCredentials()
        {
            var scopes = new[]
            {
                AnalyticsService.Scope.Analytics, // view and manage your analytics data    
                AnalyticsService.Scope.AnalyticsEdit, // edit management actives    
                AnalyticsService.Scope.AnalyticsManageUsers, // manage users    
                AnalyticsService.Scope.AnalyticsReadonly
            };

            ServiceAccountCredential credential;
            using (var stream = new FileStream(_environment.WebRootPath + "/TheConsultancyFirm-033bc6305f69.json",
                FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(scopes)
                    .UnderlyingCredential as ServiceAccountCredential;
            }

            _service = new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });
        }
    }
}
