using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Repositories
{
    public interface IItemTranslationRepository
    {
        Task<List<ItemTranslation>> GetAllCases();
        Task<List<ItemTranslation>> GetAllDownloads();
        Task<List<ItemTranslation>> GetAllNewsitems();
        Task<List<ItemTranslation>> GetAllSolutions();

        Task<List<Tuple<int, string>>> GetCasesWithoutTranslation();
        Task<List<Tuple<int, string>>> GetDownloadsWithoutTranslation();
        Task<List<Tuple<int, string>>> GetNewsItemsWithoutTranslation();
        Task<List<Tuple<int, string>>> GetSolutionsWithoutTranslation();
    }
}

