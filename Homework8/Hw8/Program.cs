using System.Diagnostics.CodeAnalysis;
using Hw8.Calculator;
using Hw8.Implementation;
using Hw8.Interfaces;

namespace Hw8;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton<ICalculator, CalculatorImpl>();
        builder.Services.AddSingleton<IParser, ParserImpl>();

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
            pattern: "{controller=Calculator}/{action=Index}");

        app.Run();
    }
}