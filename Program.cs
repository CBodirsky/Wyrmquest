using Wyrmquest.Services;

namespace Wyrmquest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSession();

            builder.Services.AddSingleton<EnemyRepository>(provider =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "Data", "Enemies.json");
                return new EnemyRepository(path);
            });

            builder.Services.AddSingleton<EnemySpawnResolver>();
            builder.Services.AddSingleton<MapService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            //Security purposes. Reapply when closer to active
            //app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
