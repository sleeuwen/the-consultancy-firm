using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class NewsItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PhotoPath { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }

        public List<Block> Blocks { get; set; }
        public List<NewsItemTag> NewsItemTags { get; set; }
    }
}
