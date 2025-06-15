using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Models;

namespace TreeCount.Domain.Interfaces.Repository
{
    public interface IHistoryRepository : IRepositoryBase<HistoryModel>
    {
        Task<IEnumerable<HistoryModel>> GetByUserIdPaginated(string userId, int page, int pageSize);
    }
}
