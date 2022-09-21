using _mpesaIntegration.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// http client
builder.Services.AddHttpClient("mpesa", c =>
 {
     c.BaseAddress = new Uri("https://sandbox.safaricom.co.ke");
     //  c.DefaultRequestHeaders.Add("Content-Type", "application/json");
 }
);


var serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
// database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options
.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion)
);






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
