using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(3)]
    public class AddProductCategoryTable : Migration
    {
        public override void Up()
        {
            if (!Schema.Schema("App").Table("ProductCategory").Exists())
            {
                Create.Table("ProductCategory").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("Name").AsText();
            }
            if (Schema.Schema("App").Table("Products").Exists())
            {
                Rename.Column("Category").OnTable("Products").InSchema("App").To("CategoryId");
                Alter.Table("Products").InSchema("App").AlterColumn("CategoryId")
                    .AsInt64()
                    .NotNullable()
                    .ForeignKey("FK_Product_CategoryId", "App", "ProductCategory", "Id");
            }
        }
        public override void Down()
        {
            if (!Schema.Schema("App").Table("ProductCategory").Exists())
            {
                Delete.Table("ProductCategory").InSchema("App");
            }
            if (Schema.Schema("App").Table("Product").Exists())
            {
                Rename.Column("CategoryId").OnTable("Product").To("Category");
                Alter.Table("Product").InSchema("App").AlterColumn("Category")
                    .AsInt32()
                    .Nullable();
            }
        }
    }
}
