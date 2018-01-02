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

        [HtmlAttributeName("active-area")]
        public string Area { get; set; }

        [HtmlAttributeName("active-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("active-action")]
        public string Action { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private RouteValueDictionary RouteData => ViewContext.RouteData.Values;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.RemoveAll("active-area");
            output.Attributes.RemoveAll("active-controller");
            output.Attributes.RemoveAll("active-action");

            // active-area matches
            if (Area != null && Area.Split(",").All(a => (string) RouteData["Area"] != a))
                return;

            // active-controller matches
            if (Controller != null && Controller.Split(",").All(c => (string) RouteData["Controller"] != c))
                return;

            // active-action matches
            if (Action != null && Action.Split(",").All(a => (string) RouteData["Action"] != a))
                return;

            AddActiveClass(output);
        }

        private void AddActiveClass(TagHelperOutput output)
        {
            var currentClass = output.Attributes.FirstOrDefault(a => a.Name == "class");
            var newClass = ActiveClass;
            if (currentClass != null)
            {
                newClass = $"{currentClass.Value} {ActiveClass}";
                output.Attributes.Remove(currentClass);
            }
            output.Attributes.Add(new TagHelperAttribute("class", newClass));
        }
    }
}
