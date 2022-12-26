using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.Property(p => p.Id)
            .IsRequired();

        builder.Property(p => p.Names)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(p => p.Roles)
            .WithMany(p => p.Users)
            .UsingEntity<UsersRols>(
            j => j
            .HasOne(pt => pt.Role)
            .WithMany(t => t.UsersRoles)
            .HasForeignKey(pt => pt.RoleId),
            j => j
            .HasOne(pt => pt.User)
            .WithMany(p => p.UsersRoles)
            .HasForeignKey(pt => pt.UserId),
            j =>
            {
                j.HasKey(t => new { t.UserId, t.RoleId });
            });

        builder.HasMany(p => p.RefreshTokens)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
    }
}
