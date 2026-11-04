using Microsoft.EntityFrameworkCore;
using Hotel.Web.Models;
using Hotel.Web.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext con la cadena de conexión
builder.Services.AddDbContext<HotelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelConnection")));

// Registrar Repositorios
builder.Services.AddScoped<IHabitacionRepositorio, HabitacionRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();

// Agregar MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    pattern: "{controller=Habitaciones}/{action=Index}/{id?}");

app.Run();