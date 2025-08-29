namespace ShopAdmin.Models;

public partial class SPermission
{
    public int Id { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public string ControllerName { get; set; }

    public string ActionName { get; set; }

    public int? Enable { get; set; }

    public virtual ICollection<SRolePermission> SRolePermissions { get; set; } = new List<SRolePermission>();
}
