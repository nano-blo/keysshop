using Microsoft.EntityFrameworkCore;
using KeysShop.Models;

namespace KeysShop.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Существующие DbSet
        public DbSet<Game> Games { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GameGenre> GameGenres { get; set; }

        // Новые DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<BuyHistory> BuyHistory { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // У тебя дважды повторяется эта строка, убери дубликат
            modelBuilder.Entity<GameGenre>()
                .HasKey(gg => new { gg.id_игры, gg.id_жанра });

            // Добавь уникальность для email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Настройка для избранного (уникальная пара пользователь-игра)
            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.GameId })
                .IsUnique();

            // Настройка для истории покупок
            modelBuilder.Entity<BuyHistory>()
                .HasIndex(bh => bh.UserId);

            modelBuilder.Entity<BuyHistory>()
                .HasIndex(bh => bh.PurchaseTime);

            // Настройка ролей по умолчанию
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("User");
        }
    }
}
