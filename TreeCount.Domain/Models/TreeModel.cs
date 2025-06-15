using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Models
{
    [Table("Tree")]
    public class TreeModel : ModelBase
    {
        [Column("nome_popular")]
        [Required]
        [MaxLength(100)]
        public string NomePopular { get; set; }

        [Column("nome_cientifico")]
        [Required]
        [MaxLength(150)]
        public string NomeCientifico { get; set; }

        [Column("descricao")]
        [MaxLength(255)]
        public string? Descricao { get; set; }

        [Column("formula_carbono")]
        [MaxLength(255)]
        public string? FormulaCarbono { get; set; }

        [Column("tipo")]
        [MaxLength(50)]
        public string? Tipo { get; set; }

    }
}
