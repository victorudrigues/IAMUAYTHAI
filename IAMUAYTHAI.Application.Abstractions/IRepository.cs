﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
    }
}
