using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ssd_authorization_solution.Models;

namespace ssd_authorization_solution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
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

            // Retrieve the permissions associated with the user's roles
            var roleClaims = await _userManager.GetClaimsAsync(user);
            var permissions = roleClaims
                .Where(rc => rc.Type == "Permission")
                .Select(rc => rc.Value)
                .ToList();

            var sessionData = new SessionData(user.UserName, role, permissions);

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
