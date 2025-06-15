using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TreeCount.Domain.Models
{
    public abstract class ModelBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("Create_at")]
        public DateTime CreateAt{ get; set; }

        [Column("Update_at")]
        public DateTime UpdateAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        // Simula @PrePersist no EF Core
        public void OnCreate()
        {
            CreateAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
            DeletedAt = null;
        }

        // Simula @PreUpdate no EF Core
        public void OnUpdate()
        {
            UpdateAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            DeletedAt = DateTime.UtcNow;
        }
    }
}
