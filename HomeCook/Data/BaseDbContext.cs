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
        public virtual DbSet<CommentsRecipe> CommentsRecipes { get; set; } = null!;
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

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'12dc7bcb-7e6f-4a60-b2eb-caa51c16262b'::bpchar")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.AuthorId)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.DateDeletedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'6d26e836-7e58-4a70-a05c-abc1268b1746'::bpchar")
                    .IsFixedLength();
            });

            modelBuilder.Entity<CommentsRecipe>(entity =>
            {
                entity.ToTable("CommentsRecipe", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'12e71f63-f215-4070-9a5c-0c528fabec6e'::bpchar")
                    .IsFixedLength();

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.CommentsRecipes)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommentsRecipe_CommentId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.CommentsRecipes)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CommentsRecipe_RecipeId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'c1985a7d-7e3e-4090-a87e-c4a4aac04307'::bpchar")
                    .IsFixedLength();
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.ToTable("ProductCategory", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'a10ee241-3a1d-4e7e-a34c-39cd4146ff80'::bpchar")
                    .IsFixedLength();
            });

            modelBuilder.Entity<ProfileImage>(entity =>
            {
                entity.ToTable("ProfileImages", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.Path).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'6cedf967-2fde-4b2b-8c11-8c63440a8c53'::bpchar")
                    .IsFixedLength();

                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipes", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.AuthorId)
                    .HasMaxLength(36)
                    .IsFixedLength();

                entity.Property(e => e.CreatedBy)
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
                    .HasDefaultValueSql("'8e9a1d79-ead9-47ba-8da0-0a3c91824827'::bpchar")
                    .IsFixedLength();

                entity.Property(e => e.Title).HasMaxLength(65535);
            });

            modelBuilder.Entity<RecipeProduct>(entity =>
            {
                entity.ToTable("RecipeProduct", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Amount).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'3d9d7a67-64a4-448a-ab0e-eb749b40f339'::bpchar")
                    .IsFixedLength();

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.RecipeProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeProduct_ProductId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeProducts)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserProducts_RecipeId");
            });

            modelBuilder.Entity<RecipesCategory>(entity =>
            {
                entity.ToTable("RecipesCategories", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'f5e67b96-e2ab-478a-8774-d030f2d902bb'::bpchar")
                    .IsFixedLength();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.RecipesCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipesCategories_CategoryId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesCategories)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipesCategories_RecipeId");
            });

            modelBuilder.Entity<RecipesImage>(entity =>
            {
                entity.ToTable("RecipesImages", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.Path).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'02387012-67d2-456a-af61-f0dca7cdb3fa'::bpchar")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesImages)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipesImages_RecipeId");
            });

            modelBuilder.Entity<RecipesTag>(entity =>
            {
                entity.ToTable("RecipesTags", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'eb90d578-e3dc-4516-937b-685456b9137f'::bpchar")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipesTags)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
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

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'5f90806c-65ac-4e60-a3c9-a71418671083'::bpchar")
                    .IsFixedLength();
            });

            modelBuilder.Entity<UserProduct>(entity =>
            {
                entity.ToTable("UserProducts", "App");

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Amount).HasMaxLength(65535);

                entity.Property(e => e.ExpirationDate).HasColumnType("timestamp without time zone");

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("'23cd273d-e4df-49b6-ae68-548d44b1c384'::bpchar")
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
