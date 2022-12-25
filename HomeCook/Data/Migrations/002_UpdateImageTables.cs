using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(2)]
    public class UpdateImageTables : Migration
    {
        public override void Up()
        {
            if (Schema.Schema("App").Table("ProfileImage").Exists())
            {
                Alter.Table("ProfileImage").InSchema("App").AddColumn("Path").AsText().Nullable();
            }
            if (Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Alter.Table("RecipeImage").InSchema("App").AddColumn("Path").AsText().Nullable();
            }
        }
        public override void Down()
        {
            if (Schema.Schema("App").Table("ProfileImage").Exists())
            {
                Delete.Column("Path").FromTable("ProfileImage").InSchema("App");
            }
            if (Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Delete.Column("Path").FromTable("RecipeImage").InSchema("App");
            }
        }
    }
}
