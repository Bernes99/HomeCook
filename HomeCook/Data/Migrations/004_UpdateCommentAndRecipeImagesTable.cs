using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(4)]
    public class UpdateCommentAndRecipeImagesTable : Migration
    {
        public override void Up()
        {
            if (Schema.Schema("App").Table("CommentRecipe").Exists())
            {
                Delete.Table("CommentRecipe").InSchema("App");
            }

            if (Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Delete.Column("Path").FromTable("RecipeImage").InSchema("App");
                Alter.Table("RecipeImage").InSchema("App").AddColumn("MainPicture").AsBoolean();
            }
            if (Schema.Schema("App").Table("Comment").Exists())
            {
                Alter.Table("Comment").InSchema("App")
                    .AddColumn("RecipeId")
                    .AsInt64()
                    .NotNullable()
                    .ForeignKey("FK_Comment_RecipeId", "App", "Recipe", "Id")
                    .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .AddColumn("DateCreatedUtc")
                    .AsDateTime2()
                    .NotNullable();
            }
        }
        public override void Down()
        {
            if (!Schema.Schema("App").Table("CommentRecipe").Exists())
            {
                Create.Table("CommentRecipe").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_CommentRecipe_RecipeId", "App", "Recipe", "Id")
                   .WithColumn("CommentId").AsInt64()
                        .ForeignKey("FK_CommentRecipe_CommentId", "App", "Comment", "Id");
            }
            if (Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Delete.Column("MainPicture").FromTable("RecipeImage").InSchema("App");
                Alter.Table("RecipeImage").InSchema("App").AddColumn("Path").AsText();
            }
            if (Schema.Schema("App").Table("Comment").Exists())
            {
                Delete.Column("RecipeId").FromTable("Comment").InSchema("App");
                Delete.Column("DateCreatedUtc").FromTable("Comment").InSchema("App");
            }
        }
    }
}
