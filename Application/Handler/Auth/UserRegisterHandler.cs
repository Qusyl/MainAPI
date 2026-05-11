using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain;
using Domain.Entity;
using Domain.Events.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handler.Auth
{
    public class UserRegisterHandler : IHandler<RegisterUserEvent>
    {
        private readonly IUserRepository _userRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IPasswordHasherService _passwordHasher;

        public UserRegisterHandler(IUserRepository userRepository, IPasswordHasherService passwordHasher, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid, ApplicationError>> HandleAsync(RegisterUserEvent @event, CancellationToken cts = default)
        {

            var newUser = User.Create(@event.Email, _passwordHasher.HashPassword(@event.Password));

            if (!newUser.IsSuccess)
            {
                return Result<Guid, ApplicationError>.Failure(ApplicationError.EntityError);
            }
            await _userRepository.AddAsync(newUser.Value);

            var save = await _unitOfWork.SaveChangesAsync(cts);

            if (!save.IsSuccess)
            {
                return Result<Guid, ApplicationError>.Failure(ApplicationError.ConcurrencyError);
            }

            return Result<Guid, ApplicationError>.Success(newUser.Value.Id);
        }
    }
}
