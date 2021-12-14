using Pandora.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Pandora.Core.Migrations
{
    public class PandoraDbContext : DbContext
    {
        public DbSet<TablesEntity> Tables { get; set; }
        public DbSet<RoomsEntity> Rooms { get; set; }
        public DbSet<RestaurantsEntity> Restaurants { get; set; }
        public DbSet<OrdersEntity> Orders { get; set; }
        public DbSet<OrdersDetailEntity> OrdersDetail { get; set; }
        public DbSet<MenusEntity> Menus { get; set; }
        public DbSet<DishesEntity> Dishes { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<InvoicesEntity> Invoices { get; set; }
        public DbSet<ClientsEntity> Clients { get; set; }
        public DbSet<IngredientEntity> Ingredients { get; set; }

        public PandoraDbContext(DbContextOptions<PandoraDbContext> options)
            : base(options)
        { }

        [ExcludeFromCodeCoverage]
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                DoOnModelCreating(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }

        private void DoOnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdersDetailEntity>()
                .HasOne(a => a.Order)
                .WithMany(c => c.Details)
                .HasForeignKey(c => c.OrderId);

            modelBuilder.Entity<DishesDetailEntity>()
                .HasOne(a => a.Dish)
                .WithMany(c => c.Ingredients)
                .HasForeignKey(c => c.DishId);

            modelBuilder.Entity<TablesEntity>()
                .HasOne(a => a.Room)
                .WithMany(c => c.Tables)
                .HasForeignKey(c => c.RoomId);
        }
    }
}