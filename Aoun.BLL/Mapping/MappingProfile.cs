
using Aoun.BLL.DTOs.Auth;
using AutoMapper;
using Aoun.DAL.Entities;

namespace Aoun.BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, ApplicationUser>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
    }
}




