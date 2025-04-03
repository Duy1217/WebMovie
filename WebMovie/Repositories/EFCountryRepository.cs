using Microsoft.EntityFrameworkCore;
using WebMovie.Models;

namespace WebMovie.Repositories
{
    public class EFCountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCountryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries
            .Include(p => p.Movies) // Include thông tin về category
            .ToListAsync();
        }
        public async Task<Country> GetByIdAsync(int id)
        {
            // return await _context.Genres.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Countries.Include(p =>
            p.Movies).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Country country)
        {
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var Country = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(Country);
            await _context.SaveChangesAsync();
        }
    }
}
