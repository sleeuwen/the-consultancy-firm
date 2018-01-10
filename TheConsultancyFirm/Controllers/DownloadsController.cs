using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly IDownloadRepository _downloadRepository;

        public DownloadsController(IDownloadRepository downloadRepository)
        {
            _downloadRepository = downloadRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DownloadsViewModel
            {
                MostDownloaded = await _downloadRepository.GetAll().Where(c => c.Enabled && !c.Deleted).OrderByDescending(d => d.AmountOfDownloads)
                    .FirstOrDefaultAsync(),
                MostRecent = await _downloadRepository.GetAll().Where(c => c.Enabled && !c.Deleted).OrderByDescending(c => c.Date).FirstOrDefaultAsync()
            };

            viewModel.AllDownloads = await _downloadRepository.GetAll().Where(d => d.Id != viewModel.MostDownloaded.Id && !d.Deleted && !d.Deleted)
                .OrderBy(c => c.Date).Skip(1).ToListAsync();

            return View(viewModel);
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var selected = await _downloadRepository.Get(id);
            if (selected.Deleted || selected.Enabled == false) return View("Index");

            var viewModel = new DownloadsViewModel
            {
                Selected = selected,
                AllDownloads = await _downloadRepository.GetAll().Where(c => c.Id != id && !c.Deleted && !c.Deleted).ToListAsync()
            };

            return View("/Views/Downloads/Index.cshtml", viewModel);
        }
    }
}
