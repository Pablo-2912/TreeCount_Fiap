using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Enums
{
    public enum HistoryCreateStatus
    {
        Success,
        InvalidData,
        AlreadyExists,
        Unauthorized,
        DatabaseError,
        Error
    }

    public enum HistoryUpdateStatus
    {
        Success,
        NotFound,
        InvalidData,
        Unauthorized,
        DatabaseError,
        Error
    }

    public enum HistoryDeleteStatus
    {
        Success,
        NotFound,
        Unauthorized,
        DatabaseError,
        Error
    }

    public enum HistoryGetStatus
    {
        Success,
        NotFound,
        Unauthorized,
        DatabaseError,
        Error
    }

}
