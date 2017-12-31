using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Data;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Services
{
    public class RelatedItemsServiceTest
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public RelatedItemsServiceTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CalculateScore()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                List<Tag> tags = await SeedTags(4);

                var cases = new List<Case>
                {
                    new Case
                    {
                        CaseTags = new List<CaseTag>(tags.Select(t => new CaseTag {TagId = t.Id})),
                    },
                    new Case
                    {
                        CaseTags = new List<CaseTag>
                        {
                            new CaseTag {TagId = tags.ElementAt(0).Id},
                            new CaseTag {TagId = tags.ElementAt(1).Id}
                        }
                    }
                };

                context.AddRange(cases);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var service = new RelatedItemsService(new CaseRepository(context), new SolutionRepository(context),
                    new NewsRepository(context), new DownloadRepository(context));

                var result = await service.GetRelatedItems(1, Enumeration.ContentItemType.Case);

                Assert.Equal(0.5, result.First().Score);
            }
        }

        [Fact]
        public async Task CalculateScoreNoCommonTags()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                List<Tag> tags = await SeedTags(4);

                var cases = new List<Case>
                {
                    new Case
                    {
                        CaseTags = new List<CaseTag>
                        {
                            new CaseTag {TagId = tags.ElementAt(2).Id},
                            new CaseTag {TagId = tags.ElementAt(3).Id}
                        },
                    },
                    new Case
                    {
                        CaseTags = new List<CaseTag>
                        {
                            new CaseTag {TagId = tags.ElementAt(0).Id},
                            new CaseTag {TagId = tags.ElementAt(1).Id}
                        }
                    }
                };

                context.AddRange(cases);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var service = new RelatedItemsService(new CaseRepository(context), new SolutionRepository(context), new NewsRepository(context), new DownloadRepository(context));

                var result = await service.GetRelatedItems(1, Enumeration.ContentItemType.Case);

                Assert.Equal(0, result.First().Score);
            }
        }

        private async Task<List<Tag>> SeedTags(int amount)
        {
            using (var context = new ApplicationDbContext(_options))
            {
                var tags = new List<Tag>();
                for (var i = 0; i < amount; i++)
                {
                    tags.Add(new Tag {Text = $"Tag {i}"});
                }

                context.AddRange(tags);
                await context.SaveChangesAsync();
                return tags;
            }
        }
    }
}
