using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;

namespace TheConsultancyFirm.Services
{
	public class RelatedItemsService : IRelatedItemsService
    {
        private readonly ICaseRepository _caseRepository;
        private readonly ISolutionRepository _solutionRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IDownloadRepository _downloadRepository;

		public RelatedItemsService(ICaseRepository caseRepository, ISolutionRepository solutionRepository,
			INewsRepository newsRepository, IDownloadRepository downloadRepository)
        {
			_caseRepository = caseRepository;
	        _solutionRepository = solutionRepository;
	        _newsRepository = newsRepository;
	        _downloadRepository = downloadRepository;
        }

        public List<ContentItem> GetRelatedItems(int id, Enumeration.ContentItemType type)
        {
	        var tags = GetTags(id, type);
            var matchingItems = new List<ContentItem>();

	        foreach (var caseItem in _caseRepository.GetAll().Include(i => i.CaseTags))
	        {
		        if (type == Enumeration.ContentItemType.Case && caseItem.Id == id) continue;


				var incommon = tags.Intersect(caseItem.CaseTags.Select(i => i.TagId));
		        var max = tags.Count > caseItem.CaseTags.Count ? tags.Count : caseItem.CaseTags.Count;
		        int score = (incommon.Count() * 100) / max;

				matchingItems.Add(new ContentItem{Id = caseItem.Id, Type = Enumeration.ContentItemType.Case, Score = score});
	        }

	        foreach (var solutionItem in _solutionRepository.GetAll().Include(i => i.SolutionTags))
	        {
		        if (type == Enumeration.ContentItemType.Solution && solutionItem.Id == id) continue;

				var incommon = tags.Intersect(solutionItem.SolutionTags.Select(i => i.TagId));
		        var max = tags.Count > solutionItem.SolutionTags.Count ? tags.Count : solutionItem.SolutionTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = solutionItem.Id, Type = Enumeration.ContentItemType.Solution, Score = score });
	        }

	        foreach (var downloadItem in _downloadRepository.GetAll().Include(i => i.DownloadTags))
	        {
				if (type == Enumeration.ContentItemType.Download && downloadItem.Id == id) continue;

				var incommon = tags.Intersect(downloadItem.DownloadTags.Select(i => i.TagId));
		        var max = tags.Count > downloadItem.DownloadTags.Count ? tags.Count : downloadItem.DownloadTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = downloadItem.Id, Type = Enumeration.ContentItemType.Download, Score = score });
	        }

	        foreach (var newsItem in _newsRepository.GetAll().Include(i => i.NewsItemTags))
	        {
				if (type == Enumeration.ContentItemType.News && newsItem.Id == id) continue;

				var incommon = tags.Intersect(newsItem.NewsItemTags.Select(i => i.TagId));
		        var max = tags.Count > newsItem.NewsItemTags.Count ? tags.Count : newsItem.NewsItemTags.Count;
		        int score = (incommon.Count() * 100) / max;

		        matchingItems.Add(new ContentItem { Id = newsItem.Id, Type = Enumeration.ContentItemType.Solution, Score = score });
	        }

	        return matchingItems.OrderByDescending(item => item.Score).Take(3).ToList();
        }

	    private List<int> GetTags(int id, Enumeration.ContentItemType type)
	    {
			List<int> tags = new List<int>();

		    switch (type)
		    {
			    case Enumeration.ContentItemType.Case:
				    var c = _caseRepository.GetAll().Include(i => i.CaseTags).FirstOrDefault(i => i.Id == id);
				    tags = c.CaseTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.Solution:
				    var solution = _solutionRepository.GetAll().Include(i => i.SolutionTags).FirstOrDefault(i => i.Id == id);
				    tags = solution.SolutionTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.Download:
				    var download = _downloadRepository.GetAll().Include(i => i.DownloadTags).FirstOrDefault(i => i.Id == id);
				    tags = download.DownloadTags.Select(t => t.TagId).ToList();
				    break;
			    case Enumeration.ContentItemType.News:
				    var news = _newsRepository.GetAll().Include(i => i.NewsItemTags).FirstOrDefault(i => i.Id == id);
				    tags = news.NewsItemTags.Select(t => t.TagId).ToList();
				    break;
		    }
		    return tags;
	    }
    }
}
