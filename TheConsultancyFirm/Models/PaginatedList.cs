using System;
using System.Collections.Generic;
using System.Linq;

namespace TheConsultancyFirm.Models
{
    public class PaginatedList<T> : List<T>
    {
        private const int PageSize = 2;
        public int PageIndex { get; }
        public int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageIndex)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static PaginatedList<T> Create(
            IEnumerable<T> source, int pageIndex)
        {
            var count = source.Count();
            var items = source
                .Skip((pageIndex - 1) * PageSize)
                .Take(PageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex);
        }

    }
}
