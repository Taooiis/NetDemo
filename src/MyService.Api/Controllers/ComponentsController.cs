using Microsoft.AspNetCore.Mvc;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;

namespace MyService.Api.Controllers;

/// <summary>构件控制器，提供建筑构件信息的 CRUD 操作</summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class ComponentsController : ControllerBase
{
    private readonly IComponentService _componentService;

    public ComponentsController(IComponentService componentService)
    {
        _componentService = componentService;
    }

    /// <summary>获取所有构件列表</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComponentDto>>> GetAll()
    {
        var list = await _componentService.GetAllAsync();
        return Ok(list);
    }

    /// <summary>模糊搜索构件（按关键字匹配所有字符串字段）</summary>
    /// <param name="request">搜索条件</param>
    [HttpPost]
    public async Task<ActionResult<IEnumerable<ComponentDto>>> Search([FromBody] ComponentSearchRequest request)
    {
        var list = await _componentService.SearchAsync(request);
        return Ok(list);
    }

    /// <summary>根据 ID 获取单个构件</summary>
    /// <param name="id">构件 ID</param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ComponentDto>> GetById(Guid id)
    {
        var component = await _componentService.GetByIdAsync(id);
        return component is null ? NotFound() : Ok(component);
    }

    /// <summary>创建新构件</summary>
    /// <param name="request">构件创建信息</param>
    [HttpPost]
    public async Task<ActionResult<ComponentDto>> Create([FromBody] CreateComponentRequest request)
    {
        var component = await _componentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = component!.Id }, component);
    }

    /// <summary>批量创建构件（每批最多 1000 条）</summary>
    /// <param name="requests">构件列表</param>
    [HttpPost]
    public async Task<ActionResult<int>> CreateRange([FromBody] List<CreateComponentRequest> requests)
    {
        var count = await _componentService.CreateRangeAsync(requests);
        return Ok(count);
    }

    /// <summary>更新构件信息</summary>
    /// <param name="id">构件 ID</param>
    /// <param name="request">构件更新信息</param>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ComponentDto>> Update(Guid id, [FromBody] UpdateComponentRequest request)
    {
        var component = await _componentService.UpdateAsync(id, request);
        return component is null ? NotFound() : Ok(component);
    }

    /// <summary>删除构件</summary>
    /// <param name="id">构件 ID</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _componentService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
