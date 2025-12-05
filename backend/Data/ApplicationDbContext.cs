using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace backend.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        // DbSets will be added here later
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<WorkspaceMember> WorkspaceMembers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskAttachment> TaskAttachments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Custom configurations will be added here
        }

    }
}