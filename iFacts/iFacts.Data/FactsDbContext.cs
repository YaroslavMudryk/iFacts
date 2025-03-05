using Extensions.DeviceDetector.Models;
using iFacts.Data.Audit;
using iFacts.Data.Entities;
using iFacts.Shared.Auth;
using iFacts.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace iFacts.Data;

public class FactsDbContext(DbContextOptions<FactsDbContext> options) : DbContext(options)
{
    public DbSet<PasswordHistory> PasswordHistories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<AppClaim> AppClaims { get; set; }
    public DbSet<App> Apps { get; set; }
    public DbSet<Fact> Facts { get; set; }

    public DbSet<AuditItem> Audits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordHistory>(entity =>
        {
            entity.ToTable("passwordHistories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.UserId).HasDatabaseName("idx_passwordHistory_userId");
            entity.HasIndex(p => p.IsActive).HasDatabaseName("idx_passwordHistory_isActive");
            entity.Property(p => p.Hint).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.Login).IsUnique().HasDatabaseName("idx_user_login");
            entity.HasIndex(p => p.UserName).IsUnique().HasDatabaseName("idx_user_userName");

            entity.HasIndex(p => p.FirstName)
                .HasMethod("GIN")
                .HasOperators("gin_trgm_ops")
                .HasDatabaseName("idx_user_first_name_trgm");
            entity.HasIndex(p => p.LastName)
                .HasMethod("GIN")
                .HasOperators("gin_trgm_ops")
                .HasDatabaseName("idx_user_last_name_trgm");

            entity.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.LastName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.UserName).HasMaxLength(25);
            entity.Property(p => p.Login).HasMaxLength(150);

            entity.HasQueryFilter(qf => qf.DeletedAt == null);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Client).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<ClientInfo>());

            entity.Property(x => x.Location).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<Location>());

            entity.Property(x => x.App).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<AppModel>());

            entity.HasQueryFilter(qf => qf.DeletedAt == null);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("userRoles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.UserId).HasDatabaseName("idx_userRole_userId");
            entity.HasIndex(p => p.RoleId).HasDatabaseName("idx_userRole_roleId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(p => p.Name).HasMaxLength(100).IsRequired();
            entity.Property(p => p.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.ToTable("roleClaims");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.RoleId).HasDatabaseName("idx_roleClaim_roleId");
            entity.HasIndex(p => p.ClaimId).HasDatabaseName("idx_roleClaim_claimId");
        });

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.ToTable("claims");
            entity.HasKey(e => e.Id);
            entity.Property(p => p.Type).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Value).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<AppClaim>(entity =>
        {
            entity.ToTable("appClaims");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.AppId).HasDatabaseName("idx_appClaim_appId");
            entity.HasIndex(p => p.ClaimId).HasDatabaseName("idx_appClaim_claimId");
        });

        modelBuilder.Entity<App>(entity =>
        {
            entity.ToTable("apps");
            entity.HasKey(e => e.Id);
            entity.HasIndex(p => p.IsActive).HasDatabaseName("idx_app_isActive");
            entity.Property(p => p.Name).HasMaxLength(150).IsRequired();
            entity.Property(p => p.ShortName).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Image).HasMaxLength(1500);
            entity.Property(p => p.ClientId).HasMaxLength(35).IsRequired();
            entity.Property(p => p.ClientSecret).HasMaxLength(35).IsRequired();
        });

        modelBuilder.Entity<AuditItem>(entity =>
        {
            entity.ToTable("auditItems");
            entity.HasKey(e => e.Id);
            entity.Property(p => p.By).HasMaxLength(75);
            entity.Property(p => p.Event).HasMaxLength(50).IsRequired();
            entity.Property(p => p.ItemId).HasMaxLength(50);
            entity.Property(p => p.ItemType).HasMaxLength(100);
            entity.Property(p => p.TransactionId).HasMaxLength(75);
            entity.Property(c => c.Changes).HasConversion(
                v => v.ToJson(),
                v => v.FromJson<List<PropertyInfo>>()).HasColumnType("jsonb");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AuditItemsBeforeSaveChanges();
        var audits = OnBeforeSaveChanges();

        var result = await base.SaveChangesAsync(cancellationToken);

        await OnAfterSaveChangesAsync(audits);
        return result;
    }

    protected virtual List<AuditItem> OnBeforeSaveChanges()
    {
        var auditRepo = this.GetService<AuditRepo>();
        var timeProvider = this.GetService<TimeProvider>();
        var userContext = this.GetService<IUserContext>();

        return auditRepo.GetAuditsFromEntries(this, timeProvider, userContext);
    }

    protected virtual Task OnAfterSaveChangesAsync(List<AuditItem> audits)
    {
        if (audits == null || audits.Count == 0)
            return Task.CompletedTask;

        return this.GetService<AuditRepo>().SaveAuditsAsync(this, audits);
    }

    protected virtual void AuditItemsBeforeSaveChanges()
    {
        var timeProvider = this.GetService<TimeProvider>();
        var userContext = this.GetService<IUserContext>();
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var by = userContext.GetBy<BasicAuthenticatedUser>();
        var entries = ChangeTracker.Entries().Where(s => s.Entity is IAuditable).Select(entry =>
        {
            var entity = entry.Entity as IAuditable;
            ArgumentNullException.ThrowIfNull(entity);
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeleted sd)
            {
                sd.DeletedAt = now;
                sd.DeletedBy = by;
                entry.State = EntityState.Modified;
            }
            switch (entry.State)
            {
                case EntityState.Modified:
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = by;
                    break;
                case EntityState.Added:
                    entity.CreatedAt = now;
                    entity.CreatedBy = by;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            entity.Version++;
            return entry;
        }).ToList();
    }
}
