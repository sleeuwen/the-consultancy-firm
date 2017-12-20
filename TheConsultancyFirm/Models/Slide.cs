namespace TheConsultancyFirm.Models
{
    public class Slide
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string PhotoPath { get; set; }

        public CarouselBlock Block { get; set; }
    }
}
