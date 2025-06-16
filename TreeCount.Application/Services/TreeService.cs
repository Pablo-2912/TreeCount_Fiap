using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Enums;
using TreeCount.Domain.Models;
using TreeCount.Repository.Repository;

namespace TreeCount.Application.Services
{
    public class TreeService : ServiceBase<TreeModel>, ITreeService
    {
        private readonly ITreeRepository _treeRepository;

        public TreeService(ITreeRepository treeRepository)
            : base(treeRepository)
        {
            _treeRepository = treeRepository;
        }

        protected override TreeModel MapToEntity<D>(D dto)
        {
            return dto switch
            {
                CreateTreeDTO create => new TreeModel
                {
                    NomePopular = create.NomePopular,
                    NomeCientifico = create.NomeCientifico,
                    Descricao = create.Descricao,
                    FormulaCarbono = create.FormulaCarbono,
                    Tipo = create.Tipo
                },
                DeleteTreeDTO del => new TreeModel { Id = del.Id },
                UpdateTreeDTO update => new TreeModel
                {
                    Id = update.Id,
                    NomePopular = update.NomePopular,
                    NomeCientifico = update.NomeCientifico,
                    Descricao = update.Descricao,
                    FormulaCarbono = update.FormulaCarbono,
                    Tipo = update.Tipo
                },
                _ => throw new InvalidCastException("DTO não suportado.")
            };
        }

        protected override VM MapToViewModel<VM>(TreeModel model)
        {
            object result = typeof(VM).Name switch
            {
                nameof(TreeCreateResponseViewModel) => new TreeCreateResponseViewModel
                {
                    Status = TreeCreateStatus.Success,
                    TreeId = model.Id
                },
                nameof(TreeDeleteResponseViewModel) => new TreeDeleteResponseViewModel
                {
                    Status = TreeDeleteStatus.Success
                },
                nameof(TreeListAllResponseViewModel) => new TreeListAllResponseViewModel
                {
                    Status = TreeGetStatus.Success,
                    Itens = new List<TreeModel> { model }
                },
                nameof(TreeGetByIdResponseViewModel) => new TreeGetByIdResponseViewModel
                {
                    Status = TreeGetStatus.Success,
                    Item = model
                },
                _ => throw new InvalidCastException("ViewModel não suportada.")
            };

            return (VM)result;
        }

        protected override object ExtractId<D>(D dto)
        {
            return dto switch
            {
                DeleteTreeDTO del => del.Id,
                GetTreeByIdDTO get => get.Id,
                _ => throw new InvalidCastException("DTO não contém ID.")
            };
        }
    }
}
