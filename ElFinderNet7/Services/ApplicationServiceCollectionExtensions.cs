using System;
using System.Linq;
using Castle.DynamicProxy;
using elFinder.Net.Core;
using elFinder.Net.Core.Helpers;
using elFinder.Net.Core.Plugins;
using elFinder.Net.Core.Services;
using elFinder.Net.Core.Services.Drawing;
using elFinder.Net.Drivers.FileSystem;
using elFinder.Net.Drivers.FileSystem.Services;
using elFinder.Net.Plugins.FileSystemQuotaManagement;
using elFinder.Net.Plugins.FileSystemQuotaManagement.Contexts;
using elFinder.Net.Plugins.FileSystemQuotaManagement.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ElFinderNet7.Services
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationFileSystemDriver(this IServiceCollection services,
            Action<TempFileCleanerOptions> tempFileCleanerConfig = null)
        {
            if (tempFileCleanerConfig == null)
                tempFileCleanerConfig = (opt) => { };

            return services.AddScoped<IDriver, ApplicationFileSystemDriver>()
                .AddSingleton<IZipDownloadPathProvider, TempZipDownloadPathProvider>()
                .AddSingleton<IZipFileArchiver, ZipFileArchiver>()
                .AddSingleton<IThumbnailBackgroundGenerator, DefaultThumbnailBackgroundGenerator>()
                .AddSingleton<ICryptographyProvider, DefaultCryptographyProvider>()
                .AddSingleton<ITempFileCleaner, DefaultTempFileCleaner>()
                .Configure(tempFileCleanerConfig);
        }
    }
}