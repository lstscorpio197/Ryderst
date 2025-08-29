namespace ShopAdmin.Models;

public partial class SUserRole
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int UserId { get; set; }

    public virtual SRole Role { get; set; }

    public virtual SUser User { get; set; }
}
