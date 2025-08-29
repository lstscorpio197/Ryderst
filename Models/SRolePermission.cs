namespace ShopAdmin.Models;

public partial class SRolePermission
{
    public int Id { get; set; }

    public int? RoleId { get; set; }

    public int? UserId { get; set; }

    public int? PermissionId { get; set; }

    public int? IsGranted { get; set; }

    public virtual SPermission Permission { get; set; }

    public virtual SRole Role { get; set; }

    public virtual SUser User { get; set; }
}
