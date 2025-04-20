using MediatR;
using Microsoft.AspNetCore.Mvc;
using ZakupekApi.Wrapper.Contract.Commands.Auth;

namespace ZakupekApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="command">Registration data</param>
    /// <returns>User information with authentication tokens</returns>
    /// <response code="201">User successfully created</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">Email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mediator.Send(command);

        return CreatedAtAction(nameof(Register), result);
    }

    /// <summary>
    /// Authenticates a user and returns access tokens
    /// </summary>
    /// <param name="command">Login credentials</param>
    /// <returns>Authentication tokens and user information</returns>
    /// <response code="200">Authentication successful</response>
    /// <response code="400">Invalid login data</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await mediator.Send(command);
        
        return Ok(result);
    }
}
