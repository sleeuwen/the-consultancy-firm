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
    public class ItemTranslationRepository : IItemTranslationRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemTranslationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tuple<int,string>>> GetCasesWithoutTranslation()
        {
            var list = (await GetAllCases()).Where(c => c.IdEn == 0).Select(c => c.IdNl).ToList();

            return await _context.Cases.Where(c => list.Contains(c.Id)).Select(c => new Tuple<int,string>(c.Id,c.Title)).ToListAsync();
        }


        public async Task<List<Tuple<int,string>>> GetDownloadsWithoutTranslation()
        {
            var list = (await GetAllDownloads()).Where(d => d.IdEn == 0).Select(d => d.IdNl).ToList();

            return await _context.Downloads.Where(d => list.Contains(d.Id)).Select(d => new Tuple<int,string>(d.Id,d.Title)).ToListAsync();
        }


        public async Task<List<Tuple<int, string>>> GetNewsItemsWithoutTranslation()
        {
            var list = (await GetAllNewsitems()).Where(n => n.IdEn == 0).Select(n => n.IdNl).ToList();

            return await _context.NewsItems.Where(n => list.Contains(n.Id)).Select(n => new Tuple<int, string>(n.Id, n.Title)).ToListAsync();
        }


        public async Task<List<Tuple<int, string>>> GetSolutionsWithoutTranslation()
        {
            var list = (await GetAllSolutions()).Where(s => s.IdEn == 0).Select(s => s.IdNl).ToList();

            return await _context.Solutions.Where(s => list.Contains(s.Id)).Select(s => new Tuple<int, string>(s.Id, s.Title)).ToListAsync();
        }

        public async Task<List<ItemTranslation>> GetAllCases()
        {
            return await _context.ItemTranslations.Where(c => c.ContentType == Enumeration.ContentItemType.Case).ToListAsync();
        }
        public async Task<List<ItemTranslation>> GetAllDownloads()
        {
            return await _context.ItemTranslations.Where(d => d.ContentType == Enumeration.ContentItemType.Download).ToListAsync();
        }
        public async Task<List<ItemTranslation>> GetAllNewsitems()
        {
            return await _context.ItemTranslations.Where(n => n.ContentType == Enumeration.ContentItemType.NewsItem).ToListAsync();
        }
        public async Task<List<ItemTranslation>> GetAllSolutions()
        {
            return await _context.ItemTranslations.Where(s => s.ContentType == Enumeration.ContentItemType.Solution).ToListAsync();
        }
    }
}
