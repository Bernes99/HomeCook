﻿using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Infrastructure;

namespace HomeCook.Data.Migrations
{
    public static class Extensions
    {
        #region AsText()
        public static T AsText<T>(this IColumnTypeSyntax<T> builder) where T : IFluentSyntax
        {
            return builder.AsString(65535);
        }
        #endregion

        #region AsMediumText()
        public static T AsMediumText<T>(this IColumnTypeSyntax<T> builder) where T : IFluentSyntax
        {
            return builder.AsString(int.MaxValue / 2);
        }
        #endregion

        #region AsLongText()
        public static T AsInt64Text<T>(this IColumnTypeSyntax<T> builder) where T : IFluentSyntax
        {
            return builder.AsString(int.MaxValue);
        }
        #endregion

        #region WithId()
        public static ICreateTableWithColumnSyntax WithId(this ICreateTableWithColumnSyntax builder)
        {
            return builder
                .WithColumn("Id").AsInt64().PrimaryKey().Identity();
        }
        #endregion

        #region WithPublicId()
        public static ICreateTableWithColumnSyntax WithPublicId(this ICreateTableWithColumnSyntax builder)
        {
            return builder
                .WithColumn("PublicId").AsFixedLengthAnsiString(36).Unique().WithDefaultValue(RawSql.Insert("uuid_generate_v4()::character(36)"));//asGuid().WithDefault(SystemMethods.NewGuid);
        }
        #endregion

        #region WithAuditable()
        public static ICreateTableWithColumnSyntax WithAuditable(this ICreateTableWithColumnSyntax builder)
        {
            return builder
                .WithColumn("DateCreatedUtc").AsDateTime2()
                .WithColumn("CreatedBy").AsFixedLengthAnsiString(36)
                .WithColumn("DateModifiedUtc").AsDateTime2().Nullable()
                .WithColumn("ModifiedBy").AsFixedLengthAnsiString(36).Nullable();
        }
        #endregion

        #region WithSoftDeletes()
        public static ICreateTableWithColumnSyntax WithSoftDelete(this ICreateTableWithColumnSyntax builder)
        {
            return builder
                .WithColumn("DateDeletedUtc").AsDateTime2().Nullable()
                .WithColumn("DeletedBy").AsFixedLengthAnsiString(36).Nullable();
        }
        #endregion

        #region WithVersionable()
        public static ICreateTableWithColumnSyntax WithVersionable(this ICreateTableWithColumnSyntax builder)
        {
            return builder
                .WithColumn("RevisionOrigin").AsInt64()
                .WithColumn("RevisionPrevious").AsInt64().Nullable()
                .WithColumn("IsLastRevision").AsBoolean();
        }
        #endregion
    }
}
