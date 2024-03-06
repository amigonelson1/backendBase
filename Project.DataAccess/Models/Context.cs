using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class Context : IdentityDbContext<User, Role, string>
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(b => b.MigrationsAssembly("Project.IO"));
        }

        ////////////////////////////////// Region Test Table //////////////////////////////////
        public DbSet<TestParentTable> Parents { get; set; } = null!;
        public DbSet<TestChildTable> Childrens { get; set; } = null!;
        ///////////////////////////////////////////////////////////////////////////////////////

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var table = entityType.GetTableName();
                if (table!.StartsWith("AspNet"))
                {
                entityType.SetTableName(table.Substring(6));
                }
            };
        }
        
    }
}