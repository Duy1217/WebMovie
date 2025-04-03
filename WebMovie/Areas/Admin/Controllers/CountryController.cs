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
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountryController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            var country = await _countryRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                country = country.Where(p => RemoveAccents(p.Name).ToUpper().Contains(RemoveAccents(searchString).ToUpper())).ToList();
            }
            const int pageSize = 8;
            var pagedCountry = country.ToPagedList(page, pageSize);
            ViewBag.SearchString = searchString;
            return View(pagedCountry);
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
        public async Task<IActionResult> Add(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.AddAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Country country)
        {
            if (id != country.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country != null)
            {
                await _countryRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
