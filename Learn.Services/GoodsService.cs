using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Learn.Services
{
    public class GoodsService : IGoodsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GoodsService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<GoodsDto>> GetGoodsAsync(GoodsQueryDto queryDto)
        {
            return await Query(null, queryDto).ToPagedResultAsync(queryDto);
        }

        public async Task<GoodsDto> GetGoodsByIdAsync(Guid id)
        {
            return await Query(id, null).FirstOrDefaultAsync();
        }

        private IQueryable<GoodsDto> Query(Guid? id, GoodsQueryDto queryDto)
        {
            IQueryable<Goods> sourceDatas = _context.Goods;
            if (id != null)
            {
                Guid idv = id.Value;
                sourceDatas = sourceDatas.Where(x => x.Id == idv);
            }
            else if (queryDto != null)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                sourceDatas = sourceDatas.Where(x => x.EndTime > now && x.StartTime <= now);
                if (queryDto.PriceMax != null)
                {
                    sourceDatas = sourceDatas.Where(x => x.Price <= queryDto.PriceMax.Value);
                }
                if (queryDto.PriceMin != null)
                {
                    sourceDatas = sourceDatas.Where(x => x.Price >= queryDto.PriceMin.Value);
                }
                if (queryDto.OrderingsIsNullOrEmpty())
                {
                    sourceDatas = sourceDatas.OrderByDescending(x => x.CreateTime);
                }
            }
            var datas = from d in sourceDatas
                        select new GoodsDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Pricing = d.Pricing,
                            Words = d.Words,
                            Days = d.Days,
                            InitPrice = d.InitPrice,
                            Price = d.Price,
                            CurrencySymbol = d.CurrencySymbol,
                            Currency = d.Currency,
                            Description = d.Description,
                            CreateTime = d.CreateTime,
                            StartTime = d.StartTime,
                            EndTime = d.EndTime,
                        };
            Expression<Func<GoodsDto, string>> exp = x => x.Name;
            datas = datas.OrderBy<GoodsDto, string>(exp);
            // datas = datas.OrderBy(x => x.Name).ThenBy(x => x.Price);
            //var datas = _mapper.ProjectTo<GoodsDto>(sourceDatas);
            return datas;
        }
        private ResultDto ValidInput(GoodsInDto d)
        {
            ResultDto r = new ResultDto();
            if (d == null)
            {
                r.AddError("Input", "输入数据为空");
                return r;
            }
            if (d.Pricing == PricingWay.Time)
            {
                if (d.Days == null)
                {
                    r.AddError(nameof(d.Days), "不能为空");
                    return r;
                }
                if (d.Days.Value <= 0)
                {
                    r.AddError(nameof(d.Days), "必须大于0");
                    return r;
                }
            }
            if (d.Pricing == PricingWay.Words && d.Words == null)
            {
                r.AddError(nameof(d.Words), "非法数字");
                return r;
            }
            return r;
        }

        public async Task<DataResultDto<Guid?>> AddAsync(GoodsInDto d)
        {
            DataResultDto<Guid?> rd = new DataResultDto<Guid?>();
            ResultDto vr = ValidInput(d);
            if (!vr.IsSuccess)
            {
                rd.Merge(vr);
                return rd;
            }
            Goods m = new Goods
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTimeOffset.UtcNow,
            };
            _mapper.Map(m, d);
            await _context.Goods.AddAsync(m);
            await _context.SaveChangesAsync();
            rd.Data = m.Id;
            return rd;
        }

        public async Task<ResultDto> UpdateAsync(GoodsInDto d)
        {
            ResultDto r = ValidInput(d);
            if (!r.IsSuccess)
            {
                return r;
            }
            if (d.Id == null)
            {
                r.AddError(nameof(d.Id), "输入Id为空");
                return r;
            }

            Guid id = d.Id.Value;
            Goods m = await _context.Goods.FindAsync(id);
            if (m == null)
            {
                r.AddError("Goods", "没有对应的数据");
                return r;
            }
            _mapper.Map(d, m);
            await _context.SaveChangesAsync();
            return r;
        }

        public async Task<int> RemoveByIdAsync(Guid id)
        {
            var m = await _context.Goods.FindAsync(id);
            if (m != null)
            {
                _context.Goods.Remove(m);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
