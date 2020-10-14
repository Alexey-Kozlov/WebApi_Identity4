using System;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Serilog;

namespace WebApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel((context, serverOptions) =>
                {
                    serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
                    .Endpoint("HTTPS", listenOptions =>
                    {
                        listenOptions.HttpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    });
                });
                webBuilder.ConfigureKestrel(options =>
                {
                    options.ConfigureHttpsDefaults(listenOptions =>
                    {
                        listenOptions.ServerCertificate = CertificateLoader.LoadFromStoreCert("Ws-PC-70", "Root", StoreLocation.LocalMachine, false);
                    });
                });
            })
            .UseSerilog();
    }
}
