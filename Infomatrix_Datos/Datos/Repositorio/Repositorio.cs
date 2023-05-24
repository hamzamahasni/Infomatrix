using Infomatrix_Datos.Datos.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infomatrix_Datos.Datos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            this.db = db;
            this.dbSet = db.Set<T>();
        }
        public void Agregar(T entidad)
        {
            dbSet.Add(entidad);
        }

        public void grabar()
        {
            db.SaveChanges();
        }

        public T Obtener(int id)
        {
            return dbSet.Find(id);
        }

        public T ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);
            }
            if (incluirPropiedades != null)
            {
                foreach (var incluirProp in incluirPropiedades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); //ejemplo "Categoria,Marca"
                }

            }
            if (!isTracking)
            {
                query = query.AsNoTracking(); // Se pueda modificar los datos
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if(filtro != null)
            {
                query = query.Where(filtro);
            }
            if(incluirPropiedades != null)
            {
                foreach(var incluirProp in incluirPropiedades.Split(new char[] { ','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(incluirProp); //ejemplo "Categoria,Marca"
                }

            }
            if (orderBy != null) { 
                query = orderBy(query);
            }
            if(!isTracking)
            {
                query = query.AsNoTracking(); // Se pueda modificar los datos
            }
            return query.ToList();
        }

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
