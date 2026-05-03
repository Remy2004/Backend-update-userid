
using Aoun.DAL.Repositories.Generic;
using System;
using System.Threading.Tasks;

namespace Aoun.DAL.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable {
    
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    
    Task<int> CompleteAsync();
}





