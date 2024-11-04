using Book.DataAccess.Data;
using Book.DataAccess.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public T Get(Expression<Func<T, bool>> filter, string includeProperties = null, bool tracked = false)
    {
        IQueryable<T> query;
        if (tracked)
        {
            query = dbSet;  
        }
        else
        {
            query = dbSet.AsNoTracking();
        }

        query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (_db.Model.FindEntityType(typeof(T)).FindNavigation(includeProp) != null)
                {
                    query = query.Include(includeProp);
                }
            }
        }

        return query.FirstOrDefault();
    }
    public T GetById(int id, string includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        // Assuming the entity has a property named "Id"
        query = query.Where(e => e.GetType().GetProperty("Id").GetValue(e).Equals(id));

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (_db.Model.FindEntityType(typeof(T)).FindNavigation(includeProp) != null)
                {
                    query = query.Include(includeProp);
                }
            }
        }

        return query.FirstOrDefault();
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }

		if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
             
                if (_db.Model.FindEntityType(typeof(T)).FindNavigation(includeProp) != null)
                {
                    query = query.Include(includeProp);
                }
            }
        }

        return query.ToList();
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entity)
    {
        dbSet.RemoveRange(entity);
    }
}

