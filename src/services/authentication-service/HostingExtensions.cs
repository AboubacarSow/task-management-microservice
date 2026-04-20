using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using authentication_service.Services;
using authentication_service.Validators;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;

namespace authentication_service;

internal static class HostingExtensions
{


    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        //builder.Services.AddRazorPages();
          
    var cert = X509CertificateLoader
                    .LoadPkcs12FromFile("/app/keys/identityserver.pfx", builder.Configuration["Certificate:Password"]!); 

            
        builder.Services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

        }).AddSigningCredential(cert)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator>()
            .AddProfileService<UserProfileService>()
            .AddLicenseSummary();

        

        builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("/app/dataprotection"))
        .SetApplicationName("authentication-service");
        builder.Services.AddEndpointsApiExplorer();
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";   
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        //app.UseStaticFiles();
        //app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        //app.UseAuthorization();
        //app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
