using elFinder.Net.AspNetCore.Extensions;
using elFinder.Net.Core.Extensions;
using elFinder.Net.Core.Plugins;
using elFinder.Net.Core.Services.Drawing;
using elFinder.Net.Drivers.FileSystem.Helpers;
using elFinder.Net.Drivers.FileSystem.Services;
using elFinder.Net.Plugins.FileSystemQuotaManagement.Extensions;
using ElFinderNet7.Models;
using ElFinderNet7.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SixLabors.ImageSharp;

namespace ElFinderNet7;

public class Program
{
    public const string StorageFolder = ".storage";
    public const string TempFileFolder = ".temp";

    public static string WebRootPath { get; private set; }
    public static string StoragePath { get; private set; }
    public static string TempPath { get; private set; }

    public static string MapStoragePath(string path)
    {
        path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
        return PathHelper.GetFullPath(PathHelper.SafelyCombine(StoragePath, StoragePath, path));
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        var pluginCollection = new PluginCollection();

        builder.Services.AddElFinderAspNetCore()
             //.AddFileSystemDriver(tempFileCleanerConfig: (opt) =>
             //{
             //    opt.ScanFolders.Add(TempPath, TempFileCleanerOptions.DefaultUnmanagedLifeTime);
             //})
             .AddApplicationFileSystemDriver(tempFileCleanerConfig: (opt) =>
             {
                 opt.ScanFolders.Add(TempPath, TempFileCleanerOptions.DefaultUnmanagedLifeTime);
             })
            .AddFileSystemQuotaManagement(pluginCollection)
            .AddElFinderPlugins(pluginCollection);

        // Custom IVideoEditor for generating video thumbnail
        builder.Services.AddSingleton<IVideoEditor, AppVideoEditor>();

        builder.Services.AddDbContext<DataContext>(options =>
            options.UseInMemoryDatabase(nameof(DataContext)));

        // Since elFinder use HTML5 download (hyper link), there is no Authorization header
        // --> better to use Cookies authentication (or mixed: need customization)
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                //...
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                //...
            });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        using (var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>())
        {
            dbContext.AddRange(new AppUser
            {
                Id = 1,
                UserName = "mrdavid",
                VolumePath = "mrdavid",
                QuotaInBytes = 500 * (long)Math.Pow(1024, 2),
            }, new AppUser
            {
                Id = 2,
                UserName = "msdiana",
                VolumePath = "msdiana",
                QuotaInBytes = (long)Math.Pow(1024, 3),
            });

            dbContext.SaveChanges();
        }

        app.Run();
    }
}

