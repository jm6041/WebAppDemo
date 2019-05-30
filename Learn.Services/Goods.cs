using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Learn.Services
{
    /// <summary>
    /// 货物
    /// </summary>
    public class Goods
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        [MaxLength(200)]
        public string Name { get; set; }
        /// <summary>
        /// <see cref="PricingWay"/>
        /// </summary>
        public PricingWay Pricing { get; set; }
        /// <summary>
        /// 字数
        /// </summary>
        public int? Words { get; set; }
        /// <summary>
        /// 天数，一个月当31天，可以小数
        /// </summary>
        public double? Days { get; set; }
        /// <summary>
        /// 初始价格
        /// </summary>
        public double InitPrice { get; set; }
        /// <summary>
        /// 实际价格
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 货币符号
        /// </summary>
        [MaxLength(10)]
        public string CurrencySymbol { get; set; } = "￥";
        /// <summary>
        /// 货币
        /// </summary>
        [MaxLength(3)]
        public string Currency { get; set; } = "CNY";
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTimeOffset StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTimeOffset EndTime { get; set; }
    }
}
