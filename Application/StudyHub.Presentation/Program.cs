using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Application.Security;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Repositories;
using Microsoft.AspNetCore.HttpOverrides; // Додано

using Serilog;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Group;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Services;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Infrastructure.Services;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Notifications;
using StudyHub.Infrastructure.Storage;

namespace Application;

public class Program
{
    public static void Main(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var envCandidates = new[]
        {
            Path.Combine(currentDirectory, ".env"),
            Path.Combine(currentDirectory, "StudyHub.Presentation", ".env")
        };

        foreach (var envPath in envCandidates)
        {
            if (File.Exists(envPath))
            {
                Env.Load(envPath);
                break;
            }
        }

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<SDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/user/access-denied";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Важливо для Azure
        });
        
        // Репозиторії та сервіси
        builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IBlobService, BlobService>();
        builder.Services.AddScoped<IGlobalAnnouncementService, GlobalAnnouncementService>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<ILecturerRepository, LecturerRepository>();
        builder.Services.AddScoped<ILessonRepository, LessonRepository>();
        builder.Services.AddScoped<ILessonSlotRepository, LessonSlotRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        builder.Services.AddScoped<IClaimsTransformation, UserRolesClaimsTransformation>();
        
        var authenticationBuilder = builder.Services.AddAuthentication();
        var microsoftClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
        var microsoftClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];

        if (!string.IsNullOrWhiteSpace(microsoftClientId) && !string.IsNullOrWhiteSpace(microsoftClientSecret))
        {
            authenticationBuilder.AddMicrosoftAccount(options =>
            {
                options.ClientId = microsoftClientId;
                options.ClientSecret = microsoftClientSecret;
                options.SignInScheme = IdentityConstants.ExternalScheme;
                
                // МАЄ ЗБІГАТИСЯ з Url.Action у UserController
                options.CallbackPath = "/user/callback"; 

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Events.OnRemoteFailure = context =>
                {
                    context.HandleResponse();
                    context.Response.Redirect("/login?error=external_auth_failed");
                    return System.Threading.Tasks.Task.CompletedTask;
                };
            });
        }
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetUsersStatisticHandler).Assembly));

        builder.Services.AddHttpClient<IScheduleParserClient, ScheduleParserClient>(c => 
            c.BaseAddress = new Uri(builder.Configuration["Parser:Url"] ?? "http://localhost:5678"));
        builder.Services.AddHostedService<ScheduleAutoUpdateService>();

        var app = builder.Build();

        // Додано для коректної роботи HTTPS в Azure
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication(); 

        // Оновлений Middleware
        app.Use(async (context, next) =>
        {
            var path = context.Request.Path;

            static bool IsPathMatch(string? value, string prefix)
            {
                return !string.IsNullOrWhiteSpace(value) &&
                       value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            }

            var isBypassedPath = path.HasValue &&
                                 (IsPathMatch(path.Value, "/login") ||
                                  IsPathMatch(path.Value, "/user/login-microsoft") ||
                                  IsPathMatch(path.Value, "/user/callback") || // ВАЖЛИВО: додано
                                  IsPathMatch(path.Value, "/user/access-denied") ||
                                  Path.HasExtension(path.Value));

            if (!isBypassedPath && context.User.Identity?.IsAuthenticated != true)
            {
                context.Response.Redirect("/login");
                return;
            }

            await next();
        });

        app.UseAuthorization();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
