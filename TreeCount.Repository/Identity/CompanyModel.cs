using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Repository.Identity
{
    [Table("Company")]
    internal class CompanyModel: IdentityUser
    {
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name", TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name", TypeName = "nvarchar(100)")]
        public string Cnpj { get; set; }

        [Required]
        [Column("Create_at")]
        public DateTime CreateAt { get; set; }

        [Required]
        [Column("Update_at")]
        public DateTime UpdateAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public void OnCreate()
        {
            CreateAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
            DeletedAt = null;
        }

        public void OnUpdate()
        {
            UpdateAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            DeletedAt = DateTime.UtcNow;

            // Evita conflito de email único ao reutilizar o mesmo email no futuro
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Id))
            {
                Email = $"{Email}-{Id}";
            }
        }
    }
}
