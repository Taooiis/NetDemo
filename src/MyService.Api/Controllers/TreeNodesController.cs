using Microsoft.AspNetCore.Mvc;
using MyService.Application.DTOs;
using MyService.Application.Interfaces;

namespace MyService.Api.Controllers;

/// <summary>树节点控制器，提供六层树形结构的 CRUD 与移动操作</summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class TreeNodesController : ControllerBase
{
    private readonly ITreeNodeService _treeNodeService;

    public TreeNodesController(ITreeNodeService treeNodeService)
    {
        _treeNodeService = treeNodeService;
    }

    /// <summary>获取整棵树（已组装层级嵌套结构）</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetTree()
    {
        var tree = await _treeNodeService.GetTreeAsync();
        return Ok(tree);
    }

    /// <summary>根据 ID 获取单个节点</summary>
    /// <param name="id">节点 GUID</param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TreeNodeDto>> GetById(Guid id)
    {
        var node = await _treeNodeService.GetByIdAsync(id);
        return node is null ? NotFound() : Ok(node);
    }

    /// <summary>获取指定父节点的直接子节点</summary>
    /// <param name="parentId">父节点 GUID</param>
    [HttpGet("{parentId:guid}")]
    public async Task<ActionResult<IEnumerable<TreeNodeDto>>> GetChildren(Guid parentId)
    {
        var children = await _treeNodeService.GetChildrenAsync(parentId);
        return Ok(children);
    }

    /// <summary>创建新节点</summary>
    /// <param name="request">节点创建信息</param>
    [HttpPost]
    public async Task<ActionResult<TreeNodeDto>> Create([FromBody] CreateTreeNodeRequest request)
    {
        var node = await _treeNodeService.CreateAsync(request);
        if (node is null) return BadRequest("父节点不存在或层级超过限制");
        return CreatedAtAction(nameof(GetById), new { id = node.Id }, node);
    }

    /// <summary>更新节点基本信息</summary>
    /// <param name="id">节点 GUID</param>
    /// <param name="request">节点更新信息</param>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TreeNodeDto>> Update(Guid id, [FromBody] UpdateTreeNodeRequest request)
    {
        var node = await _treeNodeService.UpdateAsync(id, request);
        return node is null ? NotFound() : Ok(node);
    }

    /// <summary>移动节点（更换父节点或调整排序）</summary>
    /// <param name="id">节点 GUID</param>
    /// <param name="request">移动目标信息</param>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TreeNodeDto>> Move(Guid id, [FromBody] MoveTreeNodeRequest request)
    {
        var node = await _treeNodeService.MoveAsync(id, request);
        return node is null ? NotFound() : Ok(node);
    }

    /// <summary>删除节点及所有子节点</summary>
    /// <param name="id">节点 GUID</param>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _treeNodeService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
