using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class NewsItemRepository : INewsItemRepository
    {
        private ApplicationDbContext _context;

        public NewsItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NewsItem> Get(int id, bool includeInactive = false)
        {
            var newsItem = await _context.NewsItems
                .Include(c => c.NewsItemTags).ThenInclude(t => t.Tag)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (newsItem == null) return null;

            // Load only active blocks, or all if includeInactive is true
            await _context.Entry(newsItem)
                .Collection(c => c.Blocks)
                .Query()
                .Where(b => b.Active || includeInactive)
                .LoadAsync();

            // Load the slides from all CarouselBlock's
            var ids = newsItem.Blocks.OfType<CarouselBlock>().Select(c => c.Id).ToList();
            _context.Blocks.OfType<CarouselBlock>()
                .Where(c => ids.Contains(c.Id))
                .Include(c => c.Slides)
                .Load();

            return newsItem;
        }

        public IQueryable<NewsItem> GetAll()
        {
            return _context.NewsItems;
        }

        public  Task Create(NewsItem newsItem)
        {
            _context.NewsItems.Add(newsItem);
            return _context.SaveChangesAsync();
        }

        public Task Update(NewsItem newsItem)
        {
            _context.NewsItems.Update(newsItem);
            return _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var newsItem = await Get(id);
            _context.NewsItems.Remove(newsItem);
            await _context.SaveChangesAsync();
        }

        public Task<NewsItem> GetLatest()
        {
            return _context.NewsItems.OrderByDescending(n => n.Date).Take(1).FirstOrDefaultAsync();
        }

        public async Task<int> CreateCopy(int id)
        {
            var newsItem = await Get(id);
            var newsItemCopy = new NewsItem
            {
                Title = newsItem.Title,
                Date = DateTime.UtcNow,
                Language = "en",
                PhotoPath = newsItem.PhotoPath,
                LastModified = DateTime.UtcNow,
                NewsItemTags = newsItem.NewsItemTags.Select(n => new NewsItemTag{TagId = n.TagId}).ToList(),
                SharingDescription = newsItem.SharingDescription,
                
            };
            await _context.NewsItems.AddAsync(newsItemCopy);
            await _context.SaveChangesAsync();

            foreach (var newsItemBlock in newsItem.Blocks)
            {
                switch (newsItemBlock)
                {
                    case TextBlock t:
                        _context.Blocks.Add(new TextBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = t.Active,
                            Order = t.Order,
                            NewsItemId = newsItemCopy.Id,
                            Text = t.Text
                        });
                        break;
                    case CarouselBlock c:
                        _context.Blocks.Add(new CarouselBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = c.Active,
                            Order = c.Order,
                            NewsItemId = newsItemCopy.Id,
                            LinkPath = c.LinkPath,
                            LinkText = c.LinkText,
                            Slides = c.Slides.Select(s => new Slide { Order = s.Order, PhotoPath = s.PhotoPath, Text = s.Text }).ToList()
                        });
                        break;
                    case QuoteBlock q:
                        _context.Blocks.Add(new QuoteBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = q.Active,
                            Order = q.Order,
                            NewsItemId = newsItemCopy.Id,
                            Text = q.Text,
                            Author = q.Author
                        });
                        break;
                    case SolutionAdvantagesBlock s:
                        _context.Blocks.Add(new SolutionAdvantagesBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = s.Active,
                            Order = s.Order,
                            NewsItemId = newsItemCopy.Id,
                            Text = s.Text,
                            PhotoPath = s.PhotoPath
                        });
                        break;
                }
            }
            await _context.SaveChangesAsync();
            return newsItemCopy.Id;
        }
    }
}
