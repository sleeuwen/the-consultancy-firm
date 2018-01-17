using TheConsultancyFirm.Common;

namespace TheConsultancyFirm.Models
{
    public class ItemTranslation
    {
        public int Id { get; set; }

        public int IdNl { get; set; }
        public int IdEn { get; set; }

        public Enumeration.ContentItemType ContentType { get; set; }
    }
}
