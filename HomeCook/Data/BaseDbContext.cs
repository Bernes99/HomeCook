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
        public virtual DbSet<RecipeCategory> RecipeCategories { get; set; } = null!;
        public virtual DbSet<RecipeImage> RecipeImages { get; set; } = null!;
        public virtual DbSet<RecipeProduct> RecipeProducts { get; set; } = null!;
        public virtual DbSet<RecipeTag> RecipeTags { get; set; } = null!;
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
            modelBuilder.HasPostgresExtension("btree_gin")
                .HasPostgresExtension("btree_gist")
                .HasPostgresExtension("citext")
                .HasPostgresExtension("cube")
                .HasPostgresExtension("dblink")
                .HasPostgresExtension("dict_int")
                .HasPostgresExtension("dict_xsyn")
                .HasPostgresExtension("earthdistance")
                .HasPostgresExtension("fuzzystrmatch")
                .HasPostgresExtension("hstore")
                .HasPostgresExtension("intarray")
                .HasPostgresExtension("ltree")
                .HasPostgresExtension("pg_stat_statements")
                .HasPostgresExtension("pg_trgm")
                .HasPostgresExtension("pgcrypto")
                .HasPostgresExtension("pgrowlocks")
                .HasPostgresExtension("pgstattuple")
                .HasPostgresExtension("tablefunc")
                .HasPostgresExtension("unaccent")
                .HasPostgresExtension("uuid-ossp")
                .HasPostgresExtension("xml2");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "App");

                entity.HasIndex(e => e.PublicId, "IX_Category_PublicId")
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
                entity.ToTable("Comment", "App");

                entity.HasIndex(e => e.PublicId, "IX_Comment_PublicId")
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
                    .HasConstraintName("FK_Comment_RecipeId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product", "App");

                entity.HasIndex(e => e.PublicId, "IX_Product_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.DateDeletedUtc).HasColumnType("timestamp without time zone");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(36)
                    .IsFixedLength();

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
                entity.ToTable("ProfileImage", "App");

                entity.HasIndex(e => e.PublicId, "IX_ProfileImage_PublicId")
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
                entity.ToTable("Recipe", "App");

                entity.HasIndex(e => e.PublicId, "IX_Recipe_PublicId")
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

            modelBuilder.Entity<RecipeCategory>(entity =>
            {
                entity.ToTable("RecipeCategory", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipeCategory_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.RecipeCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_RecipeCategory_CategoryId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeCategories)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipeCategory_RecipeId");
            });

            modelBuilder.Entity<RecipeImage>(entity =>
            {
                entity.ToTable("RecipeImage", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipeImage_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.Name).HasMaxLength(65535);

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeImages)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipeImage_RecipeId");
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
                    .HasConstraintName("FK_UserProduct_RecipeId");
            });

            modelBuilder.Entity<RecipeTag>(entity =>
            {
                entity.ToTable("RecipeTag", "App");

                entity.HasIndex(e => e.PublicId, "IX_RecipeTag_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                entity.Property(e => e.PublicId)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("(uuid_generate_v4())::character(36)")
                    .IsFixedLength();

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeTags)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipeTag_RecipeId");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.RecipeTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_RecipeTag_TagId");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tag", "App");

                entity.HasIndex(e => e.PublicId, "IX_Tag_PublicId")
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
                entity.ToTable("UserProduct", "App");

                entity.HasIndex(e => e.PublicId, "IX_UserProduct_PublicId")
                    .IsUnique();

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

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
                    .HasConstraintName("FK_UserProduct_ProductId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
