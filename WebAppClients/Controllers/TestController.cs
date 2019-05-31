using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Learn.Clents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAppClients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly GoodsClient _goodsClient;
        public TestController(GoodsClient goodsClient)
        {
            _goodsClient = goodsClient;
        }

        [HttpPost]
        public async Task<PagedResultOfGoodsDto> GetGoodsAsync(GoodsQueryDto queryDto)
        {
            return await _goodsClient.GetGoodsAsync(queryDto);
        }
    }
}