namespace MyService.Application.DTOs;

/// <summary>用户信息</summary>
public class UserDto
{
    /// <summary>用户 ID</summary>
    public Guid Id { get; set; }

    /// <summary>用户名</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>邮箱地址</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>最后更新时间</summary>
    public DateTime UpdatedAt { get; set; }
}