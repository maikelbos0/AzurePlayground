using AzurePlayground.Queries;

namespace AzurePlayground.QueryHandlers {
    public interface IQueryHandler<TQuery, TReturnValue> where TQuery : IQuery<TReturnValue> {
        TReturnValue Execute(TQuery query);
    }
}