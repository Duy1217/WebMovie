using Microsoft.AspNetCore.Mvc;
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
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            var genre = await _genreRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                genre = genre.Where(p => RemoveAccents(p.Name).ToUpper().Contains(RemoveAccents(searchString).ToUpper())).ToList();
            }
            const int pageSize = 8;
            var pagedGenres = genre.ToPagedList(page, pageSize);
            ViewBag.SearchString = searchString;
            return View(pagedGenres);
        }
        private string RemoveAccents(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{M}");
            return regex.Replace(normalizedString, string.Empty);
        }

        public IActionResult Add(int id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Genre genre)
        {
            if (ModelState.IsValid)
            {
                await _genreRepository.AddAsync(genre);
                return RedirectToAction(nameof(Index));
            }

            return View(genre);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Genre genre)
        {
            if (id != genre.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _genreRepository.UpdateAsync(genre);
                return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre != null)
            {
                await _genreRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
