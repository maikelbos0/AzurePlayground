using AzurePlayground.Queries;
using AzurePlayground.Domain.Auditing;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Serialization;
using AzurePlayground.Database;

namespace AzurePlayground.QueryHandlers.Decorators {
    public sealed class AuditDecorator<TQuery, TReturnValue> : Decorator<IQueryHandler<TQuery, TReturnValue>>, IQueryHandler<TQuery, TReturnValue> where TQuery : IQuery<TReturnValue> {
        private readonly IPlaygroundContext _context;
        private readonly ISerializer _serializer;

        public AuditDecorator(IPlaygroundContext context, ISerializer serializer) {
            _context= context;
            _serializer = serializer;
        }

        public TReturnValue Execute(TQuery query) {
            // Explicitly use the context to prevent having to use a repository
            _context.QueryExecutions.Add(new QueryExecution(typeof(TQuery).FullName, _serializer.SerializeToJson(query)));
            _context.SaveChanges();

            return Handler.Execute(query);
        }
    }
}