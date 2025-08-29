namespace ShopAdmin.Models;

public partial class SUser
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string HoTen { get; set; }

    public string Email { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string GioiTinh { get; set; }

    public int? ChucVu { get; set; }

    public int? IsActived { get; set; }

    public string SDT { get; set; }

    public int? NhanEmail { get; set; }

    public virtual ICollection<SRolePermission> SRolePermissions { get; set; } = new List<SRolePermission>();

    public virtual ICollection<SUserRole> SUserRoles { get; set; } = new List<SUserRole>();
}
