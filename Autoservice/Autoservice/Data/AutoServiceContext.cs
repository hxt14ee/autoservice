using System.Data.Entity;
using Autoservice.Models;

namespace Autoservice.Data
{
    public class AutoServiceContext : DbContext
    {
        public AutoServiceContext() : base("AutoServiceDb")
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Service> Services { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Конфигурация моделей если нужна
        }
    }
}
