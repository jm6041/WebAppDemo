using AutoMapper;
using Learn.Services;
using System;

namespace Learn.Mappers
{
    internal class GoodsProfile : Profile
    {
        public GoodsProfile()
        {
            CreateMap<Goods, GoodsDto>();
            CreateMap<GoodsInDto, Goods>();
        }
    }
}
