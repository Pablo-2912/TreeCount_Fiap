using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Enums;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Domain.Models;
using TreeCount.Repository.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using IHistoryRepository = TreeCount.Domain.Interfaces.Repository.IHistoryRepository;

namespace TreeCount.Application.Services
{
    public class HistoryService : ServiceBase<HistoryModel>, IHistoryService
    {
        private readonly IHistoryRepository _repository;

        public HistoryService(IHistoryRepository repository) : base(repository)
        {
            _repository = repository;
        }

        //public new async Task<HistoryGetByIdResponseViewModel> GetByUserId(GetByUserIdPaginatedDTO dto)
        //{
        //    try
        //    {
        //        var histories = await _repository.GetByUserIdPaginated(dto.UserId, dto.Page, dto.PageSize);

        //        if (histories == null || !histories.Any())
        //        {
        //            return new HistoryGetByIdResponseViewModel
        //            {
        //                Status = HistoryGetStatus.NotFound,
        //                Message = "No history records found for the specified user.",
        //                Data = null
        //            };
        //        }

        //        // Caso você esteja retornando um único item (o mais recente, por exemplo), você pode fazer:
        //        var history = histories.First(); // ou .FirstOrDefault(), se necessário

        //        var dtoResult = new HistoryResponseDTO
        //        {
        //            Id = history.Id.ToString(),
        //            Latitude = history.Latitude,
        //            Longitude = history.Longitude,
        //            PlantingRadius = history.PlantingRadius,
        //            Quantity = history.Quantity,
        //            TreeId = history.TreeId.ToString(),
        //            UserId = history.UserId,
        //            CreatedAt = history.CreateAt
        //        };

        //        return new HistoryGetByIdResponseViewModel
        //        {
        //            Status = HistoryGetStatus.Success,
        //            Message = "History retrieved successfully.",
        //            Data = dtoResult
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log ex se necessário

        //        return new HistoryGetByIdResponseViewModel
        //        {
        //            Status = HistoryGetStatus.Error,
        //            Message = "An error occurred while retrieving the history.",
        //            Data = null
        //        };
        //    }
        //}

