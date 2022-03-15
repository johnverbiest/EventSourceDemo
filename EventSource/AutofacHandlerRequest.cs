using System;
using System.Threading.Tasks;
using Autofac;
using JohnVerbiest.CQRS.Common;
using JohnVerbiest.CQRS.Dependencies;

namespace EventSource;

public class AutofacHandlerRequest : IHandlerRequestDependency
{
    private readonly IComponentContext _context;

    public AutofacHandlerRequest(IComponentContext context)
    {
        _context = context;
    }

    public Task<T> GetHandler<T>() where T : IHaveSingleHandler
    {
        return Task.FromResult(_context.Resolve<T>());
    }

    public Task<T[]> GetHandlers<T>(Type locateMe) where T : IHaveMultipleHandlers
    {
        return Task.FromResult((T[]) _context.Resolve(locateMe.MakeArrayType()));
    }
}