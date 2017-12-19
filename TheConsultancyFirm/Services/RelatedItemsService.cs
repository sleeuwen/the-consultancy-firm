using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IRelatedItemsService
    {
	    List<ContentItem> GetRelatedItems(int id, Enumeration.ContentItemType type);
    }

    public class RelatedItemsService : IRelatedItemsService
    {
        private readonly ApplicationDbContext _context;

        public RelatedItemsService()
        {
        }

        public RelatedItemsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ContentItem> GetRelatedItems(int id, Enumeration.ContentItemType type)
        {
	        List<int> tags = GetTags(id, type);
            List<ContentItem> matchingItems = new List<ContentItem>();

	        foreach (var caseItem in _context.Cases.Include("CaseTags"))
	        {
		        if (type == Enumeration.ContentItemType.Case && caseItem.Id == id) continue;

				var incommon = tags.Intersect(caseItem.CaseTags.Select(i => i.TagId));
		        var max = tags.Count > caseItem.CaseTags.Count ? tags.Count : caseItem.CaseTags.Count;
		        int score = (incommon.Count() * 100) / max;

				matchingItems.Add(new ContentItem{Id = caseItem.Id, Type = Enumeration.ContentItemType.Case, Score = score});
	        }

	        foreach (var solutionItem in _context.Solutions.Include("SolutionTags"))
	        {
		        if (type == Enumeration.ContentItemType.Solution && solutionItem.Id == id) continue;

				var incommon = tags.Intersect(solutionItem.SolutionTags.Select(i => i.TagId));
		        var max = tags.Count > solutionItem.SolutionTags.Count ? tags.Count : solutionItem.SolutionTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = solutionItem.Id, Type = Enumeration.ContentItemType.Solution, Score = score });
	        }

	        foreach (var downloadItem in _context.Downloads.Include("DownloadTags"))
	        {
				if (type == Enumeration.ContentItemType.Download && downloadItem.Id == id) continue;

				var incommon = tags.Intersect(downloadItem.DownloadTags.Select(i => i.TagId));
		        var max = tags.Count > downloadItem.DownloadTags.Count ? tags.Count : downloadItem.DownloadTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = downloadItem.Id, Type = Enumeration.ContentItemType.Download, Score = score });
	        }

	        foreach (var newsItem in _context.NewsItems.Include("NewsItemTags"))
	        {
				if (type == Enumeration.ContentItemType.News && newsItem.Id == id) continue;

				var incommon = tags.Intersect(newsItem.NewsItemTags.Select(i => i.TagId));
		        var max = tags.Count > newsItem.NewsItemTags.Count ? tags.Count : newsItem.NewsItemTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = newsItem.Id, Type = Enumeration.ContentItemType.Solution, Score = score });
	        }

	        return matchingItems.OrderByDescending(item => item.Score).Take(3).ToList();
        }

	    public List<int> GetTags(int id, Enumeration.ContentItemType type)
	    {
			List<int> tags = new List<int>();

		    switch (type)
		    {
			    case Enumeration.ContentItemType.Case:
				    var c = _context.Cases.Include("CaseTags").FirstOrDefault(i => i.Id == id);
				    tags = c.CaseTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.Solution:
				    var solution = _context.Solutions.Include("SolutionTags").FirstOrDefault(i => i.Id == id);
				    tags = solution.SolutionTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.Download:
				    var download = _context.Cases.Include("DownloadTags").FirstOrDefault(i => i.Id == id);
				    tags = download.CaseTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.News:
				    var news = _context.Cases.Include("NewsTags").FirstOrDefault(i => i.Id == id);
				    tags = news.CaseTags.Select(t => t.TagId).ToList();
				    break;
		    }
		    return tags;
	    }

	    public void AddMatchingCases(List<int> tags)
	    {
	    }
    }
}
