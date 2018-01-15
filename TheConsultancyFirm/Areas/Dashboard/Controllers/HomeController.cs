using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChartJSCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDownloadLogRepository _downloadLogRepository;

        public HomeController(IDownloadLogRepository downloadLogRepository)
        {
            _downloadLogRepository = downloadLogRepository;
        }

        public async Task<IActionResult> Index()
        {
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
