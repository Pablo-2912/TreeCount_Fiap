using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Models;

namespace TreeCount.Application.DTOs
{
    public class CreateTreeDTO
    {
        [Required(ErrorMessage = "O nome popular é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome popular deve ter no máximo 100 caracteres.")]
        public string NomePopular { get; set; }

        [Required(ErrorMessage = "O nome científico é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome científico deve ter no máximo 150 caracteres.")]
        public string NomeCientifico { get; set; }

        [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string? Descricao { get; set; }

        [MaxLength(255, ErrorMessage = "A fórmula de carbono deve ter no máximo 255 caracteres.")]
        public string? FormulaCarbono { get; set; }

        [MaxLength(50, ErrorMessage = "O tipo deve ter no máximo 50 caracteres.")]
        public string? Tipo { get; set; }
    }

    public class ListTreeDTO
    {
        public int PerPage { get; set; } = 50;

        public int Page { get; set; } = 0 ;

        // public string OderBy { get; set; } = "name";
    }

    public class UpdateTreeDTO
    {
        [Required(ErrorMessage = "O ID é obrigatório.")]
        public long Id { get; set; }

        [Required(ErrorMessage = "O nome popular é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome popular deve ter no máximo 100 caracteres.")]
        public string NomePopular { get; set; }

        [Required(ErrorMessage = "O nome científico é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome científico deve ter no máximo 150 caracteres.")]
        public string NomeCientifico { get; set; }

        [MaxLength(255, ErrorMessage = "A descrição deve ter no máximo 255 caracteres.")]
        public string? Descricao { get; set; }

        [MaxLength(255, ErrorMessage = "A fórmula de carbono deve ter no máximo 255 caracteres.")]
        public string? FormulaCarbono { get; set; }

        [MaxLength(50, ErrorMessage = "O tipo deve ter no máximo 50 caracteres.")]
        public string? Tipo { get; set; }
    }
    public class DeleteTreeDTO
    {
        [Required(ErrorMessage = "O ID é obrigatório.")]
        public long Id { get; set; }
    }
    public class GetTreeByIdDTO
    {
        [Required(ErrorMessage = "O ID é obrigatório.")]
        public long Id { get; set; }
    }

    public static class ToTreeDomain
    {
        public static TreeModel ToDomain(CreateTreeDTO dto)
        {
            return new TreeModel()
            {
                NomePopular = dto.NomePopular,
                NomeCientifico = dto.NomeCientifico,
                Descricao = dto.Descricao,
                FormulaCarbono = dto.FormulaCarbono,
                Tipo = dto.Tipo
            };
        }
    }
}
