using AutoMapper;
using FlatAbandonedCheckout;
using ShopifySharp;

namespace WebApplication1
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Checkout, AbandonedCheckout>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => ((DateTimeOffset)(src.CreatedAt)).ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()))
                .ForMember(dest => dest.AbandonedCheckoutId, opt => opt.MapFrom(src =>src.Id))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => ((DateTimeOffset)(src.UpdatedAt)).ToUnixTimeMilliseconds()));
        }
    }
}
