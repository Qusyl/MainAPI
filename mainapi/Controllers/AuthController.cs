using Application.Dto;
using Application.Interface;
using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Events.Auth;
using Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MainAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IHandler<RegisterUserEvent> _registerHandler;
        private readonly IHandler<LoginUserEvent> _loginHandler;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController( ITokenService tokenService, IUserRepository userRepository, IHandler<RegisterUserEvent> regHandler, IHandler<LoginUserEvent> logingHandler)
        {
            _registerHandler = regHandler;
            _loginHandler = logingHandler;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }


        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserRegisterDto request)
        {
            var existingUser = await _userRepository.AnyAsync(request.Email);
            if(existingUser)
            {
                return BadRequest(new
                {
                    error = "Email already exists"
                });
            }
            var regAttempt = await _registerHandler.HandleAsync(new RegisterUserEvent(request.Email, request.Password));
            if (!regAttempt.IsSuccess)
            {
                return BadRequest(regAttempt.Error);
            }
            var user = await _userRepository.GetAsync(regAttempt.Value);
            if (user == null) {
                return StatusCode(500, new {
                   Error =  "Registration Error"
                });
            }
            var token = _tokenService.GenerateToken(user);
            return StatusCode(201,new AuthResponseDto(token, DateTime.UtcNow.AddHours(1)));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto request)
        {
            var logAttempt = await _loginHandler.HandleAsync(new LoginUserEvent(request.Email, request.Password));
            if (!logAttempt.IsSuccess)
            {
                return BadRequest(logAttempt.Error);
            }
            var user = await _userRepository.GetAsync(logAttempt.Value);
            if (user == null) {
                return BadRequest(new
                {
                    error = logAttempt.Error.ToString()
                });
            }
                var token =  _tokenService.GenerateToken(user);

            return Ok(new AuthResponseDto(token, DateTime.UtcNow.AddHours(1)));
            }
        }
    }

