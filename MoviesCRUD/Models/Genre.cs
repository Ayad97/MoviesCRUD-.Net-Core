using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesCRUD.Models
{
    public class Genre
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte GenreId { get; set; }
        [Required,MaxLength(200)]
        [Display(Name = "GenreName")]
        public string Name { get; set; }
    }
}
