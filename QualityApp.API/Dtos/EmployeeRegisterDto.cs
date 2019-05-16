using System.ComponentModel.DataAnnotations;

namespace QualityApp.API.Dtos
{
    public class EmployeeRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string FatherName { get; set; }
        [Required]
        public string LastName { get; set; }
        [StringLength(8,MinimumLength=6,ErrorMessage="يجب أن لا تقل كلمة المرور عن ستة أحرف ولا ولا تزيد عن ثمانية")]
        public string Password { get; set; }
    }
}