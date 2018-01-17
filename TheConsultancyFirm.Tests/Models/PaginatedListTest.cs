using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TheConsultancyFirm.Models;
using Xunit;

namespace TheConsultancyFirm.Tests.Models
{
    [Area("Dashboard")]
    public class PaginatedListTest
    {
        List<int> items;

        public PaginatedListTest()
        {
            items = new List<int>();
            items.Add(1);
            items.Add(2);
            items.Add(3);
            items.Add(4);
            items.Add(5);
            items.Add(6);

        }

        [Fact]
        public void HasNextPage()
        {
            PaginatedList<int> paginatedList = new PaginatedList<int>(items, items.Count, 1, 1);
            var result = paginatedList.HasNextPage;
            Assert.True(result);
        }
        [Fact]
        public void NotHasNextPage()
        {
            PaginatedList<int> paginatedList = new PaginatedList<int>(items, items.Count, 6, 1);
            var result = paginatedList.HasNextPage;
            Assert.False(result);
        }
        [Fact]
        public void HasPrevPage()
        {
            PaginatedList<int> paginatedList = new PaginatedList<int>(items, items.Count, 6, 1);
            var result = paginatedList.HasPreviousPage;
            Assert.True(result);
        }
        [Fact]
        public void NotHasPrevPage()
        {
            PaginatedList<int> paginatedList = new PaginatedList<int>(items, items.Count, 1, 1);
            var result = paginatedList.HasPreviousPage;
            Assert.False(result);
        }
    }
}

