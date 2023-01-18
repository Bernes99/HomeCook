using FluentMigrator;

namespace HomeCook.Data.Migrations
{
    [Migration(6)]
    public class ChangeTagTableName : Migration
    {
        public override void Up()
        {

            if (Schema.Schema("App").Table("Product").Exists())
            {
                Alter.Table("Product").InSchema("App").AddColumn("DateDeletedUtc").AsDateTime2().Nullable();
                Alter.Table("Product").InSchema("App").AddColumn("DeletedBy").AsFixedLengthAnsiString(36).Nullable();
            }

        }
        public override void Down()
        {

            if (Schema.Schema("App").Table("Product").Exists())
            {
                Delete.Column("DateDeletedUtc").FromTable("Product").InSchema("App");
                Delete.Column("DeletedBy").FromTable("Product").InSchema("App");
            }

        }
    }
}
