using System.Threading.Tasks;

namespace JohnVerbiest.CQRS.Queries
{
    public interface IQueryHandler<in T, TResult> where T: IQuery
    {
        Task<TResult> Handle(T query);
    }
}