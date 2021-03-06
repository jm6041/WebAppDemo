﻿// <auto-generated />
using System;
using Learn.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WebAppDemo.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190530063300_MyFirstMigration")]
    partial class MyFirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0-preview5.19227.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Learn.Services.Goods", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreateTime");

                    b.Property<string>("Currency")
                        .HasMaxLength(3);

                    b.Property<string>("CurrencySymbol")
                        .HasMaxLength(10);

                    b.Property<double?>("Days");

                    b.Property<string>("Description")
                        .HasMaxLength(2000);

                    b.Property<DateTimeOffset>("EndTime");

                    b.Property<double>("InitPrice");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<double>("Price");

                    b.Property<int>("Pricing");

                    b.Property<DateTimeOffset>("StartTime");

                    b.Property<int?>("Words");

                    b.HasKey("Id");

                    b.ToTable("Goods");

                    b.HasData(
                        new
                        {
                            Id = new Guid("cd0147db-43a1-4641-9290-8fb6225da264"),
                            CreateTime = new DateTimeOffset(new DateTime(2019, 3, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)),
                            Currency = "CNY",
                            CurrencySymbol = "￥",
                            Description = @"WE WORK用户现享受优惠
只需3000元！",
                            EndTime = new DateTimeOffset(new DateTime(2029, 12, 31, 23, 59, 59, 600, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)),
                            InitPrice = 5000.0,
                            Name = @"40万字
无期限",
                            Price = 3000.0,
                            Pricing = 10,
                            StartTime = new DateTimeOffset(new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)),
                            Words = 400000
                        },
                        new
                        {
                            Id = new Guid("ff0db504-a1c6-4973-a676-ced0adb10321"),
                            CreateTime = new DateTimeOffset(new DateTime(2019, 3, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)),
                            Currency = "CNY",
                            CurrencySymbol = "￥",
                            Days = 31.0,
                            Description = @"WE WORK用户现享受优惠
只需3000元！",
                            EndTime = new DateTimeOffset(new DateTime(2029, 12, 31, 23, 59, 59, 600, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)),
                            InitPrice = 5000.0,
                            Name = @"字数无限
有效期一个月",
                            Price = 3000.0,
                            Pricing = 20,
                            StartTime = new DateTimeOffset(new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0))
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
