
using System.Security.Claims;
using Authentication.Lab.Entity;
using Authentication.Lab.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Lab.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]  
public class UsersController : ControllerBase
{
	private readonly IJWTManagerRepository _jWTManager;
    private readonly JWTTokenRepository jWTTokenRepository;

	public UsersController(
        IJWTManagerRepository jWTManager,
        JWTTokenRepository jWTTokenRepository
        )
	{
        this.jWTTokenRepository = jWTTokenRepository;
		this._jWTManager = jWTManager;
	}

	[HttpGet]
	public List<string> Get()
	{
        var identity = HttpContext.User.Claims;
        var claims = identity.FirstOrDefault(x => x.Type == "id");
		var users = new List<string>
		{
			"Satinder Singh",
			"Amit Sarna",
			"Davin Jon"
		};

		return users;
	}

	[AllowAnonymous]
	[HttpPost]
	[Route("authenticate")]
	public IActionResult Authenticate(Users usersdata)
	{
		// var token = _jWTManager.Authenticate(usersdata);
        string token = this.jWTTokenRepository.GenerateToken(usersdata);

		if (token == null)
		{
			return Unauthorized();
		}

		return Ok(token);
	}
}