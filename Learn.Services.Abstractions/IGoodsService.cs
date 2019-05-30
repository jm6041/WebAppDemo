using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Services
{
    public interface IGoodsService
    {
        /// <summary>
        /// 获取当前获取
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<GoodsDto>> GetGoodsAsync(GoodsQueryDto queryDto);
        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<GoodsDto> GetGoodsByIdAsync(Guid id);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        Task<DataResultDto<Guid?>> AddAsync(GoodsInDto d);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        Task<ResultDto> UpdateAsync(GoodsInDto d);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> RemoveByIdAsync(Guid id);
    }
}
