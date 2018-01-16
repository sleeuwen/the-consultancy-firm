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
    public interface IItemTranslationRepository
    {
        Task<List<ItemTranslation>> GetAllCases();
    }

    public class ItemTranslationRepository : IItemTranslationRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemTranslationRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<ItemTranslation>> GetAllCases()
        {
            return await _context.ItemTranslations.Where(c => c.ContentType == Enumeration.ContentItemType.Case).ToListAsync();
        }
    }
}

