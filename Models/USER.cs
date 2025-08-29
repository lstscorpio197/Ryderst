using Microsoft.EntityFrameworkCore;
using ShopAdmin.Common;

namespace ShopAdmin.Models
{
    public class USER
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public Nullable<int> ChucVu { get; set; }

        public string ChucVuText { get; set; }

        public List<SPermission> LstPermission { get; set; }

        public USER()
        {
            LstPermission = new List<SPermission>();
        }
        public USER(SUser us)
        {
            Id = us.Id;
            Username = us.Username;
            HoTen = us.HoTen;
            Email = us.Email;
            ChucVu = us.ChucVu;

            ChucVuText = LstChucVu.Init.FirstOrDefault(x => x.Id == ChucVu)?.Name;
            LstPermission = GetListPermission(us.Id);
        }

        private List<SPermission> GetListPermission(int id)
        {
            using (var db = new ShopDbContext())
            {
                try
                {
                    List<int> lstRole = db.SUserRoles.AsNoTracking().Where(x => x.UserId == id).Select(x => x.RoleId).ToList();
                    List<int> lstPermissionForRole = db.SRolePermissions.AsNoTracking().Where(x => x.IsGranted == 1 && lstRole.Contains(x.RoleId.Value)).Select(x => x.PermissionId.Value).ToList();
                    List<int> lstPermissionNotGrant = db.SRolePermissions.AsNoTracking().Where(x => x.IsGranted == 0 && x.UserId == id).Select(x => x.PermissionId.Value).ToList();

                    List<int> lstPermissionForUser = lstPermissionForRole.Except(lstPermissionNotGrant).ToList();
                    var result = db.SPermissions.AsNoTracking().Where(x => lstPermissionForUser.Contains(x.Id)).ToList();
                    result.ForEach(x => x.SRolePermissions = null);
                    return result;
                }
                catch (Exception ex)
                {
                    return new List<SPermission>();
                }
            }
        }
    }
}