using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Models;

namespace TreeCount.Application.Interfaces
{
    public interface IHistoryService : IServiceBase
    {
        Task<HistoryCreateResponseViewModel> CreateAsync(CreateHistoryDTO dto);

        Task<HistoryListPaginatedResponseViewModel> ListByUserIdAsync(GetByUserIdPaginatedDTO dto);
    }
}
