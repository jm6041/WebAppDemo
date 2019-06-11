using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Learn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAppDemo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private readonly IGoodsService _goodsService;
        public GoodsController(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }

        /// <summary>
        /// 货物查询
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<PagedResult<GoodsDto>> GetGoodsAsync([FromBody]GoodsQueryDto queryDto)
        {
            return await _goodsService.GetGoodsAsync(queryDto);
        }

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<GoodsDto> GetGoodsByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            return await _goodsService.GetGoodsByIdAsync(id);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public async Task<ResultDto> UpdateAsync([FromBody]GoodsInDto d)
        {
            return await _goodsService.UpdateAsync(d);
        }
    }
}