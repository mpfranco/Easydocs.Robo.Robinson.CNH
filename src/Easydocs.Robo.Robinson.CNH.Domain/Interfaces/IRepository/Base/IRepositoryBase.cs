using Easydocs.Robo.Robinson.CNH.Domain.DomainObjects;
using System;

namespace Easydocs.Robo.Robinson.CNH.Domain.Interfaces.IRepository.Base
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : IAggregateRoot
    {

    }
}
