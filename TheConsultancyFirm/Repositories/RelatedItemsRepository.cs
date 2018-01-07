using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class RelatedItemsRepository : IRelatedItemsRepository
    {
        private ApplicationDbContext _context;

        public RelatedItemsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ContentItem> GetRelatedItems(int id, Enumeration.ContentItemType type)
        {
            var contentItems = new List<ContentItem>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Query here";
                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        contentItems.Add( new ContentItem
                        {
                            Type = (Enumeration.ContentItemType)Enum.Parse(typeof(Enumeration.ContentItemType), result["Type"].ToString()),
                            Id = int.Parse(result["Id"].ToString()),
                            Title = result["Title"].ToString(),
                            PhotoPath = result["PhotoPath"].ToString(),
                            Score = int.Parse(result["Score"].ToString()),
                        });
                    }
                }
            }
            return contentItems;
        }
    }
}
