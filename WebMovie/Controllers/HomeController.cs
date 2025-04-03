using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using WebMovie.Models;
using WebMovie.Repositories;
using X.PagedList;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _dbContext;
        public HomeController(IMovieRepository movieRepository,
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
        public async Task<IActionResult> Index(string searchString)
        {
            var movies = await _movieRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(p => RemoveAccents(p.Name).ToUpper().Contains(RemoveAccents(searchString).ToUpper())
                    || p.Description.ToUpper().Contains(searchString.ToUpper())
                    || RemoveAccents(p.Director).ToUpper().Contains(RemoveAccents(searchString).ToUpper())).ToList();
            }
            ViewBag.SearchString = searchString;

            return View(movies);
        }
        private string RemoveAccents(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{M}");
            return regex.Replace(normalizedString, string.Empty);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
