using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace TreeCount.Application.DTOs
{

    public class CreateHistoryDTO
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double PlantingRadius { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string TreeId { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class UpdateHistoryDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public double PlantingRadius { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string TreeId { get; set; }

        [Required]
        public string UserId { get; set; }
    }

    public class DeleteHistoryDTO
    {
        [Required]
        public string Id { get; set; }
    }

    public class GetByIdHistoryDTO
    {
        [Required]
        public string Id { get; set; }
    }

    public class PaginatedHistoryDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetByUserIdPaginatedDTO : PaginatedHistoryDTO
    {
        [Required]
        public string UserId { get; set; }
    }

    public class GetByTreeIdPaginatedDTO : PaginatedHistoryDTO
    {
        [Required]
        public string TreeId { get; set; }
    }

    public class HistoryResponseDTO
    {
        public string Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double PlantingRadius { get; set; }
        public int Quantity { get; set; }
        public string TreeId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
