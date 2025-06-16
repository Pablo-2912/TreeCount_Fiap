using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Models
{
    [Table("History")]
    public class HistoryModel : ModelBase
    {
        [Column("latitude")]
        [Required]
        public double Latitude { get; set; }

        [Column("longitude")]
        [Required]
        public double Longitude { get; set; }

        [Column("planting_radius")]
        [Required]
        public double PlantingRadius { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("tree_id")]
        [Required]
        public long TreeId { get; set; }

        [ForeignKey("TreeId")]
        public virtual TreeModel Tree { get; set; }

        [Column("user_id", TypeName = "varchar(36)")]
        [Required]
        public string UserId { get; set; }

    }
}
