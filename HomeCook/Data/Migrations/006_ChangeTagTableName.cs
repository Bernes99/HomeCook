using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    //[Migration(6)]
    //public class ChangeTagTableName : Migration
    //{
    //    public override void Up()
    //    {
            
    //        if (Schema.Schema("App").Table("Tags").Exists())
    //        {
    //            Rename.Table("Tags").InSchema("App").To("KeyWords");            
    //        }
    //        if (Schema.Schema("App").Table("RecipesTags").Exists())
    //        {
    //            Rename.Table("RecipesTags").InSchema("App").To("RecipeKeyWord");

    //            Delete.Column("TagId").FromTable("RecipeKeyWord").InSchema("App");
    //            Alter.Table("RecipeKeyWord").InSchema("App").AddColumn("KeyWordId").AsInt64().ForeignKey("FK_RecipeKeyWord_KeyWordId", "App", "KeyWords", "Id");

    //            Delete.Column("RecipeId").FromTable("RecipeKeyWord").InSchema("App");
    //            Alter.Table("RecipeKeyWord").InSchema("App").AddColumn("RecipeId").AsInt64().ForeignKey("FK_RecipeKeyWord_RecipeId", "App", "Recipes", "Id");

    //        }

    //    }
    //    public override void Down()
    //    {

    //        if (Schema.Schema("App").Table("Tags").Exists())
    //        {
    //            Rename.Table("KeyWords").InSchema("App").To("Tags");
    //        }
    //        if (Schema.Schema("App").Table("RecipesTags").Exists())
    //        {
    //            Rename.Table("RecipeKeyWord").InSchema("App").To("RecipesTags");

    //            Delete.Column("KeyWordId").FromTable("RecipesTag").InSchema("App");
    //            Alter.Table("RecipesTag").InSchema("App").AddColumn("TagId").AsInt64().ForeignKey("FK_RecipesTags_TagId", "App", "Tags", "Id");

    //            Delete.Column("RecipeId").FromTable("RecipesTag").InSchema("App");
    //            Alter.Table("RecipesTag").InSchema("App").AddColumn("RecipeId").AsInt64().ForeignKey("FK_RecipesTags_RecipeId", "App", "Recipes", "Id");

    //        }
    //    }
    //}
}
