using System.ComponentModel.DataAnnotations;

namespace Carz.Dto
{
    public class RegisterRequest
    {
        [Required,EmailAddress]
        public string  Email { get; set; }=string.Empty;

        public string UserName { get; set; } = string.Empty;
        [Required]
        public string FullNmae { get; set; } = string.Empty;

        [Required,DataType(DataType.Password)]
        public string Password{ get; set; }=string.Empty;

        [Required, DataType(DataType.Password),Compare(nameof(Password),ErrorMessage ="Password do not match ")]
        public string PasswordConfirmed { get; set;} = string.Empty;  
    }
}
