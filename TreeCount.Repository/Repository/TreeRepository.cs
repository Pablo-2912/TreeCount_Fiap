using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.Interfaces;
using TreeCount.Domain.Models;
using TreeCount.Repository.Context;

namespace TreeCount.Repository.Repository
{
    public class TreeRepository : RepositoryBase<TreeModel>, ITreeRepository
    {
        public TreeRepository(AppDbContext context) : base(context)
        {
        }
    }
}
