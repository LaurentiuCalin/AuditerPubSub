using AuditingPubApi.Domain.Exceptions;
using AuditingPubApi.Domain.Services;
using AuditingPubApi.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuditingPubApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpPost(Name = nameof(CreateUser))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody, BindRequired] CreateUserRequest createUserRequest)
    {
        try
        {
            var id = await _userService.Create(createUserRequest);
            return CreatedAtAction(nameof(CreateUser), id);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
    }
}