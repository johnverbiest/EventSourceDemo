using System.Threading.Tasks;

namespace JohnVerbiest.CQRS.Queries
{
    public interface IQueryHandler<in TQuery, TResult> where TQuery: IQuery<TResult> where TResult : class
    {
        Task<TResult> Handle(TQuery query);
    }
}