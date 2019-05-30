using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppDemo.Migrations
{
    public partial class MyFirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Goods",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Pricing = table.Column<int>(nullable: false),
                    Words = table.Column<int>(nullable: true),
                    Days = table.Column<double>(nullable: true),
                    InitPrice = table.Column<double>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    CurrencySymbol = table.Column<string>(maxLength: 10, nullable: true),
                    Currency = table.Column<string>(maxLength: 3, nullable: true),
                    Description = table.Column<string>(maxLength: 2000, nullable: true),
                    CreateTime = table.Column<DateTimeOffset>(nullable: false),
                    StartTime = table.Column<DateTimeOffset>(nullable: false),
                    EndTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goods", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Goods",
                columns: new[] { "Id", "CreateTime", "Currency", "CurrencySymbol", "Days", "Description", "EndTime", "InitPrice", "Name", "Price", "Pricing", "StartTime", "Words" },
                values: new object[] { new Guid("cd0147db-43a1-4641-9290-8fb6225da264"), new DateTimeOffset(new DateTime(2019, 3, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "CNY", "￥", null, @"WE WORK用户现享受优惠
只需3000元！", new DateTimeOffset(new DateTime(2029, 12, 31, 23, 59, 59, 600, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), 5000.0, @"40万字
无期限", 3000.0, 10, new DateTimeOffset(new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), 400000 });

            migrationBuilder.InsertData(
                table: "Goods",
                columns: new[] { "Id", "CreateTime", "Currency", "CurrencySymbol", "Days", "Description", "EndTime", "InitPrice", "Name", "Price", "Pricing", "StartTime", "Words" },
                values: new object[] { new Guid("ff0db504-a1c6-4973-a676-ced0adb10321"), new DateTimeOffset(new DateTime(2019, 3, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), "CNY", "￥", 31.0, @"WE WORK用户现享受优惠
只需3000元！", new DateTimeOffset(new DateTime(2029, 12, 31, 23, 59, 59, 600, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), 5000.0, @"字数无限
有效期一个月", 3000.0, 20, new DateTimeOffset(new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)), null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goods");
        }
    }
}
