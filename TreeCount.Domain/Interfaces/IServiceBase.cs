using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Interfaces.Repository;

namespace TreeCount.Domain.Interfaces
{
    public interface IServiceBase<T> : IRepositoryBase<T> where T : class
    {

    }
}
