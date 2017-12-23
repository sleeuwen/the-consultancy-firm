using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace TheConsultancyFirm.TagHelpers
{
    [HtmlTargetElement(Attributes = "active-action")]
    [HtmlTargetElement(Attributes = "active-controller")]
    [HtmlTargetElement(Attributes = "active-area")]
    public class ActiveRouteTagHelper : TagHelper
    {
        public override int Order => 0;

        private const string ActiveClass = "active";

        [HtmlAttributeName("active-action")]
        public string Action { get; set; }

        [HtmlAttributeName("active-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("active-area")]
        public string Area { get; set; }

        [HtmlAttributeName("asp-action")]
        public string AspAction { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string AspController { get; set; }

        [HtmlAttributeName("asp-area")]
        public string AspArea { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private RouteValueDictionary RouteData => ViewContext.RouteData.Values;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("active-route");
            output.Attributes.RemoveAll("active-controller");
            output.Attributes.RemoveAll("active-action");

            if (Action != null || Controller != null || Area != null)
            {
                ProcessRouteTags(context, output);
            }
            else if (AspAction != null || AspController != null || AspArea != null)
            {
                ProcessAnchorTags(context, output);
            }
        }

        private void ProcessRouteTags(TagHelperContext context, TagHelperOutput output)
        {
            if (Action != null && Action.Split(",").All(a => (string) RouteData["Action"] != a))
                return;

            if (Controller != null && Controller.Split(",").All(c => (string) RouteData["Controller"] != c))
                return;

            if (Area != null && Area.Split(",").All(a => (string) RouteData["Area"] != a))
                return;

            AddActiveClass(context, output);
        }

        private void ProcessAnchorTags(TagHelperContext context, TagHelperOutput output)
        {
            if (AspAction != null && (string) RouteData["Action"] != AspAction)
                return;

            if (AspController != null && (string) RouteData["Controller"] != AspController)
                return;

            if (AspArea != null && (string) RouteData["Area"] != AspArea)
                return;

            AddActiveClass(context, output);
        }

        private void AddActiveClass(TagHelperContext context, TagHelperOutput output)
        {
            var currentClass = output.Attributes.FirstOrDefault(a => a.Name == "class");
            var newClass = ActiveClass;
            if (currentClass != null) newClass += $" {currentClass.Value}";
            if (currentClass != null) output.Attributes.Remove(currentClass);
            output.Attributes.Add(new TagHelperAttribute("class", newClass));
        }
    }
}
