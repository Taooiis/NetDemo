using Microsoft.AspNetCore.Mvc;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;

namespace MyService.Api.Controllers;

/// <summary>用户控制器，提供用户信息的 CRUD 操作</summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>获取所有用户列表</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>根据 ID 获取单个用户</summary>
    /// <param name="id">用户 GUID</param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>创建新用户</summary>
    /// <param name="request">用户创建信息</param>
    /// <returns>创建成功的用户信息</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);
        if (user is null) return Conflict();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>更新用户信息</summary>
    /// <param name="id">用户 GUID</param>
    /// <param name="request">用户更新信息</param>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userService.UpdateAsync(id, request);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>删除用户</summary>
    /// <param name="id">用户 GUID</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}