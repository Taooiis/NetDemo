using System.ComponentModel.DataAnnotations;

namespace MyService.Application.DTOs;

/// <summary>更新用户请求</summary>
public class UpdateUserRequest
{
    /// <summary>用户名（最长 50 字）</summary>
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>邮箱地址（最长 100 字）</summary>
    [Required]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
}
