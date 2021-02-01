using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Application.Interfaces.Generic
{
    public interface IGenericRepository<TId>: IBusinessService
    {
        T FindById<T>(TId id) where T : BaseEntity<TId>;
        T FindById<T>(TId id, string[] include) where T : BaseEntity<TId>;
        Task<T> FindByIdAsync<T>(TId id) where T : BaseEntity<TId>;
        Task<T> FindByIdAsync<T>(TId id, string[] include) where T : BaseEntity<TId>;
        IEnumerable<T> Find<T>(ISpecification<T> spec) where T : BaseEntity<TId>;
        IEnumerable<T> Find<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>;
        Task<IEnumerable<T>> FindAsync<T>(ISpecification<T> spec) where T : BaseEntity<TId>;
        Task<IEnumerable<T>> FindAsync<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>;
        T FindOne<T>(ISpecification<T> spec) where T : BaseEntity<TId>;
        T FindOne<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>;
        Task<T> FindOneAsync<T>(ISpecification<T> spec) where T : BaseEntity<TId>;
        Task<T> FindOneAsync<T>(ISpecification<T> spec, string[] includes) where T : BaseEntity<TId>;
        T Add<T>(T entity) where T : BaseEntity<TId>;
        void Update<T>(T entity) where T : BaseEntity<TId>;
        void Delete<T>(T entity) where T : BaseEntity<TId>;
        IEnumerable<T> FindAll<T>() where T : BaseEntity<TId>;
        IEnumerable<T> FindAll<T>(string[] includes) where T : BaseEntity<TId>;
        int Count<T>() where T : BaseEntity<TId>;
        Task<int> CountAsync<T>() where T : BaseEntity<TId>;
        Task<IEnumerable<T>> FindAllAsync<T>() where T : BaseEntity<TId>;
        Task<IEnumerable<T>> FindAllAsync<T>(string[] includes) where T : BaseEntity<TId>;
        IPagedQueryResult<T> FindAllPaginated<T>(IPageOption pageDescriptor) where T : BaseEntity<TId>;
    }
}