        public new async Task<HistoryListPaginatedResponseViewModel> ListByUserIdAsync(GetByUserIdPaginatedDTO dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            int page = dto.Page < 1 ? 1 : dto.Page;
            int pageSize = dto.PageSize switch
            {
                < 1 => 1,
                > 50 => 50,
                _ => dto.PageSize
            };

            try
            {
                var query = _repository.Query(); // IQueryable<HistoryModel>

                var filtered = query.Where(h => h.UserId == dto.UserId);

                int totalItems = await filtered.CountAsync();

                var items = await filtered
                    .OrderByDescending(h => h.CreateAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var data = items.Select(entity => new HistoryResponseDTO
                {
                    Id = entity.Id.ToString(),
                    Latitude = entity.Latitude,
                    Longitude = entity.Longitude,
                    PlantingRadius = entity.PlantingRadius,
                    Quantity = entity.Quantity,
                    TreeId = entity.TreeId.ToString(),
                    UserId = entity.UserId,
                    CreatedAt = entity.CreateAt
                });

                if (!data.Any())
                {
                    return new HistoryListPaginatedResponseViewModel
                    {
                        Status = HistoryGetStatus.NotFound,
                        Message = "Nenhum histórico encontrado para o usuário.",
                        Data = Enumerable.Empty<HistoryResponseDTO>(),
                        TotalItems = 0,
                        CurrentPage = page,
                        PageSize = pageSize
                    };
                }

                return new HistoryListPaginatedResponseViewModel
                {
                    Status = HistoryGetStatus.Success,
                    Message = "Histórico carregado com sucesso.",
                    Data = data,
                    TotalItems = totalItems,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception)
            {
                return new HistoryListPaginatedResponseViewModel
                {
                    Status = HistoryGetStatus.DatabaseError,
                    Message = "Erro ao acessar os dados.",
                    Data = Enumerable.Empty<HistoryResponseDTO>(),
                    TotalItems = 0,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
        }

        public new async Task<HistoryCreateResponseViewModel> CreateAsync(CreateHistoryDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                var model = new HistoryModel
                {
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    PlantingRadius = dto.PlantingRadius,
                    Quantity = dto.Quantity,
                    TreeId = dto.TreeId,
                    UserId = dto.UserId
                };

                model.OnCreate();

                var result = await _repository.CreateAsync(model);

                if (result == null)
                {
                    return new HistoryCreateResponseViewModel
                    {
                        Status = HistoryCreateStatus.InvalidTree,
                        Message = "Árvore não encontrada."
                    };
                }

                return new HistoryCreateResponseViewModel
                {
                    Status = HistoryCreateStatus.Success,
                    Message = "Histórico criado com sucesso.",
                    Data = new HistoryResponseDTO
                    {
                        Id = result.Id.ToString(),
                        Latitude = result.Latitude,
                        Longitude = result.Longitude,
                        PlantingRadius = result.PlantingRadius,
                        Quantity = result.Quantity,
                        TreeId = result.TreeId.ToString(),
                        UserId = result.UserId,
                        CreatedAt = result.CreateAt
                    }
                };
            }
            catch
            {
                return new HistoryCreateResponseViewModel
                {
                    Status = HistoryCreateStatus.Error,
                    Message = "Erro ao criar o histórico."
                };
            }
        }

        protected override object ExtractId<D>(D dto)
        {
            return dto switch
            {
                DeleteHistoryDTO d => d.Id,
                GetByIdHistoryDTO g => g.Id,
                UpdateHistoryDTO u => u.Id,
                // Se quiser incluir os paginados, mesmo que tecnicamente não sejam usados pra isso:
                GetByUserIdPaginatedDTO user => user.UserId,
                GetByTreeIdPaginatedDTO tree => tree.TreeId,
                _ => throw new InvalidCastException("DTO não suportado ou não contém ID.")
            };
        }

        protected override HistoryModel MapToEntity<D>(D dto)
        {
            return dto switch
            {
                CreateHistoryDTO create => new HistoryModel
                {
                    Latitude = create.Latitude,
                    Longitude = create.Longitude,
                    PlantingRadius = create.PlantingRadius,
                    Quantity = create.Quantity,
                    TreeId = create.TreeId,
                    UserId = create.UserId
                },
                UpdateHistoryDTO update => new HistoryModel
                {
                    Id = Convert.ToInt64(update.Id),
                    Latitude = update.Latitude,
                    Longitude = update.Longitude,
                    PlantingRadius = update.PlantingRadius,
                    Quantity = update.Quantity,
                    TreeId = Convert.ToInt64(update.TreeId),
                    UserId = update.UserId
                },
                DeleteHistoryDTO delete => new HistoryModel
                {
                    Id = Convert.ToInt64(delete.Id)
                },
                _ => throw new InvalidCastException("DTO não suportado para mapeamento de entidade.")
            };

        }

        protected override VM MapToViewModel<VM>(HistoryModel model)
        {
            object result = typeof(VM).Name switch
            {
                nameof(HistoryCreateResponseViewModel) => new HistoryCreateResponseViewModel
                {
                    Status = model != null ? HistoryCreateStatus.Success : HistoryCreateStatus.Error,
                    Message = model != null ? "Histórico criado com sucesso." : "Histórico não foi criado.",
                    Data = model != null ? new HistoryResponseDTO
                    {
                        Id = model.Id.ToString(),
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        PlantingRadius = model.PlantingRadius,
                        Quantity = model.Quantity,
                        TreeId = model.TreeId.ToString(),
                        UserId = model.UserId,
                        CreatedAt = model.CreateAt
                    } : null
                },

                nameof(HistoryUpdateResponseViewModel) => new HistoryUpdateResponseViewModel
                {
                    Status = model != null ? HistoryUpdateStatus.Success : HistoryUpdateStatus.Error,
                    Message = model != null ? "Histórico atualizado com sucesso." : "Histórico não foi atualizado."
                },

                nameof(HistoryDeleteResponseViewModel) => new HistoryDeleteResponseViewModel
                {
                    Status = model != null ? HistoryDeleteStatus.Success : HistoryDeleteStatus.Error,
                    Message = model != null ? "Histórico deletado com sucesso." : "Histórico não foi deletado."
                },

                nameof(HistoryGetByIdResponseViewModel) => new HistoryGetByIdResponseViewModel
                {
                    Status = model != null ? HistoryGetStatus.Success : HistoryGetStatus.NotFound,
                    Message = model != null ? "Histórico encontrado com sucesso." : "Histórico não encontrado.",
                    Data = model != null ? new HistoryResponseDTO
                    {
                        Id = model.Id.ToString(),
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        PlantingRadius = model.PlantingRadius,
                        Quantity = model.Quantity,
                        TreeId = model.TreeId.ToString(),
                        UserId = model.UserId,
                        CreatedAt = model.CreateAt
                    } : null
                },

                _ => Activator.CreateInstance<VM>()
            };

            return (VM)result;
        }



    }
}
