using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IUserRepository
    {
        Task AddAsync(User user);

        Task<User?> GetAsync(Guid id);

        Task<bool> AnyAsync(string email);

    }
}
