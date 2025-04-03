using Microsoft.EntityFrameworkCore;
using WebMovie.Models;

namespace WebMovie.Repositories
{
    public class EFGenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;
        public EFGenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            // return await _context.Genres.ToListAsync();
            return await _context.Genres
            .Include(p => p.Movies) // Include thông tin về category
            .ToListAsync();
        }
        public async Task<Genre> GetByIdAsync(int id)
        {
            // return await _context.Genres.FindAsync(id);
            // lấy thông tin kèm theo category
            return await _context.Genres.Include(p =>
            p.Movies).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Genre genre)
        {
            _context.Genres.Add(genre);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var Genre = await _context.Genres.FindAsync(id);
            _context.Genres.Remove(Genre);
            await _context.SaveChangesAsync();
        }
    }
}
