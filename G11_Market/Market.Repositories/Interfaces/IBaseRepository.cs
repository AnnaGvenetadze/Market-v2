namespace Market.Repositories.Interfaces;

public interface IBaseRepository<TDto, TInsert, TUpdate>
{
    TDto? GetById(object id);
    int Insert(TInsert entity);
    void Update(TUpdate entity);
    void Delete(object id);
    IEnumerable<TDto> GetAll();
    IEnumerable<TDto> Search(Predicate<TDto> predicate);
}
