using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebMovie.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        [DisplayName("Thể loại")]
        public string Name { get; set; }
        public List<Movie>? Movies {  get; set; }
    }
}
