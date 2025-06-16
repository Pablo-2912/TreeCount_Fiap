using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Common.Helpers;
using TreeCount.Domain.Enums;
using TreeCount.Repository.Identity;

namespace TreeCount.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        private readonly UserManager<UserModel> _userManager;
        public HistoryController(IHistoryService historyService, UserManager<UserModel> userManager)
        {
            _userManager = userManager;
            _historyService = historyService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateHistoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new HistoryCreateResponseViewModel
                {
                    Status = HistoryCreateStatus.InvalidData,
                    Message = "Invalid data.",
                    Data = null
                });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new HistoryCreateResponseViewModel
                {
                    Status = HistoryCreateStatus.Unauthorized,
                    Message = "User not authenticated.",
                    Data = null
                });
            }

            dto.UserId = user.Id;

            var result = await _historyService.CreateAsync(dto);

            if (result == null)
            {
                return BadRequest(new HistoryCreateResponseViewModel
                {
                    Status = HistoryCreateStatus.Error,
                    Message = "Could not create history.",
                    Data = null
                });
            }

            // Aqui já espera que o service retorne o ViewModel com status, message e data preenchidos
            return result.Status switch
            {
                HistoryCreateStatus.Success => CreatedAtAction(nameof(GetByIdAsync), new { id = result.Data.Id }, result),
                HistoryCreateStatus.AlreadyExists => Conflict(result),
                HistoryCreateStatus.DatabaseError => StatusCode(500, result),
                HistoryCreateStatus.InvalidData => BadRequest(result),
                HistoryCreateStatus.Unauthorized => Unauthorized(result),
                _ => BadRequest(result)
            };

        }

        [HttpDelete("")]
        [Authorize(Roles = "SUPER_ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteHistoryDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new HistoryDeleteResponseViewModel
                {
                    Status = HistoryDeleteStatus.Error,
                    Message = "Invalid data."
                });

            var result = await _historyService.DeleteAsync<DeleteHistoryDTO, HistoryDeleteResponseViewModel>(dto);

            return result.Status switch
            {
                HistoryDeleteStatus.Success => NoContent(),

                HistoryDeleteStatus.NotFound => NotFound(result),

                HistoryDeleteStatus.Unauthorized => Unauthorized(result),

                HistoryDeleteStatus.DatabaseError => StatusCode(500, result),

                _ => BadRequest(result)
            };
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string id)
        {
            var dto = new GetByIdHistoryDTO { Id = id };
            var result = await _historyService.GetByIdAsync<GetByIdHistoryDTO, HistoryGetByIdResponseViewModel>(dto);

            return result.Status switch
            {
                HistoryGetStatus.Success => Ok(result),

                HistoryGetStatus.NotFound => NotFound(result),

                HistoryGetStatus.Unauthorized => Unauthorized(result),

                HistoryGetStatus.DatabaseError => StatusCode(500, result),

                _ => BadRequest(result)
            };
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetByUserIdPaginatedAsync([FromQuery] string? userId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("Usuário não autenticado.");

            var isSuperAdmin = await _userManager.IsInRoleAsync(user, UserRolesHelper.ToStringRole(UserRoles.SuperAdmin) );

            // Se não for superadmin, força o userId a ser o próprio
            var resolvedUserId = isSuperAdmin && !string.IsNullOrEmpty(userId) ? userId : user.Id;

            var dto = new GetByUserIdPaginatedDTO
            {
                UserId = resolvedUserId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _historyService.ListByUserIdAsync(dto);

            if (result != null && result.Data.Any())
                return Ok(result);

            return NotFound("Nenhum histórico encontrado para o usuário.");
        }

        [Authorize(Roles = "SUPER_ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var dto = new PaginatedHistoryDTO
            {
                Page = page,
                PageSize = pageSize
            };

            var result = await _historyService.ListPaginatedAsync<PaginatedHistoryDTO, IEnumerable<HistoryResponseDTO>>(dto);

            if (result != null)
                return Ok(result);

            return NotFound("No histories found.");
        }
    }


}
