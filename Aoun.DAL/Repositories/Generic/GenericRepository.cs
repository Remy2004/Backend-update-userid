using Aoun.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Aoun.DAL.Repositories.Generic; // العنوان الصحيح الجديد

public class GenericRepository<T> : IGenericRepository<T> where T : class {
    private readonly ApplicationDbContext _context;
    public GenericRepository(ApplicationDbContext context) => _context = context;

    public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) {
        return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public void Add(T entity) => _context.Set<T>().Add(entity);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);
}




