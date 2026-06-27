using Kong_Gateway.ConsoleApp.Data;
using Kong_Gateway.ConsoleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace Kong_Gateway.ConsoleApp.Service
{
    public class BookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task AddAsync(string title, string author, string genre)
        {
            var newBook = new Book { Title = title, Author = author, Genre = genre };
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
        }
    }
}
