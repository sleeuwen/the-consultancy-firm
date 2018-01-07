using System.ComponentModel.DataAnnotations;

namespace TheConsultancyFirm.Common
{
    public class Enumeration
    {
        public enum ContentItemType
        {
            Download,
            Solution,
            [Display(Name = "Nieuws")]
            NewsItem,
            Case
        }
    }
}
