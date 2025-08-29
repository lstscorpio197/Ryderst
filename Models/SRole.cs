namespace ShopAdmin.Models;

public partial class SRole
{
    public int Id { get; set; }

    public string Ma { get; set; }

    public string Ten { get; set; }

    public string GhiChu { get; set; }

    public int? Enable { get; set; }

    public virtual ICollection<SRolePermission> SRolePermissions { get; set; } = new List<SRolePermission>();

    public virtual ICollection<SUserRole> SUserRoles { get; set; } = new List<SUserRole>();
}
