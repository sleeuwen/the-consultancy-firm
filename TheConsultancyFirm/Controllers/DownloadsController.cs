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

        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new DownloadsViewModel();

            if (id != null)
            {
                viewModel.Selected = await _downloadRepository.Get((int) id);
                viewModel.AllDownloads = await _downloadRepository.GetAll().Where(c => c.Id != id).ToListAsync();
            }
            else
            {
                viewModel.MostDownloaded =
                    await _downloadRepository.GetAll().OrderByDescending(d => d.AmountOfDownloads).FirstOrDefaultAsync();
                viewModel.MostRecent =
                    await _downloadRepository.GetAll().OrderByDescending(c => c.Date).FirstOrDefaultAsync();

                viewModel.AllDownloads = await _downloadRepository.GetAll().Where(d => d.Id != viewModel.MostDownloaded.Id).OrderBy(c => c.Date).Skip(1).ToListAsync();
            }

            return View(viewModel);
        }
    }
}
