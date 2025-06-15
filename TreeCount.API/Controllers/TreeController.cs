using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeCount.API.Interfaces;
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
    public class TreeController : ControllerBase
    {
        private readonly ITreeService _treeService;


        public TreeController(ITreeService treeService)
        {
            _treeService = treeService;
        }

        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Route("create")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTreeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var result = await _treeService.CreateAsync<CreateTreeDTO, TreeCreateResponseViewModel>(dto);

            return result.Status switch
            {
                TreeCreateStatus.Success => Ok(result),
                TreeCreateStatus.InvalidData => BadRequest(result),
                TreeCreateStatus.AlreadyExists => Conflict(result),
                TreeCreateStatus.Unauthorized => Unauthorized(result),
                TreeCreateStatus.DatabaseError => StatusCode(500, result),
                _ => StatusCode(500, result)
            };
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> ListAllAsync([FromQuery] ListTreeDTO dto)
        {
            var result = await _treeService.ListPaginatedAsync<ListTreeDTO, TreeListAllResponseViewModel>(dto);

            var firstStatus = result.FirstOrDefault()?.Status ?? TreeGetStatus.Error;

            return firstStatus switch
            {
                TreeGetStatus.Success => Ok(result),
                TreeGetStatus.DatabaseError => StatusCode(StatusCodes.Status500InternalServerError, result),
                TreeGetStatus.Unauthorized => Unauthorized(result),
                TreeGetStatus.Error => StatusCode(StatusCodes.Status500InternalServerError, result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "UnhandledError",
                    Message = "Um erro inesperado ocorreu."
                })
            };
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromQuery] int id)
        {
            var result = await _treeService.ListPaginatedAsync<int, TreeGetByIdResponseViewModel>(id);

            var firstStatus = result.FirstOrDefault()?.Status ?? TreeGetStatus.Error;

            return firstStatus switch
            {
                TreeGetStatus.Success => Ok(result),
                TreeGetStatus.DatabaseError => StatusCode(StatusCodes.Status500InternalServerError, result),
                TreeGetStatus.Unauthorized => Unauthorized(result),
                TreeGetStatus.Error => StatusCode(StatusCodes.Status500InternalServerError, result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Status = "UnhandledError",
                    Message = "Um erro inesperado ocorreu."
                })
            };
        }

        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var result = await _treeService.DeleteAsync<int, TreeDeleteResponseViewModel>(id);

            return result.Status switch
            {
                TreeDeleteStatus.Success => NoContent(),
                TreeDeleteStatus.NotFound => NotFound(result),
                TreeDeleteStatus.Unauthorized => Unauthorized(result),
                TreeDeleteStatus.DatabaseError => StatusCode(500, result),
                TreeDeleteStatus.Error => StatusCode(500, result),
                _ => StatusCode(500, result)
            };
        }

        [HttpPut]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Route("")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTreeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos.");

            var result = await _treeService.UpdateAsync<UpdateTreeDTO, TreeUpdateResponseViewModel>(dto);

            return result.Status switch
            {
                TreeUpdateStatus.Success => NoContent(),
                TreeUpdateStatus.NotFound => NotFound(result),
                TreeUpdateStatus.InvalidData => BadRequest(result),
                TreeUpdateStatus.Unauthorized => Unauthorized(result),
                TreeUpdateStatus.DatabaseError => StatusCode(500, result),
                TreeUpdateStatus.Error => StatusCode(500, result),
                _ => StatusCode(500, result)
            };
        }

    }
}
