using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace TheConsultancyFirm.Models
{
    public class SolutionAdvantagesBlock : Block
    {
        public string Text { get; set; }
        public string PhotoPath { get; set; }

        [NotMapped]
        public IFormFile Image { get; set; }

        [NotMapped]
        public List<string> Advantages
        {
            get => (Text?.Length ?? 0) > 0 ? Text.Split("\n").ToList() : new List<string>();
            set => Text = string.Join('\n',
                value?.Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()) ?? new List<string>());
        }
    }
}
