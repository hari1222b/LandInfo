using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Models;

namespace LandInfoSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LandProperty> LandProperties { get; set; }
        public DbSet<Request> Requests { get; set; }

        // NEW TABLE FOR BUYER MESSAGES
        public DbSet<Message> Messages { get; set; }
        
        // LOAN REQUESTS
        public DbSet<LoanRequest> LoanRequests { get; set; }

        // PROPERTY-BASED LAND LOAN REQUESTS
        public DbSet<LandLoanRequest> LandLoanRequests { get; set; }

        // FRAUD ANALYSIS
        public DbSet<FraudAnalysis> FraudAnalyses { get; set; }

        public DbSet<LandDocument> LandDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure LandProperty entity
            modelBuilder.Entity<LandProperty>()
                .HasKey(lp => lp.PropertyId);
            modelBuilder.Entity<LandProperty>()
                .HasOne(lp => lp.Seller)
                .WithMany(u => u.LandProperties)
                .HasForeignKey(lp => lp.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Request entity
            modelBuilder.Entity<Request>()
                .HasKey(r => r.RequestId);
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Buyer)
                .WithMany(u => u.BuyerRequests)
                .HasForeignKey(r => r.BuyerId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Property)
                .WithMany(lp => lp.Requests)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Message entity
            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageId);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.SellerId);

            modelBuilder.Entity<Message>()
                .HasIndex(m => m.PropertyId);

            // Configure LandDocument
            modelBuilder.Entity<LandDocument>()
                .HasKey(ld => ld.Id);
            modelBuilder.Entity<LandDocument>()
                .HasOne(ld => ld.Property)
                .WithMany(lp => lp.Documents)
                .HasForeignKey(ld => ld.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes
            modelBuilder.Entity<LandProperty>()
                .HasIndex(lp => lp.SellerId);
            modelBuilder.Entity<Request>()
                .HasIndex(r => r.BuyerId);
            modelBuilder.Entity<Request>()
                .HasIndex(r => r.PropertyId);
            modelBuilder.Entity<LandDocument>()
                .HasIndex(ld => ld.PropertyId);
        }
    }
}