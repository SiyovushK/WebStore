using AutoMapper;
using Domain.DTOs;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        // CreateMap<Source, Destination>();

        CreateMap<RegisterDto, User>();
        CreateMap<User, GetUserDto>();

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<Product, ProductDto>();

        CreateMap<CartItemDTO, CartItem>();
        CreateMap<CartItem, GetCartItemDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
            .ForMember(dest => dest.QuantityInCart, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : new List<string>()));
    }
}
