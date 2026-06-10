using System.Linq.Expressions;

namespace Market.Repositories.Interfaces;

public interface IBaseRepository<T>
{
    T? GetById(object id);
    int Insert(T entity);
    void Update(T entity);
    void Delete(object id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Search(Expression<Func<T, bool>> expression);
}
