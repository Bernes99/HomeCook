using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(2)]
    public class UpdateImageTables : Migration
    {
        public override void Up()
        {
            if (Schema.Schema("App").Table("ProfileImages").Exists())
            {
                Alter.Table("ProfileImages").InSchema("App").AddColumn("Path").AsText().Nullable();
                //Alter.Table("ProfileImages").InSchema("App").AlterColumn("Value").AsBinary().Nullable();
            }
            if (Schema.Schema("App").Table("RecipesImages").Exists())
            {
                Alter.Table("RecipesImages").InSchema("App").AddColumn("Path").AsText().Nullable();
                //Alter.Table("RecipesImages").InSchema("App").AlterColumn("Value").AsBinary().Nullable();
            }
        }
        public override void Down()
        {
            if (Schema.Schema("App").Table("ProfileImages").Exists())
            {
                Delete.Column("Path").FromTable("ProfileImages").InSchema("App");
                //Alter.Table("ProfileImages").InSchema("App").AlterColumn("Value").AsInt64Text().Nullable();
            }
            if (Schema.Schema("App").Table("RecipesImages").Exists())
            {
                Delete.Column("Path").FromTable("RecipesImages").InSchema("App");
                //Alter.Table("RecipesImages").InSchema("App").AlterColumn("Value").AsInt64Text().Nullable();
            }
        }
    }
}
