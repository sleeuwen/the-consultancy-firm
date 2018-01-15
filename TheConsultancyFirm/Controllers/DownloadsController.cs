﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly IDownloadRepository _downloadRepository;
        private readonly IDownloadLogRepository _downloadLogRepository;

        public DownloadsController(IDownloadRepository downloadRepository, IDownloadLogRepository downloadLogRepository)
        {
            _downloadRepository = downloadRepository;
            _downloadLogRepository = downloadLogRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DownloadsViewModel
            {
                MostDownloaded = await _downloadRepository.GetAll().Where(d => d.Enabled && !d.Deleted).OrderByDescending(d => d.AmountOfDownloads)
                    .FirstOrDefaultAsync(),
                MostRecent = await _downloadRepository.GetAll().Where(d => d.Enabled && !d.Deleted).OrderByDescending(d => d.Date).FirstOrDefaultAsync()
            };

            viewModel.AllDownloads = await _downloadRepository.GetAll().Where(d => d.Id != viewModel.MostDownloaded.Id && d.Enabled && !d.Deleted)
                .OrderBy(d => d.Date).Skip(1).ToListAsync();

            return View(viewModel);
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var selected = await _downloadRepository.Get(id);
            if (selected.Deleted || !selected.Enabled) return NotFound();

            var viewModel = new DownloadsViewModel
            {
                Selected = selected,
                AllDownloads = await _downloadRepository.GetAll().Where(d => d.Id != id && d.Enabled && !d.Deleted).ToListAsync()
            };

            return View("/Views/Downloads/Index.cshtml", viewModel);
        }

        [Route("api/[controller]/[action]/{id}")]
        public async Task LogDownload(int id)
        {
            var downloadLog = new DownloadLog
            {
                Date = DateTime.UtcNow,
                DownloadId = id
            };

            await _downloadLogRepository.Log(downloadLog);
        }
    }
}
