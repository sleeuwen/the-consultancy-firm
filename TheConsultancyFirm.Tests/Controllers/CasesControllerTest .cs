using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TheConsultancyFirm.Common;
using TheConsultancyFirm.Controllers;
using TheConsultancyFirm.Models;
using TheConsultancyFirm.Repositories;
using TheConsultancyFirm.Services;
using Xunit;

namespace TheConsultancyFirm.Tests.Controllers
{
    public class CasesControllerTest
    {
        private readonly Mock<ICaseRepository> _caseRepository;
        public CasesControllerTest()
        {
            _caseRepository = new Mock<ICaseRepository>();
        }

        [Fact]
        public void Index()
        {
            var controller = new CasesController(null,null);
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void CheckSurroundings()
        {
            var c = new Case
            {
                Id = 2,
                CaseTags = new List<CaseTag>
                    {
                        new CaseTag{CaseId=2,TagId = 1},
                        new CaseTag{CaseId=2,TagId = 2},
                    },

                Date = new DateTime(2009, 6, 1, 7, 47, 0),
                Title = "case2"
            };
            List<Case> cases = new List<Case>
            {
                new Case
                {
                    Id = 1,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag{CaseId=1,TagId = 1},
                        new CaseTag{CaseId=1,TagId = 2},
                    },
                    Date = new DateTime(2008, 6, 1, 7, 47, 0),
                    Title = "case1"
                },  new Case
                {
                    Id = 3,
                    CaseTags = new List<CaseTag>
                    {
                        new CaseTag{CaseId=3,TagId = 1},
                        new CaseTag{CaseId=3,TagId = 2},

                    },

                    Date = new DateTime(2010, 6, 1, 7, 47, 0),
                    Title = "case3"

                },
            };

            _caseRepository.Setup(repo => repo.GetSurrounding(c)).Returns(cases);

            var service = new Mock<IRelatedItemsService>();

            var controller = new CasesController(service.Object, _caseRepository.Object);

            var list = controller.GetSurroundings(c);

            int result = DateTime.Compare(list.ToList()[0].Date, c.Date);
            int result2 = DateTime.Compare(list.ToList()[1].Date, c.Date);


            Assert.Equal(-1, result);
            Assert.Equal(1, result2);
        }
    }
}
