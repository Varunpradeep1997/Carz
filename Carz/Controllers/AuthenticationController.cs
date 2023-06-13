using Carz.Dto;
using Carz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Carz.Controllers
{
    [ApiController]
    [Route("api/v1/authenticate")]

    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }




        [HttpPost]
        [Route("roles/add")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var appRole = new ApplicationRole { Name = request.Role};
            var createRole = await _roleManager.CreateAsync(appRole);
            return Ok(new {message="role created succesfully"});
        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await RegisterAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);
        }





        [HttpPost]
        [Route("login")]
        [ProducesResponseType((int) HttpStatusCode.OK,Type= typeof(LoginResponse))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await LoginAsync(request);

            return result.IsSuccess ? Ok(result) : BadRequest(result.Message);

        }









        private async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {

                var userExists = await _userManager.FindByEmailAsync(request.Email);
                if (userExists != null)
                {
                    return new RegisterResponse { Message = "User already exists", IsSuccess = false };

                }

                userExists = new ApplicationUser
                {
                    FullName = request.Email,
                    Email = request.Email,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    UserName = request.UserName,
                };

                var createUserResult = await _userManager.CreateAsync(userExists, request.Password);
                if (!createUserResult.Succeeded)
                {
                    return new RegisterResponse
                    {
                        Message = $"User creation failed{createUserResult?.Errors.First()?.Description}",
                        IsSuccess = false
                    };
                }

                    var addUserToRoleResult = await _userManager.AddToRoleAsync(userExists, "USER");

                    if (!addUserToRoleResult.Succeeded) return new RegisterResponse { Message = $"creation of user succeeded but could not add user to role{addUserToRoleResult?.Errors.First()?.Description}", IsSuccess = false };

                    return new RegisterResponse
                    {
                        IsSuccess = true,
                        Message = "User Registered successfully"
                    };



                
            }
            catch (Exception ex)
            {

                return new RegisterResponse { Message = ex.Message, IsSuccess = false };
            }
        }
















        private async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return new LoginResponse { Message = "Invalid email/Password", IsSuccess = false };

                var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())


            };

                var roles = await _userManager.GetRolesAsync(user);
                var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x));
                claims.AddRange(roleClaims);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("lsbjsdbwuhd7647hkjshd"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(30);

                var token = new JwtSecurityToken(
                    issuer: "https://localhost:5001",
                    audience: "https://localhost:5001",
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds


                    );

                return new LoginResponse
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Login Succesfull",
                    Email = user?.Email,
                    IsSuccess = true,
                    UserId = user.Id.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new LoginResponse { IsSuccess= false ,Message=ex.Message};

                
            }


        
        }


       
    }
}
