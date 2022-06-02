using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static NationalParksAPI.Models.Trail;

namespace NationalParksAPI.Models.DTOs
{
    public class TrailCreateDTO
    {
        
        [Required]
        public string Name { get; set; }
        [Required]
        public double Distance { get; set; }
        [Required]
        public double Elevation { get; set; }
        public DifficultyType Difficulty { get; set; }
        [Required]
        public int NationalParkId { get; set; }

    

    }
}
