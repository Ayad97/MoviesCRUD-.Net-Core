using MoviesCRUD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesCRUD.MoviesViewModel
{
    public class MovieViewModel
    {

        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Title { get; set; }
        public int Year { get; set; }
        [Range(1,10)]
        public double Rate { get; set; }
        [Required, MaxLength(2500)]
        public string StoreLine { get; set; }
        [Display(Name ="Select Poster...")]
        public byte[] Poster { get; set; }
        public Genre Genre { get; set; }
        [Display(Name ="Genre")]
        public byte GenreId { get; set; }
        public IEnumerable<Genre>genres { get; set; }
    }
}
