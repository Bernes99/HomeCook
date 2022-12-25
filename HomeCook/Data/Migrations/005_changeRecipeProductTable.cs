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
            if (Schema.Schema("App").Table("Recipe").Exists())
            {
                Delete.Column("CreatedBy").FromTable("Recipe").InSchema("App");
            }
            
        }
        public override void Down()
        {

            if (Schema.Schema("App").Table("RecipeProduct").Exists())
            {
                Delete.Column("Amount").FromTable("RecipeProduct").InSchema("App");
                Alter.Table("RecipeProduct").InSchema("App").AddColumn("Amount").AsText();
            }
            if (Schema.Schema("App").Table("Recipe").Exists())
            {
                Alter.Table("Recipe").InSchema("App").AddColumn("CreatedBy").AsFixedLengthAnsiString(36);
            }
        }
    }
}
