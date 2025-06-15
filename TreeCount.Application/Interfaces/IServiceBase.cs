using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.ViewModels;
using TreeCount.Repository.Identity;

namespace TreeCount.Application.Interfaces
{
    public interface IServiceBase
    {
        Task<VM> CreateAsync<D, VM>(D model);
        Task<VM> UpdateAsync<D, VM>(D model);
        Task<IEnumerable<DR>> ListPaginatedAsync<D, DR>(D model);
        Task<VM> DeleteAsync<D, VM>(D model);
        Task<VM> GetByIdAsync<D, VM>(D model);
        Task<IEnumerable<VM>> ListAllAsync<D, VM>(D model);
    }

}
