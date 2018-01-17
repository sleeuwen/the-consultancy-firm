using System;
using System.Collections.Generic;

namespace TheConsultancyFirm.Models
{
    public class CarouselBlock : Block
    {
        public string LinkText { get; set; }
        public string LinkPath { get; set; }

        public List<Slide> Slides { get; set; }

        public bool HomepageCarousel { get; set; }
    }
}
