using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ssd_authorization_solution.Models;

namespace ssd_authorization_solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;

        public LoginController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            JwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestData loginRequest)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            // Validate password
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            // Retrieve the user's roles
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var sessionData = new SessionData(user.UserName, user.Id, role);

            var token = _jwtTokenService.GenerateJwtToken(sessionData);

            // Return the token
            return Ok(new { token });
        }

        public class LoginRequestData
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
