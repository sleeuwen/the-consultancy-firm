using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace TheConsultancyFirm.TagHelpers
{
    [HtmlTargetElement(Attributes = "active-controller")]
    public class ActiveRouteTagHelper : TagHelper
    {
        // In what order should this tag helper run compared to other tag helpers.
        public override int Order => 0;

        private const string ActiveClass = "active";

        // Value specified in html via `active-controller="value"`
        [HtmlAttributeName("active-controller")]
        public string Controller { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private RouteValueDictionary RouteData => ViewContext.RouteData.Values;

        // This method is called every time the `active-controller` attribute is encountered
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // remove the `active-controller` attribute from the html
            output.Attributes.RemoveAll("active-controller");

            // Check if the value of `active-controller` matches the current route controller
            if (Controller != null && Controller.Split(",")
                    .All(c => !c.Equals((string) RouteData["Controller"], StringComparison.InvariantCultureIgnoreCase)))
                return;

            // Add the active class to the current tag
            AddActiveClass(output);
        }

        private void AddActiveClass(TagHelperOutput output)
        {
            var newClass = ActiveClass;

            // Get the current `class` attribute
            var currentClass = output.Attributes.FirstOrDefault(a => a.Name == "class");
            if (currentClass != null)
            {
                // Append the `ActiveClass` to the current class value and remove the current class attribute
                newClass = $"{currentClass.Value} {ActiveClass}";
                output.Attributes.Remove(currentClass);
            }

            // Add the new class attribute with the active class
            output.Attributes.Add(new TagHelperAttribute("class", newClass));
        }
    }
}
