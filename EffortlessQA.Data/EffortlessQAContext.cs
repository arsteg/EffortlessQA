using System.Reflection;
using System.Text.Json;
using EffortlessQA.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Data
{
    public class EffortlessQAContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EffortlessQAContext(
            DbContextOptions<EffortlessQAContext> options,
            IHttpContextAccessor httpContextAccessor
        )
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Requirement> Requirements { get; set; }
        public DbSet<RequirementTestCase> RequirementTestCases { get; set; }
        public DbSet<RequirementTestSuite> RequirementTestSuites { get; set; }
        public DbSet<TestSuite> TestSuites { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }
        public DbSet<TestRunResult> TestRunResults { get; set; }
        public DbSet<Defect> Defects { get; set; }
        public DbSet<DefectHistory> DefectHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<TestFolder> TestFolders { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<UserEmailConfirmation> UserEmailConfirmations { get; set; }
        public DbSet<TenantEmailConfirmation> TenantEmailConfirmations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // JSONB mappings
            modelBuilder
                .Entity<TestRunResult>()
                .Property(t => t.Attachments)
                .HasColumnType("jsonb");
            modelBuilder.Entity<Defect>().Property(d => d.Attachments).HasColumnType("jsonb");
            modelBuilder.Entity<DefectHistory>().Property(d => d.Details).HasColumnType("jsonb");
            modelBuilder.Entity<UserProject>().Property(u => u.Preferences).HasColumnType("jsonb");
            modelBuilder.Entity<AuditLog>().Property(a => a.Details).HasColumnType("jsonb");

            // Enum conversions
            modelBuilder.Entity<TestRunResult>().Property(t => t.Status).HasConversion<string>();
            modelBuilder.Entity<Defect>().Property(d => d.Severity).HasConversion<string>();
            modelBuilder.Entity<Defect>().Property(d => d.Status).HasConversion<string>();
            modelBuilder.Entity<TestCase>().Property(t => t.Priority).HasConversion<string>();

            // Indexes
            modelBuilder.Entity<Tenant>().HasIndex(t => t.Id);
            modelBuilder.Entity<User>().HasIndex(u => u.TenantId);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Project>().HasIndex(p => p.TenantId);
            modelBuilder.Entity<Requirement>().HasIndex(r => new { r.TenantId, r.ProjectId });
            modelBuilder.Entity<TestSuite>().HasIndex(s => new { s.TenantId, s.ProjectId });
            modelBuilder
                .Entity<TestCase>()
                .HasIndex(t => new
                {
                    t.TenantId,
                    t.TestSuiteId,
                    t.Priority
                });
            modelBuilder.Entity<TestRun>().HasIndex(t => new { t.TenantId, t.ProjectId });
            modelBuilder
                .Entity<TestRunResult>()
                .HasIndex(t => new
                {
                    t.TenantId,
                    t.TestRunId,
                    t.Status
                });
            modelBuilder
                .Entity<Defect>()
                .HasIndex(d => new
                {
                    d.TenantId,
                    d.Status,
                    d.Severity
                });
            modelBuilder
                .Entity<DefectHistory>()
                .HasIndex(d => new
                {
                    d.TenantId,
                    d.DefectId,
                    d.CreatedAt
                });
            modelBuilder.Entity<AuditLog>().HasIndex(a => new { a.TenantId, a.CreatedAt });

            // Many-to-many configurations
            modelBuilder
                .Entity<RequirementTestCase>()
                .HasKey(rt => new { rt.RequirementId, rt.TestCaseId });
            modelBuilder
                .Entity<RequirementTestSuite>()
                .HasOne(rt => rt.Requirement)
                .WithMany(r => r.RequirementTestSuites)
                .HasForeignKey(rt => rt.RequirementId);
            modelBuilder
                .Entity<RequirementTestCase>()
                .HasOne(rt => rt.TestCase)
                .WithMany(t => t.RequirementTestCases)
                .HasForeignKey(rt => rt.TestCaseId);

            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            modelBuilder.Entity<RolePermission>().HasOne(rp => rp.Role);
            modelBuilder
                .Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder
                .Entity<UserProject>()
                .HasIndex(u => new { u.UserId, u.ProjectId })
                .IsUnique();
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst("sub")?.Value;
            Guid? currentUserId =
                userId != null && Guid.TryParse(userId, out var parsedId) ? parsedId : null;

            var auditEntries = new List<AuditLog>();
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUserId ?? Guid.Empty;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = currentUserId;
                }

                var entityType = entry.Entity.GetType();
                var isAuditableEntity = entityType.GetCustomAttribute<AuditableAttribute>() != null;
                var auditableProperties = entityType
                    .GetProperties()
                    .Where(p => p.GetCustomAttribute<AuditableAttribute>() != null)
                    .Select(p => p.Name)
                    .ToList();

                if (!isAuditableEntity && !auditableProperties.Any())
                    continue;

                Guid? projectId = null;
                string tenantId =
                    entry
                        .Entity.GetType()
                        .GetProperty("TenantId")
                        ?.GetValue(entry.Entity)
                        ?.ToString() ?? "Unknown";
                if (entityType.GetProperty("ProjectId") != null)
                {
                    projectId = (Guid?)entityType.GetProperty("ProjectId")?.GetValue(entry.Entity);
                }

                var details = new Dictionary<string, object>();

                switch (entry.State)
                {
                    case EntityState.Added:
                        details = auditableProperties.Any()
                            ? auditableProperties.ToDictionary(
                                p => p,
                                p => entityType.GetProperty(p)?.GetValue(entry.Entity) ?? "N/A"
                            )
                            : new Dictionary<string, object> { { "FullEntity", "Created" } };
                        auditEntries.Add(
                            new AuditLog
                            {
                                Action = $"{entityType.Name}Created",
                                EntityType = entityType.Name,
                                EntityId = GetEntityId(entityType, entry.Entity),
                                TenantId = tenantId,
                                Details = JsonDocument.Parse(JsonSerializer.Serialize(details)),
                                CreatedBy = currentUserId ?? Guid.Empty,
                                CreatedAt = DateTime.UtcNow
                            }
                        );
                        break;
                }
            }

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private Guid GetEntityId(Type entityType, object entity)
        {
            var idProperty = entityType.GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException(
                    $"Entity {entityType.Name} does not have an Id property."
                );

            var idValue = idProperty.GetValue(entity);
            if (idValue == null)
                throw new InvalidOperationException(
                    $"Id property of entity {entityType.Name} is null."
                );

            if (idValue is Guid guidId)
                return guidId;
            else if (idValue is string stringId && Guid.TryParse(stringId, out var parsedGuid))
                return parsedGuid;
            else
                throw new InvalidOperationException(
                    $"Id property of entity {entityType.Name} is of unsupported type {idValue.GetType().Name}."
                );
        }
    }
}
