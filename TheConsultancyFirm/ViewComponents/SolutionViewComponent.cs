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
            var solutions = _solutionRepository.GetAll().OrderByDescending(c => c.Date).Take(3).ToList();

            return View(solutions);
        }
    }
}
