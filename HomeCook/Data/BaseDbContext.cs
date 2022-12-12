using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using HomeCook.Data.Models; 
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeCook.Data
{
    public partial class BaseDbContext : IdentityDbContext<AppUser>
    {
        public BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProfileImage> ProfileImages { get; set; } = null!;
        public virtual DbSet<Recipe> Recipes { get; set; } = null!;
        public virtual DbSet<RecipeProduct> RecipeProducts { get; set; } = null!;
        public virtual DbSet<RecipesCategory> RecipesCategories { get; set; } = null!;
        public virtual DbSet<RecipesImage> RecipesImages { get; set; } = null!;
        public virtual DbSet<RecipesTag> RecipesTags { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<UserProduct> UserProducts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

				optionsBuilder.UseNpgsql("DefaultDatabase");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories", "App");

                entity.HasIndex(e => e.PublicId, "IX_Categories_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments", "App");

                entity.HasIndex(e => e.PublicId, "IX_Comments_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.AuthorId)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.DateCreatedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DateDeletedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_RecipeId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "App");

                entity.HasIndex(e => e.PublicId, "IX_Products_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_CategoryId");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory", "App");

                entity.HasIndex(e => e.PublicId, "IX_ProductCategory_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();
            });

            modelBuilder.Entity<ProfileImage>(entity =>
            {
                entity.ToTable("ProfileImages", "App");

                entity.HasIndex(e => e.PublicId, "IX_ProfileImages_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.Path).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipes", "App");

                entity.HasIndex(e => e.PublicId, "IX_Recipes_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.AuthorId)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.DateCreatedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DateDeletedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DateModifiedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.Portion).HasMaxLength(65535);

                entity.Property(e => e.PreparingTime).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.Property(e => e.Title).HasMaxLength(65535);
            });

            modelBuilder.Entity<RecipeProduct>(entity =>
            {
                entity.ToTable("RecipeProduct", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipeProduct_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.RecipeProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeProduct_ProductId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeProducts)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_UserProducts_RecipeId");
            });

            modelBuilder.Entity<RecipesCategory>(entity =>
            {
                entity.ToTable("RecipesCategories", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipesCategories_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.RecipesCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipesCategories_CategoryId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesCategories)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipesCategories_RecipeId");
            });

            modelBuilder.Entity<RecipesImage>(entity =>
            {
                entity.ToTable("RecipesImages", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipesImages_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesImages)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipesImages_RecipeId");
            });

            modelBuilder.Entity<RecipesTag>(entity =>
            {
                entity.ToTable("RecipesTags", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipesTags_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesTags)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipesTags_RecipeId");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.RecipesTags)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipesTags_TagId");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags", "App");

                entity.HasIndex(e => e.PublicId, "IX_Tags_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();
            });

            modelBuilder.Entity<UserProduct>(entity =>
            {
                entity.ToTable("UserProducts", "App");

                entity.HasIndex(e => e.PublicId, "IX_UserProducts_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Amount).HasMaxLength(65535);

                entity.Property(e => e.ExpirationDate).HasColumnType("timestamp without time zone");

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.UserProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProducts_ProductId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
