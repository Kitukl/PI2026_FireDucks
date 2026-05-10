using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.Domain.Entities;

namespace StudyHub.Infrastructure.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(session => session.Id);

        builder.Property(session => session.EntryTimeUtc).IsRequired();
        builder.Property(session => session.LastSeenUtc).IsRequired();
        builder.Property(session => session.IsClosed).IsRequired().ValueGeneratedNever();
        builder.Property(session => session.DurationSeconds).IsRequired().ValueGeneratedNever();

        builder.HasIndex(session => session.UserId);
        builder.HasIndex(session => new { session.UserId, session.IsClosed });

        builder.HasOne(session => session.User)
            .WithMany(user => user.Sessions)
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
