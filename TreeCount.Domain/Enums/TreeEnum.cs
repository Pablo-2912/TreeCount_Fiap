using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Enums
{
    public enum TreeCreateStatus
    {
        Success,
        InvalidData,
        AlreadyExists,
        Unauthorized,
        DatabaseError,
        Error
    }

    public enum TreeGetStatus
    {
        Success,
        DatabaseError,
        Unauthorized,
        Error
    }

    public enum TreeUpdateStatus
    {
        Success,
        NotFound,
        InvalidData,
        Unauthorized,
        DatabaseError,
        Error
    }

    public enum TreeDeleteStatus
    {
        Success,
        NotFound,
        Unauthorized,
        DatabaseError,
        Error
    }

}
