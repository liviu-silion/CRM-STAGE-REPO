namespace PlannerCRM.Server.Interfaces;

public interface IRepository
{
    public Task AddAsync<TInput>(TInput model) 
        where TInput : class
    ;

    public Task EditAsync<TInput>(TInput model, int id)
        where TInput : class
    ;

    public Task DeleteAsync<TInput>(int id)
        where TInput : class
    ;

    public Task<TOutput> GetByIdAsync<TOutput>(int id)
        where TOutput : class
    ;

    public Task<ICollection<TOutput>> GetWithPagination<TOutput>(int offset, int limit)
        where TOutput : class
    ;
}