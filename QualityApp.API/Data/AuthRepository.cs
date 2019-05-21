using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QualityApp.API.Models;

namespace QualityApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> EmployeeExistsUserName(string userName)
        {
            var result = await _context.Employees.AnyAsync(x=>x.UserName == userName);
            if(result)return true;
            return false;
        }

        public async Task<bool> EmployeeExistsEmail(string email)
        {
            var result = await _context.Employees.AnyAsync(x=>x.Email == email);
            if(result)return true;
            return false;
        }

        public async Task<Employee> Login(string userName, string password)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x=>x.UserName == userName);
            if(employee==null)return null;
            if(!VerifyPasswordHash(password,employee.PasswordSalt,employee.PasswordHash))
            return null;
            return employee;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                
                var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputedHash.Length; i++)
                {
                    if(ComputedHash[i]!=passwordHash[i]){
                        return false;
                    }
                }
                return true;
            }
        }

        public async Task<Employee> Register(Employee employee, string password)
        {
            byte[] passwordHash,passwordSalt;
            createPasswordHash(password,out passwordHash,out passwordSalt);
            employee.PasswordSalt = passwordSalt;
            employee.PasswordHash = passwordHash;
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}