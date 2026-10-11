namespace TurboHub.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarEngine> CarEngines { get; set; }
        public DbSet<ProductCompatibility> ProductCompatibilities { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SpecialRequest> SpecialRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft delete global filters
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => !c.IsDeleted);

            modelBuilder.Entity<Order>()
                .HasQueryFilter(o => !o.IsDeleted);

            modelBuilder.Entity<ProductCompatibility>()
    .HasQueryFilter(pc => !pc.Product.IsDeleted);

            modelBuilder.Entity<ProductCompatibility>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.Compatibilities)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCompatibility>()
                .HasOne(pc => pc.CarBrand)
                .WithMany()
                .HasForeignKey(pc => pc.CarBrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCompatibility>()
                .HasOne(pc => pc.CarModel)
                .WithMany()
                .HasForeignKey(pc => pc.CarModelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCompatibility>()
                .HasOne(pc => pc.CarEngine)
                .WithMany(e => e.ProductCompatibilities)
                .HasForeignKey(pc => pc.CarEngineId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasQueryFilter(oi => !oi.Order.IsDeleted);

            modelBuilder.Entity<ProductImage>()
                .HasQueryFilter(pi => !pi.Product.IsDeleted);

            // Product → Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → ApplicationUser (optional)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // OrderItem → Order
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → Product
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // SpecialRequest → ApplicationUser (optional)
            modelBuilder.Entity<SpecialRequest>()
                .HasOne(sr => sr.User)
                .WithMany(u => u.SpecialRequests)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Category self-referencing
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.SubTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.VatAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasPrecision(18, 2);
        }
    }
}