using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Contracts.Repository;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository {

	public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class {

		private COADAPTContext CoadaptContext { get; }

		protected RepositoryBase(COADAPTContext coadaptContext) {
			CoadaptContext = coadaptContext;
		}

		public IQueryable<T> FindAll() {
			return CoadaptContext.Set<T>().AsNoTracking();
		}

		public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) {
			return CoadaptContext.Set<T>().Where(expression).AsNoTracking();
		}

		public void Create(T entity) {
			CoadaptContext.Set<T>().Add(entity);
		}

		public void Update(T entity) {
			CoadaptContext.Set<T>().Update(entity);
		}

		public void Delete(T entity) {
			CoadaptContext.Set<T>().Remove(entity);
		}

		public void DeleteRange(List<T> entities) {
			CoadaptContext.Set<T>().RemoveRange(entities);
		}

		public async Task<int> CountAll() {
			return await CoadaptContext.Set<T>().CountAsync();
		}

		public async Task<int> CountByCondition(Expression<Func<T, bool>> expression) {
			return await CoadaptContext.Set<T>().Where(expression).CountAsync();
		}

		public void Detach(T entity) {
			CoadaptContext.Entry(entity).State = EntityState.Detached;
		}

	}

}
