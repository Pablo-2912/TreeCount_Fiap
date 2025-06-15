using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Domain.Models;

namespace TreeCount.Application.Interfaces
{
    public interface ITreeRepository : IRepositoryBase<TreeModel>
    {


    }
}
