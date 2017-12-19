using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;

namespace TheConsultancyFirm.Services
{
    public interface IRelatedItemsService
    {
        void GetRelatedItemsCase(Case c);
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

        public void GetRelatedItemsCase(Case c)
        {
            List<Case> matchingCases = new List<Case>();
            
            foreach (var t in c.CaseTags)
            {
                foreach (var kaas in _context.Cases.Include("CaseTag"))
                {
                    if (kaas.Id == c.Id) continue;

                    matchingCases.AddRange(from kaasCaseTag in kaas.CaseTags where kaasCaseTag.TagId == t.TagId select kaas);
                }
            }
            var a = 0;
        }

    }
}
