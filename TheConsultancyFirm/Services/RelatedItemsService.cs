using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var tags = c.CaseTags;
            var casetags = _context.Cases.Select(s => s.CaseTags).ToList();
            List<Case> matchingCases = new List<Case>();
            
            for (var i = 0; i < tags.Count; i++)
            {
                List<Case> cases = new List<Case>();

                foreach (var kaas in _context.Cases)
                {
                    if (kaas.Id == c.Id) continue;
                    
                    foreach (var kaasCaseTag in kaas.CaseTags)
                    {
                        if (kaasCaseTag.TagId == tags[i].TagId)
                        {
                            cases.Add(kaas);
                        }
                    }
                }

                var a = 0;
            }
        }

    }
}
