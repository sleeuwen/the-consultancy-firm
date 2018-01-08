using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly INewsItemRepository _newsItemRepository;
        private readonly IDownloadRepository _downloadRepository;

        public RelatedItemsService(ICaseRepository caseRepository, ISolutionRepository solutionRepository,
            INewsItemRepository newsItemRepository, IDownloadRepository downloadRepository)
        {
            _caseRepository = caseRepository;
            _solutionRepository = solutionRepository;
            _newsItemRepository = newsItemRepository;
            _downloadRepository = downloadRepository;
        }

        public async Task<List<ContentItem>> GetRelatedItems(int id, Enumeration.ContentItemType type)
        {
            var tags = await GetTags(id, type);
            var matchingItems = new List<ContentItem>();

            foreach (var caseItem in await _caseRepository.GetAll().Include(i => i.CaseTags).ToListAsync())
            {
                if (type == Enumeration.ContentItemType.Case && caseItem.Id == id) continue;

                double score = CalculateScore(tags, caseItem.CaseTags.Select(i => i.TagId).ToList());

                matchingItems.Add(new ContentItem
                {
                    Id = caseItem.Id,
                    Type = Enumeration.ContentItemType.Case,
                    Score = score,
                    PhotoPath = caseItem.PhotoPath,
                    Title = caseItem.Title,
                });
            }

            foreach (var solutionItem in await _solutionRepository.GetAll().Include(i => i.SolutionTags).ToListAsync())
            {
                if (type == Enumeration.ContentItemType.Solution && solutionItem.Id == id) continue;

                double score = CalculateScore(tags, solutionItem.SolutionTags.Select(i => i.TagId).ToList());

                matchingItems.Add(new ContentItem
                {
                    Id = solutionItem.Id,
                    Type = Enumeration.ContentItemType.Solution,
                    Score = score,
                    Title = solutionItem.Title,
                });
            }

            foreach (var downloadItem in await _downloadRepository.GetAll().Include(i => i.DownloadTags).ToListAsync())
            {
                if (type == Enumeration.ContentItemType.Download && downloadItem.Id == id) continue;

                double score = CalculateScore(tags, downloadItem.DownloadTags.Select(i => i.TagId).ToList());

                matchingItems.Add(new ContentItem
                {
                    Id = downloadItem.Id,
                    Type = Enumeration.ContentItemType.Download,
                    Score = score,
                    Title = downloadItem.Title,
                });
            }

            foreach (var newsItem in await _newsItemRepository.GetAll().Include(i => i.NewsItemTags).ToListAsync())
            {
                if (type == Enumeration.ContentItemType.NewsItem && newsItem.Id == id) continue;

                double score = CalculateScore(tags, newsItem.NewsItemTags.Select(i => i.TagId).ToList());

                matchingItems.Add(new ContentItem
                {
                    Id = newsItem.Id,
                    Type = Enumeration.ContentItemType.NewsItem,
                    Score = score,
                    PhotoPath = newsItem.PhotoPath,
                    Title = newsItem.Title,
                });
            }

            return matchingItems.OrderByDescending(item => item.Score).Take(3).ToList();
        }

        private Task<List<int>> GetTags(int id, Enumeration.ContentItemType type)
        {
            switch (type)
            {
                case Enumeration.ContentItemType.Case:
                    return _caseRepository.GetAll().Include(i => i.CaseTags)
                        .Where(i => i.Id == id)
                        .SelectMany(i => i.CaseTags.Select(t => t.TagId))
                        .ToListAsync();
                case Enumeration.ContentItemType.Solution:
                    return _solutionRepository.GetAll().Include(i => i.SolutionTags)
                        .Where(i => i.Id == id)
                        .SelectMany(i => i.SolutionTags.Select(t => t.TagId))
                        .ToListAsync();
                case Enumeration.ContentItemType.Download:
                    return _downloadRepository.GetAll().Include(i => i.DownloadTags)
                        .Where(i => i.Id == id)
                        .SelectMany(i => i.DownloadTags.Select(t => t.TagId))
                        .ToListAsync();
                case Enumeration.ContentItemType.NewsItem:
                    return _newsItemRepository.GetAll().Include(i => i.NewsItemTags)
                        .Where(i => i.Id == id)
                        .SelectMany(i => i.NewsItemTags.Select(t => t.TagId))
                        .ToListAsync();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid type");
            }
        }

        /// <summary>
        /// Calculate the related item score for two tag lists
        /// </summary>
        /// <param name="tags">The tag list of the original item</param>
        /// <param name="myTags">The tag list of the related item</param>
        /// <returns>
        /// A score between 0 and 1 how related the two items are.
        /// 0 being not related at all, 1 being completely related.
        /// </returns>
        private static double CalculateScore(ICollection<int> tags, ICollection<int> myTags)
        {
            double intersection = tags.Intersect(myTags).Count();
            double union = tags.Union(myTags).Count();

            return intersection / union;
        }
    }
}
