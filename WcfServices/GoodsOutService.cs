using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Learn.Services;

namespace WcfServices
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = true, InstanceContextMode = InstanceContextMode.PerSession)]
    public class GoodsOutService : IGoodsOutService
    {
        private readonly IGoodsService _goodsService;
        public GoodsOutService(IGoodsService goodsService)
        {
            _goodsService = goodsService;
        }

        public async Task<PagedResult<GoodsDto>> GetGoodsAsync(GoodsQueryDto queryDto)
        {
            return await _goodsService.GetGoodsAsync(queryDto);
        }

        public async Task<GoodsDto> GetGoodsByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return null;
            }
            return await _goodsService.GetGoodsByIdAsync(id);
        }
    }
}
