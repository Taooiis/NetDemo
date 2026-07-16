using System.ComponentModel.DataAnnotations;

namespace MyService.Application.DTOs;

/// <summary>创建用户请求</summary>
public class CreateUserRequest
{
    /// <summary>用户名（最长 50 字）</summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>邮箱地址（最长 100 字）</summary>
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    /// <summary>密码</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
