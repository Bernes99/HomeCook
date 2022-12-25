using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(4)]
    public class UpdateCommentAndRecipeImagesTable : Migration
    {
        public override void Up()
        {
            if (Schema.Schema("App").Table("CommentsRecipe").Exists())
            {
                Delete.Table("CommentsRecipe").InSchema("App");
            }

            if (Schema.Schema("App").Table("RecipesImages").Exists())
            {
                Delete.Column("Path").FromTable("RecipesImages").InSchema("App");
                Alter.Table("RecipesImages").InSchema("App").AddColumn("MainPicture").AsBoolean();
            }
            if (Schema.Schema("App").Table("Comments").Exists())
            {
                Alter.Table("Comments").InSchema("App")
                    .AddColumn("RecipeId")
                    .AsInt64()
                    .NotNullable()
                    .ForeignKey("FK_Comments_RecipeId", "App", "Recipes", "Id")
                    .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .AddColumn("DateCreatedUtc")
                    .AsDateTime2()
                    .NotNullable();
            }
        }
        public override void Down()
        {
            if (!Schema.Schema("App").Table("CommentsRecipe").Exists())
            {
                Create.Table("CommentsRecipe").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_CommentsRecipe_RecipeId", "App", "Recipes", "Id")
                   .WithColumn("CommentId").AsInt64()
                        .ForeignKey("FK_CommentsRecipe_CommentId", "App", "Comments", "Id");
            }
            if (Schema.Schema("App").Table("RecipesImages").Exists())
            {
                Delete.Column("MainPicture").FromTable("RecipesImages").InSchema("App");
                Alter.Table("RecipesImages").InSchema("App").AddColumn("Path").AsText();
            }
            if (Schema.Schema("App").Table("Comments").Exists())
            {
                Delete.Column("RecipeId").FromTable("Comments").InSchema("App");
                Delete.Column("DateCreatedUtc").FromTable("Comments").InSchema("App");
            }
        }
    }
}
