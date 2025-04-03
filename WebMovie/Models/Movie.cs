using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebMovie.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        [DisplayName("Tên")]
        public string Name { get; set; }
        [DisplayName("Mô tả")]
        public string Description { get; set; }
        [Required, StringLength(100)]
        [DisplayName("Đạo diễn")]
        public string Director {  get; set; }
        [DisplayName("Link chi tiết")]
        public string LinkInfo {  get; set; }
        [DisplayName("Thời lượng")]
        public string Duration {  get; set; }
        [DisplayName("Năm phát hành")]
        public int ReleaseYear {  get; set; }
        [DisplayName("Lượt xem")]
        public int view { get; set; }
        [DisplayName("Đường dẫn Trailer")]
        public string? TrailerUrl {  get; set; }
        [DisplayName("Đường dẫn ảnh")]
        public string? ImageUrl { get; set; }
        [DisplayName("Đường dẫn video")]
        public string? MovieUrl { get; set; }
        
        public int GenreId { get; set; }
        [DisplayName("Thể loại")]
        public Genre?  Genre { get; set; }
        
        public int CountryId { get; set; }
        [DisplayName("Quốc gia")]
        public Country? Country { get; set; }
    }
}
