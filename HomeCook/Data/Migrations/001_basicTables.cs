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
            
            if (!Schema.Table("Products").Exists())
            {
                Create.Table("Products").InSchema("App")
                    .WithId()
                    .WithPublicId()
                    .WithColumn("Name").AsString(128)
                    .WithColumn("Calories").AsInt64().Nullable()
                    .WithColumn("Category").AsInt32().Nullable()
                    .WithColumn("UnitType").AsInt32().Nullable();
            }
            if (!Schema.Table("Recipes").Exists())
            {
                Create.Table("Recipes").InSchema("App")
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
            if (!Schema.Table("Comments").Exists())
            {
                Create.Table("Comments").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("AuthorId").AsFixedLengthAnsiString(36)
                   .WithSoftDelete()
                   .WithColumn("Text").AsInt64Text().Nullable();
            }
            if (!Schema.Table("CommentsRecipe").Exists())
            {
                Create.Table("CommentsRecipe").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_CommentsRecipe_RecipeId", "App", "Recipes", "Id")
                   .WithColumn("CommentId").AsInt64()
                        .ForeignKey("FK_CommentsRecipe_CommentId", "App", "Comments", "Id");
            }

            if (!Schema.Table("Categories").Exists())
            {
                Create.Table("Categories").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("Name").AsText();
            }
            if (!Schema.Table("RecipesCategories").Exists())
            {
                Create.Table("RecipesCategories").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("CategoryId").AsInt64()
                        .ForeignKey("FK_RecipesCategories_CategoryId", "App", "Categories", "Id")
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipesCategories_RecipeId", "App", "Recipes", "Id");
            }

            if (!Schema.Table("Tags").Exists())
            {
                Create.Table("Tags").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("Name").AsText();
            }
            if (!Schema.Table("RecipesTags").Exists())
            {
                Create.Table("RecipesTags").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("TagId").AsInt64()
                        .ForeignKey("FK_RecipesTags_TagId", "App", "Tags", "Id")
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipesTags_RecipeId", "App", "Recipes", "Id");
            }

            if (!Schema.Table("RecipesImages").Exists())
            {
                Create.Table("RecipesImages").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_RecipesImages_RecipeId", "App", "Recipes", "Id")
                   .WithColumn("Name").AsText().Nullable()
                   .WithColumn("Value").AsInt64Text();
            }

            if (!Schema.Table("ProfileImages").Exists())
            {
                Create.Table("ProfileImages").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("UserId").AsFixedLengthAnsiString(36)
                   .WithColumn("Name").AsText().Nullable()
                   .WithColumn("Value").AsInt64Text();
            }

            if (!Schema.Table("UserProducts").Exists())
            {
                Create.Table("UserProducts").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("UserId").AsFixedLengthAnsiString(36)
                    .WithColumn("ProductId").AsInt64()
                        .ForeignKey("FK_UserProducts_ProductId", "App", "Products", "Id")
                   .WithColumn("ExpirationDate").AsDateTime2().Nullable()
                   .WithColumn("Amount").AsText().Nullable()
                   .WithColumn("IsOnShoppingList").AsBoolean();
            }

            if (!Schema.Table("RecipeProduct").Exists())
            {
                Create.Table("RecipeProduct").InSchema("App")
                   .WithId()
                   .WithPublicId()
                   .WithColumn("RecipeId").AsInt64()
                        .ForeignKey("FK_UserProducts_RecipeId", "App", "Recipes", "Id")
                    .WithColumn("ProductId").AsInt64()
                        .ForeignKey("FK_RecipeProduct_ProductId", "App", "Products", "Id")
                   .WithColumn("Amount").AsText().Nullable();
            }
        }

        public override void Down()
        {
            // zachowac dobą kolejnosc usuwania tak aby nie prowowac usuwac tabel z istniejaca zaleznoscia do innej

            if (!Schema.Table("Products").Exists())
            {
                Delete.Table("Products").InSchema("App");
            }
            if (!Schema.Table("Recipes").Exists())
            {
                Delete.Table("Recipes").InSchema("App");
            }
            if (!Schema.Table("Comments").Exists())
            {
                Delete.Table("Comments").InSchema("App");   
            }
            if (!Schema.Table("CommentsRecipe").Exists())
            {
                Delete.Table("CommentsRecipe").InSchema("App");
            }
            if (!Schema.Table("Categories").Exists())
            {
                Delete.Table("Categories").InSchema("App");
            }
            if (!Schema.Table("RecipesCategories").Exists())
            {
                Delete.Table("RecipesCategories").InSchema("App");
            }
            if (!Schema.Table("Tags").Exists())
            {
                Delete.Table("Tags").InSchema("App");
            }
            if (!Schema.Table("RecipesTags").Exists())
            {
                Delete.Table("RecipesTags").InSchema("App");
            }
            if (!Schema.Table("RecipesImages").Exists())
            {
                Delete.Table("RecipesImages").InSchema("App");
            }
            if (!Schema.Table("ProfileImages").Exists())
            {
                Delete.Table("ProfileImages").InSchema("App");
            }
            if (!Schema.Table("UserProducts").Exists())
            {
                Delete.Table("UserProducts").InSchema("App");
            }
            if (!Schema.Table("RecipeProduct").Exists())
            {
                Delete.Table("RecipeProduct").InSchema("App");
            }


        }
    }
}
