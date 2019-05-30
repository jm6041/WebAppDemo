using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Learn.Services
{
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// <see cref="OnModelCreating(ModelBuilder)"/>
        /// </summary>
        /// <param name="modelBuilder"><see cref="ModelBuilder"/></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 表名使用类型名表示
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                Regex regex = new Regex(@"`\d+$");
                string tableName = regex.Replace(entity.ClrType.Name, string.Empty);
                modelBuilder.Entity(entity.ClrType).ToTable(tableName);

                //foreach (var property in entity.GetProperties())
                //{
                //    property.Relational().ColumnName = property.Name.ToLower();
                //}
            }

            // 限制一对多级联删除
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //foreach (var property in modelBuilder.Model.GetEntityTypes()
            //    .SelectMany(t => t.GetProperties())
            //    .Where(p => p.ClrType == typeof(decimal)))
            //{
            //    property.Relational().ColumnType = "numeric(18, 6)";
            //}

            // 字符串默认长度设置为 2000
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                if (property.GetMaxLength() == null)
                {
                    property.SetMaxLength(2000);
                }
            }

            modelBuilder.ApplyConfiguration(new GoodsConfig());
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public virtual DbSet<Goods> Goods { get; set; }
    }

    class GoodsConfig : IEntityTypeConfiguration<Goods>
    {
        /// <summary>
        /// 字数货物
        /// </summary>
        public static readonly Goods GoodsWords = new Goods()
        {
            Id = new Guid("CD0147DB-43A1-4641-9290-8FB6225DA264"),
            Name = "40万字\r\n无期限",
            Pricing = PricingWay.Words,
            Words = 400000,
            InitPrice = 5000,
            Price = 3000,
            CreateTime = new DateTimeOffset(2019, 3, 1, 12, 0, 0, 0, TimeSpan.FromHours(8)),
            StartTime = new DateTimeOffset(2019, 3, 22, 0, 0, 0, 0, TimeSpan.FromHours(8)),
            EndTime = new DateTimeOffset(2029, 12, 31, 23, 59, 59, 600, TimeSpan.FromHours(8)),
            Description = "WE WORK用户现享受优惠\r\n只需3000元！"
        };

        /// <summary>
        /// 时间货物
        /// </summary>
        public static readonly Goods GoodsTime = new Goods()
        {
            Id = new Guid("FF0DB504-A1C6-4973-A676-CED0ADB10321"),
            Name = "字数无限\r\n有效期一个月",
            Pricing = PricingWay.Time,
            Days = 31,
            InitPrice = 5000,
            Price = 3000,
            CreateTime = new DateTimeOffset(2019, 3, 1, 12, 0, 0, 0, TimeSpan.FromHours(8)),
            StartTime = new DateTimeOffset(2019, 3, 22, 0, 0, 0, 0, TimeSpan.FromHours(8)),
            EndTime = new DateTimeOffset(2029, 12, 31, 23, 59, 59, 600, TimeSpan.FromHours(8)),
            Description = "WE WORK用户现享受优惠\r\n只需3000元！"
        };

        public void Configure(EntityTypeBuilder<Goods> builder)
        {
            builder.HasData(GoodsWords, GoodsTime);
        }
    }
}
