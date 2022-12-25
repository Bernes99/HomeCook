using FluentMigrator;
using System.Data;

namespace HomeCook.Data.Migrations
{
    [Migration(1)]
    public class basicTables : Migration
    {
        public override void Up()
        {
            //Create.Table("Users")
            //    .WithColumn("Id").AsInt64().PrimaryKey().Identity()  
            //    .WithColumn("PublicId").AsFixedLengthAnsiString(36)  
            //    .WithColumn("DateCreatedUtc").AsDateTime2().WithColumn("CreatedBy").AsInt64().WithColumn("DateModifiedUtc").AsDateTime2().Nullable().WithColumn("ModifiedBy").AsInt64().Nullable() 
            //    .WithColumn("DateDeletedUtc").AsDateTime2().Nullable().WithColumn("DeletedBy").AsInt64().Nullable() 
            //    .WithColumn("FirstName").AsString(150)
            //    .WithColumn("Surname").AsString(150)
            //    .WithColumn("Login").AsString(250)
            //    .WithColumn("PasswordHash").AsString(250).Nullable()
            //    .WithColumn("IsAdmin").AsBoolean();
            
            if (!Schema.Schema("App").Table("Product").Exists())
            {
                Create.Table("Product").InSchema("App")
                    .WithId()
                    .WithPublicId()
                    .WithColumn("Name").AsString(128)
                    .WithColumn("Calories").AsInt64().Nullable()
                    .WithColumn("Category").AsInt32().Nullable()
                    .WithColumn("UnitType").AsInt32().Nullable();
            }
            if (!Schema.Schema("App").Table("Recipe").Exists())
            {
                Create.Table("Recipe").InSchema("App")
                    .WithId()
                    .WithPublicId()
                    .WithAuditable()
                    .WithSoftDelete()
                    .WithColumn("Title").AsText()
                    .WithColumn("Introdution").AsInt64Text().Nullable()
                    .WithColumn("Text").AsInt64Text().Nullable()
                    .WithColumn("Rating").AsFloat().Nullable()
                    .WithColumn("Portion").AsText()
                    .WithColumn("AuthorId").AsFixedLengthAnsiString(36)
                    .WithColumn("PreparingTime").AsText()
                    .WithColumn("Difficulty").AsFloat();

                    //.WithColumn("Name").AsString(128)
                    //.WithColumn("LastLogin").AsDateTime2().Nullable()
                    //.WithColumn("Login").AsString(64).Unique()
                    //.WithColumn("Password").AsString(256)
                    //.WithColumn("FirstName").AsString(32)
                    //.WithColumn("LastName").AsString(32)
                    //.WithColumn("Email").AsString(64).Unique()
                    //.WithColumn("Photo").AsString(512).Nullable()
                    //.WithColumn("RefreshToken").AsString(256).Nullable()
                    //.WithColumn("RefreshTokenExpiryTime").AsDateTime2().Nullable()
                    //.WithColumn("RoleId").AsInt64()
                    //    .ForeignKey("FK_User_RoleId", "Role", "Id")
                    //    .OnDeleteOrUpdate(Rule.Cascade);
            }
            if (!Schema.Schema("App").Table("Comment").Exists())
            {
                Create.Table("Comment").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("AuthorId").AsFixedLengthAnsiString(36)
                   .WithSoftDelete()
                   .WithColumn("Text").AsInt64Text().Nullable();
            }
            if (!Schema.Schema("App").Table("CommentRecipe").Exists())
            {
                Create.Table("CommentRecipe").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_CommentRecipe_RecipeId", "App", "Recipe", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                   .WithColumn("CommentId").AsInt64()
                        .ForeignKey("FK_CommentRecipe_CommentId", "App", "Comment", "Id");
            }

            if (!Schema.Schema("App").Table("Category").Exists())
            {
                Create.Table("Category").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("Name").AsText();
            }
            if (!Schema.Schema("App").Table("RecipeCategory").Exists())
            {
                Create.Table("RecipeCategory").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("CategoryId").AsInt64()
                        .ForeignKey("FK_RecipeCategory_CategoryId", "App", "Category", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipeCategory_RecipeId", "App", "Recipe", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade);
            }

            if (!Schema.Schema("App").Table("Tag").Exists())
            {
                Create.Table("Tag").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("Name").AsText();
            }
            if (!Schema.Schema("App").Table("RecipeTag").Exists())
            {
                Create.Table("RecipeTag").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("TagId").AsInt64()
                        .ForeignKey("FK_RecipeTag_TagId", "App", "Tag", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipeTag_RecipeId", "App", "Recipe", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade);
            }

            if (!Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Create.Table("RecipeImage").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipeImage_RecipeId", "App", "Recipe", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                   .WithColumn("Name").AsText().Nullable()
                   .WithColumn("Value").AsBinary().Nullable();
            }

            if (!Schema.Schema("App").Table("ProfileImage").Exists())
            {
                Create.Table("ProfileImage").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("UserId").AsFixedLengthAnsiString(36)
                        .ForeignKey("FK_ProfileImage_UserId", "public", "AspNetUsers", "Id")
                   .WithColumn("Name").AsText().Nullable()
                   .WithColumn("Value").AsBinary().Nullable();
            }

            if (!Schema.Schema("App").Table("UserProduct").Exists())
            {
                Create.Table("UserProduct").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("UserId").AsFixedLengthAnsiString(36)
                        .ForeignKey("FK_UserProduct_UserId", "public", "AspNetUsers", "Id")
                    .WithColumn("ProductId").AsInt64()
                        .ForeignKey("FK_UserProduct_ProductId", "App", "Product", "Id")
                   .WithColumn("ExpirationDate").AsDateTime2().Nullable()
                   .WithColumn("Amount").AsFloat().Nullable()
                   .WithColumn("IsOnShoppingList").AsBoolean();
            }

            if (!Schema.Schema("App").Table("RecipeProduct").Exists())
            {
                Create.Table("RecipeProduct").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_UserProduct_RecipeId", "App", "Recipe", "Id")
                        .OnDeleteOrUpdate(System.Data.Rule.Cascade)
                    .WithColumn("ProductId").AsInt64()
                        .ForeignKey("FK_RecipeProduct_ProductId", "App", "Product", "Id")
                   .WithColumn("Amount").AsText().Nullable();
            }
        }

        public override void Down()
        {
            // zachowac dobą kolejnosc usuwania tak aby nie prowowac usuwac tabel z istniejaca zaleznoscia do innej

            if (Schema.Schema("App").Table("Product").Exists())
            {
                Delete.Table("Product").InSchema("App");
            }
            if (Schema.Schema("App").Table("Recipe").Exists())
            {
                Delete.Table("Recipe").InSchema("App");
            }
            if (Schema.Schema("App").Table("Comment").Exists())
            {
                Delete.Table("Comment").InSchema("App");   
            }
            if (Schema.Schema("App").Table("CommentRecipe").Exists())
            {
                Delete.Table("CommentRecipe").InSchema("App");
            }
            if (Schema.Schema("App").Table("Category").Exists())
            {
                Delete.Table("Category").InSchema("App");
            }
            if (Schema.Schema("App").Table("RecipeCategory").Exists())
            {
                Delete.Table("RecipeCategory").InSchema("App");
            }
            if (Schema.Schema("App").Table("Tag").Exists())
            {
                Delete.Table("Tag").InSchema("App");
            }
            if (Schema.Schema("App").Table("RecipeTag").Exists())
            {
                Delete.Table("RecipeTag").InSchema("App");
            }
            if (Schema.Schema("App").Table("RecipeImage").Exists())
            {
                Delete.Table("RecipeImage").InSchema("App");
            }
            if (Schema.Schema("App").Table("ProfileImage").Exists())
            {
                Delete.Table("ProfileImage").InSchema("App");
            }
            if (Schema.Schema("App").Table("UserProduct").Exists())
            {
                Delete.Table("UserProduct").InSchema("App");
            }
            if (Schema.Schema("App").Table("RecipeProduct").Exists())
            {
                Delete.Table("RecipeProduct").InSchema("App");
            }


        }
    }
}
