using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSL.ASPNET.Identity.Host;
using NSL.Database.EntityFramework.ASPNET;
using NSL.Management.CentralService.Client.Pages;
using NSL.Management.CentralService.ExternalClient.Data.Models.RequestModels;
using NSL.Management.CentralService.Shared.Models;
using NSL.Management.CentralService.Shared.Server.Data;
using NSL.Management.CentralService.Shared.Server.Manages;
using NSL.Management.CentralService.Utils.Sync;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NSL.Management.CentralService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
            .AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();


            builder.Services.AddAPIBaseIdentity<UserModel, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager<AppSignInManager>()
                .AddUserManager<AppUserManager>()
                .AddRoleManager<AppRoleManager>();

            builder.Services.AddDefaultAuthenticationForAPIBaseJWT()
                .AddAPIBaseJWTBearer(builder.Configuration);

            builder.Services.ConfigureApplicationCookie(c =>
            {
                c.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents()
                {
                    OnRedirectToAccessDenied = c =>
                    {
                        c.Response.StatusCode = (int)HttpStatusCode.Forbidden;

                        return Task.CompletedTask;
                    },
                    OnRedirectToLogin = c =>
                    {
                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddSyncIdentityService();

            var app = builder.Build();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            await app.AcceptDbMigrations<ApplicationDbContext>();

            await app.LoadSyncIdentityServiceAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
#if DEBUG
            app.UseHttpsRedirection();
#endif

            app.UseAuth();

            app.UseStaticFiles();
            //app.UseAntiforgery();

            app.MapDefaultControllerRoute();

            app.UseBlazorFrameworkFiles();
            app.MapFallbackToFile("index.html");

            //await using var scope = app.Services.CreateAsyncScope();
            //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //var serverId = Guid.Parse("aa31fa1b-f543-4747-ae17-b4fb49761d21");

            //var query = new SyncReportMetricsRequestModel()
            //{
            //    Metrics = [
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 },
            //        new SyncReportMetricDataModel() {
            //        CreateTime = new DateTime(2024, 10, 08, 14, 56, 55,560,176),
            //        Name = "dev1",
            //        OperationType = MetricsOperationType.Increment,
            //        ValidInterval = TimeSpan.FromSeconds(5),
            //        Value = 1 }
            //        ]
            //};

            //var dbMetrics = dbContext.ServerMetrics.Where(x => x.ServerId == serverId);
            //foreach (var item in query.Metrics.Where(x => x.OperationType == MetricsOperationType.Increment).ToArray())
            //{
            //    var dbitem = await dbMetrics.FirstOrDefaultAsync(x => x.Name == item.Name
            //    && (item.ValidInterval.HasValue && x.CreateTime == item.CreateTime));

            //    if (dbitem != null)
            //    {
            //        dbitem.Value += item.Value;
            //        continue;
            //    }

            //    item.OperationType = MetricsOperationType.New;
            //}

            //dbContext.ServerMetrics.AddRange(query.Metrics
            //    .Where(x => x.OperationType == MetricsOperationType.New)
            //    .Select(x =>
            //    {
            //        var log = new ServerMetricsModel();

            //        log.FillFrom(x);

            //        log.ServerId = serverId;

            //        return log;
            //    }));

            //await dbContext.SaveChangesAsync();


            // Add additional endpoints required by the Identity /Account Razor components.
            //app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
