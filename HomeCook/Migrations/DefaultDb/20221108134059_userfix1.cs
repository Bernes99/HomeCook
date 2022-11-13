using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeCook.Migrations.DefaultDb
{
    public partial class userfix1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "suername",
                table: "AspNetUsers",
                newName: "surname");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "surname",
                table: "AspNetUsers",
                newName: "suername");

        }
    }
}
