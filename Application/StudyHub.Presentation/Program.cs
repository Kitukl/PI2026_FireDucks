using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Repositories;

using Serilog;
using StudyHub.Core.Comments.Interfaces;
using StudyHub.Core.Feedbacks.Interfaces;
using StudyHub.Core.Group;
using StudyHub.Core.Lecturers.Interfaces;
using StudyHub.Core.Lessons.Interfaces;
using StudyHub.Core.LessonSlots.Interfaces;
using StudyHub.Core.Schedules.Interfaces;
using StudyHub.Core.Statistics.Interfaces;
using StudyHub.Core.Statistics.Queries;
using StudyHub.Core.Subjects.Interfaces;
using StudyHub.Core.Tasks.Interfaces;
using StudyHub.Core.Users.Interfaces;
using StudyHub.Domain.Entities;

namespace Application;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
        
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<SDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<SDbContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICommentRepository, CommentRepository>();
        builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        builder.Services.AddScoped<IGroupRepository, GroupRepository>();
        builder.Services.AddScoped<ILecturerRepository, LecturerRepository>();
        builder.Services.AddScoped<ILessonRepository, LessonRepository>();
        builder.Services.AddScoped<ILessonSlotRepository, LessonSlotRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
        
        builder.Services.AddAuthentication()
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ??
                                   throw new Exception("Null client id");
                options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ??
                                       throw new Exception("Null client secrets");

                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(GetUsersStatisticHandler).Assembly));
        
        var app = builder.Build();

        app.UseSerilogRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication(); 
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}