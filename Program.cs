using Microsoft.AspNetCore.Localization;
using RamiloAlonsoSaraTarea4.Models;
using RamiloAlonsoSaraTarea4.Models.Repository;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton(new Conexion(builder.Configuration.GetConnectionString("ConexionTarea4")));
builder.Services.AddScoped<IPokemonRepository,PokemonRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
//establece las comas como separador decimal, de forma que en el formulario no se transformen en números enteros al esperar un punto
app.Use(async (context, next) =>
{
    var currentThreadCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
    currentThreadCulture.NumberFormat = NumberFormatInfo.InvariantInfo;

    Thread.CurrentThread.CurrentCulture = currentThreadCulture;
    Thread.CurrentThread.CurrentUICulture = currentThreadCulture;

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "filtrar",
    pattern: "{controller=Pokemon}/{action=FiltrarPokemonPorTipoPesoAltura}/{idTipo}/{peso}/{altura}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
