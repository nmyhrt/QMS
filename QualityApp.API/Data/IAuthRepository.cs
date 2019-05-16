using System.Threading.Tasks;
using QualityApp.API.Models;

namespace QualityApp.API.Data
{
    public interface IAuthRepository
    {
         Task<Employee> Register (Employee employee, string password);
         Task<Employee> Login (string userName, string password);
         Task<bool> EmployeeExists (string userName);
    }
}