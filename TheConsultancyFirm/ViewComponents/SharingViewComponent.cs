using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.ViewModels;

namespace TheConsultancyFirm.ViewComponents
{
    public class SharingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(object block)
        {
            var model = new SharingViewModel();
            switch (block)
            {
                case Case c:
                    model.Title = c.Title;
                    model.Description = c.SharingDescription;
                    model.Link = "/cases/" + c.Id;
                    model.ImageUrl = c.PhotoPath;
                    break;
                case NewsItem n:
                    model.Title = n.Title;
                    model.Description = n.SharingDescription;
                    model.Link = "/newsitems/" + n.Id;
                    model.ImageUrl = n.PhotoPath;
                    break;
                case Solution s:
                    model.Title = s.Title;
                    model.Description = s.SharingDescription;
                    model.Link = "/solutions/" + s.Id;
                    model.ImageUrl = null;
                    break;
            }
            model.Twitter = $"https://twitter.com/intent/tweet?text={model.Description}";
            return View(model);
        }
    }
}
