using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SMARTchat.DAL.Entities;

namespace SMARTchat.DAL.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private static string _connection;

        public ApplicationDbContext(string connectionString)
            : base("ApplicationDbContext", false)
        {
            _connection = connectionString;
        }

        public virtual DbSet<Channel> Channels { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext(_connection);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasMany(m => m.Parents).WithMany();
            base.OnModelCreating(modelBuilder);
        }
    }
}