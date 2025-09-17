namespace ShopAdmin.Common
{
    public static class AppMenu
    {
        public static List<Menu> ListMenu = new List<Menu>
        {
            new Menu(1,null,"QT","Quản trị",0,"fa fa-cogs"),
            new Menu(0,1,"SUser","Quản lý tài khoản"),
            new Menu(0,1,"SRole","Quản lý nhóm quyền"),
            new Menu(0,1,"SPermission","Quản lý quyền"),
            new Menu(2,null,"DM","Danh mục",0, "fa fa-list"),
            new Menu(0,2,"Category","Danh mục sản phẩm"),
            new Menu(0,2,"SNews","Danh mục tin tức"),
            new Menu(0,2,"Collection","Bộ sưu tập"),
            new Menu(3,null,"Product","Quản lý sản phẩm",1, "fas fa-tshirt"),
            new Menu(4,null,"Order","Quản lý đơn hàng",1, "fa fa-shopping-cart"),
        };

        public static List<Action> ListActionDefault = new List<Action>
        {
            new Action("view","Xem"),
            new Action("create","Thêm"),
            new Action("update","Sửa"),
            new Action("delete","Xóa"),
            new Action("import","Import"),
            new Action("export","Export"),
        };
    }

    public class Menu
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Controller { get; set; }
        public string ControllerName { get; set; }

        /// <summary>
        /// Loại menu:0-Menu cha; 1-Menu
        /// </summary>
        public int Type { get; set; }

        public string Icon { get; set; }

        public Menu(int id, int? parentId, string controller, string name, int type = 1, string icon = "fas fa-tags")
        {
            Id = id;
            ParentId = parentId;
            Controller = controller;
            ControllerName = name;
            Type = type;
            Icon = icon;
        }
    }

    public class Action
    {
        public string TypeHandle { get; set; }
        public string Name { get; set; }
        public Action(string typeHandle, string name)
        {
            TypeHandle = typeHandle;
            Name = name;
        }
    }
}