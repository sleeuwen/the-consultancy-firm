using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    public class TagsController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _tagRepository.GetAll());
        }

        // GET: Dashboard/Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text")] Tag tag)
        {
            if (!ModelState.IsValid) return View(tag);

            if (string.IsNullOrWhiteSpace(tag.Text))
            {
                ModelState.AddModelError("Text", "Tag text can't be empty");
                return View(tag);
            }

            await _tagRepository.Create(tag);
            return RedirectToAction(nameof(Index));
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            await _tagRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("api/dashboard/[controller]/[action]")]
        public async Task<ObjectResult> Autocomplete(string term = "")
        {
            return new ObjectResult(new
            {
                results = (await _tagRepository.Search(term)).Select(t => new {id = t.Id, text = t.Text})
            });
        }
    }
}
