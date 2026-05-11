using Application.Interface.Repository;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistance.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
         await _context.Users.AddAsync(user);
        }

        public async Task<bool> AnyAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetAsync(Guid id)
        {
          return await _context.Users.FirstOrDefaultAsync(x => x.Id == id); 

        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
