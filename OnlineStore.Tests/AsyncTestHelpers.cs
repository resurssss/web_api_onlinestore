using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace OnlineStore.Tests
{
    public static class AsyncTestHelpers
    {
        public static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();
            
            // Настройка синхронных операций
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            
            // Настройка асинхронных операций
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
                
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
            
            // Настройка FindAsync
            mockSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((object[] keyValues, CancellationToken ct) => 
                    queryable.FirstOrDefault(e => EF.Property<object>(e, "Id").Equals(keyValues[0])));
            
            // Настройка Add и Remove
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(entity => 
            {
                var list = data.ToList();
                list.Add(entity);
                var newQueryable = list.AsQueryable();
                mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(newQueryable.Provider);
                mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(newQueryable.Expression);
                mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(newQueryable.ElementType);
                mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => newQueryable.GetEnumerator());
            });
            
            mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>(entity => 
            {
                var list = data.ToList();
                list.Remove(entity);
                var newQueryable = list.AsQueryable();
                mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(newQueryable.Provider);
                mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(newQueryable.Expression);
                mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(newQueryable.ElementType);
                mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => newQueryable.GetEnumerator());
            });
            
            return mockSet;
        }
    }

    internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: new[] { typeof(Expression) })
                .MakeGenericMethod(resultType)
                .Invoke(_inner, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { executionResult });
        }
        
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }
    }

    internal class TestAsyncEnumerable<T> : IAsyncEnumerable<T>, IQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _queryable;

        public TestAsyncEnumerable(IQueryable<T> queryable)
        {
            _queryable = queryable;
        }

        public TestAsyncEnumerable(IEnumerable<T> enumerable)
        {
            _queryable = enumerable.AsQueryable();
        }

        public TestAsyncEnumerable(Expression expression)
        {
            _queryable = new EnumerableQuery<T>(expression);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(_queryable.GetEnumerator());
        }

        public Type ElementType => _queryable.ElementType;
        public Expression Expression => _queryable.Expression;
        public IQueryProvider Provider => new TestAsyncQueryProvider<T>(_queryable.Provider);
        
        public IEnumerator<T> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}