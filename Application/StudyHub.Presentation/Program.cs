using Application.Middleware;
using Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Repositories;
using Serilog;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Group;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Services;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Storage.Interfaces;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.UserSessions.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Core.Notifications.Interfaces;
using StudyHub.Domain.Entities;
using StudyHub.Infrastructure.Services;
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
            if (!File.Exists(envPath))
            {
                continue;
            }

            Env.Load(envPath);
            break;
        }

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddControllersWithViews();
        builder.Services.Configure<SessionTrackingOptions>(
            builder.Configuration.GetSection(SessionTrackingOptions.SectionName));
        builder.Services.AddDbContext<SDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/user/access-denied";
        });

        builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserAuthRepository, UserAuthRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<ILessonRepository, LessonRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();   
        
        builder.Services.AddScoped<IBlobService, BlobService>();
        builder.Services.AddScoped<IUserSessionCookieStore, UserSessionCookieStore>();
        builder.Services.AddScoped<IUserSessionTrackingService, UserSessionTrackingService>();
        builder.Services.AddScoped<IGlobalAnnouncementService, GlobalAnnouncementService>();
        
        builder.Services.AddHostedService<DeadlineSender>();
        builder.Services.AddHostedService<MonthlyStatisticsAggregationService>();
        builder.Services.AddHostedService<UserSessionExpirationService>();
        
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
                options.CallbackPath = "/signin-microsoft";

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

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

        builder.Services.AddHttpClient<IScheduleParserClient, ScheduleParserClient>(c => c.BaseAddress = new Uri(builder.Configuration["Parser:Url"] ?? "http://localhost:5678"));
        builder.Services.AddHostedService<ScheduleAutoUpdateService>();

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/UserPlatform/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseMiddleware<AccessControlMiddleware>();

        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=UserPlatform}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
