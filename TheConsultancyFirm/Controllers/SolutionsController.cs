using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.Controllers
{
    public class SolutionsController : Controller
    {
        private readonly IRelatedItemsRepository _relatedItemsRepository;
        private readonly ISolutionRepository _solutionRepository;

        public SolutionsController(IRelatedItemsRepository relatedItemsRepository, ISolutionRepository solutionRepository, ICustomerRepository customerRepository)
        {
            _relatedItemsRepository = relatedItemsRepository;
            _solutionRepository = solutionRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _solutionRepository.GetAll().Where(c => c.Enabled && !c.Deleted).ToListAsync());
        }

        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            // Parse everything till the first '-' as integer into `solutionId`
            int.TryParse(id.Split('-', 2)[0], out int solutionId);

            var solutionItem = await _solutionRepository.Get(solutionId, false);
            if (solutionItem == null || solutionItem.Deleted || !solutionItem.Enabled) return NotFound();

            if (id != solutionItem.Slug)
                return RedirectToAction("Details", new { id = solutionItem.Slug });

            var relatedItems =
                await _relatedItemsRepository.GetRelatedItems(solutionItem.Id, Enumeration.ContentItemType.Solution);

            var relatedCustomers = solutionItem.CustomerSolutions.Select(cs => cs.Customer).Where(c => !c.Deleted && c.Enabled).Take(12).ToList();
                
            return View(new SolutionDetailViewModel
            {
                Solution = solutionItem,
                ContentItems = relatedItems,
                Customers = relatedCustomers
            });
        }
    }
}
