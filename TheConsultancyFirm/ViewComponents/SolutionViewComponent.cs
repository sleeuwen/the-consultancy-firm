using System.Linq;
using Microsoft.AspNetCore.Localization;
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
            var language = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture
                .TwoLetterISOLanguageName;
            var solutions = _solutionRepository.GetAll().Where(s => s.Enabled && !s.Deleted && s.Language == language).OrderByDescending(s => s.Date).Take(3).ToList();

            return View(solutions);
        }
    }
}
