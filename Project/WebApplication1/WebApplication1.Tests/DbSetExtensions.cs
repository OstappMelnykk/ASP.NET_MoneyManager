using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApplication1.Models;

namespace WebApplication1.Tests
{
    public static class DbSetExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this Mock<DbSet<T>> mockDbSet, IEnumerable<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockDbSet.Object;
        }
    }

}


