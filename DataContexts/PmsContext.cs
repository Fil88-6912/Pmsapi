using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PmsApi.Models;
using Task = PmsApi.Models.Task;

namespace PmsApi.DataContexts
{
    public class PmsContext : IdentityDbContext
    {
        private string connectionString = String.Empty;
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public new DbSet<Role> Roles { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskAttachement> TaskAttachements { get; set; }
        public new DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        
        public PmsContext() {
            
        }

        public PmsContext(string connectionString) {
            this.connectionString = connectionString;
        }

        public PmsContext(DbContextOptions<PmsContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != String.Empty) { 
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder
            //.UseCollation()
            //.HasCharSet("utf8mb4");

            ConfigureProject(modelBuilder);

            modelBuilder.Entity<ProjectCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });

            ConfigureTask(modelBuilder);

            modelBuilder.Entity<TaskAttachement>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.FileName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.FileData).HasMaxLength(50);

                entity.HasOne(d => d.Task).WithMany(p => p.Attachement).HasForeignKey(d => d.TaskId).IsRequired();
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<Priority>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            });

            //OnModelCreatingPartial(modelBuilder);
            populateDatabase(modelBuilder);
        }

        private void ConfigureProject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.Name).IsUnique();

                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                entity.HasOne(d => d.Category).WithMany(p => p.Projects).HasForeignKey(d => d.CategoryId).IsRequired();
                entity.HasOne(d => d.Manager).WithMany(p => p.Projects).HasForeignKey(d => d.ManagerId).IsRequired();
            });
        }

        private void ConfigureTask(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.Title).HasMaxLength(100).IsRequired();;
                entity.Property(d => d.StatusId).IsRequired();
                entity.Property(d => d.PriorityId).IsRequired();

                //entity.HasOne(d => d.Status);
                //entity.HasOne(d => d.Priority);
                entity.HasOne(d => d.User).WithMany(p => p.Tasks).HasForeignKey(d => d.AssignedUserId).IsRequired();
                entity.HasOne(d => d.Project).WithMany(p => p.Tasks).HasForeignKey(d => d.ProjectId).IsRequired();
            });
        }

        private void populateDatabase(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Name = "Admin", NormalizedName = "ADMIN" },
                new Role { Name = "Editor" },
                new Role { Name = "User" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { UserName = "userSnow", FirstName = "Jhon", LastName = "Snow", Email = "red@yellow.it" },
                new User { UserName = "user2", FirstName = "Pippo", LastName = "Balli", Email = "orange@yellow.it" },
                new User { UserName = "user3", FirstName = "Jhonny", LastName = "Snowie", Email = "red3@yellow.it" },
                new User { UserName = "monello", FirstName = "Charlie", LastName = "Chaplin", Email = "monello@yellow.it" }
            );

            modelBuilder.Entity<ProjectCategory>().HasData(
                new ProjectCategory { Id = 1, Name = "Categoria1" },
                new ProjectCategory { Id = 2, Name = "Categoria2" },
                new ProjectCategory { Id = 3, Name = "Categoria3" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 2, Name = "Status 2" },
                new Status { Id = 4, Name = "Status 4" },
                new Status { Id = 5, Name = "Status 5" }
            );

            modelBuilder.Entity<Priority>().HasData(
                new Priority { Id = 1, Name = "Priority 1" }
            );
        }

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}