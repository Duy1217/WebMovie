using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
using System.Text;
using WebMovie.Models;
using WebMovie.Repositories;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;
using WebMovie.Areas.Admin.Models;

namespace WebMovie.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MovieManagerController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public MovieManagerController(IMovieRepository movieRepository,
        IGenreRepository genreRepository,
        ICountryRepository countryRepository,
        IWebHostEnvironment hostingEnvironment,
        ApplicationDbContext dbContext)
        {
            _genreRepository = genreRepository;
            _movieRepository = movieRepository;
            _countryRepository = countryRepository;
            _hostingEnvironment = hostingEnvironment;
            _dbContext = dbContext;
        }
        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(string searchString, int? selectedGenre, int? selectedCountry, int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var movies = await _movieRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString) && !selectedGenre.HasValue && !selectedCountry.HasValue)
            {
                movies = movies.Where(p => RemoveAccents(p.Name).ToUpper().Contains(RemoveAccents(searchString).ToUpper())
                    || p.Description.ToUpper().Contains(searchString.ToUpper())
                    || RemoveAccents(p.Director).ToUpper().Contains(RemoveAccents(searchString).ToUpper())).ToList();
            }
            if (selectedGenre.HasValue && selectedGenre != 0)
            {
                movies = movies.Where(m => m.GenreId == selectedGenre).ToList();
            }

            if (selectedCountry.HasValue && selectedCountry != 0)
            {
                movies = movies.Where(m => m.CountryId == selectedCountry).ToList();
            }
            var genres = await _genreRepository.GetAllAsync();

            var countries = await _countryRepository.GetAllAsync();

            ViewBag.SelectedGenre = new SelectList(genres, "Id", "Name", selectedGenre);
            ViewBag.SelectedCountry = new SelectList(countries, "Id", "Name", selectedCountry);
            ViewBag.Genres = selectedGenre;
            ViewBag.Countries = selectedCountry;
            ViewBag.SearchString = searchString;
            const int pageSize = 8;
            var pagedMovie = movies.ToPagedList(page, pageSize);

            return View(pagedMovie);
        }
        private string RemoveAccents(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{M}");
            return regex.Replace(normalizedString, string.Empty);
        }

        // Hiển thị form thêm sản phẩm mới
        public async Task<IActionResult> Add()
        {
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            return View();
        }
        // Xử lý thêm sản phẩm mới
        [HttpPost]
        public async Task<IActionResult> Add(Movie movie, IFormFile imageUrl, IFormFile movieUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage
                    movie.ImageUrl = await SaveImage(imageUrl, movie.Name);
                    movie.MovieUrl = await SaveMovie(movieUrl, movie.Name);
                }
                else
                {
                    movie.ImageUrl = null;
                    movie.MovieUrl = null;
                }
                movie.view = 0;
                await _movieRepository.AddAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            return View(movie);
        }
        private async Task<string> SaveImage(IFormFile image, string name)
        {
            // Xóa các ký tự không hợp lệ trong tên tệp để đảm bảo an toàn
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            var safeMovieName = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

            // Tạo tên file image từ tên của phim và phần mở rộng
            var fileName = $"{safeMovieName}{Path.GetExtension(image.FileName)}";

            var savePath = Path.Combine("wwwroot/Images", fileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/Images/" + fileName; // Trả về đường dẫn tương đối
        }
        // Viết thêm hàm SaveImage (tham khảo bài 02)
        private async Task<string> SaveMovie(IFormFile video, string name)
        {
            // Xóa các ký tự không hợp lệ trong tên tệp để đảm bảo an toàn
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            var safeMovieName = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

            // Tạo tên file video từ tên của phim và phần mở rộng
            var fileName = $"{safeMovieName}{Path.GetExtension(video.FileName)}";

            // Kết hợp đường dẫn lưu trữ với tên file
            var savePath = Path.Combine("wwwroot/videos", fileName);

            // Lưu file video vào thư mục lưu trữ
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await video.CopyToAsync(fileStream);
            }
            return "/videos/" + fileName; // Trả về đường dẫn tương đối
        }

        //Hien thi chi tiet
        public async Task<IActionResult> Detail(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        public async Task<IActionResult> Update(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            return View(movie);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Movie movie, IFormFile imageUrl, IFormFile movieUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
            ModelState.Remove("MovieUrl");
            if (id != movie.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingMovie = await _movieRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (imageUrl == null)
                {
                    movie.ImageUrl = existingMovie.ImageUrl;
                }
                else
                {
                    // Xác định đường dẫn đến file hình ảnh cũ
                    var oldImageFileName = existingMovie.ImageUrl;
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, "Images", oldImageFileName);
                    // Nếu file cũ tồn tại, xóa file cũ đi
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Lưu hình ảnh mới
                    movie.ImageUrl = await SaveImage(imageUrl, movie.Name);
                }
                if (movieUrl == null)
                {
                    movie.MovieUrl = existingMovie.MovieUrl;
                }
                else
                {
                    // Xác định đường dẫn đến file hình ảnh cũ
                    var oldImageFileName = existingMovie.ImageUrl;
                    var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, "videos", oldImageFileName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    movie.MovieUrl = await SaveMovie(movieUrl, movie.Name);
                }
                // Cập nhật các thông tin khác của sản phẩm
                existingMovie.Name = movie.Name;
                existingMovie.Description = movie.Description;
                existingMovie.Director = movie.Director;
                existingMovie.LinkInfo = movie.LinkInfo;
                existingMovie.Duration = movie.Duration;
                existingMovie.ReleaseYear = movie.ReleaseYear;
                existingMovie.view = movie.view;
                existingMovie.TrailerUrl = movie.TrailerUrl;
                existingMovie.GenreId = movie.GenreId;
                existingMovie.CountryId = movie.CountryId;
                existingMovie.ImageUrl = movie.ImageUrl;
                existingMovie.MovieUrl = movie.MovieUrl;
                await _movieRepository.UpdateAsync(existingMovie);
                return RedirectToAction(nameof(Index));
            }
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.Genres = new SelectList(genres, "Id", "Name");
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");
            return View(movie);
        }
        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _movieRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin của bộ phim từ cơ sở dữ liệu
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            // Lấy đường dẫn hình ảnh và video của bộ phim
            var imagePath = movie.ImageUrl;
            var videoPath = movie.MovieUrl;

            // Xóa bộ phim từ cơ sở dữ liệu
            await _movieRepository.DeleteAsync(id);

            // Xóa file hình ảnh và video từ thư mục wwwroot/images và wwwroot/movies
            if (!string.IsNullOrEmpty(imagePath))
            {
                var imagePathToDelete = Path.Combine(_hostingEnvironment.WebRootPath, "images", Path.GetFileName(imagePath));
                if (System.IO.File.Exists(imagePathToDelete))
                {
                    System.IO.File.Delete(imagePathToDelete);
                }

            }
            if (!string.IsNullOrEmpty(videoPath))
            {
                var videoPathToDelete = Path.Combine(_hostingEnvironment.WebRootPath, "videos", Path.GetFileName(videoPath));
                if (System.IO.File.Exists(videoPathToDelete))
                {
                    System.IO.File.Delete(videoPathToDelete);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
