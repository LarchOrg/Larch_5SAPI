using Microsoft.EntityFrameworkCore;
using _5sAudit.Models;

namespace _5sAudit.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public virtual DbSet<FsaUser> FsaUsers { get; set; }
        //public virtual DbSet<FsaChecklistMst> Questions { get; set; }
        public virtual DbSet<FsaDepartment> FsaDepartments { get; set; }
        public virtual DbSet<FsaAuditInit> FsaAuditInits { get; set; }
        public virtual DbSet<FsaChecklistMst> FsaChecklistMsts { get; set; }
        public virtual DbSet<FsaAuditResponse> FsaAuditResponses { get; set; }
        public virtual DbSet<FsaScoreSummary> FsaScoreSummaries { get; set; }
        public virtual DbSet<FsaRole> FsaRoleMsts { get; set; }
        //public virtual DbSet<FsaDepartment> FsaDepartment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<FsaUser>(entity =>
            {
                entity.Property(e => e.Status).IsFixedLength();
            });*/
            modelBuilder.Entity<FsaUser>(entity =>
            {
                // Mapping to your specific table name in DB
                entity.ToTable("fsa_users");
                entity.Property(e => e.Status).IsFixedLength();

                entity.Property(e => e.CompanyId)
              .HasColumnName("CompanyId")
              .HasColumnType("int")
              .IsRequired();

                // If ProfilePicture is a URL/String, ensure it isn't mapped as varbinary
                entity.Property(e => e.ProfilePicture)
                      .HasColumnType("varchar(max)");
            });
            /*
            modelBuilder.Entity<FsaChecklistMst>(entity =>
            {
                entity.ToTable("fsa_checklist_mst");
                entity.HasKey(e => e.Id);
            });
            */
            // Map the Question model to the master checklist table
            modelBuilder.Entity<FsaChecklistMst>(entity =>
            {
                entity.ToTable("fsa_checklist_mst");
                entity.HasKey(e => e.Id);

                // Explicitly map properties to column names if they differ
                entity.Property(e => e.Question).HasColumnName("Question");
                entity.Property(e => e.AuditType).HasColumnName("AuditType");
                entity.Property(e => e.Category).HasColumnName("Category");
                entity.Property(e => e.Status).HasColumnName("Status");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy");
                entity.Property(e => e.CreatedDt).HasColumnName("CreatedDt");
            });

            modelBuilder.Entity<FsaRole>(entity =>
            {
                entity.ToTable("fsa_role_mst");
                entity.HasKey(e => e.RoleId);
            });

            // Mapping for Department
            modelBuilder.Entity<FsaDepartment>(entity =>
            {
                entity.ToTable("fsa_department_mst");
                entity.HasKey(e => e.CdIId); // Primary Key
            });

            modelBuilder.Entity<FsaAuditResponse>(entity =>
            {
                entity.ToTable("fsa_audit_responses");
                entity.HasKey(e => e.id);
            });

            modelBuilder.Entity<FsaScoreSummary>(entity =>
            {
                entity.ToTable("fsa_score_summary");
                entity.HasKey(e => e.id);

                // Ensure decimal precision matches SQL Server (18, 2)
                entity.Property(e => e.total_score).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.max_possible_score).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.percentage).HasColumnType("decimal(18, 2)");
            });

            // Mapping for Audit Initiation
            /*
            modelBuilder.Entity<FsaAuditInit>(entity =>
            {
                entity.ToTable("fsa_audit_init");
                entity.HasKey(e => e.FaiIId);
            });*/
            //OnModelCreatingPartial(modelBuilder);
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}