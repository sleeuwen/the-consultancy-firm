using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private const string SearchQuery =
@"WITH MatchingBlocks AS (
  SELECT
    B.CaseId,
    B.NewsItemId,
    B.SolutionId,
    0
      + (ISNULL(LEN(B.Author) - LEN(REPLACE(B.Author, @q, '')), 0) / LEN(@q))
      + (ISNULL(LEN(B.Text) - LEN(REPLACE(B.Text, @q, '')), 0) / LEN(@q))
      + (ISNULL(LEN(B.Text) - LEN(REPLACE(B.Text, @q, '')), 0) / LEN(@q))
      + (ISNULL(LEN(B.SolutionAdvantagesBlock_Text) - LEN(REPLACE(B.SolutionAdvantagesBlock_Text, @q, '')), 0) / LEN(@q))
      + (ISNULL(LEN(B.TextBlock_Text) - LEN(REPLACE(B.TextBlock_Text, @q, '')), 0) / LEN(@q))
      + (ISNULL(LEN(B.LinkText) - LEN(REPLACE(B.LinkText, @q, '')), 0) / LEN(@q))
    AS score

  FROM Blocks B
)
SELECT *
FROM (
  SELECT
    'Case' AS type,
    Cases.Id,
    Cases.Title,
    NULL AS q,
    NULL AS len,
    (SELECT (5 * (ISNULL(LEN(Cases.Title), 0) - ISNULL(LEN(REPLACE(Cases.Title, @q, '')), 0)) / LEN(@q)) + SUM(score) FROM MatchingBlocks WHERE CaseId = Cases.Id GROUP BY CaseId) AS score
  FROM Cases

  UNION

  SELECT
    'Download' AS type,
    Downloads.Id,
    Downloads.Title,
    NULL,
    NULL,
    (SELECT (5 * (ISNULL(LEN(Downloads.Title), 0) - ISNULL(LEN(REPLACE(Downloads.Title, @q, '')), 0)) / LEN(@q)) + (ISNULL(LEN(Downloads.Description), 0) - ISNULL(LEN(REPLACE(Downloads.Description, @q, '')), 0)) / LEN(@q))
  FROM Downloads

  UNION

  SELECT
    'NewsItem' AS type,
    NewsItems.Id,
    NewsItems.Title,
    NULL,
    NULL,
    (SELECT (5 * (ISNULL(LEN(NewsItems.Title), 0) - ISNULL(LEN(REPLACE(NewsItems.Title, @q, '')), 0)) / LEN(@q)) + SUM(score) FROM MatchingBlocks WHERE NewsItemId = NewsItems.Id GROUP BY NewsItemId)
  FROM NewsItems

  UNION

  SELECT
    'Solution' AS type,
    Solutions.Id,
    Solutions.Title,
    NULL,
    NULL,
    (SELECT (5 * (ISNULL(LEN(Solutions.Title), 0) - ISNULL(LEN(REPLACE(Solutions.Title, @q, '')), 0)) / LEN(@q)) + SUM(score) FROM MatchingBlocks WHERE SolutionId = Solutions.Id GROUP BY SolutionId)
  FROM Solutions
) t
WHERE score > 0
ORDER BY score DESC
;
";
        private readonly ApplicationDbContext _context;

        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ContentItem>> Search(string term)
        {
            var contentItems = new List<ContentItem>();

            if (string.IsNullOrWhiteSpace(term)) return contentItems;

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = SearchQuery;
                command.Parameters.Add(new SqlParameter("q", term));

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
                            Score = decimal.ToDouble(result["Score"] as decimal? ?? 0),
                        });
                    }
                }
            }

            return contentItems;
        }
    }
}
