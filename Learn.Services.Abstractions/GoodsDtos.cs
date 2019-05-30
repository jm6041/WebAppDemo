using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Learn.Services
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class GoodsQueryDto : PageParameter
    {
        /// <summary>
        /// 最低价
        /// </summary>
        public double? PriceMin { get; set; }
        /// <summary>
        /// 最高价
        /// </summary>
        public double? PriceMax { get; set; }
    }

    /// <summary>
    /// 定价方式
    /// </summary>
    public enum PricingWay
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        /// <summary>
        /// 字数
        /// </summary>
        [Description("字数计价")]
        Words = 10,
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间计价")]
        Time = 20,
    }

    /// <summary>
    /// 货物输入
    /// </summary>
    public class GoodsInDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        [Required]
        [StringLength(200)]
        [DisplayName("名字")]
        public string Name { get; set; }
        /// <summary>
        /// <see cref="PricingWay"/>
        /// </summary>
        [Required]
        [DisplayName("定价方式")]
        public PricingWay Pricing { get; set; }
        /// <summary>
        /// 字数
        /// </summary>
        [DisplayName("字数")]
        [Range(1, int.MaxValue)]
        public int? Words { get; set; }
        /// <summary>
        /// 天数，一个月当31天，可以小数
        /// </summary>
        [DisplayName("持续时间")]
        [Range(0, 5000)]
        public double? Days { get; set; }
        /// <summary>
        /// 初始价格
        /// </summary>
        [Required]
        [DisplayName("初始价格")]
        public double InitPrice { get; set; }
        /// <summary>
        /// 实际价格
        /// </summary>
        [Required]
        [DisplayName("实际价格")]
        public double Price { get; set; }
        /// <summary>
        /// 货币
        /// </summary>
        [Required]
        [DisplayName("货币")]
        public string Currency { get; set; } = "CNY";
        /// <summary>
        /// 货币
        /// </summary>
        [Required]
        [DisplayName("货币符号")]
        public string CurrencySymbol { get; set; } = "￥";
        /// <summary>
        /// 描述
        /// </summary>
        [Required]
        [DisplayName("描述")]
        public string Description { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        [DisplayName("开始时间")]
        public DateTimeOffset StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        [DisplayName("结束时间")]
        public DateTimeOffset EndTime { get; set; }
    }

    /// <summary>
    /// 货物
    /// </summary>
    public class GoodsDto : GoodsInDto
    {
        /// <summary>
        /// 是否有折扣
        /// </summary>
        [DisplayName("是否有折扣")]
        public bool HasDiscount
        {
            get
            {
                if (Price < InitPrice)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTimestamp => StartTime.ToUnixTimeMilliseconds();
        /// <summary>
        /// 结束时间
        /// </summary>
        public long EndTimestamp => EndTime.ToUnixTimeMilliseconds();
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public long CreateTimestamp => CreateTime.ToUnixTimeMilliseconds();
    }
}
