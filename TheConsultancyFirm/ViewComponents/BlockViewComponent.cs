using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.ViewComponents
{
    public class BlockViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(object block)
        {
            switch (block)
            {
                case CarouselBlock c:
                    return View("Carousel", c);
                case QuoteBlock q:
                    return View("Quote", q);
                case SolutionAdvantagesBlock s:
                    return View("SolutionAdvantages", s);
                case TextBlock t:
                    return View("Text", t);
                default:
                    return null;
            }
        }
    }
}
