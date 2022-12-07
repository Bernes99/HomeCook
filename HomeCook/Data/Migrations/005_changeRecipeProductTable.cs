using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(5)]
    public class ChangeRecipeProductTable: Migration
    {
        public override void Up()
        {

            if (Schema.Schema("App").Table("RecipeProduct").Exists())
            {
                Delete.Column("Amount").FromTable("RecipeProduct").InSchema("App");
                Alter.Table("RecipeProduct").InSchema("App").AddColumn("Amount").AsFloat();

            }
            if (Schema.Schema("App").Table("Recipes").Exists())
            {
                Delete.Column("CreatedBy").FromTable("Recipes").InSchema("App");
            }
            
        }
        public override void Down()
        {

            if (Schema.Schema("App").Table("RecipeProduct").Exists())
            {
                Delete.Column("Amount").FromTable("RecipeProduct").InSchema("App");
                Alter.Table("RecipeProduct").InSchema("App").AddColumn("Amount").AsText();
            }
            if (Schema.Schema("App").Table("Recipes").Exists())
            {
                Alter.Table("Recipes").InSchema("App").AddColumn("CreatedBy").AsFixedLengthAnsiString(36);
            }
        }
    }
}
