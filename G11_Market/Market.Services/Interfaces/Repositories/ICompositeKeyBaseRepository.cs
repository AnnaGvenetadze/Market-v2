namespace Market.Services.Interfaces.Repositories;

public interface ICompositeKeyBaseRepository<T>
{
    IEnumerable<T> GetById(object firstId);
    void Insert(T entity);
    void Update(T entity);
    void Delete(object firstId, object secondId);
    IEnumerable<T> GetAll();
}