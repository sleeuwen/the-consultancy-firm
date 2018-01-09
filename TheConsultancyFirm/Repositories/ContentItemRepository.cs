using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class ContentItemRepository : IContentItemRepository
    {

        public const string LatestItemQuery =
            @"SELECT *
FROM (
    SELECT
    'Case' AS type,
    Cases.Id,
    Cases.Date

    FROM Cases

    Where Date == SELECT MIN(Date) From Cases

    UNION

    SELECT
    'NewsItem' AS type,
    NewsItems.Id,
    NewsItems.Date

    FROM NewsItems

    Where Date == SELECT MIN(Date) From NewsItems

    UNION

    SELECT
    'Solution' AS type,
    Solutions.Id,
    Solutions.Date

    FROM Solutions

    Where Date == SELECT MIN(Date) From Solutions

    UNION

    SELECT
    'Download' AS type,
    Downloads.Id,
    Downloads.Date

    FROM Downloads

    Where Date == SELECT MIN(Date) From Downloads
)


";

        private ApplicationDbContext _context;

        public ContentItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContentItem>> GetLatestItem()
        {
            var contentItems = new List<ContentItem>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = string.Format(LatestItemQuery);

                await _context.Database.OpenConnectionAsync();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        contentItems.Add(new ContentItem
                        {
                            Type = Enum.Parse<Enumeration.ContentItemType>(result["type"] as string),
                            Id = (int) result["Id"],
                            Title = result["Title"] as string,
                            PhotoPath = result["PhotoPath"] as string
                        });
                    }
                }
            }
            return contentItems;
        }
    }
}
