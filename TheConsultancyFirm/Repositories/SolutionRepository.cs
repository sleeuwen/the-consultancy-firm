using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class SolutionRepository : ISolutionRepository
    {
        private readonly ApplicationDbContext _context;

        public SolutionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Solution> Get(int id, bool includeInactive = false)
        {
            var solution = await _context.Solutions
                .Include(c => c.SolutionTags).ThenInclude(t => t.Tag)
                .Include(c => c.CustomerSolutions).ThenInclude(cs => cs.Customer)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (solution == null) return null;

            await _context.Entry(solution)
                .Collection(s => s.Blocks)
                .Query()
                .Where(b => b.Active || includeInactive)
                .LoadAsync();

            var ids = solution.Blocks.OfType<CarouselBlock>().Select(s => s.Id).ToList();
            _context.Blocks.OfType<CarouselBlock>()
                .Where(s => ids.Contains(s.Id))
                .Include(s => s.Slides)
                .Load();

            return solution;
        }

	    public IQueryable<Solution> GetAll()
	    {
	        return _context.Solutions;
	    }

	    public async Task Create(Solution solution)
	    {
	        _context.Solutions.Add(solution);
	        await _context.SaveChangesAsync();
            _context.ItemTranslations.Add(new ItemTranslation()
	        {
	            ContentType = Enumeration.ContentItemType.Solution,
	            IdNl = solution.Id
	        });
            await _context.SaveChangesAsync();
	    }

        public Task Update(Solution solution)
        {
            _context.Update(solution);
            return _context.SaveChangesAsync();
        }

        public Task Delete(Solution solution)
        {
            _context.Remove(solution);
            return _context.SaveChangesAsync();
        }

        public async Task<int> CreateCopy(int id)
        {
            var solution = await Get(id);
            var solutionCopy = new Solution
            {
                Title = solution.Title,
                Date = DateTime.UtcNow,
                Language = "en",
                PhotoPath = solution.PhotoPath,
                LastModified = DateTime.UtcNow,
                SolutionTags = solution.SolutionTags.Select(n => new SolutionTag { TagId = n.TagId }).ToList(),
                SharingDescription = solution.SharingDescription,
                Summary = solution.Summary,
            };
            await _context.Solutions.AddAsync(solutionCopy);
            await _context.SaveChangesAsync();

            foreach (var newsItemBlock in solution.Blocks)
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
                            SolutionId = solutionCopy.Id,
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
                            SolutionId = solutionCopy.Id,
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
                            SolutionId = solutionCopy.Id,
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
                            SolutionId = solutionCopy.Id,
                            Text = s.Text,
                            PhotoPath = s.PhotoPath
                        });
                        break;
                }
            }
            await _context.SaveChangesAsync();
            var itemTranslation = await _context.ItemTranslations.FirstOrDefaultAsync(s => s.IdNl == id);
            itemTranslation.IdEn = solutionCopy.Id;
            await _context.SaveChangesAsync();
            return solutionCopy.Id;
        }
    }
}

