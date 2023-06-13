namespace Carz.Dto
{
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }

        public string AccessToken { get; set; } = string.Empty;


        public string Email { get; set; }= string.Empty;

        public string UserId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;


    }
}
