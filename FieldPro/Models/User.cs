using System.ComponentModel.DataAnnotations;

namespace FieldPro.Models;

public class User
{
    public int Id { get; set; }

    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(64)]
    public string PasswordSalt { get; set; } = string.Empty;

    [MaxLength(128)]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
