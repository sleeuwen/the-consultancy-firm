using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public class CaseRepository : ICaseRepository
    {
        private ApplicationDbContext _context;

        public CaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Case> Get(int id, bool includeInactive = false)
        {
            var @case = await _context.Cases
                .Include(c => c.CaseTags).ThenInclude(t => t.Tag)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@case == null) return null;

            // Load only active blocks, or all if includeInactive is true
            await _context.Entry(@case)
                .Collection(c => c.Blocks)
                .Query()
                .Where(b => b.Active || includeInactive)
                .LoadAsync();

            // Load the slides from all CarouselBlock's
            var ids = @case.Blocks.OfType<CarouselBlock>().Select(c => c.Id).ToList();
            _context.Blocks.OfType<CarouselBlock>()
                .Where(c => ids.Contains(c.Id))
                .Include(c => c.Slides)
                .Load();

            return @case;
        }

        public IQueryable<Case> GetAll()
        {
            return _context.Cases;
        }

        public async Task<(Case Previous, Case Next)> GetAdjacent(Case c)
        {
            var previous = await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date)
                               .Where(i => i.Date < c.Date && !i.Deleted && i.Enabled && i.Language == c.Language).Take(1).FirstOrDefaultAsync() ??
                           await _context.Cases.Include(i => i.Customer).OrderByDescending(i => i.Date)
                               .Where(i => i.Id != c.Id && !i.Deleted && i.Enabled && i.Language == c.Language).FirstOrDefaultAsync();

            var next = await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Date > c.Date && !i.Deleted && i.Enabled && i.Language == c.Language)
                           .Take(1).FirstOrDefaultAsync() ??
                       await _context.Cases.Include(i => i.Customer).OrderBy(i => i.Date).Where(i => i.Id != c.Id && !i.Deleted && i.Enabled && i.Language == c.Language)
                           .FirstOrDefaultAsync();

            return (previous, next);
        }

        public async Task Create(Case @case)
        {
            _context.Cases.Add(@case);
            
            await _context.SaveChangesAsync();
            _context.ItemTranslations.Add(new ItemTranslation()
            {
                ContentType = Enumeration.ContentItemType.Case,
                IdNl = @case.Id
            });
            await _context.SaveChangesAsync();
        }

        public Task Update(Case @case)
        {
            _context.Cases.Update(@case);
            return _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var @case = await Get(id);
            _context.Cases.Remove(@case);
            await _context.SaveChangesAsync();
        }

        public Task<Case> GetLatest()
        {
            return _context.Cases.OrderByDescending(c => c.Date).Take(1).FirstOrDefaultAsync();
        }

        public async Task<int> CreateCopy(int id)
        {
            var @case = await Get(id);
            var caseCopy = new Case
            {
                CaseTags = @case.CaseTags.Select(c => new CaseTag { TagId = c.TagId }).ToList(),
                Customer = @case.Customer,
                Date = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                PhotoPath = @case.PhotoPath,
                Language = "en",
                Title = @case.Title,
                SharingDescription = @case.SharingDescription
            };
            await _context.Cases.AddAsync(caseCopy);
            await _context.SaveChangesAsync();

            foreach (var caseBlock in @case.Blocks)
            {
                switch (caseBlock)
                {
                    case TextBlock t:
                        _context.Blocks.Add(new TextBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = t.Active,
                            Order = t.Order,
                            CaseId = caseCopy.Id,
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
                            CaseId = caseCopy.Id,
                            LinkPath = c.LinkPath,
                            LinkText = c.LinkText,
                            Slides = c.Slides.Select(s => new Slide{Order = s.Order, PhotoPath = s.PhotoPath, Text = s.Text}).ToList()
                        });
                        break;
                    case QuoteBlock q:
                        _context.Blocks.Add(new QuoteBlock
                        {
                            Date = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            Active = q.Active,
                            Order = q.Order,
                            CaseId = caseCopy.Id,
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
                            CaseId = caseCopy.Id,
                            Text = s.Text,
                            PhotoPath = s.PhotoPath
                        });
                        break;
                }
            }
            await _context.SaveChangesAsync();
            var itemTranslation = await _context.ItemTranslations.FirstOrDefaultAsync(c => c.IdNl == id);
            itemTranslation.IdEn = caseCopy.Id;
            await _context.SaveChangesAsync();
            return caseCopy.Id;
        }
    }
}
