using WebMovie.Models;
using WebMovie.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics.Metrics;
using X.PagedList;
namespace WebMovie.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public MovieController(IMovieRepository movieRepository,
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
        public async Task<IActionResult> Index(int page =1)
        {
            if (page <= 0)
            {
                page = 1;
            }
            // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;

            // Lấy danh sách phim từ repository và lọc theo releaseYear
            var pageMovie = await _movieRepository.GetAllAsync();
            pageMovie = pageMovie.Where(m => m.ReleaseYear <= currentYear);
            const int pageSize = 8;
            var pagedMovie = pageMovie.ToPagedList(page, pageSize);
            return View(pagedMovie);
        }
        public async Task<IActionResult> search(string searchString, int page=1)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var movies = await _movieRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(p => RemoveAccents(p.Name).ToUpper().Contains(RemoveAccents(searchString).ToUpper())
                    || p.Description.ToUpper().Contains(searchString.ToUpper())
                    || RemoveAccents(p.Director).ToUpper().Contains(RemoveAccents(searchString).ToUpper())).ToList();
            }
            ViewBag.SearchString = searchString;
            const int pageSize = 8;
            var pagedMovie = movies.ToPagedList(page, pageSize);
            return View(pagedMovie);
        }
        public async Task<IActionResult> FilterGenre(int? selectedGenre, int page = 1) {
            if (page <= 0)
            {
                page = 1;
            }
            var movies = await _movieRepository.GetAllAsync();
            if (selectedGenre.HasValue && selectedGenre != 0)
            {
                movies = movies.Where(m => m.GenreId == selectedGenre).ToList();
            }
            var genres = await _genreRepository.GetAllAsync();
            ViewBag.SelectedGenre = new SelectList(genres, "Id", "Name", selectedGenre);
            ViewBag.Genres = selectedGenre;
            const int pageSize = 8;
            var pagedMovie = movies.ToPagedList(page, pageSize);
            return View(pagedMovie);
        }
        public async Task<IActionResult> FilterCountry(int? selectedCountry, int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }
            var movies = await _movieRepository.GetAllAsync();
            if (selectedCountry.HasValue && selectedCountry != 0)
            {
                movies = movies.Where(m => m.CountryId == selectedCountry).ToList();
            }
            var countries = await _countryRepository.GetAllAsync();
            ViewBag.SelectedCountry = new SelectList(countries, "Id", "Name", selectedCountry);

            ViewBag.Countries = selectedCountry;
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

        public async Task<IActionResult> Detail(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            var current = DateTime.Now.Year;
            if(movie.ReleaseYear > current)
            {
                return View("DetailSCC", movie);
            }
            return View(movie);
        }

        public async Task<IActionResult> GetMovieUrl(int id)
        {
            // Gọi phương thức trong repository của bạn để lấy thông tin của phim từ ID
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            movie.view++;
            await _movieRepository.UpdateAsync(movie);
            // Trả về URL của video
            return Json(movie.MovieUrl);
        }

    }
}
