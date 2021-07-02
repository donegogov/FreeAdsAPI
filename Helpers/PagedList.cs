using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeAds.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FreeAds.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public static PagedList<T> Create(IList<T> source, int pagaNumber, int pageSize)
        {
            int count = source.Count();
            var items = source.Skip((pagaNumber - 1) * pageSize).Take(pageSize);
            return new PagedList<T>(items.ToList(), count, pagaNumber, pageSize);
        }
    }
}