using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TightWiki
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHost(options =>
            {
                options.ConfigureKestrel(opt =>
                {
                    opt.ListenLocalhost(44335);
                    opt.ListenAnyIP(44335);
                });
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
