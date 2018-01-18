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
    public class AboutControllerTest
    {
        private readonly Mock<IVacancyRepository> _vacancyRepository;

        public AboutControllerTest()
        {
            _vacancyRepository = new Mock<IVacancyRepository>();
        }

        [Fact]
        public void CheckOrder()
        {
            var v = new Vacancy
            {
                Id = 2,
                VacancySince = new DateTime(2009, 6, 1, 7, 47, 0)
            };

            var vacancies = (new Vacancy
            {
                Id = 1,

                VacancySince = new DateTime(2008, 6, 1, 7, 47, 0)
            }, new Vacancy
            {
                Id = 3,
                
                VacancySince = new DateTime(2010, 6, 1, 7, 47, 0)
            });


            var service = new Mock<IRelatedItemsRepository>();

            var controller = new AboutController(_vacancyRepository.Object);

            var list = controller.Index();
            var numQuery =
           from num in list
           where (num % 2) == 0
           select num;


            var result = DateTime.Compare(list.Result[0], list.Result[1]);
            var result2 = DateTime.Compare(list.Result.Next.Date, c.Date);

            Assert.Equal(-1, result);
            Assert.Equal(1, result2);
        }

        [Fact]
        public async Task Index()
        {
            var controller = new CasesController(null, _caseRepository.Object);

            var model = new List<Case>().AsQueryable().BuildMock();
            _caseRepository.Setup(repo => repo.GetAll()).Returns(model.Object);
            var result = await controller.Index();
            Assert.IsType<ViewResult>(result);
        }
    }
}
