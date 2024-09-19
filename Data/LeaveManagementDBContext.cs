using LeaveManagementSystemNew.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystemNew.Data
{
    public class LeaveManagementDBContext : IdentityDbContext<IdentityUser>
    {
        public LeaveManagementDBContext(DbContextOptions<LeaveManagementDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(e =>
            {
                e.ToTable("employees");
                e.HasKey(p => p.Id);

                e.Property(p => p.Id).UseIdentityAlwaysColumn().IsRequired();
                e.Property(p => p.Name).IsRequired();
                e.Property(p => p.RegistrationNumber).IsRequired();

                e.HasOne(e => e.Account)
                    .WithOne(a => a.Employee)
                    .HasForeignKey<Account>(a => a.EmployeeId);
            });

            modelBuilder.Entity<Leave>(l =>
            {
                l.ToTable("leaves");
                l.HasKey(p => p.Id);

                l.Property(p => p.Id).UseIdentityAlwaysColumn().IsRequired();
                l.Property(p => p.LeaveDuration).IsRequired();
                l.Property(p => p.StartTime).IsRequired();
                l.Property(p => p.EmployeeId).IsRequired();

                l.HasOne(l => l.Employee)
                    .WithMany(e => e.Leaves)
                    .HasForeignKey(l => l.EmployeeId);
            });

            modelBuilder.Entity<Account>(a =>
            {
                a.ToTable("accounts");
                a.HasKey(p => p.Id);

                a.Property(p => p.Id).UseIdentityAlwaysColumn().IsRequired();
                a.Property(p => p.Username).IsRequired();
                a.Property(p => p.Password).IsRequired();
                a.Property(p => p.Role).IsRequired();

                a.HasOne(a => a.Employee)
                    .WithOne(e => e.Account)
                    .HasForeignKey<Account>(a => a.EmployeeId);
            });
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
