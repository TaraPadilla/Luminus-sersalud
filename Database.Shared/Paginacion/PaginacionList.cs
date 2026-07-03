using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Database.Shared.Paginacion
{
    public class PaginacionList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginacionList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        // public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        // {
        //     var count = await source.CountAsync();
        //     var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        //     return new PaginatedList<T>(items, count, pageIndex, pageSize);
        // }

        public static PaginacionList<T> CreateAsyncc(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginacionList<T>(items, count, pageIndex, pageSize);
        }

        public static PaginacionList<T> CreateAsynccCustom(List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginacionList<T>(items, count, pageIndex, pageSize);
        }
    }
}