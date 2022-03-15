using System;
using System.Threading.Tasks;
using JohnVerbiest.CQRS.Common;

namespace JohnVerbiest.CQRS.Dependencies
{
    public interface IHandlerRequestDependency
    {
        Task<T> GetHandler<T>() where T : IHaveSingleHandler;

        Task<T[]> GetHandlers<T>(Type locateMe) where T : IHaveMultipleHandlers;
    }
}