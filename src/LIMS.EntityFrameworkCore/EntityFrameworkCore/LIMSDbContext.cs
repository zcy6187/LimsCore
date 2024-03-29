using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using LIMS.Authorization.Roles;
using LIMS.Authorization.Users;
using LIMS.MultiTenancy;
using LIMS.Assay;

namespace LIMS.EntityFrameworkCore
{
    public class LIMSDbContext : AbpZeroDbContext<Tenant, Role, User, LIMSDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public LIMSDbContext(DbContextOptions<LIMSDbContext> options)
            : base(options)
        {
        }

        public DbSet<SysManager.OrgInfo> OrgInfos { get; set; }
        public DbSet<Assay.Attendance> Attendances { get; set; }
        public DbSet<Assay.Element> Elements { get; set; }
        public DbSet<Assay.Specimen> Specimens { get; set; }
        public DbSet<Assay.Template> Templates { get; set; }
        public DbSet<Assay.TplElement> TplElements { get; set; }
        public DbSet<Assay.TplSpecimen> TplSpecimens { get; set; }
        public DbSet<Assay.Unit> Units { get; set; }
        public DbSet<Assay.TypeIn> TypeIns { get; set; }
        public DbSet<Assay.TypeInItem> TypeInItems { get; set; }
        public DbSet<Assay.Token> Tokens { get; set; }
        public DbSet<Assay.UserTpl> UserTpls { get; set; }
        public virtual DbSet<V_UserTpl> VUserTpls { get; set; }
        public virtual DbSet<V_Attendance> VAttendances { get; set; }
        public DbSet<SysManager.ZtCode> ZtCode { get; set; }
        public DbSet<Assay.AssayUser> AssayUser { get; set; }
        public DbSet<SysManager.UserZt> UserZt { get; set; }
        public DbSet<SysManager.UserTplSpecimens> UserTplSpecimen { get; set; }
        public DbSet<Assay.SelfTpl> SelfTpl { get; set; }
        public DbSet<Assay.AssayEleFormula> EleFormula { get; set; }
        public DbSet<Assay.AssayFormulaPram> FormulaPram { get; set; }
        public DbSet<Assay.AssayConst> FormulaConst { get; set; }
        public DbSet<Assay.UserOrg> UserOrgs { get; set; }
        public DbSet<Assay.UserOrgTpl> UserOrgTpls { get; set; }
        public DbSet<MaterialPrintData> MaterialPrintData { get; set; }
        public DbSet<DetectMainInfo> DetectMainInfo { get; set; }
        public DbSet<DetectMainInfoItems> DetectMainInfoItems { get; set; }
        public DbSet<DuplicationInfoItems> DuplicationInfoItems { get; set; }
        public DbSet<DuplicationElements> DuplicationElements { get; set; }
        public DbSet<ImportHistory> ImporHistory { get; set; }
        public DbSet<DuplicationModifyElements> DuplicationModifyElements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Assay.Attendance>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Attendance>().Property(x => x.Flag)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Element>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Specimen>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Template>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.TplElement>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.TplSpecimen>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Unit>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.TypeIn>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.TypeInItem>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.Token>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.UserTpl>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assay.AssayUser>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<SysManager.ZtCode>().Property(x => x.IsDeleted)
                .HasDefaultValue(0);

            modelBuilder.Entity<SysManager.UserTplSpecimens>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<AssayEleFormula>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<AssayConst>().Property(x => x.IsDeleted)
               .HasDefaultValue(0);

            modelBuilder.Entity<Assay.UserOrg>().Property(x => x.IsDeleted)
              .HasDefaultValue(0);

            modelBuilder.Entity<Assay.UserOrgTpl>().Property(x => x.IsDeleted)
              .HasDefaultValue(0);

            modelBuilder.Entity<Assay.MaterialPrintData>().Property(x => x.IsDeleted)
              .HasDefaultValue(0);

            modelBuilder.Entity<Assay.DetectMainInfo>().Property(x => x.IsDeleted)
              .HasDefaultValue(0);

            modelBuilder.Entity<Assay.DetectMainInfoItems>().Property(x => x.IsDeleted)
             .HasDefaultValue(0);

            modelBuilder.Entity<Assay.DuplicationInfoItems>().Property(x => x.IsDeleted)
            .HasDefaultValue(0);

            modelBuilder.Entity<Assay.DuplicationModifyElements>().Property(x => x.IsDeleted)
            .HasDefaultValue(0);

            modelBuilder.Entity<V_UserTpl>(entity =>
            {
                entity.ToTable("v_userTpl");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<V_Attendance>(entity =>
            {
                entity.ToTable("v_attendance");
                entity.HasKey(x => x.Id);
            });

        }
    }
}
