using Learn.Services;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfServices
{
    [ServiceContract(Namespace = "http://www.jmlearn.com/")]
    public interface IGoodsOutService
    {
        /// <summary>
        /// 货物查询
        /// </summary>
        /// <param name="queryDto"></param>
        /// <returns></returns>
        [OperationContract]
        Task<PagedResult<Learn.Services.GoodsDto>> GetGoodsAsync(GoodsQueryDto queryDto);

        /// <summary>
        /// 根据Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Task<GoodsDto> GetGoodsByIdAsync(Guid id);
    }
}
