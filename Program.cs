using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using ShopAdmin.Common;
using ShopAdmin.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Cấu hình DbContext với chuỗi kết nối
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        // Giữ nguyên chữ hoa đầu cho các thuộc tính (không chuyển thành camelCase)
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Thời gian session hết hạn
    options.Cookie.HttpOnly = true; // Đảm bảo cookie chỉ được truy cập qua HTTP, không thể truy cập qua JavaScript
    options.Cookie.IsEssential = true; // Cookie là bắt buộc
});


var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Lấy dữ liệu từ DB và tạo route động
using (var scope = app.Services.CreateScope())
{
    var lstMenu = AppMenu.ListMenu.Where(x => x.Type == 1).ToList();
    var endpointRouteBuilder = app.MapGroup("/");
    foreach (var menu in lstMenu)
    {
        endpointRouteBuilder.MapControllerRoute(
            name: menu.Controller,
            pattern: $"/Admin/{StringConverter.ConvertToSlug(menu.ControllerName)}",
            defaults: new { controller = menu.Controller, action = "Index", area = "Admin" });
    }

    //var dbContext = scope.ServiceProvider.GetRequiredService<ShopDbContext>();
    //var routes = await dbContext.SUsers.ToListAsync();

    //var endpointRouteBuilder = app.MapGroup("/");

    //foreach (var route in routes)
    //{
    //    endpointRouteBuilder.MapControllerRoute(
    //        name: route.Username,
    //        pattern: route.Username,
    //        defaults: new { controller = "SUser", action = "Index" });
    //}
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "User" } // chỉ định area mặc định là Shop
);

app.MapControllerRoute(
    name: "admin-login",
    pattern: "admin/dang-nhap",
    defaults: new { controller = "Login", action = "Index", area = "Admin" }
);
app.MapControllerRoute(
    name: "admin-login",
    pattern: "admin/dang-xuat",
    defaults: new { controller = "Login", action = "Logout", area = "Admin" }
);

app.MapControllerRoute(
    name: "view-category",
    pattern: "{name}-pc{id}",
    defaults: new { controller = "Categories", action = "Index", area = "User" }
);
app.MapControllerRoute(
    name: "view-category",
    pattern: "{name}-p{id}",
    defaults: new { controller = "Product", action = "Index", area = "User" }
);
app.MapControllerRoute(
    name: "tin-tuc",
    pattern: "tin-tuc",
    defaults: new { controller = "News", action = "Index", area = "User" }
);
app.MapControllerRoute(
    name: "bo-suu-tap",
    pattern: "bo-suu-tap",
    defaults: new { controller = "Collection", action = "Index", area = "User" }
);
app.MapControllerRoute(
    name: "tin-tuc-chi-tiet",
    pattern: "tin-tuc/{title}-n{id}",
    defaults: new { controller = "News", action = "ViewDetail", area = "User" }
);
app.MapControllerRoute(
    name: "bo-suu-tap-chi-tiet",
    pattern: "bo-suu-tap/{slug}-cl{id}",
    defaults: new { controller = "Collection", action = "ViewDetail", area = "User" }
);
app.MapControllerRoute(
    name: "chinh-sach-bao-mat",
    pattern: "chinh-sach-bao-mat",
    defaults: new { controller = "About", action = "ChinhSachBaoMat", area = "User" }
);
app.MapControllerRoute(
    name: "chinh-sach-van-chuyen",
    pattern: "chinh-sach-van-chuyen",
    defaults: new { controller = "About", action = "ChinhSachVanChuyen", area = "User" }
);
app.MapControllerRoute(
    name: "chinh-sach-doi-tra",
    pattern: "chinh-sach-doi-tra",
    defaults: new { controller = "About", action = "ChinhSachDoiTra", area = "User" }
);
app.MapControllerRoute(
    name: "hinh-thuc-thanh-toan",
    pattern: "hinh-thuc-thanh-toan",
    defaults: new { controller = "About", action = "HinhThucThanhToan", area = "User" }
);

app.Run();
