using Microsoft.OpenApi.Models;
using AklujEatsAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Register DbContext for MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))
    )
);

// ✅ 2. Add MVC support (Controllers + Views)
builder.Services.AddControllersWithViews();

// ✅ 3. Add Swagger for API docs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AklujEats API",
        Version = "v1",
        Description = "API for AklujEats food delivery platform"
    });
});

// ✅ 4. Add Session support
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Auto-expire after 30 min
    options.Cookie.HttpOnly = true;                  // More secure: JS can't access
    options.Cookie.IsEssential = true;               // Required for GDPR
});

var app = builder.Build();

// ✅ 5. Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AklujEats API V1");
    c.RoutePrefix = "swagger";
});

// ✅ 6. Essential middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ 7. Enable Session support before routing
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// ✅ 8. Default MVC route: Admin login first
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}/{id?}"
);

// ✅ 9. Map API controllers (attribute-based routing, like [Route("api/[controller]")])
app.MapControllers();

// ✅ 10. Let the show begin!
app.Run();
