using accounting.src.Models;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Data
{

    /*
        Контекст для взаимодействия с базой данных
        Он легковесный и создается при каждом запросе клиента к серверу
        DbSet - таблицы
    */

    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public DbSet<User> Users { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MaterialAccounting> MaterialAccountings { get; set; }
        public DbSet<ProductAccounting> ProductAccountings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _config = configuration;
        }


        //Настройки подключения и взаимодействия с базой данных
        //Включено подробное логирование ошибок и запросов

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();

            base.OnConfiguring(optionsBuilder);
        }

        //Настройка связей таблиц (1 к 1; 1 к многим  ...)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<ConsumptionOfMaterialPerProduct>(options =>
            {
                options
                    .HasKey(c => new { c.MaterialId, c.ProductId });

                options
                    .HasOne(e => e.Product)
                    .WithMany(e => e.Materials)
                    .OnDelete(DeleteBehavior.Restrict);

                options
                    .HasOne(e => e.Material)
                    .WithMany(e => e.Products)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AmountOfAccountedMaterials>(options =>
            {
                options.HasKey(e => new { e.MaterialAccountingId, e.MaterialId });

                options
                    .HasOne(e => e.Material)
                    .WithMany(e => e.AccountedMaterials)
                    .OnDelete(DeleteBehavior.Restrict);

                options
                    .HasOne(e => e.MaterialAccounting)
                    .WithMany(e => e.AccountedMaterials)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<AmountOfAccountedProduct>(options =>
            {
                options.HasKey(e => new { e.ProductAccountingId, e.ProductId });

                options
                    .HasOne(e => e.ProductAccounting)
                    .WithMany(e => e.AccountedProducts)
                    .OnDelete(DeleteBehavior.Restrict);

                options
                    .HasOne(e => e.Product)
                    .WithMany(e => e.AccountedProducts)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
