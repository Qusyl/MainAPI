using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain;
using Domain.Events.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Auth
{
    public class UserLoginHandler : IHandler<LoginUserEvent>
    {
        private readonly IUserRepository _userRepository;

        private readonly IPasswordHasherService _passwordHasher;

        public UserLoginHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<Guid, ApplicationError>> HandleAsync(LoginUserEvent @event, CancellationToken cts = default)
        {
            var user = await _userRepository.GetAsync(@event.UserId);
            if (user == null){
                return Result<Guid, ApplicationError>.Failure(ApplicationError.EntityError);
            }

            if (!_passwordHasher.VerifyPassword(@event.Password, user.PasswordHash)) {
                return Result<Guid, ApplicationError>.Failure(ApplicationError.InvalidPassword);
            }

            return Result<Guid, ApplicationError>.Success(user.Id);
        }
    }
}
