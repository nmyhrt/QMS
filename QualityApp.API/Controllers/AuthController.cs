using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QualityApp.API.Data;
using QualityApp.API.Dtos;
using QualityApp.API.Models;

namespace QualityApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(EmployeeRegisterDto employeeRegisterDto)
        {
            employeeRegisterDto.UserName = employeeRegisterDto.UserName.ToLower();
            employeeRegisterDto.Email = employeeRegisterDto.Email.ToLower();

            if (await _repo.EmployeeExistsUserName(employeeRegisterDto.UserName))
                return BadRequest("اسم المستخدم مستجل مسبقا");
            if (await _repo.EmployeeExistsEmail(employeeRegisterDto.Email))
                return BadRequest("البريد الالكتروني مستجل مسبقا");
            var employeeToCreate = new Employee
            {
                UserName = employeeRegisterDto.UserName,
                Email = employeeRegisterDto.Email,
                FirstName = employeeRegisterDto.FirstName,
                FatherName = employeeRegisterDto.FatherName,
                LastName = employeeRegisterDto.LastName,
            };

            var CreatedEmployee = await _repo.Register(employeeToCreate, employeeRegisterDto.Password);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(EmployeeForLoginDto employeeForLoginDto)
        {
            var employeeFromRepo = await _repo.Login(employeeForLoginDto.userName.ToLower(), employeeForLoginDto.password);
            if (employeeFromRepo == null) return Unauthorized();
            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier,employeeFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,employeeFromRepo.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials= creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new{
                token=tokenHandler.WriteToken(token)
            });
        }
    }
}