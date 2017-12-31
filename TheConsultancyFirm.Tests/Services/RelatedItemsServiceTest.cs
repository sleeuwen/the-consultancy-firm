using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Services
{
    public class RelatedItemsServiceTest
    {
        private readonly Mock<ICaseRepository> _caseRepository;
        private readonly Mock<IDownloadRepository> _downloadRepository;
        private readonly Mock<INewsRepository> _newsRepository;
        private readonly Mock<ISolutionRepository> _solutionRepository;

        public RelatedItemsServiceTest()
        {
            _caseRepository = new Mock<ICaseRepository>();
            _downloadRepository = new Mock<IDownloadRepository>();
            _newsRepository = new Mock<INewsRepository>();
            _solutionRepository = new Mock<ISolutionRepository>();
        }

        [Fact]
        public async Task CalculateScore()
        {
            List<Case> cases = new List<Case>
            {
                new Case
                {
                    Id = 1,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag {TagId = 1},
                        new CaseTag {TagId = 2},
                        new CaseTag {TagId = 3},
                        new CaseTag {TagId = 4},
                    }
                },
                new Case
                {
                    Id = 2,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag {TagId = 1},
                        new CaseTag {TagId = 2}
                    }
                },
            };

            _caseRepository.Setup(repo => repo.GetAll()).Returns(cases.AsQueryable());
            _downloadRepository.Setup(repo => repo.GetAll()).Returns(new List<Download>().AsQueryable());
            _newsRepository.Setup(repo => repo.GetAll()).Returns(new List<NewsItem>().AsQueryable());
            _solutionRepository.Setup(repo => repo.GetAll()).Returns(new List<Solution>().AsQueryable());

            var service = new RelatedItemsService(_caseRepository.Object, _solutionRepository.Object, _newsRepository.Object, _downloadRepository.Object);

            var result = await service.GetRelatedItems(1, Enumeration.ContentItemType.Case);

            Assert.Equal(50, result.First().Score);
        }

        [Fact]
        public async Task CalculateScoreNoCommonTags()
        {
            List<Case> cases = new List<Case>
            {
                new Case
                {
                    Id = 1,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag {TagId = 3},
                        new CaseTag {TagId = 4},
                    }
                },
                new Case
                {
                    Id = 2,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag {TagId = 1},
                        new CaseTag {TagId = 2}
                    }
                },
            };

            _caseRepository.Setup(repo => repo.GetAll()).Returns(cases.AsQueryable());
            _downloadRepository.Setup(repo => repo.GetAll()).Returns(new List<Download>().AsQueryable());
            _newsRepository.Setup(repo => repo.GetAll()).Returns(new List<NewsItem>().AsQueryable());
            _solutionRepository.Setup(repo => repo.GetAll()).Returns(new List<Solution>().AsQueryable());

            var service = new RelatedItemsService(_caseRepository.Object, _solutionRepository.Object, _newsRepository.Object, _downloadRepository.Object);

            var result = await service.GetRelatedItems(1, Enumeration.ContentItemType.Case);

            Assert.Equal(0, result.First().Score);
        }
    }
}
