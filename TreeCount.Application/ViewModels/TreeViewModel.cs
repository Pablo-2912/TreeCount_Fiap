using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Enums;
using TreeCount.Domain.Models;

namespace TreeCount.Application.ViewModels
{
    public class TreeCreateResponseViewModel
    {
        public TreeCreateStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public long TreeId { get; set; }
    }

    public class TreeListAllResponseViewModel
    {
        public TreeGetStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public IEnumerable<TreeModel> Itens { get; set; } = Enumerable.Empty<TreeModel>();
    }    
    
    public class TreeGetByIdResponseViewModel
    {
        public TreeGetStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public TreeModel? Item { get; set; }
    }

    public class TreeUpdateResponseViewModel
    {
        public TreeUpdateStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class TreeDeleteResponseViewModel
    {
        public TreeDeleteStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }


}
