using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using TreeCount.Application.DTOs;
using TreeCount.Domain.Enums;

namespace TreeCount.Application.ViewModels
{
    public class HistoryCreateResponseViewModel
    {
        public HistoryCreateStatus Status { get; set; }
        public string Message { get; set; }
        public HistoryResponseDTO Data { get; set; }
    }

    public class HistoryUpdateResponseViewModel
    {
        public HistoryUpdateStatus Status { get; set; }
        public string Message { get; set; }
    }

    public class HistoryDeleteResponseViewModel
    {
        public HistoryDeleteStatus Status { get; set; }
        public string Message { get; set; }
    }

    public class HistoryGetByIdResponseViewModel
    {
        public HistoryGetStatus Status { get; set; }
        public string Message { get; set; }
        public HistoryResponseDTO Data { get; set; }
    }

    public class HistoryListPaginatedResponseViewModel
    {
        public HistoryGetStatus Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<HistoryResponseDTO> Data { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
