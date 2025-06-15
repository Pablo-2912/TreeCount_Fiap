using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TreeCount.API.Controllers;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Enums;
using TreeCount.Domain.Models;
using Xunit;

public class TreeControllerTests
{
    private readonly Mock<ITreeService> _treeServiceMock;
    private readonly TreeController _controller;

    public TreeControllerTests()
    {
        _treeServiceMock = new Mock<ITreeService>();
        _controller = new TreeController(_treeServiceMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ReturnsOk_WhenSuccess()
    {
        var createDto = new CreateTreeDTO
        {
            NomePopular = "Ipê",
            NomeCientifico = "Handroanthus",
            Descricao = "Árvore brasileira",
            FormulaCarbono = "C6H12O6",
            Tipo = "Florífera"
        };

        var response = new TreeCreateResponseViewModel
        {
            Status = TreeCreateStatus.Success,
            TreeId = 123
        };

        _treeServiceMock
            .Setup(s => s.CreateAsync<CreateTreeDTO, TreeCreateResponseViewModel>(createDto))
            .ReturnsAsync(response);

        var result = await _controller.CreateAsync(createDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TreeCreateResponseViewModel>(okResult.Value);
        Assert.Equal(TreeCreateStatus.Success, returned.Status);
        Assert.Equal(123, returned.TreeId);
    }

    [Fact]
    public async Task ListAllAsync_ReturnsOk_WhenSuccess()
    {
        var listDto = new ListTreeDTO { Page = 1, PerPage = 10 };

        var treeList = new List<TreeListAllResponseViewModel>
        {
            new TreeListAllResponseViewModel
            {
                Status = TreeGetStatus.Success,
                Itens = new List<TreeModel>
                {
                    new TreeModel { NomePopular = "Ipê", NomeCientifico = "Handroanthus" }
                }
            }
        };

        _treeServiceMock
            .Setup(s => s.ListPaginatedAsync<ListTreeDTO, TreeListAllResponseViewModel>(listDto))
            .ReturnsAsync(treeList);

        var result = await _controller.ListAllAsync(listDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<TreeListAllResponseViewModel>>(okResult.Value);

        Assert.Single(returned);
        Assert.Equal(TreeGetStatus.Success, returned.First().Status);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOk_WhenSuccess()
    {
        int treeId = 10;
        var treeByIdResponse = new List<TreeGetByIdResponseViewModel>
    {
        new TreeGetByIdResponseViewModel
        {
            Status = TreeGetStatus.Success,
            Item = new TreeModel
            {
                NomePopular = "Ipê",
                NomeCientifico = "Handroanthus"
            }
        }
    };

        _treeServiceMock
            .Setup(s => s.ListPaginatedAsync<int, TreeGetByIdResponseViewModel>(treeId))
            .ReturnsAsync(treeByIdResponse);

        var result = await _controller.GetByIdAsync(treeId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<TreeGetByIdResponseViewModel>>(okResult.Value);

        Assert.Single(returned);
        Assert.Equal(TreeGetStatus.Success, returned.First().Status);
        Assert.Equal("Ipê", returned.First().Item.NomePopular);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenSuccess()
    {
        int treeId = 5;

        var response = new TreeDeleteResponseViewModel
        {
            Status = TreeDeleteStatus.Success
        };

        _treeServiceMock
            .Setup(s => s.DeleteAsync<int, TreeDeleteResponseViewModel>(treeId))
            .ReturnsAsync(response);

        var result = await _controller.DeleteAsync(treeId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNoContent_WhenSuccess()
    {
        var updateDto = new UpdateTreeDTO
        {
            Id = 1,
            NomePopular = "Ipê Amarelo",
            NomeCientifico = "Handroanthus albus",
            Descricao = "Árvore brasileira muito comum",
            FormulaCarbono = "C6H12O6",
            Tipo = "Florífera"
        };

        var response = new TreeUpdateResponseViewModel
        {
            Status = TreeUpdateStatus.Success
        };

        _treeServiceMock
            .Setup(s => s.UpdateAsync<UpdateTreeDTO, TreeUpdateResponseViewModel>(updateDto))
            .ReturnsAsync(response);

        var result = await _controller.UpdateAsync(updateDto);

        Assert.IsType<NoContentResult>(result);
    }
}
