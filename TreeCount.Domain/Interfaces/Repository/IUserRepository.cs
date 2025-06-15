using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Interfaces.Repository
{
    public interface IUserRepository<T> : IRepositoryBase<T> where T : class
    {
    }
}
