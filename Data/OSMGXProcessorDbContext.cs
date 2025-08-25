using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Scryfall.API.Models;
using TCGProcessor.Models.Cache;

namespace TCGProcessor.Data
{
    public class OSMGXProcessorDbContext : DbContext
    {
        public OSMGXProcessorDbContext()
        {
        }

        public OSMGXProcessorDbContext(DbContextOptions<OSMGXProcessorDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CachedScryfallCard> ScryfallCache { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CachedScryfallCard>(entity =>
            {
                // Configure primary key (Guid)
                entity.HasKey(e => e.Id);

                // Configure CardData to be stored as JSON string
                entity.Property(e => e.CardData)
                    .HasConversion(
                        v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                        v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Card>(v, (System.Text.Json.JsonSerializerOptions)null))
                    .HasColumnType("JSON"); // MySQL JSON column type

                // Configure CachedAt property
                entity.Property(e => e.CachedAt)
                    .IsRequired()
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("TIMESTAMP");

                // Configure IsFound property
                entity.Property(e => e.IsFound)
                    .IsRequired()
                    .HasDefaultValue(false);

                // Configure Error property (nullable string)
                entity.Property(e => e.Error)
                    .HasMaxLength(1000); // Adjust length as needed

                // Add index on CachedAt for cache expiration queries
                entity.HasIndex(e => e.CachedAt);

                // Add index on IsFound for filtering queries
                entity.HasIndex(e => e.IsFound);
            });
        }
    }
}