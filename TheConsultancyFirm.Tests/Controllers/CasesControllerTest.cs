using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class CasesControllerTest
    {
        public CasesControllerTest()
        {
            _caseRepository = new Mock<ICaseRepository>();
        }

        private readonly Mock<ICaseRepository> _caseRepository;

        [Fact]
        public void CheckSurroundings()
        {
            var c = new Case
            {
                Id = 2,
                CaseTags = new List<CaseTag>
                {
                    new CaseTag {CaseId = 2, TagId = 1},
                    new CaseTag {CaseId = 2, TagId = 2}
                },

                Date = new DateTime(2009, 6, 1, 7, 47, 0),
                Title = "case2"
            };

            var cases = (new Case
            {
                Id = 1,
                CaseTags = new List<CaseTag>
                {
                    new CaseTag {CaseId = 1, TagId = 1},
                    new CaseTag {CaseId = 1, TagId = 2}
                },
                Date = new DateTime(2008, 6, 1, 7, 47, 0),
                Title = "case1"
            }, new Case
            {
                Id = 3,
                CaseTags = new List<CaseTag>
                {
                    new CaseTag {CaseId = 3, TagId = 1},
                    new CaseTag {CaseId = 3, TagId = 2}
                },

                Date = new DateTime(2010, 6, 1, 7, 47, 0),
                Title = "case3"
            });

            _caseRepository.Setup(repo => repo.GetAdjacent(c)).Returns(Task.FromResult(cases));

            var service = new Mock<IRelatedItemsRepository>();

            var controller = new CasesController(service.Object, _caseRepository.Object);

            var list = controller.GetAdjacent(c);

            var result = DateTime.Compare(list.Result.Previous.Date, c.Date);
            var result2 = DateTime.Compare(list.Result.Next.Date, c.Date);

            Assert.Equal(-1, result);
            Assert.Equal(1, result2);
        }

        [Fact]
        public void Index()
        {
            var controller = new CasesController(null, _caseRepository.Object);

            var model = new List<Case>().AsQueryable().BuildMock();
            _caseRepository.Setup(repo => repo.GetAll()).Returns(model.Object);
            var result = controller.Index().Result;
            Assert.IsType<ViewResult>(result);
        }
    }
}
