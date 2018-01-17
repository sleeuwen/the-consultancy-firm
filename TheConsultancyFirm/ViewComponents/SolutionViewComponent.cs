using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.ViewComponents
{
    public class SolutionViewComponent : ViewComponent
    {
        private readonly ISolutionRepository _solutionRepository;

        public SolutionViewComponent(ISolutionRepository solutionRepository)
        {
            _solutionRepository = solutionRepository;
        }

        public IViewComponentResult Invoke()
        {
            var language = HttpContext?.Request?.Cookies[".AspNetCore.Culture"] == "c=en-US|uic=en-US" ? "en" : "nl";
            var solutions = _solutionRepository.GetAll().Where(s => s.Enabled && !s.Deleted && s.Language == language).OrderByDescending(s => s.Date).Take(3).ToList();

            return View(solutions);
        }
    }
}
