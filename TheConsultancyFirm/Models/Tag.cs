using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public List<CaseTag> CaseTags { get; set; }
        public List<DownloadTag> DownloadTags { get; set; }
        public List<NewsItemTag> NewsItemTags { get; set; }
        public List<SolutionTag> SolutionTags { get; set; }
    }
}
