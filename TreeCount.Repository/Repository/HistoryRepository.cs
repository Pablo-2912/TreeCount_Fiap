using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Domain.Models;
using TreeCount.Repository.Context;

namespace TreeCount.Repository.Repository
{
    public class HistoryRepository : RepositoryBase<HistoryModel>, IHistoryRepository
    {
        public HistoryRepository(AppDbContext context) : base(context)
        {
           

        }
        public async Task<IEnumerable<HistoryModel>> GetByUserIdPaginated(string userId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            return await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreateAt) // opcional, ordenação recomendada
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public override async Task<HistoryModel> CreateAsync(HistoryModel model)
        {
            // Verifica se a Tree existe no banco
            bool treeExists = await _context.Tree.AnyAsync(t => t.Id == model.TreeId);

            if (!treeExists)
            {
                return null;
            }

            // Se passou na validação, chama o base para salvar normalmente
            return await base.CreateAsync(model);
        }
    }
}
